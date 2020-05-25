using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstDevice
{
    public abstract partial class InstDeviceStateM<TInfo, TData> : InstDeviceBase
        where TInfo : InstInfoBase
        where TData : InstDataBase
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
        /// 仪器接收到数据 - 事件
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

        public delegate void StateChangedEventHandler(StateInst st);
        /// <summary>
        /// 仪器接收到数据 - 事件
        /// </summary>
        public event StateChangedEventHandler StateChangedEvent;
    }
}
