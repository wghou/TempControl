using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace Device
{
    public partial class SensorDevice
    {
        /// <summary>
        /// 设定串口号 / 初始化
        /// </summary>
        /// <param name="portName"></param>
        /// <returns></returns>
        public bool SetPortName(string portName)
        {
            // 当 Enable == false 时，返回 true
            srDevicePortName = portName;

            if (sensorInfo.sensorIdx >= maxSensorNum)
            {
                nlogger.Error("the sensorInfo.sensorIdx exceed the maxSensorNum: " + sensorCount.ToString());
                return false;
            }

            try
            {
                // 先主动关闭串口
                try { sPort.Close(); } catch { }

                sPort.PortName = portName;

                string[] portNames = SerialPort.GetPortNames();
                if (!portNames.Contains(portName.ToUpper()))
                {
                    nlogger.Error("端口 " + portName + " 不存在");
                    return !Enable;
                }
                // 串口打开 / 关闭测试
                if (!sPort.IsOpen)
                    sPort.Open();
                //Thread.Sleep(intervalOfWR);
                //if (sPort.IsOpen)
                //    sPort.Close();
                return true;

            }
            catch (Exception ex)
            {
                nlogger.Error("传感器设备新建串口时发生异常：" + ex.Message);
                return !Enable;
            }
        }


        /// <summary>
        /// 检查传感器设备的错误状态
        /// </summary>
        public bool FetchErrorStatus()
        {
            return err_Sr != Err_sr.NoError;
        }


        /// <summary>
        /// 识别传感器设备
        /// </summary>
        public bool IdentifySensor()
        {
            // 当前已经处在传感器识别状态
            if (_sensorState == StateSensor.Identify) return true;

            // 仅能在 idle 状态下进入传感器识别状态
            if (_sensorState != StateSensor.Idle) return false;

            _sensorMachine.Fire(TriggerSensor.StartIdentify);

            // 进行一次传感器状态判断
            _sensorMachine.Fire(_sensorTickTrigger, 10);

            return true;
        }


        /// <summary>
        /// 开始读取数据
        /// </summary>
        public bool startRead()
        {
            // 当前已经处在数据读取状态
            if (_sensorState == StateSensor.Read) return true;

            // 仅能在 Measure 状态下，转换到 Read 状态
            if (_sensorState != StateSensor.Measure) return false;

            _sensorMachine.Fire(TriggerSensor.StartRead);

            return true;
        }


        /// <summary>
        /// 开始测量并存储数据
        /// </summary>
        public bool startMeasure(float tpPoint)
        {
            // 仅能在 read 状态下，才能进入 measure 状态
            if (_sensorState != StateSensor.Read) return false;

            // 存储当前的温度点
            currentTemptPoint = tpPoint;
            _sensorMachine.Fire(TriggerSensor.StartMeasure);

            return true;
        }


        /// <summary>
        /// 暂停测量
        /// </summary>
        /// <returns></returns>
        public bool suspendMeasure()
        {
            // 仅对于 measure 状态下，才能暂停测量，并进入 read 状态
            if (_sensorState != StateSensor.Measure) return true;

            if(_sensorState == StateSensor.Measure) _sensorMachine.Fire(TriggerSensor.StartRead);

            return true;
        }


        /// <summary>
        /// 完成全部测量
        /// </summary>
        /// <returns></returns>
        public bool finishAll()
        {
            // 在 Idle  Identify  Finish  Stop 状态下，不需要再进入完成状态
            if (_sensorState == StateSensor.Idle || _sensorState == StateSensor.Identify
                || _sensorState == StateSensor.Finish || _sensorState == StateSensor.Stop)
                return true;

            // 进入 Finish 状态
            _sensorMachine.Fire(TriggerSensor.FinishedAll);

            return true;
        }


        /// <summary>
        /// 停止传感器设别
        /// </summary>
        /// <returns></returns>
        public bool stopSensorDevie()
        {
            _sensorMachine.Fire(TriggerSensor.ForceStop);

            return true;
        }
    }
}
