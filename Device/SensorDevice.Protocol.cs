using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Ports;
using NLog;

namespace Device
{
    public partial class SensorDevice
    {
        /// <summary>串口</summary>
        private SerialPort sPort;
        public string srDevicePortName;
        public bool Enable = true;
        /// <summary>串口读-写时间间隔</summary>
        private const int intervalOfWR = 20;

        /// <summary>
        /// 设备线程锁，同一时间只允许单一线程访问设备资源（串口 / 数据）
        /// </summary>
        private object srLocker = new object();
        public List<Tuple<float, float>> data = new List<Tuple<float, float>>();
        private int dataMaxLen = 1000;


        /// <summary>
        /// 传感器内部通信指令 RS232
        /// </summary>
        private enum Cmd_s : int
        {
            DS = 0,
            TS,
            QS
        }

        private string[] cmdString = new string[] { "DS", "TS", "QS" };


        /// <summary>
        /// 读取设备类型及状态 - 初始化设备
        /// </summary>
        /// <returns></returns>
        private bool getDeviceStatus()
        {
            string dtRaw = sendCMD(Cmd_s.DS);
            if (dtRaw == string.Empty) return false;

            try
            {
                // SBE37SI-RS232 v6.0.5 SERIAL NO. 30050 09 Jul 2018 14:46:5
                string[] status = dtRaw.Split(' ');

                deviceInfo.typeName = status[0];
                deviceInfo.version = status[1];
                deviceInfo.serialNo = status[4];

                // 解析设备类型
                switch (deviceInfo.typeName)
                {
                    case "SBE37SI-RS232":
                        deviceInfo.sensorType = SensorType.SBE37SI;
                        break;

                    default:
                        deviceInfo.sensorType = SensorType.Undefined;
                        // 设备类型读取错误
                        nlogger.Error("unknow sensor device type: " + deviceInfo.typeName);
                        break;
                }
            }
            catch(Exception ex)
            {
                // 设备类型读取错误
                nlogger.Error("failed to get the sensor device status.");
                return false;
            }
            
            return true;
        }


        /// <summary>
        /// 从传感器设备中读取数据
        /// </summary>
        /// <returns></returns>
        private bool readData()
        {
            string dtRaw = sendCMD(Cmd_s.TS);
            if (dtRaw == string.Empty) return false;

            try
            {
                // ··23.7021,··0.09802,···0.000,···0.4966,·1493.732,·-2.2522,··0.10064,·09·Jul·2018 ,·15:20:00
                // (temperature, conductivity, depth, salinity, sound velocity, local density, specific conductivity, date, time)
                string[] status = dtRaw.Split(',');

                float tp = float.Parse(status[0]);
                float cd = float.Parse(status[1]);

                AddData(new Tuple<float, float>(tp, cd));
            }
            catch (Exception ex)
            {
                // 设备类型读取错误
                nlogger.Error("failed to get the sensor device status.");
                return false;
            }

            return true;
        }


        /// <summary>
        /// 停止传感器工作
        /// </summary>
        private void stopSensor()
        {
            sendCMD(Cmd_s.QS);
        }


        /// <summary>
        /// 向设备发送指令
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private string sendCMD(Cmd_s cmd)
        {
            string dt = string.Empty;
            try
            {
                // 打开串口
                if (!sPort.IsOpen) this.sPort.Open();
                // 写入指令
                this.sPort.WriteLine(cmdString[(int)cmd]);
                //读取返回数据
                Thread.Sleep(intervalOfWR);
                dt = this.sPort.ReadLine();
                sPort.DiscardInBuffer();
                sPort.Close();
            }
            catch(Exception ex)
            {
                nlogger.Error("传感器设备读取参数失败！");
                // 关闭串口
                try { sPort.Close(); } catch{ }

                return string.Empty;
            }
            return dt;
        }


        /// <summary>
        /// 向 data 值列表中添加数据
        /// </summary>
        /// <param name="val"> 温度，电导率 </param>
        private void AddData(Tuple<float,float> val)
        {
            if (data.Count == dataMaxLen)
            {
                data.RemoveAt(0);
            }
            data.Add(val);
        }
    }
}
