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

            try
            {
                // 如果配置文件不存在，则新建
                if (!File.Exists(configFilePath))
                {
                    // 设备端口号
                    Utils.IniReadWrite.INIWriteValue(configFilePath, "PortName", "tempMain", "COM1");
                    Utils.IniReadWrite.INIWriteValue(configFilePath, "PortName", "relay", "COM2");
                    Utils.IniReadWrite.INIWriteValue(configFilePath, "PortName", "sensor", "COM3");
                }

                //////////////////////////////////////////
                // 配置参数
                // 主槽控温设备
                confOK &= tpDeviceM.SetDevicePortName(Utils.IniReadWrite.INIGetStringValue(configFilePath, "PortName", "tempMain", "COM1"));
                // 温度读取时间间隔
                tpDeviceM.readTempIntervalSec = int.Parse(Utils.IniReadWrite.INIGetStringValue(configFilePath, "Paramters", "ReadIntervalSec", "2"));
                Debug.WriteLineIf(!confOK, "配置主槽控温设备失败! 端口号: " + tpDeviceM.tpDevicePortName);
                Debug.WriteLineIf(confOK, "配置主槽控温设备成功! 端口号: " + tpDeviceM.tpDevicePortName);
                if (!confOK)
                    Utils.Logger.Sys("配置主槽控温设备失败! 端口号: " + tpDeviceM.tpDevicePortName);


                // 继电器设备
                confOK &= ryDevice.SetPortName(Utils.IniReadWrite.INIGetStringValue(configFilePath, "PortName", "relay", "COM3"));
                Debug.WriteLineIf(!confOK, "配置继电器设备失败! 端口号: " + ryDevice.ryDevicePortName);
                Debug.WriteLineIf(confOK, "配置继电器设备成功! 端口号: " + ryDevice.ryDevicePortName);
                if (!confOK)
                    Utils.Logger.Sys("配置继电器设备失败! 端口号: " + ryDevice.ryDevicePortName);
            }
            catch (Exception ex)
            {
                Utils.Logger.Sys("从配置文件读取参数过程中发生异常：" + ex.Message.ToString());
                confOK = false;
            }

            Debug.WriteLineIf(confOK, "设备串口配置成功!");


            // 从配置文件中读取参数
            if (confOK == true)
            {
                confOK = _thresholdParameters.ReadValueConfig(configFilePath);
            }

            return confOK;
        }


        public bool SelfCheck()
        {
            List<Tuple<RelayDevice.Cmd_r, bool>> slfChkList
                = new List<Tuple<RelayDevice.Cmd_r, bool>>(){
                    new Tuple<RelayDevice.Cmd_r, bool>(RelayDevice.Cmd_r.OUT_0, true),
                    new Tuple<RelayDevice.Cmd_r, bool>(RelayDevice.Cmd_r.OUT_1, true),
                    new Tuple<RelayDevice.Cmd_r, bool>(RelayDevice.Cmd_r.OUT_2, true),
                    new Tuple<RelayDevice.Cmd_r, bool>(RelayDevice.Cmd_r.OUT_3, true),
                    new Tuple<RelayDevice.Cmd_r, bool>(RelayDevice.Cmd_r.OUT_4, true),
                    new Tuple<RelayDevice.Cmd_r, bool>(RelayDevice.Cmd_r.OUT_5, true),
                    new Tuple<RelayDevice.Cmd_r, bool>(RelayDevice.Cmd_r.OUT_6, true),
                    new Tuple<RelayDevice.Cmd_r, bool>(RelayDevice.Cmd_r.OUT_7, true),
                    new Tuple<RelayDevice.Cmd_r, bool>(RelayDevice.Cmd_r.OUT_7, false),
                    new Tuple<RelayDevice.Cmd_r, bool>(RelayDevice.Cmd_r.OUT_6, false),
                    new Tuple<RelayDevice.Cmd_r, bool>(RelayDevice.Cmd_r.OUT_5, false),
                    new Tuple<RelayDevice.Cmd_r, bool>(RelayDevice.Cmd_r.OUT_4, false),
                    new Tuple<RelayDevice.Cmd_r, bool>(RelayDevice.Cmd_r.OUT_3, false),
                    new Tuple<RelayDevice.Cmd_r, bool>(RelayDevice.Cmd_r.OUT_2, false),
                    new Tuple<RelayDevice.Cmd_r, bool>(RelayDevice.Cmd_r.OUT_1, false)
                };
            ryDevice.SelfCheck(slfChkList);
            return true;
        }
    }
}
