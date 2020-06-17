namespace IotMonitor
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
            this.richTextBox_istVal = new System.Windows.Forms.RichTextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.textBox_info = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // richTextBox_istVal
            // 
            this.richTextBox_istVal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_istVal.Location = new System.Drawing.Point(12, 70);
            this.richTextBox_istVal.Name = "richTextBox_istVal";
            this.richTextBox_istVal.Size = new System.Drawing.Size(412, 315);
            this.richTextBox_istVal.TabIndex = 50;
            this.richTextBox_istVal.Text = "";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label13.Location = new System.Drawing.Point(12, 51);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(56, 16);
            this.label13.TabIndex = 49;
            this.label13.Text = "仪器值";
            // 
            // textBox_info
            // 
            this.textBox_info.Location = new System.Drawing.Point(12, 12);
            this.textBox_info.Name = "textBox_info";
            this.textBox_info.Size = new System.Drawing.Size(412, 21);
            this.textBox_info.TabIndex = 51;
            // 
            // FormInst
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(436, 397);
            this.Controls.Add(this.textBox_info);
            this.Controls.Add(this.richTextBox_istVal);
            this.Controls.Add(this.label13);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormInst";
            this.Text = "FormInst";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormInst_FormClosing);
            this.Shown += new System.EventHandler(this.FormInst_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox_istVal;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBox_info;
    }
}