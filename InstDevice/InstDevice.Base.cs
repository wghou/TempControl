using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Ports;
using NLog;
using Others;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InstDevice
{
    /// <summary>
    /// 测试温度点的集合
    /// </summary>
    public class TestOrderBase
    {
        /// <summary>
        /// 测试温度点的集合
        /// </summary>
        public static List<float> TemptPoints { set; get; } = new List<float>();
    }

    /// <summary>
    /// 仪器基类
    /// </summary>
    /// <typeparam name="TInfo">仪器状态类</typeparam>
    /// <typeparam name="TData">仪器数据类</typeparam>
    public class InstDeviceBase : TestOrderBase
    {
        /// <summary>
        /// 日志记录
        /// </summary>
        protected static readonly Logger nlogger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 写入数据 sql
        /// </summary>
        protected static readonly MySqlWriter sqlWriter = new MySqlWriter();

        // todo: 可以把温度点列表，从数据库读取到这里面
        /// <summary>
        /// 所有测试点的集合
        /// </summary>
        public static List<TestOrderSqlrd> testOrders { set; get; } = new List<TestOrderSqlrd>();


        /// <summary>
        /// 已有的仪器设备的总数
        /// </summary>
        protected static int InstCount { get; set; } = 0;
        /// <summary>
        /// 可支持的最大仪器设备数
        /// </summary>
        public static int MaxInstNum { get; } = 16;
        /// <summary>
        /// 仪器设备的编号
        /// </summary>
        public int InstIdx { get; private set; } = -1;
        /// <summary>
        /// 错误状态
        /// </summary>
        public Err_sr ErrorStatus { set; get; } = Err_sr.NoError;
        /// <summary>
        /// 启用/未启用
        /// 当 Enable == false 时，即可通过 InitWithInfo() 配置设备
        /// </summary>
        public bool Enable { set; get; } = false;
        

        /// <summary>串口</summary>
        protected SerialPort sPort;
        /// <summary>串口读-写时间间隔</summary>
        protected const int intervalOfWR = 20;


        public delegate void ErrorOccurEventHandler(Err_sr err);
        /// <summary>
        /// 发生错误 - 事件
        /// </summary>
        public event ErrorOccurEventHandler ErrorOccurEvent;
        /// <summary>
        /// 事件传递函数
        /// </summary>
        /// <param name="err"></param>
        protected virtual void OnErrorOccur(Err_sr err)
        {
            if (!Enable) return;

            ErrorOccurEvent?.Invoke(err);
        }


        public InstDeviceBase()
        {
            InstIdx = InstCount++;

            sPort = new SerialPort()
            {
                ReadTimeout = 500,
                WriteTimeout = 200,
                BaudRate = 9600,
                //NewLine = "\r\n"
            };

            // 串口接收到数据 - 事件处理函数
            sPort.DataReceived += SPort_DataReceived; ;
        }

        /// <summary>
        /// 获取仪器的基本信息
        /// </summary>
        /// <returns></returns>
        public virtual InstInfoBase GetBasicInfo()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 开始测量数据
        /// </summary>
        /// <returns></returns>
        public virtual bool StartMeasure() { return false; }
        /// <summary>
        /// 开始存储数据
        /// </summary>
        /// <returns></returns>
        public virtual bool StartStore() { return false; }
        /// <summary>
        /// 停止测量数据
        /// </summary>
        /// <returns></returns>
        public virtual bool StopMeasure() { return false; }
        

        /// <summary>
        /// 初始化仪器设备，通过 Info 信息及 cfg 包含的端口信息
        /// </summary>
        /// <returns></returns>
        public virtual bool InitWithInfo()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 关闭/停止仪器工作
        /// </summary>
        /// <returns></returns>
        public virtual bool DisableInstDevice()
        {
            Enable = false;
            try { sPort.Close(); } catch { }
            return true;
        }


        /// <summary>
        /// 设定串口号 / 初始化
        /// </summary>
        /// <param name="portName"> 端口号 </param>
        /// <param name="baudRate"> 波特率 </param>
        /// <returns></returns>
        protected bool SetPortName(string portName, int baudRate)
        {
            try
            {
                // 先主动关闭串口
                try { sPort.Close(); } catch { }

                sPort.PortName = portName;
                sPort.BaudRate = baudRate;

                string[] portNames = SerialPort.GetPortNames();
                if (!portNames.Contains(portName.ToUpper()))
                {
                    nlogger.Error("端口 " + portName + " 不存在");
                    return false;
                }
                // 串口打开 / 关闭测试
                if (!sPort.IsOpen)
                    sPort.Open();

                return true;

            }
            catch (Exception ex)
            {
                nlogger.Error("仪器设备新建串口时发生异常：" + ex.Message);
                return false;
            }
        }


        /// <summary>
        /// 处理串口接收到的数据
        /// </summary>
        /// <param name="data"></param>
        protected virtual void internalProceedReceivedData(string data)
        {
            // todo: 抛出异常
            throw new NotImplementedException();
        }

        /// <summary>
        /// 向设备发送指令
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns> 是否发送成功？true: 发送成功；false: 发送失败 </returns>
        protected bool sendCMD(string cmd)
        {
            // 空指令，不发送
            if (cmd == string.Empty) { return true; }

            try
            {
                // 打开串口
                if (!sPort.IsOpen) this.sPort.Open();
                // 写入指令
                this.sPort.WriteLine(cmd + "\r\n");
            }
            catch (Exception ex)
            {
                nlogger.Error("仪器设备读取参数失败！");
                // 关闭串口
                try { sPort.Open(); } catch { }

                return false;
            }
            return true;
        }

        /// <summary>
        /// 串口接收数据 - 事件处理函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // 设备未启用
            if (Enable == false) return;

            // todo: 如果数据接收结尾符号不同？
            try
            {
                Thread.Sleep(30);
                string data = sPort.ReadExisting();
                sPort.DiscardInBuffer();

                internalProceedReceivedData(data);
            }
            catch (Exception ex)
            {
                nlogger.Error("标准数据采集器设备接受数据发生异常：" + ex.Message);
                // 引发错误
                OnErrorOccur(Err_sr.Error);
            }
        }
    }
}
