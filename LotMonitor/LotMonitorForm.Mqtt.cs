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

namespace LotMonitor
{
    public partial class LotMonitorForm
    {
        private MqttClient mqttClient = null;
        private IMqttClientOptions options = null;

        private string mqttServerUrl = "183.230.40.39";
        //private string mqttServerUrl = "192.168.1.101";
        private int mqttPort = 6002;
        private string mqttDeviceApiKey = "123123123M";
        private string mqttProductId = "270595";
        private string mqttDeviceId = "542228212";
        private TimeSpan mqttTs2 = new TimeSpan(0, 1, 0);


        //
        // Topic 的构成形式为： topicDeviceId/topicData
        // 也就是说，每制作一台设备时，都会新建一个 DeviceId，该设备下，有两个子项（主题）（Data和Control）
        /// <summary>
        /// 设备序列号 - 在同一类产品中，每个单独的设备都有一个相应的编号
        /// </summary>
        private static string topicDeviceId = "lot_tst";
        /// <summary>
        /// 数据类型 - 包括 Data: 温度等数据   Control: 控制指令
        /// </summary>
        private static string topicData = "Data";
        /// <summary>
        /// 指令类型 - 包括 Data: 温度等数据   Control: 控制指令
        /// </summary>
        private static string topicCmd = "Control";

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

        bool setupMqtt()
        {
            if (mqttClient == null)
            {
                var factory = new MqttFactory();
                mqttClient = factory.CreateMqttClient() as MqttClient;

                options = new MqttClientOptionsBuilder()
                .WithTcpServer(mqttServerUrl, mqttPort)
                .WithCredentials(mqttProductId, mqttDeviceApiKey)
                //.WithClientId(Guid.NewGuid().ToString().Substring(0,10))
                .WithClientId(mqttDeviceId)
                .WithKeepAlivePeriod(mqttTs2)
                .WithCleanSession(true)
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

        public void Publish(string Message)
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

                string myTopic = topicDeviceId + "/" + topicData;

                Console.WriteLine("Publish >>Message: " + Message);
                MqttApplicationMessageBuilder mamb = new MqttApplicationMessageBuilder()
                 .WithTopic(myTopic)
                 .WithPayload(Message)
                 .WithExactlyOnceQoS()
                 .WithRetainFlag(Retained);

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


        private async Task Connected(MqttClientConnectedEventArgs e)
        {
            try
            {
                string Topic = topicDeviceId + "/" + topicData;
                var topicFilterBulder = new TopicFilterBuilder().WithTopic(Topic).Build();
                Console.WriteLine("Connected >>Subscribe " + Topic);

                await mqttClient.SubscribeAsync(topicFilterBulder);
                Console.WriteLine("Connected >>Subscribe Success");
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
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
