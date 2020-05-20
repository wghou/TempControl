using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stateless;
using System.Timers;
using NLog;

namespace SensorDevice
{
    /// <summary>
    /// 传感器当前的状态
    /// </summary>
    public enum StateSensor : int
    {
        /// <summary> 空闲 </summary>
        Idle = 0,
        /// <summary> 测量数据 </summary>
        Measure,
        /// <summary> 存储数据 </summary>
        Store
    }

    public enum TriggerSensor : int
    {
        /// <summary> 时刻 </summary>
        TimerTick,
        /// <summary> 开始测量 </summary>
        StartMeasure,
        /// <summary> 开始存储数据 </summary>
        StartStore,
        /// <summary> 停止 </summary>
        Stop
    }

    /// <summary>
    /// 传感器基类-加入了状态机
    /// </summary>
    /// <typeparam name="TInfo"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public abstract class SensorDeviceStateM<TInfo, TData> : SensorDeviceBase
        where TInfo : SensorInfoBase
        where TData : SensorDataBase
    {
        /// <summary>
        /// 设备信息
        /// </summary>
        public TInfo Info { get; set; }
        /// <summary>
        /// 传感器设备的类型
        /// </summary>
        public override SensorType sensorType {
            set { Info.sensorType = value; }  get { return Info.sensorType; } }
        /// <summary>
        /// 传感器设备的编号
        /// </summary>
        public override int sensorIdx {
            set { Info.sensorIdx = value; } get { return Info.sensorIdx; } }

        /// <summary>
        /// 设备线程锁，同一时间只允许单一线程访问设备资源（串口 / 数据）
        /// </summary>
        protected object sdLocker = new object();
        /// <summary>
        /// 传感器数据缓存
        /// </summary>
        protected List<TData> sensorData = new List<TData>();
        private int dataMaxLen = 1000;


        /// <summary>
        /// 状态机类
        /// </summary>
        protected StateMachine<StateSensor, TriggerSensor> _sensorMachine;
        /// <summary>
        /// 当前处于的状态
        /// </summary>
        public StateSensor _sensorState { set; get; } = StateSensor.Idle;
        /// <summary>
        /// 时刻Trigger - 带参数 ms
        /// </summary>
        protected StateMachine<StateSensor, TriggerSensor>.TriggerWithParameters<int> _sensorTickTrigger;

        /// <summary>
        /// 定时器
        /// </summary>
        protected Timer _tickTimerSensor;
        /// <summary>
        /// 自动取样 - 状态计数器
        /// </summary>
        protected uint sensorStateCounts = 0;

        /// <summary>
        /// 当前需要测量的温度点
        /// </summary>
        protected float currentTemptPoint = 0.0f;


        public SensorDeviceStateM()
        {
            ConfigSensorStateless();
        }

        /// <summary>
        /// 执行读取信息步骤
        /// </summary>
        /// <returns></returns>
        public override bool StartMeasure()
        {
            if(_sensorState != StateSensor.Idle)
            {
                return false;
            }

            // 开始测量
            _sensorMachine.Fire(TriggerSensor.StartMeasure);
            return true;
        }
        /// <summary>
        /// 开始存储数据
        /// </summary>
        /// <returns></returns>
        public override bool StartStore()
        {
            if (_sensorState != StateSensor.Measure)
            {
                return false;
            }

            // 停止测量，并进入存储数据
            _sensorMachine.Fire(TriggerSensor.StartStore);
            // 执行一次步骤
            _sensorMachine.Fire(_sensorTickTrigger, 10);
            return true;
        }
        /// <summary>
        /// 停止测量数据
        /// </summary>
        /// <returns></returns>
        public override bool StopMeasure()
        {
            _sensorMachine.Fire(TriggerSensor.Stop);
            return true;
        }
        /// <summary>
        /// 获取当前设备的值
        /// </summary>
        /// <returns></returns>
        public virtual bool GetCurrentValue(out TData val)
        {
            if (sensorData.Count == 0)
            {
                val = null;
                return false;
            }
            else
            {
                val = sensorData.Last();
                return true;
            }
        }


        /// <summary>
        /// 执行 Idle 步骤
        /// </summary>
        protected virtual void internalIdleStep() { }
        /// <summary>
        /// 进入 Measure 步骤
        /// </summary>
        protected virtual void internalEnterMeasureStep() { }
        /// <summary>
        /// 执行 Measure 步骤
        /// </summary>
        protected virtual void internalMeasureStep() { }
        /// <summary>
        /// 进入 Store 步骤
        /// </summary>
        protected virtual void internalEnterStoreStep() { }
        /// <summary>
        /// 执行 Store 步骤
        /// </summary>
        protected virtual void internalStoreStep() { }


        /// <summary>
        /// 配置传感器相关的状态机
        /// </summary>
        protected void ConfigSensorStateless()
        {
            // new object
            _sensorMachine = new StateMachine<StateSensor, TriggerSensor>(() => _sensorState, s => _sensorState = s);
            _sensorTickTrigger = _sensorMachine.SetTriggerParameters<int>(TriggerSensor.TimerTick);

            // on transition action
            _sensorMachine.OnTransitioned(sensorOnTransitionedAction);

            // on unhandled trigger
            _sensorMachine.OnUnhandledTrigger(sensorOnUnhandledTrigger);

            // StateSensor.Idle
            // -> StateSensor.Measure
            _sensorMachine.Configure(StateSensor.Idle)
                .OnEntry(t => sensorIdleEntry())
                .OnExit(t => sensorIdleExit())
                .InternalTransition(_sensorTickTrigger, (tic, t) => sensorIdleTick(tic))
                .Permit(TriggerSensor.StartMeasure, StateSensor.Measure)
                .Ignore(TriggerSensor.StartStore)
                .Ignore(TriggerSensor.Stop);


            // StateSensor.Measure
            // -> StateSensor.StoreData
            // -> StateSensor.Idle
            _sensorMachine.Configure(StateSensor.Measure)
                .OnEntry(t => sensorMeasureEntry())
                .OnExit(t => sensorMeasureExit())
                .InternalTransition(_sensorTickTrigger, (tic, t) => sensorMeasureTick(tic))
                .Permit(TriggerSensor.StartStore, StateSensor.Store)
                .Permit(TriggerSensor.Stop, StateSensor.Idle);


            // StateSensor.Store
            // -> StateSensor.Idle
            _sensorMachine.Configure(StateSensor.Store)
                .OnEntry(t => sensorStoreEntry())
                .OnExit(t => sensorStoreExit())
                .InternalTransition(_sensorTickTrigger, (tic, t) => sensorStoreTick(tic))
                .Permit(TriggerSensor.Stop, StateSensor.Idle)
                .Ignore(TriggerSensor.StartMeasure);
        }

        /// <summary>
        /// 传感器单步执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _tickTimerSample_Elapsed(object sender, ElapsedEventArgs e)
        {
            // 计数
            sensorStateCounts++;

            _sensorMachine.Fire(_sensorTickTrigger, 10);
        }


        /// <summary>
        /// 状态转变事件函数
        /// </summary>
        /// <param name="act"></param>
        protected void sensorOnTransitionedAction(StateMachine<StateSensor, TriggerSensor>.Transition act)
        {
            nlogger.Debug("On StateSensor Transitioned.");

            TriggerSensor trigger = act.Trigger;
            StateSensor source = act.Source;
            StateSensor dest = act.Destination;

            // 状态清零
            sensorStateCounts = 0;
        }


        /// <summary>
        /// 未定义事件函数
        /// </summary>
        /// <param name="st"></param>
        /// <param name="tg"></param>
        protected void sensorOnUnhandledTrigger(StateSensor st, TriggerSensor tg)
        {
            nlogger.Error("sensor Unhandled trigger: state.");
        }


        /// <summary>
        /// sensor Idle Entry
        /// </summary>
        protected void sensorIdleEntry()
        {
            nlogger.Debug("Sensor Idle Entry.");
        }

        /// <summary>
        /// sensor Idle Tick
        /// </summary>
        /// <param name="tic"></param>
        protected void sensorIdleTick(int tic)
        {
            nlogger.Debug("Sensor Idle Tick: " + tic.ToString() + " ms");

            // 执行 idle 步骤
            internalIdleStep();
        }

        /// <summary>
        /// sensor Idle Exit
        /// </summary>
        protected void sensorIdleExit()
        {
            nlogger.Debug("Sensor Idle Exit.");
        }


        /// <summary>
        /// sensor Measure Entry
        /// </summary>
        protected void sensorMeasureEntry()
        {
            nlogger.Debug("Sensor Measure Entry.");

            // 进入 store 步骤
            internalEnterMeasureStep();
        }

        /// <summary>
        /// sensor Measure Tick
        /// </summary>
        /// <param name="tic"></param>
        protected void sensorMeasureTick(int tic)
        {
            nlogger.Debug("Sensor Measure Tick: " + tic.ToString() + " ms");

            // 执行 Measure 步骤
            internalMeasureStep();
        }

        /// <summary>
        /// sensor Measure Exit
        /// </summary>
        protected void sensorMeasureExit()
        {
            nlogger.Debug("Sensor Measure Exit.");
        }


        /// <summary>
        /// sensor Store Entry
        /// </summary>
        protected void sensorStoreEntry()
        {
            nlogger.Debug("Sensor Store Entry.");

            // 进入 store 步骤
            internalEnterStoreStep();
        }

        /// <summary>
        /// sensor Store Tick
        /// </summary>
        /// <param name="tic"></param>
        protected void sensorStoreTick(int tic)
        {
            nlogger.Debug("Sensor Store Tick: " + tic.ToString() + " ms");

            // 执行 store 步骤
            internalStoreStep();
        }

        /// <summary>
        /// sensor Store Exit
        /// </summary>
        protected void sensorStoreExit()
        {
            nlogger.Debug("Sensor Store Exit.");
        }

        /// <summary>
        /// 向 data 值列表中添加数据
        /// </summary>
        /// <param name="val"> 温度，电导率 </param>
        protected void appendData(TData val)
        {
            if (sensorData.Count == dataMaxLen)
            {
                sensorData.RemoveAt(0);
            }
            sensorData.Add(val);
        }
    }
}
