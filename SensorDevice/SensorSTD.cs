using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Diagnostics;
using System.Threading;
using NLog;

namespace SensorDevice
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class SensorSTD : SensorDeviceStateM<SensorInfo, SensorSTDData>
    {
        // todo: 设置传感器的具体型号，参数等

        // 传感器的型号等信息
        // 其实也不用一定要定义，可以分散到 SensorInfo 中
        public InstrumentSqlrd InstrumentInfo { set; get; } = new InstrumentSqlrd();

        public SensorSTD()
        {
            Info = new SensorInfo();
            Info.sensorIdx = SensorCount++;
            Info.sensorType = SensorType.Standard;

            sPort.DataReceived += SPort_DataReceived;
        }
    }
}
