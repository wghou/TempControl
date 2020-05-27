﻿using System;
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

    /// 说明：定义 InfoBase 的用处
    /// 1、可以作为基类，实现多态
    /// 2、在存放信息的地方，定义 List<InfoBase> ，可以满足不同类型信息的存放
    /// 3、可以将 SqlData 中的数据与 Info 中的数据隔绝开来。也就是说，从数据
    /// 库中读取的数据，放在 SqlData 部分，而一些常用、共有的信息，定义在 Info 中。

    /// <summary>
    /// Info 类的公共接口
    /// 在 sql 类中，部分属性是直接从数据库读写的，但有一部分成员函数是手动修改的。
    /// 因此，便需要根据 sql 数据，将这部分手动数据进行更新
    /// </summary>
    interface ISql2InfoBase
    {
        /// <summary>
        /// 根据 Sql 数据（一般为属性）刷新 Info 数据（一般为成员变量）
        /// </summary>
        /// <returns></returns>
        bool FreshFromSql2Info();
        /// <summary>
        /// 根据 Info 数据（一般为成员变量）刷新 Sql 数据（一般为属性）
        /// </summary>
        /// <returns></returns>
        /// bool FreshFromInfo2Sql();
    }


    /// <summary>
    /// Data 类的公共接口
    /// 在 sql 类中，部分属性是直接从数据库读写的，但有一部分成员函数是手动修改的。
    /// 因此，便需要根据 data 数据，将这部分sql 数据进行更新
    /// </summary>
    interface IData2SqlBase
    {
        /// <summary>
        /// 根据 Data 数据（一般为成员变量）刷新 Sql 数据（一般为属性）
        /// </summary>
        /// <returns></returns>
        bool FreshFromData2Sql();
    }


    /// <summary>
    /// 设备硬件信息的基类
    /// </summary>
    public class InstInfoBase : mysqlData, ISql2InfoBase
    {
        /// <summary> 设备类型 </summary>
        public TypeInst InstType = TypeInst.Undefined;
        /// <summary>
        /// 当前仪器设备的编号，范围 0 ～ maxSensorNum - 1（值为5）
        /// 暂时不用！！
        /// </summary>
        public int InstIdx_NotUsed = -1;
        /// <summary>
        /// 包含仪器标志位
        /// </summary>
        public TypeSensor SensorFlag = TypeSensor.None;
        /// <summary>
        /// 测试 id
        /// </summary>
        public string testId = "";
        /// <summary>
        /// 仪器 id
        /// </summary>
        public string instrumentId = "";
        /// <summary>
        /// 设备的端口号
        /// </summary>
        public string PortName = "COM";
        /// <summary>
        /// 设备的端口波特率
        /// </summary>
        public int BaudRate = 9600;

        /// <summary>
        /// 根据 Sql 数据（一般为属性）刷新 Info 数据（一般为成员变量）
        /// </summary>
        /// <returns></returns>
        public virtual bool FreshFromSql2Info() { return true; }
    }

    /// <summary>
    /// 设备中传感器信息的基类
    /// </summary>
    public class SensorInfoBase : mysqlData, ISql2InfoBase
    {
        /// <summary>
        /// 传感器类型
        /// </summary>
        public TypeSensor SensorType = TypeSensor.None;
        /// <summary>
        /// 传感器所在仪器的编号
        /// 暂时不用！！
        /// </summary>
        public int InstIdx_NotUsed = -1;

        /// <summary>
        /// 根据 Sql 数据（一般为属性）刷新 Info 数据（一般为成员变量）
        /// </summary>
        /// <returns></returns>
        public virtual bool FreshFromSql2Info() { return true; }
    }

    /// <summary>
    /// 仪器数据的基类
    /// </summary>
    public abstract class InstDataBase : mysqlData, IComparable, IData2SqlBase
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
        /// <summary>
        /// 根据 Data 数据（一般为成员变量）刷新 Sql 数据（一般为属性）
        /// </summary>
        /// <returns></returns>
        public abstract bool FreshFromData2Sql();
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
    public class InstSqlrd : InstInfoBase
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

        /// <summary> 仪器上挂载的传感器 </summary>
        public List<SensorInfoBase> sensors = new List<SensorInfoBase>();


        /// <summary>
        /// 根据 Sql 数据（一般为属性）刷新 Info 数据（一般为成员变量）
        /// </summary>
        /// <returns></returns>
        public override bool FreshFromSql2Info()
        {
            // 更新设备类型
            // todo: 如何辨识设备类型
            if(vSpecification.Contains("SMP"))
            {
                InstType = TypeInst.SBE37SMP;
            }
            else if(vSpecification.Contains("SM"))
            {
                InstType = TypeInst.SBE37SM;
            }
            else
            {
                InstType = TypeInst.Undefined;
            }

            // 更新传感器类型
            foreach(var itm in sensors)
            {
                itm.FreshFromSql2Info();
                SensorFlag |= itm.SensorType;
            }

            testId = vTestID;
            instrumentId = vInstrumentID;

            return true;
        }
    }


    /// <summary>
    /// s_sensor 表 for mysql
    /// </summary>
    [SugarTable("s_sensor")]
    public class SensorSqlrd : SensorInfoBase
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

        /// <summary>
        /// 根据 Sql 数据（一般为属性）刷新 Info 数据（一般为成员变量）
        /// </summary>
        /// <returns></returns>
        public override bool FreshFromSql2Info()
        {
            if(vSensorType == "C")
            {
                SensorType = TypeSensor.Conduct;
            }
            else if(vSensorType == "T")
            {
                SensorType = TypeSensor.Tempt;
            }
            else
            {
                SensorType = TypeSensor.None;
            }

            return true;
        }
    }

    /// <summary>
    /// 未知的数据类型
    /// </summary>
    public class InstUDFData : InstDataBase {
        public InstUDFData() { InstType = TypeInst.Undefined; }
        
        /// <summary>
        /// 根据 Data 数据（一般为成员变量）刷新 Sql 数据（一般为属性）
        /// </summary>
        /// <returns></returns>
        public override bool FreshFromData2Sql()
        {
            return true;
        }
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

        /// <summary>
        /// 根据 Data 数据（一般为成员变量）刷新 Sql 数据（一般为属性）
        /// </summary>
        /// <returns></returns>
        public override bool FreshFromData2Sql()
        {
            throw new NotImplementedException();
        }
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
        /// 根据 Data 数据（一般为成员变量）刷新 Sql 数据（一般为属性）
        /// </summary>
        /// <returns></returns>
        public override bool FreshFromData2Sql()
        {
            throw new NotImplementedException();
        }

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
