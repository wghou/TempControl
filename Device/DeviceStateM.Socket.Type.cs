﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device
{
    /// <summary>
    /// 通信所用的指令
    /// </summary>
    public enum SocketCmd : int
    {
        /// <summary> 开始自动控温 </summary>
        AutoStart = 0,
        /// <summary> 停止 </summary>
        Stop,
        /// <summary> 测量完成 </summary>
        Finished,
        /// <summary> 测试 Idx </summary>
        TestId,
        /// <summary> 设备状态 </summary>
        DeviceState,
        /// <summary> 传感器部件状态 </summary>
        DeviceStatus,
        /// <summary> 未知 </summary>
        Unknown
    }

    /// <summary>
    /// 用于 Socket 通信的数据格式 - 命令
    /// </summary>
    public class SocketCmdMessage
    {
        /// <summary> Socket Message 的类型 </summary>
        public SocketCmd cmdType { get; set; } = SocketCmd.Unknown;
        /// <summary> 该条指令是否正确执行 </summary>
        public bool ExecuteSucceed { set; get; } = false;

        public SocketCmdMessage(SocketCmd tp) { cmdType = tp; }
    }

    /// <summary>
    /// 用于 Socket 通信的数据格式 - TestIdx
    /// </summary>
    public class SocketTestIdxMessage : SocketCmdMessage
    {
        public SocketTestIdxMessage() : base(SocketCmd.TestId) { }

        /// <summary> 测试编号 </summary>
        public string TestIdx { get; set; }
    }

    /// <summary>
    /// 用于 Socket 通信的数据格式 - 设备状态
    /// </summary>
    public class SocketStateMessage : SocketCmdMessage
    {
        public SocketStateMessage() : base(SocketCmd.DeviceState) { }

        /// <summary> 设备状态 </summary>
        public State devSt { set; get; } = State.Idle;
        /// <summary> 是否发生了错误：true 表示发生了错误 </summary>
        public bool ErrorSt { set; get; } = false;
    }

    public class SocketStatusMessage : SocketCmdMessage
    {
        public SocketStatusMessage() : base(SocketCmd.DeviceStatus) { }

        public float ZhucaoTemperature = 35.0000f;//主槽设定温度
        public float ZhucaoTemperatureReal = 35.0000f;//主槽当前温度
        public int ZhucaoPower = 0; //主槽加热功率
        public float FucaoTemperature = 34.0000f; //辅槽设定温度
        public float FucaoTemperatureReal = 34.0000f; //辅槽当前温度
        public int FucaoPower = 0; //辅槽加热功率
        public bool Zongdianyuan = false; //总电源开关
        public bool Zhucaozhileng = false; //主槽制冷开关
        public bool Zhucaokongwen = false; //主槽控温开关
        public bool Fucaokongwen = false; //辅槽控温开关
        public bool Fucaozhileng = false; //辅槽制冷开关
        public bool Fucaoxunhuan = false; //辅槽循环开关
        public bool Fucaokuaileng = false; //辅槽快冷开关
        public bool Zhucaokuaileng = false; //主槽快冷开关
        public DateTime DataTime = DateTime.Now;//数据采集时间
        public double Temperature = double.NaN;//标准温度
        public double Conductivity = double.NaN;//标准电导率
        public double Salinity = double.NaN;//标准盐度
    }

    /*
    Example:
    private MySocketClient _socketClient = new MySocketClient();

    // 初始化
    bool confOK = _socketClient.Init(cfg);
    _socketClient.MessageReceievedEvent += _socketClient_MessageReceievedEvent;
     
    // 发送指令
    SocketCmdMessage msg1 = new SocketCmdMessage(cmd);
    _socketClient.pushMessage(JObject.FromObject(msg1));

    // 解析收到的指令
    SocketCmdMessage msg = message.ToObject<SocketCmdMessage>();
    switch (msg.cmdType)
    {
             
        // 开始控温流程
        case SocketCmd.AutoStart:
            break;
                
        // 停止控温流程
        case SocketCmd.Stop:
            break;

        // 读取仪器信息
        case SocketCmd.TestId:
            // 解析数据
            SocketTestIdxMessage msgSend1 = message.ToObject<SocketTestIdxMessage>();
            break;

        case SocketCmd.DeviceState:
            // 解析数据
            SocketStateMessage msgSend2 = message.ToObject<SocketStateMessage>();
            break;

        default:
            nlogger.Error("unknow socket cmd: " + msg.cmdType.ToString());
            break;
    }
     */
}
