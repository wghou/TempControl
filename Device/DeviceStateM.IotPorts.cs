using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using IotPort;

namespace Device
{
    public partial class DeviceStateM
    {
        /// <summary>
        /// 用户接口
        /// </summary>
        private IotPorts _userPorts = new IotPorts();

        bool InitIotPort(JObject child)
        {
            IotPort.Topic[] tpSub = new IotPort.Topic[] { IotPort.Topic.ParamT, IotPort.Topic.Relay, Topic.Error};
            bool confOK = _userPorts.configUserPorts(child, tpSub);
            if (!confOK) nlogger.Error("配置 UserPort 失败");
            else nlogger.Debug("配置 UserPort 失败");

            _userPorts.IotPortRvMsgSetEvent += _userPorts_UserPortMsgRvSetEvent;
            TimerTickEndEvent += DeviceStateM_TimerTickEndEvent;
            StateChangedEvent += DeviceStateM_StateChangedEvent;
            RelayDeviceMStatusUpdatedEvent += DeviceStateM_RelayDeviceStatusUpdatedEvent;
            RelayDeviceSStatusUpdatedEvent += DeviceStateM_RelayDeviceStatusUpdatedEvent;
            ErrorStatusChangedEvent += DeviceStateM_ErrorStatusChangedEvent;
            DeviceClosedEvent += DeviceStateM_DeviceClosedEvent;

            // 初始时，刷新一下状态
            DeviceStateM_DeviceClosedEvent();

            return confOK;
        }

        // 当设备关闭时，发布继电器、自动控温状态
        private void DeviceStateM_DeviceClosedEvent()
        {
            iotPublishMessage(Topic.Relay);
            iotPublishMessage(Topic.AutoState);
        }

        // 当错误状态改变时，发布错误信息
        private void DeviceStateM_ErrorStatusChangedEvent(Dictionary<ErrorCode, uint> errDict)
        {
            iotPublishMessage(Topic.Error);
        }

        // 当继电器状态改变时，发布
        private void DeviceStateM_RelayDeviceStatusUpdatedEvent(RelayDevice.Err_r err, bool[] ryStatus)
        {
            iotPublishMessage(Topic.Relay);
        }

        // 当自动控温状态改变时，发布
        private void DeviceStateM_StateChangedEvent(State st)
        {
            iotPublishMessage(Topic.AutoState);
        }

        // 定时器事件，定时发布温度等数据
        private void DeviceStateM_TimerTickEndEvent()
        {
            iotPublishMessage(Topic.ParamT);
        }

        // 查看 iot 连接是否正确
        public bool isUserPortConnected
        {
            get { return true; }
        }

        /// <summary>
        /// 从用户接口收到数据
        /// </summary>
        /// <param name="Ptype"></param>
        /// <param name="topic"></param>
        /// <param name="message"></param>
        private void _userPorts_UserPortMsgRvSetEvent(Topic topic, JObject message)
        {
            nlogger.Info("the message topic is " + topic.ToString() + ", and the message is: " + message);

            switch (topic)
            {
                case Topic.ParamT:
                    nlogger.Info("Receive message from mqtt with topic Topic.ParamT");
                    break;

                case Topic.Relay:
                    nlogger.Info("Receive message from mqtt with topic Topic.Relay");
                    break;

                default:
                    Console.WriteLine("Unknown message receieved.");
                    break;
            }
        }

        public bool iotPublishMessage(Topic tp)
        {
            switch (tp)
            {
                case Topic.ParamT:
                    JsonParamTs jPt = new JsonParamTs();
                    jPt.d_s = DorS.Display;
                    jPt.paramM = new ParamT();
                    jPt.paramS = new ParamT();

                    float[] pM = new float[10];
                    tpDeviceM.tpParam.CopyTo(pM, 0);
                    jPt.paramM.setValue(pM);
                    jPt.paramM.TempShow = tpDeviceM.temperatures.Count > 0 ? tpDeviceM.temperatures.Last() : 0.0f;
                    jPt.paramM.PowerShow = tpDeviceM.tpPowerShow;
                    // 波动度
                    float fluc = 0.0f;
                    tpDeviceM.GetFlucDurCountOrLess(_runningParameters.steadyTimeSec / _runningParameters.readTempIntervalSec, out fluc);
                    jPt.paramM.Fluc = fluc;

                    float[] pS = new float[10];
                    tpDeviceS.tpParam.CopyTo(pS, 0);
                    jPt.paramS.setValue(pS);
                    jPt.paramS.TempShow = tpDeviceS.temperatures.Count > 0 ? tpDeviceS.temperatures.Last() : 0.0f;
                    jPt.paramS.PowerShow = tpDeviceS.tpPowerShow;
                    // 波动度
                    tpDeviceS.GetFlucDurCountOrLess(_runningParameters.steadyTimeSec / _runningParameters.readTempIntervalSec, out fluc);
                    jPt.paramS.Fluc = fluc;

                    string jpString = JsonConvert.SerializeObject(jPt);

                    _userPorts.PublishMessage(Topic.ParamT, jpString);
                    break;

                case Topic.Relay:
                    JsonRelay88 jR = new JsonRelay88();
                    jR.d_s = DorS.Display;
                    jR.relayM = new Relay8();
                    jR.relayS = new Relay8();

                    jR.relayM.setValue(ryDeviceM.ryStatus);
                    jR.relayS.setValue(ryDeviceS.ryStatus);

                    string jrString = JsonConvert.SerializeObject(jR);

                    _userPorts.PublishMessage(Topic.Relay, jrString);
                    break;

                case Topic.AutoState:
                    JsonAutoState jSt = new JsonAutoState();
                    jSt.d_s = DorS.Display;
                    jSt.state = new DeviceState();

                    jSt.state = (DeviceState)_machine.State;

                    string jstString = JsonConvert.SerializeObject(jSt);

                    _userPorts.PublishMessage(Topic.AutoState, jstString);
                    break;

                case Topic.Error:
                    JsonError jEr = new JsonError();
                    jEr.d_s = DorS.Display;

                    jEr.errCnt = new Dictionary<IotPort.ErrorCode, uint>();

                    foreach (Device.ErrorCode item in Enum.GetValues(typeof(Device.ErrorCode)))
                    {
                        jEr.errCnt[(IotPort.ErrorCode)item] = _deviceCurrentError[item];
                    }

                    string jerString = JsonConvert.SerializeObject(jEr);

                    _userPorts.PublishMessage(Topic.Error, jerString);
                    break;

                default:
                    break;
            }
            return true;
        }
    }
}
