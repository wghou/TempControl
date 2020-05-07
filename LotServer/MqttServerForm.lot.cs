using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using MQTTnet;
using MQTTnet.Server;
using MQTTnet.Server.Status;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Packets;
using MQTTnet.Protocol;
using MQTTnet.Client.Receiving;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Connecting;
using NLog;

namespace IotServer
{
    public partial class MqttServerForm
    {
        private MqttServer mqttServer = null;
        private IMqttServerOptions options = null;
        private int mqttPort = 5001;
        private IPAddress mqttServerUrl = new IPAddress(new byte[] { 192, 168, 1, 101 });
        private string mqttUsername = "admin";
        private string mqttPassword = "public";

        private static readonly Logger nlogger = LogManager.GetCurrentClassLogger();

        private void writeLog(string log)
        {
            richTextBox_log.AppendText(log);
            richTextBox_log.AppendText(Environment.NewLine);
            richTextBox_log.ScrollToCaret();

            limitLine(50);
        }

        private void limitLine(int maxLength)
        {
            if(richTextBox_log.Lines.Length > maxLength)
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

        private bool setupMqtt()
        {
            nlogger.Info("setup mqtt.");
            writeLog("setup mqtt.");

            // get local ip address
            foreach (IPAddress _ipaddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_ipaddress.AddressFamily.ToString() == "InterNetwork") {
                    nlogger.Info("get the local ip address: " + _ipaddress.ToString());
                    writeLog("get the local ip address: " + _ipaddress.ToString());
                    mqttServerUrl = _ipaddress;
                }
            }

            if (mqttServer == null)
            {
                nlogger.Info("create new mqttServer. IP:" + mqttServerUrl.ToString() + " port:" + mqttPort.ToString());
                writeLog("create new mqttServer. IP:" + mqttServerUrl.ToString() + " port:" + mqttPort.ToString());

                var factory = new MqttFactory();
                mqttServer = factory.CreateMqttServer() as MqttServer;

                options = new MqttServerOptionsBuilder()
                    .WithConnectionBacklog(1000)
                    .WithDefaultEndpointPort(mqttPort)
                    .WithDefaultEndpointBoundIPAddress(mqttServerUrl)
                    .Build();

                (options as MqttServerOptions).ConnectionValidator = new MqttServerConnectionValidatorDelegate(new Func<MqttConnectionValidatorContext, Task>(ConnectionValidation));

                mqttServer.ClientConnectedHandler = new MqttServerClientConnectedHandlerDelegate(new Func<MqttServerClientConnectedEventArgs, Task>(Connected));
                mqttServer.ClientDisconnectedHandler = new MqttServerClientDisconnectedHandlerDelegate(new Func<MqttServerClientDisconnectedEventArgs, Task>(DisConnected));
                mqttServer.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(new Action<MqttApplicationMessageReceivedEventArgs>(MqttApplicationMessageReceived));
                mqttServer.ClientSubscribedTopicHandler = new MqttServerClientSubscribedHandlerDelegate(new Func<MqttServerClientSubscribedTopicEventArgs, Task>(SubscribeTopic));
                mqttServer.ClientUnsubscribedTopicHandler = new MqttServerClientUnsubscribedTopicHandlerDelegate(new Func<MqttServerClientUnsubscribedTopicEventArgs, Task>(UnSubscribeTopic));
                mqttServer.StartedHandler = new MqttServerStartedHandlerDelegate(new Action<EventArgs>(Started));
                mqttServer.StoppedHandler = new MqttServerStoppedHandlerDelegate(new Action<EventArgs>(Stopped));

                return true;
            }
            else
            {
                nlogger.Warn("There is a mqttServer exit already.");
                writeLog("There is a mqttServer exit already.");
                return false;
            }
        }

        private async Task ConnectionValidation(MqttConnectionValidatorContext context)
        {
            nlogger.Info("connection validation.");
            writeLog("connection validation.");

            if (context.ClientId.Length < 10)
            {
                nlogger.Error("ClientId.Length < 10");
                writeLog("ClientId.Length < 10");

                context.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
                return;
            }
            if (!context.Username.Equals(mqttUsername))
            {
                nlogger.Error("Username is not admin");
                writeLog("Username is not admin");

                context.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                return;
            }
            if (!context.Password.Equals(mqttPassword))
            {
                nlogger.Error("Password is not public");
                writeLog("Password is not public");

                context.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                return;
            }
            nlogger.Info("connection validation success.");
            writeLog("connection validation success.");

            context.ReasonCode = MqttConnectReasonCode.Success;
        }

        private async Task Connected(MqttServerClientConnectedEventArgs arg)
        {
            nlogger.Info("client connected. cientID: " + arg.ClientId);
            writeLog("client connected. cientID: " + arg.ClientId);
        }

        private async Task DisConnected(MqttServerClientDisconnectedEventArgs arg)
        {
            nlogger.Info("client disconnected. cientID: " + arg.ClientId);
            writeLog("client disconnected. cientID: " + arg.ClientId);
        }

        private void MqttApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs message)
        {
            nlogger.Info("server receive message from cientID: " + message.ClientId);
            writeLog("server receive message from cientID: " + message.ClientId);
            // process the message
        }

        private async Task SubscribeTopic(MqttServerClientSubscribedTopicEventArgs topicArg)
        {
            nlogger.Info("topic subscribed." + topicArg.TopicFilter.Topic);
            writeLog("topic subscribed." + topicArg.TopicFilter.Topic);
        }

        private async Task UnSubscribeTopic(MqttServerClientUnsubscribedTopicEventArgs topicArg)
        {
            nlogger.Info("topic unSubscribed." + topicArg.TopicFilter.GetType());
            writeLog("topic unSubscribed." + topicArg.TopicFilter.GetType());
        }

        private void Started(EventArgs e)
        {
            nlogger.Info("server started. ");
            writeLog("server started. ");

            this.BeginInvoke(new EventHandler(delegate
            {
                this.checkBox_start.Text = "停止服务";
            }));
        }

        private void Stopped(EventArgs e)
        {
            nlogger.Info("server stopped. ");
            writeLog("server stopped. ");

            this.BeginInvoke(new EventHandler(delegate
            {
                this.checkBox_start.Text = "启动服务";
            }));
        }
    }
}
