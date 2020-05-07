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
        public class DeviceInfo
        {
            /// <summary> 设备型号名称 </summary>
            public string typeName { get; internal set; }
            /// <summary> xx 版本 </summary>
            public string version { get; internal set; }
            /// <summary> 序列号 </summary>
            public string serialNo { get; internal set; }
            /// <summary>
            /// 当前传感器设备的编号
            /// </summary>
            public int sensorIdx { get; internal set; } = -1;
            /// <summary> 设备类型 </summary>
            public SensorType sensorType { get; internal set; } = SensorType.Undefined;
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
            Undefined
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
        public DeviceInfo deviceInfo = new DeviceInfo();

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

            deviceInfo.sensorIdx = sensorCount++;

            if(sensorCount > maxSensorNum)
            {
                nlogger.Error("the number of SensorDevice exceed the maxSensorNum: " + sensorCount.ToString());
            }
        }

        
        /// <summary>
        /// 设定串口号 / 初始化
        /// </summary>
        /// <param name="portName"></param>
        /// <returns></returns>
        public bool SetPortName(string portName)
        {
            // 当 Enable == false 时，返回 true
            srDevicePortName = portName;

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
    }
}
