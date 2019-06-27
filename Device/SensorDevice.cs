using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;
using NLog;

namespace Device
{
    public class SensorDevice
    {
        private static readonly Logger nlogger = LogManager.GetCurrentClassLogger();


        /// <summary>串口</summary>
        private const int baudrate = 9600;
        private const int dataBits = 8;
        private const StopBits stopBits = StopBits.One;
        private const Parity parity = Parity.None;
        private const int readBufferSize = 64;
        private const int writeBufferSize = 64;
        /// <summary>串口</summary>
        private SerialPort sPort = new SerialPort()
        {
            // Init all parameters except portname, as other parameter should not be easily changed.
            BaudRate = baudrate,
            DataBits = dataBits,
            StopBits = stopBits,
            Parity = parity,
            ReadBufferSize = readBufferSize,
            WriteBufferSize = writeBufferSize,
        };
        /// <summary>串口读-写时间间隔</summary>
        private const int intervalOfWR = 20;


        public string srDeviceName = string.Empty;
        public string srDevicePortName = string.Empty;

        public bool currentComStatus = true;
        public bool Enable = true;

        /// <summary>
        /// 传感器测得的温度值
        /// </summary>
        public List<float> temperatures = new List<float>();
        /// <summary>
        /// 温度读取时间间隔
        /// </summary>
        public int readTempIntervalSec = 5;
        private int tempMaxLen = 1000;
        // 用于显示温度曲线的，只保存最新的数据，可以被清空
        public object srShowLocker = new object();
        public List<float> temperaturesShow = new List<float>();

        /// <summary>
        /// 请求进入下一个温度点
        /// </summary>
        private bool nextTempPointRQT = false;
        private float nextTempPoint = 16.0f;
        /// <summary>
        /// 请求结束程序运行
        /// </summary>
        private bool stopRunRQT = false;
        /// <summary>
        /// 锁
        /// </summary>
        private Object srLocker = new object();

        /// <summary>
        /// 设置端口号并连接设备
        /// </summary>
        /// <param name="portName"></param>
        /// <returns></returns>
        public bool ConfigSyn(string portName)
        {
            // 当 Enable == False 时，返回 true
            // 设置端口号
            if (SetDevicePortName(portName) == false)
            {
                nlogger.Error("配置传感器设备失败! 端口号: " + portName);
                currentComStatus = false;
                return !Enable;
            }

            try
            {
                sPort.Open();

                currentComStatus = true;
            }
            catch(Exception ex)
            {
                currentComStatus = false;
            }
            
            return true;
        }

        /// <summary>
        /// 初始化传感器设备 - 设定串口
        /// </summary>
        /// <param name="portName">端口名称</param>
        /// <returns></returns>
        public bool SetDevicePortName(string portName)
        {
            try
            {
                // 先主动关闭串口
                try { sPort.Close(); } catch { }

                string[] portNames = SerialPort.GetPortNames();
                if (portNames.Contains(portName.ToUpper()))
                {
                    sPort.PortName = portName;
                }
                else
                {
                    return false;
                }
                // 串口打开 / 关闭测试
                if (!sPort.IsOpen)
                    sPort.Open();
                Thread.Sleep(intervalOfWR);
                if (sPort.IsOpen)
                    sPort.Close();
                srDevicePortName = portName;

                sPort.DataReceived += SPort_DataReceived;

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("继电器设备新建串口时发生异常：" + ex.Message);
                return false;
            }

            return true;
        }


        // 串口数据接收
        private void SPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // 接收数据
            string dataRev = string.Empty;
            string dataBack = string.Empty;
            try
            {
                dataRev = sPort.ReadTo("@");
                sPort.DiscardInBuffer();
            }
            catch(Exception ex)
            {
                Debug.WriteLine("接收另外一台计算机数据时发生错误：" + ex.Message);
                Utils.Logger.Sys("接收另外一台计算机数据时发生错误：" + ex.Message);

                Thread.Sleep(intervalOfWR);
                // 返回错误标志
                sPort.WriteLine("ERROR@");
                return;
            }


            // 解析数据
            float val = 0.0f;
            if(dataRev.Contains("CTEMP:"))
            {
                // 温度数据
                if(float.TryParse(dataRev.Substring(6),out val))
                {
                    lock (srLocker) {
                        if (temperatures.Count == tempMaxLen)
                            temperatures.RemoveAt(0);

                        temperatures.Add(val);
                    }

                    // 添加温度值，用于温度曲线显示
                    lock (srShowLocker)
                    {
                        if (temperaturesShow.Count == tempMaxLen)
                        {
                            temperaturesShow.RemoveAt(0);
                        }
                        temperaturesShow.Add(val);
                    }


                    Thread.Sleep(intervalOfWR);
                    // 返回错误标志
                    sPort.WriteLine("CTEMP@");
                }
                else
                {
                    // error
                    Thread.Sleep(intervalOfWR);
                    // 返回错误标志
                    sPort.WriteLine("ERROR@");
                }
            }
            else if(dataRev.Contains("NEXTP:"))
            {
                // 下一个温度点
                if (float.TryParse(dataRev.Substring(6), out val))
                {
                    lock (srLocker) { nextTempPointRQT = true; nextTempPoint = val; }
                    Thread.Sleep(intervalOfWR);
                    // 返回错误标志
                    sPort.WriteLine("NEXTP@");
                }
                else
                {
                    // error
                    Thread.Sleep(intervalOfWR);
                    // 返回错误标志
                    sPort.WriteLine("ERROR@");
                }
            }
            else if(dataRev.Contains("STOPR"))
            {
                // 停止测量，并关闭软件
                lock (srLocker) { stopRunRQT = true; }
                Thread.Sleep(intervalOfWR);
                // 返回错误标志
                sPort.WriteLine("STOPR@");
            }
            else
            {
                // 未知指令，错误
                Thread.Sleep(intervalOfWR);
                // 返回错误标志
                sPort.WriteLine("ERROR@");
            }

            return;
        }


        /// <summary>
        /// 传感器设备自检
        /// </summary>
        /// <returns></returns>
        public bool SelfCheck()
        {

            return true;
        }



        /// <summary>
        /// 查询当前 后端水槽温度
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool GetCurrentTemp(out float val)
        {

            lock(srLocker)
            {
                if(temperatures.Count!=0)
                {
                    val = temperatures.Last();
                    return true;
                }
                else
                {
                    val = -1.0f;
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// 查询是否请求进入下一个温度点
        /// </summary>
        /// <param name="pointVal"></param>
        /// <returns></returns>
        public bool CheckNextPointRQT(out float pointVal)
        {
            lock(srLocker)
            {
                pointVal = nextTempPoint;
                if(nextTempPointRQT == true)
                {
                    nextTempPointRQT = false;
                    return true;
                }
                else
                {
                    nextTempPointRQT = false;
                    return false;
                }
            }
        }



        /// <summary>
        /// 查询是否请求退出程序
        /// </summary>
        /// <returns></returns>
        public bool CheckStopRunRQT()
        {
            lock (srLocker)
            {
                if (stopRunRQT == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        /// <summary>
        /// 计算并获取温度波动值
        /// </summary>
        /// <param name="count">温度监测次数</param>
        /// <param name="fluctuation">温度波动值</param>
        /// <returns>返回成功与否</returns>
        public bool GetFluc(int count, out float fluctuation)
        {
            lock (srLocker)
            {
                if (temperatures.Count == 0 || temperatures.Count < count)
                {
                    // If there is not temperature data in list, output extreme fluctuation
                    fluctuation = -1;
                    return false;
                }
                else
                {
                    fluctuation = temperatures.GetRange(temperatures.Count - count, count).Max() -
                        temperatures.GetRange(temperatures.Count - count, count).Min();
                    return true;
                }
            }
        }


        public bool GetFlucDurCountOrLess(int count, out float fluctuation)
        {
            lock (srLocker)
            {
                if (temperatures.Count < 2)
                {
                    // If there is not temperature data in list, output extreme fluctuation
                    fluctuation = -1;
                    return false;
                }
                else if (temperatures.Count < count)
                {
                    // If there doesnot contain enough temperature data, output current fluctuation
                    fluctuation = temperatures.GetRange(0, temperatures.Count).Max() -
                        temperatures.GetRange(0, temperatures.Count).Min();
                    return false;
                }
                else
                {
                    fluctuation = temperatures.GetRange(temperatures.Count - count, count).Max() -
                        temperatures.GetRange(temperatures.Count - count, count).Min();
                    return true;
                }
            }
        }


        /// <summary>
        /// 判断传感器设备的温度波动度是否满足条件
        /// </summary>
        /// <param name="secends">时间长度 / 秒</param>
        /// <param name="crt">波动度阈值</param>
        /// <returns></returns>
        public bool chekFluc(int count, float crt)
        {
            float fluc = 0.0f;
            if (!GetFluc(count, out fluc))
                return false;
            else
                return fluc < crt;
        }
    }
}
