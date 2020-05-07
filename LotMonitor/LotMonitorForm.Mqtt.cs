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
using IotPort;

namespace IotMonitor
{
    public partial class IotMonitorForm
    {
        private IotPorts _iotClient = new IotPorts();

        /// <summary>
        /// 是否与 mqtt server 连接成功
        /// </summary>
        public bool isMqttConnected { get { return _iotClient.isConnected(); } }


        private bool setupIotClient()
        {
            // json config file
            string confFile = @"./cfgCloud.json";
            try
            {
                System.IO.StreamReader file = System.IO.File.OpenText(confFile);
                JsonTextReader reader = new JsonTextReader(file);
                JObject obj = (JObject)JToken.ReadFrom(reader);

                // 设置接口
                if (obj.ContainsKey("IotPort"))
                {
                    JObject child = (JObject)obj["IotPort"];

                    Topic[] tpSub = new Topic[] {
                        Topic.ParamT, Topic.Relay, Topic.Error, Topic.AutoState, Topic.Error, Topic.SampleState };
                    _iotClient.configUserPorts(child, tpSub);
                    _iotClient.IotPortRvMsgDisplayEvent += IotClient_MessageReceievedEvent;
                }  
            }
            catch(Exception ex)
            {

            }

            return true;
        }

        /// <summary>
        /// 接收到 Server 消息
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="message"></param>
        private void IotClient_MessageReceievedEvent(Topic topic, JObject message)
        {
            switch (topic)
            {
                case Topic.ParamT:

                    JsonParamTs jP = message.ToObject<JsonParamTs>();

                    this.BeginInvoke(new EventHandler(delegate
                    {
                        // tempM
                        this.hslGaugeChart_temptM.Value = Math.Round(jP.paramM.TempShow, 3);
                        this.hslGauge_powerM.Value = (float)Math.Round(jP.paramM.PowerShow, 3);
                        this.hslLedDisplay_temptSetM.DisplayText = jP.paramM.TemptSet.ToString("0.0000");

                        // tempS
                        this.hslGaugeChart_temptS.Value = Math.Round(jP.paramS.TempShow, 3);
                        this.hslGauge_powerS.Value = (float)Math.Round(jP.paramS.PowerShow, 3);
                        this.hslLedDisplay_temptSetS.DisplayText = jP.paramS.TemptSet.ToString("0.0000");
                    }));
                    break;

                case Topic.Relay:

                    JsonRelay88 jR = message.ToObject<JsonRelay88>();

                    this.BeginInvoke(new EventHandler(delegate
                    {
                        // relayM
                        for (int i = 0; i < 8; i++)
                        {
                            switchRyM[i].SwitchStatus = jR.relayM.getValue()[i];
                        }

                        // relayS
                        for (int i = 0; i < 8; i++)
                        {
                            switchRyS[i].SwitchStatus = jR.relayS.getValue()[i];
                        }
                    }));
                    break;
                    
                case Topic.Error:
                    JsonError err = message.ToObject<JsonError>();

                    foreach (ErrorCode itm in Enum.GetValues(typeof(ErrorCode)))
                    {
                        // 记录这段时间内的错误
                        if(err.errCnt[itm] > 0)
                        {
                            writeLog(DateTime.Now.ToString("hh-mm-ss") + " : " + itm.ToString());
                        }
                    }

                    break;

                case Topic.AutoState:
                    JsonAutoState st = message.ToObject<JsonAutoState>();
                    this.BeginInvoke(new EventHandler(delegate
                    {
                        label_state.Text = "当前状态：" + st.state.ToString();
                    }));
                    break;

                case Topic.SampleState:
                    JsonSampleState sm = message.ToObject<JsonSampleState>();
                    this.BeginInvoke(new EventHandler(delegate
                    {
                        label_sample.Text = "采样状态：" + sm.state.ToString();
                    }));
                    break;

                case Topic.Sensor:
                    JsonSensor sr = message.ToObject<JsonSensor>();
                    this.BeginInvoke(new EventHandler(delegate
                    {
                        // 将传感器信息显示
                    }));
                    break;

                default:
                    Console.WriteLine("Unknown message receieved.");
                    break;
            }
        }


        private void writeLog(string log)
        {
            richTextBox_log.AppendText(log);
            richTextBox_log.AppendText(Environment.NewLine);
            richTextBox_log.ScrollToCaret();

            limitLine(20);
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
