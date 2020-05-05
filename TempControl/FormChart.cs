using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TempControl
{
    public partial class FormChart : Form
    {
        Device.DeviceStateM _device;
        private ChartDeviceType _deviceType;
        private List<float> dataCache = new List<float>();

        public FormChart(Device.DeviceStateM dev, ChartDeviceType type)
        {
            InitializeComponent();

            _device = dev;
            this._deviceType = type;

            hslCurve1.SetLeftCurve("A", null, Color.LimeGreen);

            timer1.Interval = 200;
            timer1.Tick += Timer1_Tick;
            timer1.Start();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            System.TimeSpan tmSpan = System.DateTime.Now - _device.startTime;
            this.label1.Text = "控温时间： " + tmSpan.Hours.ToString("00") + " : " + tmSpan.Minutes.ToString("00") + " : " + tmSpan.Seconds.ToString("00") + " s";
        }

        private void TemperatureChart_Load(object sender, EventArgs e)
        {
            System.TimeSpan tmSpan = System.DateTime.Now - _device.startTime;
            this.label1.Text = "控温时间： " + tmSpan.Hours.ToString("00") + " h " + tmSpan.Minutes.ToString("00") + " m " + tmSpan.Seconds.ToString("00") + " s";

            if(this._deviceType == ChartDeviceType.MainDevice)
            {
                this.Text = "主槽温度曲线";
                _device.TempDeviceMReadTemptEvent += _device_TempDeviceReadTemptEvent;
            }
            else
            {
                this.Text = "辅槽温度曲线";
                _device.TempDeviceSReadTemptEvent += _device_TempDeviceReadTemptEvent;
            }
        }

        private void _device_TempDeviceReadTemptEvent(float tempt, float fluc, bool isError)
        {
            // 如果发生了错误，则暂时不更新曲线
            if (isError == true) return;

            // 用于计算曲线图的上下界限
            dataCache.Add(tempt);
            if (dataCache.Count >= hslCurve1.StrechDataCountMax) dataCache.RemoveAt(0);

            double ValCent, valPhase;

            if(_deviceType == ChartDeviceType.MainDevice)
            {
                ValCent = Math.Round((dataCache.Max() + dataCache.Min()) / 2, 4);
                valPhase = Math.Round((dataCache.Max() - dataCache.Min()) / 6, 4) + 0.001;
            }
            else
            {
                ValCent = Math.Round((dataCache.Max() + dataCache.Min()) / 2, 3);
                valPhase = Math.Round((dataCache.Max() - dataCache.Min()) / 6, 3) + 0.001;
            }
            

            // 只要是定时器函数执行了，不管有没有正确的从下位机读取到参数，都会重新绘制
            // 也就是说，不处理错误
            this.BeginInvoke(new EventHandler(delegate
            {
                hslCurve1.ValueMaxLeft = (float)(ValCent + valPhase * 3);
                hslCurve1.ValueMinLeft = (float)(ValCent - valPhase * 3);

                hslCurve1.AddCurveData("A", tempt);

                if (_deviceType == ChartDeviceType.MainDevice) this.label2.Text = "5分钟波动度： " + fluc.ToString("0.0000") + " ℃";
                else if (_deviceType == ChartDeviceType.SubDevice) this.label2.Text = "5分钟波动度： " + fluc.ToString("0.000") + " ℃";
            }));
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TemperatureChart_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this._deviceType == ChartDeviceType.MainDevice)
            {
                _device.TempDeviceMReadTemptEvent -= _device_TempDeviceReadTemptEvent;
            }
            else
            {
                _device.TempDeviceSReadTemptEvent -= _device_TempDeviceReadTemptEvent;
            }
        }

        private void button_Clear_Click(object sender, EventArgs e)
        {
            dataCache.Clear();
            hslCurve1.ValueMaxLeft = 50.0f;
            hslCurve1.ValueMinLeft = -10.0f;
            hslCurve1.RemoveAllCurveData();
        }

        /// <summary>
        /// 曲线显示的是哪一组设备 - 主槽温度 or 辅槽温度
        /// </summary>
        public enum ChartDeviceType : int
        {
            /// <summary> 主槽的数据曲线 </summary>
            MainDevice = 0,
            /// <summary> 辅槽的数据曲线 </summary>
            SubDevice
        }   
    }
}
