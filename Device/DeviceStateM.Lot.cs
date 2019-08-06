﻿using System;
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

        /// <summary>
        /// 订阅主题
        /// </summary>
        List<string> topicsSubscribeList = new List<string>() { "Control", "Status" };

        private bool Retained = false;

        /// <summary>
        /// 是否启用 mqtt monitor
        /// </summary>
        private bool monitorEnable = true;

        /// <summary>
        /// 所发布的主题
        /// </summary>
        public enum LotTopicsPublish : int
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

        private Dictionary<LotTopicsPublish, Tuple<string, int>> topicPublishDict = new Dictionary<LotTopicsPublish, Tuple<string, int>>();

        bool setupMqtt()
        {
            if(mqttClient == null)
            {
                topicPublishDict.Clear();
                foreach (LotTopicsPublish item in Enum.GetValues(typeof(LotTopicsPublish)))
                {
                    topicPublishDict.Add(item, new Tuple<string, int>(Enum.GetName(typeof(LotTopicsPublish), item), 0));
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

        /// <summary>
        /// 设置 monitor Enable 的状态
        /// </summary>
        /// <param name="st"></param>
        public void setMqttEnable(bool st)
        {
            monitorEnable = st;

            if(st) mqttClient.ConnectAsync(options);
        }


        public void Publish(LotTopicsPublish topic, string Message)
        {
            if (!monitorEnable) return;

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

                string myTopic = Model + "/" + topicPublishDict[topic].Item1 + "/" + ProductID;

                Console.WriteLine("Publish >>Topic: " + myTopic + "; QoS: " + topicPublishDict[topic].Item2 + "; Retained: " + Retained + ";");
                Console.WriteLine("Publish >>Message: " + Message);
                MqttApplicationMessageBuilder mamb = new MqttApplicationMessageBuilder()
                 .WithTopic(myTopic)
                 .WithPayload(Message).WithRetainFlag(Retained);
                if (topicPublishDict[topic].Item2 == 0)
                {
                    mamb = mamb.WithAtMostOnceQoS();
                }
                else if (topicPublishDict[topic].Item2 == 1)
                {
                    mamb = mamb.WithAtLeastOnceQoS();
                }
                else if (topicPublishDict[topic].Item2 == 2)
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
            if (!monitorEnable) return;

            try
            {
                string text = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                string Topic = e.ApplicationMessage.Topic;
                string QoS = e.ApplicationMessage.QualityOfServiceLevel.ToString();
                string Retained = e.ApplicationMessage.Retain.ToString();
                Console.WriteLine("MessageReceived >>Topic:" + Topic + "; QoS: " + QoS + "; Retained: " + Retained + ";");
                Console.WriteLine("MessageReceived >>Msg: " + text);

                switch(Topic)
                {
                    // control cmd from the monitor
                    case "LOT/Control/20190805":
                        break;

                    // update all status request from the monitor
                    case "LOT/Status/20190805":
                        Publish(LotTopicsPublish.TemptSetM, tpDeviceM.tpParam[0].ToString());
                        Publish(LotTopicsPublish.TemptSetS, tpDeviceS.tpParam[0].ToString());
                        Publish(LotTopicsPublish.FlowState, _state.ToString());
                        Publish(LotTopicsPublish.RelayM, string.Join(",", ryDeviceM.ryStatus.Select(b => b.ToString()).ToArray()));
                        Publish(LotTopicsPublish.RelayS, string.Join(",", ryDeviceS.ryStatus.Select(b => b.ToString()).ToArray()));
                        break;

                    // default
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
                    foreach(string tp in topicsSubscribeList)
                    {
                        string Topic = Model + "/" + tp + "/" + ProductID;
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
        }

        private async Task Disconnected(MqttClientDisconnectedEventArgs e)
        {
            if (!monitorEnable) return;

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
