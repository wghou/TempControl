using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TempControl
{
    public partial class Form1 : Form
    {
        private Device.DeviceStateM _device;
        private Timer _timer;

        public Form1()
        {
            InitializeComponent();

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
