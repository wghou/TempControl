﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ComTest
{
    public partial class Form1 : Form
    {

        Bitmap mBmp_Sr;

        private string confFile = "config.json";

        public Form1()
        {
            InitializeComponent();

            // 界面显示值
            this.label_SetTempM.Text = tmParam.SetTemp.ToString("0.0000");
            this.label_SteadyM.Text = tmParam.Steady.ToString();
            this.label_ChangeRateM.Text = tmParam.ChangeRate.ToString();
            this.label_FlucM.Text = tmParam.Fluc.ToString();

            this.label_SetTempS.Text = tsParam.SetTemp.ToString("0.0000");
            this.label_SteadyS.Text = tsParam.Steady.ToString();
            this.label_ChangeRateS.Text = tsParam.ChangeRate.ToString();
            this.label_FlucS.Text = tsParam.Fluc.ToString();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            try
            {
                System.IO.StreamReader file = System.IO.File.OpenText(confFile);
                JsonTextReader reader = new JsonTextReader(file);
                JObject obj = (JObject)JToken.ReadFrom(reader);

                // 主槽控温 - 通信端口
                if (obj.ContainsKey("TempM"))
                {
                    JObject child = (JObject)obj["TempM"];

                    SportTm_enable = child.ContainsKey("Enable") ? (bool)child["Enable"] : true;
                    if (SportTm_enable)
                    {
                        sPortTm.PortName = child.ContainsKey("PortName") ? child["PortName"].ToString() : "COM0";
                        sPortTm.BaudRate = child.ContainsKey("BaudRate") ? (int)child["BaudRate"] : 2400;
                        sPortTm.DataBits = 8;
                        sPortTm.StopBits = StopBits.One;
                        sPortTm.Parity = Parity.None;
                        sPortTm.ReadBufferSize = 64;
                        sPortTm.WriteBufferSize = 64;
                    }
                }

                // 辅槽控温 - 通信端口
                if (obj.ContainsKey("TempS"))
                {
                    JObject child = (JObject)obj["TempS"];

                    SportTs_enable = child.ContainsKey("Enable") ? (bool)child["Enable"] : true;
                    if (SportTs_enable)
                    {
                        sPortTs.PortName = child.ContainsKey("PortName") ? child["PortName"].ToString() : "COM1";
                        sPortTs.BaudRate = child.ContainsKey("BaudRate") ? (int)child["BaudRate"] : 2400;
                        sPortTs.DataBits = 8;
                        sPortTs.StopBits = StopBits.One;
                        sPortTs.Parity = Parity.None;
                        sPortTs.ReadBufferSize = 64;
                        sPortTs.WriteBufferSize = 64;
                    } 
                }

                // 继电器 - 通信端口
                initNModbus(obj);

                // 电桥 - 通信端口
                if (obj.ContainsKey("SensorSTD"))
                {
                    JObject child = (JObject)obj["SensorSTD"];

                    SportSTD_enable = child.ContainsKey("Enable") ? (bool)child["Enable"] : true;
                    if (SportSTD_enable)
                    {
                        sPortSTD.PortName = child.ContainsKey("PortName") ? child["PortName"].ToString() : "COM4";
                        sPortSTD.BaudRate = child.ContainsKey("BaudRate") ? (int)child["BaudRate"] : 9600;
                        sPortSTD.DataBits = 8;
                        sPortSTD.StopBits = StopBits.One;
                        sPortSTD.Parity = Parity.None;
                        sPortSTD.ReadBufferSize = 64;
                        sPortSTD.WriteBufferSize = 64;
                        sPortSTD.ReadTimeout = 500;

                        _stdTimer.Interval = 1000;
                        _stdTimer.AutoReset = true;
                        _stdTimer.Elapsed += _stdTimer_Elapsed;
                        _stdTimer.Start();
                    }
                }

                // 传感器 - 通信端口
                if (obj.ContainsKey("SensorSBE37"))
                {
                    JObject child = (JObject)obj["SensorSBE37"];

                    SportSBE37_enable = child.ContainsKey("Enable") ? (bool)child["Enable"] : true;
                    if (SportSBE37_enable)
                    {
                        sPortSBE37.PortName = child.ContainsKey("PortName") ? child["PortName"].ToString() : "COM5";
                        sPortSBE37.BaudRate = child.ContainsKey("BaudRate") ? (int)child["BaudRate"] : 9600;
                        sPortSBE37.DataBits = 8;
                        sPortSBE37.StopBits = StopBits.One;
                        sPortSBE37.Parity = Parity.None;
                        sPortSBE37.ReadBufferSize = 64;
                        sPortSBE37.WriteBufferSize = 64;
                    }
                }

                if(obj.ContainsKey("Socket"))
                {
                    JObject child = (JObject)obj["Socket"];

                    _socketClient_enable = child.ContainsKey("Enable") ? (bool)child["Enable"] : true;
                    if (_socketClient_enable)
                    {
                        _socketClient_enable = InitSocket(child);
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine("配置端口失败！ errMessage: " + ex.Message);
                this.Close();
                return;
            }


            // 添加串口收发信息事件
            if (SportTm_enable) sPortTm.DataReceived += SPortTm_DataReceived;
            if (SportTs_enable) sPortTs.DataReceived += SPortTs_DataReceived;
            if (SportSBE37_enable) sPortSBE37.DataReceived += SPortSBE37_DataReceived;


            // 打开端口
            try
            {
                if (SportTm_enable) sPortTm.Open();
                if (SportTs_enable) sPortTs.Open();
                if (SportSBE37_enable) sPortSBE37.Open();
                if (SportSTD_enable) sPortSTD.Open();
            }
            catch(Exception ex)
            {
                Debug.WriteLine("打开端口失败！ errMessage: " + ex.Message);
                this.Close();
                return;
            }


            this.comboBox_SrStatus.SelectedIndex = 0;
            this.comboBox_RyStatus.SelectedIndex = 0;



            // 个中能模块状态指示
            mBmp_Sr = new Bitmap(pictureBox_Sr.Width, pictureBox_Sr.Height);
            Graphics mGhp_Sr = Graphics.FromImage(mBmp_Sr);
            mGhp_Sr.Clear(Color.Green);
            pictureBox_Sr.Image = mBmp_Sr;
        }


        /// <summary>
        /// 传感器设备 - 错误状态设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_SrStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 说明：虽然在错误状态不保持的情况下，控制流程会修改 SelectedIndex ，会再次（冗余）触发该事件，但是也不会有太坏的影响
            lock(srLocker)
            {
                switch(comboBox_SrStatus.SelectedIndex)
                {
                    case 0:
                        this.srErrStatus = SrStatus.OK;
                        break;

                    case 1:
                        this.srErrStatus = SrStatus.DisConnected;
                        break;

                    case 2:
                        this.srErrStatus = SrStatus.DataErr;
                        break;

                    default:
                        this.srErrStatus = SrStatus.OK;
                        break;
                }
            }
        }

        /// <summary>
        /// 传感器设备 - 错误状态持续
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox_SrErrLast_CheckedChanged(object sender, EventArgs e)
        {
            lock(srLocker)
            {
                srErrLast = this.checkBox_SrErrLast.Checked;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // 关闭程序时，关闭串口
                if (sPortTm.IsOpen) sPortTm.Close();
                if (sPortTs.IsOpen) sPortTs.Close();
                if (sPortSBE37.IsOpen) sPortSBE37.Close();
                if (sPortSTD.IsOpen) sPortSTD.Close();

                slaveThreadM?.Abort();
                slaveThreadS?.Abort();
            }
            catch(Exception ex) { }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_RyStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 说明：虽然在错误状态不保持的情况下，控制流程会修改 SelectedIndex ，会再次（冗余）触发该事件，但是也不会有太坏的影响
            lock (ryLocker)
            {
                switch (comboBox_RyStatus.SelectedIndex)
                {
                    case 0:
                        this.ryErrStatus = RyStatus.OK;
                        break;

                    case 1:
                        this.ryErrStatus = RyStatus.DisConnected;
                        break;

                    case 2:
                        this.ryErrStatus = RyStatus.DataErr;
                        break;

                    default:
                        this.ryErrStatus = RyStatus.OK;
                        break;
                }
            }
        }


        /// <summary>
        /// 继电器设备 - 错误状态持续
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox_RyErrLast_CheckedChanged(object sender, EventArgs e)
        {
            lock(ryLocker)
            {
                ryErrLast = this.checkBox_RyErrLast.Checked;
            }
        }


        /// <summary>
        /// 主控温槽 - 当前温度附加值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hScrollBar_CurTempPlusM_ValueChanged(object sender, EventArgs e)
        {
            this.label_CurTempPlusM.Text = ((this.hScrollBar_CurTempPlusM.Value - 50) * 0.05).ToString();
            lock (tmLocker) { tmParam.CurTempPlus = (float)((this.hScrollBar_CurTempPlusM.Value - 50) * 0.05); }
        }


        /// <summary>
        /// 主控温槽 - 当前温度变化率附加值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hScrollBar_ChangeRatePlusM_ValueChanged(object sender, EventArgs e)
        {
            this.label_ChangeRatePlusM.Text = ((this.hScrollBar_ChangeRatePlusM.Value - 50) * 0.001).ToString();
            lock (tmLocker) { tmParam.ChangeRatePlus = (float)((this.hScrollBar_ChangeRatePlusM.Value - 50) * 0.001); }
        }


        /// <summary>
        /// 主控温槽 - 当前波动度附加值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hScrollBar_FlucPlusM_ValueChanged(object sender, EventArgs e)
        {
            this.label_FlucPlusM.Text = ((this.hScrollBar_FlucPlusM.Value - 50) * 0.001).ToString();
            lock (tmLocker) { tmParam.FlucPlus = (float)((this.hScrollBar_FlucPlusM.Value - 50) * 0.001); }
            this.label_FlucM.Text = (tmParam.Fluc + tmParam.FlucPlus).ToString();
        }



        /// <summary>
        /// 辅控温槽 - 当前温度附加值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hScrollBar_CurTempPlusS_ValueChanged(object sender, EventArgs e)
        {
            this.label_CurTempPlusS.Text = ((this.hScrollBar_CurTempPlusS.Value - 50) * 0.05).ToString();
            lock (tsLocker) { tsParam.CurTempPlus = (float)((this.hScrollBar_CurTempPlusS.Value - 50) * 0.05); }
        }



        /// <summary>
        /// 辅控温槽 - 当前温度变化率附加值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hScrollBar_ChangeRatePlusS_ValueChanged(object sender, EventArgs e)
        {
            this.label_ChangeRatePlusS.Text = ((this.hScrollBar_ChangeRatePlusS.Value - 50) * 0.001).ToString();
            lock (tsLocker) { tsParam.ChangeRatePlus = (float)((this.hScrollBar_ChangeRatePlusS.Value - 50) * 0.001); }
        }



        /// <summary>
        /// 辅控温槽 - 当前波动度附加值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hScrollBar_FlucPlusS_ValueChanged(object sender, EventArgs e)
        {
            this.label_FlucPlusS.Text = ((this.hScrollBar_FlucPlusS.Value - 50) * 0.001).ToString();
            lock (tsLocker) { tsParam.FlucPlus = (float)((this.hScrollBar_FlucPlusS.Value - 50) * 0.001); }
            this.label_FlucS.Text = (tsParam.Fluc + tsParam.FlucPlus).ToString();
        }

        private void hScrollBar_Sr_ValueChanged(object sender, EventArgs e)
        {
            this.label_Sr.Text = (12.0f + (hScrollBar_Sr.Value - 50) * 0.2f).ToString("0.000");

            lock (srLocker) { this.srValue = 12.0f + (hScrollBar_Sr.Value - 50) * 0.2f; }
        }
    }
}
