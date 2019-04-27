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
        /// 写入继电器状态 - 需异步调用 - 外部接口，禁止内部调用
        /// </summary>
        /// <param name="cntErr"> 是否在 ErrorMonitor 中记录错误 </param>
        public void WriteRelayDevice(bool cntErr)
        {
            RelayDevice.Err_r err = ryDevice.UpdateStatusToDevice();

            RelayDeviceStatusUpdatedEvent?.Invoke(err, ryDevice.ryStatus);

            // 记录错误状态
            // 仅记录，在 timerTickEvent 中检查全局错误状态
            if (cntErr && err != RelayDevice.Err_r.NoError)
            {
                SetErrorStatus(ErrorCode.RelayError);
            }
        }


        /// <summary>
        /// 写入控温板参数 - 需要异步调用
        /// </summary>
        /// <param name="cntErr"> 是否在 ErrorMonitor 中记录错误 </param>
        public void WriteTempDevice(bool cntErr)
        {
            TempProtocol.Err_t err = tpDeviceM.UpdateParamToDevice();

            TempDeviceParamUpdatedEvent?.Invoke(err, tpDeviceM.tpParam);

            // 记录错误状态
            // 仅记录，在 timerTickEvent 中检查全局错误状态
            if (cntErr && err != TempProtocol.Err_t.NoError)
            {
                SetErrorStatus(ErrorCode.TempParamSetError);
            }
        }


        /// <summary>
        /// 读取控温板参数 - 需要异步调用
        /// </summary>
        /// <param name="cntErr"> 是否在 ErrorMonitor 中记录错误 </param>
        public void ReadTempDevice(bool cntErr)
        {
            TempProtocol.Err_t err = tpDeviceM.UpdateParamFromDevice();

            TempDeviceParamUpdatedEvent?.Invoke(err, tpDeviceM.tpParam);

            // 记录错误状态
            // 仅记录，在 timerTickEvent 中检查全局错误状态
            if (cntErr && err != TempProtocol.Err_t.NoError)
            {
                SetErrorStatus(ErrorCode.TemptError);
            }
        }
    }
}
