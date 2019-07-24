using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device
{
    public partial class DeviceStateM
    {
        /// <summary>
        /// （通过配置文件）配置设备参数
        /// </summary>
        /// <param name="configFilePath">配置文件路径</param>
        /// <returns></returns>
        public bool Configure(string configFilePath = @"./config.ini")
        {
            // 配置成功标志位
            bool confOK = true;

            // 读取配置运行参数
            if (_runningParameters.ReadValueConfig(configFilePath) == false) nlogger.Warn("读取配置文件错误，使用默认参数运行");

            _tickTimer.Interval = _runningParameters.readTempIntervalSec * 1000;

            try
            {
                //////////////////////////////////////////
                // 配置参数
                // 主槽控温设备
                confOK &= tpDeviceM.ConfigSyn(_runningParameters.portTp1);
                if (!confOK) nlogger.Error("配置主槽控温设备失败! 端口号: " + tpDeviceM.tpDevicePortName);
                else nlogger.Debug("配置主槽控温设备成功! 端口号: " + tpDeviceM.tpDevicePortName);

                // 辅槽控温设备
                confOK &= tpDeviceS.ConfigSyn(_runningParameters.portTp2);
                if (!confOK) nlogger.Error("配置辅槽控温设备失败! 端口号: " + tpDeviceS.tpDevicePortName);
                else nlogger.Debug("配置辅槽控温设备成功! 端口号: " + tpDeviceS.tpDevicePortName);

                // 继电器设备 1
                confOK &= ryDeviceM.SetPortName(_runningParameters.portRy1);
                if (!confOK) nlogger.Error("配置继电器设备 1 失败! 端口号: " + ryDeviceM.ryDevicePortName);
                else nlogger.Debug("配置继电器设备 1 成功! 端口号: " + ryDeviceM.ryDevicePortName);

                // 继电器设备 2
                //confOK &= ryDeviceS.SetPortName(_runningParameters.portRy2);
                //if (!confOK) nlogger.Error("配置继电器设备 2 失败! 端口号: " + ryDeviceS.ryDevicePortName);
                //else nlogger.Debug("配置继电器设备 2 成功! 端口号: " + ryDeviceM.ryDevicePortName);
            }
            catch (Exception ex)
            {
                nlogger.Error("从配置文件读取参数过程中发生异常：" + ex.Message.ToString());
                confOK = false;
            }

            return confOK;
        }


        /// <summary>
        /// 刷新事件，使主界面更新状态
        /// </summary>
        public void updateEvents()
        {
            StateChangedEvent?.Invoke(_state);

            TempDeviceMParamUpdatedEvent?.Invoke(TempProtocol.Err_t.NoError, tpDeviceM.tpParam);

            TempDeviceSParamUpdatedEvent?.Invoke(TempProtocol.Err_t.NoError, tpDeviceS.tpParam);

            RelayDeviceMStatusUpdatedEvent?.Invoke(RelayDevice.Err_r.NoError, ryDeviceM.ryStatus);

            RelayDeviceSStatusUpdatedEvent?.Invoke(RelayDevice.Err_r.NoError, ryDeviceS.ryStatus);
        }


        /// <summary>
        /// _stateM 开始自动控温流程
        /// </summary>
        public void StartAutoControl()
        {
            if (_state != State.Idle) return;
            _machine.Fire(Trigger.StartAutoStep);
        }


        /// <summary>
        /// _stateM 暂停自动控温流程，进入 空闲 状态
        /// </summary>
        public void SuspendAutoControl()
        {
            _machine.Fire(Trigger.SuspendAutoControl);
        }


        /// <summary>
        /// _stateM 停止控温，并关闭计算机
        /// </summary>
        public void ShutdownComputer()
        {
            _machine.Fire(Trigger.ForceShutdownPC);
        }


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


        /// <summary>
        /// 写入控温板 2 参数 - 需要异步调用
        /// 另：如果 cntErr == true，则由 ErrorMonitor 记录并处理错误，
        /// 并且只向 TempDeviceSParamUpdatedEvent 中反馈结果，不再反馈错误信息
        /// </summary>
        /// <param name="cntErr"> 是否在 ErrorMonitor 中记录错误 </param>
        public void WriteTempDeviceS(bool cntErr)
        {
            if (tpDeviceS.Enable == false) return;

            TempProtocol.Err_t err = tpDeviceS.UpdateParamToDevice();

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

            TempDeviceSParamUpdatedEvent?.Invoke(err, tpDeviceS.tpParam);
        }


        /// <summary>
        /// 读取控温板 2 参数 - 需要异步调用
        /// 另：如果 cntErr == true，则由 ErrorMonitor 记录并处理错误，
        /// 并且只向 TempDeviceSParamUpdatedEvent 中反馈结果，不再反馈错误信息
        /// </summary>
        /// <param name="cntErr"> 是否在 ErrorMonitor 中记录错误 </param>
        public void ReadTempDeviceS(bool cntErr)
        {
            if (tpDeviceS.Enable == false) return;

            TempProtocol.Err_t err = tpDeviceS.UpdateParamFromDevice();

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

            TempDeviceSParamUpdatedEvent?.Invoke(err, tpDeviceS.tpParam);
        }
    }
}
