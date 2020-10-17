using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using InstDevice;

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

                        bool rlt = tpDeviceM.ConfigSyn(child2.ContainsKey("PortName") ? child2["PortName"].ToString() : "COM0");
                        tpDeviceM.Enable = child2.ContainsKey("Enable") ? (bool)child2["Enable"] : true;
                        if (!rlt) nlogger.Error("配置主槽控温设备失败! 端口号: " + tpDeviceM.tpDevicePortName);
                        else nlogger.Debug("配置主槽控温设备成功! 端口号: " + tpDeviceM.tpDevicePortName);
                        confOK &= rlt;
                    }

                    // 设置辅控温表
                    if (child.ContainsKey("TempS"))
                    {
                        JObject child2 = (JObject)child["TempS"];

                        bool rlt = tpDeviceS.ConfigSyn(child2.ContainsKey("PortName") ? child2["PortName"].ToString() : "COM0");
                        tpDeviceS.Enable = child2.ContainsKey("Enable") ? (bool)child2["Enable"] : true;
                        if (!rlt) nlogger.Error("配置辅槽控温设备失败! 端口号: " + tpDeviceM.tpDevicePortName);
                        else nlogger.Debug("配置辅槽控温设备成功! 端口号: " + tpDeviceM.tpDevicePortName);
                        confOK &= rlt;
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

                        bool rlt = ryDeviceM.SetPortName(child2.ContainsKey("PortName") ? child2["PortName"].ToString() : "COM0");
                        ryDeviceM.Enable = child2.ContainsKey("Enable") ? (bool)child2["Enable"] : true;
                        if (!rlt) nlogger.Error("配置主继电器失败! 端口号: " + ryDeviceM.ryDevicePortName);
                        else nlogger.Debug("配置主继电器成功! 端口号: " + ryDeviceM.ryDevicePortName);
                        confOK &= rlt;
                    }

                    // 设置继电器2
                    if (child.ContainsKey("RelayS"))
                    {
                        JObject child2 = (JObject)child["RelayS"];

                        bool rlt = ryDeviceS.SetPortName(child2.ContainsKey("PortName") ? child2["PortName"].ToString() : "COM0");
                        ryDeviceS.Enable = child2.ContainsKey("Enable") ? (bool)child2["Enable"] : true;
                        if (!rlt) nlogger.Error("配置辅继电器失败! 端口号: " + ryDeviceS.ryDevicePortName);
                        else nlogger.Debug("配置辅继电器成功! 端口号: " + ryDeviceS.ryDevicePortName);
                        confOK &= rlt;
                    }
                }

                // 设置仪器
                if (obj.ContainsKey("InstDev"))
                {
                    bool rlt = false;
                    JArray child = (JArray)obj["InstDev"];

                    if (obj.ContainsKey("InstDeviceCmd"))
                    {
                        JObject cmd = (JObject)obj["InstDeviceCmd"];
                        rlt = initInstDevices(child, cmd);
                    }
                    else
                    {
                        rlt = initInstDevices(child);
                    }
                    
                    if (!rlt) nlogger.Error("配置仪器失败!");
                    else nlogger.Debug("配置仪器成功!");
                    confOK &= rlt;
                }

                // 配置 IotService
                {
                    // 启动 IotService
                    bool rlt = InitIotService();
                    if (rlt == true) nlogger.Debug("配置 IotService 成功！");
                    else nlogger.Error("配置 IotService 失败！");
                    confOK &= rlt;
                }
                

                // 设置iot接口
                if (obj.ContainsKey("IotPort"))
                {
                    JObject child = (JObject)obj["IotPort"];

                    bool rlt = InitIotPort(child);
                    if (!rlt) nlogger.Error("配置 Iot 接口失败!");
                    else nlogger.Debug("配置 Iot 接口成功!");
                    confOK &= rlt;
                }

                // 设置 socket 接口
                if (obj.ContainsKey("Socket"))
                {
                    JObject child = (JObject)obj["Socket"];

                    bool rlt = InitSocketServer(child);
                    if (!rlt) nlogger.Error("配置 Socket 接口失败!");
                    else nlogger.Debug("配置 Socket 接口成功!");
                    confOK &= rlt;
                }

                {
                    bool rlt = readTempPointList();
                    if(!rlt) nlogger.Error("读取温度点列表失败!");
                    else nlogger.Debug("读取温度点列表成功!");
                    confOK &= rlt;
                }

                if (obj.ContainsKey("AutoSample"))
                {
                    JObject child = (JObject)obj["AutoSample"];

                    confOK &= ReadSampleConfig(child);
                    if (!confOK) nlogger.Error("配置 AutoSample 失败");
                    else nlogger.Debug("配置 AutoSample 失败");
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


        /// <summary>
        /// 退出系统
        /// </summary>
        public void ExitDevice()
        {
            _machine.Fire(Trigger.SuspendAutoControl);

            ryDeviceM.closeDevice();
            ryDeviceS.closeDevice();

            DeviceClosedEvent?.Invoke();
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
