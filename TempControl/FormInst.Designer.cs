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
            this.comboBox_port = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBox_cfg = new System.Windows.Forms.CheckBox();
            this.checkBox_cfgC = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBox_inst
            // 
            this.comboBox_inst.FormattingEnabled = true;
            this.comboBox_inst.Location = new System.Drawing.Point(6, 50);
            this.comboBox_inst.Name = "comboBox_inst";
            this.comboBox_inst.Size = new System.Drawing.Size(187, 20);
            this.comboBox_inst.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(6, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 23);
            this.label1.TabIndex = 2;
            this.label1.Text = "仪器编号：";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox_cfgC);
            this.groupBox1.Controls.Add(this.checkBox_cfg);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.comboBox_port);
            this.groupBox1.Controls.Add(this.comboBox_inst);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(724, 96);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "仪器配置";
            // 
            // comboBox_port
            // 
            this.comboBox_port.FormattingEnabled = true;
            this.comboBox_port.Location = new System.Drawing.Point(219, 50);
            this.comboBox_port.Name = "comboBox_port";
            this.comboBox_port.Size = new System.Drawing.Size(187, 20);
            this.comboBox_port.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(219, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 23);
            this.label2.TabIndex = 2;
            this.label2.Text = "硬件接口：";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // checkBox_cfg
            // 
            this.checkBox_cfg.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox_cfg.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox_cfg.Location = new System.Drawing.Point(431, 35);
            this.checkBox_cfg.Name = "checkBox_cfg";
            this.checkBox_cfg.Size = new System.Drawing.Size(111, 39);
            this.checkBox_cfg.TabIndex = 3;
            this.checkBox_cfg.Text = "确认配置";
            this.checkBox_cfg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox_cfg.UseVisualStyleBackColor = true;
            this.checkBox_cfg.Click += new System.EventHandler(this.checkBox_cfg_Click);
            // 
            // checkBox_cfgC
            // 
            this.checkBox_cfgC.Appearance = System.Windows.Forms.Appearance.Button;
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
            // FormInst
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.groupBox1);
            this.Name = "FormInst";
            this.Text = "FormInst";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox_inst;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox_port;
        private System.Windows.Forms.CheckBox checkBox_cfg;
        private System.Windows.Forms.CheckBox checkBox_cfgC;
    }
}