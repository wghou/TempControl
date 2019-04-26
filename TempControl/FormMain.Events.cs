using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace TempControl
{
    public partial class FormMain
    {
        void RegistEventHandler()
        {
            _device.RelayDeviceStatusUpdatedEvent += _device_RelayDeviceStatusUpdatedEvent;
            _device.TempDeviceParamUpdatedEvent += _device_TempDeviceParamUpdatedEvent;
            _device.StateChangedEvent += _device_StateChangedEvent;
            _device.ErrorStatusChangedEvent += _device_ErrorStatusChangedEvent;
            _device.TimerTickEvent += _device_TimerTickEvent;
        }

        // 定时器更新事件
        private void _device_TimerTickEvent()
        {
            this.BeginInvoke(new EventHandler(delegate
            {
                // 波动度
                this.label_fluc.Text = "主控温槽波动度: " + "fluc";

                // 功率系数
                label_powerM.Text = this._device.tpDeviceM.tpPowerShow.ToString("0") + "%";
                
                // 温度显示值
                if (this._device.tpDeviceM.temperatures.Count != 0)
                    label_tempM.Text = this._device.tpDeviceM.temperatures.Last().ToString("0.0000") + "℃";
            }));
        }

        private void _device_ErrorStatusChangedEvent(System.Collections.Generic.Dictionary<Device.ErrorCode, uint> errDict)
        {
            throw new System.NotImplementedException();
        }

        private void _device_StateChangedEvent(Device.State st)
        {
            this.BeginInvoke(new EventHandler(delegate
            {
                // 系统流程
                this.label_controlState.Text = "系统启动";
            }));
        }

        private void _device_TempDeviceParamUpdatedEvent(Device.TempProtocol.Err_t err, float[] param)
        {
            this.BeginInvoke(new EventHandler(delegate
            {
                // 温度设定值
                label_tempSetM.Text = param[0].ToString("0.0000") + "℃";
            }));
        }

        private void _device_RelayDeviceStatusUpdatedEvent(Device.RelayDevice.Err_r err, bool[] ryStatus)
        {
            this.BeginInvoke(new EventHandler(delegate
            {
                // 按钮状态
                foreach(var cmd in Enum.GetValues(typeof(Device.RelayDevice.Cmd_r)))
                {
                    this.dictCheckBoxs[(Device.RelayDevice.Cmd_r)cmd].Checked = ryStatus[(int)cmd];
                }
            }));

            if(err != Device.RelayDevice.Err_r.NoError)
            {
                MessageBox.Show("继电器设置错误！");
            }
        }
    }
}