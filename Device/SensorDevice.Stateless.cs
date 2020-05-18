using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stateless;
using System.Timers;
using NLog;

namespace Device
{
    /// <summary>
    /// 传感器当前的状态
    /// </summary>
    public enum StateSensor : int
    {
        /// <summary> 空闲 </summary>
        Idle = 0,
        /// <summary> 识别传感器 </summary>
        Identify,
        /// <summary> 读取数据 </summary>
        Read,
        /// <summary> 测量数据 </summary>
        Measure,
        /// <summary> 完成测量 </summary>
        Finish,
        /// <summary> 停止 </summary>
        Stop
    }

    public enum TriggerSensor : int
    {
        /// <summary> 开始识别传感器 </summary>
        StartIdentify,
        /// <summary> 时刻 </summary>
        TimerTick,
        /// <summary> 开始读取数据 </summary>
        StartRead,
        /// <summary> 正式开始测量数据 / 存储 </summary>
        StartMeasure,
        /// <summary> 完成全部测量 </summary>
        FinishedAll,
        /// <summary> 强制停止 </summary>
        ForceStop
    }

    public partial class SensorDevice
    {
        StateMachine<StateSensor, TriggerSensor> _sensorMachine;
        public StateSensor _sensorState = StateSensor.Idle;

        /// <summary>
        /// 时刻 - 带参数 ms
        /// </summary>
        private StateMachine<StateSensor, TriggerSensor>.TriggerWithParameters<int> _sensorTickTrigger;
        
        /// <summary>
        /// 时钟
        /// </summary>
        private Timer _tickTimerSensor;

        /// <summary>
        /// 自动取样 - 状态计数器
        /// </summary>
        uint sensorStateCounts = 0;

        /// <summary>
        /// 当前需要测量的温度点
        /// </summary>
        float currentTemptPoint = 0.0f;


        /// <summary>
        /// 配置传感器相关的状态机
        /// </summary>
        public void ConfigSensorStateless()
        {
            // new object
            _sensorMachine = new StateMachine<StateSensor, TriggerSensor>(() => _sensorState, s => _sensorState = s);
            _sensorTickTrigger = _sensorMachine.SetTriggerParameters<int>(TriggerSensor.TimerTick);

            // on transition action
            _sensorMachine.OnTransitioned(sensorOnTransitionedAction);

            // on unhandled trigger
            _sensorMachine.OnUnhandledTrigger(sensorOnUnhandledTrigger);

            // StateSensor.Idle
            // -> StateSensor.Identify
            // -> StateSensor.Stop
            _sensorMachine.Configure(StateSensor.Idle)
                .OnEntry(t => sensorIdleEntry())
                .OnExit(t => sensorIdleExit())
                .InternalTransition(_sensorTickTrigger, (tic, t) => sensorIdleTick(tic))
                .Permit(TriggerSensor.StartIdentify, StateSensor.Identify)
                .Permit(TriggerSensor.ForceStop, StateSensor.Stop);


            // StateSensor.Identify
            // -> StateSensor.Read
            // -> StateSensor.Stop
            _sensorMachine.Configure(StateSensor.Identify)
                .OnEntry(t => sensorIdentifyEntry())
                .OnExit(t => sensorIdentifyExit())
                .InternalTransition(_sensorTickTrigger, (tic, t) => sensorIdentifyTick(tic))
                .Permit(TriggerSensor.StartRead, StateSensor.Read)
                .Permit(TriggerSensor.ForceStop, StateSensor.Stop);


            // StateSensor.Read
            // -> StateSensor.Measure
            // -> StateSensor.Finish
            // -> StateSensor.Stop
            _sensorMachine.Configure(StateSensor.Read)
                .OnEntry(t => sensorReadEntry())
                .OnExit(t => sensorReadExit())
                .InternalTransition(_sensorTickTrigger, (tic, t) => sensorReadTick(tic))
                .Permit(TriggerSensor.StartMeasure, StateSensor.Measure)
                .Permit(TriggerSensor.FinishedAll, StateSensor.Finish)
                .Permit(TriggerSensor.ForceStop, StateSensor.Stop);


            // StateSensor.Measure
            // -> StateSensor.Read
            // -> StateSensor.Finish
            // -> StateSensor.Stop
            _sensorMachine.Configure(StateSensor.Measure)
                .OnEntry(t => sensorMeasureEntry())
                .OnExit(t => sensorMeasureExit())
                .InternalTransition(_sensorTickTrigger, (tic, t) => sensorMeasureTick(tic))
                .Permit(TriggerSensor.StartRead, StateSensor.Read)
                .Permit(TriggerSensor.FinishedAll, StateSensor.Finish)
                .Permit(TriggerSensor.ForceStop, StateSensor.Stop);


            // StateSensor.Finish
            // -> StateSensor.Idle
            // -> StateSensor.Stop
            _sensorMachine.Configure(StateSensor.Finish)
                .OnEntry(t => sensorFinishEntry())
                .OnExit(t => sensorFinishExit())
                .InternalTransition(_sensorTickTrigger, (tic, t) => sensorFinishTick(tic))
                .Permit(TriggerSensor.FinishedAll, StateSensor.Idle)
                .Permit(TriggerSensor.ForceStop, StateSensor.Stop);


            // StateSensor.Stop
            // -> StateSensor.Idle
            _sensorMachine.Configure(StateSensor.Stop)
                .OnEntry(t => sensorStopEntry())
                .OnExit(t => sensorStopExit())
                .InternalTransition(_sensorTickTrigger, (tic, t) => sensorStopTick(tic))
                .Permit(TriggerSensor.ForceStop, StateSensor.Idle);

        }
    }
}
