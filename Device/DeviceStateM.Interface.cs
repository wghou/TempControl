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
        /// 写入继电器 1 状态 - 需异步调用 - 外部接口，禁止内部调用
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

            // 如果记录错误，则不向主界面反馈错误
            if (cntErr) err = RelayDevice.Err_r.NoError;
            RelayDeviceMStatusUpdatedEvent?.Invoke(err, ryDeviceM.ryStatus);
        }


        /// <summary>
        /// 写入继电器 2 状态 - 需异步调用 - 外部接口，禁止内部调用
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

            // 如果记录错误，则不向主界面反馈错误
            if (cntErr) err = RelayDevice.Err_r.NoError;
            RelayDeviceSStatusUpdatedEvent?.Invoke(err, ryDeviceS.ryStatus);
        }


        /// <summary>
        /// 写入控温板 1 参数 - 需要异步调用
        /// </summary>
        /// <param name="cntErr"> 是否在 ErrorMonitor 中记录错误 </param>
        public void WriteTempDeviceM(bool cntErr)
        {
            TempProtocol.Err_t err = tpDeviceM.UpdateParamToDevice();

            TempDeviceMParamUpdatedEvent?.Invoke(err, tpDeviceM.tpParam);

            // 记录错误状态
            // 仅记录，在 timerTickEvent 中检查全局错误状态
            if (cntErr && err != TempProtocol.Err_t.NoError)
            {
                SetErrorStatus(ErrorCode.TempParamSetError);
            }
        }


        /// <summary>
        /// 读取控温板 1 参数 - 需要异步调用
        /// </summary>
        /// <param name="cntErr"> 是否在 ErrorMonitor 中记录错误 </param>
        public void ReadTempDeviceM(bool cntErr)
        {
            TempProtocol.Err_t err = tpDeviceM.UpdateParamFromDevice();

            TempDeviceMParamUpdatedEvent?.Invoke(err, tpDeviceM.tpParam);

            // 记录错误状态
            // 仅记录，在 timerTickEvent 中检查全局错误状态
            if (cntErr && err != TempProtocol.Err_t.NoError)
            {
                SetErrorStatus(ErrorCode.TemptError);
            }
        }


        /// <summary>
        /// 写入控温板 2 参数 - 需要异步调用
        /// </summary>
        /// <param name="cntErr"> 是否在 ErrorMonitor 中记录错误 </param>
        public void WriteTempDeviceS(bool cntErr)
        {
            if (tpDeviceS.Enable == false) return;

            TempProtocol.Err_t err = tpDeviceS.UpdateParamToDevice();

            TempDeviceSParamUpdatedEvent?.Invoke(err, tpDeviceS.tpParam);

            // 记录错误状态
            // 仅记录，在 timerTickEvent 中检查全局错误状态
            if (cntErr && err != TempProtocol.Err_t.NoError)
            {
                SetErrorStatus(ErrorCode.TempParamSetError);
            }
        }


        /// <summary>
        /// 读取控温板 2 参数 - 需要异步调用
        /// </summary>
        /// <param name="cntErr"> 是否在 ErrorMonitor 中记录错误 </param>
        public void ReadTempDeviceS(bool cntErr)
        {
            if (tpDeviceS.Enable == false) return;

            TempProtocol.Err_t err = tpDeviceS.UpdateParamFromDevice();

            TempDeviceSParamUpdatedEvent?.Invoke(err, tpDeviceS.tpParam);

            // 记录错误状态
            // 仅记录，在 timerTickEvent 中检查全局错误状态
            if (cntErr && err != TempProtocol.Err_t.NoError)
            {
                SetErrorStatus(ErrorCode.TemptError);
            }
        }
    }
}
