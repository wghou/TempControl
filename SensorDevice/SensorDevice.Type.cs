using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorDevice
{
    /// <summary>
    /// 传感器设备的类型
    /// </summary>
    public enum SensorType : int
    {
        SBE37SI = 0,
        SBE37SIP,
        SBE37SM,
        SBE37SMP,
        SBE37SMPODO,
        Undefined,
        Standard
    }

    /// <summary>
    /// 传感器的错误状态
    /// </summary>
    public enum Err_sr : int
    {
        NoError = 0,
        Error
    }

    /// <summary>
    /// 传感器数据的基类
    /// </summary>
    public abstract class SensorDataBase {
        /// <summary> 设备类型 </summary>
        public SensorType sensorType { get; protected set; } = SensorType.Undefined;
    }

    /// <summary>
    /// 设备硬件信息的基类
    /// </summary>
    public abstract class SensorInfoBase {
        /// <summary> 设备类型 </summary>
        public SensorType sensorType { get; set; } = SensorType.Undefined;
        /// <summary>
        /// 当前传感器设备的编号，范围 0 ～ maxSensorNum - 1（值为5）
        /// </summary>
        public int sensorIdx { get; set; } = -1;
    }

    /// <summary>
    /// 设备硬件信息
    /// </summary>
    public class SensorInfo : SensorInfoBase
    {
        /// <summary> 设备型号名称 </summary>
        public string typeName { get; set; }
        /// <summary> xx 版本 </summary>
        public string version { get; set; }
        /// <summary> 序列号 </summary>
        public string serialNo { get; set; }
    }

    /// <summary>
    /// 未知的数据类型
    /// </summary>
    public class UndefinedSensorData : SensorDataBase {
        public UndefinedSensorData() { sensorType = SensorType.Undefined; }
    }

    /// <summary>
    /// 标准传感器设备的数据
    /// </summary>
    public class StandardDeviceData : SensorDataBase
    {
        public StandardDeviceData() { sensorType = SensorType.Standard; }

        /// <summary> 时间字段 </summary>
        public DateTime dateTime { get; set; }

        /// <summary> 电导率频率 </summary>
        public float freqC { set; get; }

        /// <summary> 标准电导率示值 </summary>
        public float conduct { set; get; }

        /// <summary> 温度频率 </summary>
        public float freqT { set; get; }

        /// <summary> 标准温度示值 </summary>
        public float tempt { set; get; }

        /// <summary> 盐度 </summary>
        public float salt { set; get; }

        /// <summary> 标志树 </summary>
        public int count { set; internal get; }
    }

    /// <summary>
    /// 某传感器的数值
    /// </summary>
    public class SensorDeviceData : SensorDataBase
    {
        public SensorDeviceData() { sensorType = SensorType.SBE37SI; }

        /// <summary> 温度 </summary>
        public float tempt { set; get; }
        /// <summary> 电导率 </summary>
        public float conduct { set; get; }
        /// <summary>  pressure sensor pressure counts </summary>
        public float valA { set; get; }
        /// <summary> pressure sensor temperature compensation </summary>
        public float valB { set; get; }
    }
}
