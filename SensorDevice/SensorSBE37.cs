using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SensorDevice
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class SensorSBE37 : SensorDeviceStateM<SensorInfo, SensorSBE37Data>
    {
        // todo: 设置传感器的具体型号，参数等

        /// <summary>
        /// 传感器的信息
        /// 不是必须有的，也可以分散存储在 SensorInfo 中
        /// </summary>
        public SensorSqlrd SensorInfoSql { set; get; } = new SensorSqlrd();

        public SensorSBE37()
        {
            Info = new SensorInfo();
            Info.sensorIdx = SensorCount++;
            Info.sensorType = SensorType.SBE37SI;

            // 配置传感器相关状态机
            ConfigSensorStateless();

            // todo: 
            _tickTimerSensor = new System.Timers.Timer();
            _tickTimerSensor.Interval = 1000;
            _tickTimerSensor.AutoReset = true;
            _tickTimerSensor.Elapsed += _tickTimerSample_Elapsed;
            _tickTimerSensor.Start();

            // 串口接收到数据 - 事件处理函数
            sPort.DataReceived += SPort_DataReceived;
        }
    }
}
