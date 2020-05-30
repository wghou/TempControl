using System;
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
using System.Xml;

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

        /// <summary>
        /// configuration data of SBE37
        /// </summary>
        public class SBE37ConfigData
        {
            public string DeviceType { get; set; }

            public string SerialNumber { get; set; }

            public bool PressureInstalled { get; set; }

            public bool PumpInstalled { get; set; }

            public float MinCondFreq { get; set; }

            public string SampleMode { get; set; }

            public string SampleDataFormat { get; set; }

            public string ConductivityUnits { get; set; }

            public bool OutputPressure { get; set; }

            public bool OutputSalinity { get; set; }

            public bool OutputSV { get; set; }

            public bool OutputDepth { get; set; }

            public string Latitude { get; set; }

            public bool OutputDensity { get; set; }

            public bool TxSampleNumber { get; set; }

            public int SampleInterval { get; set; }

            public bool OutputTime { get; set; }

            public bool AutoRun { get; set; }

            public bool StoreData { get; set; }
        }

        static void Main(string[] args)
        {
#if false
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

#if false
            string CalibCoeffXml = "<ConfigurationData DeviceType='SBE37SIP-RS232' SerialNumber='03720175'>\r<PressureInstalled>yes</PressureInstalled>\r</ConfigurationData>";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(CalibCoeffXml);

            SBE37ConfigData cfgdata = new SBE37ConfigData();

            XmlNode node = xmlDoc.SelectSingleNode("ConfigurationData");

            cfgdata.DeviceType = node.Attributes["DeviceType"].InnerText;
            cfgdata.SerialNumber = node.Attributes["SerialNumber"].InnerText;

            cfgdata.PressureInstalled = Boolean.Parse(node["PressureInstalled"].InnerText);
            cfgdata.PumpInstalled = Boolean.Parse(node["PumpInstalled"].InnerText);
            cfgdata.MinCondFreq = float.Parse(node["MinCondFreq"].InnerText);
            cfgdata.SampleMode = node["SampleMode"].InnerText;
            cfgdata.SampleDataFormat = node["SampleDataFormat"].InnerText;
            cfgdata.ConductivityUnits = node["ConductivityUnits"].InnerText;
            cfgdata.OutputPressure = Boolean.Parse(node["OutputPressure"].InnerText);
            cfgdata.OutputSalinity = Boolean.Parse(node["OutputSalinity"].InnerText);
            cfgdata.OutputSV = Boolean.Parse(node["OutputSV"].InnerText);
            cfgdata.OutputDepth = Boolean.Parse(node["OutputDepth"].InnerText);
            cfgdata.Latitude = node["Latitude"].InnerText;
            cfgdata.OutputDensity = Boolean.Parse(node["OutputDensity"].InnerText);
            cfgdata.TxSampleNumber = Boolean.Parse(node["TxSampleNumber"].InnerText);
            cfgdata.SampleInterval = int.Parse(node["SampleInterval"].InnerText);
            cfgdata.OutputTime = Boolean.Parse(node["OutputTime"].InnerText);
            cfgdata.AutoRun = Boolean.Parse(node["AutoRun"].InnerText);
            cfgdata.StoreData = Boolean.Parse(node["StoreData"].InnerText);

#endif
            InstSqlrd info = new InstSqlrd();
            info.PortName = "COM11";
            info.BaudRate = 9600;
            info.InstType = TypeInst.SBE37SI;
            info.vSpecification = "SMP";

            InstSBE instDevice = new InstSBE(info);

            instDevice.InitWithInfo();

            instDevice.SetupSBE37();

            instDevice.StartMeasure();

            int i = 0;
            while (i < 30)
            {
                System.Threading.Thread.Sleep(3000);
            }

            instDevice.StartStore();

            instDevice.StopMeasure();

            return;
        }
    }
}
