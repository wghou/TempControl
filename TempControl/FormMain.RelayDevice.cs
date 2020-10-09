using System;
using System.Windows.Forms;

namespace TempControl
{
    public partial class FormMain
    {
        /// <summary> 设置继电器设备状态 - 委托 </summary>
        private delegate void RySetHandler(bool cntErr);

        private void checkBox_ryAll_Click(object sender, EventArgs e)
        {
            // 继电器状态 M
            foreach (var itm in dictCheckBoxsRyM)
            {
                _device.ryDeviceM.ryStatusToSet[(int)itm.Key] = itm.Value.Checked;
            }

            // 继电器状态 S
            foreach (var itm in dictCheckBoxsRyS)
            {
                _device.ryDeviceS.ryStatusToSet[(int)itm.Key] = itm.Value.Checked;
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

            if ((sender as CheckBox).Checked == true)
            {
                nlogger.Trace("打开电源: " + (sender as CheckBox).Text);
                Utils.Logger.Op("打开电源: " + (sender as CheckBox).Text);
            }
            else
            {
                nlogger.Trace("关闭电源 M6!");
                Utils.Logger.Op("关闭电源 M6!");
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

            nlogger.Trace("打开主槽控温设备参数设置界面!");
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

            nlogger.Trace("打开辅槽控温设备参数设置界面!");
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

        private void checkBox_data_Click_1(object sender, EventArgs e)
        {
            // 自动取样
            _device.SampleButtonClick();
        }

        private void checkBox_logS_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(System.IO.Directory.GetCurrentDirectory() + "/Logs/Data");
            }
            catch (Exception ex) { }
        }
    }
}