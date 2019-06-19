using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using NLog;

namespace TempControl
{
    public partial class FormAlarm : Form
    {
        private static readonly Logger nlogger = LogManager.GetCurrentClassLogger();

        // 显示时间
        private int errTime = 0;
        Device.DeviceStateM _device;
        public event EventHandler shutdownSystem;

        /// <summary>
        /// 创建警示窗口
        /// </summary>
        /// <param name="msg">警示信息</param>
        /// <param name="time">持续多久后关闭系统</param>
        public FormAlarm(Device.DeviceStateM _dev,int time = 600)
        {
            InitializeComponent();

            this.Name = "FormAlarm";

            this._device = _dev;

            // 默认十分钟后关闭程序
            if (time == 0 || time < 0)
            {
                errTime = 10 * 60;
            }
            else
            {
                errTime = time;
            }

            // 初始化剩余时间
            this.label_errTime.Text = (errTime / 60).ToString("0") + " 分钟 " + (errTime % 60).ToString("0") + "秒后将自动关闭程序！";
            timer1.Tick += Timer1_Tick;
            timer1.Interval = 1000;
            timer1.Start();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            nlogger.Debug("timer for error message and try to shut down PC.");

            // 时间减一
            errTime--;

            string errMsg = string.Empty;
            // 显示警示信息
            foreach (var item in _device.GetErrorStatus())
            {
                switch (item.Key)
                {
                    case Device.ErrorCode.TemptError:
                        if (item.Value != 0)
                            errMsg += "\r\n温控设备通信失败!   " + item.Value.ToString() + " 次";
                        break;

                    case Device.ErrorCode.TempParamSetError:
                        if (item.Value != 0)
                            errMsg += "\r\n温控设备写入参数值失败!   " + item.Value.ToString() + " 次";
                        break;

                    case Device.ErrorCode.RelayError:
                        if (item.Value != 0)
                            errMsg += "\r\n继电器设备写入状态失败!   " + item.Value.ToString() + " 次";
                        break;

                    case Device.ErrorCode.TempNotDown:
                        if (item.Value != 0)
                            errMsg += "\r\n温度不下降警报!   " + item.Value.ToString() + " 次";
                        break;

                    case Device.ErrorCode.TempNotUp:
                        if (item.Value != 0)
                            errMsg += "\r\n温度不上升警报!   " + item.Value.ToString() + " 次";
                        break;

                    case Device.ErrorCode.TempFlucLarge:
                        if (item.Value != 0)
                            errMsg += "\r\n温度波动过大警报!   " + item.Value.ToString() + " 次";
                        break;

                    case Device.ErrorCode.TempBasis:
                        if (item.Value != 0)
                            errMsg += "\r\n温度偏离设定点过大警报!   " + item.Value.ToString() + " 次";
                        break;

                    case Device.ErrorCode.TempOutRange:
                        if (item.Value != 0)
                            errMsg += "\r\n温度超出工作范围报警!   " + item.Value.ToString() + " 次";
                        break;

                    case Device.ErrorCode.SensorError:
                        if (item.Value != 0)
                            errMsg += "\r\n传感器设备读取数值失败!   " + item.Value.ToString() + " 次";
                        break;

                    case Device.ErrorCode.CodeError:
                        if (item.Value != 0)
                            errMsg += "\r\n代码错误!   " + item.Value.ToString() + " 次";
                        break;

                    case Device.ErrorCode.BridgeError:
                        if (item.Value != 0)
                            errMsg += "\r\n测温电桥读数错误!   " + item.Value.ToString() + " 次";
                        break;

                    default:
                        break;
                }
            }
            this.textBox_errMessage.Text = errMsg;
            // 显示剩余时间
            this.label_errTime.Text = (errTime / 60).ToString("0") + " 分钟 " + (errTime % 60).ToString("0") + "秒后将自动关闭程序！";

            // 最后一分钟，始终将窗口置于最前
            if (errTime < 60)
                this.BringToFront();

            // 时间结束则返回请求关闭系统
            if (errTime == 0 || errTime < 0)
            {
                // 通知关闭系统
                shutdownSystem?.Invoke(null, null);
                this.Close();
            }

            nlogger.Debug("formAlarm.timer");
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            _device.ResetErrorStatus();
            nlogger.Info("用户点击关闭了报警窗口");
            this.Close();
        }
    }
}
