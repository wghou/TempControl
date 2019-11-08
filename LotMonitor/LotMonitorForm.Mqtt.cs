using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Packets;
using MQTTnet.Protocol;
using MQTTnet.Client.Receiving;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Connecting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UserPort;

namespace LotMonitor
{
    public partial class LotMonitorForm
    {
        private MyMqttClient _lotClient = new MyMqttClient();

        /// <summary>
        /// 是否与 mqtt server 连接成功
        /// </summary>
        public bool isMqttConnected { get { return _lotClient.isConnected; } }

        public void setMqttEnable(bool st)
        {
            _lotClient.Enabled = st;
        }

        private bool setupLotClient()
        {
            // json config file
            string confFile = @"./cfgLocal.json";
            try
            {
                System.IO.StreamReader file = System.IO.File.OpenText(confFile);
                JsonTextReader reader = new JsonTextReader(file);
                JObject obj = (JObject)JToken.ReadFrom(reader);

                _lotClient.Initialize(obj, SubTopic.Data);
                _lotClient.MessageReceievedEvent += LotClient_MessageReceievedEvent;
            }
            catch(Exception ex)
            {

            }

            return true;
        }

        /// <summary>
        /// 接收到 Server 消息
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="message"></param>
        private void LotClient_MessageReceievedEvent(SubTopic topic, string message)
        {
            switch (topic)
            {
                case SubTopic.Data:
                    JObject allData = (JObject)JsonConvert.DeserializeObject(message);
                    this.BeginInvoke(new EventHandler(delegate
                    {
                        // temptM
                        if (allData.ContainsKey("mTp"))
                        {
                            var data = JsonConvert.DeserializeObject<double>(allData["mTp"].ToString()); this.hslGaugeChart_temptM.Value = data;
                        }
                        if (allData.ContainsKey("mPw"))
                        {
                            var data = JsonConvert.DeserializeObject<float>(allData["mPw"].ToString()); this.hslGauge_powerM.Value = data;
                        }
                        if (allData.ContainsKey("mSt"))
                        {
                            var data = JsonConvert.DeserializeObject<float>(allData["mSt"].ToString()); this.hslLedDisplay_temptSetM.DisplayText = data.ToString("0.0000");
                        }

                        // temptS
                        if (allData.ContainsKey("sTp"))
                        {
                            var data = JsonConvert.DeserializeObject<double>(allData["sTp"].ToString()); this.hslGaugeChart_temptS.Value = data;
                        }
                        if (allData.ContainsKey("sPw"))
                        {
                            var data = JsonConvert.DeserializeObject<float>(allData["sPw"].ToString()); this.hslGauge_powerS.Value = data;
                        }
                        if (allData.ContainsKey("sSt"))
                        {
                            var data = JsonConvert.DeserializeObject<float>(allData["sSt"].ToString()); this.hslLedDisplay_temptSetS.DisplayText = data.ToString("0.0000");
                        }

                        // relayM
                        if(allData.ContainsKey("mRy"))
                        {
                            var data = JsonConvert.DeserializeObject<bool[]>(allData["mRy"].ToString());
                            for(int i = 0; i < 8; i++)
                            {
                                switchRyM[i].SwitchStatus = data[i];
                            }
                        }

                        // relayM
                        if (allData.ContainsKey("sRy"))
                        {
                            var data = JsonConvert.DeserializeObject<bool[]>(allData["sRy"].ToString());
                            for (int i = 0; i < 8; i++)
                            {
                                switchRyS[i].SwitchStatus = data[i];
                            }
                        }

                        // error
                        if(allData.ContainsKey("err"))
                        {
                            try
                            {
                                string[] data = allData["err"].ToString().Split(';');
                                Dictionary<string, int> err = new Dictionary<string, int>();
                                foreach(var itm in data)
                                {
                                    string[] subdata = itm.Split('=');
                                    err[subdata[0]] = int.Parse(subdata[1]);
                                }

                                this.label_err.Text = "";
                                foreach(var itm in err)
                                {
                                    this.label_err.Text += itm.Key + ": " + itm.Value.ToString() + "\n";
                                }
                            }
                            catch(Exception ex)
                            {

                            }
                        }
                    }));
                    break;

                default:
                    Console.WriteLine("Unknown message receieved.");
                    break;
            }
        }
    }
}
