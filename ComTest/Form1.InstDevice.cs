using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Diagnostics;
using System.Drawing;
using System.Timers;

namespace ComTest
{
    public partial class Form1
    {
        /// <summary>
        /// 标准仪器 - 通信端口
        /// </summary>
        private SerialPort sPortSTD = new SerialPort();
        private bool SportSTD_enable = false;
        private Timer _stdTimer = new Timer();

        /// <summary>
        /// SBE37SM仪器 - 通信端口
        /// </summary>
        private SerialPort sPortSBE37SM = new SerialPort();
        private bool SportSBE37SM_enable = false;

        /// <summary>
        /// SBE37SM仪器 - 通信端口
        /// </summary>
        private SerialPort sPortSBE37SMP = new SerialPort();
        private bool SportSBE37SMP_enable = false;

        ///////////////////////////////
        // 仪器设备
        ///
        /// <summary>
        /// 采样状态
        /// </summary>
        enum SampleFormat : int
        {
            /// <summary>
            /// 轮询采样 + 数据格式 0
            /// </summary>
            PolledSampleFormat_0 = 0,
            /// <summary>
            /// 轮询采样 + 数据格式 1
            /// </summary>
            PolledSampleFormat_1,
            /// <summary>
            /// 自动采样 + 数据格式 0
            /// </summary>
            AutoSampleFormat_0,
            /// <summary>
            /// 自动采样 + 数据格式 1
            /// </summary>
            AutoSampleFormat_1
        }
        /// <summary>
        /// 仪器温度值
        /// </summary>
        float srValue = 12.00f;
        /// <summary>
        /// 锁 - 用于锁定仪器状态参数
        /// </summary>
        object srLocker = new object();
        /// <summary>
        /// 仪器 - 错误状态
        /// </summary>
        SampleFormat srErrStatus = SampleFormat.AutoSampleFormat_0;
        /// <summary>
        /// 仪器 - 在产生一次错误状态后，是否保持错误状态 - 默认不保持
        /// </summary>
        bool srErrLast = false;
        /// <summary>
        /// 仪器设备代码错误
        /// </summary>
        bool srCodeErr = false;


        ////////////////////////////////////
        // 端口数据传输函数
        ////////////////////////////////////

        string cdString = "<ConfigurationData DeviceType='SBE37SI-RS232' SerialNumber='03730050'>\r"
                            + "<PressureInstalled>no</PressureInstalled>\r"
                            + "<ReferencePressure>0.000000e+00</ReferencePressure>\r"
                            + "<PumpInstalled>No</PumpInstalled>\r"
                            + "<SampleMode>single sample</SampleMode>\r"
                            + "<SampleDataFormat>XML</SampleDataFormat>\r"
                            + "<ConductivityUnits>S/m</ConductivityUnits>\r"
                            + "<OutputSalinity>yes</OutputSalinity>\r"
                            + "<OutputSV>no</OutputSV>\r"
                            + "<OutputDepth>yes</OutputDepth>\r"
                            + "<Latitude>10.0</Latitude>\r"
                            + "<OutputDensity>yes</OutputDensity>\r"
                            + "<TxSampleNumber>yes</TxSampleNumber>\r"
                            + "<SampleInterval>6</SampleInterval>\r"
                            + "<OutputTime>no</OutputTime>\r"
                            + "<AutoRun>no</AutoRun>\r"
                            + "<StoreData>yes</StoreData>\r"
                            + "</ConfigurationData>";

        /// <summary>
        /// 标准仪器发送数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _stdTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (SportSTD_enable == false) return;

            try
            {
                if (!sPortSTD.IsOpen) sPortSTD.Open();

                string msg = DateTime.Now.ToString("yyyy_MM_dd HH:mm:ss");
                msg = msg + "-12.000000-12.000000-12.000000-12.00000-" + (12.0f + (hScrollBar_Sr.Value - 50) * 0.2f).ToString("0.000000") + "-37";

                sPortSTD.WriteLine(msg);
            }
            catch(Exception ex)
            {
                nlogger.Error("Exception in std timer func: " + ex.Message);
            }
        }

        /// <summary>
        /// SBE37SM 仪器 - 信息收发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SPortSBE37SM_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string dataRev = string.Empty;

            try
            {
                dataRev = sPortSBE37SM.ReadLine();
                sPortSBE37SM.DiscardInBuffer();
            }
            catch(Exception ex)
            {
                Debug.WriteLine("SPortSBE37SM_DataReceived Exception when ReadData : " + ex.Message);

                // 出现代码错误
                srCodeErr = true;
            }

            lock(ryLocker)
            {
                // 调试信息
                Debug.WriteLine("仪器设备 sPortSBE37SM 读取到了数据：" + dataRev + " 设备状态：" + srErrStatus.ToString());

                try
                {
                    // 正常工作状态
                    if (dataRev.Contains("GetCD"))
                    {
                        // 返回数据
                        sPortSBE37SM.WriteLine(cdString);
                        sPortSBE37SM.WriteLine("<Executed/>");
                    }
                    else if (dataRev.Contains("GetCC"))
                    {
                        // 返回数据
                        // sPortSBE37SM.WriteLine("   0.3034,  2.91179,    0.607, 24 Sep 2019, 17:22:28");
                    }
                    else if (dataRev.Contains("start"))
                    {
                        sPortSBE37SM.WriteLine("   0.3034,  2.91179,    0.607, 24 Sep 2019, 17:22:28");
                    }
                    else if (dataRev.Contains("stop"))
                    {
                        if (this.checkBox_flag.Checked) { sPortSBE37SM.WriteLine("<Executed/>"); }
                    }
                    else if (dataRev.Contains("ts"))
                    {
                        sPortSBE37SM.WriteLine("   0.3034,  2.91179,    0.607, 24 Sep 2019, 17:22:28");
                        if (this.checkBox_flag.Checked) { sPortSBE37SM.WriteLine("<Executed/>"); }
                    }
                    else if (dataRev.Contains("tsr"))
                    {
                        sPortSBE37SM.WriteLine("   0.3034,  2.91179,    0.607, 24 Sep 2019, 17:22:28");
                        if (this.checkBox_flag.Checked) { sPortSBE37SM.WriteLine("<Executed/>"); }
                    }
                    else
                    {

                    }
                    
                }
                catch(Exception ex)
                {
                    Debug.WriteLine("SPortSBE37SM_DataReceived exception when writeData : " + ex.Message);

                    // 出现代码错误
                    srCodeErr = true;
                }
            }
        }

        /// <summary>
        /// SBE37SM 仪器 - 信息收发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SPortSBE37SMP_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string dataRev = string.Empty;

            try
            {
                dataRev = sPortSBE37SMP.ReadLine();
                sPortSBE37SMP.DiscardInBuffer();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SPortSBE37SMP_DataReceived Exception when ReadData : " + ex.Message);

                // 出现代码错误
                srCodeErr = true;
            }

            lock (ryLocker)
            {
                // 调试信息
                Debug.WriteLine("仪器设备 sPortSBE37SM 读取到了数据：" + dataRev + " 设备状态：" + srErrStatus.ToString());

                try
                {
                    // 正常工作状态
                    if (dataRev.Contains("GetCD"))
                    {
                        // 返回数据
                        sPortSBE37SMP.WriteLine(cdString);
                        sPortSBE37SMP.WriteLine("<Executed/>");
                    }
                    else if (dataRev.Contains("GetCC"))
                    {
                        // 返回数据
                        // sPortSBE37SM.WriteLine("   0.3034,  2.91179,    0.607, 24 Sep 2019, 17:22:28");
                    }
                    else if (dataRev.Contains("start"))
                    {
                        sPortSBE37SMP.WriteLine("   0.3034,  2.91179,    0.607, 24 Sep 2019, 17:22:28");
                    }
                    else if (dataRev.Contains("stop"))
                    {
                        if (this.checkBox_flag.Checked) { sPortSBE37SMP.WriteLine("<Executed/>"); }
                    }
                    else if (dataRev.Contains("ts"))
                    {
                        sPortSBE37SMP.WriteLine("   0.3034,  2.91179,    0.607, 24 Sep 2019, 17:22:28");
                        if (this.checkBox_flag.Checked) { sPortSBE37SMP.WriteLine("<Executed/>"); }
                    }
                    else if (dataRev.Contains("tsr"))
                    {
                        sPortSBE37SMP.WriteLine("   0.3034,  2.91179,    0.607, 24 Sep 2019, 17:22:28");
                        if (this.checkBox_flag.Checked) { sPortSBE37SMP.WriteLine("<Executed/>"); }
                    }
                    else
                    {

                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine("SPortSBE37SM_DataReceived exception when writeData : " + ex.Message);

                    // 出现代码错误
                    srCodeErr = true;
                }
            }

        }

    }
}
