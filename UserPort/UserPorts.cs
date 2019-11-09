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
    /// <summary>
    /// 用于标记某项数据源自接口的形式
    /// </summary>
    public enum UserPortType : int
    {
        All = 0,
        Serial = 1,
        Socket = 2,
        MqttLocal = 3,
        MqttCloud = 4
    }

    /// <summary>
    /// 数据的类型/主题
    /// </summary>
    public enum SubTopic : int
    {
        Data = 0,
        Control = 1
    }

    public partial class UserPorts
    {
        private static readonly Logger nlogger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 用于连接串口的客户端
        /// </summary>
        private MySerialClient _serialClient = new MySerialClient();

        /// <summary>
        /// 用于连接 socket 的客户端
        /// </summary>
        private MySocket.MySocketServer _socketClient = new MySocket.MySocketServer();

        /// <summary>
        /// 用于连接本地 mqtt 服务器的客户端
        /// </summary>
        private MyMqttClient _mqttLocal = new MyMqttClient();

        /// <summary>
        /// 用于连接移动 OneNet 云平台的 mqtt 客户端
        /// </summary>
        private MyMqttClient _mqttCloud = new MyMqttClient();


        //
        public delegate void UserPortMessageReceievedEventHandler(UserPortType Ptype, SubTopic topic, string message);
        /// <summary>
        /// 事件 - 接收到消息
        /// </summary>
        public event UserPortMessageReceievedEventHandler UserPortMsgRvEvent;
 
    }
}
