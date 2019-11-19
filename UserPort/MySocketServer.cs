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
    namespace MySocket
    {
        // session 中数据的格式
        // key: Subscribe / UnSubscribe / Post / Request
        // body: Topic
        // parameter: data

        /// <summary>
        /// socket 数据/命令的用途
        /// </summary>
        public enum SocketCmd : int
        {
            /// <summary> 未知主题 - 通常用于错误提示 </summary>
            Unknown = 0,
            /// <summary> 订阅主题 </summary>
            Subscribe = 1,
            /// <summary> 注销主题 </summary>
            UnSubscribe = 2,
            /// <summary> 推送数据 </summary>
            Post = 3,
            /// <summary> 请求数据 </summary>
            Request = 4
        }

        class MySocketServer
        {
            private static readonly Logger nlogger = LogManager.GetCurrentClassLogger();

            /// <summary>
            /// Server
            /// </summary>
            private AppServer _appServer = new AppServer();

            /// <summary>
            /// client 端所订阅的主题。当 client 订阅该主题时，server 端便向这些 session 发布相关信息
            /// </summary>
            private Dictionary<SubTopic, List<AppSession>> topicsSubs = new Dictionary<SubTopic, List<AppSession>>();

            /// <summary>
            /// 是否启用 mqtt
            /// </summary>
            public bool Enabled { get; set; } = false;

            //
            public delegate void MessageReceievedEventHandler(SubTopic topic, string message);
            /// <summary>
            /// 事件 - 接收到 post 消息
            /// </summary>
            public event MessageReceievedEventHandler MessageReceievedPostEvent;
            /// <summary>
            /// 事件 - 接收到 request 消息
            /// </summary>
            public event MessageReceievedEventHandler MessageReceievedRequestEvent;


            ~MySocketServer()
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
                if (!Enabled) return;

                nlogger.Debug("Server receive new request from the client.");

                Console.WriteLine(requestInfo.Body);

                // session 中数据的格式
                // key: Subscribe / UnSubscribe / Post / Request
                // body: Topic
                // parameter: data

                SubTopic topic;
                SocketCmd cmd;
                string data = requestInfo.Parameters[0];

                if (!Enum.TryParse(requestInfo.Key, out cmd))
                {
                    nlogger.Error("cannot parse the SocketCmd from the requestInfo.key: " + requestInfo.Body);
                    return;
                }

                if (!Enum.TryParse(requestInfo.Body, out topic))
                {
                    nlogger.Error("cannot parse the SubTopic from the requestInfo.body: " + requestInfo.Body);
                    return;
                }

                switch (cmd)
                {
                    case SocketCmd.Subscribe:
                        if (!topicsSubs.ContainsKey(topic))
                        {
                            topicsSubs.Add(topic, new List<AppSession>());
                        }

                        List<AppSession> itm = topicsSubs[topic];
                        if (!itm.Contains(session))
                        {
                            itm.Add(session);
                            nlogger.Info("new topic " + topic.ToString() + " subscribed from sessionID " + session.SessionID);
                        }
                        break;

                    case SocketCmd.UnSubscribe:
                        if (topicsSubs.ContainsKey(topic))
                        {
                            List<AppSession> itm2 = topicsSubs[topic];

                            int idx = itm2.IndexOf(session);
                            if (idx != -1)
                            {
                                itm2.RemoveAt(idx);
                                nlogger.Info("remove topic " + topic.ToString() + " from sessionID " + session.SessionID);
                            }
                        }
                        break;

                    case SocketCmd.Post:
                        MessageReceievedPostEvent?.Invoke(topic, data);
                        break;

                    case SocketCmd.Request:
                        MessageReceievedRequestEvent?.Invoke(topic, data);
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

                Enabled = true;
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
                foreach (var ses in _appServer.GetAllSessions())
                {
                    // wghou
                    // 如果断开了，是不是应该删除相应的主题订阅
                    if (!ses.Connected) continue;

                    if (!topicsSubs.ContainsKey(topic)) continue;

                    if (topicsSubs[topic].Contains(ses))
                    {
                        byte[] arr = System.Text.Encoding.Default.GetBytes(string.Format("{0} {1}\r\n", topic.ToString(), Message));
                        ses.Send(arr, 0, arr.Length);
                    }
                }
            }
        }
    }    
}
