using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Others;

namespace Device
{
    public partial class DeviceStateM
    {
        private MySocketServer _socketServer = new MySocketServer();

        /// <summary>
        /// 初始化网络端口
        /// </summary>
        /// <param name="cfg"></param>
        /// <returns></returns>
        private bool InitSocketServer(JObject cfg)
        {
            bool rt = _socketServer.Init(cfg);

            if (rt != false)
            {
                _socketServer.MessageReceievedEvent += _socketServer_MessageReceievedEvent;
            }

            return rt;
        }

        /// <summary>
        /// socket 接受到指令 - 事件处理函数
        /// </summary>
        /// <param name="cmd"></param>
        private void _socketServer_MessageReceievedEvent(MySocketServer.SocketCmd cmd, JObject msg)
        {
            // todo: 
            switch (cmd)
            {
                case MySocketServer.SocketCmd.AutoStart:
                    StartAutoControl();
                    break;

                case MySocketServer.SocketCmd.Suspend:
                    SuspendAutoControl();
                    break;

                case MySocketServer.SocketCmd.Stop:
                    ShutdownComputer();
                    break;

                case MySocketServer.SocketCmd.SensorInfo:
                    List<SensorDevice.DeviceInfo> states = new List<SensorDevice.DeviceInfo>();
                    SocketSensorMessage srMsg = new SocketSensorMessage(MySocketServer.SocketCmd.SensorInfo);
                    foreach (var itm in srDevices)
                    {
                        states.Add(itm.deviceInfo);
                    }
                    srMsg.sensorStates = states;
                    _socketServer.pushMessage(srMsg);
                    break;

                default:
                    nlogger.Error("unknow socket cmd: " + cmd.ToString());
                    break;
            }

            switch (cmd)
            {
                case MySocketServer.SocketCmd.AutoStart:
                case MySocketServer.SocketCmd.Suspend:
                case MySocketServer.SocketCmd.Stop:
                    SocketCmdMessage msgSend = new SocketCmdMessage(MySocketServer.SocketCmd.AutoStart);
                    msgSend.deviceState = _state;
                    _socketServer.pushMessage(msgSend);
                    break;
            }
        }

        /// <summary>
        /// socket 指令返回数据
        /// </summary>
        internal class SocketCmdMessage : MySocketServer.SocketMessageBase
        {
            public SocketCmdMessage(MySocketServer.SocketCmd cmd):base(cmd) { }

            /// <summary> 控温设备当前的状态 </summary>
            public State deviceState { get; set; }
        }

        internal class SocketSensorMessage : MySocketServer.SocketMessageBase
        {
            public SocketSensorMessage(MySocketServer.SocketCmd cmd) : base(cmd) { }

            /// <summary> 传感器设备的状态 </summary>
            public List<SensorDevice.DeviceInfo> sensorStates { get; set; }
        }
    }
}
