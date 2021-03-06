﻿namespace IotMonitor
{
    partial class IotMonitorForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.hslGaugeChart_temptM = new HslControls.HslGaugeChart();
            this.hslGaugeChart_temptS = new HslControls.HslGaugeChart();
            this.hslLedDisplay_temptSetM = new HslControls.HslLedDisplay();
            this.hslGauge_powerM = new HslControls.HslGauge();
            this.hslLedDisplay_temptSetS = new HslControls.HslLedDisplay();
            this.hslSwitch_RelayM0 = new HslControls.HslSwitch();
            this.hslSwitch_RelayM1 = new HslControls.HslSwitch();
            this.hslSwitch_RelayM2 = new HslControls.HslSwitch();
            this.hslSwitch_RelayM3 = new HslControls.HslSwitch();
            this.hslSwitch_RelayM4 = new HslControls.HslSwitch();
            this.hslSwitch_RelayM5 = new HslControls.HslSwitch();
            this.hslSwitch_RelayM6 = new HslControls.HslSwitch();
            this.hslSwitch_RelayM7 = new HslControls.HslSwitch();
            this.hslSwitch_RelayS7 = new HslControls.HslSwitch();
            this.hslSwitch_RelayS6 = new HslControls.HslSwitch();
            this.hslSwitch_RelayS5 = new HslControls.HslSwitch();
            this.hslSwitch_RelayS4 = new HslControls.HslSwitch();
            this.hslSwitch_RelayS3 = new HslControls.HslSwitch();
            this.hslSwitch_RelayS2 = new HslControls.HslSwitch();
            this.hslSwitch_RelayS1 = new HslControls.HslSwitch();
            this.hslSwitch_RelayS0 = new HslControls.HslSwitch();
            this.hslGauge_powerS = new HslControls.HslGauge();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.pictureBox_iot = new System.Windows.Forms.PictureBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label_state = new System.Windows.Forms.Label();
            this.label_tm4 = new System.Windows.Forms.Label();
            this.label_tm3 = new System.Windows.Forms.Label();
            this.label_tm2 = new System.Windows.Forms.Label();
            this.label_tm1 = new System.Windows.Forms.Label();
            this.label_tm5 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.richTextBox_log = new System.Windows.Forms.RichTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_iot)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // hslGaugeChart_temptM
            // 
            this.hslGaugeChart_temptM.Location = new System.Drawing.Point(6, 6);
            this.hslGaugeChart_temptM.Name = "hslGaugeChart_temptM";
            this.hslGaugeChart_temptM.SegmentCount = 6;
            this.hslGaugeChart_temptM.Size = new System.Drawing.Size(250, 120);
            this.hslGaugeChart_temptM.TabIndex = 1;
            this.hslGaugeChart_temptM.UnitText = "℃";
            this.hslGaugeChart_temptM.Value = 12D;
            this.hslGaugeChart_temptM.ValueAlarmMax = 40D;
            this.hslGaugeChart_temptM.ValueAlarmMin = -5D;
            this.hslGaugeChart_temptM.ValueMax = 50D;
            this.hslGaugeChart_temptM.ValueStart = -10D;
            // 
            // hslGaugeChart_temptS
            // 
            this.hslGaugeChart_temptS.Location = new System.Drawing.Point(6, 148);
            this.hslGaugeChart_temptS.Name = "hslGaugeChart_temptS";
            this.hslGaugeChart_temptS.SegmentCount = 6;
            this.hslGaugeChart_temptS.Size = new System.Drawing.Size(250, 120);
            this.hslGaugeChart_temptS.TabIndex = 2;
            this.hslGaugeChart_temptS.UnitText = "℃";
            this.hslGaugeChart_temptS.Value = 12D;
            this.hslGaugeChart_temptS.ValueAlarmMax = 40D;
            this.hslGaugeChart_temptS.ValueAlarmMin = -5D;
            this.hslGaugeChart_temptS.ValueMax = 50D;
            this.hslGaugeChart_temptS.ValueStart = -10D;
            // 
            // hslLedDisplay_temptSetM
            // 
            this.hslLedDisplay_temptSetM.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.hslLedDisplay_temptSetM.DisplayBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(62)))));
            this.hslLedDisplay_temptSetM.DisplayNumber = 7;
            this.hslLedDisplay_temptSetM.DisplayText = "10.0000";
            this.hslLedDisplay_temptSetM.LedNumberSize = 4;
            this.hslLedDisplay_temptSetM.Location = new System.Drawing.Point(543, 39);
            this.hslLedDisplay_temptSetM.Name = "hslLedDisplay_temptSetM";
            this.hslLedDisplay_temptSetM.Size = new System.Drawing.Size(282, 64);
            this.hslLedDisplay_temptSetM.TabIndex = 3;
            this.hslLedDisplay_temptSetM.UnitText = "℃";
            // 
            // hslGauge_powerM
            // 
            this.hslGauge_powerM.Location = new System.Drawing.Point(287, 6);
            this.hslGauge_powerM.Name = "hslGauge_powerM";
            this.hslGauge_powerM.PointerColor = System.Drawing.Color.Cyan;
            this.hslGauge_powerM.SegmentCount = 5;
            this.hslGauge_powerM.Size = new System.Drawing.Size(250, 120);
            this.hslGauge_powerM.TabIndex = 4;
            this.hslGauge_powerM.UnitText = "%";
            this.hslGauge_powerM.Value = 20F;
            this.hslGauge_powerM.ValueAlarmMax = 100F;
            this.hslGauge_powerM.ValueAlarmMin = 0F;
            // 
            // hslLedDisplay_temptSetS
            // 
            this.hslLedDisplay_temptSetS.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.hslLedDisplay_temptSetS.DisplayBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(62)))));
            this.hslLedDisplay_temptSetS.DisplayNumber = 7;
            this.hslLedDisplay_temptSetS.DisplayText = "10.0000";
            this.hslLedDisplay_temptSetS.LedNumberSize = 4;
            this.hslLedDisplay_temptSetS.Location = new System.Drawing.Point(543, 138);
            this.hslLedDisplay_temptSetS.Name = "hslLedDisplay_temptSetS";
            this.hslLedDisplay_temptSetS.Size = new System.Drawing.Size(282, 64);
            this.hslLedDisplay_temptSetS.TabIndex = 6;
            this.hslLedDisplay_temptSetS.UnitText = "℃";
            // 
            // hslSwitch_RelayM0
            // 
            this.hslSwitch_RelayM0.AutoSize = true;
            this.hslSwitch_RelayM0.Cursor = System.Windows.Forms.Cursors.Hand;
            this.hslSwitch_RelayM0.Enabled = false;
            this.hslSwitch_RelayM0.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.hslSwitch_RelayM0.Location = new System.Drawing.Point(62, 307);
            this.hslSwitch_RelayM0.Name = "hslSwitch_RelayM0";
            this.hslSwitch_RelayM0.Size = new System.Drawing.Size(80, 80);
            this.hslSwitch_RelayM0.SwitchForeground = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.hslSwitch_RelayM0.SwitchStatusDescription = "off;on";
            this.hslSwitch_RelayM0.TabIndex = 7;
            this.hslSwitch_RelayM0.Text = "开关 M0";
            this.hslSwitch_RelayM0.Click += new System.EventHandler(this.hslSwitch_RelayM0_Click_1);
            // 
            // hslSwitch_RelayM1
            // 
            this.hslSwitch_RelayM1.AutoSize = true;
            this.hslSwitch_RelayM1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.hslSwitch_RelayM1.Enabled = false;
            this.hslSwitch_RelayM1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.hslSwitch_RelayM1.Location = new System.Drawing.Point(148, 307);
            this.hslSwitch_RelayM1.Name = "hslSwitch_RelayM1";
            this.hslSwitch_RelayM1.Size = new System.Drawing.Size(80, 80);
            this.hslSwitch_RelayM1.SwitchForeground = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.hslSwitch_RelayM1.SwitchStatusDescription = "off;on";
            this.hslSwitch_RelayM1.TabIndex = 8;
            this.hslSwitch_RelayM1.Text = "开关 M1";
            this.hslSwitch_RelayM1.Click += new System.EventHandler(this.hslSwitch_RelayM0_Click_1);
            // 
            // hslSwitch_RelayM2
            // 
            this.hslSwitch_RelayM2.AutoSize = true;
            this.hslSwitch_RelayM2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.hslSwitch_RelayM2.Enabled = false;
            this.hslSwitch_RelayM2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.hslSwitch_RelayM2.Location = new System.Drawing.Point(234, 307);
            this.hslSwitch_RelayM2.Name = "hslSwitch_RelayM2";
            this.hslSwitch_RelayM2.Size = new System.Drawing.Size(80, 80);
            this.hslSwitch_RelayM2.SwitchForeground = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.hslSwitch_RelayM2.SwitchStatusDescription = "off;on";
            this.hslSwitch_RelayM2.TabIndex = 9;
            this.hslSwitch_RelayM2.Text = "开关 M2";
            this.hslSwitch_RelayM2.Click += new System.EventHandler(this.hslSwitch_RelayM0_Click_1);
            // 
            // hslSwitch_RelayM3
            // 
            this.hslSwitch_RelayM3.AutoSize = true;
            this.hslSwitch_RelayM3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.hslSwitch_RelayM3.Enabled = false;
            this.hslSwitch_RelayM3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.hslSwitch_RelayM3.Location = new System.Drawing.Point(320, 307);
            this.hslSwitch_RelayM3.Name = "hslSwitch_RelayM3";
            this.hslSwitch_RelayM3.Size = new System.Drawing.Size(80, 80);
            this.hslSwitch_RelayM3.SwitchForeground = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.hslSwitch_RelayM3.SwitchStatusDescription = "off;on";
            this.hslSwitch_RelayM3.TabIndex = 10;
            this.hslSwitch_RelayM3.Text = "开关 M3";
            this.hslSwitch_RelayM3.Click += new System.EventHandler(this.hslSwitch_RelayM0_Click_1);
            // 
            // hslSwitch_RelayM4
            // 
            this.hslSwitch_RelayM4.AutoSize = true;
            this.hslSwitch_RelayM4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.hslSwitch_RelayM4.Enabled = false;
            this.hslSwitch_RelayM4.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.hslSwitch_RelayM4.Location = new System.Drawing.Point(62, 393);
            this.hslSwitch_RelayM4.Name = "hslSwitch_RelayM4";
            this.hslSwitch_RelayM4.Size = new System.Drawing.Size(80, 80);
            this.hslSwitch_RelayM4.SwitchForeground = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.hslSwitch_RelayM4.SwitchStatusDescription = "off;on";
            this.hslSwitch_RelayM4.TabIndex = 11;
            this.hslSwitch_RelayM4.Text = "开关 M4";
            this.hslSwitch_RelayM4.Click += new System.EventHandler(this.hslSwitch_RelayM0_Click_1);
            // 
            // hslSwitch_RelayM5
            // 
            this.hslSwitch_RelayM5.AutoSize = true;
            this.hslSwitch_RelayM5.Cursor = System.Windows.Forms.Cursors.Hand;
            this.hslSwitch_RelayM5.Enabled = false;
            this.hslSwitch_RelayM5.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.hslSwitch_RelayM5.Location = new System.Drawing.Point(148, 393);
            this.hslSwitch_RelayM5.Name = "hslSwitch_RelayM5";
            this.hslSwitch_RelayM5.Size = new System.Drawing.Size(80, 80);
            this.hslSwitch_RelayM5.SwitchForeground = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.hslSwitch_RelayM5.SwitchStatusDescription = "off;on";
            this.hslSwitch_RelayM5.TabIndex = 12;
            this.hslSwitch_RelayM5.Text = "开关 M5";
            this.hslSwitch_RelayM5.Click += new System.EventHandler(this.hslSwitch_RelayM0_Click_1);
            // 
            // hslSwitch_RelayM6
            // 
            this.hslSwitch_RelayM6.AutoSize = true;
            this.hslSwitch_RelayM6.Cursor = System.Windows.Forms.Cursors.Hand;
            this.hslSwitch_RelayM6.Enabled = false;
            this.hslSwitch_RelayM6.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.hslSwitch_RelayM6.Location = new System.Drawing.Point(234, 393);
            this.hslSwitch_RelayM6.Name = "hslSwitch_RelayM6";
            this.hslSwitch_RelayM6.Size = new System.Drawing.Size(80, 80);
            this.hslSwitch_RelayM6.SwitchForeground = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.hslSwitch_RelayM6.SwitchStatusDescription = "off;on";
            this.hslSwitch_RelayM6.TabIndex = 13;
            this.hslSwitch_RelayM6.Text = "开关 M6";
            this.hslSwitch_RelayM6.Click += new System.EventHandler(this.hslSwitch_RelayM0_Click_1);
            // 
            // hslSwitch_RelayM7
            // 
            this.hslSwitch_RelayM7.AutoSize = true;
            this.hslSwitch_RelayM7.Cursor = System.Windows.Forms.Cursors.Hand;
            this.hslSwitch_RelayM7.Enabled = false;
            this.hslSwitch_RelayM7.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.hslSwitch_RelayM7.Location = new System.Drawing.Point(320, 393);
            this.hslSwitch_RelayM7.Name = "hslSwitch_RelayM7";
            this.hslSwitch_RelayM7.Size = new System.Drawing.Size(80, 80);
            this.hslSwitch_RelayM7.SwitchForeground = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.hslSwitch_RelayM7.SwitchStatusDescription = "off;on";
            this.hslSwitch_RelayM7.TabIndex = 14;
            this.hslSwitch_RelayM7.Text = "开关 M7";
            this.hslSwitch_RelayM7.Click += new System.EventHandler(this.hslSwitch_RelayM0_Click_1);
            // 
            // hslSwitch_RelayS7
            // 
            this.hslSwitch_RelayS7.AutoSize = true;
            this.hslSwitch_RelayS7.Cursor = System.Windows.Forms.Cursors.Hand;
            this.hslSwitch_RelayS7.Enabled = false;
            this.hslSwitch_RelayS7.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.hslSwitch_RelayS7.Location = new System.Drawing.Point(320, 565);
            this.hslSwitch_RelayS7.Name = "hslSwitch_RelayS7";
            this.hslSwitch_RelayS7.Size = new System.Drawing.Size(80, 80);
            this.hslSwitch_RelayS7.SwitchForeground = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.hslSwitch_RelayS7.SwitchStatusDescription = "off;on";
            this.hslSwitch_RelayS7.TabIndex = 22;
            this.hslSwitch_RelayS7.Text = "开关 S7";
            this.hslSwitch_RelayS7.Click += new System.EventHandler(this.hslSwitch_RelayM0_Click_1);
            // 
            // hslSwitch_RelayS6
            // 
            this.hslSwitch_RelayS6.AutoSize = true;
            this.hslSwitch_RelayS6.Cursor = System.Windows.Forms.Cursors.Hand;
            this.hslSwitch_RelayS6.Enabled = false;
            this.hslSwitch_RelayS6.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.hslSwitch_RelayS6.Location = new System.Drawing.Point(234, 565);
            this.hslSwitch_RelayS6.Name = "hslSwitch_RelayS6";
            this.hslSwitch_RelayS6.Size = new System.Drawing.Size(80, 80);
            this.hslSwitch_RelayS6.SwitchForeground = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.hslSwitch_RelayS6.SwitchStatusDescription = "off;on";
            this.hslSwitch_RelayS6.TabIndex = 21;
            this.hslSwitch_RelayS6.Text = "开关 S6";
            this.hslSwitch_RelayS6.Click += new System.EventHandler(this.hslSwitch_RelayM0_Click_1);
            // 
            // hslSwitch_RelayS5
            // 
            this.hslSwitch_RelayS5.AutoSize = true;
            this.hslSwitch_RelayS5.Cursor = System.Windows.Forms.Cursors.Hand;
            this.hslSwitch_RelayS5.Enabled = false;
            this.hslSwitch_RelayS5.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.hslSwitch_RelayS5.Location = new System.Drawing.Point(148, 565);
            this.hslSwitch_RelayS5.Name = "hslSwitch_RelayS5";
            this.hslSwitch_RelayS5.Size = new System.Drawing.Size(80, 80);
            this.hslSwitch_RelayS5.SwitchForeground = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.hslSwitch_RelayS5.SwitchStatusDescription = "off;on";
            this.hslSwitch_RelayS5.TabIndex = 20;
            this.hslSwitch_RelayS5.Text = "开关 S5";
            this.hslSwitch_RelayS5.Click += new System.EventHandler(this.hslSwitch_RelayM0_Click_1);
            // 
            // hslSwitch_RelayS4
            // 
            this.hslSwitch_RelayS4.AutoSize = true;
            this.hslSwitch_RelayS4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.hslSwitch_RelayS4.Enabled = false;
            this.hslSwitch_RelayS4.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.hslSwitch_RelayS4.Location = new System.Drawing.Point(62, 565);
            this.hslSwitch_RelayS4.Name = "hslSwitch_RelayS4";
            this.hslSwitch_RelayS4.Size = new System.Drawing.Size(80, 80);
            this.hslSwitch_RelayS4.SwitchForeground = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.hslSwitch_RelayS4.SwitchStatusDescription = "off;on";
            this.hslSwitch_RelayS4.TabIndex = 19;
            this.hslSwitch_RelayS4.Text = "开关 S4";
            this.hslSwitch_RelayS4.Click += new System.EventHandler(this.hslSwitch_RelayM0_Click_1);
            // 
            // hslSwitch_RelayS3
            // 
            this.hslSwitch_RelayS3.AutoSize = true;
            this.hslSwitch_RelayS3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.hslSwitch_RelayS3.Enabled = false;
            this.hslSwitch_RelayS3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.hslSwitch_RelayS3.Location = new System.Drawing.Point(320, 479);
            this.hslSwitch_RelayS3.Name = "hslSwitch_RelayS3";
            this.hslSwitch_RelayS3.Size = new System.Drawing.Size(80, 80);
            this.hslSwitch_RelayS3.SwitchForeground = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.hslSwitch_RelayS3.SwitchStatusDescription = "off;on";
            this.hslSwitch_RelayS3.TabIndex = 18;
            this.hslSwitch_RelayS3.Text = "开关 S3";
            this.hslSwitch_RelayS3.Click += new System.EventHandler(this.hslSwitch_RelayM0_Click_1);
            // 
            // hslSwitch_RelayS2
            // 
            this.hslSwitch_RelayS2.AutoSize = true;
            this.hslSwitch_RelayS2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.hslSwitch_RelayS2.Enabled = false;
            this.hslSwitch_RelayS2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.hslSwitch_RelayS2.Location = new System.Drawing.Point(234, 479);
            this.hslSwitch_RelayS2.Name = "hslSwitch_RelayS2";
            this.hslSwitch_RelayS2.Size = new System.Drawing.Size(80, 80);
            this.hslSwitch_RelayS2.SwitchForeground = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.hslSwitch_RelayS2.SwitchStatusDescription = "off;on";
            this.hslSwitch_RelayS2.TabIndex = 17;
            this.hslSwitch_RelayS2.Text = "开关 S2";
            this.hslSwitch_RelayS2.Click += new System.EventHandler(this.hslSwitch_RelayM0_Click_1);
            // 
            // hslSwitch_RelayS1
            // 
            this.hslSwitch_RelayS1.AutoSize = true;
            this.hslSwitch_RelayS1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.hslSwitch_RelayS1.Enabled = false;
            this.hslSwitch_RelayS1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.hslSwitch_RelayS1.Location = new System.Drawing.Point(148, 479);
            this.hslSwitch_RelayS1.Name = "hslSwitch_RelayS1";
            this.hslSwitch_RelayS1.Size = new System.Drawing.Size(80, 80);
            this.hslSwitch_RelayS1.SwitchForeground = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.hslSwitch_RelayS1.SwitchStatusDescription = "off;on";
            this.hslSwitch_RelayS1.TabIndex = 16;
            this.hslSwitch_RelayS1.Text = "开关 S1";
            this.hslSwitch_RelayS1.Click += new System.EventHandler(this.hslSwitch_RelayM0_Click_1);
            // 
            // hslSwitch_RelayS0
            // 
            this.hslSwitch_RelayS0.AutoSize = true;
            this.hslSwitch_RelayS0.Cursor = System.Windows.Forms.Cursors.Hand;
            this.hslSwitch_RelayS0.Enabled = false;
            this.hslSwitch_RelayS0.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.hslSwitch_RelayS0.Location = new System.Drawing.Point(62, 479);
            this.hslSwitch_RelayS0.Name = "hslSwitch_RelayS0";
            this.hslSwitch_RelayS0.Size = new System.Drawing.Size(80, 80);
            this.hslSwitch_RelayS0.SwitchForeground = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.hslSwitch_RelayS0.SwitchStatusDescription = "off;on";
            this.hslSwitch_RelayS0.TabIndex = 15;
            this.hslSwitch_RelayS0.Text = "开关 S0";
            this.hslSwitch_RelayS0.Click += new System.EventHandler(this.hslSwitch_RelayM0_Click_1);
            // 
            // hslGauge_powerS
            // 
            this.hslGauge_powerS.Location = new System.Drawing.Point(287, 148);
            this.hslGauge_powerS.Name = "hslGauge_powerS";
            this.hslGauge_powerS.PointerColor = System.Drawing.Color.Cyan;
            this.hslGauge_powerS.SegmentCount = 5;
            this.hslGauge_powerS.Size = new System.Drawing.Size(250, 120);
            this.hslGauge_powerS.TabIndex = 23;
            this.hslGauge_powerS.UnitText = "%";
            this.hslGauge_powerS.Value = 20F;
            this.hslGauge_powerS.ValueAlarmMax = 100F;
            this.hslGauge_powerS.ValueAlarmMin = 0F;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(95, 129);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 16);
            this.label1.TabIndex = 24;
            this.label1.Text = "主槽温度";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(8, 339);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 16);
            this.label2.TabIndex = 25;
            this.label2.Text = "M0-M3";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(360, 129);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 16);
            this.label3.TabIndex = 26;
            this.label3.Text = "主槽加热功率";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(360, 271);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 16);
            this.label4.TabIndex = 27;
            this.label4.Text = "辅槽加热功率";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(540, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(120, 16);
            this.label5.TabIndex = 28;
            this.label5.Text = "主槽设定温度值";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(540, 119);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(120, 16);
            this.label6.TabIndex = 29;
            this.label6.Text = "辅槽设定温度值";
            // 
            // pictureBox_iot
            // 
            this.pictureBox_iot.Location = new System.Drawing.Point(816, 15);
            this.pictureBox_iot.Name = "pictureBox_iot";
            this.pictureBox_iot.Size = new System.Drawing.Size(40, 10);
            this.pictureBox_iot.TabIndex = 43;
            this.pictureBox_iot.TabStop = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(887, 677);
            this.tabControl1.TabIndex = 45;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.richTextBox_log);
            this.tabPage1.Controls.Add(this.label_state);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.label_tm4);
            this.tabPage1.Controls.Add(this.label_tm3);
            this.tabPage1.Controls.Add(this.label_tm2);
            this.tabPage1.Controls.Add(this.label_tm1);
            this.tabPage1.Controls.Add(this.label_tm5);
            this.tabPage1.Controls.Add(this.hslGauge_powerM);
            this.tabPage1.Controls.Add(this.pictureBox_iot);
            this.tabPage1.Controls.Add(this.hslGaugeChart_temptM);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.hslGaugeChart_temptS);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.hslLedDisplay_temptSetM);
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.hslLedDisplay_temptSetS);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.hslSwitch_RelayM0);
            this.tabPage1.Controls.Add(this.label9);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.hslSwitch_RelayM1);
            this.tabPage1.Controls.Add(this.hslSwitch_RelayM2);
            this.tabPage1.Controls.Add(this.hslGauge_powerS);
            this.tabPage1.Controls.Add(this.hslSwitch_RelayM3);
            this.tabPage1.Controls.Add(this.hslSwitch_RelayS7);
            this.tabPage1.Controls.Add(this.hslSwitch_RelayM4);
            this.tabPage1.Controls.Add(this.hslSwitch_RelayS6);
            this.tabPage1.Controls.Add(this.hslSwitch_RelayM5);
            this.tabPage1.Controls.Add(this.hslSwitch_RelayS5);
            this.tabPage1.Controls.Add(this.hslSwitch_RelayM6);
            this.tabPage1.Controls.Add(this.hslSwitch_RelayS4);
            this.tabPage1.Controls.Add(this.hslSwitch_RelayM7);
            this.tabPage1.Controls.Add(this.hslSwitch_RelayS3);
            this.tabPage1.Controls.Add(this.hslSwitch_RelayS0);
            this.tabPage1.Controls.Add(this.hslSwitch_RelayS2);
            this.tabPage1.Controls.Add(this.hslSwitch_RelayS1);
            this.tabPage1.Location = new System.Drawing.Point(4, 26);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(879, 647);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "监测界面";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label_state
            // 
            this.label_state.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label_state.Location = new System.Drawing.Point(543, 251);
            this.label_state.Name = "label_state";
            this.label_state.Size = new System.Drawing.Size(282, 36);
            this.label_state.TabIndex = 47;
            this.label_state.Text = "未连接...";
            this.label_state.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_tm4
            // 
            this.label_tm4.AutoSize = true;
            this.label_tm4.Location = new System.Drawing.Point(836, 775);
            this.label_tm4.Name = "label_tm4";
            this.label_tm4.Size = new System.Drawing.Size(48, 16);
            this.label_tm4.TabIndex = 46;
            this.label_tm4.Text = "13:14";
            // 
            // label_tm3
            // 
            this.label_tm3.AutoSize = true;
            this.label_tm3.Location = new System.Drawing.Point(565, 775);
            this.label_tm3.Name = "label_tm3";
            this.label_tm3.Size = new System.Drawing.Size(48, 16);
            this.label_tm3.TabIndex = 46;
            this.label_tm3.Text = "13:14";
            // 
            // label_tm2
            // 
            this.label_tm2.AutoSize = true;
            this.label_tm2.Location = new System.Drawing.Point(294, 775);
            this.label_tm2.Name = "label_tm2";
            this.label_tm2.Size = new System.Drawing.Size(48, 16);
            this.label_tm2.TabIndex = 46;
            this.label_tm2.Text = "13:14";
            // 
            // label_tm1
            // 
            this.label_tm1.AutoSize = true;
            this.label_tm1.Location = new System.Drawing.Point(23, 775);
            this.label_tm1.Name = "label_tm1";
            this.label_tm1.Size = new System.Drawing.Size(48, 16);
            this.label_tm1.TabIndex = 46;
            this.label_tm1.Text = "13:14";
            // 
            // label_tm5
            // 
            this.label_tm5.AutoSize = true;
            this.label_tm5.Location = new System.Drawing.Point(1107, 775);
            this.label_tm5.Name = "label_tm5";
            this.label_tm5.Size = new System.Drawing.Size(48, 16);
            this.label_tm5.TabIndex = 46;
            this.label_tm5.Text = "13:14";
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 26);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(879, 647);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "控制界面";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // richTextBox_log
            // 
            this.richTextBox_log.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_log.Location = new System.Drawing.Point(406, 339);
            this.richTextBox_log.Name = "richTextBox_log";
            this.richTextBox_log.Size = new System.Drawing.Size(470, 306);
            this.richTextBox_log.TabIndex = 48;
            this.richTextBox_log.Text = "";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(8, 425);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(48, 16);
            this.label7.TabIndex = 25;
            this.label7.Text = "M4-M7";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(8, 511);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(48, 16);
            this.label8.TabIndex = 25;
            this.label8.Text = "S0-S3";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(8, 597);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(48, 16);
            this.label9.TabIndex = 25;
            this.label9.Text = "S4-S7";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(540, 232);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(72, 16);
            this.label10.TabIndex = 27;
            this.label10.Text = "当前流程";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label11.Location = new System.Drawing.Point(406, 320);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(80, 16);
            this.label11.TabIndex = 27;
            this.label11.Text = "错误日志 ";
            // 
            // IotMonitorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(887, 677);
            this.Controls.Add(this.tabControl1);
            this.Name = "IotMonitorForm";
            this.Text = "Form1";
            this.Shown += new System.EventHandler(this.IotMonitorForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_iot)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private HslControls.HslGaugeChart hslGaugeChart_temptM;
        private HslControls.HslGaugeChart hslGaugeChart_temptS;
        private HslControls.HslLedDisplay hslLedDisplay_temptSetM;
        private HslControls.HslGauge hslGauge_powerM;
        private HslControls.HslLedDisplay hslLedDisplay_temptSetS;
        private HslControls.HslSwitch hslSwitch_RelayM0;
        private HslControls.HslSwitch hslSwitch_RelayM1;
        private HslControls.HslSwitch hslSwitch_RelayM2;
        private HslControls.HslSwitch hslSwitch_RelayM3;
        private HslControls.HslSwitch hslSwitch_RelayM4;
        private HslControls.HslSwitch hslSwitch_RelayM5;
        private HslControls.HslSwitch hslSwitch_RelayM6;
        private HslControls.HslSwitch hslSwitch_RelayM7;
        private HslControls.HslSwitch hslSwitch_RelayS7;
        private HslControls.HslSwitch hslSwitch_RelayS6;
        private HslControls.HslSwitch hslSwitch_RelayS5;
        private HslControls.HslSwitch hslSwitch_RelayS4;
        private HslControls.HslSwitch hslSwitch_RelayS3;
        private HslControls.HslSwitch hslSwitch_RelayS2;
        private HslControls.HslSwitch hslSwitch_RelayS1;
        private HslControls.HslSwitch hslSwitch_RelayS0;
        private HslControls.HslGauge hslGauge_powerS;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox pictureBox_iot;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label_tm5;
        private System.Windows.Forms.Label label_tm1;
        private System.Windows.Forms.Label label_tm4;
        private System.Windows.Forms.Label label_tm3;
        private System.Windows.Forms.Label label_tm2;
        private System.Windows.Forms.Label label_state;
        private System.Windows.Forms.RichTextBox richTextBox_log;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
    }
}

