using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stateless;
using System.Diagnostics;
using System.Timers;
using System.IO;

namespace Device
{
    /// <summary>
    /// 附带状态机的设备
    /// </summary>
    public partial class DeviceStateM
    {
        public DeviceStateM()
        {
            // 配置设备状态机
            ConfigStateless();

            // 配置设备错误状态监测器
            ResetErrorStatus();

            startTime = DateTime.Now;
        }


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
            if (_runningParameters.ReadValueConfig(configFilePath) == false) Utils.Logger.Sys("读取配置文件错误，使用默认参数运行");

            _tickTimer.Interval = _runningParameters.readTempIntervalSec * 1000;

            try
            {
                //////////////////////////////////////////
                // 配置参数
                // 主槽控温设备
                confOK &= tpDeviceM.ConfigSyn(_runningParameters.portTp1);
                if (!confOK) Utils.Logger.Sys("配置主槽控温设备失败! 端口号: " + tpDeviceM.tpDevicePortName);
                else Debug.WriteLine("配置主槽控温设备成功! 端口号: " + tpDeviceM.tpDevicePortName);

                if(tpDeviceM.Enable == true)
                {
                    if (tpDeviceM.UpdateParamFromDevice() != TempProtocol.Err_t.NoError)
                    {
                        Utils.Logger.Sys("初始化时，从主槽控温设备读取参数失败");
                        Debug.WriteLine("初始化时，从主槽控温设备读取参数失败");
                        confOK = false;
                    }
                }

                // 辅槽控温设备
                confOK &= srDevice.ConfigSyn(_runningParameters.portTp2);
                if (!confOK) Utils.Logger.Sys("配置传感器设备失败! 端口号: " + srDevice.srDevicePortName);
                else Debug.WriteLine("配置传感器设备成功! 端口号: " + srDevice.srDevicePortName);

                // 继电器设备 1
                confOK &= ryDeviceM.SetPortName(_runningParameters.portRy1);
                if (!confOK) Utils.Logger.Sys("配置继电器设备 1 失败! 端口号: " + ryDeviceM.ryDevicePortName);
                else Debug.WriteLine("配置继电器设备 1 成功! 端口号: " + ryDeviceM.ryDevicePortName);

                // 继电器设备 2
                confOK &= ryDeviceS.SetPortName(_runningParameters.portRy2);
                if (!confOK) Utils.Logger.Sys("配置继电器设备 2 失败! 端口号: " + ryDeviceS.ryDevicePortName);
                else Debug.WriteLine("配置继电器设备 2 成功! 端口号: " + ryDeviceM.ryDevicePortName);
            }
            catch (Exception ex)
            {
                Utils.Logger.Sys("从配置文件读取参数过程中发生异常：" + ex.Message.ToString());
                confOK = false;
            }

            return confOK;
        }


        public bool SelfCheck()
        {
            return true;
        }
    }
}
