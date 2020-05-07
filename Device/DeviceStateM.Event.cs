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
        public event TimerTickEventHandler TimerTickEndEvent;


        // 
        public delegate void StateChangedEventHandler(State st);
        /// <summary>
        /// 状态改变事件
        /// </summary>
        public event StateChangedEventHandler StateChangedEvent;


        // 
        public delegate void SampleStateChangedEventHandler(AutoSample.StateSample st);
        /// <summary>
        /// 状态改变事件
        /// </summary>
        public event SampleStateChangedEventHandler SampleStateChangedEvent;


        //
        public delegate void ErrorStatusChangedEventHandler(Dictionary<ErrorCode, uint> errDict);
        /// <summary>
        /// 错误状态改变事件
        /// </summary>
        public event ErrorStatusChangedEventHandler ErrorStatusChangedEvent;


        //
        public delegate void TempDeviceParamUpdatedEventHandler(Device.TempProtocol.Err_t err, float[] param);
        /// <summary>
        /// 主槽控温板参数更新事件 - 写入/读取
        /// </summary>
        public event TempDeviceParamUpdatedEventHandler TempDeviceMParamUpdatedEvent;
        /// <summary>
        /// 辅槽控温板参数更新事件 - 写入/读取
        /// </summary>
        public event TempDeviceParamUpdatedEventHandler TempDeviceSParamUpdatedEvent;


        //
        public delegate void RelayDeviceStatusUpdatedEventHandler(Device.RelayDevice.Err_r err, bool[] ryStatus);
        /// <summary>
        /// 继电器模块 1 状态更新事件
        /// </summary>
        public event RelayDeviceStatusUpdatedEventHandler RelayDeviceMStatusUpdatedEvent;
        /// <summary>
        /// 继电器模块 2 状态更新事件
        /// </summary>
        public event RelayDeviceStatusUpdatedEventHandler RelayDeviceSStatusUpdatedEvent;

        //
        public delegate void DeviceClosedEventHandler();
        /// <summary>
        /// 设备关闭触发事件
        /// </summary>
        public event DeviceClosedEventHandler DeviceClosedEvent;

        //
        public delegate void SensorIdentifiedEventHandler(SensorDevice.DeviceInfo info);
        /// <summary>
        /// 识别出传感器设备
        /// </summary>
        public event SensorIdentifiedEventHandler SensorIdentifiedEvent;
    }
}
