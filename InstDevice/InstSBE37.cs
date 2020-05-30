using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace InstDevice
{
    /// <summary>
    /// SBE 37 系列仪器
    /// </summary>
    public sealed partial class InstSBE : InstDeviceStateM<InstSBE37Data, InstSqlrd>
    {
        /// <summary>
        /// 仪器的采样方式 / 数据读取方式
        /// </summary>
        private InstSampleMode sampleMode = InstSampleMode.AutoSample_Fmt1;
        /// <summary>
        /// 当前发送给仪器的指令
        /// </summary>
        private SBE37Cmd currentCmd = SBE37Cmd.NoneCmd;
        /// <summary>
        /// 当前发送的指令，是否已返回结果
        /// </summary>
        private bool CmdExecuted = false;
        /// <summary>
        /// 是否使用 OutputExecutedFlag
        /// 暂时不考虑用这个
        /// </summary>
        private readonly bool UseExecutedFlag = false;
        /// <summary>
        /// 指令执行完成返回标志位
        /// </summary>
        private string OutputFlag = "<Executed/>";
        /// <summary>
        /// 仪器读取数据的返回格式（默认为 format_1）
        /// </summary>
        private SBE37OutputFormat outputFormat = SBE37OutputFormat.Format_1;
        /// <summary>
        /// Configuration Data in Xml format
        /// </summary>
        private string ConfigDataXml = "";
        /// <summary>
        /// Configuration Data
        /// </summary>
        private SBE37ConfigData configData = new SBE37ConfigData();
        /// <summary>
        /// calibration coefficients in Xml format
        /// </summary>
        private string CalibCoeffXml = "";
        /// <summary>
        /// calibration coefficients
        /// </summary>
        private SBE37CalibCoeff calibCCoeff = new SBE37CalibCoeff();


        public InstSBE(InstSqlrd info) : base(info)
        {
            // 配置仪器相关状态机
            ConfigInstStateless();

            // todo: 
            _tickTimerInst = new System.Timers.Timer();
            _tickTimerInst.Interval = info.sampleIntervalSec * 1000;
            _tickTimerInst.AutoReset = true;
            _tickTimerInst.Elapsed += _tickTimerSample_Elapsed;
            //_tickTimerInst.Start(); 
        }


        /// <summary>
        /// 配置当前的仪器
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public bool SetupSBE37(InstSampleMode mode = InstSampleMode.PolledSample_Fmt10)
        {
            Enable = true;

            _tickTimerInst.Interval = Info.sampleIntervalSec * 1000;

            // 读取 calibration coefficients
            //currentCmd = SBE37Cmd.GetCC;
            //CmdExecuted = false;
            //sendCMD("GetCC");
            //while (!CmdExecuted) { System.Threading.Thread.Sleep(10); }
            //bool rlt1 = calibCCoeff.ResolveXml2Value(CalibCoeffXml);
            //if (rlt1 == false)
            //{
            //    nlogger.Error("error when read and resolve configuration data from xml string");
            //    return false;
            //}

            // 设置返回标志位
            //CmdExecuted = false;
            //bool rlt2 = sendCMD("OutputExecutedTag=N");
            //System.Threading.Thread.Sleep(10);
            //if(rlt2 == false)
            //{
            //    nlogger.Error("Error in setup instDevice 1.");
            //    return false;
            //}

            // 写入一些设置信息
            bool rlt3 = true;
            switch (mode)
            {
                // 暂时还不用这种方法
                case InstSampleMode.AutoSample_Fmt0:
                    rlt3 &= sendCMD("SAMPLEMODE=2");
                    System.Threading.Thread.Sleep(10);
                    rlt3 &= sendCMD("SAMPLEINTERVAL=4");
                    System.Threading.Thread.Sleep(10);
                    rlt3 &= sendCMD("OUTPUTFORMAT=0");
                    System.Threading.Thread.Sleep(10);
                    rlt3 &= sendCMD("AUTORUN=N");
                    System.Threading.Thread.Sleep(10);
                    outputFormat = SBE37OutputFormat.Format_0;
                    break;

                // 暂时还不用这种方法
                case InstSampleMode.AutoSample_Fmt1:
                    rlt3 &= sendCMD("SAMPLEMODE=2");
                    System.Threading.Thread.Sleep(10);
                    rlt3 &= sendCMD("SAMPLEINTERVAL=4");
                    System.Threading.Thread.Sleep(10);
                    rlt3 &= sendCMD("OUTPUTFORMAT=1");
                    System.Threading.Thread.Sleep(10);
                    rlt3 &= sendCMD("AUTORUN=N");
                    System.Threading.Thread.Sleep(10);
                    outputFormat = SBE37OutputFormat.Format_1;
                    break;

                // 暂时还不用这种方法
                case InstSampleMode.PolledSample_Fmt0:
                    rlt3 &= sendCMD("SAMPLEMODE=1");
                    System.Threading.Thread.Sleep(10);
                    rlt3 &= sendCMD("AUTORUN=N");
                    System.Threading.Thread.Sleep(10);
                    rlt3 &= sendCMD("OUTPUTFORMAT=0");
                    System.Threading.Thread.Sleep(10);
                    outputFormat = SBE37OutputFormat.Format_0;
                    break;

                // 暂时还不用这种方法
                case InstSampleMode.PolledSample_Fmt1:
                    rlt3 &= sendCMD("SAMPLEMODE=1");
                    System.Threading.Thread.Sleep(10);
                    rlt3 &= sendCMD("AUTORUN=N");
                    System.Threading.Thread.Sleep(10);
                    rlt3 &= sendCMD("OUTPUTFORMAT=1");
                    System.Threading.Thread.Sleep(10);
                    outputFormat = SBE37OutputFormat.Format_1;
                    break;


                // 
                case InstSampleMode.PolledSample_Fmt10:
                    rlt3 &= sendCMD("SAMPLEMODE=1");
                    System.Threading.Thread.Sleep(10);
                    rlt3 &= sendCMD("AUTORUN=N");
                    System.Threading.Thread.Sleep(10);
                    rlt3 &= sendCMD("OUTPUTFORMAT=1");
                    System.Threading.Thread.Sleep(10);
                    outputFormat = SBE37OutputFormat.Format_1;
                    break;

                default:
                    break;
            }

            if(rlt3 == false)
            {
                nlogger.Error("Error in setup instDevice 2.");
                return false;
            }

            sampleMode = mode;

            // todo: 这里的 while 感觉不是很好
            // 读取 configuration data
            currentCmd = SBE37Cmd.GetCD;
            CmdExecuted = false;
            bool rlt4 = sendCMD("GetCD");
            int i = 0;
            while (!CmdExecuted && i<20) { System.Threading.Thread.Sleep(10); i++; }
            if (i > 19)
            {
                nlogger.Error("timeout when read configuration data from xml string");
                return false;
            }
            rlt4 &= configData.ResolveXml2Value(ConfigDataXml);
            if (rlt4 == false)
            {
                nlogger.Error("error when read and resolve configuration data from xml string");
                return false;
            }

            //bool rlt5 = true;
            //UseExecutedFlag = useOptFlg;
            //if (UseExecutedFlag == true ) {
            //    CmdExecuted = false;
            //    rlt5 = sendCMD("OutputExecutedTag=Y");
            //    if(rlt5 == false)
            //    {
            //        return false;
            //    }

            //    while (!CmdExecuted) { System.Threading.Thread.Sleep(10); }
            //}

            return true;
        }

        /// <summary>
        /// 关闭/停止仪器工作
        /// </summary>
        /// <returns></returns>
        public override bool DisableInstDevice()
        {
            sendCMD("QS");

            base.DisableInstDevice();
            return true;
        }

        /// <summary>
        /// 进入 Measure 步骤
        /// </summary>
        protected override void internalEnterMeasureStep()
        {
            base.internalEnterMeasureStep();

            // 确保打开了串口，确保清空串口中的数据
            try { sPort.Open(); sPort.DiscardInBuffer(); } catch { }

            if(sampleMode == InstSampleMode.AutoSample_Fmt0 || sampleMode == InstSampleMode.AutoSample_Fmt1)
            {
                currentCmd = SBE37Cmd.Start;
                CmdExecuted = false;
                bool rlt = sendCMD("Start");
                if(rlt == false)
                {
                    nlogger.Error("error in sendCmd with internalEnterMeasureStep");
                    OnErrorOccur(Err_sr.Error);
                }
            }
            else
            {
                currentCmd = SBE37Cmd.Ts;
                CmdExecuted = false;
                _tickTimerInst.Start();
            }
        }
        /// <summary>
        /// 执行 Measure 步骤
        /// </summary>
        protected override void internalMeasureStep()
        {
            if (sampleMode == InstSampleMode.AutoSample_Fmt0 || sampleMode == InstSampleMode.AutoSample_Fmt1)
            {

            }
            else
            {
                if(UseExecutedFlag == true && CmdExecuted == false)
                {
                    nlogger.Error("the CmdExecuted flag is not been flap to true before send cmd again.");
                    OnErrorOccur(Err_sr.Error);
                }
            }


            if (sampleMode == InstSampleMode.AutoSample_Fmt0 || sampleMode == InstSampleMode.AutoSample_Fmt1)
            {
                
            }
            else if(sampleMode == InstSampleMode.PolledSample_Fmt0)
            {
                currentCmd = SBE37Cmd.Tsr;
                CmdExecuted = false;
                bool rlt = sendCMD("tsr");
                if (rlt == false)
                {
                    nlogger.Error("error in sendCmd with internalMeasureStep");
                    OnErrorOccur(Err_sr.Error);
                }
            }
            else if(sampleMode == InstSampleMode.PolledSample_Fmt1)
            {
                currentCmd = SBE37Cmd.Ts;
                CmdExecuted = false;
                bool rlt = sendCMD("ts");
                if (rlt == false)
                {
                    nlogger.Error("error in sendCmd with internalMeasureStep");
                    OnErrorOccur(Err_sr.Error);
                }
            }
            else if(sampleMode == InstSampleMode.PolledSample_Fmt10)
            {
                if(currentCmd == SBE37Cmd.Ts)
                {
                    currentCmd = SBE37Cmd.Tsr;
                    CmdExecuted = false;
                    bool rlt = sendCMD("tsr");
                    if (rlt == false)
                    {
                        nlogger.Error("error in sendCmd with internalMeasureStep");
                        OnErrorOccur(Err_sr.Error);
                    }
                }
                else if(currentCmd == SBE37Cmd.Tsr)
                {
                    currentCmd = SBE37Cmd.Ts;
                    CmdExecuted = false;
                    bool rlt = sendCMD("ts");
                    if (rlt == false)
                    {
                        nlogger.Error("error in sendCmd with internalMeasureStep");
                        OnErrorOccur(Err_sr.Error);
                    }
                }
                else{
                    OnErrorOccur(Err_sr.Error);
                }     
            }
        }
        /// <summary>
        /// 进入 Store 步骤
        /// </summary>
        protected override void internalEnterStoreStep()
        {
            if (sampleMode == InstSampleMode.AutoSample_Fmt0 || sampleMode == InstSampleMode.AutoSample_Fmt1)
            {
                currentCmd = SBE37Cmd.Stop;
                CmdExecuted = false;
                bool rlt = sendCMD("stop");
                if (rlt == false)
                {
                    nlogger.Error("error in sendCmd with internalEnterStoreStep");
                    OnErrorOccur(Err_sr.Error);
                }
            }
            else
            {
                _tickTimerInst.Stop();
            }
        }

        /// <summary>
        /// 由收到的字符串解析为指令。
        /// 其实就是检查 <Executed/>
        /// </summary>
        /// <param name="str"> 串口接收到的字符串 </param>
        /// <returns> 是否成功解析为指令 </returns>
        protected override bool ResolveStr2Cmd(string str)
        {
            // 是否要判断行数呢？
            //if(str.Split('\r').Length != 1)
            //{
            //    OnErrorOccur(Err_sr.Error);
            //    return true;
            //}

            // 如果当前字符串是返回标志位
            if (str.Contains(OutputFlag))
            {
                if(CmdExecuted == false)
                {
                    CmdExecuted = true;
                }
                else if(UseExecutedFlag == true)
                {
                    nlogger.Error("received the executed flag agin.");
                    // 重复接收到 ExecutedFlag
                    OnErrorOccur(Err_sr.Error);
                }

                return true;
            }

            return false;
        }
        /// <summary>
        /// 由收到的字符串解析为数据。
        /// 根据不同的传感器/配置，进行数据的解析
        /// </summary>
        /// <param name="str"> 串口接收到的字符串 </param>
        /// <param name="dt"> 解析得到的数据，为空表示 暂时未解析成一组数据 </param>
        /// <returns> 是否成功解析为数据 </returns>
        protected override bool ResolveStr2Data(string str, out InstSBE37Data data)
        {
            // 程序执行到这里，说明已经通过 ResolveStr2Cmd(string str) 判断得到，
            // str 中不包含有 <Executed/>

            data = null;
            bool rlt = true;

            switch (currentCmd)
            {
                case SBE37Cmd.GetCD:
                    rlt = GetXmlData(str, out ConfigDataXml);
                    break;

                case SBE37Cmd.GetCC:
                    rlt = GetXmlData(str, out CalibCoeffXml);
                    break;

                case SBE37Cmd.Start:
                case SBE37Cmd.Stop:
                    // 是否会发生错误呢？
                    rlt = SBE37ResolveStr2Value(str, out data);
                    break;

                case SBE37Cmd.Ts:
                    rlt = SBE37ResolveStr2Value(str, out data);
                    // 如果是 轮询 10 的方式查询数据
                    // todo: 不知道这儿这么写合理与否
                    if (sampleMode == InstSampleMode.PolledSample_Fmt10)
                    {
                        data = null;
                    }
                    break;

                case SBE37Cmd.Tsr:
                    rlt = SBE37ResolveStr2Value(str, out data);
                    break;

                default:
                    // 这里就产生了错误
                    // 因为现有的指令中，不会有其他的指令会返回数据
                    nlogger.Error("unknown cmd in InstBE37. ResolveStr2Data");
                    rlt = false;
                    data = null;
                    break;
            }

            // 在不是 tc 和 tcr 的情况下，数据后面都会跟随有 <Executed/>
            if (UseExecutedFlag == true && (currentCmd == SBE37Cmd.Ts && currentCmd == SBE37Cmd.Tsr))
            {
                // 读取数据末尾跟随的结束标志位
                string str2 = "";
                try { str2 = sPort.ReadLine(); } catch { }
                    
                if (!str2.Contains(OutputFlag))
                {
                    nlogger.Error("did not receive executed flag at the end of ts / tsr.");
                    return false;
                }
                CmdExecuted = true;
            }
            else
            {
                // 直接舍弃其他的接收数据
                // 理论上，应该也啥也没有了
                try { sPort.DiscardInBuffer(); } catch { }
            }

            return rlt;
        }


        /// <summary>
        /// 重新读取 information data in xml format, 
        /// such as configuration data & calibration coefficients
        /// </summary>
        /// <param name="str"> 第一行字符串 </param>
        /// <param name="xmlStr"> xml string 应放置的对象 </param>
        /// <returns> 是否成功获取 </returns>
        private bool GetXmlData(string str, out string xmlStr)
        {
            // 清空原有的 xml data
            xmlStr = "";

            // 读取 configuration data until get the <Executed/>
            try
            {
                do
                {
                    xmlStr += str;
                    //xmlStr += str + "\r";
                    str = sPort.ReadLine();
                } while (!str.Contains(OutputFlag));
            }
            catch (Exception ex)
            {
                nlogger.Error("error in get xml data");
                CmdExecuted = true;
                return false;
            }
            CmdExecuted = true;
            return true;
        }

        /// <summary>
        /// SBE37 解析数据
        /// </summary>
        /// <param name="str"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool SBE37ResolveStr2Value(string str, out InstSBE37Data data)
        {
            data = null;
            bool rlt = true;

            try
            {
                instData1Cache.vTestID = Info.testId;
                instData1Cache.vInstrumentID = Info.instrumentId;
                instData1Cache.vItemType = "";
                instData1Cache.vTitularValue = currentTemptPoint;

                if(currentCmd == SBE37Cmd.Tsr)
                {
                    rlt = SBE37ResolveStr2ValueFmt0(str, out data);
                }
                else
                {
                    // 解析数据
                    switch (outputFormat)
                    {
                        case SBE37OutputFormat.Format_0:
                            rlt = SBE37ResolveStr2ValueFmt0(str, out data);
                            break;

                        case SBE37OutputFormat.Format_1:
                            rlt = SBE37ResolveStr2ValueFmt1(str, out data);
                            break;

                        case SBE37OutputFormat.Format_2:
                            data = null;
                            rlt = false;
                            nlogger.Error("error of output format: Format_2");
                            break;

                        case SBE37OutputFormat.Format_3:
                            data = null;
                            rlt = false;
                            nlogger.Error("error of output format: Format_3");
                            break;

                        default:
                            data = null;
                            rlt = false;
                            nlogger.Error("error of output format: unknown");
                            break;
                    }
                }  
            }
            catch (Exception ex)
            {
                nlogger.Error("exception in SBE37ResolveStr2Value(..)");
                return false;
            }

            return rlt;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool SBE37ResolveStr2ValueFmt0(string str, out InstSBE37Data data)
        {
            data = null;
            string[] strSplit = str.Split(',');

            nlogger.Info("SBE37ResolveStr2ValueFmt0: " + str);

            instData1Cache.addTime = DateTime.Now;

            int idx = 0;
            try
            {
                int ttt = 0; double ccc = 0;
                int ppp, vvv;
                double oo, ot;
                DateTime dt = DateTime.Now;

                // tttttt =temperature A/D counts
                if (configData.OutputTemperature.ToUpper().Contains("YES"))
                {
                    ttt = int.Parse(strSplit[idx++]);
                }

                // cccc.ccc = conductivity frequency (Hz)
                if (configData.OutputConductivity.ToUpper().Contains("YES"))
                {
                    ccc = double.Parse(strSplit[idx++]);
                }

                instData1Cache.vTemperatureRaw = ttt;
                instData1Cache.vConductivityRaw = ccc;

                //// pppppp = pressure sensor pressure A/D counts;
                //// vvvv = pressure sensor pressure temperature compensation A/D counts;
                //// sent only if pressure sensor installed
                //if (configData.PressureInstalled.ToUpper().Contains("YES") && configData.OutputPressure.ToUpper().Contains("YES"))
                //{
                //    ppp = int.Parse(strSplit[idx++]);
                //    vvv = int.Parse(strSplit[idx++]);
                //}

                //// oo.ooo = oxygen sensor phase (µsec)
                //// t.tttttt = oxygen sensor temperature voltage.
                //if (configData.OutputOxygen.ToUpper().Contains("YES"))
                //{
                //    oo = double.Parse(strSplit[idx++]);
                //    ot = double.Parse(strSplit[idx++]);
                //}

                //// dd mmm yyyy = day, month, year; sent only if OutputTime=Y
                //// hh:mm:ss = hour, minute, second; sent only if OutputTime=Y.
                //if (configData.OutputTime.ToUpper().Contains("YES"))
                //{
                //    dt = DateTime.Parse(strSplit[idx++]);
                //    dt = DateTime.Parse(strSplit[idx++]);
                //}

                data = instData1Cache;
            }
            catch(Exception ex)
            {
                nlogger.Error("exception in SBE37ResolveStr2ValueFmt0: " + ex.Message);
                nlogger.Error("error string: " + str);
                return false;
            }

            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool SBE37ResolveStr2ValueFmt1(string str, out InstSBE37Data data)
        {
            data = null;
            string[] strSplit = str.Split(',');

            nlogger.Info("SBE37ResolveStr2ValueFmt1: " + str);
            instData1Cache.addTime = DateTime.Now;

            int idx = 0;
            try
            {
                double ttt = 0; double ccc = 0;
                double ppp, ddd;
                double sss = 0,vvv;
                double rrr, sc;
                double xxx;
                double oo;
                int nn;
                DateTime dt = DateTime.Now;

                // tttt.tttt = temperature (°C, ITS-90).
                if (configData.OutputTemperature.ToUpper().Contains("YES"))
                {
                    ttt = double.Parse(strSplit[idx++]);
                }

                // ccc.ccccc = conductivity (S/m).
                if (configData.OutputConductivity.ToUpper().Contains("YES"))
                {
                    ccc = double.Parse(strSplit[idx++]);
                }

                // ppppp.ppp = pressure (decibars)
                // sent only if pressure sensor installed
                if (configData.PressureInstalled.ToUpper().Contains("YES") && configData.OutputPressure.ToUpper().Contains("YES"))
                {
                    ppp = double.Parse(strSplit[idx++]);
                }

                // dddd.ddd = depth (meters); 
                // sent only if OutputDepth=Y
                if (configData.OutputDepth.ToUpper().Contains("YES"))
                {
                    ddd = double.Parse(strSplit[idx++]);
                }

                // oo.ooo = oxygen sensor phase (µsec)
                // t.tttttt = oxygen sensor temperature voltage.
                if (configData.OutputOxygen.ToUpper().Contains("YES"))
                {
                    oo = double.Parse(strSplit[idx++]);
                }

                // ssss.ssss= salinity (psu); 
                // sent only if OutputSal=Y
                if (configData.OutputSalinity.ToUpper().Contains("YES"))
                {
                    sss = double.Parse(strSplit[idx++]);
                }

                instData1Cache.vTemperature = ttt;
                instData1Cache.vConductivity = ccc;
                instData1Cache.vSalinity = sss;
                instData1Cache.measureTime = DateTime.Now;
                instData1Cache.addTime = instData1Cache.measureTime;
                instData1Cache.updateTime = instData1Cache.measureTime;

                //// vvvv.vvv = sound velocity (meters/second); 
                //// sent only if OutputSV=Y
                //if (configData.OutputSV.ToUpper().Contains("YES"))
                //{
                //    vvv = double.Parse(strSplit[idx++]);
                //}

                //// rrr.rrrr = local density (kg/m3); 
                //// sent only if OutputDensity=Y
                //if (configData.OutputDensity.ToUpper().Contains("YES"))
                //{
                //    rrr = double.Parse(strSplit[idx++]);
                //}

                //// x = specific conductivity; 
                ////sent if OutputSC=Y
                //if (configData.OutputSC.ToUpper().Contains("YES"))
                //{
                //    xxx = double.Parse(strSplit[idx++]);
                //}

                //// dd mmm yyyy = day, month, year; sent only if OutputTime=Y
                //// hh:mm:ss = hour, minute, second; sent only if OutputTime=Y.
                //if (configData.OutputTime.ToUpper().Contains("YES"))
                //{
                //    dt = DateTime.Parse(strSplit[idx++]);
                //    dt = DateTime.Parse(strSplit[idx++]);
                //}

                //// n = sample number in FLASH memory 
                //// sent if TxSampleNum=y
                //if (configData.TxSampleNumber.ToUpper().Contains("YES"))
                //{
                //    nn = int.Parse(strSplit[idx++]);
                //}

                data = instData1Cache;
            }
            catch (Exception ex)
            {
                nlogger.Error("exception in SBE37ResolveStr2ValueFmt1: " + ex.Message);
                nlogger.Error("error string: " + str);
                return false;
            }

            return true;
        }
    }
}
