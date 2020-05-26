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
using IotCS.Client;
using Device;
using InstDevice;

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

                    IotTopic[] tpSub = new IotTopic[] {
                        IotTopic.ParamT, IotTopic.Relay, IotTopic.Error, IotTopic.DeviceState, IotTopic.Error, IotTopic.SampleState, IotTopic.InstState, IotTopic.SensorValue };
                    _iotClient.configIotPorts(child, tpSub);
                    _iotClient.IotPortReceiveMessageEvent += IotClient_MessageReceievedEvent;
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
        private void IotClient_MessageReceievedEvent(IotTopic topic, JObject message)
        {
            switch (topic)
            {
                case IotTopic.ParamT:

                    IotParamTMessage jP = message.ToObject<IotParamTMessage>();

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

                case IotTopic.Relay:

                    IotRelay88Message jR = message.ToObject<IotRelay88Message>();

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
                    
                case IotTopic.Error:
                    IotErrorMessage err = message.ToObject<IotErrorMessage>();

                    foreach (ErrorCode itm in Enum.GetValues(typeof(ErrorCode)))
                    {
                        // 记录这段时间内的错误
                        if(err.errCnt[itm] > 0)
                        {
                            writeLog(DateTime.Now.ToString("hh-mm-ss") + " : " + itm.ToString());
                        }
                    }

                    break;

                case IotTopic.DeviceState:
                    IotDeviceStateMessage st = message.ToObject<IotDeviceStateMessage>();
                    this.BeginInvoke(new EventHandler(delegate
                    {
                        label_state.Text = "当前状态：" + st.state.ToString();
                    }));
                    break;

                case IotTopic.SampleState:
                    IotSampleStateMessage sm = message.ToObject<IotSampleStateMessage>();
                    this.BeginInvoke(new EventHandler(delegate
                    {
                        label_sample.Text = "采样状态：" + sm.state.ToString();
                    }));
                    break;

                case IotTopic.InstState:
                    IotInstStateMessage srSt = message.ToObject<IotInstStateMessage>();
                    this.BeginInvoke(new EventHandler(delegate
                    {
                        // 将仪器信息显示
                        foreach(var itm in srSt.InstInfos)
                        {
                            if (itm.InstIdx_NotUsed < 0 || itm.InstIdx_NotUsed > 6) continue;

                            
                        }
                    }));
                    break;

                case IotTopic.SensorValue:
                    // todo: 优化
                    IotInstValueMessage srVal = new IotInstValueMessage();
                    srVal = JsonConvert.DeserializeObject<IotInstValueMessage>(message.ToString(), new JsonInstDataConverter());

                    this.BeginInvoke(new EventHandler(delegate
                    {
                        // 将仪器信息显示
                        foreach(var itm in srVal.InstData)
                        {
                            // todo: 解析
                            switch(itm.InstType)
                            {
                                case TypeInst.Standard:
                                    InstSTDData dt = (InstSTDData)itm;
                                    this.textBox_vTitularValue.Text = dt.vTitularValue.ToString();
                                    this.textBox_vStandardC.Text = dt.vStandardC.ToString();
                                    this.textBox_vStandardT.Text = dt.vStandardT.ToString();
                                    break;

                                case TypeInst.SBE37SI:
                                    
                                    break;

                                default:
                                    break;
                            }
                        }
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
