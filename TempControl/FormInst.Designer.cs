namespace TempControl
{
    partial class FormInst
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.comboBox_inst = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox_cfgC = new System.Windows.Forms.CheckBox();
            this.checkBox_rdDev = new System.Windows.Forms.CheckBox();
            this.checkBox_set2dev = new System.Windows.Forms.CheckBox();
            this.checkBox_cfg = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox_baudRate = new System.Windows.Forms.ComboBox();
            this.comboBox_port = new System.Windows.Forms.ComboBox();
            this.comboBox_spInterval = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_instID = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBox_inst
            // 
            this.comboBox_inst.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_inst.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox_inst.FormattingEnabled = true;
            this.comboBox_inst.Location = new System.Drawing.Point(6, 50);
            this.comboBox_inst.Name = "comboBox_inst";
            this.comboBox_inst.Size = new System.Drawing.Size(141, 22);
            this.comboBox_inst.TabIndex = 0;
            this.comboBox_inst.SelectedIndexChanged += new System.EventHandler(this.comboBox_inst_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(6, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 23);
            this.label1.TabIndex = 2;
            this.label1.Text = "仪器编号：";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox_cfgC);
            this.groupBox1.Controls.Add(this.checkBox_rdDev);
            this.groupBox1.Controls.Add(this.checkBox_set2dev);
            this.groupBox1.Controls.Add(this.checkBox_cfg);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.comboBox_baudRate);
            this.groupBox1.Controls.Add(this.comboBox_port);
            this.groupBox1.Controls.Add(this.comboBox_inst);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(724, 140);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "仪器配置";
            // 
            // checkBox_cfgC
            // 
            this.checkBox_cfgC.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_cfgC.AutoCheck = false;
            this.checkBox_cfgC.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox_cfgC.ForeColor = System.Drawing.Color.Red;
            this.checkBox_cfgC.Location = new System.Drawing.Point(561, 35);
            this.checkBox_cfgC.Name = "checkBox_cfgC";
            this.checkBox_cfgC.Size = new System.Drawing.Size(111, 39);
            this.checkBox_cfgC.TabIndex = 3;
            this.checkBox_cfgC.Text = "清除配置";
            this.checkBox_cfgC.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_cfgC.UseVisualStyleBackColor = true;
            this.checkBox_cfgC.Click += new System.EventHandler(this.checkBox_cfgC_Click);
            // 
            // checkBox_rdDev
            // 
            this.checkBox_rdDev.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_rdDev.AutoCheck = false;
            this.checkBox_rdDev.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox_rdDev.Location = new System.Drawing.Point(561, 90);
            this.checkBox_rdDev.Name = "checkBox_rdDev";
            this.checkBox_rdDev.Size = new System.Drawing.Size(111, 39);
            this.checkBox_rdDev.TabIndex = 3;
            this.checkBox_rdDev.Text = "读取信息";
            this.checkBox_rdDev.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_rdDev.UseVisualStyleBackColor = true;
            this.checkBox_rdDev.Click += new System.EventHandler(this.checkBox_rdDev_Click);
            // 
            // checkBox_set2dev
            // 
            this.checkBox_set2dev.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_set2dev.AutoCheck = false;
            this.checkBox_set2dev.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox_set2dev.Location = new System.Drawing.Point(431, 90);
            this.checkBox_set2dev.Name = "checkBox_set2dev";
            this.checkBox_set2dev.Size = new System.Drawing.Size(111, 39);
            this.checkBox_set2dev.TabIndex = 3;
            this.checkBox_set2dev.Text = "写入设备";
            this.checkBox_set2dev.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_set2dev.UseVisualStyleBackColor = true;
            this.checkBox_set2dev.Click += new System.EventHandler(this.checkBox_set2dev_Click);
            // 
            // checkBox_cfg
            // 
            this.checkBox_cfg.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_cfg.AutoCheck = false;
            this.checkBox_cfg.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox_cfg.Location = new System.Drawing.Point(431, 35);
            this.checkBox_cfg.Name = "checkBox_cfg";
            this.checkBox_cfg.Size = new System.Drawing.Size(111, 39);
            this.checkBox_cfg.TabIndex = 3;
            this.checkBox_cfg.Text = "暂存配置";
            this.checkBox_cfg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_cfg.UseVisualStyleBackColor = true;
            this.checkBox_cfg.Click += new System.EventHandler(this.checkBox_cfg_Click);
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(281, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 23);
            this.label3.TabIndex = 2;
            this.label3.Text = "波特率：";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(163, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 23);
            this.label2.TabIndex = 2;
            this.label2.Text = "硬件接口：";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboBox_baudRate
            // 
            this.comboBox_baudRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_baudRate.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox_baudRate.FormattingEnabled = true;
            this.comboBox_baudRate.Location = new System.Drawing.Point(281, 50);
            this.comboBox_baudRate.Name = "comboBox_baudRate";
            this.comboBox_baudRate.Size = new System.Drawing.Size(126, 22);
            this.comboBox_baudRate.TabIndex = 0;
            // 
            // comboBox_port
            // 
            this.comboBox_port.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_port.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox_port.FormattingEnabled = true;
            this.comboBox_port.Location = new System.Drawing.Point(163, 50);
            this.comboBox_port.Name = "comboBox_port";
            this.comboBox_port.Size = new System.Drawing.Size(98, 22);
            this.comboBox_port.TabIndex = 0;
            // 
            // comboBox_spInterval
            // 
            this.comboBox_spInterval.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_spInterval.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox_spInterval.FormattingEnabled = true;
            this.comboBox_spInterval.Location = new System.Drawing.Point(293, 184);
            this.comboBox_spInterval.Name = "comboBox_spInterval";
            this.comboBox_spInterval.Size = new System.Drawing.Size(126, 22);
            this.comboBox_spInterval.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(293, 155);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(126, 23);
            this.label4.TabIndex = 2;
            this.label4.Text = "采样间隔 / 秒：";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBox_instID
            // 
            this.textBox_instID.Location = new System.Drawing.Point(21, 186);
            this.textBox_instID.Name = "textBox_instID";
            this.textBox_instID.Size = new System.Drawing.Size(252, 21);
            this.textBox_instID.TabIndex = 5;
            // 
            // FormInst
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.textBox_instID);
            this.Controls.Add(this.comboBox_spInterval);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormInst";
            this.Text = "FormInst";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox_inst;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox_port;
        private System.Windows.Forms.CheckBox checkBox_cfg;
        private System.Windows.Forms.CheckBox checkBox_cfgC;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBox_baudRate;
        private System.Windows.Forms.CheckBox checkBox_set2dev;
        private System.Windows.Forms.CheckBox checkBox_rdDev;
        private System.Windows.Forms.ComboBox comboBox_spInterval;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_instID;
    }
}