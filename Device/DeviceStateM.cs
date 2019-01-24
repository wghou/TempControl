using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stateless;
using System.Diagnostics;
using System.Timers;

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

            // 配置设备参数
            ConfigDeviceParameters();

            // 配置设备错误状态监测器
            ConfigErrorMonitor();

            // 开始时钟
            _tickTimer.Start();
        }
    }
}
