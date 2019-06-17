﻿using System;
using System.Windows.Forms;

namespace TempControl
{
    public partial class FormMain
    {
        private delegate void ry();
        ////////////////    勾选框    /////////////
        private void checkBox_tempS_CheckedChanged(object sender, EventArgs e)
        {
            _device.tpDeviceS.Enable = checkBox_tempS.Checked;
            this.groupBox_tempS.Enabled = checkBox_tempS.Checked;
        }

        private void checkBox_ryEn2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox_ryEn2.Checked) {
                _device.ryDeviceS.Enable = true;
                _device.ryDeviceM.numCoils = 8;
                this.groupBox_ry2.Text = "继电器模块 2";
            }
            else {
                _device.ryDeviceS.Enable = false;
                _device.ryDeviceM.numCoils = 16;
                this.groupBox_ry2.Text = "继电器模块 1 (备用)";
            }
        }

        private void checkBox_protect_CheckedChanged(object sender, EventArgs e)
        {
            _device.ryDeviceM.DisconnectProtect = this.checkBox_protect.Checked;
            _device.ryDeviceS.DisconnectProtect = this.checkBox_protect.Checked;
        }

        /// <summary> 设置继电器设备状态 - 委托 </summary>
        private delegate void RySetHandler(bool cntErr);

        private void checkBox_ryM0_Click(object sender, EventArgs e)
        {
            _device.ryDeviceM.ryStatusToSet[(int)Device.RelayDevice.Cmd_r.OUT_0] = this.checkBox_ryM0.Checked;
            RySetHandler setRyStatus = new RySetHandler(this._device.WriteRelayDeviceM);
            setRyStatus.BeginInvoke(false, null, null);

            if (checkBox_ryM0.Checked == true)
            {
                Utils.Logger.Sys("打开电源 M0!");
                Utils.Logger.Op("打开电源 M0!");
            }
            else
            {
                Utils.Logger.Sys("关闭电源 M0!");
                Utils.Logger.Op("关闭电源 M0!");
            }
        }

        private void checkBox_ryM1_Click(object sender, EventArgs e)
        {
            _device.ryDeviceM.ryStatusToSet[(int)Device.RelayDevice.Cmd_r.OUT_1] = this.checkBox_ryM1.Checked;
            RySetHandler setRyStatus = new RySetHandler(this._device.WriteRelayDeviceM);
            setRyStatus.BeginInvoke(false, null, null);

            if (checkBox_ryM1.Checked == true)
            {
                Utils.Logger.Sys("打开电源 M1!");
                Utils.Logger.Op("打开电源 M1!");
            }
            else
            {
                Utils.Logger.Sys("关闭电源 M1!");
                Utils.Logger.Op("关闭电源 M1!");
            }
        }

        private void checkBox_ryM2_Click(object sender, EventArgs e)
        {
            _device.ryDeviceM.ryStatusToSet[(int)Device.RelayDevice.Cmd_r.OUT_2] = this.checkBox_ryM2.Checked;
            RySetHandler setRyStatus = new RySetHandler(this._device.WriteRelayDeviceM);
            setRyStatus.BeginInvoke(false, null, null);

            if (checkBox_ryM2.Checked == true)
            {
                Utils.Logger.Sys("打开电源 M2!");
                Utils.Logger.Op("打开电源 M2!");
            }
            else
            {
                Utils.Logger.Sys("关闭电源 M2!");
                Utils.Logger.Op("关闭电源 M2!");
            }
        }

        private void checkBox_ryM3_Click(object sender, EventArgs e)
        {
            _device.ryDeviceM.ryStatusToSet[(int)Device.RelayDevice.Cmd_r.OUT_3] = this.checkBox_ryM3.Checked;
            RySetHandler setRyStatus = new RySetHandler(this._device.WriteRelayDeviceM);
            setRyStatus.BeginInvoke(false, null, null);

            if (checkBox_ryM3.Checked == true)
            {
                Utils.Logger.Sys("打开电源 M3!");
                Utils.Logger.Op("打开电源 M3!");
            }
            else
            {
                Utils.Logger.Sys("关闭电源 M3!");
                Utils.Logger.Op("关闭电源 M3!");
            }
        }

        private void checkBox_ryM4_Click(object sender, EventArgs e)
        {
            _device.ryDeviceM.ryStatusToSet[(int)Device.RelayDevice.Cmd_r.OUT_4] = this.checkBox_ryM4.Checked;
            RySetHandler setRyStatus = new RySetHandler(this._device.WriteRelayDeviceM);
            setRyStatus.BeginInvoke(false, null, null);

            if (checkBox_ryM4.Checked == true)
            {
                Utils.Logger.Sys("打开电源 M4!");
                Utils.Logger.Op("打开电源 M4!");
            }
            else
            {
                Utils.Logger.Sys("关闭电源 M4!");
                Utils.Logger.Op("关闭电源 M4!");
            }
        }

        private void checkBox_ryM5_Click(object sender, EventArgs e)
        {
            _device.ryDeviceM.ryStatusToSet[(int)Device.RelayDevice.Cmd_r.OUT_5] = this.checkBox_ryM5.Checked;
            RySetHandler setRyStatus = new RySetHandler(this._device.WriteRelayDeviceM);
            setRyStatus.BeginInvoke(false, null, null);

            if (checkBox_ryM5.Checked == true)
            {
                Utils.Logger.Sys("打开电源 M5!");
                Utils.Logger.Op("打开电源 M5!");
            }
            else
            {
                Utils.Logger.Sys("关闭电源 M5!");
                Utils.Logger.Op("关闭电源 M5!");
            }
        }

        private void checkBox_ryM6_Click(object sender, EventArgs e)
        {
            _device.ryDeviceM.ryStatusToSet[(int)Device.RelayDevice.Cmd_r.OUT_6] = this.checkBox_ryM6.Checked;
            RySetHandler setRyStatus = new RySetHandler(this._device.WriteRelayDeviceM);
            setRyStatus.BeginInvoke(false, null, null);

            if (checkBox_ryM6.Checked == true)
            {
                Utils.Logger.Sys("打开电源 M6!");
                Utils.Logger.Op("打开电源 M6!");
            }
            else
            {
                Utils.Logger.Sys("关闭电源 M6!");
                Utils.Logger.Op("关闭电源 M6!");
            }
        }

        private void checkBox_ryM7_Click(object sender, EventArgs e)
        {
            _device.ryDeviceM.ryStatusToSet[(int)Device.RelayDevice.Cmd_r.OUT_7] = this.checkBox_ryM7.Checked;
            RySetHandler setRyStatus = new RySetHandler(this._device.WriteRelayDeviceM);
            setRyStatus.BeginInvoke(false, null, null);

            if (checkBox_ryM7.Checked == true)
            {
                Utils.Logger.Sys("打开电源 M7!");
                Utils.Logger.Op("打开电源 M7!");
            }
            else
            {
                Utils.Logger.Sys("关闭电源 M7!");
                Utils.Logger.Op("关闭电源 M7!");
            }
        }

        private void checkBox_ryS0_Click(object sender, EventArgs e)
        {
            if(this.checkBox_ryEn2.Checked == true)
            {
                _device.ryDeviceS.ryStatusToSet[(int)Device.RelayDevice.Cmd_r.OUT_0] = this.checkBox_ryS0.Checked;
                RySetHandler setRyStatus = new RySetHandler(this._device.WriteRelayDeviceS);
                setRyStatus.BeginInvoke(false, null, null);

                if (checkBox_ryS0.Checked == true)
                {
                    Utils.Logger.Sys("打开电源 S0!");
                    Utils.Logger.Op("打开电源 S0!");
                }
                else
                {
                    Utils.Logger.Sys("关闭电源 S0!");
                    Utils.Logger.Op("关闭电源 S0!");
                }
            }
            else
            {
                _device.ryDeviceM.ryStatusToSet[(int)Device.RelayDevice.Cmd_r.OUT_8] = this.checkBox_ryS0.Checked;
                RySetHandler setRyStatus = new RySetHandler(this._device.WriteRelayDeviceM);
                setRyStatus.BeginInvoke(false, null, null);

                if (checkBox_ryS0.Checked == true)
                {
                    Utils.Logger.Sys("打开电源 M8!");
                    Utils.Logger.Op("打开电源 M8!");
                }
                else
                {
                    Utils.Logger.Sys("关闭电源 M8!");
                    Utils.Logger.Op("关闭电源 M8!");
                }
            }
        }

        private void checkBox_ryS1_Click(object sender, EventArgs e)
        {
            if (this.checkBox_ryEn2.Checked == true)
            {
                _device.ryDeviceS.ryStatusToSet[(int)Device.RelayDevice.Cmd_r.OUT_1] = this.checkBox_ryS1.Checked;
                RySetHandler setRyStatus = new RySetHandler(this._device.WriteRelayDeviceS);
                setRyStatus.BeginInvoke(false, null, null);

                if (checkBox_ryS1.Checked == true)
                {
                    Utils.Logger.Sys("打开电源 S1!");
                    Utils.Logger.Op("打开电源 S1!");
                }
                else
                {
                    Utils.Logger.Sys("关闭电源 S1!");
                    Utils.Logger.Op("关闭电源 S1!");
                }
            }
            else
            {
                _device.ryDeviceM.ryStatusToSet[(int)Device.RelayDevice.Cmd_r.OUT_9] = this.checkBox_ryS1.Checked;
                RySetHandler setRyStatus = new RySetHandler(this._device.WriteRelayDeviceM);
                setRyStatus.BeginInvoke(false, null, null);

                if (checkBox_ryS1.Checked == true)
                {
                    Utils.Logger.Sys("打开电源 M9!");
                    Utils.Logger.Op("打开电源 M9!");
                }
                else
                {
                    Utils.Logger.Sys("关闭电源 M9!");
                    Utils.Logger.Op("关闭电源 M9!");
                }
            }
        }

        private void checkBox_ryS2_Click(object sender, EventArgs e)
        {
            if (this.checkBox_ryEn2.Checked == true)
            {
                _device.ryDeviceS.ryStatusToSet[(int)Device.RelayDevice.Cmd_r.OUT_2] = this.checkBox_ryS2.Checked;
                RySetHandler setRyStatus = new RySetHandler(this._device.WriteRelayDeviceS);
                setRyStatus.BeginInvoke(false, null, null);

                if (checkBox_ryS2.Checked == true)
                {
                    Utils.Logger.Sys("打开电源 S2!");
                    Utils.Logger.Op("打开电源 S2!");
                }
                else
                {
                    Utils.Logger.Sys("关闭电源 S2!");
                    Utils.Logger.Op("关闭电源 S2!");
                }
            }
            else
            {
                _device.ryDeviceM.ryStatusToSet[(int)Device.RelayDevice.Cmd_r.OUT_10] = this.checkBox_ryS2.Checked;
                RySetHandler setRyStatus = new RySetHandler(this._device.WriteRelayDeviceM);
                setRyStatus.BeginInvoke(false, null, null);

                if (checkBox_ryS2.Checked == true)
                {
                    Utils.Logger.Sys("打开电源 M10!");
                    Utils.Logger.Op("打开电源 M10!");
                }
                else
                {
                    Utils.Logger.Sys("关闭电源 M10!");
                    Utils.Logger.Op("关闭电源 M10!");
                }
            }
        }

        private void checkBox_ryS3_Click(object sender, EventArgs e)
        {
            if (this.checkBox_ryEn2.Checked == true)
            {
                _device.ryDeviceS.ryStatusToSet[(int)Device.RelayDevice.Cmd_r.OUT_3] = this.checkBox_ryS3.Checked;
                RySetHandler setRyStatus = new RySetHandler(this._device.WriteRelayDeviceS);
                setRyStatus.BeginInvoke(false, null, null);

                if (checkBox_ryS3.Checked == true)
                {
                    Utils.Logger.Sys("打开电源 S3!");
                    Utils.Logger.Op("打开电源 S3!");
                }
                else
                {
                    Utils.Logger.Sys("关闭电源 S3!");
                    Utils.Logger.Op("关闭电源 S3!");
                }
            }
            else
            {
                _device.ryDeviceM.ryStatusToSet[(int)Device.RelayDevice.Cmd_r.OUT_11] = this.checkBox_ryS3.Checked;
                RySetHandler setRyStatus = new RySetHandler(this._device.WriteRelayDeviceM);
                setRyStatus.BeginInvoke(false, null, null);

                if (checkBox_ryS3.Checked == true)
                {
                    Utils.Logger.Sys("打开电源 M11!");
                    Utils.Logger.Op("打开电源 M11!");
                }
                else
                {
                    Utils.Logger.Sys("关闭电源 M11!");
                    Utils.Logger.Op("关闭电源 M11!");
                }
            }
        }

        private void checkBox_ryS4_Click(object sender, EventArgs e)
        {
            if (this.checkBox_ryEn2.Checked == true)
            {
                _device.ryDeviceS.ryStatusToSet[(int)Device.RelayDevice.Cmd_r.OUT_4] = this.checkBox_ryS4.Checked;
                RySetHandler setRyStatus = new RySetHandler(this._device.WriteRelayDeviceS);
                setRyStatus.BeginInvoke(false, null, null);

                if (checkBox_ryS4.Checked == true)
                {
                    Utils.Logger.Sys("打开电源 S4!");
                    Utils.Logger.Op("打开电源 S4!");
                }
                else
                {
                    Utils.Logger.Sys("关闭电源 S4!");
                    Utils.Logger.Op("关闭电源 S4!");
                }
            }
            else
            {
                _device.ryDeviceM.ryStatusToSet[(int)Device.RelayDevice.Cmd_r.OUT_12] = this.checkBox_ryS4.Checked;
                RySetHandler setRyStatus = new RySetHandler(this._device.WriteRelayDeviceM);
                setRyStatus.BeginInvoke(false, null, null);

                if (checkBox_ryS4.Checked == true)
                {
                    Utils.Logger.Sys("打开电源 M12!");
                    Utils.Logger.Op("打开电源 M12!");
                }
                else
                {
                    Utils.Logger.Sys("关闭电源 M12!");
                    Utils.Logger.Op("关闭电源 M12!");
                }
            }
        }

        private void checkBox_ryS5_Click(object sender, EventArgs e)
        {
            if (this.checkBox_ryEn2.Checked == true)
            {
                _device.ryDeviceS.ryStatusToSet[(int)Device.RelayDevice.Cmd_r.OUT_5] = this.checkBox_ryS5.Checked;
                RySetHandler setRyStatus = new RySetHandler(this._device.WriteRelayDeviceS);
                setRyStatus.BeginInvoke(false, null, null);

                if (checkBox_ryS5.Checked == true)
                {
                    Utils.Logger.Sys("打开电源 S5!");
                    Utils.Logger.Op("打开电源 S5!");
                }
                else
                {
                    Utils.Logger.Sys("关闭电源 S5!");
                    Utils.Logger.Op("关闭电源 S5!");
                }
            }
            else
            {
                _device.ryDeviceM.ryStatusToSet[(int)Device.RelayDevice.Cmd_r.OUT_13] = this.checkBox_ryS5.Checked;
                RySetHandler setRyStatus = new RySetHandler(this._device.WriteRelayDeviceM);
                setRyStatus.BeginInvoke(false, null, null);

                if (checkBox_ryS5.Checked == true)
                {
                    Utils.Logger.Sys("打开电源 M13!");
                    Utils.Logger.Op("打开电源 M13!");
                }
                else
                {
                    Utils.Logger.Sys("关闭电源 M13!");
                    Utils.Logger.Op("关闭电源 M13!");
                }
            }
        }

        private void checkBox_ryS6_Click(object sender, EventArgs e)
        {
            if (this.checkBox_ryEn2.Checked == true)
            {
                _device.ryDeviceS.ryStatusToSet[(int)Device.RelayDevice.Cmd_r.OUT_6] = this.checkBox_ryS6.Checked;
                RySetHandler setRyStatus = new RySetHandler(this._device.WriteRelayDeviceS);
                setRyStatus.BeginInvoke(false, null, null);

                if (checkBox_ryS6.Checked == true)
                {
                    Utils.Logger.Sys("打开电源 S6!");
                    Utils.Logger.Op("打开电源 S6!");
                }
                else
                {
                    Utils.Logger.Sys("关闭电源 S6!");
                    Utils.Logger.Op("关闭电源 S6!");
                }
            }
            else
            {
                _device.ryDeviceM.ryStatusToSet[(int)Device.RelayDevice.Cmd_r.OUT_14] = this.checkBox_ryS6.Checked;
                RySetHandler setRyStatus = new RySetHandler(this._device.WriteRelayDeviceM);
                setRyStatus.BeginInvoke(false, null, null);

                if (checkBox_ryS6.Checked == true)
                {
                    Utils.Logger.Sys("打开电源 M14!");
                    Utils.Logger.Op("打开电源 M14!");
                }
                else
                {
                    Utils.Logger.Sys("关闭电源 M14!");
                    Utils.Logger.Op("关闭电源 M14!");
                }
            }
        }

        private void checkBox_ryS7_Click(object sender, EventArgs e)
        {
            if (this.checkBox_ryEn2.Checked == true)
            {
                _device.ryDeviceS.ryStatusToSet[(int)Device.RelayDevice.Cmd_r.OUT_7] = this.checkBox_ryS7.Checked;
                RySetHandler setRyStatus = new RySetHandler(this._device.WriteRelayDeviceS);
                setRyStatus.BeginInvoke(false, null, null);

                if (checkBox_ryS7.Checked == true)
                {
                    Utils.Logger.Sys("打开电源 S7!");
                    Utils.Logger.Op("打开电源 S7!");
                }
                else
                {
                    Utils.Logger.Sys("关闭电源 S7!");
                    Utils.Logger.Op("关闭电源 S7!");
                }
            }
            else
            {
                _device.ryDeviceM.ryStatusToSet[(int)Device.RelayDevice.Cmd_r.OUT_15] = this.checkBox_ryS7.Checked;
                RySetHandler setRyStatus = new RySetHandler(this._device.WriteRelayDeviceM);
                setRyStatus.BeginInvoke(false, null, null);

                if (checkBox_ryS7.Checked == true)
                {
                    Utils.Logger.Sys("打开电源 M15!");
                    Utils.Logger.Op("打开电源 M15!");
                }
                else
                {
                    Utils.Logger.Sys("关闭电源 M15!");
                    Utils.Logger.Op("关闭电源 M15!");
                }
            }
        }

        private void checkBox_paramM_Click(object sender, EventArgs e)
        {
            bool formExist = false;
            foreach (Form fm in Application.OpenForms)
            {
                if (fm.Name == "FormSettingM")
                {
                    // Avoid form being minimized
                    fm.WindowState = FormWindowState.Normal;

                    fm.BringToFront();
                    formExist = true;
                }
            }

            if (!formExist)
            {
                FormSetting fm = new FormSetting(_device.tpDeviceM, _device);
                fm.Name = "FormSettingM";
                fm.Text = "主槽控温参数设置";
                //fm.Location = new System.Drawing.Point(600,300);
                fm.Show();
            }

            Utils.Logger.Sys("打开主槽控温设备参数设置界面!");
            Utils.Logger.Op("打开主槽控温设备参数设置界面!");
        }

        private void checkBox_paramS_Click(object sender, EventArgs e)
        {
            bool formExist = false;
            foreach (Form fm in Application.OpenForms)
            {
                if (fm.Name == "FormSettingS")
                {
                    // Avoid form being minimized
                    fm.WindowState = FormWindowState.Normal;

                    fm.BringToFront();
                    formExist = true;
                }
            }

            if (!formExist)
            {
                FormSetting fm = new FormSetting(_device.tpDeviceS, _device);
                fm.Name = "FormSettingS";
                fm.Text = "辅槽控温参数设置";
                //fm.Location = new System.Drawing.Point(600,300);
                fm.Show();
            }

            Utils.Logger.Sys("打开辅槽控温设备参数设置界面!");
            Utils.Logger.Op("打开辅槽控温设备参数设置界面!");
        }

        private void checkBox_logM_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(System.IO.Directory.GetCurrentDirectory() + "/Logs/OperationLog");
            }
            catch (Exception ex) { }
        }
    }
}