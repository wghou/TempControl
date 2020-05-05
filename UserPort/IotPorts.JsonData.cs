using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotPort
{
    /// <summary>
    /// 数据的类型/主题
    /// </summary>
    public enum Topic : int
    {
        /// <summary> 控温表数据 </summary>
        ParamT = 0,
        /// <summary> 继电器数据 </summary>
        Relay = 1,
        /// <summary> 错误信息 </summary>
        Error,
        /// <summary> 自动控温步骤 </summary>
        AutoState
    }

    /// <summary>
    /// 用于区别数据是用于显示，还是设置
    /// </summary>
    public enum DorS : int
    {
        /// <summary>  数据用于显示 </summary>
        Display = 0,
        /// <summary>  数据用于设置 </summary>
        Set = 1
    }

    /// <summary>
    /// 控温表数据
    /// </summary>
    public class ParamT
    {
        /// <summary>
        /// 温控设备的参数值
        /// </summary>
        private float[] param = new float[10];

        /// <summary>温度设定值</summary>
        public float TemptSet { get { return param[0]; } set { param[0] = value; } }
        /// <summary>温度调整值</summary>
        public float TempCorrect { get { return param[1]; } set { param[1] = value; } }
        /// <summary>
        /// 超前调整值
        /// </summary>
        public float LeadAdjust { get { return param[2]; } set { param[2] = value; } }
        /// <summary>
        /// 模糊系数
        /// </summary>
        public float Fuzzy { get { return param[3]; } set { param[3] = value; } }
        /// <summary>
        /// 比例系数
        /// </summary>
        public float Ratio { get { return param[4]; } set { param[4] = value; } }
        /// <summary>
        /// 积分系数
        /// </summary>
        public float Integration { get { return param[5]; } set { param[5] = value; } }
        /// <summary>
        /// 功率系数
        /// </summary>
        public float Power { get { return param[6]; } set { param[6] = value; } }
        /// <summary>
        /// 当前温度值
        /// </summary>
        public float TempShow { get { return param[7]; } set { param[7] = value; } }
        /// <summary>
        /// 当前功率值
        /// </summary>
        public float PowerShow { get { return param[8]; } set { param[8] = value; } }
        /// <summary>
        /// 五分钟波动度
        /// </summary>
        public float Fluc { get { return param[9]; } set { param[9] = value; } }

        public float[] getValue() { return param; }
        public bool setValue(float[] val)
        {
            try
            {
                val.CopyTo(param, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// 用于传输温控表数据的 Json 字符串类
    /// </summary>
    public class JsonParamTs
    {
        /// <summary>
        /// 该数据用于显示 or 设置
        /// </summary>
        public DorS d_s { set; get; }

        /// <summary>
        /// 主槽控温表数据
        /// </summary>
        public ParamT paramM { set; get; }

        /// <summary>
        /// 辅槽控温表数据
        /// </summary>
        public ParamT paramS { set; get; }
    }


    /// <summary>
    /// 十六个继电器的类
    /// </summary>
    public class Relay16
    {
        private bool[] st = new bool[16];

        /// <summary> 继电器 0 </summary>
        public bool relay_0 { get { return st[0]; } set { st[0] = value; } }

        /// <summary> 继电器 1 </summary>
        public bool relay_1 { get { return st[1]; } set { st[1] = value; } }

        /// <summary> 继电器 2 </summary>
        public bool relay_2 { get { return st[2]; } set { st[2] = value; } }

        /// <summary> 继电器 3 </summary>
        public bool relay_3 { get { return st[3]; } set { st[3] = value; } }

        /// <summary> 继电器 4 </summary>
        public bool relay_4 { get { return st[4]; } set { st[4] = value; } }

        /// <summary> 继电器 5 </summary>
        public bool relay_5 { get { return st[5]; } set { st[5] = value; } }

        /// <summary> 继电器 6 </summary>
        public bool relay_6 { get { return st[6]; } set { st[6] = value; } }

        /// <summary> 继电器 7 </summary>
        public bool relay_7 { get { return st[7]; } set { st[7] = value; } }

        /// <summary> 继电器 8 </summary>
        public bool relay_8 { get { return st[8]; } set { st[8] = value; } }

        /// <summary> 继电器 9 </summary>
        public bool relay_9 { get { return st[9]; } set { st[9] = value; } }

        /// <summary> 继电器 10 </summary>
        public bool relay_10 { get { return st[10]; } set { st[10] = value; } }

        /// <summary> 继电器 11 </summary>
        public bool relay_11 { get { return st[11]; } set { st[11] = value; } }

        /// <summary> 继电器 12 </summary>
        public bool relay_12 { get { return st[12]; } set { st[12] = value; } }

        /// <summary> 继电器 13 </summary>
        public bool relay_13 { get { return st[13]; } set { st[13] = value; } }

        /// <summary> 继电器 14 </summary>
        public bool relay_14 { get { return st[14]; } set { st[14] = value; } }

        /// <summary> 继电器 15 </summary>
        public bool relay_15 { get { return st[15]; } set { st[15] = value; } }

        public bool[] getValue() { return st; }
        public bool setValue(bool[] val)
        {
            try
            {
                val.CopyTo(st, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }
    }


    /// <summary>
    /// 八个继电器的类
    /// </summary>
    public class Relay8
    {
        private bool[] st = new bool[8];

        /// <summary> 继电器 0 </summary>
        public bool relay_0 { get { return st[0]; } set { st[0] = value; } }

        /// <summary> 继电器 1 </summary>
        public bool relay_1 { get { return st[1]; } set { st[1] = value; } }

        /// <summary> 继电器 2 </summary>
        public bool relay_2 { get { return st[2]; } set { st[2] = value; } }

        /// <summary> 继电器 3 </summary>
        public bool relay_3 { get { return st[3]; } set { st[3] = value; } }

        /// <summary> 继电器 4 </summary>
        public bool relay_4 { get { return st[4]; } set { st[4] = value; } }

        /// <summary> 继电器 5 </summary>
        public bool relay_5 { get { return st[5]; } set { st[5] = value; } }

        /// <summary> 继电器 6 </summary>
        public bool relay_6 { get { return st[6]; } set { st[6] = value; } }

        /// <summary> 继电器 7 </summary>
        public bool relay_7 { get { return st[7]; } set { st[7] = value; } }

        public bool[] getValue() { return st; }
        public bool setValue(bool[] val)
        {
            try
            {
                Array.Copy(val, st, 8);
                //val.CopyTo(st, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }
    }

    // <summary>
    /// 用于传输继电器数据的 Json 字符串类
    /// </summary>
    public class JsonRelay88
    {
        /// <summary>
        /// 该数据用于显示 or 设置
        /// </summary>
        public DorS d_s { set; get; }

        /// <summary>
        /// 主继电器板卡
        /// </summary>
        public Relay8 relayM { set; get; }

        /// <summary>
        /// 辅继电器板卡
        /// </summary>
        public Relay8 relayS { set; get; }
    }


    /// <summary>
    /// 设备状态 / forked from Device.DeviceStateM.Stateless
    /// </summary>
    public enum DeviceState : int
    {
        /// <summary> 开始 </summary>
        Start,
        /// <summary> 升温 </summary>
        TempUp,
        /// <summary> 降温 </summary>
        TempDown,
        /// <summary> 控温 </summary>
        Control,
        /// <summary> 稳定 </summary>
        Stable,
        /// <summary> 测量 </summary>
        Measure,
        /// <summary> 停止 </summary>
        ShutdownPC,
        /// <summary> 空闲 </summary>
        Idle
    }

    /// <summary>
    /// 用于传输控温状态数据的 Json 字符串类
    /// </summary>
    public class JsonAutoState
    {
        /// <summary>
        /// 该数据用于显示 or 设置
        /// </summary>
        public DorS d_s { set; get; }

        /// <summary>
        /// 当前自动控温状态
        /// </summary>
        public DeviceState state { set; get; }
    }

    /// <summary>
    /// 系统错误代码 / forked form Device.DeviceStateM.ErrorMonitor
    /// </summary>
    public enum ErrorCode : int
    {
        /// <summary>
        /// 温度不降
        /// </summary>
        TempNotDown = 0,
        /// <summary>
        /// 温度持续下降
        /// </summary>
        TempNotUp,
        /// <summary>
        /// 温度波动过大
        /// </summary>
        TempFlucLarge,
        /// <summary>
        /// 温度持续上升
        /// </summary>
        TempBasis,
        /// <summary>
        /// 控温槽中的温度超出界限
        /// </summary>
        TempOutRange,
        /// <summary>
        /// 读电桥温度错误
        /// </summary>
        SensorError,
        /// <summary>
        /// 继电器设备错误
        /// </summary>
        RelayError,
        /// <summary>
        /// 温控设备错误
        /// </summary>
        TemptError,
        /// <summary>
        /// 温控设备参数写入错误
        /// </summary>
        TempParamSetError,
        /// <summary>
        /// 测温电桥错误
        /// </summary>
        BridgeError,
        /// <summary>
        /// 其他错误
        /// </summary>
        CodeError,
        /// <summary>
        /// 清除所有错误状态
        /// </summary>
        ClearErrorStatus
    }


    public class JsonError
    {
        /// <summary>
        /// 该数据用于显示 or 设置
        /// </summary>
        public DorS d_s { set; get; }

        /// <summary>
        /// 当前的错误状态
        /// </summary>
        public Dictionary<ErrorCode, uint> errCnt { set; get; }
    }
}
