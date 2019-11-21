using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stateless;
using System.Timers;

namespace Device
{
    namespace AutoSample {
		/// <summary>
        /// 自动采样的状态
        /// </summary>
		public enum StateSample : int
        {
            /// <summary>
            /// 常态，所有按键都关闭
            /// </summary>
			Normal = 0,
            /// <summary>
            /// 自动取样 - 准备中 - 第一阶段：开电磁阀 1，5分钟后关闭电磁阀 1
            /// </summary>
			Prepare_1,
            /// <summary>
            /// 自动取样准备中 - 第二阶段：开电磁阀 4-3-2
            /// </summary>
            Prepare_2,
            /// <summary>
            /// 自动取样 - 取样中：关 3-4，开 1-2，一分钟（可调）后关闭，并回到常态
            /// </summary>
			OnSample
        }

		public enum TriggerSample
        {
            /// <summary>
            /// 第一次按键：进入 Prepare_1
            /// </summary>
			ClickFist = 0,
            /// <summary>
            /// 5分钟后，从 Prepare_1 进入 Prepare_2
            /// </summary>
            ClickFist_5m = 1,
            /// <summary>
            /// 第二次按键：进入 OnSample
            /// </summary>
            ClickSecond = 2,
            /// <summary>
            /// 一分钟（可调）取样之后，回到常态 Normal
            /// </summary>
			End = 3,
            /// <summary>
            /// 时钟事件
            /// </summary>
			TimerTick,
            /// <summary>
            /// 强制停止当前自动采样流程
            /// </summary>
            ForceStop
        }

        public class SampleParam
        {
            /// <summary>
            /// 定时器时间间隔
            /// </summary>
            public int timer_interval = 5;

            /// <summary>
            /// 继电器 1 通电时长 - 5 分钟
            /// </summary>
            public int tim_1 = 5 * 60;

            /// <summary>
            /// 取样准备时长
            /// </summary>
            public int tim_prepare = 1 * 60;

            /// <summary>
            /// 取样时长 - 1 分钟
            /// </summary>
            public int tim_onsample = 1 * 60;
        }
    }

    public partial class DeviceStateM
    {
        StateMachine<AutoSample.StateSample, AutoSample.TriggerSample> _sampleMachine;
        public AutoSample.StateSample _sampleState = AutoSample.StateSample.Normal;

        /// <summary>
        /// 时刻 - 带参数 ms
        /// </summary>
        private StateMachine<AutoSample.StateSample, AutoSample.TriggerSample>.TriggerWithParameters<int> _sampleTickTrigger;

        /// <summary>
        /// 时钟
        /// </summary>
        private Timer _tickTimerSample;

        /// <summary>
        /// 自动取样 - 状态计数器
        /// </summary>
        uint sampleStateCounts = 0;

        /// <summary>
        /// 关于自动取样的一些参数
        /// </summary>
        AutoSample.SampleParam sampleParam = new AutoSample.SampleParam();


		private void ConfigStatelessSample()
        {
            _sampleMachine = new StateMachine<AutoSample.StateSample, AutoSample.TriggerSample>(() => _sampleState, s => _sampleState = s);
            _sampleTickTrigger = _sampleMachine.SetTriggerParameters<int>(AutoSample.TriggerSample.TimerTick);

            // on transition action
            _sampleMachine.OnTransitioned(sampleOnTransitionedAction);

            // on unhandled trigger
            _sampleMachine.OnUnhandledTrigger(sampleOnUnhandledTrigger);

            // StateSample.Normal
            _sampleMachine.Configure(AutoSample.StateSample.Normal)
                .OnEntry(t => sampleNormalEntry())
                .OnExit(t => sampleNormalExit())
                .InternalTransition(_sampleTickTrigger, (tic, t) => sampleNormalTick(tic))
                .Permit(AutoSample.TriggerSample.ClickFist, AutoSample.StateSample.Prepare_1);


            // StateSample.Prepare_1
            _sampleMachine.Configure(AutoSample.StateSample.Prepare_1)
                .OnEntry(t => samplePrepare_1Entry())
                .OnExit(t => samplePrepare_1Exit())
                .InternalTransition(_sampleTickTrigger, (tic, t) => samplePrepare_1Tick(tic))
                .Permit(AutoSample.TriggerSample.ClickFist_5m, AutoSample.StateSample.Prepare_2)
                .Permit(AutoSample.TriggerSample.ForceStop, AutoSample.StateSample.Normal);

            // StateSample.Prepare_2
            _sampleMachine.Configure(AutoSample.StateSample.Prepare_2)
                .OnEntry(t => samplePrepare_2Entry())
                .OnExit(t => samplePrepare_2Exit())
                .InternalTransition(_sampleTickTrigger, (tic, t) => samplePrepare_2Tick(tic))
                .Permit(AutoSample.TriggerSample.ClickSecond, AutoSample.StateSample.OnSample)
                .Permit(AutoSample.TriggerSample.ForceStop, AutoSample.StateSample.Normal);


            // StateSample.OnSample
            _sampleMachine.Configure(AutoSample.StateSample.OnSample)
                .OnEntry(t => sampleStartEntry())
                .OnExit(t => sampleStartExit())
                .InternalTransition(_sampleTickTrigger, (tic, t) => sampleStartTick(tic))
                .Permit(AutoSample.TriggerSample.End, AutoSample.StateSample.Normal)
                .Permit(AutoSample.TriggerSample.ForceStop, AutoSample.StateSample.Normal);

            // 设置定时器
            _tickTimerSample = new Timer();
            _tickTimerSample.Interval = sampleParam.timer_interval * 1000; // 默认 5s
            _tickTimerSample.AutoReset = true;
            _tickTimerSample.Elapsed += _tickTimerSample_Elapsed;
        }

        private void _tickTimerSample_Elapsed(object sender, ElapsedEventArgs e)
        {
            // 计数
            sampleStateCounts++;

            _sampleMachine.Fire(_sampleTickTrigger, sampleParam.timer_interval * 1000);
        }


        /// <summary>
        /// 自动采样，按键
        /// </summary>
        public void SampleButtonClick()
        {
            switch (_sampleState)
            {
                case AutoSample.StateSample.Normal:
                    // 当处于常态时，按一次按键，则进入下一状态
                    _sampleMachine.Fire(AutoSample.TriggerSample.ClickFist);
                    break;
                case AutoSample.StateSample.Prepare_2:
                    // 当处于 准备二时，则进入采样
                    _sampleMachine.Fire(AutoSample.TriggerSample.ClickSecond);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 强制终止采样流程
        /// </summary>
        public void SampleForceStop()
        {
            if (_sampleMachine.IsInState(AutoSample.StateSample.Normal)) return;

            _sampleMachine.Fire(AutoSample.TriggerSample.ForceStop);
        }


        /// <summary>
        /// 状态转变事件函数
        /// </summary>
        /// <param name="act"></param>
        private void sampleOnTransitionedAction(StateMachine<AutoSample.StateSample, AutoSample.TriggerSample>.Transition act)
        {
            nlogger.Debug("On State Transitioned.");

            AutoSample.TriggerSample trigger = act.Trigger;
            AutoSample.StateSample source = act.Source;
            AutoSample.StateSample dest = act.Destination;

            // 状态计数器 清零
            sampleStateCounts = 0;

            // wghou
            SampleStateChangedEvent?.Invoke(dest);
        }

        /// <summary>
        /// 状态转变事件函数
        /// </summary>
        /// <param name="act"></param>
        private void sampleOnUnhandledTrigger(AutoSample.StateSample st, AutoSample.TriggerSample tg)
        {
            nlogger.Error("Unhandled trigger: state.");

            SetErrorStatus(ErrorCode.CodeError);
        }

        /// <summary>
        /// Normal Entry
        /// </summary>
        private void sampleNormalEntry()
        {
            nlogger.Debug("Sample Idle Entry.");

            // 1-2-3-4 全部关闭
            ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_0] = false;
            ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_1] = false;
            ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_2] = false;
            ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_3] = false;

            WriteRelayDeviceS(true);
        }

        /// <summary>
        /// Normal Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void sampleNormalTick(int tic)
        {
            nlogger.Debug("Sample Idle Tick: " + tic.ToString() + " ms");
        }

        /// <summary>
        /// Normal Exit
        /// </summary>
        private void sampleNormalExit()
        {
            nlogger.Debug("Sample Idle Exit.");
        }

        /// <summary>
        /// Prepare_1 Entry
        /// </summary>
        private void samplePrepare_1Entry()
        {
            nlogger.Debug("Sample Prepare_1 Entry.");

            // 开 1 ，并关闭 2-3-4
            ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_0] = true;
            ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_1] = false;
            ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_2] = false;
            ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_3] = false;

            WriteRelayDeviceS(true);
        }

        /// <summary>
        /// Prepare_1 Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void samplePrepare_1Tick(int tic)
        {
            nlogger.Debug("Sample Prepare_1 Tick: " + tic.ToString() + " ms");

            // 5 分钟后，进入 SampleState.Prepare_2
            if (sampleStateCounts > sampleParam.tim_1 / sampleParam.timer_interval) _sampleMachine.Fire(AutoSample.TriggerSample.ClickFist_5m);
        }

        /// <summary>
        /// Prepare_1 Exit
        /// </summary>
        private void samplePrepare_1Exit()
        {
            nlogger.Debug("Sample Prepare_1 Exit.");
        }

        /// <summary>
        /// Prepare_2 Entry
        /// </summary>
        private void samplePrepare_2Entry()
        {
            nlogger.Debug("Sample Prepare_2 Entry.");

            // 5分钟后，关闭 1，开 4-3-2
            ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_0] = false;
            ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_1] = true;
            ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_2] = true;
            ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_3] = true;

            WriteRelayDeviceS(true);
        }

        /// <summary>
        /// Prepare_2 Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void samplePrepare_2Tick(int tic)
        {
            nlogger.Debug("Sample Prepare_2 Tick: " + tic.ToString() + " ms");
        }

        /// <summary>
        /// Prepare_2 Exit
        /// </summary>
        private void samplePrepare_2Exit()
        {
            nlogger.Debug("Sample Prepare_2 Exit.");
        }

        /// <summary>
        /// OnSample Entry
        /// </summary>
        private void sampleStartEntry()
        {
            nlogger.Debug("Sample Start Entry.");

            // 关闭 3-4 ，打开 1-2 ，一分钟后结束
            ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_0] = true;
            ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_1] = true;
            ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_2] = false;
            ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_3] = false;

            WriteRelayDeviceS(true);
        }

        /// <summary>
        /// OnSample Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void sampleStartTick(int tic)
        {
            nlogger.Debug("Sample Start Tick: " + tic.ToString() + " ms");

            // 1 分钟后，结束采样
            if (sampleStateCounts > sampleParam.tim_onsample / sampleParam.timer_interval) {
                _sampleMachine.Fire(AutoSample.TriggerSample.End);
            }
        }

        /// <summary>
        /// OnSample Exit
        /// </summary>
        private void sampleStartExit()
        {
            nlogger.Debug("Sample Start Exit.");
        }
    }
}
