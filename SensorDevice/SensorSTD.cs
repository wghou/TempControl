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

        /// <summary>
        /// 检测标准温度传感器的波动度
        /// </summary>
        /// <param name="cnt"></param>
        /// <param name="crt"></param>
        /// <returns></returns>
        public bool CheckFluc(int cnt, double crt)
        {
            double fluc = 0;
            if (sensorData.Count == 0 || sensorData.Count < cnt)
            {
                // If there is not temperature data in list, output extreme fluctuation
   
                return false;
            }
            else
            {
                fluc = sensorData.GetRange(sensorData.Count - cnt, cnt).Max().vStandardT -
                    sensorData.GetRange(sensorData.Count - cnt, cnt).Min().vStandardT;
                
                if(fluc < crt) { return true; }
                else { return false; }
            }
        }
    }
}
