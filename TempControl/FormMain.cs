using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Brushes = System.Windows.Media.Brushes;
using NLog;
using System.Diagnostics;

namespace TempControl
{
    public partial class FormMain : Form
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private Device.DeviceStateM _device = new Device.DeviceStateM();
        private Dictionary<Device.RelayDevice.Cmd_r, CheckBox> dictCheckBoxsRyM = new Dictionary<Device.RelayDevice.Cmd_r, CheckBox>();
        private Dictionary<Device.RelayDevice.Cmd_r, PictureBox> pictureBoxRyM = new Dictionary<Device.RelayDevice.Cmd_r, PictureBox>();

        bool ErrorAskForClose = false;

        // 闪烁等
        Bitmap mBmpM;
        private bool flp = false;
        private Timer timPic = new Timer();

        Bitmap mBmpRelayRed;
        Bitmap mBmpRelayGreen;

        // 温度曲线
        DrawChart mDrawChart;

        public FormMain()
        {
            InitializeComponent();

            // check box
            dictCheckBoxsRyM[Device.RelayDevice.Cmd_r.OUT_0] = this.checkBox_ryM0;
            dictCheckBoxsRyM[Device.RelayDevice.Cmd_r.OUT_4] = this.checkBox_ryM4;
            dictCheckBoxsRyM[Device.RelayDevice.Cmd_r.OUT_5] = this.checkBox_ryM5;

            // picture box
            pictureBoxRyM.Add(Device.RelayDevice.Cmd_r.OUT_0, pictureBox_ryM0);
            pictureBoxRyM.Add(Device.RelayDevice.Cmd_r.OUT_4, pictureBox_ryM4);
            pictureBoxRyM.Add(Device.RelayDevice.Cmd_r.OUT_5, pictureBox_ryM5);

            // 曲线
            mDrawChart = new DrawChart(_device.tpDeviceM, _device._runningParameters, TempPic.Height, TempPic.Width, 6, 7);

            // 用于继电器的指示灯
            mBmpRelayRed = new Bitmap(this.pictureBox_ryM0.Width, pictureBox_ryM0.Height);
            Graphics mGhpRed = Graphics.FromImage(mBmpRelayRed);
            mGhpRed.Clear(Color.Red);
            mBmpRelayGreen = new Bitmap(this.pictureBox_ryM0.Width, pictureBox_ryM0.Height);
            Graphics mGhpGreen = Graphics.FromImage(mBmpRelayGreen);
            mGhpGreen.Clear(Color.Green);

            // 用于状态指示灯
            mBmpM = new Bitmap(pictureBox1.Width, pictureBox1.Height);
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

            TempPic.Image = mDrawChart.Draw();
           
            _device_RelayDeviceMStatusUpdatedEvent(Device.RelayDevice.Err_r.NoError, _device.ryDeviceM.ryStatus);
        }

        ///////////////////////////////////////////////////
        // 控温板通讯指示灯闪烁
        private void TimPic_Tick(object sender, EventArgs e)
        {
            Graphics mGhp1 = Graphics.FromImage(mBmpM);
            Graphics mGhp2 = Graphics.FromImage(mBmpM);
            mGhp1.Clear(SystemColors.Control);
            if (flp)
            {
                mGhp1.Clear(SystemColors.Control);
                mGhp2.Clear(SystemColors.Control);
                flp = false;
            }
            else
            {
                mGhp1.Clear(this._device.tpDeviceM.currentComStatus ? Color.Green : Color.Red);
                mGhp2.Clear(this._device.tpDeviceS.currentComStatus ? Color.Green : Color.Red);
                flp = true;
            }

            pictureBox1.Image = mBmpM;

            System.TimeSpan tmSpan = System.DateTime.Now - _device.startTime;
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
            this.BeginInvoke(new EventHandler(delegate
            {
                _device.ryDeviceM.Enable = true;
                _device.ryDeviceM.DisconnectProtect = false;

                _device.ryDeviceS.Enable = false;
                _device.tpDeviceS.Enable = false;
            }));

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
                    _device.startTimeStep();
                }));
            }
            else
            {
                _device.startTimeStep();
            }
        }


        ///////////////////////////////////////////////////////////
        // Button Click 事件
        private void checkBox_auto_Click(object sender, EventArgs e)
        {
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
                fm.SetAutoButtonEvent += SetAutoButton;
                fm.Name = "FormAutoSet";
                fm.Show();
            }
        }

        void SetAutoButton(bool st)
        {
            this.BeginInvoke(new EventHandler(delegate
            {
                if (st == true)
                {
                    this.checkBox_auto.Text = "停止";

                    foreach(var chk in dictCheckBoxsRyM)
                    {
                        chk.Value.Enabled = false;
                    }
                }
                else
                {
                    this.checkBox_auto.Text = "自动";

                    foreach (var chk in dictCheckBoxsRyM)
                    {
                        chk.Value.Enabled = true;
                    }

                }
            }));

        }

        private void checkBox_exit_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("是否退出程序？", "程序关闭确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) 
                != DialogResult.Yes)
            {
                return;
            }

            // 关闭所有继电器
            _device.ryDeviceM.ryStatusToSet = new bool[16];
            _device.ryDeviceM.closeDevice();
            _device.ryDeviceS.ryStatusToSet = new bool[16];
            _device.ryDeviceS.closeDevice();
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


        private void checkBox_data_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(System.IO.Directory.GetCurrentDirectory() + "/Logs/Data");
            }
            catch (Exception ex) { }
        }

        private void checkBox_clear_Click(object sender, EventArgs e)
        {
            lock (_device.tpDeviceM.tpShowLocker)
            {
                _device.tpDeviceM.temperaturesShow.Clear();
            }
            TempPic.Image = mDrawChart.Draw();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ErrorAskForClose)
            {
                if (DialogResult.No == MessageBox.Show("您确定要退出程序吗？", "程序关闭确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    e.Cancel = true;
                    return;
                }
            }

            mDrawChart.Dispose();

            _device.ryDeviceM.closeDevice();
            _device.ryDeviceS.closeDevice(); 
        }
    }
}
