using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstDevice
{
    public interface ICmdChain<TData>
        where TData: InstDataBase
    {
        /// <summary> 提取下一条指令 </summary>
        string FetchNextCmd();
        /// <summary> 重置指令 </summary>
        void ResetCmd();
        /// <summary>
        /// 解析传感器数据
        /// </summary>
        /// <param name="str"> 输入字符串 </param>
        /// <param name="data"> 解析得到的结果，为空时，表示还未形成结果 </param>
        /// <returns> 是否发生错误？ true: 未发生错误；false: 发生错误</returns>
        bool ResolveData(string str,out TData data);
    }

    /// <summary>
    /// 仪器命令
    /// </summary>
    public class CmdChainSTD : ICmdChain<InstSTDData>
    {
        /// <summary> 提取下一条指令 </summary>
        public string FetchNextCmd()
        {
            return string.Empty;
        }
        /// <summary> 重置指令 </summary>
        public void ResetCmd() { }
        
        /// <summary>
        /// 解析传感器数据
        /// </summary>
        /// <param name="str"> 输入字符串 </param>
        /// <param name="data"> 解析得到的结果，为空时，表示还未形成结果 </param>
        /// <returns> 是否发生错误？ true: 未发生错误；false: 发生错误</returns>
        bool ICmdChain<InstSTDData>.ResolveData(string str, out InstSTDData data)
        {
            data = new InstSTDData();

            try
            {
                string[] valStrs = str.Split('-');

                data.vTestID = "wghou";
                data.vTitularValue = 123123;
                data.vStandardC = double.Parse(valStrs[4]);
                data.vStandardT = double.Parse(valStrs[5]);
                data.measureTime = DateTime.ParseExact(valStrs[0], "yyyy_MM_dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                data.addTime = data.measureTime;
                data.updateTime = data.measureTime;
            }
            catch(Exception ex)
            {
                return false;
            }

            return true;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public abstract class CmdChainSBE37 : ICmdChain<InstSBE37Data>
    {
        /// <summary>
        /// 仪器一组数据
        /// </summary>
        protected InstSBE37Data instData = new InstSBE37Data();

        public abstract string FetchNextCmd();
        public abstract void ResetCmd();
        public abstract bool ResolveData(string str, out InstSBE37Data data);
    }

    /// <summary>
    /// 仪器命令
    /// </summary>
    public sealed class CmdChainSMP : CmdChainSBE37
    {
        private int currentCmdIdx = -1;
        private readonly int maxCmdIdx = 1;  // 0 / 1
        private string[] cmdString = new string[] { "ts", "tsr" };

        /// <summary> 提取下一条指令 </summary>
        public override string FetchNextCmd()
        {
            if (++currentCmdIdx > maxCmdIdx) currentCmdIdx = 0;

            return cmdString[currentCmdIdx];
        }
        /// <summary> 重置指令 </summary>
        public override void ResetCmd() { currentCmdIdx = -1; }
        /// <summary>
        /// 解析传感器数据
        /// </summary>
        /// <param name="str"> 输入字符串 </param>
        /// <param name="data"> 解析得到的结果，为空时，表示还未形成结果 </param>
        /// <returns> 是否发生错误？ true: 未发生错误；false: 发生错误</returns>
        public override bool ResolveData(string str, out InstSBE37Data data)
        {
            data = null;

            try
            {
                string[] valStrs = str.Split(',');

                switch(cmdString[currentCmdIdx])
                {
                    case "ts":
                        break;

                    case "tsr":
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// 仪器命令
    /// </summary>
    public sealed class CmdChainSM : CmdChainSBE37
    {
        private int currentCmdIdx = -1;
        private readonly int maxCmdIdx = 1;  // 0 / 1
        private string[] cmdString = new string[] { "ts", "tsr" };

        /// <summary> 提取下一条指令 </summary>
        public override string FetchNextCmd()
        {
            if (++currentCmdIdx > maxCmdIdx) currentCmdIdx = 0;

            return cmdString[currentCmdIdx];
        }
        /// <summary> 重置指令 </summary>
        public override void ResetCmd() { currentCmdIdx = -1; }

        /// <summary>
        /// 解析传感器数据
        /// </summary>
        /// <param name="str"> 输入字符串 </param>
        /// <param name="data"> 解析得到的结果，为空时，表示还未形成结果 </param>
        /// <returns> 是否发生错误？ true: 未发生错误；false: 发生错误</returns>
        public override bool ResolveData(string str, out InstSBE37Data data)
        {
            data = null;

            try
            {
                string[] valStrs = str.Split(',');

                switch (cmdString[currentCmdIdx])
                {
                    case "ts":
                        break;

                    case "tsr":
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
    }
}
