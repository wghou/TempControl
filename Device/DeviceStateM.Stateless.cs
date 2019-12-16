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
        /// <summary> 加氧 </summary>
        AddOxygen,
        /// <summary> 加氮 </summary>
        AddNitrogen,
        /// <summary> 静置 </summary>
        Stand,
        /// <summary> 稳定 </summary>
        Steady,
        /// <summary> 测量 </summary>
        Measure,
        /// <summary> 关闭计算机 </summary>
        ShutdownPC,
        /// <summary> 空闲 </summary>
        Idle
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
        /// <summary> 需要加氧气 </summary>
        NeedOxygen,
        /// <summary> 需要加氮气 </summary>
        NeedNitrogen,
        /// <summary> 进入静置 </summary>
        StartStand,
        /// <summary> 等待稳定 </summary>
        WaitSteady,
        /// <summary> 开始测量 </summary>
        StartMeasure,
        /// <summary> 暂停自动控温 </summary>
        SuspendAutoControl,
        /// <summary> 测量完成 </summary>
        FinishedAll,
        /// <summary> 强制停止 </summary>
        ForceShutdownPC
    }


    public partial class DeviceStateM
    {
        /// <summary>
        /// 状态机
        /// </summary>
        StateMachine<State, Trigger> _machine;
        public State _state = State.Idle;


        /// <summary>
        /// 上一次接通的气体。也就是管路中残存的气体类型
        /// </summary>
        private State _lastGasState = State.Idle;


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
            _TickTrigger = _machine.SetTriggerParameters<int>(Trigger.TimerTick);

            // on transition action
            _machine.OnTransitioned(OnTransitionedAction);

            // on unhandled trigger
            _machine.OnUnhandledTrigger(OnUnhandledTrigger);


            // State.Idle
            // -> State.Start
            // -> State.Stop
            _machine.Configure(State.Idle)
                .OnEntry(t => IdleEntry())
                .OnExit(t => IdleExit())
                .InternalTransition(_TickTrigger, (tic, t) => IdleTick(tic))
                .Permit(Trigger.StartAutoStep, State.Start)
                .Permit(Trigger.ForceShutdownPC, State.ShutdownPC)
                .Ignore(Trigger.SuspendAutoControl)
                .Ignore(Trigger.FinishedAll);


            // State.Start
            _machine.Configure(State.Start)
                .OnEntry(t => StartEntry())
                .OnExit(t => StartExit())
                .InternalTransition(_TickTrigger, (tic, t) => StartTick(tic))
                .Permit(Trigger.StartStand,State.Stand)
                .Permit(Trigger.NeedOxygen, State.AddOxygen)
                .Permit(Trigger.NeedNitrogen, State.AddNitrogen)
                .Permit(Trigger.FinishedAll,State.Idle)
                .Permit(Trigger.SuspendAutoControl, State.Idle)
                .Permit(Trigger.ForceShutdownPC, State.ShutdownPC);


            // state AddOxygen
            _machine.Configure(State.AddOxygen)
                .OnEntry(t => AddOxygenEntry())
                .OnExit(t => AddOxygenExit())
                .InternalTransition(_TickTrigger, (tic, t) => AddOxygenTick(tic))
                .Permit(Trigger.NeedNitrogen, State.AddNitrogen)
                .Permit(Trigger.StartStand, State.Stand)
                .Permit(Trigger.SuspendAutoControl, State.Idle)
                .Permit(Trigger.ForceShutdownPC, State.ShutdownPC)
                .InternalTransition(Trigger.NeedOxygen, AddOxygenRefresh);


            // state AddNitrogen
            _machine.Configure(State.AddNitrogen)
                .OnEntry(t => AddNitrogenEntry())
                .OnExit(t => AddNitrogenExit())
                .InternalTransition(_TickTrigger, (tic, t) => AddNitrogenTick(tic))
                .Permit(Trigger.NeedOxygen, State.AddOxygen)
                .Permit(Trigger.StartStand, State.Stand)
                .Permit(Trigger.SuspendAutoControl, State.Idle)
                .Permit(Trigger.ForceShutdownPC, State.ShutdownPC)
                .InternalTransition(Trigger.NeedNitrogen, addNitrogenRefresh);


            // state Stand
            _machine.Configure(State.Stand)
                .OnEntry(t => StandEntry())
                .OnExit(t => StandExit())
                .InternalTransition(_TickTrigger, (tic, t) => StandTick(tic))
                .Permit(Trigger.NeedOxygen, State.AddOxygen)
                .Permit(Trigger.NeedNitrogen, State.AddNitrogen)
                .Permit(Trigger.WaitSteady, State.Steady)
                .Permit(Trigger.SuspendAutoControl, State.Idle)
                .Permit(Trigger.ForceShutdownPC, State.ShutdownPC);

            // state Steady
            _machine.Configure(State.Steady)
                .OnEntry(t => SteadyEntry())
                .OnExit(t => SteadyExit())
                .InternalTransition(_TickTrigger, (tic, t) => SteadyTick(tic))
                .Permit(Trigger.NeedOxygen, State.AddOxygen)
                .Permit(Trigger.NeedNitrogen, State.AddNitrogen)
                .Permit(Trigger.StartStand, State.Stand)
                .Permit(Trigger.StartMeasure, State.Measure)
                .Permit(Trigger.SuspendAutoControl, State.Idle)
                .Permit(Trigger.ForceShutdownPC, State.ShutdownPC);


            // state Measure
            _machine.Configure(State.Measure)
                .OnEntry(t => MeasureEntry())
                .OnExit(t => MeasureExit())
                .InternalTransition(_TickTrigger, (tic, t) => MeasureTick(tic))
                .Permit(Trigger.StartAutoStep, State.Start)
                .Permit(Trigger.FinishedAll, State.Idle)
                .Permit(Trigger.SuspendAutoControl, State.Idle)
                .Permit(Trigger.ForceShutdownPC, State.ShutdownPC);


            // state ShutdownPC
            _machine.Configure(State.ShutdownPC)
                .OnEntry(t => ShutdownPCEntry())
                .OnExit(t => ShutdownPCExit())
                .InternalTransition(_TickTrigger, (tic, t) => ShutdownPCTick(tic))
                .Permit(Trigger.FinishedAll, State.Idle)
                .Ignore(Trigger.SuspendAutoControl)
                .Ignore(Trigger.ForceShutdownPC);


            // 设置定时器
            _tickTimer = new Timer();
            _tickTimer.Interval = 5000; // 默认 5s
            _tickTimer.AutoReset = true;
            _tickTimer.Elapsed += _stepTimer_Elapsed;

            _ryConnectTimer = new Timer();
            _ryConnectTimer.Interval = 4000;
            _ryConnectTimer.AutoReset = true;
            _ryConnectTimer.Elapsed += _ryConnectTimer_Elapsed;
        }
    }
}
