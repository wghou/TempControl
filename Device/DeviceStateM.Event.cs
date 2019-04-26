using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device
{
    public partial class DeviceStateM
    {
        //
        public delegate void TimerTickEventHandler();
        /// <summary>
        /// 时刻触发事件
        /// </summary>
        public event TimerTickEventHandler TimerTickEvent;


        // 
        public delegate void StateChangedEventHandler(State st);
        /// <summary>
        /// 状态改变事件
        /// </summary>
        public event StateChangedEventHandler StateChangedEvent;


        //
        public delegate void ErrorStatusChangedEventHandler(Dictionary<ErrorCode, uint> errDict);
        /// <summary>
        /// 错误状态改变事件
        /// </summary>
        public event ErrorStatusChangedEventHandler ErrorStatusChangedEvent;


        //
        public delegate void TempDeviceParamUpdatedEventHandler(Device.TempProtocol.Err_t err, float[] param);
        /// <summary>
        /// 控温板参数更新事件 - 写入/读取
        /// </summary>
        public event TempDeviceParamUpdatedEventHandler TempDeviceParamUpdatedEvent;


        //
        public delegate void RelayDeviceStatusUpdatedEventHandler(Device.RelayDevice.Err_r err, bool[] ryStatus);
        /// <summary>
        /// 继电器状态更新事件
        /// </summary>
        public event RelayDeviceStatusUpdatedEventHandler RelayDeviceStatusUpdatedEvent;
    }
}
