using System;
using System.Windows.Forms;

namespace TempControl
{
    public partial class FormMain
    {
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