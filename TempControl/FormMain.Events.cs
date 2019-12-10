using System;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace TempControl
{
    public partial class FormMain
    {
        void RegistEventHandler()
        {
            _device.RelayDeviceMStatusUpdatedEvent += _device_RelayDeviceMStatusUpdatedEvent;
            _device.RelayDeviceSStatusUpdatedEvent += _device_RelayDeviceSStatusUpdatedEvent;
            _device.TempDeviceMParamUpdatedEvent += _device_TempDeviceMParamUpdatedEvent;
            _device.TempDeviceSParamUpdatedEvent += _device_TempDeviceSParamUpdatedEvent;
            _device.StateChangedEvent += _device_StateChangedEvent;
            _device.ErrorStatusChangedEvent += _device_ErrorStatusChangedEvent;
            _device.TimerTickEvent += _device_TimerTickEvent;
        }

        public delegate void mainFormTimeTickEvent();
        /// <summary>
        /// 主界面定时器事件
        /// </summary>
        public event mainFormTimeTickEvent mainFormTimeTickEventHandler;

        // 定时器更新事件
        private void _device_TimerTickEvent()
        {
            this.BeginInvoke(new EventHandler(delegate
            {
                // 波动度
                float fluc = 0.0f;
                _device.tpDeviceM.GetFlucDurCountOrLess(_device._runningParameters.steadyTimeSec / _device._runningParameters.readTempIntervalSec, out fluc);
                this.label_fluc.Text = "波动度: " + fluc.ToString("0.000") + "%/" + (_device._runningParameters.steadyTimeSec / 60).ToString("0") + "分钟";

                // 主槽功率系数
                //label_powerM.Text = this._device.tpDeviceM.tpPowerShow.ToString("0") + "%";

                // 主槽温度显示值
                if (this._device.tpDeviceM.temperatures.Count != 0)
                    label_tempM.Text = this._device.tpDeviceM.temperatures.Last().ToString("0.000") + "%";


                label_tempValue.Text = this._device.tpDeviceM.tpParam[2].ToString("0.000") + "℃";

                // 主槽温度设定值
                //label_tempSetM.Text = this._device.tpDeviceM.tpParam[0].ToString("0.0000") + "℃";

                TempPic.Image = mDrawChart.Draw();
            }));

            // 执行主界面定时器事件 handler
            mainFormTimeTickEventHandler?.Invoke();
        }

        private void _device_ErrorStatusChangedEvent(System.Collections.Generic.Dictionary<Device.ErrorCode, uint> errDict)
        {
            // 警告信息及处理时间
            int errTm = 600;
            this.BeginInvoke(new EventHandler(delegate
            {
                FormAlarm fmA = null;
                bool formExist = false;
                foreach (Form fm in Application.OpenForms)
                {
                    if (fm.Name == "FormAlarm")
                    {
                        fmA = (FormAlarm)fm;
                        formExist = true;
                    }
                }

                if (!formExist)
                {
                    fmA = new FormAlarm(_device, errTm);
                    fmA.Name = "FormAlarm";
                    fmA.shutdownSystem += FmA_shutdownSystem;
                }

                // 只是确保一下，应该没什么作用
                if (fmA == null)
                    return;

                // wghou
                // 出现错误时，如果是首次出现，则新建 Alarm 窗口并弹出，但如果窗口已经存在，则只闪烁任务栏提示
                if (formExist)
                {
                    
                }
                else
                {
                    fmA.Show();
                }

            }));
            Debug.WriteLine("Error occur.");
        }


        private void FmA_shutdownSystem(object sender, EventArgs e)
        {
            this.BeginInvoke(new EventHandler(delegate
            {
                this.ErrorAskForClose = true;
                nlogger.Error("出现错误，用户未做处理，关闭系统软件！");

                _device.ShutdownComputer();

                this.Close();
            }));
        }

        private void _device_StateChangedEvent(Device.State st)
        {
            this.BeginInvoke(new EventHandler(delegate
            {
                switch (st)
                {
                    case Device.State.Start:
                        // 系统流程
                        this.label_controlState.Text = "开始自动控制";
                        break;
                    case Device.State.AddOxygen:
                        // 系统流程
                        this.label_controlState.Text = "加氧气";
                        break;
                    case Device.State.AddNitrogen:
                        // 系统流程
                        this.label_controlState.Text = "加氮气";
                        break;
                    case Device.State.Stand:
                        // 系统流程
                        this.label_controlState.Text = "静置";
                        break;
                    case Device.State.Steady:
                        // 稳定
                        this.label_controlState.Text = "稳定";
                        break;
                    case Device.State.Measure:
                        // 系统流程
                        this.label_controlState.Text = "测量";
                        break;
                    case Device.State.ShutdownPC:
                        // 系统流程
                        this.label_controlState.Text = "系统停止";
                        break;
                    case Device.State.Idle:
                        // 系统流程
                        this.label_controlState.Text = "空闲";
                        break;
                }

                // 自动流程中，禁止继电器按键
                if(st == Device.State.Idle || st == Device.State.ShutdownPC)
                {
                    foreach (var itm in dictCheckBoxsRyM) itm.Value.Enabled = true;
                    foreach (var itm in dictCheckBoxsRyS) itm.Value.Enabled = true;
                }
                else
                {
                    foreach (var itm in dictCheckBoxsRyM) itm.Value.Enabled = false;
                    foreach (var itm in dictCheckBoxsRyS) itm.Value.Enabled = false;

                    this.label_tempSetM.Text = _device.currentTemptPointState.stateTemp.ToString("0.000");
                }
            }));
        }

        private void _device_TempDeviceMParamUpdatedEvent(Device.TempProtocol.Err_t err, float[] param)
        {
            this.BeginInvoke(new EventHandler(delegate
            {
                // 温度设定值
                label_tempSetM.Text = param[0].ToString("0.0000") + "℃";
            }));
        }

        private void _device_TempDeviceSParamUpdatedEvent(Device.TempProtocol.Err_t err, float[] param)
        {
        }

        private void _device_RelayDeviceMStatusUpdatedEvent(Device.RelayDevice.Err_r err, bool[] ryStatus)
        {
            this.BeginInvoke(new EventHandler(delegate
            {
                // 按钮状态
                foreach (var chk in this.dictCheckBoxsRyM) chk.Value.Checked = ryStatus[(int)chk.Key];
                // 指示灯状态
                foreach (var pic in this.pictureBoxRyM) pictureBoxRyM[pic.Key].Image = ryStatus[(int)pic.Key] ? mBmpRelayGreen : mBmpRelayRed;
            }));

            if(err != Device.RelayDevice.Err_r.NoError)
            {
                MessageBox.Show("继电器模块 1 设置错误！");
            }
        }

        private void _device_RelayDeviceSStatusUpdatedEvent(Device.RelayDevice.Err_r err, bool[] ryStatus)
        {
            this.BeginInvoke(new EventHandler(delegate
            {
                // 按钮状态
                foreach (var chk in this.dictCheckBoxsRyS) chk.Value.Checked = ryStatus[(int)chk.Key];
                // 指示灯状态
                foreach (var pic in this.pictureBoxRyS) pictureBoxRyS[pic.Key].Image = ryStatus[(int)pic.Key] ? mBmpRelayGreen : mBmpRelayRed;
            }));

            if (err != Device.RelayDevice.Err_r.NoError)
            {
                MessageBox.Show("继电器模块 2 设置错误！");
            }
        }
    }
}