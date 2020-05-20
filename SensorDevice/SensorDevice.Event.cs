using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorDevice
{
    public abstract partial class SensorDeviceStateM<TInfo, TData> : SensorDeviceBase
        where TInfo : SensorInfoBase
        where TData : SensorDataBase
    {
        public delegate void ErrorOccurEventHandler(Err_sr err);
        /// <summary>
        /// 发生错误 - 事件
        /// </summary>
        public event ErrorOccurEventHandler ErrorOccurEvent;
        /// <summary>
        /// 事件传递函数
        /// </summary>
        /// <param name="err"></param>
        protected virtual void OnErrorOccur(Err_sr err)
        {
            ErrorOccurEvent?.Invoke(err);
        }

        public delegate void DataReceivedEventHandler(TData data);
        /// <summary>
        /// 传感器接收到数据 - 事件
        /// </summary>
        public event DataReceivedEventHandler DataReceivedEvent;
        /// <summary>
        /// 事件传递函数
        /// </summary>
        /// <param name="data"></param>
        protected virtual void OnDataReceived(TData data)
        {
            DataReceivedEvent?.Invoke(data);
        }

        public delegate void StateChangedEventHandler(StateSensor st);
        /// <summary>
        /// 传感器接收到数据 - 事件
        /// </summary>
        public event StateChangedEventHandler StateChangedEvent;
    }
}
