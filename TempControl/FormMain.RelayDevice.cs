using System;
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

        private void checkBox_remoteMonitor_CheckedChanged(object sender, EventArgs e)
        {
            _device.setMqttEnable(this.checkBox_remoteMonitor.Checked);

            pictureBox_lot.Enabled = this.checkBox_remoteMonitor.Checked;
        }

        /// <summary> 设置继电器设备状态 - 委托 </summary>
        private delegate void RySetHandler(bool cntErr);

        private void checkBox_ryAll_Click(object sender, EventArgs e)
        {
            // 继电器状态 M
            foreach(var itm in dictCheckBoxsRyM)
            {
                _device.ryDeviceM.ryStatusToSet[(int)itm.Key] = itm.Value.Checked;
            }

            // 继电器状态 S
            foreach (var itm in dictCheckBoxsRyS)
            {
                if(this.checkBox_ryEn2.Checked == true) _device.ryDeviceS.ryStatusToSet[(int)itm.Key] = itm.Value.Checked;
                else _device.ryDeviceM.ryStatusToSet[(int)itm.Key + 8] = itm.Value.Checked;
            }

            // 写入 M
            if (dictCheckBoxsRyM.ContainsValue((sender as CheckBox)))
            {
                RySetHandler setRyMStatus = new RySetHandler(this._device.WriteRelayDeviceM);
                setRyMStatus.BeginInvoke(false, null, null);
            }

            // 写入 S
            if (dictCheckBoxsRyS.ContainsValue((sender as CheckBox)))
            {
                RySetHandler setRySStatus = new RySetHandler(this._device.WriteRelayDeviceS);
                setRySStatus.BeginInvoke(false, null, null);
            }

            if((sender as CheckBox).Checked == true)
            {
                nlogger.Info("打开电源: " + (sender as CheckBox).Text);
                Utils.Logger.Op("打开电源: " + (sender as CheckBox).Text);
            }
            else
            {
                nlogger.Info("关闭电源: " + (sender as CheckBox).Text);
                Utils.Logger.Op("关闭电源: " + (sender as CheckBox).Text);
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

            nlogger.Info("打开主槽控温设备参数设置界面!");
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

            nlogger.Info("打开辅槽控温设备参数设置界面!");
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