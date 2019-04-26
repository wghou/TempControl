using System;
using System.Windows.Forms;

namespace TempControl
{
    public partial class FormMain
    {
        /// <summary> 设置继电器设备状态 - 委托 </summary>
        private delegate void RySetHandler(bool cntErr);

        private void checkBox_ry0_Click(object sender, EventArgs e)
        {
            _device.ryDevice.ryStatusToSet[(int)Device.RelayDevice.Cmd_r.OUT_0] = this.checkBox_ry0.Checked;
            RySetHandler setRyStatus = new RySetHandler(this._device.WriteRelayDevice);
            setRyStatus.BeginInvoke(false, null, null);

            if (checkBox_ry0.Checked == true)
            {
                Utils.Logger.Sys("打开总电源!");
                Utils.Logger.Op("打开总电源!");
            }
            else
            {
                Utils.Logger.Sys("关闭总电源!");
                Utils.Logger.Op("关闭总电源!");
            }
        }

        private void checkBox_ry1_Click(object sender, EventArgs e)
        {
            _device.ryDevice.ryStatusToSet[(int)Device.RelayDevice.Cmd_r.OUT_1] = this.checkBox_ry1.Checked;
            RySetHandler setRyStatus = new RySetHandler(this._device.WriteRelayDevice);
            setRyStatus.BeginInvoke(false, null, null);

            if (checkBox_ry1.Checked == true)
            {
                Utils.Logger.Sys("打开总电源!");
                Utils.Logger.Op("打开总电源!");
            }
            else
            {
                Utils.Logger.Sys("关闭总电源!");
                Utils.Logger.Op("关闭总电源!");
            }
        }

        private void checkBox_ry2_Click(object sender, EventArgs e)
        {
            _device.ryDevice.ryStatusToSet[(int)Device.RelayDevice.Cmd_r.OUT_2] = this.checkBox_ry2.Checked;
            RySetHandler setRyStatus = new RySetHandler(this._device.WriteRelayDevice);
            setRyStatus.BeginInvoke(false, null, null);

            if (checkBox_ry2.Checked == true)
            {
                Utils.Logger.Sys("打开总电源!");
                Utils.Logger.Op("打开总电源!");
            }
            else
            {
                Utils.Logger.Sys("关闭总电源!");
                Utils.Logger.Op("关闭总电源!");
            }
        }

        private void checkBox_ry3_Click(object sender, EventArgs e)
        {
            _device.ryDevice.ryStatusToSet[(int)Device.RelayDevice.Cmd_r.OUT_3] = this.checkBox_ry3.Checked;
            RySetHandler setRyStatus = new RySetHandler(this._device.WriteRelayDevice);
            setRyStatus.BeginInvoke(false, null, null);

            if (checkBox_ry3.Checked == true)
            {
                Utils.Logger.Sys("打开总电源!");
                Utils.Logger.Op("打开总电源!");
            }
            else
            {
                Utils.Logger.Sys("关闭总电源!");
                Utils.Logger.Op("关闭总电源!");
            }
        }

        private void checkBox_ry4_Click(object sender, EventArgs e)
        {
            _device.ryDevice.ryStatusToSet[(int)Device.RelayDevice.Cmd_r.OUT_4] = this.checkBox_ry4.Checked;
            RySetHandler setRyStatus = new RySetHandler(this._device.WriteRelayDevice);
            setRyStatus.BeginInvoke(false, null, null);

            if (checkBox_ry4.Checked == true)
            {
                Utils.Logger.Sys("打开总电源!");
                Utils.Logger.Op("打开总电源!");
            }
            else
            {
                Utils.Logger.Sys("关闭总电源!");
                Utils.Logger.Op("关闭总电源!");
            }
        }

        private void checkBox_ry5_Click(object sender, EventArgs e)
        {
            _device.ryDevice.ryStatusToSet[(int)Device.RelayDevice.Cmd_r.OUT_5] = this.checkBox_ry5.Checked;
            RySetHandler setRyStatus = new RySetHandler(this._device.WriteRelayDevice);
            setRyStatus.BeginInvoke(false, null, null);

            if (checkBox_ry5.Checked == true)
            {
                Utils.Logger.Sys("打开总电源!");
                Utils.Logger.Op("打开总电源!");
            }
            else
            {
                Utils.Logger.Sys("关闭总电源!");
                Utils.Logger.Op("关闭总电源!");
            }
        }

        private void checkBox_ry6_Click(object sender, EventArgs e)
        {
            _device.ryDevice.ryStatusToSet[(int)Device.RelayDevice.Cmd_r.OUT_6] = this.checkBox_ry6.Checked;
            RySetHandler setRyStatus = new RySetHandler(this._device.WriteRelayDevice);
            setRyStatus.BeginInvoke(false, null, null);

            if (checkBox_ry6.Checked == true)
            {
                Utils.Logger.Sys("打开总电源!");
                Utils.Logger.Op("打开总电源!");
            }
            else
            {
                Utils.Logger.Sys("关闭总电源!");
                Utils.Logger.Op("关闭总电源!");
            }
        }

        private void checkBox_ry7_Click(object sender, EventArgs e)
        {
            _device.ryDevice.ryStatusToSet[(int)Device.RelayDevice.Cmd_r.OUT_7] = this.checkBox_ry7.Checked;
            RySetHandler setRyStatus = new RySetHandler(this._device.WriteRelayDevice);
            setRyStatus.BeginInvoke(false, null, null);

            if (checkBox_ry7.Checked == true)
            {
                Utils.Logger.Sys("打开总电源!");
                Utils.Logger.Op("打开总电源!");
            }
            else
            {
                Utils.Logger.Sys("关闭总电源!");
                Utils.Logger.Op("关闭总电源!");
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