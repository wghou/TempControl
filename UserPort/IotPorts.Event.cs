using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace IotPort
{
    public partial class IotPorts
    {
        //
        public delegate void UserPortMessageReceievedEventHandler(IotTopic topic, JObject message);
        /// <summary>
        /// 事件 - 接收到消息
        /// </summary>
        public event UserPortMessageReceievedEventHandler IotPortReceiveMessageEvent;
    }
}
