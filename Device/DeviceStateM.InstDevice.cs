using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ATControl.Utils;

namespace Device
{
    public partial class DeviceStateM
    {
        public ATControl.Utils.UtilsDeviceStateM instDevice = new ATControl.Utils.UtilsDeviceStateM();

        /// <summary>
        /// 配置仪器设备和socket
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool init_InstDevice(JObject obj)
        {
            bool confOK = true;
            // 设置仪器
            if (obj.ContainsKey("InstDev"))
            {
                bool rlt = false;
                JArray child = (JArray)obj["InstDev"];

                if (obj.ContainsKey("InstDeviceCmd"))
                {
                    JObject cmd = (JObject)obj["InstDeviceCmd"];
                    rlt = instDevice.initInstDevices(child, cmd);
                }
                else
                {
                    rlt = instDevice.initInstDevices(child);
                }

                if (!rlt) nlogger.Error("配置仪器失败!");
                else nlogger.Debug("配置仪器成功!");
                confOK &= rlt;
            }

            // 设置 socket 接口
            if (obj.ContainsKey("Socket"))
            {
                JObject child = (JObject)obj["Socket"];

                bool rlt = instDevice.InitSocketServer(child);
                if (!rlt) nlogger.Error("配置 Socket 接口失败!");
                else nlogger.Debug("配置 Socket 接口成功!");
                confOK &= rlt;
            }

            if(confOK == true)
            {
                instDevice.SocketReceiveMessageEvent += InstDevice_SocketReceiveMessageEvent;
                instDevice.InstDeviceErrorOccurEvent += InstDevice_InstDeviceErrorOccurEvent;
            }

            return confOK;
        }

        /// <summary>
        /// 仪器设备发生错误事件处理函数
        /// </summary>
        /// <param name="err"></param>
        private void InstDevice_InstDeviceErrorOccurEvent(ATControl.SeaBirdInst.Err_sr err)
        {
            SetErrorStatus(ErrorCode.CodeError);
        }

        /// <summary>
        /// socket 接收到信息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private void InstDevice_SocketReceiveMessageEvent(JObject message)
        {
            // 解析收到的指令
            SocketCmdMessage msg = message.ToObject<SocketCmdMessage>();

            bool rlt = false;
            switch (msg.cmdType)
            {
                // 开始控温流程
                case SocketCmd.AutoStart:
                    msg.ExecuteSucceed = StartAutoControl();
                    instDevice.socketSendMessage(JObject.FromObject(msg));
                    break;

                // 停止控温流程
                case SocketCmd.Stop:
                    msg.ExecuteSucceed = SuspendAutoControl();
                    instDevice.socketSendMessage(JObject.FromObject(msg));
                    break;

                // 读取仪器信息
                case SocketCmd.TestId:
                    instDevice.socketSendMessage(JObject.FromObject(msg));
                    break;

                case SocketCmd.DeviceState:
                    // 返回收到的指令
                    SocketStateMessage msgSend2 = message.ToObject<SocketStateMessage>();
                    msgSend2.devSt = (int)_state;
                    instDevice.socketSendMessage(JObject.FromObject(msgSend2));
                    break;

                case SocketCmd.DeviceStatus:
                    instDevice.socketSendMessage(JObject.FromObject(msg));
                    break;

                default:
                    //nlogger.Error("unknow socket cmd: " + msg.cmdType.ToString());
                    break;
            }
        }
    }
}
