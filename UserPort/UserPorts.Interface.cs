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

namespace UserPort
{
    public partial class UserPorts
    {
        public bool configUserPorts(JObject cfg)
        {
            bool confOK = true;

            try
            {
                // 配置用户接口 - Serial
                if (cfg.ContainsKey("Serial"))
                {
                    JObject child = (JObject)cfg["Serial"];

                    if (child.ContainsKey("Enable") ? (bool)child["Enable"] : true)
                    {
                        confOK &= _serialClient.Initialize(child, SubTopic.Control);
                        _serialClient.MessageReceievedEvent += _serialClient_MessageReceievedEvent;

                        if (!confOK) nlogger.Error("配置 serialClient 失败");
                        else nlogger.Debug("配置 serialClient 成功");
                    }  
                }

                // 配置用户接口 - Socket
                if (cfg.ContainsKey("Socket"))
                {
                    JObject child = (JObject)cfg["Socket"];

                    if (child.ContainsKey("Enable") ? (bool)child["Enable"] : true)
                    {
                        confOK &= _socketClient.Initialize(child, SubTopic.Control);
                        _socketClient.MessageReceievedEvent += _socketClient_MessageReceievedEvent;

                        if (!confOK) nlogger.Error("配置 socketClient 失败");
                        else nlogger.Debug("配置 socketClient 成功");
                    }
                        
                }

                // 配置用户接口 - MqttLocal
                if (cfg.ContainsKey("MqttLocal"))
                {
                    JObject child = (JObject)cfg["MqttLocal"];

                    if (child.ContainsKey("Enable") ? (bool)child["Enable"] : true)
                    {
                        confOK &= _mqttLocal.Initialize(child, SubTopic.Control);
                        _mqttLocal.MessageReceievedEvent += _mqttLocal_MessageReceievedEvent;

                        if (!confOK) nlogger.Error("配置 MqttLocal 失败");
                        else nlogger.Debug("配置 MqttLocal 成功");
                    }     
                }

                // 配置用户接口 - MqttCloud
                if (cfg.ContainsKey("MqttCloud"))
                {
                    JObject child = (JObject)cfg["MqttCloud"];

                    if (child.ContainsKey("Enable") ? (bool)child["Enable"] : true)
                    {
                        confOK &= _mqttCloud.Initialize(child, SubTopic.Control);
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
        public bool isConnected(UserPortType port)
        {
            bool rlt = false;
            switch (port)
            {
                case UserPortType.Serial:
                    rlt = _serialClient.isConnected;
                    break;

                case UserPortType.Socket:
                    rlt = _serialClient.isConnected;
                    break;

                case UserPortType.MqttLocal:
                    rlt = _mqttLocal.isConnected;
                    break;

                case UserPortType.MqttCloud:
                    rlt = _mqttCloud.isConnected;
                    break;

                default:
                    rlt = false;
                    break;
            }

            return rlt;
        }


        /// <summary>
        /// 向特定接口发布消息
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="Message"></param>
        /// <param name="isWait"></param>
        /// <param name="ports"></param>
        /// <returns></returns>
        public bool PublishMessage(SubTopic topic, string Message, bool isWait = false, params UserPortType[] ports)
        {
            // 向全部接口广播发布
            if(ports.Contains(UserPortType.All))
            {
                _serialClient.Publish(topic, Message, isWait);
                _socketClient.Publish(topic, Message, isWait);
                _mqttLocal.Publish(topic, Message, isWait);
                _mqttCloud.Publish(topic, Message, isWait);
                return true;
            }

            // 依照 UserPortType 发布消息
            foreach(var itm in ports)
            {
                switch (itm)
                {
                    case UserPortType.Serial:
                        _serialClient.Publish(topic, Message, isWait);
                        break;

                    case UserPortType.Socket:
                        _socketClient.Publish(topic, Message, isWait);
                        break;

                    case UserPortType.MqttLocal:
                        _mqttLocal.Publish(topic, Message, isWait);
                        break;

                    case UserPortType.MqttCloud:
                        _mqttCloud.Publish(topic, Message, isWait);
                        break;

                    default:
                        break;
                }
            }

            return true;
        }


        // receive message from the serial
        private void _serialClient_MessageReceievedEvent(SubTopic topic, string message)
        {
            UserPortMsgRvEvent?.Invoke(UserPortType.Serial, topic, message);
            nlogger.Debug("收到来自 serialClient 的数据: SubTopic - " + topic.ToString() + "  message - " + message);
        }

        // receive message from the socket
        private void _socketClient_MessageReceievedEvent(SubTopic topic, string message)
        {
            UserPortMsgRvEvent?.Invoke(UserPortType.Socket, topic, message);
            nlogger.Debug("收到来自 socketClient 的数据: SubTopic - " + topic.ToString() + "  message - " + message);
        }

        // receive message from the mqtt local
        private void _mqttLocal_MessageReceievedEvent(SubTopic topic, string message)
        {
            UserPortMsgRvEvent?.Invoke(UserPortType.MqttLocal, topic, message);
            nlogger.Debug("收到来自 mqttLocal 的数据: SubTopic - " + topic.ToString() + "  message - " + message);
        }

        // receive message from the mqtt cloud
        private void _mqttCloud_MessageReceievedEvent(SubTopic topic, string message)
        {
            UserPortMsgRvEvent?.Invoke(UserPortType.MqttCloud, topic, message);
            nlogger.Debug("收到来自 mqttCloud 的数据: SubTopic - " + topic.ToString() + "  message - " + message);
        }
    }
}
