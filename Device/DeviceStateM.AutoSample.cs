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
			Normal = 0,
			FirstStep,
			SecondStep
        }

		public enum TriggerSample
        {
			Prepare = 0,
			Start = 1,
			End = 3,
			TimerTick
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

        uint sampleStateCounts = 0;


		private void ConfigStatelessSample()
        {
            _sampleMachine = new StateMachine<AutoSample.StateSample, AutoSample.TriggerSample>(() => _sampleState, s => _sampleState = s);
            _sampleTickTrigger = _sampleMachine.SetTriggerParameters<int>(AutoSample.TriggerSample.TimerTick);

            // on transition action
            _sampleMachine.OnTransitioned(sampleOnTransitionedAction);

            // on unhandled trigger
            _sampleMachine.OnUnhandledTrigger(sampleOnUnhandledTrigger);

            // State.Idle
            // -> State.Start
            // -> State.Stop
            _sampleMachine.Configure(AutoSample.StateSample.Normal)
                .OnEntry(t => sampleIdleEntry())
                .OnExit(t => sampleIdleExit())
                .InternalTransition(_sampleTickTrigger, (tic, t) => sampleIdleTick(tic))
                .Permit(AutoSample.TriggerSample.Prepare, AutoSample.StateSample.FirstStep);


            // State.Start
            _sampleMachine.Configure(AutoSample.StateSample.FirstStep)
                .OnEntry(t => samplePrepareEntry())
                .OnExit(t => samplePrepareExit())
                .InternalTransition(_sampleTickTrigger, (tic, t) => samplePrepareTick(tic))
                .Permit(AutoSample.TriggerSample.Start, AutoSample.StateSample.SecondStep);


            // state TempUp
            _sampleMachine.Configure(AutoSample.StateSample.SecondStep)
                .OnEntry(t => sampleStartEntry())
                .OnExit(t => sampleStartExit())
                .InternalTransition(_sampleTickTrigger, (tic, t) => sampleStartTick(tic))
                .Permit(AutoSample.TriggerSample.End, AutoSample.StateSample.Normal);

            // 设置定时器
            _tickTimerSample = new Timer();
            _tickTimerSample.Interval = 5000; // 默认 5s
            _tickTimerSample.AutoReset = true;
            _tickTimerSample.Elapsed += _tickTimerSample_Elapsed;
        }

        private void _tickTimerSample_Elapsed(object sender, ElapsedEventArgs e)
        {
            // 计数
            sampleStateCounts++;

            _sampleMachine.Fire(_sampleTickTrigger, 5000);
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
        /// Idle Entry
        /// </summary>
        private void sampleIdleEntry()
        {
            nlogger.Debug("Sample Idle Entry.");
        }

        /// <summary>
        /// Idle Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void sampleIdleTick(int tic)
        {
            nlogger.Debug("Sample Idle Tick: " + tic.ToString() + " ms");
        }

        /// <summary>
        /// Idle Exit
        /// </summary>
        private void sampleIdleExit()
        {
            nlogger.Debug("Sample Idle Exit.");
        }

        /// <summary>
        /// Idle Entry
        /// </summary>
        private void samplePrepareEntry()
        {
            nlogger.Debug("Sample Prepare Entry.");
        }

        /// <summary>
        /// Idle Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void samplePrepareTick(int tic)
        {
            nlogger.Debug("Sample Prepare Tick: " + tic.ToString() + " ms");
        }

        /// <summary>
        /// Idle Exit
        /// </summary>
        private void samplePrepareExit()
        {
            nlogger.Debug("Sample Prepare Exit.");
        }

        /// <summary>
        /// Idle Entry
        /// </summary>
        private void sampleStartEntry()
        {
            nlogger.Debug("Sample Start Entry.");
        }

        /// <summary>
        /// Idle Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void sampleStartTick(int tic)
        {
            nlogger.Debug("Sample Start Tick: " + tic.ToString() + " ms");
        }

        /// <summary>
        /// Idle Exit
        /// </summary>
        private void sampleStartExit()
        {
            nlogger.Debug("Sample Start Exit.");
        }
    }
}
