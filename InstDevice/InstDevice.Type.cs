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
    /// 仪器的采样方式 / 数据读取方式
    /// </summary>
    public enum InstSampleMode : int
    {
        /// <summary> 自动采样 - 格式 0 </summary>
        AutoSample_Fmt0 = 0,
        /// <summary> 自动采样 - 格式 1 </summary>
        AutoSample_Fmt1,
        /// <summary> 轮询采样 - 格式 0 </summary>
        PolledSample_Fmt0,
        /// <summary> 轮询采样 - 格式 1 </summary>
        PolledSample_Fmt1,
        /// <summary> 轮询采样 - 格式 1 + 格式 0 </summary>
        PolledSample_Fmt10
    }

    /// <summary>
    /// 仪器中传感器的标志位 - OutputFormat=0: raw decimal data, for diagnostic use at Sea-Bird
    /// tttttt, cccc.ccc, pppppp, vvvv, dd mmm yyyy, hh:mm:ss（有的略有差异）
    /// </summary>
    [Flags]
    public enum TypeSensorFmt0 : uint
    {
        /// <summary> 未定义传感器类型 </summary>
        None = 0,
        /// <summary> tttttt = temperature A/D counts. </summary>
        tt = 0x00000001,
        /// <summary> ccccc.ccc = conductivity frequency (Hz). </summary>
        cc = 0x00000002,
        /// <summary> pppppp = pressure sensor pressure A/D counts; sent if optional pressure sensor installed. </summary>
        pp = 0x00000004,
        /// <summary> vvvv = pressure sensor pressure temperature compensation A/D counts; sent if optional pressure sensor installed. </summary>
        vv = 0x00000008,
        /// <summary> oo.ooo = oxygen sensor phase (µsec). </summary>
        oo = 0x00000010,
        /// <summary> t.tttttt = oxygen sensor temperature voltage. </summary>
        ot = 0x00000020,
        /// <summary> dd mmm yyyy = day, month, year. </summary>
        dm = 0x00000040,
        /// <summary> hh:mm:ss = hour, minute, second. </summary>
        hm = 0x00000080
    }

    /// <summary>
    /// 仪器中传感器的标志位 - OutputFormat=1: converted decimal data
    /// tttt.tttt,ccc.ccccc,ppppp.ppp,ssss.ssss,vvvvv.vvv,ccc.ccccc,dd mmm yyyy, hh:mm:ss
    /// </summary>
    [Flags]
    public enum TypeSensorFmt1 : uint
    {
        /// <summary> 未定义传感器类型 </summary>
        None = 0,
        /// <summary> tttt.tttt = temperature (°C, ITS-90). </summary>
        tt = 0x00000001,
        /// <summary> ccccc.ccc = conductivity (S/m). </summary>
        cc = 0x00000002,
        /// <summary> ppppp.ppp = pressure (decibars); sent only if pressure sensor installed. </summary>
        pp = 0x00000004,
        /// <summary> dddd.ddd = depth (meters); sent only if OutputDepth=Y. </summary>
        dd = 0x00000008,
        /// <summary> oo.ooo = oxygen (sent if OutputOx=Y; units defined by SetOxUnits =). </summary>
        oo = 0x00000010,
        /// <summary> ssss.ssss = salinity (psu); sent only if OutputSal=Y. </summary>
        ss = 0x00000020,
        /// <summary> vvvv.vvv = sound velocity (meters/second); sent only if OutputSV=Y. </summary>
        vv = 0x00000040,
        /// <summary> rrr.rrrr = local density (kg/m3); sent only if OutputDensity=Y. </summary>
        rr = 0x00000080,
        /// <summary> ccc.ccccc = Specific Conductivity (S/m) </summary>
        sc = 0x00000100,
        /// <summary> x = specific conductivity; sent if OutputSC=Y </summary>
        xx = 0x00000200,
        /// <summary> dd mmm yyyy = day, month, year. </summary>
        dm = 0x00000400,
        /// <summary> hh:mm:ss = hour, minute, second. </summary>
        hm = 0x00000800,
        /// <summary> n = sample number in FLASH memory (sent if TxSampleNum=y, </summary>
        nn = 0x00001000
    }

    /// <summary>
    /// 
    /// </summary>
    public static class SBE37SensorType
    {
        /// <summary> SBE37SI </summary>
        public static readonly TypeSensorFmt0 SBE37SI_Fmt0 = TypeSensorFmt0.tt | TypeSensorFmt0.cc;
        /// <summary> SBE37SI </summary>
        public static readonly TypeSensorFmt1 SBE37SI_Fmt1 = TypeSensorFmt1.tt | TypeSensorFmt1.cc;


    }

    /// <summary>
    /// 读取数据的返回格式
    /// </summary>
    public enum SBE37OutputFormat : int
    {
        Format_0 = 0,
        Format_1 = 1,
        Format_2 = 2,
        Format_3 = 3
    }

    /// <summary>
    /// SBE37 采样命令
    /// </summary>
    internal enum SBE37Cmd : int
    {
        NoneCmd = 0,
        OutputExecutedFlag = 1,
        Dc = 2,
        InitCmds = 3,
        Start = 4,
        Stop = 5,
        Tc = 6,
        Tcr = 7
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
        /// 包含仪器（数据）标志位 - format_0
        /// </summary>
        public TypeSensorFmt0 SensorFlagFmt0 = TypeSensorFmt0.None;
        /// <summary>
        /// 包含仪器（数据）标志位 - format_1
        /// </summary>
        public TypeSensorFmt1 SensorFlagFmt1 = TypeSensorFmt1.None;
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
        public TypeSensorFmt1 SensorType = TypeSensorFmt1.None;
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
                SensorFlagFmt1 |= itm.SensorType;
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
                SensorType = TypeSensorFmt1.cc;
            }
            else if(vSensorType == "T")
            {
                SensorType = TypeSensorFmt1.tt;
            }
            else
            {
                SensorType = TypeSensorFmt1.None;
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
