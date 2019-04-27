﻿using System;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
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
                float fluc = 0.0f;
                _device.tpDeviceM.GetFlucDurCountOrLess(_device._runningParameters.steadyTimeSec / _device._runningParameters.readTempIntervalSec, out fluc);
                this.label_fluc.Text = "波动度: " + fluc.ToString("0.0000")+ "℃/" + (_device._runningParameters.steadyTimeSec / 60).ToString("0") + "分钟";

                // 功率系数
                label_powerM.Text = this._device.tpDeviceM.tpPowerShow.ToString("0") + "%";
                
                // 温度显示值
                if (this._device.tpDeviceM.temperatures.Count != 0)
                    label_tempM.Text = this._device.tpDeviceM.temperatures.Last().ToString("0.0000") + "℃";

                // 更新温度曲线
                if(this._device.tpDeviceM.temperatures.Count != 0)
                {
                    double tpValue = this._device.tpDeviceM.temperatures.Last();
                    this.cartesianChart.Series[0].Values.Add(new DateModel { DateTime = System.DateTime.Now, Value = tpValue });
                    if (this.cartesianChart.Series[0].Values.Count == 100) this.cartesianChart.Series[0].Values.RemoveAt(0);
                }
                cartesianChart.AxisX[0].MinValue = (DateTime.Now.Ticks - TimeSpan.FromMinutes(10).Ticks) / TimeSpan.FromSeconds(1).Ticks;
                cartesianChart.AxisX[0].MaxValue = (DateTime.Now.Ticks) / TimeSpan.FromSeconds(1).Ticks;
            }));
        }

        private void _device_ErrorStatusChangedEvent(System.Collections.Generic.Dictionary<Device.ErrorCode, uint> errDict)
        {
            // wghou
            //throw new System.NotImplementedException();
            Debug.WriteLine("Error occur.");
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