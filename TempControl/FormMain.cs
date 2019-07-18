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
        private static readonly Logger nlogger = LogManager.GetCurrentClassLogger();

        private Device.DeviceStateM _device = new Device.DeviceStateM();
        private Dictionary<Device.RelayDevice.Cmd_r, CheckBox> dictCheckBoxsRyM = new Dictionary<Device.RelayDevice.Cmd_r, CheckBox>();
        private Dictionary<Device.RelayDevice.Cmd_r, CheckBox> dictCheckBoxsRyS = new Dictionary<Device.RelayDevice.Cmd_r, CheckBox>();
        private Dictionary<Device.RelayDevice.Cmd_r, PictureBox> pictureBoxRyM = new Dictionary<Device.RelayDevice.Cmd_r, PictureBox>();
        private Dictionary<Device.RelayDevice.Cmd_r, PictureBox> pictureBoxRyS = new Dictionary<Device.RelayDevice.Cmd_r, PictureBox>();

        bool ErrorAskForClose = false;

        // 闪烁等
        Bitmap mBmpM;
        Bitmap mBmpS;
        private bool flp = false;
        private Timer timPic = new Timer();

        Bitmap mBmpRelayRed;
        Bitmap mBmpRelayGreen;

        /// <summary>
        /// 是否是程序要求关闭自身
        /// 当出现错误，用户未响应时，程序要求关闭自身
        /// 则在关闭时，不再弹出提示框
        /// </summary>
        bool ErrorAskForClose = false;

        public FormMain()
        {
            InitializeComponent();

            // check box
            dictCheckBoxsRyM[Device.RelayDevice.Cmd_r.OUT_0] = this.checkBox_ryM0;
            dictCheckBoxsRyM[Device.RelayDevice.Cmd_r.OUT_4] = this.checkBox_ryM4;
            dictCheckBoxsRyM[Device.RelayDevice.Cmd_r.OUT_5] = this.checkBox_ryM5;
            dictCheckBoxsRyM[Device.RelayDevice.Cmd_r.OUT_6] = this.checkBox_ryM6;
            dictCheckBoxsRyM[Device.RelayDevice.Cmd_r.OUT_7] = this.checkBox_ryM7;

            dictCheckBoxsRyS[Device.RelayDevice.Cmd_r.OUT_0] = this.checkBox_ryS0;
            dictCheckBoxsRyS[Device.RelayDevice.Cmd_r.OUT_1] = this.checkBox_ryS1;

            // picture box
            pictureBoxRyM.Add(Device.RelayDevice.Cmd_r.OUT_0, pictureBox_ryM0);
            pictureBoxRyM.Add(Device.RelayDevice.Cmd_r.OUT_4, pictureBox_ryM4);
            pictureBoxRyM.Add(Device.RelayDevice.Cmd_r.OUT_5, pictureBox_ryM5);
            pictureBoxRyM.Add(Device.RelayDevice.Cmd_r.OUT_6, pictureBox_ryM6);
            pictureBoxRyM.Add(Device.RelayDevice.Cmd_r.OUT_7, pictureBox_ryM7);

            pictureBoxRyS.Add(Device.RelayDevice.Cmd_r.OUT_0, pictureBox_ryS0);
            pictureBoxRyS.Add(Device.RelayDevice.Cmd_r.OUT_1, pictureBox_ryS1);

            // 用于继电器的指示灯
            mBmpRelayRed = new Bitmap(this.pictureBox_ryM0.Width, pictureBox_ryM0.Height);
            Graphics mGhpRed = Graphics.FromImage(mBmpRelayRed);
            mGhpRed.Clear(Color.Red);
            mBmpRelayGreen = new Bitmap(this.pictureBox_ryM0.Width, pictureBox_ryM0.Height);
            Graphics mGhpGreen = Graphics.FromImage(mBmpRelayGreen);
            mGhpGreen.Clear(Color.Green);

            // 用于状态指示灯
            mBmpM = new Bitmap(pictureBoxM.Width, pictureBoxM.Height);
            mBmpS = new Bitmap(pictureBoxS.Width, pictureBoxS.Height);
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

            _device.ryDeviceM.DisconnectProtect = this.checkBox_protect.Checked;
            _device.ryDeviceS.DisconnectProtect = this.checkBox_protect.Checked;
        }

        ///////////////////////////////////////////////////
        // 控温板通讯指示灯闪烁
        private void TimPic_Tick(object sender, EventArgs e)
        {
            Graphics mGhpM = Graphics.FromImage(mBmpM);
            Graphics mGhpS = Graphics.FromImage(mBmpS);
            mGhpM.Clear(SystemColors.Control);
            if (flp)
            {
                mGhpM.Clear(SystemColors.Control);
                mGhpS.Clear(SystemColors.Control);
                flp = false;
            }
            else
            {
                mGhpM.Clear(this._device.tpDeviceM.currentComStatus ? Color.Green : Color.Red);
                mGhpS.Clear(this._device.tpDeviceS.currentComStatus ? Color.Green : Color.Red);
                flp = true;
            }

            pictureBoxM.Image = mBmpM;
            pictureBoxS.Image = mBmpS;
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
                _device.tpDeviceS.Enable = true;

                _device.ryDeviceM.Enable = true;
                _device.ryDeviceS.Enable = true;

                _device.ryDeviceM.DisconnectProtect = true;
                _device.ryDeviceS.DisconnectProtect = true;
            }));

            bool confDevice = _device.Configure();
            if (confDevice == false)
            {
                this.BeginInvoke(new EventHandler(delegate
                {
                    if (DialogResult.Yes == MessageBox.Show("设备端口错误，是否退出程序？", "程序关闭确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    {
                        ErrorAskForClose = true;
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

            _device.updateEvents();
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

                    foreach (var chk in dictCheckBoxsRyS)
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

                    foreach (var chk in dictCheckBoxsRyS)
                    {
                        chk.Value.Enabled = true;
                    }
                }
            }));

        }

        private void checkBox_exit_Click(object sender, EventArgs e)
        {
            this.ErrorAskForClose = false;

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

        private void checkBox_curveM_Click(object sender, EventArgs e)
        {
            bool formExist = false;
            foreach (Form fm in Application.OpenForms)
            {
                if (fm.Name == "FormChartM")
                {
                    // Avoid form being minimized
                    fm.WindowState = FormWindowState.Normal;
                    fm.Location = new System.Drawing.Point(10, 12);
                    fm.BringToFront();
                    formExist = true;
                }
            }

            if (!formExist)
            {
                ChartConfig cfg = new ChartConfig();
                cfg.chartTitle = "主槽控温";
                cfg.column = 10;
                cfg.row = 7;
                cfg.startTime = _device.startTime;
                cfg.dataShow = _device.tpDeviceM.temperaturesShow;
                cfg.digits = 3;
                cfg.dataLocker = _device.tpDeviceM.tpShowLocker;
                cfg.dataIntervalSec = _device._runningParameters.readTempIntervalSec;
                cfg.funcPtr = _device.tpDeviceM.GetFlucDurCountOrLess;

                FormChart fm = new FormChart(cfg, this);
                fm.Location = new System.Drawing.Point(10, 12);
                fm.Show();
            }

            Utils.Logger.Op("打开主槽控温设备温度曲线界面!");
        }

        private void checkBox_curveS_Click(object sender, EventArgs e)
        {
            bool formExist = false;
            foreach (Form fm in Application.OpenForms)
            {
                if (fm.Name == "FormChartS")
                {
                    // Avoid form being minimized
                    fm.WindowState = FormWindowState.Normal;
                    fm.Location = new System.Drawing.Point(10, 12);
                    fm.BringToFront();
                    formExist = true;
                }
            }

            if (!formExist)
            {
                ChartConfig cfg = new ChartConfig();
                cfg.chartTitle = "辅槽控温";
                cfg.column = 10;
                cfg.row = 7;
                cfg.startTime = _device.startTime;
                cfg.dataShow = _device.tpDeviceS.temperaturesShow;
                cfg.digits = 3;
                cfg.dataLocker = _device.tpDeviceS.tpShowLocker;
                cfg.dataIntervalSec = _device._runningParameters.readTempIntervalSec;
                cfg.funcPtr = _device.tpDeviceS.GetFlucDurCountOrLess;

                FormChart fm = new FormChart(cfg, this);
                fm.Location = new System.Drawing.Point(10, 12);
                fm.Show();
            }

            Utils.Logger.Op("打开辅槽控温设备温度曲线界面!");
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
