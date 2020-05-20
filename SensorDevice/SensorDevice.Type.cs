using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Others;
using SqlSugar;

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
    public abstract class SensorDataBase : mysqlData
    {
        /// <summary> 设备类型 </summary>
        public SensorType sensorType { get; protected set; } = SensorType.Undefined;
        /// <summary>
        /// 当前传感器设备的编号，范围 0 ～ maxSensorNum - 1（值为5）
        /// </summary>
        public int sensorIdx { get; set; } = -1;
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
    /// s_testorder 表中的记录 record for mysql
    /// </summary>
    [SugarTable("s_testorder")]
    public class TestOrderSqlrd : mysqlData
    {
        /// <summary> </summary>
        public string vTestID { get; set; }

        /// <summary> </summary>
        public string vStartTime
        {
            get { return startTime.ToString("yyyy-MM-dd HH:mm:ss"); }
            set { startTime = Convert.ToDateTime(value); }
        }

        /// <summary> </summary>
        public string vEndTime
        {
            get { return endTime.ToString("yyyy-MM-dd HH:mm:ss"); }
            set { endTime = Convert.ToDateTime(value); }
        }

        /// <summary> </summary>
        public string vStatus { get; set; }

        /// <summary> </summary>
        public string vPlace { get; set; }

        /// <summary> </summary>
        public float vTemperature { get; set; }

        /// <summary> </summary>
        public float vHumidity { get; set; }

        /// <summary> </summary>
        public string vCharger { get; set; }

        /// <summary> </summary>
        public string vAddTime
        {
            get { return addTime.ToString("yyyy-MM-dd HH:mm:ss"); }
            set { addTime = Convert.ToDateTime(value); }
        }

        /// <summary> </summary>
        public string vUpdateTime
        {
            get { return updateTime.ToString("yyyy-MM-dd HH:mm:ss"); }
            set { updateTime = Convert.ToDateTime(value); }
        }

        /// <summary> </summary>
        public DateTime startTime = DateTime.Now;
        /// <summary> </summary>
        public DateTime endTime = DateTime.Now;
        /// <summary> </summary>
        public DateTime addTime = DateTime.Now;
        /// <summary> </summary>
        public DateTime updateTime = DateTime.Now;
    }


    /// <summary>
    /// s_instrument 表 for mysql
    /// </summary>
    [SugarTable("s_instrument")]
    public class InstrumentSqlrd : mysqlData
    {
        /// <summary> </summary>
        public string vInstrumentID { set; get; }

        /// <summary> </summary>
        public string vTestID { set; get; }

        /// <summary> </summary>
        public string vCustomer { set; get; }

        /// <summary> </summary>
        public string vDesignation { set; get; }

        /// <summary> </summary>
        public string vSpecification { set; get; }

        /// <summary> </summary>
        public string vSN { set; get; }

        /// <summary> </summary>
        public string vManufacture { set; get; }

        /// <summary> </summary>
        public string vTestItem { set; get; }

        /// <summary> </summary>
        public string vTestType { set; get; }
    }


    /// <summary>
    /// s_sensor 表 for mysql
    /// </summary>
    [SugarTable("s_sensor")]
    public class SensorSqlrd : mysqlData
    {
        /// <summary> </summary>
        public string vSensorID { set; get; }

        /// <summary> </summary>
        public string vInstrumentID { set; get; }

        /// <summary> </summary>
        public string vSensorName { set; get; }

        /// <summary> </summary>
        public string vSensorSN { set; get; }

        /// <summary> </summary>
        public string vSensorType { set; get; }

        /// <summary> </summary>
        public string vTestItem { set; get; }
    }

    /// <summary>
    /// 未知的数据类型
    /// </summary>
    public class UndefinedSensorData : SensorDataBase {
        public UndefinedSensorData() { sensorType = SensorType.Undefined; }
    }


    /// <summary>
    /// s_instrument 表 for mysql
    /// </summary>
    [SugarTable("s_instrumentdata")]
    public class SensorSBE37Data : SensorDataBase
    {
        public SensorSBE37Data() { sensorType = SensorType.SBE37SI; }

        /// <summary> </summary>
        public string vTestID { set; get; }

        /// <summary> </summary>
        public string vInstrumentID { set; get; }

        /// <summary> </summary>
        public string vItemType { set; get; }

        /// <summary> </summary>
        public double vTemperature { set; get; }

        /// <summary> </summary>
        public int vTitularValue { set; get; }

        /// <summary> </summary>
        public double vRealValue { set; get; }

        /// <summary> </summary>
        public double vRawValue { set; get; }

        /// <summary> </summary>
        public string vData { set; get; }

        /// <summary> </summary>
        public string vAddTime
        {
            get { return addTime.ToString("yyyy-MM-dd HH:mm:ss"); }
            set { addTime = Convert.ToDateTime(value); }
        }

        /// <summary> </summary>
        public string vUpdateTime
        {
            get { return updateTime.ToString("yyyy-MM-dd HH:mm:ss"); }
            set { updateTime = Convert.ToDateTime(value); }
        }

        /// <summary> </summary>
        public DateTime addTime = DateTime.Now;
        /// <summary> </summary>
        public DateTime updateTime = DateTime.Now;
    }


    /// <summary>
    /// s_standarddata 表 for mysql
    /// </summary>
    [SugarTable("s_standarddata")]
    public class SensorSTDData : SensorDataBase
    {
        public SensorSTDData() { sensorType = SensorType.Standard; }

        /// <summary> </summary>
        public string vTestID { set; get; }

        /// <summary> </summary>
        public int vTitularValue { set; get; }

        /// <summary> </summary>
        public double vStandardT { set; get; }

        /// <summary> </summary>
        public double vStandardC { set; get; }

        // <summary> </summary>
        public string vMeasureTime
        {
            get { return measureTime.ToString("yyyy-MM-dd HH:mm:ss"); }
            set { measureTime = Convert.ToDateTime(value); }
        }

        /// <summary> </summary>
        public string vAddTime
        {
            get { return addTime.ToString("yyyy-MM-dd HH:mm:ss"); }
            set { addTime = Convert.ToDateTime(value); }
        }

        /// <summary> </summary>
        public string vUpdateTime
        {
            get { return updateTime.ToString("yyyy-MM-dd HH:mm:ss"); }
            set { updateTime = Convert.ToDateTime(value); }
        }

        /// <summary> </summary>
        public DateTime measureTime = DateTime.Now;
        /// <summary> </summary>
        public DateTime addTime = DateTime.Now;
        /// <summary> </summary>
        public DateTime updateTime = DateTime.Now;
    }
}
