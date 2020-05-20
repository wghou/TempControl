using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Others;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SensorDevice;
using Device;
using IotPort;

namespace ConsoleApp1
{
    class Program
    {
        class ABase
        {
            public int type { set; get; }
        }

        class B : ABase
        {
            public int idx { set; get; }
        }

        class Data
        {
            public List<ABase> data { get; set; }
        }

        static void Main(string[] args)
        {
#if false
            MySqlWriter writer = new MySqlWriter();

            writer.Init();

            // 1
            MySqlWriter.TestOrderSqlrd tst = new MySqlWriter.TestOrderSqlrd();
            tst.vTestID = "id";
            tst.vStatus = "correct";
            tst.vPlace = "124";
            tst.vTemperature = 12.0f;
            tst.vHumidity = 12.0f;
            tst.vCharger = "wghou";
            if(tst.CheckData()) writer.InsertValue(tst);

            // 2
            MySqlWriter.InstrumentSqlrd ist = new MySqlWriter.InstrumentSqlrd();
            ist.vInstrumentID = "123-123";
            ist.vTestID = "20200507-123";
            ist.vCustomer = "侯文国";
            ist.vDesignation = "CTD";
            ist.vSpecification = "sbe 37";
            ist.vSN = "1";
            ist.vManufacture = "美国";
            ist.vTestItem = "ss";
            ist.vTestType = "计量站TCS校准";
            if(ist.CheckData()) writer.InsertValue(ist);

            // 3
            MySqlWriter.SensorSqlrd sr = new MySqlWriter.SensorSqlrd();
            sr.vSensorID = "123";
            sr.vInstrumentID = "123";
            sr.vSensorName = "12";
            sr.vSensorSN = "123";
            sr.vSensorType = "nuss";
            sr.vTestItem = "ss";
            if(sr.CheckData()) writer.InsertValue(sr);

            // 4
            MySqlWriter.InstrumentDataSqlrd instData = new MySqlWriter.InstrumentDataSqlrd();
            instData.vTestID = "12";
            instData.vInstrumentID = "123";
            instData.vItemType = "ss";
            instData.vTemperature = 2.0f;
            instData.vTitularValue = 3;
            instData.vRealValue = 12.3;
            instData.vRawValue = 23.3;
            instData.vData = "ssssss";
            if(instData.CheckData()) writer.InsertValue(instData);

            // 5
            MySqlWriter.StandardDataSqlrd sdData = new MySqlWriter.StandardDataSqlrd();
            sdData.vTestID = "123";
            sdData.vTitularValue = 2;
            sdData.vStandardT = 3.9;
            sdData.vStandardC = 3.9;
            if(sdData.CheckData()) writer.InsertValue(sdData);

#endif

#if false
            List<ABase> lst = new List<ABase>();
            lst.Add(new ABase());
            lst.Add(new B());
#endif
            IotSensorValueMessage jSensorVal = new IotSensorValueMessage();
            jSensorVal.DorS = IotDorS.Display;
            jSensorVal.Topic = IotTopic.SensorValue;
            jSensorVal.SensorData = new List<SensorDataBase>();

            SensorSTDData dt1 = new SensorSTDData();
            jSensorVal.SensorData.Add(dt1);

            SensorSTDData dt2 = new SensorSTDData();
            jSensorVal.SensorData.Add(dt2);

            JObject message = JObject.FromObject(jSensorVal);

            IotSensorValueMessage srVal = new IotSensorValueMessage();
            srVal = JsonConvert.DeserializeObject<IotSensorValueMessage>(message.ToString(), new JsonSensorDataConverter());


            return;
        }
    }
}
