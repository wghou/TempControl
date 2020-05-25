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
    public sealed partial class InstSBE : InstDeviceStateM<InstSBE37Data, InstSqlrd>
    {
        public InstSBE()
        {
            // 配置仪器相关状态机
            ConfigInstStateless();

            // todo: 根据设备类型设置
            cmdChain = new CmdChainSMP();

            // todo: 
            _tickTimerInst = new System.Timers.Timer();
            _tickTimerInst.Interval = 1000;
            _tickTimerInst.AutoReset = true;
            _tickTimerInst.Elapsed += _tickTimerSample_Elapsed;
            _tickTimerInst.Start(); 
        }
    }
}
