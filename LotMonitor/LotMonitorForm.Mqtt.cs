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
using System.Windows.Forms;

namespace IotMonitor
{
    public partial class IotMonitorForm
    {
        private IotPorts _iotClient = new IotPorts();

        /// <summary>
        /// 是否与 mqtt server 连接成功
        /// </summary>
        public bool isMqttConnected { get { return _iotClient.isConnected(); } }
        /// <summary>
        /// 仪器设备的信息
        /// </summary>
        public List<InstInfoBase> instInfos = new List<InstInfoBase>();


        public delegate void DataReceivedEventHandler(InstDataShow data);
        /// <summary>
        /// 仪器接收到数据 - 事件
        /// </summary>
        public event DataReceivedEventHandler InstDataReceivedEvent;


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
                        IotTopic.ParamT, IotTopic.Relay, IotTopic.Error, IotTopic.DeviceState, IotTopic.Error, IotTopic.SampleState, IotTopic.InstState, IotTopic.InstValue };
                    _iotClient.configIotPorts(child, tpSub);
                    _iotClient.IotPortReceiveMessageEvent += IotClient_MessageReceievedEvent;
                    _iotClient.UserPortConnectedEvent += _iotClient_UserPortConnectedEvent;
                }  
            }
            catch(Exception ex)
            {

            }

            return true;
        }

        /// <summary>
        /// Iot Port 连接成功
        /// </summary>
        private void _iotClient_UserPortConnectedEvent()
        {
            getDeviceState();
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
                            writeLog(DateTime.Now.ToString("hh-mm-ss") + " : " + itm.ToString(),
                                richTextBox_log, 20);
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
                    instInfos = srSt.InstInfos;
                    this.InstDataReceivedEvent?.Invoke(null);
                    break;

                case IotTopic.InstValue:
                    // todo: 优化
                    IotInstValueMessage srVal = new IotInstValueMessage();
                    //srVal = JsonConvert.DeserializeObject<IotInstValueMessage>(message.ToString(), new JsonInstDataConverter());
                    srVal = JsonConvert.DeserializeObject<IotInstValueMessage>(message.ToString());

                    this.BeginInvoke(new EventHandler(delegate
                    {
                        if(srVal.InstData.InstIdx == 0)
                        {
                            writeLog(srVal.InstData.dtTime.ToString("hh-mm-ss") + "  Tempt: " + srVal.InstData.Tempt.ToString("0.0000") + "   Conduct: " + srVal.InstData.Conduct.ToString("0.0000"),
                                richTextBox_sdVal, 50);
                        }
                        else
                        {
                            this.InstDataReceivedEvent?.Invoke(srVal.InstData);
                        }
                    }));
                    break;


                default:
                    Console.WriteLine("Unknown message receieved.");
                    break;
            }
        }

        /// <summary>
        /// 显示仪器数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox_instVal_Click(object sender, EventArgs e)
        {
            foreach(var itm in instInfos)
            {
                if(itm.InstIdx_NotUsed > 0)
                {
                    bool formExist = false;
                    foreach (Form fm in Application.OpenForms)
                    {
                        if (fm.Name == ("FormInst " + itm.InstIdx_NotUsed.ToString()))
                        {
                            // Avoid form being minimized
                            fm.WindowState = FormWindowState.Normal;
                            fm.Location = new System.Drawing.Point(10, 12);
                            fm.BringToFront();
                            formExist = true;
                        }
                    }

                    if (!formExist)
                    {
                        FormInst fm = new FormInst(this);
                        fm.instIdx = itm.InstIdx_NotUsed;
                        fm.Location = new System.Drawing.Point(10, 12);
                        fm.Name = ("FormInst " + itm.InstIdx_NotUsed.ToString());
                        fm.Show();
                    }
                }
            }
        }


        private void writeLog(string log, RichTextBox box, int maxLength)
        {
            if (box == null) return;

            try
            {
                box.AppendText(log);
                box.AppendText(Environment.NewLine);
                box.ScrollToCaret();

                if (box.Lines.Length > maxLength)
                {
                    int moreLines = box.Lines.Length - maxLength;
                    string[] lines = box.Lines;
                    Array.Copy(lines, moreLines, lines, 0, maxLength);
                    Array.Resize(ref lines, maxLength);
                    box.Lines = lines;
                    box.SelectionStart = box.Text.Length;
                    box.SelectionLength = 0;
                    //box.Focus();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }
    }
}
