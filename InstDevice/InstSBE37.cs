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
        public InstSBE(InstSqlrd info) : base(info)
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
        /// 根据 this.Info.InstType ，生成 cmdChain
        /// </summary>
        /// <returns></returns>
        protected override bool SetCmdChain()
        {
            bool confOK = true;
            // todo: 完善 cmdChain
            switch (Info.InstType)
            {
                case TypeInst.SBE37SI:
                    confOK = false;
                    break;

                case TypeInst.SBE37SIP:
                    confOK = false;
                    break;

                case TypeInst.SBE37SM:
                    cmdChain = new CmdChainSM();
                    break;

                case TypeInst.SBE37SMP:
                    cmdChain = new CmdChainSMP();
                    break;

                case TypeInst.SBE37SMPODO:
                    confOK = false;
                    break;

                default:
                    nlogger.Error("Info.InstType is Undefined when new cmdChain.");
                    confOK = false;
                    break;
            }

            return confOK;
        }
    }
}
