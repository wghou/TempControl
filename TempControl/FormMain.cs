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
using LiveCharts.Wpf;

namespace TempControl
{
    public partial class FormMain : Form
    {
        private Device.DeviceStateM _device;
        private Dictionary<Device.RelayDevice.Cmd_r, CheckBox> dictCheckBoxs = new Dictionary<Device.RelayDevice.Cmd_r, CheckBox>();
        private Timer _timer;
        

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

            cartesianChart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "水槽温度",
                    Values = new ChartValues<double>{1,2,3,4}
                }
            };
            //cartesianChart.AxisX.Add(new Axis
            //{
            //    Separator = new Separator
            //    {
            //        Step = TimeSpan.FromSeconds(30).Ticks,
            //        IsEnabled = false
            //    }
            //});
            cartesianChart.AxisY.Add(new Axis
            {
                Title = "温度值",
                LabelFormatter = value => value.ToString("F4") + "℃"
            });

            _device = new Device.DeviceStateM();
            _timer = new Timer();
            _timer.Interval = 4000;
            _timer.Tick += _timer_Tick;
            _timer.Start();

            _device.startTimeStep();
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            backgroundWorker1.DoWork += BackgroundWorker1_DoWork;
            timer1.Interval = 200;
            timer1.Tick += Timer1_Tick;
            timer1.Start();

            RegistEventHandler();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            this.backgroundWorker1.RunWorkerAsync();
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            
        }
    }
}
