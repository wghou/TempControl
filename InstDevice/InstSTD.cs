using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Diagnostics;
using System.Threading;
using NLog;

namespace InstDevice
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class InstSTD : InstDeviceStateM<InstSTDData, InstInfoBase>
    {
        public InstSTD(InstInfoBase info) : base(info)
        {
            
        }

        /// <summary>
        /// 根据 this.Info.InstType ，生成 cmdChain
        /// </summary>
        /// <returns></returns>
        protected override bool SetCmdChain()
        {
            if(Info.InstType == TypeInst.Standard)
            {
                cmdChain = new CmdChainSTD();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 检测标准温度仪器的波动度
        /// </summary>
        /// <param name="cnt"></param>
        /// <param name="crt"></param>
        /// <returns></returns>
        public bool CheckFluc(int cnt, double crt)
        {
            if (!Enable) return false;

            double fluc = 0;
            if (_instData.Count == 0 || _instData.Count < cnt)
            {
                // If there is not temperature data in list, output extreme fluctuation
   
                return false;
            }
            else
            {
                fluc = _instData.GetRange(_instData.Count - cnt, cnt).Max().vStandardT -
                    _instData.GetRange(_instData.Count - cnt, cnt).Min().vStandardT;
                
                if(fluc < crt) { return true; }
                else { return false; }
            }
        }
    }
}
