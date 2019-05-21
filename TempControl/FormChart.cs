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
        DrawChart mDrawChart;
        Device.TempDevice tpDevice;
        Device.DeviceStateM deviceAll;

        public FormChart(Device.DeviceStateM devAll, Device.TempDevice dev)
        {
            InitializeComponent();
            tpDevice = dev;
            deviceAll = devAll;
            mDrawChart = new DrawChart(dev,devAll._runningParameters,TempPic.Height, TempPic.Width, 10, 7);

            timer1.Interval = 200;
            timer1.Tick += Timer1_Tick;
            timer1.Start();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            System.TimeSpan tmSpan = System.DateTime.Now - deviceAll.startTime;
            this.label1.Text = "控温时间： " + tmSpan.Hours.ToString("00") + " : " + tmSpan.Minutes.ToString("00") + " : " + tmSpan.Seconds.ToString("00") + " s";
        }

        private void TemperatureChart_Load(object sender, EventArgs e)
        {
            TempPic.Image = mDrawChart.Draw();
            deviceAll.TimerTickEvent += DeviceAll_TpTemperatureUpdateTimerEvent;

            System.TimeSpan tmSpan = System.DateTime.Now - deviceAll.startTime;
            this.label1.Text = "控温时间： " + tmSpan.Hours.ToString("00") + " h " + tmSpan.Minutes.ToString("00") + " m " + tmSpan.Seconds.ToString("00") + " s";
        }

        private void DeviceAll_TpTemperatureUpdateTimerEvent()
        {
            // 只要是定时器函数执行了，不管有没有正确的从下位机读取到参数，都会重新绘制
            // 也就是说，不处理错误
            this.BeginInvoke(new EventHandler(delegate
            {
                TempPic.Image = mDrawChart.Draw();

                // 波动度显示
                float fluc = 0.0f;
                if(this.Name == "FormChartM")
                {
                    deviceAll.tpDeviceM.GetFlucDurCountOrLess(deviceAll._runningParameters.steadyTimeSec / deviceAll._runningParameters.readTempIntervalSec, out fluc);
                    this.label2.Text = "5分钟波动度： " + fluc.ToString("0.0000") + " ℃";
                }
                else
                {
                    deviceAll.tpDeviceS.GetFlucDurCountOrLess(deviceAll._runningParameters.steadyTimeSec / deviceAll._runningParameters.readTempIntervalSec, out fluc);
                    this.label2.Text = "5分钟波动度： " + fluc.ToString("0.000") + " ℃";
                }  
            }));
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TemperatureChart_FormClosing(object sender, FormClosingEventArgs e)
        {
            mDrawChart.Dispose();
            deviceAll.TimerTickEvent -= DeviceAll_TpTemperatureUpdateTimerEvent;
        }

        private void button_Clear_Click(object sender, EventArgs e)
        {
            lock(tpDevice.tpShowLocker)
            {
                tpDevice.temperaturesShow.Clear();
            }
            TempPic.Image = mDrawChart.Draw();
        }
    }
}
