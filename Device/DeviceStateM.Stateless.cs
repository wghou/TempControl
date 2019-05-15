using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stateless;
using System.Timers;

namespace Device
{
    /// <summary>
    /// 设备状态
    /// </summary>
    public enum State : int
    {
        /// <summary> 开始 </summary>
        Start,
        /// <summary> 升温 </summary>
        TempUp,
        /// <summary> 降温 </summary>
        TempDown,
        /// <summary> 控温 </summary>
        Control,
        /// <summary> 稳定 </summary>
        Stable,
        /// <summary> 测量 </summary>
        Measure,
        /// <summary> 停止 </summary>
        Stop,
        /// <summary> 空闲 </summary>
        Idle,
        /// <summary> 未定义 </summary>
        Undefine
    }

    /// <summary>
    /// 设备触发事件
    /// </summary>
    enum Trigger
    {
        /// <summary> 开始自动控温 </summary>
        StartAutoStep,
        /// <summary> 时刻 </summary>
        TimerTick,
        /// <summary> 下一温度设定点 </summary>
        NextTemptPoint,
        /// <summary> 开始控温 </summary>
        StartControl,
        /// <summary> 到达稳定 </summary>
        AchieveSteady,
        /// <summary> 开始测量 </summary>
        StartMeasure,
        /// <summary> 暂停自动控温 </summary>
        SuspendAutoControl,
        /// <summary> 测量完成 </summary>
        FinishedAll,
        /// <summary> 强制停止 </summary>
        ForceStop,
        /// <summary> 电源打开 </summary>
        ElectOn,
        /// <summary> 电源关闭 </summary>
        ElectOff
    }


    public partial class DeviceStateM
    {
        /// <summary>
        /// 状态机
        /// </summary>
        StateMachine<State, Trigger> _machine;
        public State _state = State.Undefine;

        /// <summary>
        /// 下一温度设定点 - 带参数 float
        /// </summary>
        private StateMachine<State, Trigger>.TriggerWithParameters<float> _nextPointTrigger;

        /// <summary>
        /// 时刻 - 带参数 ms
        /// </summary>
        private StateMachine<State, Trigger>.TriggerWithParameters<int> _TickTrigger;


        /// <summary>
        /// 时钟
        /// </summary>
        private Timer _tickTimer;


        /// <summary>
        /// 配置 stateless 状态机
        /// </summary>
        private void ConfigStateless()
        {
            // new object
            _machine = new StateMachine<State, Trigger>(() => _state, s => _state = s);
            _nextPointTrigger = _machine.SetTriggerParameters<float>(Trigger.NextTemptPoint);
            _TickTrigger = _machine.SetTriggerParameters<int>(Trigger.TimerTick);

            // on transition action
            _machine.OnTransitioned(OnTransitionedAction);

            // on unhandled trigger
            _machine.OnUnhandledTrigger(OnUnhandledTrigger);

            // State.Undefine
            // 
            _machine.Configure(State.Undefine)
                .OnEntry(t => UndefineEntry())
                .OnExit(t => UndefineExit())
                .InternalTransition(_TickTrigger, (tic, t) => UndefineTick(tic))
                .Permit(Trigger.ElectOn, State.Idle);


            // State.Idle
            // -> State.Start
            // -> State.Stop
            _machine.Configure(State.Idle)
                .OnEntry(t => IdleEntry())
                .OnExit(t => IdleExit())
                .InternalTransition(_TickTrigger, (tic, t) => IdleTick(tic))
                .Permit(Trigger.StartAutoStep, State.Start)
                .Permit(Trigger.ForceStop, State.Stop);


            // State.Start
            _machine.Configure(State.Start)
                .OnEntry(t => StartEntry())
                .OnExit(t => StartExit())
                .InternalTransition(_TickTrigger, (tic, t) => StartTick(tic))
                .Permit(Trigger.StartControl,State.Control)
                .PermitIf<float>(_nextPointTrigger, State.TempUp, tp => !nextPointDown(tp))
                .PermitIf<float>(_nextPointTrigger, State.TempDown, tp => nextPointDown(tp))
                .Permit(Trigger.FinishedAll,State.Idle)
                .Permit(Trigger.ForceStop, State.Stop);


            // state TempUp
            _machine.Configure(State.TempUp)
                .OnEntry(t => TempUpEntry())
                .OnExit(t => TempUpExit())
                .InternalTransition(_TickTrigger, (tic, t) => TempUpTick(tic))
                .Permit(Trigger.StartControl, State.Control)
                .Permit(Trigger.SuspendAutoControl, State.Idle)
                .Permit(Trigger.ForceStop, State.Stop);


            // state TempDown
            _machine.Configure(State.TempDown)
                .OnEntry(t => TempDownEntry())
                .OnExit(t => TempDownExit())
                .InternalTransition(_TickTrigger, (tic, t) => TempDownTick(tic))
                .Permit(Trigger.StartControl, State.Control)
                .Permit(Trigger.SuspendAutoControl, State.Idle)
                .Permit(Trigger.ForceStop, State.Stop);


            // state Control
            _machine.Configure(State.Control)
                .OnEntry(t => ControlEntry())
                .OnExit(t => ControlExit())
                .InternalTransition(_TickTrigger, (tic, t) => ControlTick(tic))
                .Permit(Trigger.AchieveSteady, State.Stable)
                .Permit(Trigger.SuspendAutoControl, State.Idle)
                .Permit(Trigger.ForceStop, State.Stop);


            // state Steady
            _machine.Configure(State.Stable)
                .OnEntry(t => StableEntry())
                .OnExit(t => StableExit())
                .InternalTransition(_TickTrigger, (tic, t) => StableTick(tic))
                .Permit(Trigger.StartMeasure, State.Measure)
                .Permit(Trigger.SuspendAutoControl, State.Idle)
                .Permit(Trigger.ForceStop, State.Stop);


            // state Measure
            _machine.Configure(State.Measure)
                .OnEntry(t => MeasureEntry())
                .OnExit(t => MeasureExit())
                .InternalTransition(_TickTrigger, (tic, t) => MeasureTick(tic))
                .PermitIf<float>(_nextPointTrigger, State.TempUp, tp => !nextPointDown(tp))
                .PermitIf<float>(_nextPointTrigger, State.TempDown, tp => nextPointDown(tp))
                .Permit(Trigger.FinishedAll, State.Idle)
                .Permit(Trigger.SuspendAutoControl, State.Idle)
                .Permit(Trigger.ForceStop, State.Stop);


            // state Stop
            _machine.Configure(State.Stop)
                .OnEntry(t => StopEntry())
                .OnExit(t => StopExit())
                .InternalTransition(_TickTrigger, (tic, t) => StopTick(tic));


            // 设置定时器
            _tickTimer = new Timer();
            _tickTimer.Interval = 2500; // 默认 5s
            _tickTimer.AutoReset = true;
            _tickTimer.Elapsed += _stepTimer_Elapsed;
        }
    }
}
