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
        private static readonly Logger nlogger = LogManager.GetCurrentClassLogger();

        private MySocketClient _socketClient = new MySocketClient();
        private bool _socketClient_enable = false;

        Dictionary<CheckBox, SocketCmd> dictCheckBoxsSocket = new Dictionary<CheckBox, SocketCmd>();


        /// <summary>
        /// 初始化 socket client
        /// </summary>
        /// <param name="cfg"></param>
        /// <returns></returns>
        internal bool InitSocket(JObject cfg)
        {
            bool confOK = _socketClient.Init(cfg);

            _socketClient.MessageReceievedEvent += _socketClient_MessageReceievedEvent;

            dictCheckBoxsSocket[checkBox_AutoStart] = SocketCmd.AutoStart;
            dictCheckBoxsSocket[checkBox_TestIdx] = SocketCmd.TestId;
            dictCheckBoxsSocket[checkBox_Stop] = SocketCmd.Stop;
            dictCheckBoxsSocket[checkBox_DeviceState] = SocketCmd.DeviceState;
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
        }

        /// <summary>
        /// socket 相关按键点击事件函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox_Socket_Click(object sender, EventArgs e)
        {
            if (_socketClient_enable == false) return;

            if (!dictCheckBoxsSocket.ContainsKey((sender as CheckBox))) return;

            SocketCmd cmd = dictCheckBoxsSocket[(sender as CheckBox)];
            SocketCmdMessage msg = new SocketCmdMessage(cmd);

            _socketClient.pushMessage(JObject.FromObject(msg));
        }


        /// <summary>
        /// 显示日志
        /// </summary>
        /// <param name="log"></param>
        private void writeLog(string log)
        {
            this.BeginInvoke(new EventHandler(delegate
            {
                richTextBox_log.AppendText(log);
                richTextBox_log.AppendText(Environment.NewLine);
                richTextBox_log.ScrollToCaret();

                limitLine(20);
            }));  
        }

        private void limitLine(int maxLength)
        {
            if (richTextBox_log.Lines.Length > maxLength)
            {
                int moreLines = richTextBox_log.Lines.Length - maxLength;
                string[] lines = richTextBox_log.Lines;
                Array.Copy(lines, moreLines, lines, 0, maxLength);
                Array.Resize(ref lines, maxLength);
                richTextBox_log.Lines = lines;
                richTextBox_log.SelectionStart = richTextBox_log.Text.Length;
                richTextBox_log.SelectionLength = 0;
                richTextBox_log.Focus();
            }
        }
    }
}
