namespace TempControl
{
    partial class FormChart
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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.LblCtrlTimeShow = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.LblFlucShow = new System.Windows.Forms.Label();
            this.BtnClear = new System.Windows.Forms.Button();
            this.eventLog1 = new System.Diagnostics.EventLog();
            this.button_Clear = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.hslCurve1 = new HslControls.HslCurve();
            ((System.ComponentModel.ISupportInitialize)(this.eventLog1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label1.Location = new System.Drawing.Point(17, 294);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "控温时间：";
            // 
            // LblCtrlTimeShow
            // 
            this.LblCtrlTimeShow.AutoSize = true;
            this.LblCtrlTimeShow.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblCtrlTimeShow.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.LblCtrlTimeShow.Location = new System.Drawing.Point(124, 556);
            this.LblCtrlTimeShow.Name = "LblCtrlTimeShow";
            this.LblCtrlTimeShow.Size = new System.Drawing.Size(0, 20);
            this.LblCtrlTimeShow.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label2.Location = new System.Drawing.Point(208, 294);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "5分钟波动度：";
            // 
            // LblFlucShow
            // 
            this.LblFlucShow.AutoSize = true;
            this.LblFlucShow.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblFlucShow.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.LblFlucShow.Location = new System.Drawing.Point(423, 556);
            this.LblFlucShow.Name = "LblFlucShow";
            this.LblFlucShow.Size = new System.Drawing.Size(0, 20);
            this.LblFlucShow.TabIndex = 2;
            // 
            // BtnClear
            // 
            this.BtnClear.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BtnClear.Location = new System.Drawing.Point(578, 288);
            this.BtnClear.Name = "BtnClear";
            this.BtnClear.Size = new System.Drawing.Size(100, 35);
            this.BtnClear.TabIndex = 3;
            this.BtnClear.Text = "关闭窗口";
            this.BtnClear.UseVisualStyleBackColor = true;
            this.BtnClear.Click += new System.EventHandler(this.BtnClear_Click);
            // 
            // eventLog1
            // 
            this.eventLog1.SynchronizingObject = this;
            // 
            // button_Clear
            // 
            this.button_Clear.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_Clear.Location = new System.Drawing.Point(433, 288);
            this.button_Clear.Name = "button_Clear";
            this.button_Clear.Size = new System.Drawing.Size(100, 35);
            this.button_Clear.TabIndex = 4;
            this.button_Clear.Text = "清空曲线";
            this.button_Clear.UseVisualStyleBackColor = true;
            this.button_Clear.Click += new System.EventHandler(this.button_Clear_Click);
            // 
            // hslCurve1
            // 
            this.hslCurve1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.hslCurve1.ColorLinesAndText = System.Drawing.Color.White;
            this.hslCurve1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.hslCurve1.IsRenderRightCoordinate = false;
            this.hslCurve1.Location = new System.Drawing.Point(0, 0);
            this.hslCurve1.Name = "hslCurve1";
            this.hslCurve1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.hslCurve1.Size = new System.Drawing.Size(703, 282);
            this.hslCurve1.StrechDataCountMax = 30;
            this.hslCurve1.TabIndex = 5;
            this.hslCurve1.ValueMaxLeft = 50F;
            this.hslCurve1.ValueMinLeft = -10F;
            this.hslCurve1.ValueSegment = 6;
            // 
            // FormChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(704, 328);
            this.Controls.Add(this.hslCurve1);
            this.Controls.Add(this.button_Clear);
            this.Controls.Add(this.BtnClear);
            this.Controls.Add(this.LblCtrlTimeShow);
            this.Controls.Add(this.LblFlucShow);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormChart";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "温度实时曲线";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TemperatureChart_FormClosing);
            this.Load += new System.EventHandler(this.TemperatureChart_Load);
            ((System.ComponentModel.ISupportInitialize)(this.eventLog1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label LblCtrlTimeShow;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label LblFlucShow;
        private System.Windows.Forms.Button BtnClear;
        private System.Diagnostics.EventLog eventLog1;
        private System.Windows.Forms.Button button_Clear;
        private System.Windows.Forms.Timer timer1;
        private HslControls.HslCurve hslCurve1;
    }
}