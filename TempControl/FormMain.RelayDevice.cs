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
            foreach(var itm in dictCheckBoxsRyM)
            {
                _device.ryDeviceM.ryStatusToSet[(int)itm.Key] = itm.Value.Checked;
            }

            // 写入 M
            if (dictCheckBoxsRyM.ContainsValue((sender as CheckBox)))
            {
                RySetHandler setRyMStatus = new RySetHandler(this._device.WriteRelayDeviceM);
                setRyMStatus.BeginInvoke(false, null, null);
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


        private void checkBox_ryM4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_ryM4.Checked == true && checkBox_ryM5.Checked == false)
            {
                label_powerM.Text = "加氧气";
                checkBox_ryM5.Enabled = false;

            }
            else if (checkBox_ryM4.Checked == false && checkBox_ryM5.Checked == true)
            {
                label_powerM.Text = "加氮气";
                checkBox_ryM4.Enabled = false;
            }
            else if (checkBox_ryM4.Checked == false && checkBox_ryM5.Checked == false)
            {
                label_powerM.Text = "未加气";
                checkBox_ryM4.Enabled = true;
                checkBox_ryM5.Enabled = true;
            }
            else
            {
                label_powerM.Text = "错误";
                checkBox_ryM4.Enabled = true;
                checkBox_ryM5.Enabled = true;
            }
        }

        private void checkBox_paramM_Click(object sender, EventArgs e)
        {
            // 下一点
            _device.ask4nextPoint();

            nlogger.Info("下一个点!");
            Utils.Logger.Op("下一个点!");
        }

        private void checkBox_logM_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(System.IO.Directory.GetCurrentDirectory() + "/Logs/OperationLog");
            }
            catch (Exception ex) { }
        }


        private void checkBox_data_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(System.IO.Directory.GetCurrentDirectory() + "/Logs/Data");
            }
            catch (Exception ex) { }
        }

        private void checkBox_clear_Click(object sender, EventArgs e)
        {
            lock (_device.tpDeviceM.tpShowLocker)
            {
                _device.tpDeviceM.temperaturesShow.Clear();
            }
            TempPic.Image = mDrawChart.Draw();
        }
    }
}