using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stateless;
using System.Diagnostics;
using System.Timers;
using System.IO;
using NLog;

namespace Device
{
    /// <summary>
    /// 附带状态机的设备
    /// </summary>
    public partial class DeviceStateM
    {
        private static readonly Logger nlogger = LogManager.GetCurrentClassLogger();

        public DeviceStateM()
        {
            // 配置设备状态机
            ConfigStateless();

            // 配置设备错误状态监测器
            ResetErrorStatus();

            // 记录设备运行时间
            startTime = DateTime.Now; 
        }
    }
}
