using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

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
        /// 进入该状态计时
        /// </summary>
        public UInt32 stateCounts = 0;
    }

    public class ThresholdParamStruct
    {
        
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
        /// 降温时，主槽快冷关闭，温度判定范围划分
        /// </summary>
        public float tempDownCoolFShoutdownDevision = 12.5f;
        /// <summary>
        /// 主槽快冷关 - 高温时阈值
        /// </summary>
        public float tempDownCoolFShoutdownHot = 0.4f;
        /// <summary>
        /// 主槽快冷关 - 低温时阈值
        /// </summary>
        public float tempDownCoolFShoutdownCool = 0.2f;

        /// <summary>
        /// 温度点排序 - 升序 / 降序
        /// </summary>
        public string sort = "descend";
        /// <summary>
        /// 是否禁用总电源按键
        /// </summary>
        public bool ryElecEnable = false;


        public bool ReadValueConfig(string configFilePath)
        {
            // 配置成功标志位
            bool confOK = true;

            try
            {
                // 如果配置文件不存在，则新建
                if (!File.Exists(configFilePath))
                {

                    // 相关参数
                    Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "SteadyTimeSec", "300");
                    Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "bridgeSteadyTimeSec", "120");
                    Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "FlucValue", "0.001");
                    Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "controlTempThr", "0.4");
                    Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "tempNotUpOrDownFaultTimeSec", "600");
                    Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "tempNotUpOrDwonFaultThr", "0.4");
                    Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "flucFaultTimeSec", "120");
                    Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "flucFaultThr", "0.4");
                    Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "tempBiasFaultThr", "2");
                    Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "tempMaxValue", "40");
                    Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "tempMinValue", "-2");
                    Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "tempDownCoolFShoutdownDevision", "12.5");
                    Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "tempDownCoolFShoutdownHot", "0.4");
                    Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "tempDownCoolFShoutdownCool", "0.2");

                    // 一些其他的调试参数
                    // 升序还是降序
                    Utils.IniReadWrite.INIWriteValue(configFilePath, "Others", "sort", "descend");
                    Utils.IniReadWrite.INIWriteValue(configFilePath, "Others", "ryElecEnable", "Disable");

                }


                ////////////////////////////////////////
                // 参数设置
                // 设置定时器更新时间
                //tpTemperatureUpdateTimer.Interval = tpDeviceM.readTempIntervalSec * 1000;
                // 从配置文件读取 稳定时间 / 波动度需满足的条件
                steadyTimeSec = int.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "SteadyTimeSecond", "300"));
                bridgeSteadyTimeSec = int.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "bridgeSteadyTimeSec", "120"));
                flucValue = float.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "FlucValue", "0.001"));
                controlTempThr = float.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "controlTempThr", "0.4"));
                tempNotUpOrDownFaultTimeSec = int.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "tempNotUpOrDownFaultTimeSec", "600"));
                tempNotUpOrDwonFaultThr = float.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "tempNotUpOrDwonFaultThr", "0.4"));
                //tempNotUpOrDwonFaultThrLow = float.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "tempNotUpOrDwonFaultThrLow", "0.2"));
                flucFaultTimeSec = int.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "flucFaultTimeSec", "120"));
                flucFaultThr = float.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "flucFaultThr", "0.4"));
                tempBiasFaultThr = float.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "tempBiasFaultThr", "2"));
                tempMaxValue = float.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "tempMaxValue", "40"));
                tempMinValue = float.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "tempMinValue", "-2"));
                tempDownCoolFShoutdownDevision = float.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "tempDownCoolFShoutdownDevision", "12.5"));
                tempDownCoolFShoutdownHot = float.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "tempDownCoolFShoutdownHot", "0.4"));
                tempDownCoolFShoutdownCool = float.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "tempDownCoolFShoutdownCool", "0.2"));

                // 默认降序
                sort = Utils.IniReadWrite.INIGetStringValue(configFilePath, "Others", "sort", "descend");
                if (Utils.IniReadWrite.INIGetStringValue(configFilePath, "Others", "ryElecEnable", "Disable") == "Enable") ryElecEnable = true;
                else ryElecEnable = false;
            }
            catch (Exception ex)
            {
                Utils.Logger.Sys("从配置文件读取参数过程中发生异常：" + ex.Message.ToString());
                confOK = false;
            }

            return confOK;
        }
    }


    public partial class DeviceStateM
    {
        /// <summary>
        /// relay 继电器设备
        /// </summary>
        public RelayDevice ryDevice = new RelayDevice();
        /// <summary>
        /// 主槽温控设备
        /// </summary>
        public TempDevice tpDeviceM = new TempDevice() { tpDeviceName = "主槽控温设备" };


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
        /// 实验完成后是否关闭计算机
        /// </summary>
        public bool shutDownComputer = false;
        /// <summary>
        /// 锁 - 保证自动控制流程步骤执行的时候，相应资源（例如 currentState autoStart controlFlowList etc. ）不会被访问
        /// </summary>
        public Object stepLocker = new Object();

        public ThresholdParamStruct _thresholdParameters = new ThresholdParamStruct();



        //////////////////////////////////////////
        // public interface

        /// <summary> 设置继电器设备状态 - 委托 </summary>
        private delegate void RySetHandler();
        /// <summary>读取温控设备参数 - 委托 - 用于开辟新的线程读取设备参数 </summary>
        private delegate void TempGetSetParamHandler();
        /// <summary>
        /// 设置继电器状态
        /// </summary>
        /// <param name="cmd"> 继电器命令 </param>
        /// <param name="st"> 继电器状态 </param>
        public void SetRyStatus(RelayDevice.Cmd_r cmd, bool st)
        {
            // 异步调用 RelayDevice 函数，设置继电器状态
            // 结果会触发 继电器设置事件 处理函数
            ryDevice.ryStatusToSet[(int)cmd] = st;
           // RySetHandler setRyStatus = new RySetHandler(ryDevice.UpdateStatusToDevice);
           //setRyStatus.BeginInvoke(null, null);
        }


        /// <summary>
        /// 从下位机读取温度参数
        /// </summary>
        public void ReadTpParam()
        {
            // 从硬件设备读取参数
            TempGetSetParamHandler getTempParam = new TempGetSetParamHandler(tpDeviceM.UpdateParamFromDevice);
            getTempParam.BeginInvoke(null, null);
        }


        /// <summary>
        /// 向下位机写入温度参数
        /// </summary>
        /// <param name="paramCache"> 温度参数 - 长度为 7 </param>
        public void SetTpParam(float[] paramCache)
        {
            if(paramCache.Length !=7)
            {
                _deviceErrorMonitor[ErrorCode.CodeError]++;
                Debug.WriteLine("paramCache 长度错误.");
                return;
            }

            // 将参数写入温控设备缓存
            paramCache.CopyTo(tpDeviceM.tpParamToSet, 0);

            // 向硬件设备更新参数
            TempGetSetParamHandler setTempParam = new TempGetSetParamHandler(tpDeviceM.UpdateParamToDevice);
            setTempParam.BeginInvoke(null, null);
        }


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
                _deviceErrorMonitor[ErrorCode.TemptError]++;

                Debug.WriteLine("读取主槽温度时发生错误，errorCode: " + err.ToString());
                Utils.Logger.Sys("读取主槽温度时发生错误，errorCode: " + err.ToString());
                return;
            }
            // 记录主槽温度
            Utils.Logger.TempData(tpDeviceM.temperatures.Last());


            // 读取主槽功率系数
            err = tpDeviceM.GetPowerShow(out val);
            if (err != TempProtocol.Err_t.NoError)
            {
                // 如果发生错误，则记录错误
                _deviceErrorMonitor[ErrorCode.TemptError]++;

                Debug.WriteLine("读取主槽功率时发生错误，errorCode: " + err.ToString());
                Utils.Logger.Sys("读取主槽功率时发生错误，errorCode: " + err.ToString());
                return;
            }

            return;
        }
    }
}
