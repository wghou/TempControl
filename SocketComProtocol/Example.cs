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
using Others;
using System.Windows.Forms;
using Device;

namespace ComTest
{
    public partial class Form1
    {
        private MySocketClient _socketClient = new MySocketClient();
		private bool _socketClient_enable = false;
		private string confFile = "config.json";

        /// <summary>
        /// 初始化 socket client
        /// </summary>
        /// <param name="cfg"></param>
        /// <returns></returns>
        internal bool InitSocket(JObject cfg)
        {
			bool confOK = false;
			System.IO.StreamReader file = System.IO.File.OpenText(confFile);
            JsonTextReader reader = new JsonTextReader(file);
            JObject obj = (JObject)JToken.ReadFrom(reader);
			
			if (obj.ContainsKey("Socket"))
            {
                JObject child = (JObject)obj["Socket"];

                _socketClient_enable = child.ContainsKey("Enable") ? (bool)child["Enable"]:true;
                if (_socketClient_enable)
                {
                    confOK = _socketClient.Init(cfg);
					_socketClient.MessageReceievedEvent += _socketClient_MessageReceievedEvent;
                }
			}
			
            return confOK;
        }

        /// <summary>
        /// 收到来自 socket server 的信息
        /// </summary>
        /// <param name="msg"></param>
        private void _socketClient_MessageReceievedEvent(JObject msg)
        {
            // todo:
            writeLog(msg.ToString());
			
			SocketCmdMessage jP = msg.ToObject<SocketCmdMessage>();
			
			switch(jP.cmdType){
				
			}
			
			switch(jP.ExecuteSucceed){}
        }

        /// <summary>
        /// socket 相关按键点击事件函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox_Socket_Click(object sender, EventArgs e)
        {

            SocketCmd cmd = SocketCmd.AutoStart;

            switch (cmd)
            {
                case SocketCmd.AutoStart:
                case SocketCmd.Stop:
                case SocketCmd.Finished:
                    SocketCmdMessage msg1 = new SocketCmdMessage(cmd);
                    _socketClient.pushMessage(JObject.FromObject(msg1));
                    break;

                case SocketCmd.TestId:
                    SocketTestIdxMessage msg2 = new SocketTestIdxMessage();
                    msg2.TestIdx = textBox_testId.Text;
                    _socketClient.pushMessage(JObject.FromObject(msg2));
                    break;

                case SocketCmd.DeviceState:
                    SocketStateMessage msg3 = new SocketStateMessage();
                    _socketClient.pushMessage(JObject.FromObject(msg3));
                    break;

                default:
                    break;
            }
            
        }
    }
}
