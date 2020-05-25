using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InstDevice
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class InstSBE37 : InstDeviceStateM<InstInfo, InstSBE37Data>
    {
        public InstSBE37()
        {
            Info = new InstInfo();
            Info.InstIdx = InstCount++;
            Info.InstType = TypeInst.SBE37SI;

            // 配置仪器相关状态机
            ConfigInstStateless();

            // todo: 
            _tickTimerInst = new System.Timers.Timer();
            _tickTimerInst.Interval = 1000;
            _tickTimerInst.AutoReset = true;
            _tickTimerInst.Elapsed += _tickTimerSample_Elapsed;
            _tickTimerInst.Start();

            // 串口接收到数据 - 事件处理函数
            sPort.DataReceived += SPort_DataReceived;
        }
    }
}
