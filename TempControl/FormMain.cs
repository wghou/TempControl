using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using Brushes = System.Windows.Media.Brushes;

namespace TempControl
{
    public partial class FormMain : Form
    {
        private Device.DeviceStateM _device = new Device.DeviceStateM();
        private Dictionary<Device.RelayDevice.Cmd_r, CheckBox> dictCheckBoxs = new Dictionary<Device.RelayDevice.Cmd_r, CheckBox>();

        // 曲线
        DrawChart mDrawChart;

        // 闪烁等
        Bitmap mBmp;
        private bool flp = false;
        private Timer timPic = new Timer();

        Bitmap mBmpRelayRed;
        Bitmap mBmpRelayGreen;

        public class DateModel
        {
            public System.DateTime DateTime { get; set; }
            public double Value { get; set; }
        }


        public FormMain()
        {
            InitializeComponent();

            dictCheckBoxs[Device.RelayDevice.Cmd_r.OUT_0] = this.checkBox_ry0;
            dictCheckBoxs[Device.RelayDevice.Cmd_r.OUT_1] = this.checkBox_ry1;
            dictCheckBoxs[Device.RelayDevice.Cmd_r.OUT_2] = this.checkBox_ry2;
            dictCheckBoxs[Device.RelayDevice.Cmd_r.OUT_3] = this.checkBox_ry3;
            dictCheckBoxs[Device.RelayDevice.Cmd_r.OUT_4] = this.checkBox_ry4;
            dictCheckBoxs[Device.RelayDevice.Cmd_r.OUT_5] = this.checkBox_ry5;
            dictCheckBoxs[Device.RelayDevice.Cmd_r.OUT_6] = this.checkBox_ry6;
            dictCheckBoxs[Device.RelayDevice.Cmd_r.OUT_7] = this.checkBox_ry7;

            // 曲线
            mDrawChart = new DrawChart(_device.tpDeviceM, _device._runningParameters, tempPic.Height, tempPic.Width, 6, 7);
            tempPic.Image = mDrawChart.Draw();

            // 用于状态指示灯
            mBmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            timPic.Interval = 500;
            timPic.Tick += TimPic_Tick;
            timPic.Start();
        }


        private void Form1_Shown(object sender, EventArgs e)
        {
            backgroundWorker1.DoWork += BackgroundWorker1_DoWork;
            timer1.Interval = 200;
            timer1.Tick += Timer1_Tick;
            timer1.Start();

            RegistEventHandler();
        }

        ///////////////////////////////////////////////////
        // 控温板通讯指示灯闪烁
        private void TimPic_Tick(object sender, EventArgs e)
        {
            Graphics mGhp = Graphics.FromImage(mBmp);
            mGhp.Clear(SystemColors.Control);
            if (flp)
            {
                mGhp.Clear(SystemColors.Control);
                flp = false;
            }
            else
            {
                mGhp.Clear(this._device.tpDeviceM.currentComStatus ? Color.Green : Color.Red);
                flp = true;
            }

            pictureBox1.Image = mBmp;

            TimeSpan tmSpan = DateTime.Now - _device.startTime;
            this.label_time.Text = "控温时间： " + tmSpan.Hours.ToString("00") + " h " + tmSpan.Minutes.ToString("00") + " m " + tmSpan.Seconds.ToString("00") + " s";
        }


        ////////////////////////////////////////////////////
        // 界面启动后，延迟执行的操作，比如自检
        private void Timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            this.backgroundWorker1.RunWorkerAsync();
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            bool confDevice = _device.Configure();
            if (confDevice == false)
            {
                this.BeginInvoke(new EventHandler(delegate
                {
                    if (DialogResult.Yes == MessageBox.Show("设备端口错误，是否退出程序？", "程序关闭确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    {
                        this.Close();
                        return;
                    }
                }));
                
            }
            _device.startTimeStep();
        }


        ///////////////////////////////////////////////////////////
        // Button Click 事件
        private void checkBox_auto_Click(object sender, EventArgs e)
        {
            return;

            bool fmExit = false;
            foreach (Form fm in Application.OpenForms)
            {
                if (fm.Name == "FormAutoSet")
                {
                    fm.WindowState = FormWindowState.Normal;
                    fm.BringToFront();
                    fmExit = true;
                }
            }

            if (!fmExit)
            {
                FormAutoSet fm = new FormAutoSet(_device);
                //fm.SetAutoButtonEvent += SetAutoButton;
                fm.Name = "FormAutoSet";
                fm.Show();
            }
        }

        private void checkBox_exit_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("是否退出程序？", "程序关闭确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) 
                != DialogResult.Yes)
            {
                return;
            }

            // 关闭所有继电器
            for(int i = 0; i < 8; i++)
            {
                _device.ryDevice.ryStatusToSet[i] = false;
            }
            // bug
            // 如果在退出程序的时候，继电器读写错误，则无法处理该异常
            //_device.RelayDeviceStatusUpdatedEvent -= _device_RelayDeviceStatusUpdatedEvent;
            _device.WriteRelayDevice(false);
            this.Close();
        }

        private void checkBox_debug_Click(object sender, EventArgs e)
        {
            bool formExist = false;
            foreach (Form fm in Application.OpenForms)
            {
                if (fm.Name == "FormDebug")
                {
                    // Avoid form being minimized
                    fm.WindowState = FormWindowState.Normal;

                    fm.BringToFront();
                    formExist = true;
                }
            }

            if (!formExist)
            {
                FormDebug fm = new FormDebug(_device._runningParameters);
                fm.Name = "FormDebug";
                fm.Text = "调试参数设置";
                //fm.Location = new System.Drawing.Point(600,300);
                fm.Show();
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            mDrawChart.Dispose();
        }

        private void checkBox_clc_Click(object sender, EventArgs e)
        {
            lock (_device.tpDeviceM.tpShowLocker)
            {
                _device.tpDeviceM.temperaturesShow.Clear();
            }
            tempPic.Image = mDrawChart.Draw();
        }

        ////////////////////////////////////////////////////
        //// 初始化曲线图
        //private void InitLiveCharts()
        //{
        //    var dayConfig = Mappers.Xy<DateModel>()
        //        .X(dateModel => dateModel.DateTime.Ticks / TimeSpan.FromSeconds(1).Ticks)
        //        .Y(dateModel => dateModel.Value);

        //    cartesianChart.BackColor = Color.AliceBlue;

        //    cartesianChart.Series = new SeriesCollection(dayConfig)
        //    {
        //        new LineSeries
        //        {
        //            Title = "水槽温度",
        //            Values = new ChartValues<DateModel>{ },
        //            LineSmoothness = 0, // 0 straight lines, 1 really smooth lines
        //            //PointGeometry = null,
        //            //PointGeometrySize = 0,
        //            Fill = Brushes.Transparent,
        //        }
        //    };
        //    cartesianChart.AxisX.Add(new Axis
        //    {
        //        MinValue = (DateTime.Now.Ticks - TimeSpan.FromMinutes(20).Ticks) / TimeSpan.FromSeconds(1).Ticks,
        //        MaxValue = (DateTime.Now.Ticks) / TimeSpan.FromSeconds(1).Ticks,
        //        LabelFormatter = value => new DateTime((long)(value * TimeSpan.FromSeconds(1).Ticks)).ToString("t"),
        //        FontSize = 16,
        //        Foreground = System.Windows.Media.Brushes.Black,
        //    });

        //    cartesianChart.AxisY.Add(new Axis
        //    {
        //        IsMerged = true,
        //        Separator = new Separator
        //        {
        //            StrokeThickness = 1,
        //            StrokeDashArray = new System.Windows.Media.DoubleCollection(new double[] { 3 }),
        //            Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(64, 79, 86))
        //        },
        //        Title = "温度值",
        //        Position = AxisPosition.LeftBottom,
        //        LabelFormatter = value => value.ToString("F2") + "℃",
        //        FontSize = 15,
        //        Foreground = System.Windows.Media.Brushes.Black,
        //    });
        //}
    }
}
