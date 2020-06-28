using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Diagnostics;
using System.Drawing;

namespace ComTest
{
    public partial class Form1
    {
        ////////////////////////////
        // 参数 - 用于生成相关类型的数据曲线
        ////////////////////////////


        ///////////////////////////////
        // 控温槽状态
        //
        /// <summary>
        /// 温控设备错误状态
        /// </summary>
        enum TempStatus : int
        {
            /// <summary>
            /// 工作状态正常
            /// </summary>
            OK = 0,
            /// <summary>
            /// 连接断开 - 即不返回数据
            /// </summary>
            DisConnected = 1,
            /// <summary>
            /// 数据校验错误
            /// </summary>
            DataErr = 2,
            /// <summary>
            /// 温度不下降
            /// </summary>
            //TempNotDown = 3,
            /// <summary>
            /// 温度不上升
            /// </summary>
            //TempNotUp = 4,
            /// <summary>
            /// 温度波动度大
            /// </summary>
            //TempFlucLarge = 5,
            /// <summary>
            /// 温度偏离设定值
            /// </summary>
            //TempBasis = 6,
            /// <summary>
            /// 温度超出设定界限
            /// </summary>
            //TempOutRange = 7,
        }

        /// <summary>
        /// 控温槽参数
        /// </summary>
        class TempParam
        {
            /// <summary>
            /// 当前温度
            /// </summary>
            public float CurTemp = 18.0f;
            /// <summary>
            /// 温度设定点
            /// </summary>
            public float SetTemp = 18.0f;
            /// <summary>
            /// 当前是否已经稳定
            /// </summary>
            public bool Steady = false;
            /// <summary>
            /// 温度变化速率 - 下降
            /// </summary>
            public float ChangeRate = 0.025f;
            /// <summary>
            /// 按正弦信号 - 0~10
            /// </summary>
            public int PhaseCount = 0;
            /// <summary>
            /// 温度波动度
            /// </summary>
            public float Fluc = 0.05f;
            /// <summary>
            /// 附加温度值
            /// </summary>
            public float CurTempPlus = 0.0f;
            /// <summary>
            /// 附加温度变化率
            /// </summary>
            public float ChangeRatePlus = 0.0f;
            /// <summary>
            /// 附加温度波动度
            /// </summary>
            public float FlucPlus = 0.0f;
        }

        ////////////////////////////////
        // 主控温槽状态
        ///
        /// <summary>
        /// 锁 - 用于锁定主控温槽状态参数
        /// </summary>
        object tmLocker = new object();
        /// <summary>
        /// 主控温槽 - 错误状态
        /// </summary>
        TempStatus tmErrStatus = TempStatus.OK;
        /// <summary>
        /// 主控温槽 - 在产生一次错误状态后，是否保持错误状态 - 默认不保持
        /// </summary>
        bool tmErrLast = false;
        /// <summary>
        /// 主控温槽设备代码错误
        /// </summary>
        bool tmCodeErr = false;
        /// <summary>
        /// 主控温槽 - 参数
        /// </summary>
        TempParam tmParam = new TempParam();


        //////////////////////////////
        // 辅控温槽状态
        ///
        /// <summary>
        /// 锁 - 用于锁定辅控温槽状态参数
        /// </summary>
        object tsLocker = new object();
        /// <summary>
        /// 辅控温槽 - 错误状态
        /// </summary>
        TempStatus tsErrStatus = TempStatus.OK;
        /// <summary>
        /// 辅控温槽 - 在产生一次错误状态后，是否保持错误状态 - 默认不保持
        /// </summary>
        bool tsErrLast = false;
        /// <summary>
        /// 辅控温槽代码错误
        /// </summary>
        bool tsCodeErr = false;
        /// <summary>
        /// 辅控温槽 - 参数
        /// </summary>
        TempParam tsParam = new TempParam();


        ///////////////////////////////
        // 传感器设备
        ///
        /// <summary>
        /// 传感器设备错误状态
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
        /// 传感器温度值
        /// </summary>
        float srValue = 12.00f;
        /// <summary>
        /// 锁 - 用于锁定传感器状态参数
        /// </summary>
        object srLocker = new object();
        /// <summary>
        /// 传感器 - 错误状态
        /// </summary>
        SrStatus srErrStatus = SrStatus.OK;
        /// <summary>
        /// 传感器 - 在产生一次错误状态后，是否保持错误状态 - 默认不保持
        /// </summary>
        bool srErrLast = false;
        /// <summary>
        /// 传感器设备代码错误
        /// </summary>
        bool srCodeErr = false;


        ////////////////////////////////////
        // 端口数据传输函数
        ////////////////////////////////////

        /// <summary>
        /// 辅槽控温 - 信息收发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SPortTs_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string dataRev = string.Empty;

            try
            {
                dataRev = sPortTs.ReadTo(":");
                sPortTs.DiscardInBuffer();
                //Debug.WriteLine("辅槽设备读取到了数据：" + dataRev.ToString());
            }
            catch(Exception ex)
            {
                // 端口接收数据时发生错误
                Debug.WriteLine("SPortTs_DataReceived Exception when readData : " + ex.Message);
                tsCodeErr = true;
            }
            
            lock(tsLocker)
            {
                // 调试信息输出
                //Debug.WriteLine("辅槽设备读取到了数据：" + dataRev + " 设备状态：" + tsErrStatus.ToString());

                try
                {
                    switch(tsErrStatus)
                    {
                        case TempStatus.OK:
                            processDataTs(dataRev);
                            break;

                        case TempStatus.DisConnected:
                            // 断开状态，不返回任何数据
                            break;

                        case TempStatus.DataErr:
                            // 返回错误数据
                            sPortTs.WriteLine("@35EB:");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    // 端口接收数据时发生错误
                    Debug.WriteLine("SPortTs_DataReceived Exception when writeData : " + ex.Message);
                    tsCodeErr = true;
                }

                // 如果错误不持续，则清空错误标记为
                if (!tsErrLast && tsErrStatus != TempStatus.OK) this.BeginInvoke(new EventHandler(delegate { tsErrStatus = TempStatus.OK; }));
            }
            return;
        }

        void processDataTs(string dataRev)
        {
            // 先判断温度是否已经稳定
            if (Math.Abs(tsParam.CurTemp - tsParam.SetTemp) * 2 < Math.Abs(tsParam.ChangeRate + tsParam.ChangeRatePlus)) tsParam.Steady = true;
            else tsParam.Steady = false;

            if (dataRev.Contains("W"))
            {
                // 上位机写入数据 - 一般是参数

                // 从上位机数据中提取正确数据 - 如 温度设定点等
                // wghou
                if(dataRev[4] == 'A')
                {
                    // 温度设定值
                    float val = 18.0f;
                    if(!float.TryParse(dataRev.Substring(5),out val))
                    {
                        // 发生了错误
                        sPortTs.WriteLine("@35EB:");
                        return;
                    }

                    Debug.WriteLine("辅槽更新了温度设定值：" + val.ToString());
                    // 更新温度设定值
                    tsParam.SetTemp = val;
                    this.BeginInvoke(new EventHandler(delegate { this.label_SetTempS.Text = val.ToString("0.000"); }));
                }

                // 向上位机返回正确数据
                dataRev = dataRev.Substring(0, 5) + ":";
                sPortTs.WriteLine(dataRev);
            }
            else if (dataRev.Contains("R"))
            {
                // 上位机读取数据
                if(dataRev[4] == 'H')
                {
                    // 读取温度显示值
                    // 计算出当前的温度显示值

                    // 如果未稳定，则使温度发生一个变化
                    if(!tsParam.Steady) tsParam.CurTemp += (tsParam.ChangeRate + tsParam.ChangeRatePlus) * Math.Sign(tsParam.SetTemp - tsParam.CurTemp);

                    // 如果改变后的温度值接近温度设定点，则设当前温度点为温度设定值
                    if(Math.Abs(tsParam.CurTemp - tsParam.SetTemp) <= Math.Abs((tsParam.ChangeRate + tsParam.ChangeRatePlus)/2))
                    {
                        tsParam.CurTemp = tsParam.SetTemp;
                        tsParam.Steady = true;
                    }

                    // 计算波动度
                    tsParam.PhaseCount++;
                    if (tsParam.PhaseCount >= 20) tsParam.PhaseCount = 0;

                    float fluc = (tsParam.Fluc + tsParam.FlucPlus) * (float)Math.Sin(3.1415 * tsParam.PhaseCount / 10) / 2;

                    // 如果稳定，则波动度为零
                    //if (tsParam.Steady) fluc = 0;

                    // 最终的显示值 = 当前温度值 + 附加温度值 + 计算波动度 + 附加波动度
                    float val = tsParam.CurTemp + tsParam.CurTempPlus + fluc;

                    // 界面显示的当前温度值为 curTemp + 波动度
                    this.BeginInvoke(new EventHandler(delegate { this.label_CurTempS.Text = val.ToString("0.000"); }));

                    dataRev += val.ToString("0.000");
                    dataRev += ":";
                    sPortTs.WriteLine(dataRev);
                }
                else
                {
                    dataRev += "12.000";
                    dataRev += ":";
                    sPortTs.WriteLine(dataRev);
                }
                
            }
            else
            {
                // 未知指令
                // 指令不存在
                sPortTs.WriteLine("@35EB:");
            }
        }


        /// <summary>
        /// 主槽控温 - 信息收发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SPortTm_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string dataRev = string.Empty;

            try
            {
                dataRev = sPortTm.ReadTo(":");
                sPortTm.DiscardInBuffer();
                //Debug.WriteLine("主槽设备读取到了数据：" + dataRev.ToString());
            }
            catch (Exception ex)
            {
                // 端口接收数据时发生错误
                Debug.WriteLine("SPortTm_DataReceived Exception when readData : " + ex.Message);
                tmCodeErr = true;
            }

            lock (tmLocker)
            {
                // 调试信息输出
                //Debug.WriteLine("主槽设备读取到了数据：" + dataRev + " 设备状态：" + tmErrStatus.ToString());

                try
                {
                    switch (tmErrStatus)
                    {
                        case TempStatus.OK:
                            processDataTm(dataRev);
                            break;

                        case TempStatus.DisConnected:
                            // 断开状态，不返回任何数据
                            break;

                        case TempStatus.DataErr:
                            // 返回错误数据
                            sPortTm.WriteLine("@35EB:");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    // 端口接收数据时发生错误
                    Debug.WriteLine("SPortTm_DataReceived Exception when writeData : " + ex.Message);
                    tmCodeErr = true;
                }

                // 如果错误不持续，则清空错误标记为
                if (!tmErrLast) tmErrStatus = TempStatus.OK;
            }
            return;
        }

        void processDataTm(string dataRev)
        {
            // 先判断温度是否已经稳定
            if (Math.Abs(tmParam.CurTemp - tmParam.SetTemp) * 2.0f < Math.Abs(tmParam.ChangeRate + tmParam.ChangeRatePlus)) tmParam.Steady = true;
            else tmParam.Steady = false;

            if (dataRev.Contains("W"))
            {
                // 上位机写入数据 - 一般是参数

                // 从上位机数据中提取正确数据 - 如 温度设定点等
                // wghou
                if (dataRev[4] == 'A')
                {
                    // 温度设定值
                    float val = 18.0f;
                    if (!float.TryParse(dataRev.Substring(5), out val))
                    {
                        // 发生了错误
                        sPortTm.WriteLine("@35EB:");
                        return;
                    }

                    Debug.WriteLine("主槽更新了温度设定值：" + val.ToString());
                    // 更新温度设定值
                    tmParam.SetTemp = val;

                    // 更新界面显示值
                    this.BeginInvoke(new EventHandler(delegate { this.label_SetTempM.Text = val.ToString("0.0000"); }));
                }

                // 向上位机返回正确数据
                dataRev = dataRev.Substring(0, 5) + ":";
                sPortTm.WriteLine(dataRev);
            }
            else if (dataRev.Contains("R"))
            {
                // 上位机读取数据
                if (dataRev[4] == 'H')
                {
                    // 读取温度显示值
                    // 计算出当前的温度显示值

                    // 如果未稳定，则使温度发生一个变化
                    if (!tmParam.Steady) tmParam.CurTemp += (tmParam.ChangeRate + tmParam.ChangeRatePlus) * Math.Sign(tmParam.SetTemp - tmParam.CurTemp);

                    // 如果改变后的温度值接近温度设定点，则设当前温度点为温度设定值
                    if (Math.Abs(tmParam.CurTemp - tmParam.SetTemp) * 2.0f <= Math.Abs(tmParam.ChangeRate + tmParam.ChangeRatePlus))
                    {
                        tmParam.CurTemp = tmParam.SetTemp;
                        tmParam.Steady = true;
                    }

                    // 计算波动度
                    tmParam.PhaseCount++;
                    if (tmParam.PhaseCount >= 30) tmParam.PhaseCount = 0;

                    float fluc = (tmParam.Fluc + tmParam.FlucPlus) * (float)Math.Sin(3.1415 * tmParam.PhaseCount / 15) / 2;

                    // 如果稳定，则波动度为零
                    //if (tmParam.Steady) fluc = 0;

                    // 最终的显示值 = 当前温度值 + 附加温度值 + 计算波动度 + 附加波动度
                    float val = tmParam.CurTemp + tmParam.CurTempPlus + fluc;

                    // 界面显示值 - 当前温度值 = curTemp + 波动度
                    this.BeginInvoke(new EventHandler(delegate { this.label_CurTempM.Text = val.ToString("0.0000"); }));

                    dataRev += val.ToString("0.0000");
                    dataRev += ":";
                    sPortTm.WriteLine(dataRev);
                }
                else
                {
                    dataRev += "12.0000";
                    dataRev += ":";
                    sPortTm.WriteLine(dataRev);
                }

            }
            else
            {
                // 未知指令
                // 指令不存在
                sPortTm.WriteLine("@35EB:");
            }
        }


        /// <summary>
        /// 电桥温度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SPortBg_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string dataRev = string.Empty;
            string dataSend = string.Empty;

            try
            {
                dataRev = sPortBg.ReadLine();
                sPortBg.DiscardInBuffer();
                //Debug.WriteLine("主槽设备读取到了数据：" + dataRev.ToString());
            }
            catch (Exception ex)
            {
                // 端口接收数据时发生错误
                Debug.WriteLine("SPortBg_DataReceived Exception when readData : " + ex.Message);
            }

            System.Threading.Thread.Sleep(200);
            Debug.WriteLine("电桥接收到数据：" + dataRev);

            if(dataRev.Contains("IDN"))
            {
                dataSend = "1594";
            }
            else if(dataRev.Contains("INIT:CONT 1"))
            {
                bridgeStatus = "ON";
                dataSend = string.Empty;
            }
            else if(dataRev.Contains("INIT:CONT 0"))
            {
                bridgeStatus = "OFF";
                dataSend = string.Empty;
            }
            else if(dataRev.Contains("FETCH? 1"))
            {
                dataSend = "35.666,C,1";
            }
            else if (dataRev.Contains("INIT:CONT?"))
            {
                dataSend = bridgeStatus;
                //dataSend = "OFF";
            }
            else
            {
                dataSend = string.Empty;
            }

            if(dataSend.Length !=0)
            {
                try
                {
                    sPortBg.WriteLine(dataSend);
                    Debug.WriteLine("电桥返回数据：" + dataSend);
                }
                catch(Exception ex)
                {

                }
            }
            else
            {
                Debug.WriteLine("电桥返回空数据");
            }
            
            return;
        }
    }
}
