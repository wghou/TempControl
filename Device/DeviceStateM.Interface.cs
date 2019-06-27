using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device
{
    public partial class DeviceStateM
    {

        ///////////////////////////////////////////////////
        // public interface

        /// <summary>
        /// 查询控温点列表中的完成状态
        /// </summary>
        /// <param name="st"></param>
        public void CheckTpListFinishedStatus(out List<bool> st)
        {
            st = new List<bool>();
            lock (stepLocker)
            {
                // 获取当前温度点列表中的完成状态
                for (int i = 0; i < temperaturePointList.Count; i++)
                {
                    st.Add(temperaturePointList[i].finished);
                }
            }
        }

        public void StartAutoControl()
        {
            if (_state != State.Idle) return;
            _machine.Fire(Trigger.StartAutoStep);
        }

        public void StopAutoControl()
        {
            _machine.Fire(Trigger.SuspendAutoControl);
        }


        /// <summary>
        /// 写入继电器 2 状态
        /// 外部调用需注意 - 需异步调用，以防止界面线程阻塞
        /// 另：如果 cntErr == true，则由 ErrorMonitor 记录并处理错误，
        /// 并且只向 RelayDeviceMStatusUpdatedEvent 中反馈结果，不再反馈错误信息
        /// </summary>
        /// <param name="cntErr"> 是否在 ErrorMonitor 中记录错误 </param>
        public void WriteRelayDeviceM(bool cntErr)
        {
            if (ryDeviceM.Enable == false) return;

            RelayDevice.Err_r err = ryDeviceM.UpdateStatusToDevice();

            // 记录错误状态
            // 仅记录，在 timerTickEvent 中检查全局错误状态
            if (cntErr && err != RelayDevice.Err_r.NoError)
            {
                SetErrorStatus(ErrorCode.RelayError);
            }

            // 此处的逻辑：如果在 Device 内部由 ErrorMonitor 记录错误，
            // 则不向 RelayDeviceStatusUpdateEvent 反馈错误，以免在多个地方重复处理（提示）错误
            // 实际上，Error 应该定义成一个类，并标识出是否已处理
            if (cntErr) err = RelayDevice.Err_r.NoError;

            RelayDeviceMStatusUpdatedEvent?.Invoke(err, ryDeviceM.ryStatus);
        }


        /// <summary>
        /// 写入继电器 2 状态
        /// 外部调用需注意 - 需异步调用，以防止界面线程阻塞
        /// 另：如果 cntErr == true，则由 ErrorMonitor 记录并处理错误，
        /// 并且只向 RelayDeviceSStatusUpdatedEvent 中反馈结果，不再反馈错误信息
        /// </summary>
        /// <param name="cntErr"> 是否在 ErrorMonitor 中记录错误 </param>
        public void WriteRelayDeviceS(bool cntErr)
        {
            if (ryDeviceS.Enable == false) return;

            RelayDevice.Err_r err = ryDeviceS.UpdateStatusToDevice();

            // 记录错误状态
            // 仅记录，在 timerTickEvent 中检查全局错误状态
            if (cntErr && err != RelayDevice.Err_r.NoError)
            {
                SetErrorStatus(ErrorCode.RelayError);
            }

            // 此处的逻辑：如果在 Device 内部由 ErrorMonitor 记录错误，
            // 则不向 RelayDeviceStatusUpdateEvent 反馈错误，以免在多个地方重复处理（提示）错误
            // 实际上，Error 应该定义成一个类，并标识出是否已处理
            if (cntErr) err = RelayDevice.Err_r.NoError;

            RelayDeviceSStatusUpdatedEvent?.Invoke(err, ryDeviceS.ryStatus);
        }


        /// <summary>
        /// 写入控温板 1 参数 - 需要异步调用
        /// 另：如果 cntErr == true，则由 ErrorMonitor 记录并处理错误，
        /// 并且只向 TempDeviceMParamUpdatedEvent 中反馈结果，不再反馈错误信息
        /// </summary>
        /// <param name="cntErr"> 是否在 ErrorMonitor 中记录错误 </param>
        public void WriteTempDeviceM(bool cntErr)
        {
            if (tpDeviceM.Enable == false) return;

            TempProtocol.Err_t err = tpDeviceM.UpdateParamToDevice();

            // 记录错误状态
            // 仅记录，在 timerTickEvent 中检查全局错误状态
            if (cntErr && err != TempProtocol.Err_t.NoError)
            {
                SetErrorStatus(ErrorCode.TempParamSetError);
            }

            // 此处的逻辑：如果在 Device 内部由 ErrorMonitor 记录错误，
            // 则不向 TempDeviceMParamUpdatedEvent 反馈错误，以免在多个地方重复处理（提示）错误
            // 实际上，Error 应该定义成一个类，并标识出是否已处理
            if (cntErr == true) err = TempProtocol.Err_t.NoError;

            TempDeviceMParamUpdatedEvent?.Invoke(err, tpDeviceM.tpParam);
        }


        /// <summary>
        /// 读取控温板 1 参数 - 需要异步调用
        /// 另：如果 cntErr == true，则由 ErrorMonitor 记录并处理错误，
        /// 并且只向 TempDeviceMParamUpdatedEvent 中反馈结果，不再反馈错误信息
        /// </summary>
        /// <param name="cntErr"> 是否在 ErrorMonitor 中记录错误 </param>
        public void ReadTempDeviceM(bool cntErr)
        {
            if (tpDeviceM.Enable == false) return;

            TempProtocol.Err_t err = tpDeviceM.UpdateParamFromDevice();

            // 记录错误状态
            // 仅记录，在 timerTickEvent 中检查全局错误状态
            if (cntErr && err != TempProtocol.Err_t.NoError)
            {
                SetErrorStatus(ErrorCode.TemptError);
            }

            // 此处的逻辑：如果在 Device 内部由 ErrorMonitor 记录错误，
            // 则不向 TempDeviceMParamUpdatedEvent 反馈错误，以免在多个地方重复处理（提示）错误
            // 实际上，Error 应该定义成一个类，并标识出是否已处理
            if (cntErr == true) err = TempProtocol.Err_t.NoError;

            TempDeviceMParamUpdatedEvent?.Invoke(err, tpDeviceM.tpParam);
        }
    }
}
