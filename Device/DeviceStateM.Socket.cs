using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Others;
using System.IO;
using SensorDevice;

namespace Device
{
    /// <summary>
    /// 通信所用的指令
    /// </summary>
    public enum SocketCmd : int
    {
        /// <summary> 开始自动控温 </summary>
        AutoStart = 0,
        /// <summary> 暂停 </summary>
        Suspend,
        /// <summary> 停止 </summary>
        Stop,
        /// <summary> 测量完成 </summary>
        Finished,
        /// <summary> 传感器状态 </summary>
        SensorInfo,
        /// <summary> 未知 </summary>
        Unknown
    }

    /// <summary>
    /// 用于 Socket 通信的数据格式 - 基类 - json convertor
    /// </summary>
    public abstract class SocketMessageBase
    {
        /// <summary> Socket Message 的类型 </summary>
        public SocketCmd cmdType { get; set; } = SocketCmd.Unknown;

        public SocketMessageBase(SocketCmd tp) { cmdType = tp; }
    }

    /// <summary>
    /// socket 指令返回数据
    /// </summary>
    public class SocketCmdMessage : SocketMessageBase
    {
        public SocketCmdMessage(SocketCmd cmd) : base(cmd) { }

        /// <summary> 控温设备当前的状态 </summary>
        public State deviceState { get; set; }
    }

    public class SocketSensorMessage : SocketMessageBase
    {
        public SocketSensorMessage(SocketCmd cmd) : base(cmd) { }

        /// <summary> 传感器设备的状态 </summary>
        public List<SensorInfo> sensorStates { get; set; }
    }


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
        private void _socketServer_MessageReceievedEvent(JObject message)
        {
            // 解析收到的指令
            SocketCmdMessage msg = message.ToObject<SocketCmdMessage>();

            switch (msg.cmdType)
            {
                // 开始控温流程
                case SocketCmd.AutoStart:
                    if(_state == State.Idle)
                    {
                        loadTempPointList();
                        StartAutoControl();
                    }
                    break;

                // 暂停控温流程
                case SocketCmd.Suspend:
                    SuspendAutoControl();
                    break;
                
                // 停止控温流程
                case SocketCmd.Stop:
                    ShutdownComputer();
                    break;

                // 读取传感器信息
                case SocketCmd.SensorInfo:
                    List<SensorInfo> states = new List<SensorInfo>();
                    SocketSensorMessage srMsg = new SocketSensorMessage(SocketCmd.SensorInfo);
                    foreach (var itm in srDevices)
                    {
                        switch (itm.sensorType)
                        {
                            case SensorType.SBE37SI:
                                states.Add((itm as SensorSBE37).Info);
                                break;
                            case SensorType.Standard:

                                break;
                            case SensorType.Undefined:

                                break;
                        }
                    }
                    srMsg.sensorStates = states;
                    _socketServer.pushMessage(JObject.FromObject(srMsg));
                    break;

                default:
                    nlogger.Error("unknow socket cmd: " + msg.cmdType.ToString());
                    break;
            }

            // 返回收到的指令
            switch (msg.cmdType)
            {
                case SocketCmd.AutoStart:
                case SocketCmd.Suspend:
                case SocketCmd.Stop:
                    SocketCmdMessage msgSend = new SocketCmdMessage(msg.cmdType);
                    msgSend.deviceState = _state;
                    _socketServer.pushMessage(JObject.FromObject(msgSend));
                    break;
            }
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
