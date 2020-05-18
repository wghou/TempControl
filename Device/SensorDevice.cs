using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Ports;
using NLog;
using Others;

namespace Device
{
    public partial class SensorDevice
    {
        private static readonly Logger nlogger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 设备硬件信息
        /// </summary>
        public class SensorInfo
        {
            /// <summary> 设备型号名称 </summary>
            public string typeName { get; set; }
            /// <summary> xx 版本 </summary>
            public string version { get; set; }
            /// <summary> 序列号 </summary>
            public string serialNo { get; set; }
            /// <summary>
            /// 当前传感器设备的编号，范围 0 ～ maxSensorNum - 1（值为5）
            /// </summary>
            public int sensorIdx { get; set; } = -1;
            /// <summary> 设备类型 </summary>
            public SensorType sensorType { get; set; } = SensorType.Undefined;
        }

        /// <summary>
        /// 传感器设备的类型
        /// </summary>
        public enum SensorType : int
        {
            SBE37SI = 0,
            SBE37SIP,
            SBE37SM,
            SBE37SMP,
            SBE37SMPODO,
            Undefined,
            Standard
        }

        /// <summary>
        /// 传感器的错误状态
        /// </summary>
        public enum Err_sr : int
        {
            NoError = 0,
            Error
        }

        /// <summary>
        /// 设备信息
        /// </summary>
        public SensorInfo sensorInfo { get; set; } = new SensorInfo();

        /// <summary>
        /// 写入数据 sql
        /// </summary>
        private MySqlWriter sqlWriter = new MySqlWriter();

        /// <summary>
        /// 已有的传感器设备的总数
        /// </summary>
        public static int sensorCount { get; internal set; } = 0;
        public static int maxSensorNum { get; } = 6;
        

        /// <summary>
        /// 错误状态
        /// </summary>
        private Err_sr err_Sr = Err_sr.NoError;


        public SensorDevice()
        {
            sPort = new SerialPort()
            {
                ReadTimeout = 200,
                WriteTimeout = 200,
                BaudRate = 9600,
                NewLine = "\r\n"
            };

            sensorInfo.sensorIdx = sensorCount++;

            // 配置传感器相关状态机
            ConfigSensorStateless();
        }
    }
}
