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
    public abstract class InstDeviceBase : TestOrderBase
    {
        /// <summary>
        /// 日志记录
        /// </summary>
        protected static readonly Logger nlogger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 写入数据 sql
        /// </summary>
        protected static readonly MySqlWriter sqlWriter = new MySqlWriter();


        /// <summary>
        /// 所有测试点的集合
        /// </summary>
        //public static List<TestOrderSqlrd> testOrders { set; get; } = new List<TestOrderSqlrd>();


        /// <summary>
        /// 已有的仪器设备的总数
        /// </summary>
        protected static int InstCount { get; set; } = 0;
        /// <summary>
        /// 可支持的最大仪器设备数
        /// </summary>
        public static int MaxInstNum { get; } = 16;
        /// <summary>
        /// 仪器设备的类型
        /// </summary>
        public abstract TypeInst InstType { set; get; }
        /// <summary>
        /// 仪器设备的编号
        /// </summary>
        public abstract int InstIdx { set; get; }
        /// <summary>
        /// 错误状态
        /// </summary>
        public Err_sr ErrorStatus { set; get; } = Err_sr.NoError;
        /// <summary>
        /// 启用
        /// </summary>
        public bool Enable { set; get; } = false;
        /// <summary>
        /// 包含仪器标志位
        /// </summary>
        public TypeSensor SensorFlag { set; get; } = TypeSensor.None;


        /// <summary>串口</summary>
        protected SerialPort sPort;
        /// <summary>串口读-写时间间隔</summary>
        protected const int intervalOfWR = 20;


        /// <summary>
        /// 设备线程锁，同一时间只允许单一线程访问设备资源（串口 / 数据）
        /// </summary>
        protected object srLocker = new object();


        public InstDeviceBase()
        {
            sPort = new SerialPort()
            {
                ReadTimeout = 500,
                WriteTimeout = 200,
                BaudRate = 9600,
                //NewLine = "\r\n"
            };
        }


        /// <summary>
        /// 开始测量数据
        /// </summary>
        /// <returns></returns>
        public abstract bool StartMeasure();
        /// <summary>
        /// 开始存储数据
        /// </summary>
        /// <returns></returns>
        public abstract bool StartStore();
        /// <summary>
        /// 停止测量数据
        /// </summary>
        /// <returns></returns>
        public abstract bool StopMeasure();


        /// <summary>
        /// 初始化仪器设备
        /// </summary>
        /// <param name="cfg"></param>
        /// <returns></returns>
        public virtual bool Init(JObject cfg)
        {
            bool confOK = true;
            if (this.InstIdx >= MaxInstNum)
            {
                nlogger.Error("the instumentInfo.InstIdx exceed the maxInstrumentNum: " + InstCount.ToString());
                return false;
            }

            try
            {
                if (cfg.ContainsKey("PortName"))
                {
                    confOK &= SetPortName(cfg["PortName"].ToString());
                }
                else
                {
                    confOK = false;
                }
            }
            catch (Exception ex)
            {
                nlogger.Error("exception occur when SetPortName: " + ex.Message);
                return false;
            }

            return confOK;
        }


        /// <summary>
        /// 设定串口号 / 初始化
        /// </summary>
        /// <param name="portName"></param>
        /// <returns></returns>
        protected bool SetPortName(string portName)
        {
            try
            {
                // 先主动关闭串口
                try { sPort.Close(); } catch { }

                sPort.PortName = portName;

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
    }
}
