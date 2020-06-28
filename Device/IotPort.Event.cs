using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device.IotPort
{
    public partial class IotPortServer
    {
        public delegate void MsgReceivedEventHandler(IotPortEventArgs arg);
        /// <summary>
        /// 收到指令消息 - 事件
        /// </summary>
        public event MsgReceivedEventHandler MsgReceivedEvent;

        public delegate void ErrorOccurEventHandler(IotError error);
        /// <summary>
        /// 发生错误 - 事件
        /// </summary>
        public event ErrorOccurEventHandler ErrorOccurEvent;
    }
}
