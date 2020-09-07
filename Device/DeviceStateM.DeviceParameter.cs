using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Timers;
using NLog;
using InstDevice;

namespace Device
{
    /// <summary>
    /// 当前工作状态 - 类
    /// </summary>
    public class TemptPointStruct : IComparable
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
        /// 是否自动取样
        /// </summary>
        public bool autoSample = true;
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
        /// 比较函数
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(Object obj)
        {
            if (obj == null) return 1;
            TemptPointStruct otherState = obj as TemptPointStruct;
            if (stateTemp > otherState.stateTemp) { return 1; }
            else
            {
                if (stateTemp == otherState.stateTemp) { return 0; }
                else { return -1; }
            }
        }
    }


    public class RunningParamStruct
    {
        private static readonly Logger nlogger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 读取控温板时间间隔 Second
        /// </summary>
        public int readTempIntervalSec = 5;
        /// <summary>
        /// 稳定时间 second
        /// </summary>
        public int steadyTimeSec = 300;
        /// <summary>
        /// 电桥温度稳定时间
        /// </summary>
        public int bridgeSteadyTimeSec = 120;
        /// <summary>
        /// 波动度判断
        /// </summary>
        public float flucValue = 0.001f;
        /// <summary>
        /// 进入控温状态时的温度阈值
        /// </summary>
        public float controlTempThr = 0.4f;
        /// <summary>
        /// 温度不升 / 不降故障判断时间
        /// </summary>
        public int tempNotUpOrDownFaultTimeSec = 600;
        /// <summary>
        /// 温度不升 / 不降故障温度阈值
        /// </summary>
        public float tempNotUpOrDwonFaultThr = 0.4f;
        /// <summary>
        /// 温度不升 / 不降故障温度阈值 2
        /// </summary>
        public float tempNotUpOrDwonFaultThrLow = 0.2f;
        /// <summary>
        /// 波动度过大故障判断时间
        /// </summary>
        public int flucFaultTimeSec = 120;
        /// <summary>
        /// 波动度过大故障阈值
        /// </summary>
        public float flucFaultThr = 0.4f;
        /// <summary>
        /// 温度偏离设定点故障阈值
        /// </summary>
        public float tempBiasFaultThr = 2.0f;
        /// <summary>
        /// 控温槽温度上限
        /// </summary>
        public float tempMaxValue = 40.0f;
        /// <summary>
        /// 控温槽温度下限
        /// </summary>
        public float tempMinValue = -2.0f;
        /// <summary>
        /// 辅槽制冷关闭阈值
        /// </summary>
        public float subCoolAndCircleShutdownThr = 36.0f;
        /// <summary>
        /// 降温时，主槽快冷关闭，温度判定范围划分
        /// </summary>
        public float tempDownCoolFShutdownDevision = 12.5f;
        /// <summary>
        /// 主槽快冷关 - 高温时阈值
        /// </summary>
        public float tempDownCoolFShutdownHot = 0.4f;
        /// <summary>
        /// 主槽快冷关 - 低温时阈值
        /// </summary>
        public float tempDownCoolFShutdownCool = 0.2f;

        public float subCoolFNotDownThr = 5.0f;

        /// <summary>
        /// 测量读数的时长
        /// </summary>
        public int measureTimeSec = 120;

        /// <summary>
        /// 温度点排序 - 升序 / 降序
        /// </summary>
        public string sort = "descend";
        /// <summary>
        /// 是否禁用总电源按键
        /// </summary>
        public bool ryElecEnable = false;
        /// <summary>
        /// 实验完成后是否关闭计算机
        /// </summary>
        public bool shutDownComputer = false;

        /// <summary>
        /// 关于自动取样的一些参数
        /// </summary>
        public AutoSample.SampleParam sampleParam = new AutoSample.SampleParam();


        public bool ReadValueConfig(string configFilePath)
        {
            try
            {
                // 如果配置文件不存在，则新建
                if (!File.Exists(configFilePath))
                {
                    WriteValueConfig(configFilePath);

                    sampleParam.WriteValueConfig(configFilePath);
                }

                ////////////////////////////////////////
                // 参数设置
                readTempIntervalSec = int.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "readTempIntervalSec", readTempIntervalSec.ToString()));
                steadyTimeSec = int.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "steadyTimeSec", steadyTimeSec.ToString()));
                bridgeSteadyTimeSec = int.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "bridgeSteadyTimeSec", bridgeSteadyTimeSec.ToString()));
                flucValue = float.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "FlucValue", flucValue.ToString()));
                controlTempThr = float.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "controlTempThr", controlTempThr.ToString()));
                tempNotUpOrDownFaultTimeSec = int.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "tempNotUpOrDownFaultTimeSec", tempNotUpOrDownFaultTimeSec.ToString()));
                tempNotUpOrDwonFaultThr = float.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "tempNotUpOrDwonFaultThr", tempNotUpOrDwonFaultThr.ToString()));
                tempNotUpOrDwonFaultThrLow = float.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "tempNotUpOrDwonFaultThrLow", tempNotUpOrDwonFaultThrLow.ToString()));
                flucFaultTimeSec = int.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "flucFaultTimeSec", flucFaultTimeSec.ToString()));
                flucFaultThr = float.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "flucFaultThr", flucFaultThr.ToString()));
                tempBiasFaultThr = float.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "tempBiasFaultThr", tempBiasFaultThr.ToString()));
                tempMaxValue = float.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "tempMaxValue", tempMaxValue.ToString()));
                tempMinValue = float.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "tempMinValue", tempMinValue.ToString()));
                subCoolAndCircleShutdownThr = float.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "subCoolAndCircleShutdownThr", subCoolAndCircleShutdownThr.ToString()));
                tempDownCoolFShutdownDevision = float.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "tempDownCoolFShutdownHot", tempDownCoolFShutdownHot.ToString()));
                tempDownCoolFShutdownHot = float.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "tempDownCoolFShutdownHot", tempDownCoolFShutdownHot.ToString()));
                tempDownCoolFShutdownCool = float.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "tempDownCoolFShutdownCool", tempDownCoolFShutdownCool.ToString()));
                subCoolFNotDownThr = float.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "subCoolFNotDownThr", subCoolFNotDownThr.ToString()));
                measureTimeSec = int.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "measureTimeSec", measureTimeSec.ToString()));

                // 其他
                sort = Utils.IniReadWrite.INIGetStringValue(configFilePath, "Others", "sort", sort);
                ryElecEnable = Utils.IniReadWrite.INIGetStringValue(configFilePath, "Others", "ryElecEnable", "Disable") == "Enable" ? true : false;
                shutDownComputer = Utils.IniReadWrite.INIGetStringValue(configFilePath, "Others", "shutDownComputer", "Disable") == "Enable" ? true : false;

                // 读取自动采样参数
                sampleParam.ReadValueConfig(configFilePath);
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
                Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "steadyTimeSec", steadyTimeSec.ToString());
                Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "bridgeSteadyTimeSec", bridgeSteadyTimeSec.ToString());
                Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "flucValue", flucValue.ToString());
                Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "controlTempThr", controlTempThr.ToString());
                Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "tempNotUpOrDownFaultTimeSec", tempNotUpOrDownFaultTimeSec.ToString());
                Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "tempNotUpOrDwonFaultThr", tempNotUpOrDwonFaultThr.ToString());
                Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "tempNotUpOrDwonFaultThrLow", tempNotUpOrDwonFaultThrLow.ToString());
                Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "flucFaultTimeSec", flucFaultTimeSec.ToString());
                Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "flucFaultThr", flucFaultThr.ToString());
                Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "tempBiasFaultThr", tempBiasFaultThr.ToString());
                Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "tempMaxValue", tempMaxValue.ToString());
                Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "tempMinValue", tempMinValue.ToString());
                Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "subCoolAndCircleShutdownThr", subCoolAndCircleShutdownThr.ToString());
                Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "tempDownCoolFShutdownDevision", tempDownCoolFShutdownDevision.ToString());
                Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "tempDownCoolFShutdownHot", tempDownCoolFShutdownHot.ToString());
                Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "tempDownCoolFShutdownCool", tempDownCoolFShutdownCool.ToString());
                Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "subCoolFNotDownThr", subCoolFNotDownThr.ToString());
                Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "measureTimeSec", measureTimeSec.ToString());

                // 一些其他的调试参数
                // 升序还是降序
                Utils.IniReadWrite.INIWriteValue(configFilePath, "Others", "sort", sort);
                Utils.IniReadWrite.INIWriteValue(configFilePath, "Others", "ryElecEnable", ryElecEnable ? "Enable" : "Disable");
                Utils.IniReadWrite.INIWriteValue(configFilePath, "Others", "shutDownComputer", shutDownComputer ? "Enable" : "Disable");

                // 读取自动采样参数
                sampleParam.WriteValueConfig(configFilePath);
            }
            catch (Exception ex)
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
                goto next;
            }
            // 记录主槽温度
            Utils.Logger.TempData(tpDeviceM.temperatures.Last());


            // 读取主槽功率系数
            err = tpDeviceM.GetPowerShow(out val);
            if (err != TempProtocol.Err_t.NoError)
            {
                // 如果发生错误，则记录错误
                SetErrorStatus(ErrorCode.TemptError);

                nlogger.Error("读取主槽功率时发生错误，errorCode: " + err.ToString());
                goto next;
            }

            /// Label:
            next:
            if (tpDeviceS.Enable == false) return;

            err = tpDeviceS.GetTemperatureShow(out val, 3);
            if (err != TempProtocol.Err_t.NoError)
            {
                // 如果发生错误，则记录错误
                SetErrorStatus(ErrorCode.TemptError);

                nlogger.Error("读取辅槽温度时发生错误，errorCode: " + err.ToString());
                return;
            }


            // 读取辅槽功率系数
            err = tpDeviceS.GetPowerShow(out val);
            if (err != TempProtocol.Err_t.NoError)
            {
                // 如果发生错误，则记录错误
                SetErrorStatus(ErrorCode.TemptError);

                nlogger.Error("读取辅槽功率时发生错误，errorCode: " + err.ToString());
                return;
            }
            return;
        }
    }
}