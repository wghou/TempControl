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

namespace Others
{
    public class MySocketServer
    {
        private static readonly Logger nlogger = LogManager.GetCurrentClassLogger();

        private AppServer _appServer = new AppServer();

        /// <summary>
        /// 是否启用 mqtt
        /// </summary>
        public bool Enabled { get; set; } = false;


        /// <summary>
        /// 上位机给出的指令
        /// </summary>
        public enum SocketCmd : int
        {
            /// <summary> 开始自动控温 </summary>
            AutoStart = 0,
            /// <summary> 暂停 </summary>
            Suspend,
            /// <summary> 停止 </summary>
            Stop,
            /// <summary> 测量完成 </summary>
            Finished,
            /// <summary> 传感器状态 </summary>
            SensorInfo,
            /// <summary> 未知 </summary>
            Unknown
        }


        public abstract class SocketMessageBase {
            /// <summary> Socket Message 的类型 </summary>
            SocketCmd cmdType { get; } = SocketCmd.Unknown;

            public SocketMessageBase(SocketCmd tp) { cmdType = tp; }
        }

        //
        public delegate void MessageReceievedEventHandler(SocketCmd cmd, JObject msg);
        /// <summary>
        /// 事件 - 接收到 post 消息
        /// </summary>
        public event MessageReceievedEventHandler MessageReceievedEvent;


        ~MySocketServer()
        {
            _appServer.Stop();
        }


        public bool Init(JObject config)
        {
            try
            {
                if(!_appServer.Setup((int)config["port"]))
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
            catch(Exception ex)
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
        /// 查询 mqtt 是否连接成功
        /// </summary>
        public bool isConnected
        {
            get { return true; }
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

        /// <summary>
        /// socket 接收到数据 - 事件处理函数
        /// </summary>
        /// <param name="session"></param>
        /// <param name="requestInfo"></param>
        private void _appServer_NewRequestReceived(AppSession session, StringRequestInfo requestInfo)
        {
            if (!Enabled) return;

            if (requestInfo.Body.Length != 0)
            {
                // 解析并触发事件

                JObject jMsg = (JObject)JsonConvert.DeserializeObject(requestInfo.Body);

                if(jMsg.ContainsKey("cmdType"))
                {
                    try {
                        SocketCmd cmd = (SocketCmd)Enum.ToObject(typeof(SocketCmd), (int)jMsg["cmdType"]);

                        MessageReceievedEvent?.Invoke(cmd, jMsg);
                    }
                    catch(Exception ex)
                    {
                        nlogger.Error("received bad cmdType from MySocketSever.");
                    } 
                }
                else
                {
                    nlogger.Error("received bad cmdType from MySocketSever.");
                }
            }
        }

        /// <summary>
        /// 通过 socket 发送数据
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool pushMessage<T>(T msg) where T: SocketMessageBase
        {
            string msgString = JsonConvert.SerializeObject(msg);
            // 遍历 session
            foreach (var itm in _appServer.GetAllSessions())
            {
                itm.Send(msgString);
            }

            return true;
        }
    }
}
