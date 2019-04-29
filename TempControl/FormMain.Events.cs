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

                // 温度设定值
                label_tempSetM.Text = this._device.tpDeviceM.tpParam[0].ToString("0.0000") + "℃";

                // 更新温度曲线
                if(this._device.tpDeviceM.temperatures.Count != 0)
                {
                    double tpValue = this._device.tpDeviceM.temperatures.Last();
                    this.cartesianChart.Series[0].Values.Add(new DateModel { DateTime = System.DateTime.Now, Value = tpValue });
                    if (this.cartesianChart.Series[0].Values.Count == 300) this.cartesianChart.Series[0].Values.RemoveAt(0);
                }
                cartesianChart.AxisX[0].MinValue = (DateTime.Now.Ticks - TimeSpan.FromMinutes(20).Ticks) / TimeSpan.FromSeconds(1).Ticks;
                cartesianChart.AxisX[0].MaxValue = (DateTime.Now.Ticks) / TimeSpan.FromSeconds(1).Ticks;
            }));
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
                    //fmA.Location = new System.Drawing.Point(600, 300);
                    //fmA.shutdownSystem += FmA_shutdownSystem;
                }

                // 只是确保一下，应该没什么作用
                if (fmA == null)
                    return;

                // wghou
                // 出现错误时，如果是首次出现，则新建 Alarm 窗口并弹出，但如果窗口已经存在，则只闪烁任务栏提示
                if (formExist)
                {
                    IntPtr hwnd = FindWindow("FormAlarm", null);
                    if (hwnd != null)
                        FlashWindowEx(hwnd, flashType.FLASHW_TIMER);
                }
                else
                {
                    fmA.Show();
                }

            }));
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

        #region 实现窗口闪烁

        [DllImport("user32.dll")]
        public static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        /// <summary>
        /// 闪烁类型
        /// </summary>
        public enum flashType : uint
        {
            FLASHW_STOP = 0,    //停止闪烁
            FALSHW_CAPTION = 1, //只闪烁标题
            FLASHW_TRAY = 2,    //只闪烁任务栏
            FLASHW_ALL = 3,     //标题和任务栏同时闪烁
            FLASHW_PARAM1 = 4,
            FLASHW_PARAM2 = 12,
            FLASHW_TIMER = FLASHW_TRAY | FLASHW_PARAM1,     //无条件闪烁任务栏直到发送停止标志或者窗口被激活，如果未激活，停止时高亮
            FLASHW_TIMERNOFG = FLASHW_TRAY | FLASHW_PARAM2  //未激活时闪烁任务栏直到发送停止标志或者窗体被激活，停止后高亮

            // https://msdn.microsoft.com/en-us/library/ms679348
            //FLASHW_ALL = 0x00000003,        // Flash both the window caption and taskbar button. This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags.
            //FLASHW_CAPTION = 0x00000001,    // Flash the window caption.
            //FLASHW_STOP = 0,                // Stop flashing. The system restores the window to its original state.
            //FLASHW_TIMER = 0x00000004,      // Flash continuously, until the FLASHW_STOP flag is set.
            //FLASHW_TIMERNOFG = 0x0000000C,  // Flash continuously until the window comes to the foreground.
            //FLASHW_TRAY =0x00000002,        // Flash the taskbar button.
        }

        /// <summary>
        /// 包含系统应在指定时间内闪烁窗口次数和闪烁状态的信息
        /// </summary>
        public struct FLASHWINFO
        {
            /// <summary>
            /// 结构大小
            /// </summary>
            public uint cbSize;
            /// <summary>
            /// 要闪烁或停止的窗口句柄
            /// </summary>
            public IntPtr hwnd;
            /// <summary>
            /// 闪烁的类型
            /// </summary>
            public uint dwFlags;
            /// <summary>
            /// 闪烁窗口的次数
            /// </summary>
            public uint uCount;
            /// <summary>
            /// 窗口闪烁的频度，毫秒为单位；若该值为0，则为默认图标的闪烁频度
            /// </summary>
            public uint dwTimeout;
        }

        /// <summary>
        /// 闪烁窗口
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="type">闪烁类型</param>
        /// <returns></returns>
        public static bool FlashWindowEx(IntPtr hWnd, flashType type)
        {
            FLASHWINFO fInfo = new FLASHWINFO();
            fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
            fInfo.hwnd = hWnd;//要闪烁的窗口的句柄，该窗口可以是打开的或最小化的
            fInfo.dwFlags = (uint)type;//闪烁的类型
            //fInfo.uCount = UInt32.MaxValue;//闪烁窗口的次数
            fInfo.uCount = 5;//闪烁窗口的次数
            fInfo.dwTimeout = 0; //窗口闪烁的频度，毫秒为单位；若该值为0，则为默认图标的闪烁频度
            return FlashWindowEx(ref fInfo);
        }

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);
        #endregion
    }
}