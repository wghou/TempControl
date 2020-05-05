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
using NLog;

namespace IotPort
{
    public partial class IotPorts
    {
        public bool configUserPorts(JObject cfg, params Topic[] topicsSub)
        {
            bool confOK = true;

            try
            {
                // 配置用户接口 - MqttCloud
                if (cfg.ContainsKey("MqttCloud"))
                {
                    JObject child = (JObject)cfg["MqttCloud"];

                    if (child.ContainsKey("Enable") ? (bool)child["Enable"] : true)
                    {
                        confOK &= _mqttCloud.Initialize(child, topicsSub);
                        _mqttCloud.MessageReceievedEvent += _mqttCloud_MessageReceievedEvent;

                        if (!confOK) nlogger.Error("配置 MqttCloud 失败");
                        else nlogger.Debug("配置 MqttCloud 成功");
                    }  
                }
            }
            catch(Exception ex)
            {

                confOK = false;
            }

            return confOK;
        }


        /// <summary>
        /// 检查接口是否连接/启用
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool isConnected()
        {
            return _mqttCloud.isConnected;
        }

        //
        public delegate void UserPortMessageReceievedEventHandler(Topic topic, JObject message);
        /// <summary>
        /// 事件 - 接收到需要显示的消息
        /// </summary>
        public event UserPortMessageReceievedEventHandler IotPortRvMsgDisplayEvent;
        /// <summary>
        /// 事件 - 接收到需要执行的消息
        /// </summary>
        public event UserPortMessageReceievedEventHandler IotPortRvMsgSetEvent;

        /// <summary>
        /// 向特定接口发布消息
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="Message"></param>
        /// <param name="isWait"></param>
        /// <param name="ports"></param>
        /// <returns></returns>
        public bool PublishMessage(Topic topic, string message, bool isWait = false)
        {
            _mqttCloud.Publish(topic, message, isWait);
            return true;
        }

        // receive message from the mqtt cloud
        private void _mqttCloud_MessageReceievedEvent(Topic topic, string message)
        {
            // 初步解析 message，并根据 d_s 标志位判断是显示类型的消息，还是设置类型的消息
            // 分别调用各自的事件函数
            JObject jo = (JObject)JsonConvert.DeserializeObject(message);

            if (!jo.ContainsKey("d_s"))
            {
                nlogger.Error("bad message from mqtt. no d_s identifier.");
                return;
            }

            DorS d_s;
            if(!Enum.TryParse(jo["d_s"].ToString(), out d_s))
            {
                nlogger.Error("bad message from mqtt. no d_s identifier.");
                return;
            }

            switch(d_s)
            {
                case DorS.Display:
                    IotPortRvMsgDisplayEvent?.Invoke(topic, jo);
                    break;

                case DorS.Set:
                    IotPortRvMsgSetEvent?.Invoke(topic, jo);
                    break;

                default:
                    break;
            }
            
            nlogger.Debug("收到来自 mqttCloud 的数据: SubTopic - " + topic.ToString() + "  message - " + message);
        }
    }
}
