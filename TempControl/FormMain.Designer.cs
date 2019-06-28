﻿namespace TempControl
{
    partial class FormMain
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
            this.components = new System.ComponentModel.Container();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label_tempM = new System.Windows.Forms.Label();
            this.label_tempSetM = new System.Windows.Forms.Label();
            this.label_powerM = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label_controlState = new System.Windows.Forms.Label();
            this.label_fluc = new System.Windows.Forms.Label();
            this.checkBox_logM = new System.Windows.Forms.CheckBox();
            this.checkBox_paramM = new System.Windows.Forms.CheckBox();
            this.checkBox_exit = new System.Windows.Forms.CheckBox();
            this.checkBox_auto = new System.Windows.Forms.CheckBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.checkBox_debug = new System.Windows.Forms.CheckBox();
            this.checkBox_ryM6 = new System.Windows.Forms.CheckBox();
            this.checkBox_ryM5 = new System.Windows.Forms.CheckBox();
            this.checkBox_ryM4 = new System.Windows.Forms.CheckBox();
            this.checkBox_ryM0 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.groupBox_ry1 = new System.Windows.Forms.GroupBox();
            this.pictureBox_ryM6 = new System.Windows.Forms.PictureBox();
            this.pictureBox_ryM5 = new System.Windows.Forms.PictureBox();
            this.pictureBox_ryM4 = new System.Windows.Forms.PictureBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.pictureBox_ryM3 = new System.Windows.Forms.PictureBox();
            this.pictureBox_ryM2 = new System.Windows.Forms.PictureBox();
            this.pictureBox_ryM1 = new System.Windows.Forms.PictureBox();
            this.pictureBox_ryM0 = new System.Windows.Forms.PictureBox();
            this.groupBox_tempM = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.checkBox_curveM = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox_tempS = new System.Windows.Forms.GroupBox();
            this.checkBox_shutdownPC = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox_curveS = new System.Windows.Forms.CheckBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label_tempS = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox_ry1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ryM6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ryM5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ryM4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ryM3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ryM2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ryM1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ryM0)).BeginInit();
            this.groupBox_tempM.SuspendLayout();
            this.groupBox_tempS.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // label_tempM
            // 
            this.label_tempM.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label_tempM.Font = new System.Drawing.Font("宋体", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_tempM.Location = new System.Drawing.Point(6, 17);
            this.label_tempM.Name = "label_tempM";
            this.label_tempM.Size = new System.Drawing.Size(321, 60);
            this.label_tempM.TabIndex = 0;
            this.label_tempM.Text = "0.0000℃";
            this.label_tempM.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label_tempSetM
            // 
            this.label_tempSetM.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label_tempSetM.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_tempSetM.Location = new System.Drawing.Point(6, 113);
            this.label_tempSetM.Name = "label_tempSetM";
            this.label_tempSetM.Size = new System.Drawing.Size(150, 35);
            this.label_tempSetM.TabIndex = 1;
            this.label_tempSetM.Text = "0.0000℃";
            this.label_tempSetM.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label_powerM
            // 
            this.label_powerM.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label_powerM.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_powerM.Location = new System.Drawing.Point(216, 113);
            this.label_powerM.Name = "label_powerM";
            this.label_powerM.Size = new System.Drawing.Size(111, 35);
            this.label_powerM.TabIndex = 2;
            this.label_powerM.Text = "00%";
            this.label_powerM.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Location = new System.Drawing.Point(365, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(340, 87);
            this.label3.TabIndex = 3;
            // 
            // label_controlState
            // 
            this.label_controlState.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_controlState.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label_controlState.Location = new System.Drawing.Point(369, 22);
            this.label_controlState.Name = "label_controlState";
            this.label_controlState.Size = new System.Drawing.Size(323, 23);
            this.label_controlState.TabIndex = 5;
            this.label_controlState.Text = "系统启动";
            // 
            // label_fluc
            // 
            this.label_fluc.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_fluc.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label_fluc.Location = new System.Drawing.Point(369, 61);
            this.label_fluc.Name = "label_fluc";
            this.label_fluc.Size = new System.Drawing.Size(323, 23);
            this.label_fluc.TabIndex = 6;
            this.label_fluc.Text = "波动度: ***";
            // 
            // checkBox_logM
            // 
            this.checkBox_logM.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_logM.AutoCheck = false;
            this.checkBox_logM.Location = new System.Drawing.Point(248, 162);
            this.checkBox_logM.Name = "checkBox_logM";
            this.checkBox_logM.Size = new System.Drawing.Size(79, 37);
            this.checkBox_logM.TabIndex = 16;
            this.checkBox_logM.Text = "操作日志";
            this.checkBox_logM.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_logM.UseVisualStyleBackColor = true;
            this.checkBox_logM.Click += new System.EventHandler(this.checkBox_logM_Click);
            // 
            // checkBox_paramM
            // 
            this.checkBox_paramM.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_paramM.AutoCheck = false;
            this.checkBox_paramM.Location = new System.Drawing.Point(6, 162);
            this.checkBox_paramM.Name = "checkBox_paramM";
            this.checkBox_paramM.Size = new System.Drawing.Size(79, 37);
            this.checkBox_paramM.TabIndex = 15;
            this.checkBox_paramM.Text = "参数设置";
            this.checkBox_paramM.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_paramM.UseVisualStyleBackColor = true;
            this.checkBox_paramM.Click += new System.EventHandler(this.checkBox_paramM_Click);
            // 
            // checkBox_exit
            // 
            this.checkBox_exit.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_exit.AutoCheck = false;
            this.checkBox_exit.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox_exit.ForeColor = System.Drawing.Color.Red;
            this.checkBox_exit.Location = new System.Drawing.Point(202, 241);
            this.checkBox_exit.Name = "checkBox_exit";
            this.checkBox_exit.Size = new System.Drawing.Size(138, 50);
            this.checkBox_exit.TabIndex = 18;
            this.checkBox_exit.Text = "退出系统";
            this.checkBox_exit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_exit.UseVisualStyleBackColor = true;
            this.checkBox_exit.Click += new System.EventHandler(this.checkBox_exit_Click);
            // 
            // checkBox_auto
            // 
            this.checkBox_auto.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_auto.AutoCheck = false;
            this.checkBox_auto.Location = new System.Drawing.Point(8, 177);
            this.checkBox_auto.Name = "checkBox_auto";
            this.checkBox_auto.Size = new System.Drawing.Size(138, 50);
            this.checkBox_auto.TabIndex = 17;
            this.checkBox_auto.Text = "自动";
            this.checkBox_auto.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_auto.UseVisualStyleBackColor = true;
            this.checkBox_auto.Click += new System.EventHandler(this.checkBox_auto_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(8, 19);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(40, 10);
            this.pictureBox1.TabIndex = 19;
            this.pictureBox1.TabStop = false;
            // 
            // checkBox_debug
            // 
            this.checkBox_debug.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_debug.AutoCheck = false;
            this.checkBox_debug.Location = new System.Drawing.Point(127, 96);
            this.checkBox_debug.Name = "checkBox_debug";
            this.checkBox_debug.Size = new System.Drawing.Size(79, 37);
            this.checkBox_debug.TabIndex = 20;
            this.checkBox_debug.Text = "调试";
            this.checkBox_debug.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_debug.UseVisualStyleBackColor = true;
            this.checkBox_debug.Click += new System.EventHandler(this.checkBox_debug_Click);
            // 
            // checkBox_ryM6
            // 
            this.checkBox_ryM6.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_ryM6.Location = new System.Drawing.Point(202, 175);
            this.checkBox_ryM6.Name = "checkBox_ryM6";
            this.checkBox_ryM6.Size = new System.Drawing.Size(138, 50);
            this.checkBox_ryM6.TabIndex = 26;
            this.checkBox_ryM6.Text = "循环";
            this.checkBox_ryM6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_ryM6.UseVisualStyleBackColor = true;
            this.checkBox_ryM6.Click += new System.EventHandler(this.checkBox_ryM6_Click);
            // 
            // checkBox_ryM5
            // 
            this.checkBox_ryM5.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_ryM5.Location = new System.Drawing.Point(202, 112);
            this.checkBox_ryM5.Name = "checkBox_ryM5";
            this.checkBox_ryM5.Size = new System.Drawing.Size(138, 50);
            this.checkBox_ryM5.TabIndex = 24;
            this.checkBox_ryM5.Text = "制冷";
            this.checkBox_ryM5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_ryM5.UseVisualStyleBackColor = true;
            this.checkBox_ryM5.Click += new System.EventHandler(this.checkBox_ryM5_Click);
            // 
            // checkBox_ryM4
            // 
            this.checkBox_ryM4.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_ryM4.Location = new System.Drawing.Point(202, 45);
            this.checkBox_ryM4.Name = "checkBox_ryM4";
            this.checkBox_ryM4.Size = new System.Drawing.Size(138, 50);
            this.checkBox_ryM4.TabIndex = 22;
            this.checkBox_ryM4.Text = "控温";
            this.checkBox_ryM4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_ryM4.UseVisualStyleBackColor = true;
            this.checkBox_ryM4.Click += new System.EventHandler(this.checkBox_ryM4_Click);
            // 
            // checkBox_ryM0
            // 
            this.checkBox_ryM0.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_ryM0.Location = new System.Drawing.Point(8, 45);
            this.checkBox_ryM0.Name = "checkBox_ryM0";
            this.checkBox_ryM0.Size = new System.Drawing.Size(138, 50);
            this.checkBox_ryM0.TabIndex = 21;
            this.checkBox_ryM0.Text = "总电源";
            this.checkBox_ryM0.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_ryM0.UseVisualStyleBackColor = true;
            this.checkBox_ryM0.Click += new System.EventHandler(this.checkBox_ryM0_Click);
            // 
            // checkBox2
            // 
            this.checkBox2.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox2.AutoCheck = false;
            this.checkBox2.Location = new System.Drawing.Point(8, 112);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(138, 50);
            this.checkBox2.TabIndex = 29;
            this.checkBox2.Text = "数据查询";
            this.checkBox2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.Click += new System.EventHandler(this.checkBox2_Click);
            // 
            // groupBox_ry1
            // 
            this.groupBox_ry1.AutoSize = true;
            this.groupBox_ry1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox_ry1.Controls.Add(this.pictureBox_ryM6);
            this.groupBox_ry1.Controls.Add(this.pictureBox_ryM5);
            this.groupBox_ry1.Controls.Add(this.pictureBox_ryM4);
            this.groupBox_ry1.Controls.Add(this.checkBox1);
            this.groupBox_ry1.Controls.Add(this.pictureBox_ryM3);
            this.groupBox_ry1.Controls.Add(this.checkBox_exit);
            this.groupBox_ry1.Controls.Add(this.checkBox2);
            this.groupBox_ry1.Controls.Add(this.pictureBox_ryM2);
            this.groupBox_ry1.Controls.Add(this.checkBox_auto);
            this.groupBox_ry1.Controls.Add(this.pictureBox_ryM1);
            this.groupBox_ry1.Controls.Add(this.pictureBox_ryM0);
            this.groupBox_ry1.Controls.Add(this.checkBox_ryM0);
            this.groupBox_ry1.Controls.Add(this.checkBox_ryM4);
            this.groupBox_ry1.Controls.Add(this.checkBox_ryM5);
            this.groupBox_ry1.Controls.Add(this.checkBox_ryM6);
            this.groupBox_ry1.Location = new System.Drawing.Point(365, 116);
            this.groupBox_ry1.Name = "groupBox_ry1";
            this.groupBox_ry1.Size = new System.Drawing.Size(346, 311);
            this.groupBox_ry1.TabIndex = 31;
            this.groupBox_ry1.TabStop = false;
            this.groupBox_ry1.Text = "继电器模块 1";
            // 
            // pictureBox_ryM6
            // 
            this.pictureBox_ryM6.Location = new System.Drawing.Point(204, 177);
            this.pictureBox_ryM6.Name = "pictureBox_ryM6";
            this.pictureBox_ryM6.Size = new System.Drawing.Size(30, 10);
            this.pictureBox_ryM6.TabIndex = 35;
            this.pictureBox_ryM6.TabStop = false;
            // 
            // pictureBox_ryM5
            // 
            this.pictureBox_ryM5.Location = new System.Drawing.Point(204, 114);
            this.pictureBox_ryM5.Name = "pictureBox_ryM5";
            this.pictureBox_ryM5.Size = new System.Drawing.Size(30, 10);
            this.pictureBox_ryM5.TabIndex = 34;
            this.pictureBox_ryM5.TabStop = false;
            // 
            // pictureBox_ryM4
            // 
            this.pictureBox_ryM4.Location = new System.Drawing.Point(204, 47);
            this.pictureBox_ryM4.Name = "pictureBox_ryM4";
            this.pictureBox_ryM4.Size = new System.Drawing.Size(30, 10);
            this.pictureBox_ryM4.TabIndex = 33;
            this.pictureBox_ryM4.TabStop = false;
            // 
            // checkBox1
            // 
            this.checkBox1.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox1.AutoCheck = false;
            this.checkBox1.ForeColor = System.Drawing.Color.Black;
            this.checkBox1.Location = new System.Drawing.Point(8, 241);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(138, 50);
            this.checkBox1.TabIndex = 30;
            this.checkBox1.Text = "备用";
            this.checkBox1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.Click += new System.EventHandler(this.checkBox1_Click);
            // 
            // pictureBox_ryM3
            // 
            this.pictureBox_ryM3.Location = new System.Drawing.Point(116, 22);
            this.pictureBox_ryM3.Name = "pictureBox_ryM3";
            this.pictureBox_ryM3.Size = new System.Drawing.Size(30, 10);
            this.pictureBox_ryM3.TabIndex = 32;
            this.pictureBox_ryM3.TabStop = false;
            // 
            // pictureBox_ryM2
            // 
            this.pictureBox_ryM2.Location = new System.Drawing.Point(80, 22);
            this.pictureBox_ryM2.Name = "pictureBox_ryM2";
            this.pictureBox_ryM2.Size = new System.Drawing.Size(30, 10);
            this.pictureBox_ryM2.TabIndex = 31;
            this.pictureBox_ryM2.TabStop = false;
            // 
            // pictureBox_ryM1
            // 
            this.pictureBox_ryM1.Location = new System.Drawing.Point(44, 22);
            this.pictureBox_ryM1.Name = "pictureBox_ryM1";
            this.pictureBox_ryM1.Size = new System.Drawing.Size(30, 10);
            this.pictureBox_ryM1.TabIndex = 30;
            this.pictureBox_ryM1.TabStop = false;
            // 
            // pictureBox_ryM0
            // 
            this.pictureBox_ryM0.Location = new System.Drawing.Point(8, 22);
            this.pictureBox_ryM0.Name = "pictureBox_ryM0";
            this.pictureBox_ryM0.Size = new System.Drawing.Size(30, 10);
            this.pictureBox_ryM0.TabIndex = 29;
            this.pictureBox_ryM0.TabStop = false;
            // 
            // groupBox_tempM
            // 
            this.groupBox_tempM.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox_tempM.AutoSize = true;
            this.groupBox_tempM.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox_tempM.Controls.Add(this.label5);
            this.groupBox_tempM.Controls.Add(this.checkBox_curveM);
            this.groupBox_tempM.Controls.Add(this.label2);
            this.groupBox_tempM.Controls.Add(this.label4);
            this.groupBox_tempM.Controls.Add(this.pictureBox1);
            this.groupBox_tempM.Controls.Add(this.label_tempM);
            this.groupBox_tempM.Controls.Add(this.label_tempSetM);
            this.groupBox_tempM.Controls.Add(this.label_powerM);
            this.groupBox_tempM.Controls.Add(this.checkBox_paramM);
            this.groupBox_tempM.Controls.Add(this.checkBox_logM);
            this.groupBox_tempM.Location = new System.Drawing.Point(12, 12);
            this.groupBox_tempM.Name = "groupBox_tempM";
            this.groupBox_tempM.Size = new System.Drawing.Size(333, 219);
            this.groupBox_tempM.TabIndex = 33;
            this.groupBox_tempM.TabStop = false;
            this.groupBox_tempM.Text = "前端水槽";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(8, 44);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 19);
            this.label5.TabIndex = 35;
            this.label5.Text = "槽温";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // checkBox_curveM
            // 
            this.checkBox_curveM.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_curveM.AutoCheck = false;
            this.checkBox_curveM.Location = new System.Drawing.Point(127, 162);
            this.checkBox_curveM.Name = "checkBox_curveM";
            this.checkBox_curveM.Size = new System.Drawing.Size(79, 37);
            this.checkBox_curveM.TabIndex = 36;
            this.checkBox_curveM.Text = "温度曲线";
            this.checkBox_curveM.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_curveM.UseVisualStyleBackColor = true;
            this.checkBox_curveM.Click += new System.EventHandler(this.checkBox_curveM_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(242, 89);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 19);
            this.label2.TabIndex = 34;
            this.label2.Text = "加热功率";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(6, 89);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 19);
            this.label4.TabIndex = 35;
            this.label4.Text = "温度设定值";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox_tempS
            // 
            this.groupBox_tempS.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox_tempS.AutoSize = true;
            this.groupBox_tempS.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox_tempS.Controls.Add(this.checkBox_shutdownPC);
            this.groupBox_tempS.Controls.Add(this.label1);
            this.groupBox_tempS.Controls.Add(this.checkBox_curveS);
            this.groupBox_tempS.Controls.Add(this.checkBox_debug);
            this.groupBox_tempS.Controls.Add(this.pictureBox2);
            this.groupBox_tempS.Controls.Add(this.label_tempS);
            this.groupBox_tempS.Location = new System.Drawing.Point(12, 249);
            this.groupBox_tempS.Name = "groupBox_tempS";
            this.groupBox_tempS.Size = new System.Drawing.Size(333, 157);
            this.groupBox_tempS.TabIndex = 34;
            this.groupBox_tempS.TabStop = false;
            this.groupBox_tempS.Text = "后端水槽";
            // 
            // checkBox_shutdownPC
            // 
            this.checkBox_shutdownPC.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox_shutdownPC.ForeColor = System.Drawing.Color.Red;
            this.checkBox_shutdownPC.Location = new System.Drawing.Point(223, 96);
            this.checkBox_shutdownPC.Name = "checkBox_shutdownPC";
            this.checkBox_shutdownPC.Size = new System.Drawing.Size(104, 41);
            this.checkBox_shutdownPC.TabIndex = 37;
            this.checkBox_shutdownPC.Text = "试验完成后关闭计算机";
            this.checkBox_shutdownPC.UseVisualStyleBackColor = true;
            this.checkBox_shutdownPC.CheckedChanged += new System.EventHandler(this.checkBox_shutdownPC_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(8, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 19);
            this.label1.TabIndex = 35;
            this.label1.Text = "槽温";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // checkBox_curveS
            // 
            this.checkBox_curveS.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_curveS.AutoCheck = false;
            this.checkBox_curveS.Location = new System.Drawing.Point(6, 96);
            this.checkBox_curveS.Name = "checkBox_curveS";
            this.checkBox_curveS.Size = new System.Drawing.Size(79, 37);
            this.checkBox_curveS.TabIndex = 36;
            this.checkBox_curveS.Text = "温度曲线";
            this.checkBox_curveS.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_curveS.UseVisualStyleBackColor = true;
            this.checkBox_curveS.Click += new System.EventHandler(this.checkBox_curveS_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(8, 19);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(40, 10);
            this.pictureBox2.TabIndex = 19;
            this.pictureBox2.TabStop = false;
            // 
            // label_tempS
            // 
            this.label_tempS.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label_tempS.Font = new System.Drawing.Font("宋体", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_tempS.Location = new System.Drawing.Point(6, 17);
            this.label_tempS.Name = "label_tempS";
            this.label_tempS.Size = new System.Drawing.Size(321, 60);
            this.label_tempS.TabIndex = 0;
            this.label_tempS.Text = "0.000℃";
            this.label_tempS.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FormMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(725, 443);
            this.Controls.Add(this.groupBox_tempS);
            this.Controls.Add(this.groupBox_tempM);
            this.Controls.Add(this.groupBox_ry1);
            this.Controls.Add(this.label_fluc);
            this.Controls.Add(this.label_controlState);
            this.Controls.Add(this.label3);
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.Text = "北京三原兆兴  -- 自动控温系统";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox_ry1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ryM6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ryM5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ryM4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ryM3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ryM2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ryM1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ryM0)).EndInit();
            this.groupBox_tempM.ResumeLayout(false);
            this.groupBox_tempM.PerformLayout();
            this.groupBox_tempS.ResumeLayout(false);
            this.groupBox_tempS.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label_tempM;
        private System.Windows.Forms.Label label_tempSetM;
        private System.Windows.Forms.Label label_powerM;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label_controlState;
        private System.Windows.Forms.Label label_fluc;
        private System.Windows.Forms.CheckBox checkBox_logM;
        private System.Windows.Forms.CheckBox checkBox_paramM;
        private System.Windows.Forms.CheckBox checkBox_exit;
        private System.Windows.Forms.CheckBox checkBox_auto;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox checkBox_debug;
        private System.Windows.Forms.CheckBox checkBox_ryM6;
        private System.Windows.Forms.CheckBox checkBox_ryM5;
        private System.Windows.Forms.CheckBox checkBox_ryM4;
        private System.Windows.Forms.CheckBox checkBox_ryM0;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.GroupBox groupBox_ry1;
        private System.Windows.Forms.GroupBox groupBox_tempM;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox checkBox_curveM;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox_tempS;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox_curveS;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label_tempS;
        private System.Windows.Forms.PictureBox pictureBox_ryM6;
        private System.Windows.Forms.PictureBox pictureBox_ryM5;
        private System.Windows.Forms.PictureBox pictureBox_ryM4;
        private System.Windows.Forms.PictureBox pictureBox_ryM3;
        private System.Windows.Forms.PictureBox pictureBox_ryM2;
        private System.Windows.Forms.PictureBox pictureBox_ryM1;
        private System.Windows.Forms.PictureBox pictureBox_ryM0;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox_shutdownPC;
    }
}

