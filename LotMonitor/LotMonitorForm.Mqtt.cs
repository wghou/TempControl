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

namespace LotMonitor
{
    public partial class LotMonitorForm
    {
        private MqttClient mqttClient = null;
        private IMqttClientOptions options = null;

        private string mqttServerUrl = "192.168.1.101";
        private int mqttPort = 501;
        private string mqttPassword = "public";
        private string mqttUserId = "admin";

        /// <summary>
        /// 产品型号 - 每一种的产品对应一个相应的型号
        /// </summary>
        private static string Model = "LOT";
        /// <summary>
        /// 产品编号 - 在同一类产品中，每个单独的设备都有一个相应的编号
        /// </summary>
        private static string ProductID = "20190805";

        private List<string> topicPublish = new List<string>() { "Control", "Status" };

        private bool Retained = false;

        public enum LotTopicsSubscribe : int
        {
            TemptM = 0,
            TemptSetM,
            PowerM,
            TemptS,
            TemptSetS,
            PowerS,
            FlowState,
            RelayM,
            RelayS,
            ErrStatus
        }

        private Dictionary<LotTopicsSubscribe, Tuple<string, int>> topicSubscribeDict = new Dictionary<LotTopicsSubscribe, Tuple<string, int>>();

        bool setupMqtt()
        {
            if (mqttClient == null)
            {
                topicSubscribeDict.Clear();
                foreach (LotTopicsSubscribe item in Enum.GetValues(typeof(LotTopicsSubscribe)))
                {
                    topicSubscribeDict.Add(item, new Tuple<string, int>(Enum.GetName(typeof(LotTopicsSubscribe), item), 0));
                }

                var factory = new MqttFactory();
                mqttClient = factory.CreateMqttClient() as MqttClient;

                options = new MqttClientOptionsBuilder()
                .WithTcpServer(mqttServerUrl, mqttPort)
                .WithCredentials(mqttUserId, mqttPassword)
                .WithClientId(Guid.NewGuid().ToString().Substring(0, 10))
                .Build();


                mqttClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(new Func<MqttClientConnectedEventArgs, Task>(Connected));
                mqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(new Func<MqttClientDisconnectedEventArgs, Task>(Disconnected));
                mqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(new Action<MqttApplicationMessageReceivedEventArgs>(MqttApplicationMessageReceived));

                mqttClient.ConnectAsync(options);

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 查询 mqtt 状态
        /// </summary>
        /// <returns></returns>
        public bool isMqttConnected()
        {
            if (mqttClient == null) return false;
            else return mqttClient.IsConnected;
        }

        public void Publish(string topic, string Message)
        {
            try
            {
                if (mqttClient == null) return;
                if (mqttClient.IsConnected == false)
                    mqttClient.ConnectAsync(options);

                if (mqttClient.IsConnected == false)
                {
                    Console.WriteLine("Publish >>Connected Failed! ");
                    return;
                }

                string myTopic = Model + "/" + topic + "/" + ProductID;

                Console.WriteLine("Publish >>Topic: " + myTopic + "; QoS: " + 0 + "; Retained: " + Retained + ";");
                Console.WriteLine("Publish >>Message: " + Message);
                MqttApplicationMessageBuilder mamb = new MqttApplicationMessageBuilder()
                 .WithTopic(myTopic)
                 .WithPayload(Message).WithRetainFlag(Retained);

                mamb = mamb.WithAtMostOnceQoS();
                //if (topicSubscribeDict[topic].Item2 == 0)
                //{
                //    mamb = mamb.WithAtMostOnceQoS();
                //}
                //else if (topicSubscribeDict[topic].Item2 == 1)
                //{
                //    mamb = mamb.WithAtLeastOnceQoS();
                //}
                //else if (topicSubscribeDict[topic].Item2 == 2)
                //{
                //    mamb = mamb.WithExactlyOnceQoS();
                //}

                mqttClient.PublishAsync(mamb.Build());
            }
            catch (Exception exp)
            {
                Console.WriteLine("Publish >>" + exp.Message);
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

                switch (Topic)
                {
                    case "LOT/TemptM/20190805":
                        this.BeginInvoke(new EventHandler(delegate
                        {
                            double val = 0.0f;
                            if (double.TryParse(text, out val)) this.hslGaugeChart_temptM.Value = val;
                        }));
                        break;

                    case "LOT/TemptSetM/20190805":
                        this.BeginInvoke(new EventHandler(delegate
                        {
                            double val = 0.0f;
                            if (double.TryParse(text, out val)) this.hslLedDisplay_temptSetM.DisplayText = val.ToString("0.0000");
                        }));
                        break;

                    case "LOT/PowerM/20190805":
                        this.BeginInvoke(new EventHandler(delegate
                        {
                            float val = 0.0f;
                            if (float.TryParse(text, out val)) this.hslGauge_powerM.Value = val;
                        }));
                        break;

                    case "LOT/TemptS/20190805":
                        this.BeginInvoke(new EventHandler(delegate
                        {
                            double val = 0.0f;
                            if (double.TryParse(text, out val)) this.hslGaugeChart_temptS.Value = val;
                        }));
                        break;

                    case "LOT/TemptSetS/20190805":
                        this.BeginInvoke(new EventHandler(delegate
                        {
                            double val = 0.0f;
                            if (double.TryParse(text, out val)) this.hslLedDisplay_temptSetS.DisplayText = val.ToString("0.0000");
                        }));
                        break;

                    case "LOT/PowerS/20190805":
                        this.BeginInvoke(new EventHandler(delegate
                        {
                            float val = 0.0f;
                            if (float.TryParse(text, out val)) this.hslGauge_powerS.Value = val;
                        }));
                        break;

                    case "LOT/RelayM/20190805":
                        try
                        {
                            String str = text;
                            bool[] st = text.Split(',').Select(b => Boolean.Parse(b)).ToArray();
                            if (st.Length != 16) break;

                            this.BeginInvoke(new EventHandler(delegate
                            {
                                for (int i = 0; i < 8; i++) switchRyM[i].SwitchStatus = st[i];
                            }));
                            
                        }
                        catch(Exception ex)
                        {

                        }
                        break;

                    case "LOT/RelayS/20190805":
                        try
                        {
                            String str = text;
                            bool[] st = text.Split(',').Select(b => Boolean.Parse(b)).ToArray();
                            if (st.Length != 16) break;

                            this.BeginInvoke(new EventHandler(delegate
                            {
                                for (int i = 0; i < 8; i++) switchRyS[i].SwitchStatus = st[i];
                            }));
                        }
                        catch (Exception ex)
                        {

                        }
                        break;

                    default:
                        break;
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
        }


        private async Task Connected(MqttClientConnectedEventArgs e)
        {
            try
            {
                List<TopicFilter> listTopic = new List<TopicFilter>();
                if (listTopic.Count() <= 0)
                {
                    foreach(var tp in Enum.GetValues(typeof(LotTopicsSubscribe)))
                    {
                        string Topic = Model + "/" + tp.ToString() + "/" + ProductID;
                        var topicFilterBulder = new TopicFilterBuilder().WithTopic(Topic).Build();
                        listTopic.Add(topicFilterBulder);
                        Console.WriteLine("Connected >>Subscribe " + Topic);
                    }  
                }
                await mqttClient.SubscribeAsync(listTopic.ToArray());
                Console.WriteLine("Connected >>Subscribe Success");
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }

            Publish(topicPublish[1], "requrest");
        }

        private async Task Disconnected(MqttClientDisconnectedEventArgs e)
        {
            try
            {
                Console.WriteLine("Disconnected >>Disconnected Server");
                await Task.Delay(TimeSpan.FromSeconds(2));
                try
                {
                    await mqttClient.ConnectAsync(options);
                }
                catch (Exception exp)
                {
                    Console.WriteLine("Disconnected >>Exception " + exp.Message);
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
        }
    }
}
