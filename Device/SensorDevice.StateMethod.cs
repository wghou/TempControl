using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stateless;
using System.Timers;
using Others;

namespace Device
{
    public partial class SensorDevice
    {
        /// <summary>
        /// 传感器单步执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void _tickTimerSample_Elapsed(object sender, ElapsedEventArgs e)
        {
            // 计数
            sensorStateCounts++;

            _sensorMachine.Fire(_sensorTickTrigger, 10);

            
            // 如果发生错误，则错误事件 - 触发
            // todo
            SensorErrorEvent?.Invoke();

            // 时刻事件 - 触发
            SensorTimerTickEndEvent?.Invoke();
        }


        /// <summary>
        /// 状态转变事件函数
        /// </summary>
        /// <param name="act"></param>
        private void sensorOnTransitionedAction(StateMachine<StateSensor, TriggerSensor>.Transition act)
        {
            nlogger.Debug("On StateSensor Transitioned.");

            TriggerSensor trigger = act.Trigger;
            StateSensor source = act.Source;
            StateSensor dest = act.Destination;

            // 状态清零
            sensorStateCounts = 0;

            // 状态改变事件 - 触发
            SensorStateChangedEvent?.Invoke();
        }


        /// <summary>
        /// 未定义事件函数
        /// </summary>
        /// <param name="st"></param>
        /// <param name="tg"></param>
        private void sensorOnUnhandledTrigger(StateSensor st, TriggerSensor tg)
        {
            nlogger.Error("sensor Unhandled trigger: state.");
        }


        /// <summary>
        /// sensor Idle Entry
        /// </summary>
        private void sensorIdleEntry()
        {
            nlogger.Debug("Sensor Idle Entry.");
        }


        /// <summary>
        /// sensor Idle Tick
        /// </summary>
        /// <param name="tic"></param>
        private void sensorIdleTick(int tic)
        {
            nlogger.Debug("Sensor Idle Tick: " + tic.ToString() + " ms");
        }


        /// <summary>
        /// sensor Idle Exit
        /// </summary>
        private void sensorIdleExit()
        {
            nlogger.Debug("Sensor Idle Exit.");
        }


        /// <summary>
        /// sensor Identify Entry
        /// </summary>
        private void sensorIdentifyEntry()
        {
            nlogger.Debug("Sensor Identify Entry.");
        }


        /// <summary>
        /// sensor Identify Tick
        /// </summary>
        /// <param name="tic"></param>
        private void sensorIdentifyTick(int tic)
        {
            nlogger.Debug("Sensor Identify Tick: " + tic.ToString() + " ms");

            // 如果正确识别到了设备
            // 开始读取数据
            if (getDeviceStatus())
            {
                // todo: 创建记录
                MySqlWriter.InstrumentDataSqlrd data = new MySqlWriter.InstrumentDataSqlrd();
                sqlWriter.InsertValue(data);
                _sensorMachine.Fire(TriggerSensor.StartRead);

                // 事件触发 - 识别到设备
                SensorIdentifiedEvent?.Invoke(sensorInfo);
            }
        }


        /// <summary>
        /// sensor Identify Exit
        /// </summary>
        private void sensorIdentifyExit()
        {
            nlogger.Debug("Sensor Identify Exit.");
        }


        /// <summary>
        /// sensor Read Entry
        /// </summary>
        private void sensorReadEntry()
        {
            nlogger.Debug("Sensor Read Entry.");
        }


        /// <summary>
        /// sensor Read Tick
        /// </summary>
        /// <param name="tic"></param>
        private void sensorReadTick(int tic)
        {
            nlogger.Debug("Sensor Idle Tick: " + tic.ToString() + " ms");

            // 读取数据
            bool rt = readData();

            if(rt == false)
            {
                // 读取数据发生了错误
                // todo
            }
        }


        /// <summary>
        /// sensor Read Exit
        /// </summary>
        private void sensorReadExit()
        {
            nlogger.Debug("Sensor Idle Exit.");
        }


        /// <summary>
        /// sensor Measure Entry
        /// </summary>
        private void sensorMeasureEntry()
        {
            nlogger.Debug("Sensor Measure Entry.");

            // 新建数据库表/项
            // 辨识当前的温度点
            // todo
            MySqlWriter.InstrumentDataSqlrd data = new MySqlWriter.InstrumentDataSqlrd();
            sqlWriter.InsertValue(data);
        }


        /// <summary>
        /// sensor Measure Tick
        /// </summary>
        /// <param name="tic"></param>
        private void sensorMeasureTick(int tic)
        {
            nlogger.Debug("Sensor Measure Tick: " + tic.ToString() + " ms");

            // 读取数据
            bool rt = readData();

            if (rt == true)
            {
                // 正确读取了数据
                // 存储数据
                // todo
                MySqlWriter.InstrumentDataSqlrd data = new MySqlWriter.InstrumentDataSqlrd();
                sqlWriter.InsertValue(data);
            }
            else
            {
                // 读取数据发生了错误
                // todo
            }
        }


        /// <summary>
        /// sensor Measure Exit
        /// </summary>
        private void sensorMeasureExit()
        {
            nlogger.Debug("Sensor Measure Exit.");
        }


        /// <summary>
        /// sensor Finish Entry
        /// </summary>
        private void sensorFinishEntry()
        {
            nlogger.Debug("Sensor Finish Entry.");
        }


        /// <summary>
        /// sensor Finish Tick
        /// </summary>
        /// <param name="tic"></param>
        private void sensorFinishTick(int tic)
        {
            nlogger.Debug("Sensor Finish Tick: " + tic.ToString() + " ms");
        }


        /// <summary>
        /// sensor Finish Exit
        /// </summary>
        private void sensorFinishExit()
        {
            nlogger.Debug("Sensor Finish Exit.");
        }


        /// <summary>
        /// sensor Stop Entry
        /// </summary>
        private void sensorStopEntry()
        {
            nlogger.Debug("Sensor Stop Entry.");
        }


        /// <summary>
        /// sensor Stop Tick
        /// </summary>
        /// <param name="tic"></param>
        private void sensorStopTick(int tic)
        {
            nlogger.Debug("Sensor Stop Tick: " + tic.ToString() + " ms");
        }


        /// <summary>
        /// sensor Stop Exit
        /// </summary>
        private void sensorStopExit()
        {
            nlogger.Debug("Sensor Stop Exit.");
        }
    }
}
