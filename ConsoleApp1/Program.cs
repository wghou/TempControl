﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Others;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using InstDevice;
using Device;
using IotCS.Client;

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
#if true
            MySqlWriter writer = new MySqlWriter();
            writer.Init();
            bool rlt = true;

            // 1
            List<InstSBE37Data> lst1 = new List<InstSBE37Data>();
            InstSBE37Data tst = new InstSBE37Data();
            tst.vTestID = "ss";
            tst.vInstrumentID = "ss";
            tst.vItemType = "sss";
            tst.vTemperature = 10;
            tst.vTitularValue = 10;
            tst.vRealValue = 10;
            tst.vRawValue = 10;
            tst.vData = "ss";
            tst.addTime = DateTime.Now;
            tst.updateTime = DateTime.Now;


            lst1.Add(tst);
            rlt &= writer.InsertValue<InstSBE37Data>(tst);
            rlt &= writer.InsertValue<InstSBE37Data>(lst1);

            // 2
            List<InstSTDData> lst2 = new List<InstSTDData>();
            InstSTDData ist = new InstSTDData();
            ist.vStandardC = 10;
            lst2.Add(ist);
            rlt &= writer.InsertValue<InstSTDData>(lst2);


#endif

#if false
            List<ABase> lst = new List<ABase>();
            lst.Add(new ABase());
            lst.Add(new B());
#endif

#if false
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
#endif

            return;
        }
    }
}
