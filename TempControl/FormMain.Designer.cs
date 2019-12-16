namespace TempControl
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
            this.pictureBoxM = new System.Windows.Forms.PictureBox();
            this.checkBox_debug = new System.Windows.Forms.CheckBox();
            this.checkBox_ryM4 = new System.Windows.Forms.CheckBox();
            this.checkBox_ryM5 = new System.Windows.Forms.CheckBox();
            this.checkBox_ryM0 = new System.Windows.Forms.CheckBox();
            this.checkBox_data = new System.Windows.Forms.CheckBox();
            this.groupBox_ry1 = new System.Windows.Forms.GroupBox();
            this.pictureBox_ryM5 = new System.Windows.Forms.PictureBox();
            this.pictureBox_ryM4 = new System.Windows.Forms.PictureBox();
            this.pictureBox_ryM0 = new System.Windows.Forms.PictureBox();
            this.groupBox_tempM = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.TempPic = new System.Windows.Forms.PictureBox();
            this.checkBox_clear = new System.Windows.Forms.CheckBox();
            this.label_time = new System.Windows.Forms.Label();
            this.pictureBox_logo = new System.Windows.Forms.PictureBox();
            this.label_tempValue = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxM)).BeginInit();
            this.groupBox_ry1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ryM5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ryM4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ryM0)).BeginInit();
            this.groupBox_tempM.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TempPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_logo)).BeginInit();
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
            this.label_tempM.Text = "0.00%";
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
            this.label_tempSetM.Text = "0.00%";
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
            this.label_powerM.Text = "未加气";
            this.label_powerM.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Location = new System.Drawing.Point(365, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(385, 108);
            this.label3.TabIndex = 3;
            // 
            // label_controlState
            // 
            this.label_controlState.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_controlState.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label_controlState.Location = new System.Drawing.Point(369, 22);
            this.label_controlState.Name = "label_controlState";
            this.label_controlState.Size = new System.Drawing.Size(358, 23);
            this.label_controlState.TabIndex = 5;
            this.label_controlState.Text = "系统启动";
            // 
            // label_fluc
            // 
            this.label_fluc.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_fluc.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label_fluc.Location = new System.Drawing.Point(369, 66);
            this.label_fluc.Name = "label_fluc";
            this.label_fluc.Size = new System.Drawing.Size(358, 23);
            this.label_fluc.TabIndex = 6;
            this.label_fluc.Text = "波动度: ***";
            // 
            // checkBox_logM
            // 
            this.checkBox_logM.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_logM.AutoCheck = false;
            this.checkBox_logM.Location = new System.Drawing.Point(262, 464);
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
            this.checkBox_paramM.Location = new System.Drawing.Point(141, 20);
            this.checkBox_paramM.Name = "checkBox_paramM";
            this.checkBox_paramM.Size = new System.Drawing.Size(81, 50);
            this.checkBox_paramM.TabIndex = 15;
            this.checkBox_paramM.Text = "下一点";
            this.checkBox_paramM.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_paramM.UseVisualStyleBackColor = true;
            this.checkBox_paramM.Click += new System.EventHandler(this.checkBox_paramM_Click);
            // 
            // checkBox_exit
            // 
            this.checkBox_exit.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_exit.AutoCheck = false;
            this.checkBox_exit.ForeColor = System.Drawing.Color.Red;
            this.checkBox_exit.Location = new System.Drawing.Point(141, 173);
            this.checkBox_exit.Name = "checkBox_exit";
            this.checkBox_exit.Size = new System.Drawing.Size(81, 50);
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
            this.checkBox_auto.Location = new System.Drawing.Point(141, 94);
            this.checkBox_auto.Name = "checkBox_auto";
            this.checkBox_auto.Size = new System.Drawing.Size(81, 50);
            this.checkBox_auto.TabIndex = 17;
            this.checkBox_auto.Text = "自动";
            this.checkBox_auto.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_auto.UseVisualStyleBackColor = true;
            this.checkBox_auto.Click += new System.EventHandler(this.checkBox_auto_Click);
            // 
            // pictureBoxM
            // 
            this.pictureBoxM.Location = new System.Drawing.Point(8, 19);
            this.pictureBoxM.Name = "pictureBoxM";
            this.pictureBoxM.Size = new System.Drawing.Size(40, 10);
            this.pictureBoxM.TabIndex = 19;
            this.pictureBoxM.TabStop = false;
            // 
            // checkBox_debug
            // 
            this.checkBox_debug.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_debug.AutoCheck = false;
            this.checkBox_debug.Location = new System.Drawing.Point(12, 464);
            this.checkBox_debug.Name = "checkBox_debug";
            this.checkBox_debug.Size = new System.Drawing.Size(79, 37);
            this.checkBox_debug.TabIndex = 20;
            this.checkBox_debug.Text = "调试";
            this.checkBox_debug.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_debug.UseVisualStyleBackColor = true;
            this.checkBox_debug.Click += new System.EventHandler(this.checkBox_debug_Click);
            // 
            // checkBox_ryM4
            // 
            this.checkBox_ryM4.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_ryM4.Location = new System.Drawing.Point(8, 92);
            this.checkBox_ryM4.Name = "checkBox_ryM4";
            this.checkBox_ryM4.Size = new System.Drawing.Size(81, 50);
            this.checkBox_ryM4.TabIndex = 25;
            this.checkBox_ryM4.Text = "氧气";
            this.checkBox_ryM4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_ryM4.UseVisualStyleBackColor = true;
            this.checkBox_ryM4.CheckedChanged += new System.EventHandler(this.checkBox_ryM4_CheckedChanged);
            this.checkBox_ryM4.Click += new System.EventHandler(this.checkBox_ryAll_Click);
            // 
            // checkBox_ryM5
            // 
            this.checkBox_ryM5.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_ryM5.Location = new System.Drawing.Point(10, 171);
            this.checkBox_ryM5.Name = "checkBox_ryM5";
            this.checkBox_ryM5.Size = new System.Drawing.Size(81, 50);
            this.checkBox_ryM5.TabIndex = 22;
            this.checkBox_ryM5.Text = "氮气";
            this.checkBox_ryM5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_ryM5.UseVisualStyleBackColor = true;
            this.checkBox_ryM5.CheckedChanged += new System.EventHandler(this.checkBox_ryM4_CheckedChanged);
            this.checkBox_ryM5.Click += new System.EventHandler(this.checkBox_ryAll_Click);
            // 
            // checkBox_ryM0
            // 
            this.checkBox_ryM0.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_ryM0.Location = new System.Drawing.Point(6, 20);
            this.checkBox_ryM0.Name = "checkBox_ryM0";
            this.checkBox_ryM0.Size = new System.Drawing.Size(81, 50);
            this.checkBox_ryM0.TabIndex = 21;
            this.checkBox_ryM0.Text = "总电源";
            this.checkBox_ryM0.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_ryM0.UseVisualStyleBackColor = true;
            this.checkBox_ryM0.Click += new System.EventHandler(this.checkBox_ryAll_Click);
            // 
            // checkBox_data
            // 
            this.checkBox_data.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_data.AutoCheck = false;
            this.checkBox_data.Location = new System.Drawing.Point(137, 464);
            this.checkBox_data.Name = "checkBox_data";
            this.checkBox_data.Size = new System.Drawing.Size(79, 37);
            this.checkBox_data.TabIndex = 29;
            this.checkBox_data.Text = "数据查询";
            this.checkBox_data.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_data.UseVisualStyleBackColor = true;
            this.checkBox_data.Click += new System.EventHandler(this.checkBox_data_Click);
            // 
            // groupBox_ry1
            // 
            this.groupBox_ry1.AutoSize = true;
            this.groupBox_ry1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox_ry1.Controls.Add(this.pictureBox_ryM5);
            this.groupBox_ry1.Controls.Add(this.pictureBox_ryM4);
            this.groupBox_ry1.Controls.Add(this.pictureBox_ryM0);
            this.groupBox_ry1.Controls.Add(this.checkBox_exit);
            this.groupBox_ry1.Controls.Add(this.checkBox_ryM0);
            this.groupBox_ry1.Controls.Add(this.checkBox_auto);
            this.groupBox_ry1.Controls.Add(this.checkBox_ryM5);
            this.groupBox_ry1.Controls.Add(this.checkBox_ryM4);
            this.groupBox_ry1.Controls.Add(this.checkBox_paramM);
            this.groupBox_ry1.Location = new System.Drawing.Point(499, 204);
            this.groupBox_ry1.Name = "groupBox_ry1";
            this.groupBox_ry1.Size = new System.Drawing.Size(228, 243);
            this.groupBox_ry1.TabIndex = 31;
            this.groupBox_ry1.TabStop = false;
            this.groupBox_ry1.Text = "继电器模块";
            // 
            // pictureBox_ryM5
            // 
            this.pictureBox_ryM5.Location = new System.Drawing.Point(12, 173);
            this.pictureBox_ryM5.Name = "pictureBox_ryM5";
            this.pictureBox_ryM5.Size = new System.Drawing.Size(30, 10);
            this.pictureBox_ryM5.TabIndex = 33;
            this.pictureBox_ryM5.TabStop = false;
            // 
            // pictureBox_ryM4
            // 
            this.pictureBox_ryM4.Location = new System.Drawing.Point(10, 94);
            this.pictureBox_ryM4.Name = "pictureBox_ryM4";
            this.pictureBox_ryM4.Size = new System.Drawing.Size(30, 10);
            this.pictureBox_ryM4.TabIndex = 31;
            this.pictureBox_ryM4.TabStop = false;
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
            this.groupBox_tempM.Controls.Add(this.label2);
            this.groupBox_tempM.Controls.Add(this.label4);
            this.groupBox_tempM.Controls.Add(this.pictureBoxM);
            this.groupBox_tempM.Controls.Add(this.label_tempM);
            this.groupBox_tempM.Controls.Add(this.label_tempSetM);
            this.groupBox_tempM.Controls.Add(this.label_powerM);
            this.groupBox_tempM.Location = new System.Drawing.Point(12, 12);
            this.groupBox_tempM.Name = "groupBox_tempM";
            this.groupBox_tempM.Size = new System.Drawing.Size(333, 165);
            this.groupBox_tempM.TabIndex = 33;
            this.groupBox_tempM.TabStop = false;
            this.groupBox_tempM.Text = "海水槽";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(8, 44);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(104, 19);
            this.label5.TabIndex = 35;
            this.label5.Text = "当前含氧量";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(242, 89);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 19);
            this.label2.TabIndex = 34;
            this.label2.Text = "当前状态";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(6, 89);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(123, 19);
            this.label4.TabIndex = 35;
            this.label4.Text = "含氧量设定值";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TempPic
            // 
            this.TempPic.Location = new System.Drawing.Point(12, 204);
            this.TempPic.Name = "TempPic";
            this.TempPic.Size = new System.Drawing.Size(455, 243);
            this.TempPic.TabIndex = 34;
            this.TempPic.TabStop = false;
            // 
            // checkBox_clear
            // 
            this.checkBox_clear.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_clear.AutoCheck = false;
            this.checkBox_clear.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.checkBox_clear.Location = new System.Drawing.Point(387, 464);
            this.checkBox_clear.Name = "checkBox_clear";
            this.checkBox_clear.Size = new System.Drawing.Size(79, 38);
            this.checkBox_clear.TabIndex = 35;
            this.checkBox_clear.Text = "清空曲线";
            this.checkBox_clear.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_clear.UseVisualStyleBackColor = true;
            this.checkBox_clear.Click += new System.EventHandler(this.checkBox_clear_Click);
            // 
            // label_time
            // 
            this.label_time.AutoSize = true;
            this.label_time.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_time.Location = new System.Drawing.Point(506, 473);
            this.label_time.Name = "label_time";
            this.label_time.Size = new System.Drawing.Size(88, 16);
            this.label_time.TabIndex = 36;
            this.label_time.Text = "控温时间：";
            this.label_time.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pictureBox_logo
            // 
            this.pictureBox_logo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox_logo.Image = global::TempControl.Properties.Resources.company_logo;
            this.pictureBox_logo.Location = new System.Drawing.Point(0, 0);
            this.pictureBox_logo.Name = "pictureBox_logo";
            this.pictureBox_logo.Size = new System.Drawing.Size(764, 515);
            this.pictureBox_logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_logo.TabIndex = 37;
            this.pictureBox_logo.TabStop = false;
            // 
            // label_tempValue
            // 
            this.label_tempValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label_tempValue.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_tempValue.Location = new System.Drawing.Point(549, 125);
            this.label_tempValue.Name = "label_tempValue";
            this.label_tempValue.Size = new System.Drawing.Size(150, 35);
            this.label_tempValue.TabIndex = 1;
            this.label_tempValue.Text = "0.000℃";
            this.label_tempValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(439, 135);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(104, 19);
            this.label6.TabIndex = 35;
            this.label6.Text = "当前温度值";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FormMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(764, 515);
            this.Controls.Add(this.label_time);
            this.Controls.Add(this.checkBox_clear);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.TempPic);
            this.Controls.Add(this.checkBox_data);
            this.Controls.Add(this.groupBox_tempM);
            this.Controls.Add(this.label_tempValue);
            this.Controls.Add(this.groupBox_ry1);
            this.Controls.Add(this.checkBox_debug);
            this.Controls.Add(this.label_fluc);
            this.Controls.Add(this.label_controlState);
            this.Controls.Add(this.checkBox_logM);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pictureBox_logo);
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "氧饱和系统 - 北京三原兆兴";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxM)).EndInit();
            this.groupBox_ry1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ryM5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ryM4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ryM0)).EndInit();
            this.groupBox_tempM.ResumeLayout(false);
            this.groupBox_tempM.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TempPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_logo)).EndInit();
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
        private System.Windows.Forms.PictureBox pictureBoxM;
        private System.Windows.Forms.CheckBox checkBox_debug;
        private System.Windows.Forms.CheckBox checkBox_ryM4;
        private System.Windows.Forms.CheckBox checkBox_ryM5;
        private System.Windows.Forms.CheckBox checkBox_ryM0;
        private System.Windows.Forms.CheckBox checkBox_data;
        private System.Windows.Forms.GroupBox groupBox_ry1;
        private System.Windows.Forms.GroupBox groupBox_tempM;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pictureBox_ryM5;
        private System.Windows.Forms.PictureBox pictureBox_ryM4;
        private System.Windows.Forms.PictureBox pictureBox_ryM0;
        private System.Windows.Forms.PictureBox TempPic;
        private System.Windows.Forms.CheckBox checkBox_clear;
        private System.Windows.Forms.Label label_time;
        private System.Windows.Forms.PictureBox pictureBox_logo;
        private System.Windows.Forms.Label label_tempValue;
        private System.Windows.Forms.Label label6;
    }
}