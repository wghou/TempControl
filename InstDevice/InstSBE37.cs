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
    /// SBE 37 系列仪器
    /// </summary>
    public sealed partial class InstSBE : InstDeviceStateM<InstSBE37Data, InstSqlrd>
    {
        /// <summary>
        /// 仪器的采样方式 / 数据读取方式
        /// </summary>
        private InstSampleMode sampleModel = InstSampleMode.AutoSample_Fmt1;
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
        /// </summary>
        private bool UseExecutedFlag = false;
        /// <summary>
        /// 指令执行完成返回标志位
        /// </summary>
        private string OutputFlag = "<Executed/>";
        /// <summary>
        /// 仪器读取数据的返回格式（默认为 format_1）
        /// </summary>
        private SBE37OutputFormat outputFormat = SBE37OutputFormat.Format_1;
        /// <summary>
        /// calibration coefficients 通过指令 DC 获得
        /// </summary>
        private string CalibCoeff = "";


        public InstSBE(InstSqlrd info) : base(info)
        {
            // 配置仪器相关状态机
            ConfigInstStateless();

            // todo: 
            _tickTimerInst = new System.Timers.Timer();
            _tickTimerInst.Interval = 2000;
            _tickTimerInst.AutoReset = true;
            _tickTimerInst.Elapsed += _tickTimerSample_Elapsed;
            //_tickTimerInst.Start(); 
        }


        /// <summary>
        /// 配置当前的仪器
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="useOptFlg"></param>
        /// <returns></returns>
        public bool SetupSBE37(InstSampleMode mode, bool useOptFlg)
        {
            currentCmd = SBE37Cmd.InitCmds;
            UseExecutedFlag = true;

            // todo: 这里的 while 感觉不是很好

            // 设置返回标志位
            CmdExecuted = false;
            sendCMD("OutputExecutedTag=Y");
            while (!CmdExecuted) { System.Threading.Thread.Sleep(10); }

            // 读取硬件信息
            CmdExecuted = false;
            sendCMD("DC");
            while (!CmdExecuted) { System.Threading.Thread.Sleep(10); }

            // 设置返回标志位
            CmdExecuted = true;
            sendCMD("OutputExecutedTag=N");
            System.Threading.Thread.Sleep(10);

            // 写入一些设置信息

            switch (mode)
            {
                case InstSampleMode.AutoSample_Fmt0:
                    sendCMD("SAMPLEMODE=2");
                    System.Threading.Thread.Sleep(10);
                    sendCMD("SAMPLEINTERVAL=20");
                    System.Threading.Thread.Sleep(10);
                    sendCMD("OUTPUTFORMAT=0");
                    System.Threading.Thread.Sleep(10);
                    sendCMD("OUTPUTTIME=Y");
                    System.Threading.Thread.Sleep(10);
                    sendCMD("AUTORUN=N");
                    System.Threading.Thread.Sleep(10);

                    outputFormat = SBE37OutputFormat.Format_0;
                    break;

                case InstSampleMode.AutoSample_Fmt1:
                    sendCMD("SAMPLEMODE=2");
                    System.Threading.Thread.Sleep(10);
                    sendCMD("SAMPLEINTERVAL=20");
                    System.Threading.Thread.Sleep(10);
                    sendCMD("OUTPUTFORMAT=1");
                    System.Threading.Thread.Sleep(10);
                    sendCMD("OUTPUTTIME=Y");
                    System.Threading.Thread.Sleep(10);
                    sendCMD("AUTORUN=N");
                    System.Threading.Thread.Sleep(10);

                    outputFormat = SBE37OutputFormat.Format_1;
                    break;

                case InstSampleMode.PolledSample_Fmt0:
                    sendCMD("SAMPLEMODE=1");
                    System.Threading.Thread.Sleep(10);
                    sendCMD("SAMPLEINTERVAL=20");
                    System.Threading.Thread.Sleep(10);
                    sendCMD("OUTPUTFORMAT=1");
                    System.Threading.Thread.Sleep(10);
                    sendCMD("OUTPUTTIME=Y");
                    System.Threading.Thread.Sleep(10);
                    sendCMD("AUTORUN=N");
                    System.Threading.Thread.Sleep(10);

                    outputFormat = SBE37OutputFormat.Format_0;
                    break;

                case InstSampleMode.PolledSample_Fmt1:
                    sendCMD("SAMPLEMODE=1");
                    System.Threading.Thread.Sleep(10);
                    sendCMD("SAMPLEINTERVAL=20");
                    System.Threading.Thread.Sleep(10);
                    sendCMD("OUTPUTFORMAT=1");
                    System.Threading.Thread.Sleep(10);
                    sendCMD("OUTPUTTIME=Y");
                    System.Threading.Thread.Sleep(10);
                    sendCMD("AUTORUN=N");
                    System.Threading.Thread.Sleep(10);

                    outputFormat = SBE37OutputFormat.Format_1;
                    break;

                case InstSampleMode.PolledSample_Fmt10:
                    sendCMD("SAMPLEMODE=1");
                    System.Threading.Thread.Sleep(10);
                    sendCMD("SAMPLEINTERVAL=20");
                    System.Threading.Thread.Sleep(10);
                    sendCMD("OUTPUTFORMAT=1");
                    System.Threading.Thread.Sleep(10);
                    sendCMD("OUTPUTTIME=Y");
                    System.Threading.Thread.Sleep(10);
                    sendCMD("AUTORUN=N");
                    System.Threading.Thread.Sleep(10);

                    outputFormat = SBE37OutputFormat.Format_1;
                    break;

                default:
                    break;
            }

            UseExecutedFlag = useOptFlg;
            if (UseExecutedFlag == true ) {
                sendCMD("OutputExecutedTag=Y");
                while (!CmdExecuted) { System.Threading.Thread.Sleep(10); }
            }
            return true;
        }

        /// <summary>
        /// 进入 Measure 步骤
        /// </summary>
        protected override void internalEnterMeasureStep()
        {
            base.internalEnterMeasureStep();

            if(sampleModel == InstSampleMode.AutoSample_Fmt0 || sampleModel == InstSampleMode.AutoSample_Fmt1)
            {
                currentCmd = SBE37Cmd.Start;
                CmdExecuted = false;

                sendCMD("Start");
            }
            else
            {
                _tickTimerInst.Start();
            }
        }
        /// <summary>
        /// 执行 Measure 步骤
        /// </summary>
        protected override void internalMeasureStep()
        {
            if (sampleModel == InstSampleMode.AutoSample_Fmt0 || sampleModel == InstSampleMode.AutoSample_Fmt1)
            {
                
            }
            else
            {
                if(currentCmd == SBE37Cmd.Tc)
                {
                    currentCmd = SBE37Cmd.Tcr;
                    CmdExecuted = false;
                    sendCMD("Tcr");
                }
                else if(currentCmd == SBE37Cmd.Tcr)
                {
                    currentCmd = SBE37Cmd.Tc;
                    CmdExecuted = false;
                    sendCMD("Tc");
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
            if (sampleModel == InstSampleMode.AutoSample_Fmt0 || sampleModel == InstSampleMode.AutoSample_Fmt1)
            {
                currentCmd = SBE37Cmd.Stop;
                CmdExecuted = false;

                sendCMD("Stop");
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
                else
                {
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
                case SBE37Cmd.Start:
                case SBE37Cmd.Tc:
                case SBE37Cmd.Tcr:
                    // 是否会发生错误呢？
                    rlt = SBE37ResolveStr2Data(str, out data);
                    break;

                case SBE37Cmd.Dc:
                    rlt = GetCalibCoeff(str);
                    data = null;
                    break;

                default:
                    // 这里就产生了错误
                    // 因为现有的指令中，不会有其他的指令会返回数据
                    rlt = false;
                    data = null;
                    break;
            }

            return rlt;
        }


        /// <summary>
        /// 获取仪器信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool GetCalibCoeff(string str)
        {
            // 清空原有的字符串
            CalibCoeff = "";

            // bug: 这里，在显示信息的时候，是以 <Executed/> 为结尾的
            // 或者，这里就干脆用超时代表结束吧
            try
            {
                CalibCoeff += str + "\r";
                string data2 = sPort.ReadLine();
                while (!data2.Contains(OutputFlag))
                {
                    CalibCoeff += data2 + "\r";
                }

                CmdExecuted = true;
            }
            catch(Exception ex)
            {
                CmdExecuted = true;
                // return false;
            }
            
            return true;
        }

        /// <summary>
        /// SBE37 解析数据
        /// </summary>
        /// <param name="str"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool SBE37ResolveStr2Data(string str, out InstSBE37Data data)
        {
            data = null;

            try
            {
                // 日期（yyyy_MM_dd HH:mm: ss）-电导率频率 - 标准电导率示值 - 温度频率 - 标准温度示值 - 盐度 - 标志数
                string[] valStrs = str.Split('-');

                instData1Cache.vTestID = Info.testId;
                instData1Cache.vInstrumentID = Info.instrumentId;
                instData1Cache.vItemType = "";
                


                if(sampleModel == InstSampleMode.PolledSample_Fmt10)
                {

                }
                else
                {

                }


                // 解析数据
                switch (outputFormat)
                {
                    case SBE37OutputFormat.Format_0:
                        int idx0 = 0;
                        foreach(TypeSensorFmt0 itm in Enum.GetValues(typeof(TypeSensorFmt0)))
                        {
                            // 该位被置位
                            if (Info.SensorFlagFmt0.HasFlag(itm))
                            {
                                switch (itm)
                                {
                                    case TypeSensorFmt0.tt:
                                        // tttttt = temperature A/D counts.
                                        int tt = int.Parse(valStrs[idx0]);
                                        break;
                                    case TypeSensorFmt0.cc:
                                        // ccccc.ccc = conductivity frequency (Hz).
                                        double cc = double.Parse(valStrs[idx0]);
                                        break;
                                    case TypeSensorFmt0.pp:
                                        // pppppp = pressure sensor pressure A/D counts; 
                                        // sent if optional pressure sensor installed.
                                        int pp = int.Parse(valStrs[idx0]);
                                        break;

                                    case TypeSensorFmt0.vv:
                                        // vvvv = pressure sensor pressure temperature compensation A/D counts; 
                                        // sent if optional pressure sensor installed.
                                        int vv = int.Parse(valStrs[idx0]);
                                        break;

                                    case TypeSensorFmt0.oo:
                                        // oo.ooo = oxygen sensor phase (µsec).
                                        double oo = double.Parse(valStrs[idx0]);
                                        break;

                                    case TypeSensorFmt0.ot:
                                        // t.tttttt = oxygen sensor temperature voltage.
                                        double ot = double.Parse(valStrs[idx0]);
                                        break;

                                    case TypeSensorFmt0.dm:
                                        break;

                                    case TypeSensorFmt0.hm:
                                        break;

                                    default:
                                        return false;
                                        break;
                                }

                                idx0++;
                            }
                        }
                        
                        // 返回数据缓存
                        data = instData1Cache;
                        break;

                    case SBE37OutputFormat.Format_1:
                        int idx1 = 0;
                        foreach (TypeSensorFmt1 itm in Enum.GetValues(typeof(TypeSensorFmt1)))
                        {
                            // 该位被置位
                            if (Info.SensorFlagFmt0.HasFlag(itm))
                            {
                                switch (itm)
                                {
                                    case TypeSensorFmt1.tt:
                                        // tttt.tttt = temperature (°C, ITS-90).
                                        double tt = double.Parse(valStrs[idx1]);
                                        break;
                                    case TypeSensorFmt1.cc:
                                        // ccccc.ccc = conductivity (S/m).
                                        double cc = double.Parse(valStrs[idx1]);
                                        break;
                                    case TypeSensorFmt1.pp:
                                        // ppppp.ppp = pressure (decibars);
                                        // sent if optional pressure sensor installed.
                                        int pp = int.Parse(valStrs[idx1]);
                                        break;

                                    case TypeSensorFmt1.dd:
                                        // dddd.ddd = depth (meters);
                                        // sent if optional pressure sensor installed.
                                        int dd = int.Parse(valStrs[idx1]);
                                        break;

                                    case TypeSensorFmt1.oo:
                                        // oo.ooo = oxygen (sent if OutputOx=Y; units defined by SetOxUnits =).
                                        double oo = double.Parse(valStrs[idx1]);
                                        break;

                                    case TypeSensorFmt1.ss:
                                        // ssss.ssss = salinity (psu); sent only if OutputSal=Y.
                                        double ss = double.Parse(valStrs[idx1]);
                                        break;

                                    case TypeSensorFmt1.vv:
                                        // vvvv.vvv = sound velocity (meters/second); sent only if OutputSV=Y.
                                        double vv = double.Parse(valStrs[idx1]);
                                        break;

                                    case TypeSensorFmt1.rr:
                                        // rrr.rrrr = local density (kg/m3); sent only if OutputDensity=Y.
                                        double rr = double.Parse(valStrs[idx1]);
                                        break;

                                    case TypeSensorFmt1.sc:
                                        // ccc.ccccc = Specific Conductivity (S/m)
                                        double sc = double.Parse(valStrs[idx1]);
                                        break;

                                    case TypeSensorFmt1.xx:
                                        // x = specific conductivity; sent if OutputSC=Y
                                        int xx = int.Parse(valStrs[idx1]);
                                        break;

                                    case TypeSensorFmt1.dm:
                                        // dd mmm yyyy = day, month, year.
                                        double dm = double.Parse(valStrs[idx1]);
                                        break;

                                    case TypeSensorFmt1.hm:
                                        // hh:mm:ss = hour, minute, second.
                                        double hm = double.Parse(valStrs[idx1]);
                                        break;

                                    case TypeSensorFmt1.nn:
                                        // n = sample number in FLASH memory (sent if TxSampleNum=y,
                                        int nn = int.Parse(valStrs[idx1]);
                                        break;

                                    default:
                                        return false;
                                        break;
                                }

                                idx1++;
                            }
                        }

                        // 返回数据缓存
                        data = instData1Cache;
                        break;

                    case SBE37OutputFormat.Format_2:
                        data = null;
                        return false;
                        break;

                    case SBE37OutputFormat.Format_3:
                        data = null;
                        return false;
                        break;

                    default:
                        data = null;
                        return false;
                        break;
                }



                // 在不是 start 的情况下，数据后面都会跟随有 <Executed/>
                if (UseExecutedFlag == true && currentCmd != SBE37Cmd.Start)
                {
                    // 读取数据末尾跟随的结束标志位
                    string str2 = sPort.ReadLine();
                    if (!str2.Contains(OutputFlag))
                    {
                        return false;
                    }
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
