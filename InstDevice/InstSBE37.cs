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

            // todo: 
            _tickTimerInst = new System.Timers.Timer();
            _tickTimerInst.Interval = 1000;
            _tickTimerInst.AutoReset = true;
            _tickTimerInst.Elapsed += _tickTimerSample_Elapsed;
            _tickTimerInst.Start(); 
        }

        /// <summary>
        /// 设置 cmdChain
        /// </summary>
        /// <returns></returns>
        protected override bool setCmdChain()
        {
            switch (Info.InstType)
            {
                case TypeInst.SBE37SI:
                    cmdChain = new CmdChainSMP();
                    break;

                default:
                    break;
            }
            return true;
        }
    }
}
