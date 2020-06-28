using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device.IotPort
{
    /// <summary>
    /// 指令类型
    /// </summary>
    public enum IotCmd : int
    {
        /// <summary> 空指令 </summary>
        None = 0,
        /// <summary> 开始控温 </summary>
        Start = 1,
        /// <summary> 当前温度值 </summary>
        Tempt = 2,
        /// <summary> 温度点 </summary>
        Point = 3,
        /// <summary> 停止控温 </summary>
        Stop = 4
    }

    public partial class IotPortServer
    {
        /// <summary>
        /// 指令字符串
        /// </summary>
        private readonly string[] CmdStr = new string[] { "None", "Start", "Tempt", "Point", "Stop" };
        /// <summary>
        /// 指令分隔符
        /// </summary>
        private char CmdSpChar = ':';
        /// <summary>
        /// 指令结尾符号
        /// </summary>
        private string CmdEndStr = "@";
    }

    /// <summary>
    /// 事件参量
    /// </summary>
    public class IotPortEventArgs : EventArgs
    {
        /// <summary>
        /// 指令名称
        /// </summary>
        public IotCmd cmd { set; get; } = IotCmd.None;
        /// <summary>
        /// 是否发生错误
        /// </summary>
        public IotError err { set; get; } = IotError.NoErr;
        /// <summary>
        /// 与指令相关的数据值
        /// </summary>
        public float value { set; get; } = 0.0f;
    }

    /// <summary>
    /// 错误类型
    /// </summary>
    public enum IotError : int
    {
        /// <summary> 无错误 </summary>
        NoErr = 0,
        /// <summary> 超时错误 </summary>
        TimeOut = 1,
        /// <summary> 串口错误 </summary>
        ComErr = 2,
        /// <summary> 未能识别的指令 </summary>
        UnknowCmd = 3,
        /// <summary> 异常 </summary>
        Exception = 4,
        /// <summary> 代码错误 </summary>
        CodeErr = 5
    }
}
