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


namespace Device
{
    public partial class DeviceStateM
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


        private string Topic = Model + "/" + "Control" + ProductID;

        private bool Retained = false;

        public enum LotTopics : int
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

        private Dictionary<LotTopics, Tuple<string, int>> topicDict = new Dictionary<LotTopics, Tuple<string, int>>();

        bool setupMqtt()
        {
            if(mqttClient == null)
            {
                topicDict.Clear();
                foreach (LotTopics item in Enum.GetValues(typeof(LotTopics)))
                {
                    topicDict.Add(item, new Tuple<string, int>(Enum.GetName(typeof(LotTopics), item), 0));
                }

                var factory = new MqttFactory();
                mqttClient = factory.CreateMqttClient() as MqttClient;

                options = new MqttClientOptionsBuilder()
                .WithTcpServer(mqttServerUrl, mqttPort)
                .WithCredentials(mqttUserId, mqttPassword)
                .WithClientId(Guid.NewGuid().ToString().Substring(0,10))
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

        public void Publish(LotTopics topic, string Message)
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

                string myTopic = Model + "/" + topicDict[topic].Item1 + "/" + ProductID;

                Console.WriteLine("Publish >>Topic: " + myTopic + "; QoS: " + topicDict[topic].Item2 + "; Retained: " + Retained + ";");
                Console.WriteLine("Publish >>Message: " + Message);
                MqttApplicationMessageBuilder mamb = new MqttApplicationMessageBuilder()
                 .WithTopic(myTopic)
                 .WithPayload(Message).WithRetainFlag(Retained);
                if (topicDict[topic].Item2 == 0)
                {
                    mamb = mamb.WithAtMostOnceQoS();
                }
                else if (topicDict[topic].Item2 == 1)
                {
                    mamb = mamb.WithAtLeastOnceQoS();
                }
                else if (topicDict[topic].Item2 == 2)
                {
                    mamb = mamb.WithExactlyOnceQoS();
                }

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
                    var topicFilterBulder = new TopicFilterBuilder().WithTopic(Topic).Build();
                    listTopic.Add(topicFilterBulder);
                    Console.WriteLine("Connected >>Subscribe " + Topic);
                }
                await mqttClient.SubscribeAsync(listTopic.ToArray());
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
