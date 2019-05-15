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
            if (_runningParameters.ReadValueConfig(configFilePath) == false) Debug.WriteLine("读取配置文件错误，使用默认参数运行。");

            _tickTimer.Interval = _runningParameters.readTempIntervalSec * 500;

            try
            {
                //////////////////////////////////////////
                // 配置参数
                // 主槽控温设备
                confOK &= tpDeviceM.ConfigSyn(_runningParameters.portTp1);
                Debug.WriteLineIf(!confOK, "配置主槽控温设备失败! 端口号: " + tpDeviceM.tpDevicePortName);
                Debug.WriteLineIf(confOK, "配置主槽控温设备成功! 端口号: " + tpDeviceM.tpDevicePortName);
                if (!confOK)
                    Utils.Logger.Sys("配置主槽控温设备失败! 端口号: " + tpDeviceM.tpDevicePortName);

                // 辅槽控温设备
                confOK &= tpDeviceS.ConfigSyn(_runningParameters.portTp2);
                Debug.WriteLineIf(!confOK, "配置辅槽控温设备失败! 端口号: " + tpDeviceS.tpDevicePortName);
                Debug.WriteLineIf(confOK, "配置辅槽控温设备成功! 端口号: " + tpDeviceS.tpDevicePortName);
                if (!confOK)
                    Utils.Logger.Sys("配置辅槽控温设备失败! 端口号: " + tpDeviceS.tpDevicePortName);


                // 继电器设备 1
                confOK &= ryDeviceM.SetPortName(_runningParameters.portRy1);
                Debug.WriteLineIf(!confOK, "配置继电器设备 1 失败! 端口号: " + ryDeviceM.ryDevicePortName);
                Debug.WriteLineIf(confOK, "配置继电器设备 1 成功! 端口号: " + ryDeviceM.ryDevicePortName);
                if (!confOK)
                    Utils.Logger.Sys("配置继电器设备 1 失败! 端口号: " + ryDeviceM.ryDevicePortName);

                // 继电器设备 2
                confOK &= ryDeviceS.SetPortName(_runningParameters.portRy2);
                Debug.WriteLineIf(!confOK, "配置继电器设备 2 失败! 端口号: " + ryDeviceS.ryDevicePortName);
                Debug.WriteLineIf(confOK, "配置继电器设备 2 成功! 端口号: " + ryDeviceS.ryDevicePortName);
                if (!confOK)
                    Utils.Logger.Sys("配置继电器设备 2 失败! 端口号: " + ryDeviceS.ryDevicePortName);
            }
            catch (Exception ex)
            {
                Utils.Logger.Sys("从配置文件读取参数过程中发生异常：" + ex.Message.ToString());
                confOK = false;
            }

            Debug.WriteLineIf(confOK, "设备串口配置成功!");

            return confOK;
        }


        public bool SelfCheck()
        {
            return true;
        }
    }
}
