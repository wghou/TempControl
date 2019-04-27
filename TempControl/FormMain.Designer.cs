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
            this.cartesianChart = new LiveCharts.WinForms.CartesianChart();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label_tempM = new System.Windows.Forms.Label();
            this.label_tempSetM = new System.Windows.Forms.Label();
            this.label_powerM = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label_controlState = new System.Windows.Forms.Label();
            this.label_fluc = new System.Windows.Forms.Label();
            this.checkBox_ry0 = new System.Windows.Forms.CheckBox();
            this.checkBox_ry4 = new System.Windows.Forms.CheckBox();
            this.checkBox_ry1 = new System.Windows.Forms.CheckBox();
            this.checkBox_ry5 = new System.Windows.Forms.CheckBox();
            this.checkBox_ry2 = new System.Windows.Forms.CheckBox();
            this.checkBox_ry6 = new System.Windows.Forms.CheckBox();
            this.checkBox_ry3 = new System.Windows.Forms.CheckBox();
            this.checkBox_ry7 = new System.Windows.Forms.CheckBox();
            this.checkBox_logM = new System.Windows.Forms.CheckBox();
            this.checkBox_paramM = new System.Windows.Forms.CheckBox();
            this.checkBox_exit = new System.Windows.Forms.CheckBox();
            this.checkBox_auto = new System.Windows.Forms.CheckBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.checkBox_debug = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // cartesianChart
            // 
            this.cartesianChart.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cartesianChart.Location = new System.Drawing.Point(12, 225);
            this.cartesianChart.Name = "cartesianChart";
            this.cartesianChart.Size = new System.Drawing.Size(385, 290);
            this.cartesianChart.TabIndex = 1;
            this.cartesianChart.Text = "温度值曲线";
            // 
            // label_tempM
            // 
            this.label_tempM.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label_tempM.Font = new System.Drawing.Font("宋体", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_tempM.Location = new System.Drawing.Point(38, 31);
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
            this.label_tempSetM.Location = new System.Drawing.Point(38, 112);
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
            this.label_powerM.Location = new System.Drawing.Point(209, 112);
            this.label_powerM.Name = "label_powerM";
            this.label_powerM.Size = new System.Drawing.Size(150, 35);
            this.label_powerM.TabIndex = 2;
            this.label_powerM.Text = "00%";
            this.label_powerM.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Location = new System.Drawing.Point(413, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(318, 127);
            this.label3.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(212, 113);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 33);
            this.label1.TabIndex = 4;
            this.label1.Text = "加热功率";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_controlState
            // 
            this.label_controlState.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_controlState.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label_controlState.Location = new System.Drawing.Point(417, 41);
            this.label_controlState.Name = "label_controlState";
            this.label_controlState.Size = new System.Drawing.Size(293, 23);
            this.label_controlState.TabIndex = 5;
            this.label_controlState.Text = "系统启动";
            // 
            // label_fluc
            // 
            this.label_fluc.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_fluc.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label_fluc.Location = new System.Drawing.Point(417, 80);
            this.label_fluc.Name = "label_fluc";
            this.label_fluc.Size = new System.Drawing.Size(313, 23);
            this.label_fluc.TabIndex = 6;
            this.label_fluc.Text = "波动度: ***";
            // 
            // checkBox_ry0
            // 
            this.checkBox_ry0.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_ry0.Location = new System.Drawing.Point(413, 197);
            this.checkBox_ry0.Name = "checkBox_ry0";
            this.checkBox_ry0.Size = new System.Drawing.Size(136, 50);
            this.checkBox_ry0.TabIndex = 7;
            this.checkBox_ry0.Text = "继电器 0";
            this.checkBox_ry0.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_ry0.UseVisualStyleBackColor = true;
            this.checkBox_ry0.Click += new System.EventHandler(this.checkBox_ry0_Click);
            // 
            // checkBox_ry4
            // 
            this.checkBox_ry4.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_ry4.Location = new System.Drawing.Point(595, 197);
            this.checkBox_ry4.Name = "checkBox_ry4";
            this.checkBox_ry4.Size = new System.Drawing.Size(136, 50);
            this.checkBox_ry4.TabIndex = 8;
            this.checkBox_ry4.Text = "继电器 4";
            this.checkBox_ry4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_ry4.UseVisualStyleBackColor = true;
            this.checkBox_ry4.Click += new System.EventHandler(this.checkBox_ry4_Click);
            // 
            // checkBox_ry1
            // 
            this.checkBox_ry1.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_ry1.Location = new System.Drawing.Point(413, 264);
            this.checkBox_ry1.Name = "checkBox_ry1";
            this.checkBox_ry1.Size = new System.Drawing.Size(136, 50);
            this.checkBox_ry1.TabIndex = 9;
            this.checkBox_ry1.Text = "继电器 1";
            this.checkBox_ry1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_ry1.UseVisualStyleBackColor = true;
            this.checkBox_ry1.Click += new System.EventHandler(this.checkBox_ry1_Click);
            // 
            // checkBox_ry5
            // 
            this.checkBox_ry5.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_ry5.Location = new System.Drawing.Point(595, 264);
            this.checkBox_ry5.Name = "checkBox_ry5";
            this.checkBox_ry5.Size = new System.Drawing.Size(136, 50);
            this.checkBox_ry5.TabIndex = 10;
            this.checkBox_ry5.Text = "继电器 5";
            this.checkBox_ry5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_ry5.UseVisualStyleBackColor = true;
            this.checkBox_ry5.Click += new System.EventHandler(this.checkBox_ry5_Click);
            // 
            // checkBox_ry2
            // 
            this.checkBox_ry2.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_ry2.Location = new System.Drawing.Point(413, 331);
            this.checkBox_ry2.Name = "checkBox_ry2";
            this.checkBox_ry2.Size = new System.Drawing.Size(136, 50);
            this.checkBox_ry2.TabIndex = 11;
            this.checkBox_ry2.Text = "继电器 2";
            this.checkBox_ry2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_ry2.UseVisualStyleBackColor = true;
            this.checkBox_ry2.Click += new System.EventHandler(this.checkBox_ry2_Click);
            // 
            // checkBox_ry6
            // 
            this.checkBox_ry6.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_ry6.Location = new System.Drawing.Point(595, 331);
            this.checkBox_ry6.Name = "checkBox_ry6";
            this.checkBox_ry6.Size = new System.Drawing.Size(136, 50);
            this.checkBox_ry6.TabIndex = 12;
            this.checkBox_ry6.Text = "继电器 6";
            this.checkBox_ry6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_ry6.UseVisualStyleBackColor = true;
            this.checkBox_ry6.Click += new System.EventHandler(this.checkBox_ry6_Click);
            // 
            // checkBox_ry3
            // 
            this.checkBox_ry3.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_ry3.Location = new System.Drawing.Point(413, 398);
            this.checkBox_ry3.Name = "checkBox_ry3";
            this.checkBox_ry3.Size = new System.Drawing.Size(136, 50);
            this.checkBox_ry3.TabIndex = 13;
            this.checkBox_ry3.Text = "继电器 3";
            this.checkBox_ry3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_ry3.UseVisualStyleBackColor = true;
            this.checkBox_ry3.Click += new System.EventHandler(this.checkBox_ry3_Click);
            // 
            // checkBox_ry7
            // 
            this.checkBox_ry7.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_ry7.Location = new System.Drawing.Point(595, 398);
            this.checkBox_ry7.Name = "checkBox_ry7";
            this.checkBox_ry7.Size = new System.Drawing.Size(136, 50);
            this.checkBox_ry7.TabIndex = 14;
            this.checkBox_ry7.Text = "继电器 7";
            this.checkBox_ry7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_ry7.UseVisualStyleBackColor = true;
            this.checkBox_ry7.Click += new System.EventHandler(this.checkBox_ry7_Click);
            // 
            // checkBox_logM
            // 
            this.checkBox_logM.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_logM.AutoCheck = false;
            this.checkBox_logM.Location = new System.Drawing.Point(223, 162);
            this.checkBox_logM.Name = "checkBox_logM";
            this.checkBox_logM.Size = new System.Drawing.Size(136, 37);
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
            this.checkBox_paramM.Location = new System.Drawing.Point(38, 162);
            this.checkBox_paramM.Name = "checkBox_paramM";
            this.checkBox_paramM.Size = new System.Drawing.Size(136, 37);
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
            this.checkBox_exit.ForeColor = System.Drawing.Color.Red;
            this.checkBox_exit.Location = new System.Drawing.Point(595, 465);
            this.checkBox_exit.Name = "checkBox_exit";
            this.checkBox_exit.Size = new System.Drawing.Size(136, 50);
            this.checkBox_exit.TabIndex = 18;
            this.checkBox_exit.Text = "退出系统";
            this.checkBox_exit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_exit.UseVisualStyleBackColor = true;
            this.checkBox_exit.Click += new System.EventHandler(this.checkBox_exit_Click);
            // 
            // checkBox_auto
            // 
            this.checkBox_auto.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_auto.Location = new System.Drawing.Point(413, 465);
            this.checkBox_auto.Name = "checkBox_auto";
            this.checkBox_auto.Size = new System.Drawing.Size(136, 50);
            this.checkBox_auto.TabIndex = 17;
            this.checkBox_auto.Text = "自动";
            this.checkBox_auto.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_auto.UseVisualStyleBackColor = true;
            this.checkBox_auto.Click += new System.EventHandler(this.checkBox_auto_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(39, 32);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(40, 10);
            this.pictureBox1.TabIndex = 19;
            this.pictureBox1.TabStop = false;
            // 
            // checkBox_debug
            // 
            this.checkBox_debug.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_debug.AutoCheck = false;
            this.checkBox_debug.Location = new System.Drawing.Point(2, 2);
            this.checkBox_debug.Name = "checkBox_debug";
            this.checkBox_debug.Size = new System.Drawing.Size(77, 24);
            this.checkBox_debug.TabIndex = 20;
            this.checkBox_debug.Text = "调试";
            this.checkBox_debug.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_debug.UseVisualStyleBackColor = true;
            this.checkBox_debug.Click += new System.EventHandler(this.checkBox_debug_Click);
            // 
            // FormMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(754, 532);
            this.ControlBox = false;
            this.Controls.Add(this.checkBox_debug);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.checkBox_exit);
            this.Controls.Add(this.checkBox_auto);
            this.Controls.Add(this.checkBox_logM);
            this.Controls.Add(this.checkBox_paramM);
            this.Controls.Add(this.cartesianChart);
            this.Controls.Add(this.checkBox_ry7);
            this.Controls.Add(this.checkBox_ry3);
            this.Controls.Add(this.checkBox_ry6);
            this.Controls.Add(this.checkBox_ry2);
            this.Controls.Add(this.checkBox_ry5);
            this.Controls.Add(this.checkBox_ry1);
            this.Controls.Add(this.checkBox_ry4);
            this.Controls.Add(this.checkBox_ry0);
            this.Controls.Add(this.label_fluc);
            this.Controls.Add(this.label_controlState);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label_powerM);
            this.Controls.Add(this.label_tempSetM);
            this.Controls.Add(this.label_tempM);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMain";
            this.Text = "自动控温系统";
            this.Shown += new System.EventHandler(this.Form1_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private LiveCharts.WinForms.CartesianChart cartesianChart;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label_tempM;
        private System.Windows.Forms.Label label_tempSetM;
        private System.Windows.Forms.Label label_powerM;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label_controlState;
        private System.Windows.Forms.Label label_fluc;
        private System.Windows.Forms.CheckBox checkBox_ry0;
        private System.Windows.Forms.CheckBox checkBox_ry4;
        private System.Windows.Forms.CheckBox checkBox_ry1;
        private System.Windows.Forms.CheckBox checkBox_ry5;
        private System.Windows.Forms.CheckBox checkBox_ry2;
        private System.Windows.Forms.CheckBox checkBox_ry6;
        private System.Windows.Forms.CheckBox checkBox_ry3;
        private System.Windows.Forms.CheckBox checkBox_ry7;
        private System.Windows.Forms.CheckBox checkBox_logM;
        private System.Windows.Forms.CheckBox checkBox_paramM;
        private System.Windows.Forms.CheckBox checkBox_exit;
        private System.Windows.Forms.CheckBox checkBox_auto;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox checkBox_debug;
    }
}

