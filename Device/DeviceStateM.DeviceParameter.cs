using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Timers;
using NLog;

namespace Device
{
    /// <summary>
    /// 当前工作状态 - 类
    /// </summary>
    public class TemptPointStruct
    {
        /// <summary>
        /// 当前温度点在 temperaturePointList 的位置
        /// </summary>
        public int tempPointIndex = 0;
        /// <summary>
        /// 该温度点是否测量完成
        /// </summary>
        public bool finished = false;
        /// <summary>
        /// 当前工作状态下的应达到的温度值
        /// </summary>
        public float stateTemp { get { return paramM[0]; } }
        /// <summary>
        /// 主槽控温表参数 - 7个
        /// </summary>
        public float[] paramM = new float[7];
        /// <summary>
        /// 辅槽控温表参数 - 7个
        /// </summary>
        public float[] paramS = new float[7];
        /// <summary>
        /// 进入该状态计时
        /// </summary>
        public UInt32 stateCounts = 0;
        /// <summary>
        /// 在当前状态需要持续的时间
        /// </summary>
        public UInt32 stateHoldCounts = 1;
    }


    public class RunningParamStruct
    {
        private static readonly Logger nlogger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 读取控温板时间间隔 Second
        /// </summary>
        public int readTempIntervalSec = 5;
        /// <summary>
        /// 波动度判断
        /// </summary>
        public float flucValue = 0.001f;
        /// <summary>
        /// 进入控温状态时的温度阈值
        /// </summary>
        public float controlTempThr = 0.4f;
        /// <summary>
        /// 控温槽温度上限
        /// </summary>
        public float tempMaxValue = 200.0f;
        /// <summary>
        /// 控温槽温度下限
        /// </summary>
        public float tempMinValue = -2.0f;

        /// <summary>
        /// 温度点排序 - 升序 / 降序
        /// </summary>
        public string sort = "descend";
        /// <summary>
        /// 实验完成后是否关闭计算机
        /// </summary>
        public bool shutDownComputer = false;
        /// <summary>
        /// 静置时间长度
        /// </summary>
        public uint standHoldCounts = 6;
        /// <summary>
        /// 单次加气最长时间
        /// </summary>
        public uint addGasHoldCounts = 6;


        public bool ReadValueConfig(string configFilePath)
        {
            try
            {
                // 如果配置文件不存在，则新建
                if (!File.Exists(configFilePath))
                {
                    WriteValueConfig(configFilePath);
                }

                ////////////////////////////////////////
                // 参数设置
                readTempIntervalSec = int.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "readTempIntervalSec", readTempIntervalSec.ToString()));
                flucValue = float.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "FlucValue", flucValue.ToString()));
                controlTempThr = float.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "controlTempThr", controlTempThr.ToString()));
                tempMaxValue = float.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "tempMaxValue", tempMaxValue.ToString()));
                tempMinValue = float.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "tempMinValue", tempMinValue.ToString()));

                // 其他
                sort = Utils.IniReadWrite.INIGetStringValue(configFilePath, "Others", "sort", sort);
                shutDownComputer = Utils.IniReadWrite.INIGetStringValue(configFilePath, "Others", "shutDownComputer", "Disable") == "Enable" ? true : false;

                standHoldCounts = uint.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "standHoldCounts", standHoldCounts.ToString()));
                addGasHoldCounts = uint.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "addGasHoldCounts", addGasHoldCounts.ToString()));
            }
            catch (Exception ex)
            {
                nlogger.Error("从配置文件读取参数过程中发生异常：" + ex.Message.ToString());
                return false;
            }

            return true;
        }

        public bool WriteValueConfig(string configFilePath)
        {
            try
            {
                // 相关参数
                Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "readTempIntervalSec", readTempIntervalSec.ToString());
                Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "flucValue", flucValue.ToString());
                Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "controlTempThr", controlTempThr.ToString());
                Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "tempMaxValue", tempMaxValue.ToString());
                Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "tempMinValue", tempMinValue.ToString());

                // 一些其他的调试参数
                // 升序还是降序
                Utils.IniReadWrite.INIWriteValue(configFilePath, "Others", "sort", sort);
                Utils.IniReadWrite.INIWriteValue(configFilePath, "Others", "shutDownComputer", shutDownComputer ? "Enable" : "Disable");

                Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "standHoldCounts", standHoldCounts.ToString());
                Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "addGasHoldCounts", addGasHoldCounts.ToString());
            }
            catch(Exception ex)
            {
                nlogger.Error("从配置文件写入参数过程中发生异常：" + ex.Message.ToString());
                return false;
            }

            return true;
        }
    }


    public partial class DeviceStateM
    {
        /// <summary>
        /// relay 继电器设备 1
        /// </summary>
        public RelayDevice ryDeviceM = new RelayDevice();

        /// <summary>
        /// relay 继电器设备 2
        /// </summary>
        public RelayDevice ryDeviceS = new RelayDevice();

        private Timer _ryConnectTimer;


        /// <summary>
        /// 主槽温控设备
        /// </summary>
        public TempDevice tpDeviceM = new TempDevice() { tpDeviceName = "主槽控温设备" };

        /// <summary>
        /// 辅槽控温设备
        /// </summary>
        public TempDevice tpDeviceS = new TempDevice() { tpDeviceName = "辅槽控温设备" };

        

        /// <summary>
        /// 系统开始运行的时间
        /// </summary>
        public DateTime startTime = new DateTime();


        /// <summary>
        /// 系统自动状态下，完成整个实验所需的温度点序列 - 
        /// 当温度点完成测量时，将温度点中的 finish 标记为 true
        /// </summary>
        public List<TemptPointStruct> temperaturePointList = new List<TemptPointStruct>();

        /// <summary>
        /// 当前工作状态 - 实例
        /// </summary>
        public TemptPointStruct currentTemptPointState = new TemptPointStruct();

        /// <summary>
        /// 锁 - 保证自动控制流程步骤执行的时候，相应资源（例如 currentState autoStart controlFlowList etc. ）不会被访问
        /// </summary>
        public Object stepLocker = new Object();

        public RunningParamStruct _runningParameters = new RunningParamStruct();



        /////////////////////
        // private mathod

        /// <summary>
        /// 读取水槽温度值/功率值，并记录
        /// </summary>
        private void UpdateTemptValue()
        {
            // 错误指示位
            TempProtocol.Err_t err = TempProtocol.Err_t.NoError;

            // 读取主槽温度，取小数点后 4 位
            float val = 0.0f;
            err = tpDeviceM.GetTemperatureShow(out val, 4);
            if (err != TempProtocol.Err_t.NoError)
            {
                // 如果发生错误，则记录错误
                SetErrorStatus(ErrorCode.TemptError);

                nlogger.Error("读取主槽温度时发生错误，errorCode: " + err.ToString());

                return;
            }
            // 记录主槽温度
            Utils.Logger.TempData(tpDeviceM.temperatures.Last());

            return;
        }
    }
}
