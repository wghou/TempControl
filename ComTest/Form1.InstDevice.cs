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
        /// 仪器设备错误状态
        /// </summary>
        enum SrStatus : int
        {
            /// <summary>
            /// 正常工作状态
            /// </summary>
            OK = 0,
            /// <summary>
            /// 连接断开
            /// </summary>
            DisConnected = 1,
            /// <summary>
            /// 数据错误
            /// </summary>
            DataErr = 2,
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
        SrStatus srErrStatus = SrStatus.OK;
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
                    switch (srErrStatus)
                    {
                        case SrStatus.OK:
                            // 正常工作状态
                            if(dataRev.Contains("tsr"))
                            {
                                // 返回数据
                                sPortSBE37SM.WriteLine("602818,  5298.894, 524770, 2252,  25 Mar 2020, 23:15:15");
                            }
                            else if (dataRev.Contains("ts"))
                            {
                                // 返回数据
                                sPortSBE37SM.WriteLine("   0.3034,  2.91179,    0.607, 24 Sep 2019, 17:22:28");
                            }

                            System.Threading.Thread.Sleep(5);
                            // 返回指令执行标志
                            sPortSBE37SM.WriteLine("<Executed/>");

                            //Debug.WriteLine("仪器设备返回了数据：" + dataRev.ToString());
                            break;
                        case SrStatus.DisConnected:
                            // 连接断开状态 - 不返回任何数据
                            break;
                        case SrStatus.DataErr:
                            // 错误状态
                            //sPortSBE37SM.WriteLine("@35EB:");
                            //Debug.WriteLine("仪器错误数据： @035EB.");
                            break;
                        default:
                            //Debug.WriteLine("仪器设备返回了数据：" + dataRev.ToString());
                            break;
                    }
                }
                catch(Exception ex)
                {
                    Debug.WriteLine("SPortSBE37SM_DataReceived exception when writeData : " + ex.Message);

                    // 出现代码错误
                    srCodeErr = true;
                }

                // 如果错误不持续，则清空错误标记为
                if (!srErrLast && srErrStatus != SrStatus.OK)
                    this.BeginInvoke(new EventHandler(delegate { srErrStatus = SrStatus.OK; this.comboBox_SrStatus.SelectedIndex = 0; }));
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
                Debug.WriteLine("仪器设备 sPortSBE37SMP 读取到了数据：" + dataRev + " 设备状态：" + srErrStatus.ToString());

                try
                {
                    switch (srErrStatus)
                    {
                        case SrStatus.OK:
                            // 正常工作状态
                            if (dataRev.Contains("tsr"))
                            {
                                // 返回数据
                                sPortSBE37SMP.WriteLine("602818,  5298.894, 524770, 2252,  25 Mar 2020, 23:15:15");
                            }
                            else if (dataRev.Contains("ts"))
                            {
                                // 返回数据
                                sPortSBE37SMP.WriteLine("   0.3034,  2.91179,    0.607, 24 Sep 2019, 17:22:28");
                            }
                            System.Threading.Thread.Sleep(5);
                            // 返回指令执行标志
                            sPortSBE37SMP.WriteLine("<Executed/>");

                            //Debug.WriteLine("仪器设备返回了数据：" + dataRev.ToString());
                            break;
                        case SrStatus.DisConnected:
                            // 连接断开状态 - 不返回任何数据
                            break;
                        case SrStatus.DataErr:
                            // 错误状态
                            //sPortSBE37SMP.WriteLine("@35EB:");
                            //Debug.WriteLine("仪器错误数据： @035EB.");
                            break;
                        default:
                            //Debug.WriteLine("仪器设备返回了数据：" + dataRev.ToString());
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("SPortSBE37SMP_DataReceived exception when writeData : " + ex.Message);

                    // 出现代码错误
                    srCodeErr = true;
                }

                // 如果错误不持续，则清空错误标记为
                if (!srErrLast && srErrStatus != SrStatus.OK)
                    this.BeginInvoke(new EventHandler(delegate { srErrStatus = SrStatus.OK; this.comboBox_SrStatus.SelectedIndex = 0; }));
            }
        }

    }
}
