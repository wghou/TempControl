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
                        loadTempPointList();

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
                    SuspendAutoControl();
                    msgSend2.ExecuteSucceed = true;
                    msgSend2.devSt = _state;
                    msgSend2.ErrorSt = (CheckErrorStatus() != 0);
                    _socketServer.pushMessage(JObject.FromObject(msgSend2));
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

        private void loadTempPointList()
        {
            temperaturePointList.Clear();

            // 从缓存文本中读取温度点
            try
            {
                string[] lines = File.ReadAllLines(@"./params.cache", Encoding.UTF8);
                for (int i = 0; i < lines.Length; i++)
                {
                    // 主槽参数
                    string line1 = lines[i];
                    TemptPointStruct ts = new TemptPointStruct();
                    i++;
                    string[] parmM = line1.Split(' ');
                    if (parmM.Length == 7)
                    {
                        float vl;
                        if (float.TryParse(parmM[0], out vl)) ts.paramM[0] = vl;
                        else break;
                        if (float.TryParse(parmM[1], out vl)) ts.paramM[1] = vl;
                        else break;
                        if (float.TryParse(parmM[2], out vl)) ts.paramM[2] = vl;
                        else break;
                        if (float.TryParse(parmM[3], out vl)) ts.paramM[3] = vl;
                        else break;
                        if (float.TryParse(parmM[4], out vl)) ts.paramM[4] = vl;
                        else break;
                        if (float.TryParse(parmM[5], out vl)) ts.paramM[5] = vl;
                        else break;
                        if (float.TryParse(parmM[6], out vl)) ts.paramM[6] = vl;
                        else break;
                    }
                    else
                    {
                        break;
                    }

                    // 辅槽参数
                    if (i >= lines.Length) { break; }
                    string line2 = lines[i];
                    string[] parmS = line2.Split(' ');
                    if (parmS.Length == 7)
                    {
                        float vl;
                        if (float.TryParse(parmS[0], out vl)) ts.paramS[0] = vl;
                        else break;
                        if (float.TryParse(parmS[1], out vl)) ts.paramS[1] = vl;
                        else break;
                        if (float.TryParse(parmS[2], out vl)) ts.paramS[2] = vl;
                        else break;
                        if (float.TryParse(parmS[3], out vl)) ts.paramS[3] = vl;
                        else break;
                        if (float.TryParse(parmS[4], out vl)) ts.paramS[4] = vl;
                        else break;
                        if (float.TryParse(parmS[5], out vl)) ts.paramS[5] = vl;
                        else break;
                        if (float.TryParse(parmS[6], out vl)) ts.paramS[6] = vl;
                        else break;
                    }
                    else
                    {
                        break;
                    }

                    // 装入列表中
                    temperaturePointList.Add(ts);
                }

                // 排序
                temperaturePointList.Sort();
                if (_runningParameters.sort != "ascend")
                {
                    // 降序
                    temperaturePointList.Reverse();
                }
 
            }
            catch (Exception ex)
            {

            }
        }
    }
}
