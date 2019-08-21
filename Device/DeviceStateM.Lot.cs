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


namespace Device
{
    public partial class DeviceStateM
    {
        private MqttClient mqttClient = null;
        private IMqttClientOptions options = null;

        private string mqttServerUrl = "183.230.40.39";
        //private string mqttServerUrl = "192.168.1.101";
        private int mqttPort = 6002;
        private string mqttDeviceApiKey = "123123123";
        private string mqttProductId = "270595";
        private string mqttDeviceId = "542169339";
        private TimeSpan mqttTs2 = new TimeSpan(0,1,0);

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

        /// <summary>
        /// 设置 monitor Enable 的状态
        /// </summary>
        /// <param name="st"></param>
        public void setMqttEnable(bool st)
        {
            monitorEnable = st;

            if(st) mqttClient.ConnectAsync(options);
        }

        public string packageDataJson()
        {
            // 所有数据
            JObject allData = new JObject();

            // 主槽温度
            JProperty mTp = new JProperty("mTp",tpDeviceM.temperatures.Last());
            // 主槽功率
            JProperty mPw = new JProperty("mPw", tpDeviceM.tpPowerShow);
            // 主槽设定值
            JProperty mSt = new JProperty("mSt", tpDeviceM.tpParam[0]);
            // 添加
            allData.Add(mTp); allData.Add(mPw); allData.Add(mSt);

            // 辅槽温度
            JProperty sTp = new JProperty("sTp", tpDeviceS.temperatures.Last());
            // 辅槽功率
            JProperty sPw = new JProperty("sPw", tpDeviceS.tpPowerShow);
            // 辅槽设定值
            JProperty sSt = new JProperty("sSt", tpDeviceS.tpParam[0]);
            // 添加
            allData.Add(sTp); allData.Add(sPw); allData.Add(sSt);

            // 继电器 M
            JProperty mRy = new JProperty("mRy", ryDeviceM.ryStatus);
            // 继电器 S
            JProperty sRy = new JProperty("sRy", ryDeviceS.ryStatus);
            // 添加
            allData.Add(mRy); allData.Add(sRy);

            // 错误状态
            //JProperty err = new JProperty("err", _deviceErrorMonitor);
            // 添加
            //allData.Add(err);

            return allData.ToString();
        }


        public void Publish(string Message)
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
                    case "lot_tst/Control":
                        Console.WriteLine(" control from server.");
                        break;

                    // Data from the monitor
                    case "lot_tst/Data":
                        Console.WriteLine(" Data from server.");
                        break;

                    // default
                    default:
                        Console.WriteLine(" unknown from server.");
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
                string Topic = topicDeviceId + "/" + topicCmd;
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
