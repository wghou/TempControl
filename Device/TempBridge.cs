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
    public class TempBridge
    {
        private static readonly Logger nlogger = LogManager.GetCurrentClassLogger();

        #region Serial Port
        /// <summary>串口</summary>
        private const int baudrate = 9600;
        private const int dataBits = 8;
        private const StopBits stopBits = StopBits.One;
        private const Parity parity = Parity.None;
        private const int readBufferSize = 64;
        private const int writeBufferSize = 64;
        private const int readTimeout = 1000;
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
            ReadTimeout = readTimeout
        };
        /// <summary>串口读-写时间间隔</summary>
        private const int intervalOfWR = 50;
        #endregion

        public enum Err_b : int
        {
            /// <summary>无错误</summary>
            NoError = 0,
            /// <summary>
            /// 可以忽略的错误
            /// </summary>
            ErrOmit,
            /// <summary>串口错误</summary>
            ComError,
            /// <summary>程序错误</summary>
            CodeError
        };


        #region Public Members
        public string tpBridgeName = string.Empty;
        public string tpBridgePortName = string.Empty;
        /// <summary> 当前控温板通讯状态 / true - 正常 / false - 错误 </summary>
        public bool currentComStatus = true;
        /// <summary>
        /// 使能
        /// </summary>
        public bool Enable = true;
        /// <summary>
        /// 电桥读数间隔
        /// </summary>
        public int tpBridgeReadInterval = 2;
        /// <summary>
        /// 电桥通道
        /// </summary>
        public int tpBridgeChannel = 0;
        /// <summary>
        /// 暂时不用了
        /// </summary>
        public bool tpBridgeSelfCheckStatus = true;
        
        /// <summary>
        /// 电桥读取温度时错误连续计数
        /// </summary>
        public int tpBridgeErrorCount = 0;
        #endregion


        #region Private Members
        /// <summary>
        /// 传感器测得的温度值
        /// </summary>
        public List<float> temperatures = new List<float>();
        /// <summary>
        /// 锁
        /// </summary>
        private Object srLocker = new object();
        #endregion


        #region Public Methods
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
                nlogger.Error("配置主槽控温设备失败! 端口号: " + portName);
                currentComStatus = false;
                return !Enable;
            }

            currentComStatus = true;
            return true;
        }

        /// <summary>
        /// 温控设备初始化，并设置串口名称
        /// </summary>
        /// <param name="init">初始化状态，false 则表示初始化失败</param>
        /// <returns>返回设置状态</returns>
        public bool SetDevicePortName(string portName)
        {
            // 线程锁
            lock (srLocker)
            {
                try
                {
                    try { sPort.Close(); } catch { }

                    sPort.PortName = portName;

                    string[] portNames = SerialPort.GetPortNames();
                    if (!portNames.Contains(portName.ToUpper()))
                    {
                        nlogger.Error("端口 " + portName + " 不存在");
                        return false;
                    }

                    // 串口打开 / 关闭测试
                    // 不知道为什么，Thread.Sleep() 会引发 System.EntryPointNotFoundException 异常
                    // 但是，并不会被 catch 到
                    // 串口打开 / 关闭测试
                    if (!sPort.IsOpen)
                        sPort.Open();
                    Thread.Sleep(intervalOfWR);
                    if (sPort.IsOpen)
                        sPort.Close();

                    tpBridgePortName = portName;

                    return true;
                }
                catch (Exception ex)
                {
                    nlogger.Error("温控设备新建串口时发生异常：" + ex.Message);
                    return false;
                }
            }

        }


        /// <summary>
        /// 电桥开始测量
        /// </summary>
        /// <returns></returns>
        public bool StartMeasure()
        {
            string cmd = string.Empty;
            string data = string.Empty;

            // 5 - 开始测量
            cmd = "INIT:CONT 1";
            try
            {
                if (!sPort.IsOpen)
                    sPort.Open();

                // 写入数据
                this.sPort.WriteLine(cmd);
                Thread.Sleep(intervalOfWR);

                cmd = "INIT:CONT?";
                this.sPort.WriteLine(cmd);
                Thread.Sleep(intervalOfWR);

                data = this.sPort.ReadLine();
                sPort.DiscardInBuffer();
                sPort.Close();

            }
            catch (Exception ex)
            {
                Debug.WriteLine("传感器设备 " + cmd.ToString() + " 异常: " + ex.Message);
                // 关闭串口
                try { sPort.Close(); } catch { }
                return false;
            }
            if (!data.Contains("ON") && !data.Contains("1"))
                return false;

            return true;
        }


        /// <summary>
        /// 电桥停止测量
        /// </summary>
        /// <returns></returns>
        public bool StopMeasure()
        {
            string cmd = string.Empty;
            string data = string.Empty;

            // 5 - 开始测量
            cmd = "INIT:CONT 0";
            try
            {
                if (!sPort.IsOpen)
                    sPort.Open();

                // 写入数据
                this.sPort.WriteLine(cmd);
                Thread.Sleep(intervalOfWR);

                cmd = "INIT:CONT?";
                this.sPort.WriteLine(cmd);
                Thread.Sleep(intervalOfWR);

                data = this.sPort.ReadLine();
                sPort.DiscardInBuffer();
                sPort.Close();

            }
            catch (Exception ex)
            {
                Debug.WriteLine("传感器设备 " + cmd.ToString() + " 异常: " + ex.Message);
                // 关闭串口
                try { sPort.Close(); } catch { }
                return false;
            }
            if (!data.Contains("OFF") && !data.Contains("0"))
                return false;

            return true;
        }


        /// <summary>
        /// 传感器设备自检
        /// </summary>
        /// <returns></returns>
        public bool SelfCheck()
        {
            ///
            // 1 - 读取测温电桥设备号
            string cmd = "*IDN?";
            string data = string.Empty;
            try
            {
                if (!sPort.IsOpen)
                    this.sPort.Open();

                // 写入数据
                this.sPort.WriteLine(cmd);
                // 读取返回数据
                Thread.Sleep(intervalOfWR);
                data = this.sPort.ReadLine();
                //Improve: Add BCC checker
                sPort.DiscardInBuffer();
                sPort.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("传感器设备 " + cmd.ToString() + " 异常: " + ex.Message);
                // 关闭串口
                try { sPort.Close(); } catch { }
                return false;
            }

            // 判断返回的 data 是什么
            // 说明书上写应该是 FLUKE,1594,A9C111,1.10/1.00
            if (!data.Contains("1594"))
                return false;
            Thread.Sleep(500);


            // 5 - 开始测量
            cmd = "INIT:CONT 1";
            try
            {
                if (!sPort.IsOpen)
                    sPort.Open();

                // 写入数据
                this.sPort.WriteLine(cmd);
                Thread.Sleep(intervalOfWR);

                cmd = "INIT:CONT?";
                this.sPort.WriteLine(cmd);
                Thread.Sleep(intervalOfWR);

                data = this.sPort.ReadLine();
                sPort.DiscardInBuffer();
                sPort.Close();

            }
            catch (Exception ex)
            {
                Debug.WriteLine("传感器设备 " + cmd.ToString() + " 异常: " + ex.Message);
                // 关闭串口
                try { sPort.Close(); } catch { }
                return false;
            }
            if (!data.Contains("ON") && !data.Contains("1"))
                return false;

            // 开始测量，自检成功
            Thread.Sleep(2000);
            float val = 0;
            Err_b err = Err_b.NoError;
            if (this.tpBridgeChannel != 0)
            {
                err = GetBridgeValue(out val);
                if(err == Err_b.NoError)
                {
                    // 自检成功
                    return true;
                }
            }

            for(int i = 1;i<=4;i++)
            {
                Thread.Sleep(2000);
                this.tpBridgeChannel = i;
                err = GetBridgeValue(out val);
                if (err == Err_b.NoError)
                    break;
            }

            // 最终通道判断
            string configFilePath = @"./config.ini";
            if (err == Err_b.NoError)
            {
                try
                {
                    Utils.IniReadWrite.INIWriteValue(configFilePath, "TpBridge", "tpBridgeChannel", tpBridgeChannel.ToString("0"));
                }
                catch(Exception ex)
                {
                    Utils.Logger.Sys("写入 config.ini 文件时出现异常 - tpBridgeChannel");
                }
                return true;
            }  
            else
            {
                this.tpBridgeChannel = 0;
                try
                {
                    Utils.IniReadWrite.INIWriteValue(configFilePath, "TpBridge", "tpBridgeChannel", tpBridgeChannel.ToString("0"));
                }
                catch (Exception ex)
                {
                    Utils.Logger.Sys("写入 config.ini 文件时出现异常 - tpBridgeChannel");
                }
                return false;
            }
                
        }


        /// <summary>
        /// 获取传感器的值，存储在 SensorDevide.temperature 列表中，返回错误状态
        /// </summary>
        /// <param name="val">电桥温度值</param>
        /// <returns></returns>
        private Err_b GetBridgeValue(out float val)
        {
            Err_b st = Err_b.NoError;

            // 读取电桥温度值
            val = 0.0f;
            string cmd = "FETCH? " + tpBridgeChannel.ToString("0");
            string data = string.Empty;

            try
            {
                if (!sPort.IsOpen)
                    sPort.Open();

                // 写入数据
                this.sPort.WriteLine(cmd);
                Thread.Sleep(intervalOfWR);

                data = this.sPort.ReadLine();
                sPort.DiscardInBuffer();
                sPort.Close();
            }
            catch (Exception ex)
            {
                Utils.Logger.Sys("电桥错误 " + cmd.ToString() + " 异常: " + ex.Message);
                Utils.Logger.Sys(data);
                Debug.WriteLine("电桥错误 " + cmd.ToString() + " 异常: " + ex.Message);
                // 关闭串口
                try { sPort.Close(); } catch { }
                return Err_b.ComError;
            }

            // 解析返回的数据
            string[] str2;
            str2 = data.Split(',');
            if (str2[1] != "C" || str2[2] != tpBridgeChannel.ToString("0"))
            {
                Utils.Logger.Sys(data);
                Utils.Logger.Sys("电桥错误 1 - 读数中不包含 C 和 channelNumber");
                return Err_b.ErrOmit;
            }

            string numStr = str2[0].Replace("?", string.Empty);
            if (!float.TryParse(numStr, out val))
            {
                Utils.Logger.Sys(data);
                Utils.Logger.Sys("电桥错误 2 - 温度值无法从字符转换为数字");
                return Err_b.ErrOmit;
            }

            temperatures.Add(val);

            // 返回错误状态
            return st;
        }


        /// <summary>
        /// 读取电桥温度值，存储在 SensorDevide.temperature 列表中，返回错误状态
        /// </summary>
        /// <returns></returns>
        public Err_b UpdateBridgeValue()
        {
            float val = 0.0f;
            // 读取电桥温度值，返回错误状态
            Err_b err = GetBridgeValue(out val);

            // 如果没有发生错误，则计数器清零，返回 NoErr
            if(err == Err_b.NoError)
            {
                tpBridgeErrorCount = 0;
                return Err_b.NoError;
            }
            // 如果发生了错误，则计数器加一，如果大于等于 3 ，则返回错误；如果小于3，则暂时不返回错误
            else
            {
                tpBridgeErrorCount++;
                if(tpBridgeErrorCount < 3)
                {
                    // 如果小于 3，则暂时不返回错误
                    return Err_b.NoError;
                }
                else
                {
                    // 如果大于零，则错误计数器清零
                    tpBridgeErrorCount = 0;
                    return err;
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
        #endregion
    }
}
