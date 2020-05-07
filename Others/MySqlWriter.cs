using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using NLog;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Others
{
    public class MySqlWriter
    {
        private static readonly Logger nlogger = LogManager.GetCurrentClassLogger();

        private static SqlSugarClient _sqlDB;

        /// <summary>
        /// MySql 的配置文件，主要是 connecting string
        /// </summary>
        private class SqlConfig
        {
            public string server = "localhost";
            public string user = "root";
            public string pwd = "123123";
            public string database = "autotest";

            public string ConnectionString {
                get { return "server=" + server + "; user=" + user + "; pwd=" + pwd + "; database=" + database; } }
        }
        static SqlConfig _sqlConfig = new SqlConfig();

        private class RecordDefaultValue
        {
            public string name = "wghou";
        }

        public bool Init()
        {
            if (_sqlDB != null) return true;

            // json config file
            string confFile = "mysqlConfig.json";
            try
            {
                System.IO.StreamReader file = System.IO.File.OpenText(confFile);
                JsonTextReader reader = new JsonTextReader(file);
                JObject obj = (JObject)JToken.ReadFrom(reader);

                // 设置控温表
                if (obj.ContainsKey("config"))
                {
                    JObject child = (JObject)obj["config"];
                    if (child.ContainsKey("server")) _sqlConfig.server = (string)child["server"];
                    if (child.ContainsKey("user")) _sqlConfig.server = (string)child["user"];
                    if (child.ContainsKey("pwd")) _sqlConfig.server = (string)child["pwd"];
                    if (child.ContainsKey("database")) _sqlConfig.server = (string)child["database"];
                }

                if (obj.ContainsKey("RecordDefaultValue"))
                {
                    JObject child = (JObject)obj["RecordDefaultValue"];


                }
            }
            catch(Exception ex) {
                nlogger.Error("failed to load mysqlConfig.json");

                return false;
            }

            try
            {
                _sqlDB = new SqlSugarClient(new ConnectionConfig()
                {
                    ConnectionString = _sqlConfig.ConnectionString,
                    DbType = DbType.MySql,
                    IsAutoCloseConnection = true,
                    InitKeyType = InitKeyType.Attribute,
                });

                _sqlDB.Aop.OnLogExecuting = (sql, pars) =>
                {
                    Console.WriteLine(sql + "\r\n" +
                        _sqlDB.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                    Console.WriteLine();
                };

                //_sqlDB.CodeFirst.InitTables(typeof(StudentMoel));

                nlogger.Info("Init the database successfully.");
            }
            catch(Exception ex)
            {
                nlogger.Error("Init the database failed with: " + ex.Message);
                return false;
            }

            return true;
        }


        /// <summary>
        /// 向数据库插入记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool InsertValue<T>(T data) where T: mysqlData, new()
        {
            // 初始化 sql db
            if(_sqlDB == null)
            {
                if (Init() == false) return false;
            }

            // todo: 如果有重复，怎么办？
            try
            {
                _sqlDB.Insertable(data).ExecuteCommand();

                nlogger.Info("Insert value to the data base successfully.");
            }
            catch(Exception ex)
            {
                nlogger.Error("Insert value to the data base failed.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 仅仅是为了在泛函 InsertValue<T>(T data) 中添加限定
        /// </summary>
        public abstract class mysqlData{
            /// <summary>
            /// 检查数据格式等是否正确
            /// </summary>
            /// <returns></returns>
            public abstract bool CheckData();
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
            public string vStartTime {
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

            /// <summary>
            /// 检查数据格式等是否正确
            /// </summary>
            /// <returns></returns>
            public override bool CheckData() { return true; }

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

            /// <summary>
            /// 检查数据格式等是否正确
            /// </summary>
            /// <returns></returns>
            public override bool CheckData() { return true; }
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

            /// <summary>
            /// 检查数据格式等是否正确
            /// </summary>
            /// <returns></returns>
            public override bool CheckData() { return true; }
        }


        /// <summary>
        /// s_instrument 表 for mysql
        /// </summary>
        [SugarTable("s_instrumentdata")]
        public class InstrumentDataSqlrd : mysqlData
        {
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

            /// <summary>
            /// 检查数据格式等是否正确
            /// </summary>
            /// <returns></returns>
            public override bool CheckData() { return true; }

            /// <summary> </summary>
            public DateTime addTime = DateTime.Now;
            /// <summary> </summary>
            public DateTime updateTime = DateTime.Now;
        }


        /// <summary>
        /// s_standarddata 表 for mysql
        /// </summary>
        [SugarTable("s_standarddata")]
        public class StandardDataSqlrd : mysqlData
        {
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

            /// <summary>
            /// 检查数据格式等是否正确
            /// </summary>
            /// <returns></returns>
            public override bool CheckData() { return true; }

            /// <summary> </summary>
            public DateTime measureTime = DateTime.Now;
            /// <summary> </summary>
            public DateTime addTime = DateTime.Now;
            /// <summary> </summary>
            public DateTime updateTime = DateTime.Now;
        }
    }
}
