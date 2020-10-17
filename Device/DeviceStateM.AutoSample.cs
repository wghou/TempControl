using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stateless;
using System.Timers;
using System.IO;
using NLog;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Device
{
    namespace AutoSample
    {
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
            /// 自动取样 - 准备中
            /// </summary>
			Prepare,
            /// <summary>
            /// 就绪
            /// </summary>
            Ready,
            /// <summary>
            /// 冲洗
            /// </summary>
            OnFlush,
            /// <summary>
            /// 自动取样 - 取样中
            /// </summary>
			OnSample,
            /// <summary>
            /// 强制停止
            /// </summary>
            Stop
        }

        /// <summary>
        /// 自动采样触发事件
        /// </summary>
		public enum TriggerSample
        {
            /// <summary>
            /// 第一次按键：进入 Prepare_1
            /// </summary>
			ClickFist = 0,
            /// <summary>
            /// 5分钟后，从 Prepare 进入 Ready
            /// </summary>
            ClickFist_ready = 1,
            /// <summary>
            /// 第二次按键：进入 OnSample
            /// </summary>
            ClickSecond = 2,
            /// <summary>
            /// 冲洗完成
            /// </summary>
            FlushEnd,
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


        /// <summary>
        /// 自动采样参数
        /// </summary>
        public class SampleParam
        {
            private static readonly Logger nlogger = LogManager.GetCurrentClassLogger();

            /// <summary>
            /// 定时器时间间隔
            /// </summary>
            public int timer_interval = 1;

            /// <summary>
            /// 继电器 1 通电时长 - 5 分钟
            /// </summary>
            public int tim_prepare = 5 * 60;

            /// <summary>
            /// 取样时长 - 1 分钟
            /// </summary>
            public int tim_onsample = 1 * 60;

            /// <summary>
            /// 从配置文件读取参数值
            /// </summary>
            /// <param name="configFilePath"></param>
            /// <returns></returns>
            public bool ReadValueConfig(string configFilePath)
            {

                try
                {
                    // 如果配置文件不存在，则新建
                    if (!File.Exists(configFilePath))
                    {
                        WriteValueConfig(configFilePath);
                    }

                    ////////////////////////////////////////
                    // 参数设置
                    timer_interval = int.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "SampleParam", "timer_interval", timer_interval.ToString()));
                    tim_prepare = int.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "SampleParam", "tim_1", tim_prepare.ToString()));
                    tim_onsample = int.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "SampleParam", "tim_onsample", tim_onsample.ToString()));
                }
                catch (Exception ex)
                {
                    nlogger.Error("从配置文件读取自动采样参数过程中发生异常：" + ex.Message.ToString());
                    return false;
                }

                return true;
            }

            /// <summary>
            /// 写入自动采样参数
            /// </summary>
            /// <param name="configFilePath"></param>
            /// <returns></returns>
            public bool WriteValueConfig(string configFilePath)
            {
                try
                {
                    // 相关参数
                    Utils.IniReadWrite.INIWriteValue(configFilePath, "SampleParam", "timer_interval", timer_interval.ToString());
                    Utils.IniReadWrite.INIWriteValue(configFilePath, "SampleParam", "tim_1", tim_prepare.ToString());
                    Utils.IniReadWrite.INIWriteValue(configFilePath, "SampleParam", "tim_onsample", tim_onsample.ToString());
                }
                catch (Exception ex)
                {
                    nlogger.Error("从配置文件写入自动采样参数过程中发生异常：" + ex.Message.ToString());
                    return false;
                }

                return true;
            }
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


        protected class AutoSamplePipeline
        {
            /// <summary>
            /// 清空当前的自动采样流程
            /// </summary>
            public void reInit()
            {
                prepareStatus.Clear(); prepareCount = 0;
                flushStatus.Clear(); flushCount = 0;
                sampleStatus.Clear(); sampleCount = 0;
            }

            /// <summary>
            /// 准备状态下的继电器值
            /// </summary>
            public List<Tuple<uint, bool, bool, bool, bool>> prepareStatus = new List<Tuple<uint, bool, bool, bool, bool>>();
            public int prepareCount = 0;

            /// <summary>
            /// 冲洗阶段继电器值
            /// </summary>
            public List<Tuple<uint, bool, bool, bool, bool>> flushStatus = new List<Tuple<uint, bool, bool, bool, bool>>();
            public int flushCount = 0;

            /// <summary>
            /// 采样阶段继电器值
            /// </summary>
            public List<Tuple<uint, bool, bool, bool, bool>> sampleStatus = new List<Tuple<uint, bool, bool, bool, bool>>();
            public int sampleCount = 0;
        }

        /// <summary>
        /// 采样阶段的指令流程
        /// </summary>
        protected AutoSamplePipeline pipeline = new AutoSamplePipeline();


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
                .Permit(AutoSample.TriggerSample.ClickFist, AutoSample.StateSample.Prepare)
                .Permit(AutoSample.TriggerSample.ForceStop, AutoSample.StateSample.Stop);


            // StateSample.Prepare
            _sampleMachine.Configure(AutoSample.StateSample.Prepare)
                .OnEntry(t => samplePrepareEntry())
                .OnExit(t => samplePrepareExit())
                .InternalTransition(_sampleTickTrigger, (tic, t) => samplePrepareTick(tic))
                .Permit(AutoSample.TriggerSample.ClickFist_ready, AutoSample.StateSample.Ready)
                .Permit(AutoSample.TriggerSample.ForceStop, AutoSample.StateSample.Stop);

            // StateSample.Ready
            _sampleMachine.Configure(AutoSample.StateSample.Ready)
                .OnEntry(t => sampleReadyEntry())
                .OnExit(t => sampleReadyExit())
                .InternalTransition(_sampleTickTrigger, (tic, t) => sampleReadyTick(tic))
                .Permit(AutoSample.TriggerSample.ClickSecond, AutoSample.StateSample.OnFlush)
                .Permit(AutoSample.TriggerSample.ForceStop, AutoSample.StateSample.Stop);

            // StateSample.OnFlush
            _sampleMachine.Configure(AutoSample.StateSample.OnFlush)
                .OnEntry(t => sampleFlushEntry())
                .OnExit(t => sampleFlushExit())
                .InternalTransition(_sampleTickTrigger, (tic, t) => sampleFlushTick(tic))
                .Permit(AutoSample.TriggerSample.FlushEnd, AutoSample.StateSample.OnSample)
                .Permit(AutoSample.TriggerSample.ForceStop, AutoSample.StateSample.Stop);

            // StateSample.OnSample
            _sampleMachine.Configure(AutoSample.StateSample.OnSample)
                .OnEntry(t => sampleStartEntry())
                .OnExit(t => sampleStartExit())
                .InternalTransition(_sampleTickTrigger, (tic, t) => sampleStartTick(tic))
                .Permit(AutoSample.TriggerSample.End, AutoSample.StateSample.Normal)
                .Permit(AutoSample.TriggerSample.ForceStop, AutoSample.StateSample.Stop);

            _sampleMachine.Configure(AutoSample.StateSample.Stop)
                .OnEntry(t => stopEntry())
                .OnExit(t => stoptExit())
                .InternalTransition(_sampleTickTrigger, (tic, t) => stopTick(tic))
                .Permit(AutoSample.TriggerSample.ForceStop, AutoSample.StateSample.Normal);

            // 设置定时器
            _tickTimerSample = new Timer();
            _tickTimerSample.Interval = _runningParameters.sampleParam.timer_interval * 1000; // 默认 5s
            _tickTimerSample.AutoReset = true;
            _tickTimerSample.Elapsed += _tickTimerSample_Elapsed;
        }

        private void _tickTimerSample_Elapsed(object sender, ElapsedEventArgs e)
        {
            // 计数
            sampleStateCounts++;

            _sampleMachine.Fire(_sampleTickTrigger, _runningParameters.sampleParam.timer_interval * 1000);
        }


        /// <summary>
        /// 从配置文件中，读取采样配置时间
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool ReadSampleConfig(JObject obj)
        {
            // 清空现有流程
            pipeline.reInit();

            try
            {
                JArray jPrepare = (JArray)obj["Prepare"];
                foreach (JObject itm in jPrepare)
                {
                    bool[] ry = itm["Relay"].ToObject<bool[]>();
                    pipeline.prepareStatus.Add(new Tuple<uint, bool, bool, bool, bool>((uint)itm["Time"], ry[0], ry[1], ry[2], ry[3]));
                }

                JArray jFlush = (JArray)obj["OnFlush"];
                foreach (JObject itm in jFlush)
                {
                    bool[] ry = itm["Relay"].ToObject<bool[]>();
                    pipeline.flushStatus.Add(new Tuple<uint, bool, bool, bool, bool>((uint)itm["Time"], ry[0], ry[1], ry[2], ry[3]));
                }

                JArray jSample = (JArray)obj["OnSample"];
                foreach (JObject itm in jSample)
                {
                    bool[] ry = itm["Relay"].ToObject<bool[]>();
                    pipeline.sampleStatus.Add(new Tuple<uint, bool, bool, bool, bool>((uint)itm["Time"], ry[0], ry[1], ry[2], ry[3]));
                }
            }
            catch (Exception ex)
            {
                nlogger.Error(ex.Message);
                return false;
            }
            return true;
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
                case AutoSample.StateSample.Ready:
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
            nlogger.Debug("On State Transitioned. " + act.Destination.ToString());

            AutoSample.TriggerSample trigger = act.Trigger;
            AutoSample.StateSample source = act.Source;
            AutoSample.StateSample dest = act.Destination;

            // 状态计数器 清零
            sampleStateCounts = 0;

            SampleStateChangedEvent?.Invoke(dest);
        }

        /// <summary>
        /// 状态转变事件函数
        /// </summary>
        /// <param name="act"></param>
        private void sampleOnUnhandledTrigger(AutoSample.StateSample st, AutoSample.TriggerSample tg)
        {
            nlogger.Error("Unhandled trigger: state. " + st.ToString());

            SetErrorStatus(ErrorCode.CodeError);
        }

        void WriteRelayDeviceS2()
        {
            RelayDevice.Err_r err = ryDeviceS.UpdateStatusToDevice();

            // 记录错误状态
            // 仅记录，在 timerTickEvent 中检查全局错误状态
            if (err != RelayDevice.Err_r.NoError)
            {
                SetErrorStatus(ErrorCode.RelayError);
            }

            RelayDeviceSStatusUpdatedEvent?.Invoke(RelayDevice.Err_r.NoError, ryDeviceS.ryStatus);

            if (err != RelayDevice.Err_r.NoError) _sampleMachine.Fire(AutoSample.TriggerSample.ForceStop);
        }

        /// <summary>
        /// Normal Entry
        /// </summary>
        private void sampleNormalEntry()
        {
            nlogger.Debug("Sample Idle Entry.");

            // 1-2-3-4 全部关闭
            ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_4] = false;
            ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_5] = false;
            ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_6] = false;
            ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_7] = false;

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
        /// Prepare Entry
        /// </summary>
        private void samplePrepareEntry()
        {
            nlogger.Debug("Sample Prepare Entry.");

            if (pipeline.prepareStatus.Count <= 0)
            {
                SetErrorStatus(ErrorCode.CodeError);
                nlogger.Error("stageStatus_1.Count <=0");
                _sampleMachine.Fire(AutoSample.TriggerSample.ForceStop);
            }

            pipeline.prepareCount = 0;
            var itm = pipeline.prepareStatus[pipeline.prepareCount];
            sampleStateCounts = 0;

            ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_4] = itm.Item2;
            ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_5] = itm.Item3;
            ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_6] = itm.Item4;
            ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_7] = itm.Item5;

            WriteRelayDeviceS2();
        }

        /// <summary>
        /// Prepare Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void samplePrepareTick(int tic)
        {
            nlogger.Debug("Sample Prepare Tick: " + tic.ToString() + " ms");

            var itm = pipeline.prepareStatus[pipeline.prepareCount];

            if (sampleStateCounts > itm.Item1)
            {
                if (++pipeline.prepareCount >= pipeline.prepareStatus.Count)
                {
                    // 下一状态
                    _sampleMachine.Fire(AutoSample.TriggerSample.ClickFist_ready);
                }
                else
                {
                    // 下一组继电器指令
                    var itm2 = pipeline.prepareStatus[pipeline.prepareCount];
                    sampleStateCounts = 0;

                    ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_4] = itm2.Item2;
                    ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_5] = itm2.Item3;
                    ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_6] = itm2.Item4;
                    ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_7] = itm2.Item5;

                    WriteRelayDeviceS2();
                }
            }

        }

        /// <summary>
        /// Prepare Exit
        /// </summary>
        private void samplePrepareExit()
        {
            nlogger.Debug("Sample Prepare Exit.");
        }

        /// <summary>
        /// Ready Entry
        /// </summary>
        private void sampleReadyEntry()
        {
            nlogger.Debug("Sample Ready Entry.");
        }

        /// <summary>
        /// Ready Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void sampleReadyTick(int tic)
        {
            nlogger.Debug("Sample Ready Tick: " + tic.ToString() + " ms");
        }

        /// <summary>
        /// Ready Exit
        /// </summary>
        private void sampleReadyExit()
        {
            nlogger.Debug("Sample Ready Exit.");
        }

        /// <summary>
        /// OnFlush Entry
        /// </summary>
        private void sampleFlushEntry()
        {
            nlogger.Debug("Sample Flush Entry.");

            if (pipeline.flushStatus.Count <= 0)
            {
                SetErrorStatus(ErrorCode.CodeError);
                nlogger.Error("stageStatus_1.Count <=0");
                _sampleMachine.Fire(AutoSample.TriggerSample.ForceStop);
            }

            pipeline.flushCount = 0;
            var itm = pipeline.flushStatus[pipeline.flushCount];
            sampleStateCounts = 0;

            ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_4] = itm.Item2;
            ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_5] = itm.Item3;
            ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_6] = itm.Item4;
            ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_7] = itm.Item5;

            WriteRelayDeviceS2();
        }

        /// <summary>
        /// OnFlush Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void sampleFlushTick(int tic)
        {
            nlogger.Debug("Sample Flush Tick: " + tic.ToString() + " ms");

            var itm = pipeline.flushStatus[pipeline.flushCount];

            if (sampleStateCounts > itm.Item1)
            {
                if (++pipeline.flushCount >= pipeline.flushStatus.Count)
                {
                    // 下一状态
                    _sampleMachine.Fire(AutoSample.TriggerSample.End);
                }
                else
                {
                    // 下一组继电器指令
                    var itm2 = pipeline.flushStatus[pipeline.flushCount];
                    sampleStateCounts = 0;

                    ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_4] = itm2.Item2;
                    ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_5] = itm2.Item3;
                    ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_6] = itm2.Item4;
                    ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_7] = itm2.Item5;

                    WriteRelayDeviceS2();
                }
            }
        }

        /// <summary>
        /// OnFlush Exit
        /// </summary>
        private void sampleFlushExit()
        {
            nlogger.Debug("Sample Flush Exit.");
        }

        /// <summary>
        /// OnSample Entry
        /// </summary>
        private void sampleStartEntry()
        {
            nlogger.Debug("Sample Start Entry.");

            if (pipeline.sampleStatus.Count <= 0)
            {
                SetErrorStatus(ErrorCode.CodeError);
                nlogger.Error("stageStatus_1.Count <=0");
                _sampleMachine.Fire(AutoSample.TriggerSample.ForceStop);
            }

            pipeline.sampleCount = 0;
            var itm = pipeline.sampleStatus[pipeline.sampleCount];
            sampleStateCounts = 0;

            ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_4] = itm.Item2;
            ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_5] = itm.Item3;
            ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_6] = itm.Item4;
            ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_7] = itm.Item5;

            WriteRelayDeviceS2();
        }

        /// <summary>
        /// OnSample Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void sampleStartTick(int tic)
        {
            nlogger.Debug("Sample Start Tick: " + tic.ToString() + " ms");

            var itm = pipeline.sampleStatus[pipeline.sampleCount];

            if (sampleStateCounts > itm.Item1)
            {
                if (++pipeline.sampleCount >= pipeline.sampleStatus.Count)
                {
                    // 下一状态
                    _sampleMachine.Fire(AutoSample.TriggerSample.End);
                }
                else
                {
                    // 下一组继电器指令
                    var itm2 = pipeline.sampleStatus[pipeline.sampleCount];
                    sampleStateCounts = 0;

                    ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_4] = itm2.Item2;
                    ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_5] = itm2.Item3;
                    ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_6] = itm2.Item4;
                    ryDeviceS.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_7] = itm2.Item5;

                    WriteRelayDeviceS2();
                }
            }
        }

        /// <summary>
        /// OnSample Exit
        /// </summary>
        private void sampleStartExit()
        {
            nlogger.Debug("Sample Start Exit.");
        }

        /// <summary>
        /// stop Entry
        /// </summary>
        private void stopEntry()
        {
            nlogger.Debug("Sample Stop Entry.");
        }

        /// <summary>
        /// stop Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void stopTick(int tic)
        {
            nlogger.Debug("Sample stop Tick: " + tic.ToString() + " ms");

            _sampleMachine.Fire(AutoSample.TriggerSample.ForceStop);
        }

        /// <summary>
        /// OnSample Exit
        /// </summary>
        private void stoptExit()
        {
            nlogger.Debug("Sample stop Exit.");
        }
    }
}