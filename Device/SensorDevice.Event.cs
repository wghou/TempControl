using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device
{
    public partial class SensorDevice
    {
        //
        public delegate void SensorTimerTickEndEventHandler();
        /// <summary>
        /// 时刻触发事件
        /// </summary>
        public event SensorTimerTickEndEventHandler SensorTimerTickEndEvent;


        //
        public delegate void SensorStateChangedEventHandler();
        /// <summary>
        /// 状态状变触发事件
        /// </summary>
        public event SensorStateChangedEventHandler SensorStateChangedEvent;


        //
        public delegate void SensorErrorEventHandler();
        /// <summary>
        /// 状态状变触发事件
        /// </summary>
        public event SensorErrorEventHandler SensorErrorEvent;

        //
        public delegate void SensorIdentifiedEventHandler(DeviceInfo info);
        /// <summary>
        /// 识别出传感器设备
        /// </summary>
        public event SensorIdentifiedEventHandler SensorIdentifiedEvent;
    }
}
