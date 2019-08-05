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
        Form fmain;         // 主窗体，这里有待进一步修改

        public delegate bool getDataFluc(int count, out float fluctuation);
        private getDataFluc getDataFlucPtr;
        private int steadyTimeSec = 300;
        private int dataIntervalSec = 5;

        DrawChart mDrawChart;

        DateTime startTime;     // 系统启动时间
        List<float> dataShow;   // 绘图所显示的数据
        object dataLocker;      // 源数据访问 - 锁

        int digits = 4;

        public FormChart(ChartConfig cfg, Form fm)
        {
            InitializeComponent();

            fmain = fm;

            startTime = cfg.startTime;
            dataShow = cfg.dataShow;
            dataLocker = cfg.dataLocker;
            this.digits = cfg.digits;
            this.Text = cfg.chartTitle;
            this.getDataFlucPtr = cfg.funcPtr;
            this.dataIntervalSec = cfg.dataIntervalSec;

            mDrawChart = new DrawChart(cfg, TempPic.Height, TempPic.Width);

            timer1.Interval = 200;
            timer1.Tick += Timer1_Tick;
            timer1.Start();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            System.TimeSpan tmSpan = System.DateTime.Now - startTime;
            this.label1.Text = "控温时间： " + tmSpan.Hours.ToString("00") + " : " + tmSpan.Minutes.ToString("00") + " : " + tmSpan.Seconds.ToString("00") + " s";
        }

        private void TemperatureChart_Load(object sender, EventArgs e)
        {
            TempPic.Image = mDrawChart.Draw();

            System.TimeSpan tmSpan = System.DateTime.Now - startTime;
            this.label1.Text = "控温时间： " + tmSpan.Hours.ToString("00") + " h " + tmSpan.Minutes.ToString("00") + " m " + tmSpan.Seconds.ToString("00") + " s";

            (fmain as FormMain).mainFormTimeTickEventHandler += DeviceAll_TpTemperatureUpdateTimerEvent;
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
                getDataFlucPtr?.Invoke(steadyTimeSec / dataIntervalSec, out fluc);
                if(digits == 3) this.label2.Text = "5分钟波动度： " + fluc.ToString("0.000") + " ℃";
                else if(digits == 4) this.label2.Text = "5分钟波动度： " + fluc.ToString("0.0000") + " ℃";
                else this.label2.Text = "5分钟波动度： " + fluc.ToString("0.0000") + " ℃";
            }));
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TemperatureChart_FormClosing(object sender, FormClosingEventArgs e)
        {
            mDrawChart.Dispose();

            (fmain as FormMain).mainFormTimeTickEventHandler -= DeviceAll_TpTemperatureUpdateTimerEvent;
        }

        private void button_Clear_Click(object sender, EventArgs e)
        {
            lock(dataLocker)
            {
                dataShow.Clear();
            }
            TempPic.Image = mDrawChart.Draw();
        }
    }

    public class ChartConfig
    {
        /// <summary>
        /// 曲线名称
        /// </summary>
        public string chartTitle = "";

        /// <summary>
        /// 设备运行的起始时间
        /// </summary>
        public DateTime startTime = DateTime.Now;

        /// <summary>
        /// 所要显示的曲线
        /// </summary>
        public List<float> dataShow;

        /// <summary>
        /// 读取数据时的资源锁
        /// </summary>
        public object dataLocker;

        /// <summary>
        /// 显示数据的精度
        /// </summary>
        public int digits = 4;

        /// <summary>
        /// 数据间隔时长
        /// </summary>
        public int dataIntervalSec = 1;

        /// <summary>
        /// 行数
        /// </summary>
        public int column = 10;

        /// <summary>
        /// 列数
        /// </summary>
        public int row = 7;

        /// <summary>
        /// 获取波动度 - 函数指针
        /// </summary>
        public FormChart.getDataFluc funcPtr;
    }
}
