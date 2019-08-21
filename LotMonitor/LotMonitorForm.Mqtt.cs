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
using LotClient;

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
            _lotClient.Initialize(@"./lotConfig.json", MyMqttClient.SubTopic.Data);

            _lotClient.MessageReceievedEvent += LotClient_MessageReceievedEvent;

            return true;
        }

        /// <summary>
        /// 接收到 Server 消息
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="message"></param>
        private void LotClient_MessageReceievedEvent(MyMqttClient.SubTopic topic, string message)
        {
            switch (topic)
            {
                case MyMqttClient.SubTopic.Data:
                    JObject allData = (JObject)JsonConvert.DeserializeObject(message);
                    this.BeginInvoke(new EventHandler(delegate
                    {
                        double val = 0.0f;
                        if (allData.ContainsKey("mTp"))
                        {
                            if (double.TryParse(allData["mTp"].ToString(), out val)) this.hslGaugeChart_temptM.Value = val;
                        } 
                    }));
                    break;

                default:
                    Console.WriteLine("Unknown message receieved.");
                    break;
            }
        }


        private void MqttApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            try
            {
                string text = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                string Topic = e.ApplicationMessage.Topic;
                string QoS = e.ApplicationMessage.QualityOfServiceLevel.ToString();
                string Retained = e.ApplicationMessage.Retain.ToString();
                Console.WriteLine("MessageReceived >>Topic:" + Topic + "; QoS: " + QoS + "; Retained: " + Retained + ";");
                Console.WriteLine("MessageReceived >>Msg: " + text);

                JObject allData = (JObject)JsonConvert.DeserializeObject(text);

                switch (Topic)
                {
                    // control cmd from the monitor
                    case "lot_tst/Control":
                        Console.WriteLine(" control from server.");
                        break;

                    // Data from the monitor
                    case "lot_tst/Data":
                        
                        if(allData.ContainsKey("mTp"))
                        {
                            this.BeginInvoke(new EventHandler(delegate
                        {
                            double val = 0.0f;
                            if (double.TryParse(allData["mTp"].ToString(), out val)) this.hslGaugeChart_temptM.Value = val;
                        }));
                        }
                        break;

                    // default
                    default:
                        Console.WriteLine(" unknown from server.");
                        break;

                    //case "LOT/TemptM/20190805":
                    //    this.BeginInvoke(new EventHandler(delegate
                    //    {
                    //        double val = 0.0f;
                    //        if (double.TryParse(text, out val)) this.hslGaugeChart_temptM.Value = val;
                    //    }));
                    //    break;

                    //case "LOT/TemptSetM/20190805":
                    //    this.BeginInvoke(new EventHandler(delegate
                    //    {
                    //        double val = 0.0f;
                    //        if (double.TryParse(text, out val)) this.hslLedDisplay_temptSetM.DisplayText = val.ToString("0.0000");
                    //    }));
                    //    break;

                    //case "LOT/PowerM/20190805":
                    //    this.BeginInvoke(new EventHandler(delegate
                    //    {
                    //        float val = 0.0f;
                    //        if (float.TryParse(text, out val)) this.hslGauge_powerM.Value = val;
                    //    }));
                    //    break;

                    //case "LOT/TemptS/20190805":
                    //    this.BeginInvoke(new EventHandler(delegate
                    //    {
                    //        double val = 0.0f;
                    //        if (double.TryParse(text, out val)) this.hslGaugeChart_temptS.Value = val;
                    //    }));
                    //    break;

                    //case "LOT/TemptSetS/20190805":
                    //    this.BeginInvoke(new EventHandler(delegate
                    //    {
                    //        double val = 0.0f;
                    //        if (double.TryParse(text, out val)) this.hslLedDisplay_temptSetS.DisplayText = val.ToString("0.0000");
                    //    }));
                    //    break;

                    //case "LOT/PowerS/20190805":
                    //    this.BeginInvoke(new EventHandler(delegate
                    //    {
                    //        float val = 0.0f;
                    //        if (float.TryParse(text, out val)) this.hslGauge_powerS.Value = val;
                    //    }));
                    //    break;

                    //case "LOT/RelayM/20190805":
                    //    try
                    //    {
                    //        String str = text;
                    //        bool[] st = text.Split(',').Select(b => Boolean.Parse(b)).ToArray();
                    //        if (st.Length != 16) break;

                    //        this.BeginInvoke(new EventHandler(delegate
                    //        {
                    //            for (int i = 0; i < 8; i++) switchRyM[i].SwitchStatus = st[i];
                    //        }));
                            
                    //    }
                    //    catch(Exception ex)
                    //    {

                    //    }
                    //    break;

                    //case "LOT/RelayS/20190805":
                    //    try
                    //    {
                    //        String str = text;
                    //        bool[] st = text.Split(',').Select(b => Boolean.Parse(b)).ToArray();
                    //        if (st.Length != 16) break;

                    //        this.BeginInvoke(new EventHandler(delegate
                    //        {
                    //            for (int i = 0; i < 8; i++) switchRyS[i].SwitchStatus = st[i];
                    //        }));
                    //    }
                    //    catch (Exception ex)
                    //    {

                    //    }
                    //    break;
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
        }
    }
}
