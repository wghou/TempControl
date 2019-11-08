using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using NLog;

namespace UserPort
{
    class MySocketClient
    {
        private static readonly Logger nlogger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Server
        /// </summary>
        private AppServer _appServer = new AppServer();


        /// <summary>
        /// 是否启用 mqtt
        /// </summary>
        public bool Enabled { get; set; } = false;

        //
        // Topic 的构成形式为： topicDeviceId/SubTopic
        // 也就是说，每制作一台设备时，都会新建一个 DeviceId，该设备下，有两个子主题（Data和Control）
        /// <summary>
        /// 设备序列号 - 在同一类产品中，每个单独的设备都有一个相应的编号
        /// </summary>
        private const string topicDeviceId = "lot_tst";
        /// <summary>
        /// 设备所订阅的主题
        /// </summary>
        private List<string> topicsSubs = new List<string>();

        //
        public delegate void MessageReceievedEventHandler(SubTopic topic, string message);
        /// <summary>
        /// 事件 - 接收到消息
        /// </summary>
        public event MessageReceievedEventHandler MessageReceievedEvent;


        ~MySocketClient()
        {
            _appServer.Stop();
        }


        private void _appServer_NewSessionConnected(AppSession session)
        {
            nlogger.Debug("service get connection from clinet successful.");
            var count = _appServer.GetAllSessions().Count();

            nlogger.Debug("server.session.count: " + count);

            session.Send("Welcome to SuperSocket Telnet Server.");
        }

        private void _appServer_SessionClosed(AppSession session, CloseReason value)
        {
            nlogger.Debug("Server lost one connection from the client.");
            var count = _appServer.GetAllSessions().Count();

            nlogger.Debug("server.session.count: " + count);
        }

        private void _appServer_NewRequestReceived(AppSession session, StringRequestInfo requestInfo)
        {
            nlogger.Debug("Server receive new request from the client.");

            Console.WriteLine(requestInfo.Body);

            switch (requestInfo.Key.ToUpper())
            {
                case "DATA":
                    MessageReceievedEvent?.Invoke(SubTopic.Data, requestInfo.Body);
                    break;

                case "CONTROL":
                    MessageReceievedEvent?.Invoke(SubTopic.Control, requestInfo.Body);
                    break;

                default:

                    break;
            }
        }


        /// <summary>
        /// 查询 mqtt 是否连接成功
        /// </summary>
        public bool isConnected
        {
            get { return true; }
        }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public bool Initialize(JObject socketConfig, params SubTopic[] topics)
        {
            // 清空原有主题
            topicsSubs.Clear();

            // 添加主题
            foreach (var itm in topics)
            {
                topicsSubs.Add(topicDeviceId + "/" + Enum.GetName(typeof(SubTopic), itm));
            }


            try
            {
                if (!_appServer.Setup((int)socketConfig["port"]))
                {
                    nlogger.Error("Failed to setup!");
                    Enabled = false;
                    return false;
                }

                if (!_appServer.Start())
                {
                    nlogger.Error("Failed to start.");
                    Enabled = false;
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: config lot client failed.");

                Enabled = false;
                return false;
            }

            _appServer.NewSessionConnected += _appServer_NewSessionConnected;
            _appServer.SessionClosed += _appServer_SessionClosed;
            _appServer.NewRequestReceived += _appServer_NewRequestReceived;

            return true;

        }


        /// <summary>
        /// 发布主题
        /// </summary>
        /// <param name="Message"></param>
        public void Publish(SubTopic topic, string Message, bool isWait = false)
        {
            if (!Enabled) return;

            // socket broad cast
            foreach(var ses in _appServer.GetAllSessions())
            {
                if (!ses.Connected) continue;

                byte[] arr = System.Text.Encoding.Default.GetBytes(string.Format("{0} {1}\r\n", topic.ToString(), Message));
                ses.Send(arr, 0, arr.Length);
            }
        }
    }
}
