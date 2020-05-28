using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

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
        /// <param name="info"> 仪器的基本信息 </param>
        /// <param name="str"> 输入字符串 </param>
        /// <param name="data"> 解析得到的结果，为空时，表示还未形成结果 </param>
        /// <returns> 是否发生错误？ true: 未发生错误；false: 发生错误</returns>
        bool ResolveData(InstInfoBase info, string str,out TData data);
    }


    /// <summary>
    /// 
    /// </summary>
    public abstract class CmdChainSBE37 : ICmdChain<InstSBE37Data>
    {
        /// <summary>
        /// SBE37 采样命令
        /// </summary>
        protected enum SBE37SampleCmd : int
        {
            Format_0 = 0,
            Format_1 = 1,
            TS = 2,
            TSR = 3
        }
        /// <summary>
        /// SBE37 中的指令字符串，必须与枚举类型 SBE37SampleCmd 配合使用
        /// </summary>
        protected static readonly string[] cmdStrings = new string[] { "format=0", "format=1", "ts", "tsr" };

        /// <summary>
        /// 仪器一组数据
        /// </summary>
        protected InstSBE37Data instData = new InstSBE37Data();

        /// <summary>
        /// 当前指令位置，初始时为 -1
        /// </summary>
        protected int currentCmdIdx = -1;
        /// <summary>
        /// 最大指令位置，从 0 开始计算
        /// </summary>
        protected int maxCmdIdx = 3;  // 0 / 1 / 2 /3
        /// <summary>
        /// 当前的采样方式 - 只能是 Format_0 和 Format_1
        /// </summary>
        protected SBE37SampleCmd currentSampleFormat = SBE37SampleCmd.Format_1;
        /// <summary>
        /// 某类型的 SBE37 仪器需要用到的指令，以及对应的顺序
        /// 根据不同类型的仪器，指令集也不相同
        /// </summary>
        protected SBE37SampleCmd[] sampleCmd = new SBE37SampleCmd[]
                                                    { SBE37SampleCmd.Format_0, SBE37SampleCmd.TS,
                                                    SBE37SampleCmd.Format_1, SBE37SampleCmd.TS};

        /// <summary> 提取下一条指令字符串 </summary>
        public virtual string FetchNextCmd()
        {
            if (++currentCmdIdx > maxCmdIdx) {
                //  清空数据缓存
                instData = new InstSBE37Data();
                currentCmdIdx = 0;
            }

            return cmdStrings[(int)sampleCmd[currentCmdIdx]];
        }
        /// <summary> 重置指令 </summary>
        public virtual void ResetCmd() { currentCmdIdx = -1; }

        public abstract bool ResolveData(InstInfoBase info, string str, out InstSBE37Data data);
    }

    /// <summary>
    /// 仪器命令
    /// </summary>
    public sealed class CmdChainSMP : CmdChainSBE37
    {
        public CmdChainSMP()
        {
            maxCmdIdx = 3;  // 0 / 1 / 2 /3

            sampleCmd = new SBE37SampleCmd[] { SBE37SampleCmd.Format_0, SBE37SampleCmd.TS,
                                                    SBE37SampleCmd.Format_1, SBE37SampleCmd.TSR};
        }

        /// <summary>
        /// 解析传感器数据
        /// </summary>
        /// <param name="str"> 输入字符串 </param>
        /// <param name="data"> 解析得到的结果，为空时，表示还未形成结果 </param>
        /// <returns> 是否发生错误？ true: 未发生错误；false: 发生错误</returns>
        public override bool ResolveData(InstInfoBase info, string str, out InstSBE37Data data)
        {
            Debug.WriteLine("InstSMP receive data: " + str);

            // 设置基本信息
            data = null;
            instData.vTestID = info.testId;
            instData.vInstrumentID = info.instrumentId;
            instData.vItemType = "";

            try
            {
                switch (sampleCmd[currentCmdIdx])
                {
                    case SBE37SampleCmd.Format_0:
                        currentSampleFormat = SBE37SampleCmd.Format_0;
                        break;

                    case SBE37SampleCmd.Format_1:
                        currentSampleFormat = SBE37SampleCmd.Format_1;
                        break;
                    case SBE37SampleCmd.TS:

                        if(currentSampleFormat == SBE37SampleCmd.Format_0)
                        {
                            // 226113,··2777.363,·529615,·2748,··16·Nov·2018,·13:15:48
                            // (temperature, conductivity, pressure sensor pressure, pressure sensor temperature compensation, date, time)
                            // 参见 37SMP_RS232_023.pdf   p44 p48

                            // 解析数据
                            string[] valStrs1 = str.Split(',');

                            instData.vTemperature = double.Parse(valStrs1[1]);
                            //instData.vTitularValue = int.Parse(valStrs1[0]);
                            //instData.vRealValue = 0;
                            //instData.vRawValue = 0;
                            // instData.vData = "";
                            //instData.addTime = DateTime.ParseExact(valStrs1[4], "dd MM yyyy", System.Globalization.CultureInfo.CurrentCulture);
                            instData.updateTime = DateTime.Now;
                        }
                        else if(currentSampleFormat == SBE37SampleCmd.Format_1)
                        {
                            // ··23.2611,··0.00001,····0.065,···0.0114,·1491.953,··0.00001,·16·Nov·2018,·13:16:45,·3502
                            // (temperature, conductivity, pressure, salinity, sound velocity, specific conductance, date, time, sample number)
                            // 参见 37SMP_RS232_023.pdf   p44 p48

                            // 解析数据
                            string[] valStrs2 = str.Split(',');

                            // instData.vTemperature = 0;
                            // instData.vTitularValue = 0;
                            instData.vRealValue = double.Parse(valStrs2[0]);
                            instData.vRawValue = double.Parse(valStrs2[1]);
                            // instData.vData = "";
                            // instData.addTime = DateTime.Now;
                            // instData.updateTime = DateTime.Now;
                        }
                        else
                        {
                            // error
                            return false;
                        }

                        break;

                    case SBE37SampleCmd.TSR:
                        // 226113,··2777.363,·529615,·2748,··16·Nov·2018,·13:15:48
                        // (temperature, conductivity, pressure sensor pressure, pressure sensor temperature compensation, date, time)
                        // 参见 37SMP_RS232_023.pdf   p44 p48

                        // 解析数据
                        string[] valStrs3 = str.Split(',');

                        instData.vTemperature = double.Parse(valStrs3[1]);
                        //instData.vTitularValue = int.Parse(valStrs3[0]);
                        //instData.vRealValue = 0;
                        //instData.vRawValue = 0;
                        // instData.vData = "";
                        //instData.addTime = DateTime.ParseExact(valStrs3[4], "dd MM yyyy", System.Globalization.CultureInfo.CurrentCulture);
                        instData.updateTime = DateTime.Now;

                        break;

                    default:
                        break;
                }

                // 所有命令都发送完成了，就可以返回数据了
                if(currentCmdIdx == maxCmdIdx)
                {
                    data = instData;
                }
            }
            catch (Exception ex)
            {
                // error
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
        public CmdChainSM()
        {
            maxCmdIdx = 3;  // 0 / 1 / 2 /3

            sampleCmd = new SBE37SampleCmd[] { SBE37SampleCmd.Format_0, SBE37SampleCmd.TS,
                                                    SBE37SampleCmd.Format_1, SBE37SampleCmd.TS};
        }

        /// <summary>
        /// 解析传感器数据
        /// </summary>
        /// <param name="str"> 输入字符串 </param>
        /// <param name="data"> 解析得到的结果，为空时，表示还未形成结果 </param>
        /// <returns> 是否发生错误？ true: 未发生错误；false: 发生错误</returns>
        public override bool ResolveData(InstInfoBase info, string str, out InstSBE37Data data)
        {
            Debug.WriteLine("InstSM receive data: " + str);

            // 设置基本信息
            data = null;
            instData.vTestID = info.testId;
            instData.vInstrumentID = info.instrumentId;
            instData.vItemType = "";

            try
            {
                switch (sampleCmd[currentCmdIdx])
                {
                    case SBE37SampleCmd.Format_0:
                        currentSampleFormat = SBE37SampleCmd.Format_0;
                        break;

                    case SBE37SampleCmd.Format_1:
                        currentSampleFormat = SBE37SampleCmd.Format_1;
                        break;
                    case SBE37SampleCmd.TS:

                        if (currentSampleFormat == SBE37SampleCmd.Format_0)
                        {
                            // 240268,··2710.594,·524281,·2679,··18·Oct·2018,·15:30:41
                            // (temperature, conductivity, pressure sensor pressure counts, pressure sensor temperature compensation, date, time)
                            // 参见 37SM_rs232_041.pdf  p42 p45

                            // 解析数据
                            string[] valStrs1 = str.Split(',');

                            instData.vTemperature = double.Parse(valStrs1[1]);
                            //instData.vTitularValue = int.Parse(valStrs1[0]);
                            //instData.vRealValue = 0;
                            //instData.vRawValue = 0;
                            // instData.vData = "";
                            //instData.addTime = DateTime.ParseExact(valStrs1[4], "dd MM yyyy", System.Globalization.CultureInfo.CurrentCulture);
                            instData.updateTime = DateTime.Now;
                        }
                        else if (currentSampleFormat == SBE37SampleCmd.Format_1)
                        {
                            // 24.8339,··0.97795,···-9.262,·07·Jan·2000,·04:51:39
                            // (temperature, conductivity, pressure, date, time)
                            // 参见 37SM_rs232_041.pdf  p42 p46

                            // 解析数据
                            string[] valStrs2 = str.Split(',');

                            // instData.vTemperature = 0;
                            // instData.vTitularValue = 0;
                            instData.vRealValue = double.Parse(valStrs2[0]);
                            instData.vRawValue = double.Parse(valStrs2[1]);
                            // instData.vData = "";
                            // instData.addTime = DateTime.Now;
                            // instData.updateTime = DateTime.Now;
                        }
                        else
                        {
                            // error
                            return false;
                        }

                        break;

                    case SBE37SampleCmd.TSR:
                        // 226113,··2777.363,·529615,·2748,··16·Nov·2018,·13:15:48
                        // (temperature, conductivity, pressure sensor pressure, pressure sensor temperature compensation, date, time)
                        // 参见 37SM_rs232_041.pdf  p42 p45

                        // 解析数据
                        string[] valStrs3 = str.Split(',');

                        instData.vTemperature = double.Parse(valStrs3[1]);
                        //instData.vTitularValue = int.Parse(valStrs3[0]);
                        //instData.vRealValue = 0;
                        //instData.vRawValue = 0;
                        // instData.vData = "";
                        //instData.addTime = DateTime.ParseExact(valStrs3[4], "dd MM yyyy", System.Globalization.CultureInfo.CurrentCulture);
                        instData.updateTime = DateTime.Now;
                        break;

                    default:
                        break;
                }

                // 所有命令都发送完成了，就可以返回数据了
                if (currentCmdIdx == maxCmdIdx)
                {
                    data = instData;
                }
            }
            catch (Exception ex)
            {
                // error
                return false;
            }

            return true;
        }
    }
}
