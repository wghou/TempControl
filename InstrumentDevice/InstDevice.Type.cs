using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Others;
using SqlSugar;

namespace InstDevice
{
    /// <summary>
    /// 仪器设备的类型
    /// </summary>
    public enum TypeInst : int
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
    /// 仪器标志位
    /// </summary>
    [Flags]
    public enum TypeSensor : int
    {
        None = 0,
        /// <summary> 温度 </summary>
        Tempt = 1,
        /// <summary> 电导率 </summary>
        Conduct = 2
    }


    /// <summary>
    /// 仪器的错误状态
    /// </summary>
    public enum Err_sr : int
    {
        NoError = 0,
        Error
    }

    /// <summary>
    /// 仪器数据的基类
    /// </summary>
    public abstract class InstDataBase : mysqlData, IComparable
    {
        /// <summary> 设备类型 </summary>
        public TypeInst InstType = TypeInst.Undefined;
        /// <summary>
        /// 当前仪器设备的编号，范围 0 ～ maxInstNum - 1（值为5）
        /// </summary>
        public int InstIdx = -1;

        /// <summary>
        /// 比较函数
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual int CompareTo(object obj) { return 0; }
    }

    /// <summary>
    /// 设备硬件信息的基类
    /// </summary>
    public abstract class InstInfoBase {
        /// <summary> 设备类型 </summary>
        public TypeInst InstType { get; set; } = TypeInst.Undefined;
        /// <summary>
        /// 当前仪器设备的编号，范围 0 ～ maxSensorNum - 1（值为5）
        /// </summary>
        public int InstIdx { get; set; } = -1;
    }

    /// <summary>
    /// 设备硬件信息
    /// </summary>
    public class InstInfo : InstInfoBase
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
    public class InstSqlrd : mysqlData
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
    public class InstUDFData : InstDataBase {
        public InstUDFData() { InstType = TypeInst.Undefined; }
    }


    /// <summary>
    /// s_instrument 表 for mysql
    /// </summary>
    [SugarTable("s_instrumentdata")]
    public class InstSBE37Data : InstDataBase
    {
        public InstSBE37Data() { InstType = TypeInst.SBE37SI; }

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
    public class InstSTDData : InstDataBase
    {
        public InstSTDData() { InstType = TypeInst.Standard; }

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

        /// <summary>
        /// 比较函数
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override int CompareTo(object obj) {
            if (obj == null) return 1;
            InstSTDData other = obj as InstSTDData;
            if(vStandardT > other.vStandardT) { return 1; }
            else if(vStandardT == other.vStandardT) { return 0; }
            else { return -1; }
        }
    }
}
