using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Others;
using System.IO;
using InstDevice;

namespace Device
{
    public partial class DeviceStateM
    {
        private MySocketServer _socketServer = new MySocketServer();
        /// <summary>
        /// 写入数据 sql
        /// </summary>
        protected readonly MySqlWriter sqlWriter = new MySqlWriter();
        /// <summary>
        /// 测试的 testID
        /// </summary>
        private string testIdSql = null;

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
        private void _socketServer_MessageReceievedEvent(JObject message)
        {
            // 解析收到的指令
            SocketCmdMessage msg = message.ToObject<SocketCmdMessage>();

            // todo: 如何处理错误情况

            switch (msg.cmdType)
            {
                // 开始控温流程
                case SocketCmd.AutoStart:
                    // 返回收到的指令
                    if (_state == State.Idle)
                    {
                        // todo: 这里如何载入温度点
                        readTempPointList();

                        StartAutoControl();
                        msg.ExecuteSucceed = true;
                    }
                    _socketServer.pushMessage(JObject.FromObject(msg));
                    break;
                
                // 停止控温流程
                case SocketCmd.Stop:
                    // 返回收到的指令
                    SuspendAutoControl();
                    msg.ExecuteSucceed = true;
                    _socketServer.pushMessage(JObject.FromObject(msg));
                    break;

                // 读取仪器信息
                case SocketCmd.TestId:
                    // 接收到 testID
                    SocketTestIdxMessage msgSend1 = message.ToObject<SocketTestIdxMessage>();
                    bool rlt = getInstInfoFromSql(msgSend1.TestIdx);
                    msgSend1.ExecuteSucceed = rlt;
                    _socketServer.pushMessage(JObject.FromObject(msgSend1));

                    // Iot 发布仪器信息
                    iotPublishMessage(IotCS.Client.IotTopic.InstState);
                    break;

                case SocketCmd.DeviceState:
                    // 返回收到的指令
                    SocketStateMessage msgSend2 = message.ToObject<SocketStateMessage>();
                    //SuspendAutoControl();
                    msgSend2.ExecuteSucceed = true;
                    msgSend2.devSt = _state;
                    msgSend2.ErrorSt = (CheckErrorStatus() != 0);
                    _socketServer.pushMessage(JObject.FromObject(msgSend2));
                    break;

                case SocketCmd.DeviceStatus:
                    SocketStatusMessage msgSend3 = message.ToObject<SocketStatusMessage>();
                    msgSend3.ExecuteSucceed = true;
                    //
                    // add all the parameters
                    // 主槽设定值
                    msgSend3.ZhucaoTemperature = tpDeviceM.tpParam[0];
                    // 主槽当前温度值
                    if (this.tpDeviceM.temperatures.Count != 0) msgSend3.ZhucaoTemperatureReal = tpDeviceM.temperatures.Last();
                    // 主槽加热功率
                    msgSend3.ZhucaoPower = (int)tpDeviceM.tpPowerShow;
                    // 辅槽设定值
                    msgSend3.FucaoTemperature = tpDeviceS.tpParam[0];
                    // 辅槽当前温度值
                    if (this.tpDeviceS.temperatures.Count != 0) msgSend3.FucaoTemperatureReal = tpDeviceS.temperatures.Last();
                    // 辅槽加热功率
                    msgSend3.FucaoPower = (int)tpDeviceS.tpPowerShow;

                    // 总电源开关
                    msgSend3.Zongdianyuan = ryDeviceM.ryStatus[0];
                    // 主槽制冷开关
                    msgSend3.Zhucaozhileng = ryDeviceM.ryStatus[1];
                    // 主槽控温开关
                    msgSend3.Zhucaokongwen = ryDeviceM.ryStatus[2];
                    // 辅槽控温开关
                    msgSend3.Fucaokongwen = ryDeviceM.ryStatus[3];
                    // 辅槽制冷开关
                    msgSend3.Fucaozhileng = ryDeviceM.ryStatus[4];
                    // 辅槽循环开关
                    msgSend3.Fucaoxunhuan = ryDeviceM.ryStatus[5];
                    // 辅槽快冷开关
                    msgSend3.Fucaokuaileng = ryDeviceM.ryStatus[6];
                    // 主槽快冷开关
                    msgSend3.Zhucaokuaileng = ryDeviceM.ryStatus[7];

                    // 数据采集时间
                    msgSend3.DataTime = _stdDataCache.addTime;
                    // 标准温度
                    msgSend3.Temperature = _stdDataCache.vStandardT;
                    // 标准电导率
                    msgSend3.Conductivity = _stdDataCache.vStandardC;
                    // 标准盐度
                    msgSend3.Salinity = _stdDataCache.vStandardS;


                    _socketServer.pushMessage(JObject.FromObject(msgSend3));
                    break;

                default:
                    nlogger.Error("unknow socket cmd: " + msg.cmdType.ToString());
                    break;
            }
        }

        /// <summary>
        /// 通过 socket 发送 finished 指令
        /// </summary>
        /// <returns></returns>
        private bool socketSendFinished()
        {
            // 停止仪器工作
            foreach(var itm in _instDevices)
            {
                itm.DisableInstDevice();
            }

            // 解析收到的指令
            SocketCmdMessage msg = new SocketCmdMessage(SocketCmd.Finished);
            msg.ExecuteSucceed = true;
            _socketServer.pushMessage(JObject.FromObject(msg));
            return true;
        }
    }
}
