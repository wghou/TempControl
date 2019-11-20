using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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


            // json config file
            string confFile = "config.json";
            try
            {
                System.IO.StreamReader file = System.IO.File.OpenText(confFile);
                JsonTextReader reader = new JsonTextReader(file);
                JObject obj = (JObject)JToken.ReadFrom(reader);

                // 设置控温表
                if (obj.ContainsKey("TempDev"))
                {
                    JObject child = (JObject)obj["TempDev"];

                    // 设置主控温表
                    if (child.ContainsKey("TempM"))
                    {
                        JObject child2 = (JObject)child["TempM"];

                        confOK &= tpDeviceM.ConfigSyn(child2.ContainsKey("PortName") ? child2["PortName"].ToString() : "COM0");
                        tpDeviceM.Enable = child2.ContainsKey("Enable") ? (bool)child2["Enable"] : true;
                        if (!confOK) nlogger.Error("配置主槽控温设备失败! 端口号: " + tpDeviceM.tpDevicePortName);
                        else nlogger.Debug("配置主槽控温设备成功! 端口号: " + tpDeviceM.tpDevicePortName);

                        //sPortTm.PortName = child.ContainsKey("PortName") ? child["PortName"].ToString() : "COM0";
                        //sPortTm.BaudRate = child.ContainsKey("BaudRate") ? (int)child["BaudRate"] : 2400;
                        //sPortTm.DataBits = 8;
                        //sPortTm.StopBits = StopBits.One;
                        //sPortTm.Parity = Parity.None;
                        //sPortTm.ReadBufferSize = 64;
                        //sPortTm.WriteBufferSize = 64;
                        //SportTm_enable = child.ContainsKey("Enable") ? (bool)child["Enable"] : true;
                    }

                    // 设置辅控温表
                    if (child.ContainsKey("TempS"))
                    {
                        JObject child2 = (JObject)child["TempS"];

                        confOK &= tpDeviceS.ConfigSyn(child2.ContainsKey("PortName") ? child2["PortName"].ToString() : "COM0");
                        tpDeviceS.Enable = child2.ContainsKey("Enable") ? (bool)child2["Enable"] : true;
                        if (!confOK) nlogger.Error("配置辅槽控温设备失败! 端口号: " + tpDeviceM.tpDevicePortName);
                        else nlogger.Debug("配置辅槽控温设备成功! 端口号: " + tpDeviceM.tpDevicePortName);
                    }
                }

                // 设置继电器
                if (obj.ContainsKey("RelayDev"))
                {
                    JObject child = (JObject)obj["RelayDev"];

                    // 设置继电器1
                    if (child.ContainsKey("RelayM"))
                    {
                        JObject child2 = (JObject)child["RelayM"];

                        confOK &= ryDeviceM.SetPortName(child2.ContainsKey("PortName") ? child2["PortName"].ToString() : "COM0");
                        ryDeviceM.Enable = child2.ContainsKey("Enable") ? (bool)child2["Enable"] : true;
                        if (!confOK) nlogger.Error("配置主继电器失败! 端口号: " + ryDeviceM.ryDevicePortName);
                        else nlogger.Debug("配置主继电器成功! 端口号: " + ryDeviceM.ryDevicePortName);
                    }

                    // 设置继电器2
                    if (child.ContainsKey("RelayS"))
                    {
                        JObject child2 = (JObject)child["RelayS"];

                        confOK &= ryDeviceS.SetPortName(child2.ContainsKey("PortName") ? child2["PortName"].ToString() : "COM0");
                        ryDeviceS.Enable = child2.ContainsKey("Enable") ? (bool)child2["Enable"] : true;
                        if (!confOK) nlogger.Error("配置辅继电器失败! 端口号: " + ryDeviceS.ryDevicePortName);
                        else nlogger.Debug("配置辅继电器成功! 端口号: " + ryDeviceS.ryDevicePortName);
                    }
                }

                // 设置传感器
                if (obj.ContainsKey("SensorDev"))
                {
                    JObject child = (JObject)obj["SensorDev"];

                    // 测温电桥
                    if (child.ContainsKey("Bridge"))
                    {
                        JObject child2 = (JObject)child["Bridge"];

                        confOK &= tpBridge.ConfigSyn(child2.ContainsKey("PortName") ? child2["PortName"].ToString() : "COM0");
                        tpBridge.Enable = child2.ContainsKey("Enable") ? (bool)child2["Enable"] : true;
                        tpBridge.tpBridgeReadInterval = child2.ContainsKey("ReadInterval") ? (int)child2["ReadInterval"] : 2;
                        tpBridge.tpBridgeChannel = child2.ContainsKey("Channel") ? (int)child2["Channel"] : 0;
                        if (!confOK) nlogger.Error("配置测温点电桥失败! 端口号: " + tpBridge.tpBridgePortName);
                        else nlogger.Debug("配置测温电桥成功! 端口号: " + tpBridge.tpBridgePortName);
                    }
                }

                // 设置接口
                if (obj.ContainsKey("UserPort"))
                {
                    JObject child = (JObject)obj["UserPort"];

                    confOK &= _userPorts.configUserPorts(child);
                    if (!confOK) nlogger.Error("配置 UserPort 失败");
                    else nlogger.Debug("配置 UserPort 失败");

                    _userPorts.UserPortMsgRvEvent += _userPorts_UserPortMsgRvEvent;
                }
            }
            catch(Exception ex)
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

        public void closeDevice()
        {
            ryDeviceM.closeDevice();
            ryDeviceS.closeDevice();

            // 向 mqtt server 发布主题信息
            _userPorts.PublishMessage(UserPort.SubTopic.Data, packageDataJson(), true, UserPort.UserPortType.All);
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
