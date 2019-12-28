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
    public class SensorDevice
    {
        private static readonly Logger nlogger = LogManager.GetCurrentClassLogger();

        /// <summary>串口</summary>
        private SerialPort sPort;
        public string srDevicName = string.Empty;
        public string srDevicePortName;
        public bool Enable = true;
        /// <summary>串口读-写时间间隔</summary>
        private const int intervalOfWR = 20;

        public enum Err_s : int
        {
            NoError = 0,
            ComError = 1
        }
        /// <summary>
        /// 设备线程锁，同一时间只允许单一线程访问设备资源（串口 / 数据）
        /// </summary>
        private object srLocker = new object();
        public List<float> temperatures = new List<float>();
        private int tempMaxLen = 1000;

        // 用于显示温度曲线的，只保存最新的数据，可以被清空
        public object tpShowLocker = new object();
        public List<float> temperaturesShow = new List<float>();

        public Err_s errStuts = Err_s.NoError;
        public bool FinishRqt = false;


        public SensorDevice()
        {
            sPort = new SerialPort()
            {
                ReadTimeout = 200,
                WriteTimeout = 200,
                BaudRate = 9600,
                NewLine = "\r\n"
            };

            sPort.DataReceived += SPort_DataReceived;
        }

        private void SPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = string.Empty;

            lock (srLocker)
            {
                // 从串口发送指令
                try
                {
                    //data = this.sPort.ReadTo(cmdFinish);
                    data = this.sPort.ReadLine();
                    //Improve: Add BCC checker
                    sPort.DiscardInBuffer();
                    //sPort.Close();
                }
                catch (Exception ex)
                {
                    // 串口发生错误！
                    nlogger.Error("传感器设备读取温度值异常: " + ex.Message);
                    errStuts = Err_s.ComError;
                    return;
                }

                try
                {
                    string[] subStr = data.Split(':');

                    switch (subStr[0].ToUpper())
                    {
                        case "DATA":
                            float val = 0.0f;
                            if (!float.TryParse(subStr[1], out val))
                            {
                                nlogger.Error("传感器设备读取温度值异常: ");
                                errStuts = Err_s.ComError;
                                return;
                            }
                            AddTemperature(val);
                            break;

                        case "FINISH":
                            this.FinishRqt = true;
                            break;

                        default:
                            Console.WriteLine("error");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    nlogger.Error("传感器设备读取温度值异常: ");
                    errStuts = Err_s.ComError;
                    return;
                }
            }
            
            return;
        }

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

        /// <summary>
        /// 通知传感器设备开始测量
        /// </summary>
        /// <returns></returns>
        public Err_s StartMeasure()
        {
            lock (srLocker)
            {
                FinishRqt = false;

                // 从串口发送指令
                try
                {
                    if (!sPort.IsOpen)
                        this.sPort.Open();

                    // 写入数据
                    this.sPort.WriteLine("start\r\n");
                    // 读取返回数据
                    //Thread.Sleep(intervalOfWR);
                    //data = this.sPort.ReadTo(cmdFinish);
                    //Improve: Add BCC checker
                    //sPort.DiscardInBuffer();
                }
                catch (Exception ex)
                {
                    // 串口发生错误！
                    nlogger.Error("传感器设备写入命令 start 异常: " + ex.Message);
                    // 关闭串口
                    return Err_s.ComError;
                }
            }
            
            return Err_s.NoError;
        }

        /// <summary>
        /// 定时检查传感器设备当前状态
        /// </summary>
        /// <returns></returns>
        public Err_s FetchErrorStatus()
        {
            lock (srLocker)
            {
                Err_s st = errStuts;
                try
                {
                    if (!sPort.IsOpen) this.sPort.Open();
                }
                catch { }

                errStuts = Err_s.NoError;
                return st;
            }
        }

        /// <summary>
        /// 获取 RQT 状态
        /// </summary>
        /// <returns></returns>
        public bool FetchRqt()
        {
            lock (srLocker)
            {
                bool st = FinishRqt;
                try
                {
                    if (!sPort.IsOpen) this.sPort.Open();
                }
                catch { }

                FinishRqt = false;
                return st;
            }
        }

        /// <summary>
        /// 向温度值列表中添加温度值
        /// </summary>
        /// <param name="val">温度值</param>
        private void AddTemperature(float val)
        {
            if (temperatures.Count == tempMaxLen)
            {
                temperatures.RemoveAt(0);
            }
            temperatures.Add(val);

            // 添加温度值，用于温度曲线显示
            lock (tpShowLocker)
            {
                if (temperaturesShow.Count == tempMaxLen)
                {
                    temperaturesShow.RemoveAt(0);
                }
                temperaturesShow.Add(val);
            }
        }
    }
}
