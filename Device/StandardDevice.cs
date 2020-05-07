using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Diagnostics;
using System.Threading;
using NLog;

namespace Device
{
    public class StandardDevice
    {
        private static readonly Logger nlogger = LogManager.GetCurrentClassLogger();

        /// <summary>串口</summary>
        private SerialPort sPort;
        public string sdDevicePortName;
        public bool Enable = true;
        /// <summary>串口读-写时间间隔</summary>
        private const int intervalOfWR = 20;

        private int continuityCnt = 0;

        /// <summary>
        /// 设备线程锁，同一时间只允许单一线程访问设备资源（串口 / 数据）
        /// </summary>
        private object sdLocker = new object();
        public List<Tuple<float, float>> data = new List<Tuple<float, float>>();
        private int dataMaxLen = 1000;

        public StandardDevice()
        {
            sPort = new SerialPort()
            {
                ReadTimeout = 200,
                WriteTimeout = 200,
                BaudRate = 9600,
                NewLine = "\r\n"
            };
        }


        /// <summary>
        /// 设定串口号 / 初始化
        /// </summary>
        /// <param name="portName"></param>
        /// <returns></returns>
        public bool SetPortName(string portName)
        {
            // 当 Enable == false 时，返回 true
            sdDevicePortName = portName;

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

                sPort.DataReceived += SPort_DataReceived;

                return true;

            }
            catch (Exception ex)
            {
                nlogger.Error("标准数据采集器设备新建串口时发生异常：" + ex.Message);
                return !Enable;
            }
        }

        /// <summary>
        /// 接收到数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = sPort.ReadLine();

                float freqC, conduct, freqT, tempt, salt;
                int cnt;

                string[] valStrs = data.Split('-');

                freqC = float.Parse(valStrs[0]);
                conduct = float.Parse(valStrs[1]);
                freqT = float.Parse(valStrs[2]);
                tempt = float.Parse(valStrs[3]);
                salt = float.Parse(valStrs[4]);
                cnt = int.Parse(valStrs[5]);

                if(cnt != continuityCnt)
                {
                    nlogger.Error("标准数据采集器设备接受数据发生异常：连续标志位错误");
                }

                // todo: 数据的存储与记录


                // 连续标志位累进
                continuityCnt += 24;
                if (continuityCnt > 255) continuityCnt = 0;
            }
            catch(Exception ex)
            {
                nlogger.Error("标准数据采集器设备接受数据发生异常：" + ex.Message);
            }
        }
    }
}
