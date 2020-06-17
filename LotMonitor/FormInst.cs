using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IotMonitor
{
    public partial class FormInst : Form
    {
        IotMonitorForm fmParent;

        public int instIdx = -1;

        public FormInst(IotMonitorForm fm)
        {
            InitializeComponent();

            fmParent = fm;

            fmParent.InstDataReceivedEvent += FmParent_InstDataReceivedEvent;
        }

        private void FmParent_InstDataReceivedEvent(InstDevice.InstDataShow data)
        {
            try
            {
                this.BeginInvoke(new EventHandler(delegate
                {
                    textBox_info.Text = fmParent.instInfos[instIdx].InstType.ToString() + "  " + fmParent.instInfos[instIdx].instrumentId;
                }));

                if (data == null) return;

                if (data.InstIdx == this.instIdx)
                {
                    writeLog(data.dtTime.ToString("HH:mm:ss") + "  Tempt: " + data.Tempt.ToString("0.0000") + "   Conduct: " + data.Conduct.ToString("0.0000"),
                                    richTextBox_istVal, 50);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void FormInst_FormClosing(object sender, FormClosingEventArgs e)
        {
            fmParent.InstDataReceivedEvent -= FmParent_InstDataReceivedEvent;
        }

        private void writeLog(string log, RichTextBox box, int maxLength)
        {
            if (box == null) return;

            try
            {
                this.BeginInvoke(new EventHandler(delegate
                {
                    box.AppendText(log);
                    box.AppendText(Environment.NewLine);
                    box.ScrollToCaret();

                    if (box.Lines.Length > maxLength)
                    {
                        int moreLines = box.Lines.Length - maxLength;
                        string[] lines = box.Lines;
                        Array.Copy(lines, moreLines, lines, 0, maxLength);
                        Array.Resize(ref lines, maxLength);
                        box.Lines = lines;
                        box.SelectionStart = box.Text.Length;
                        box.SelectionLength = 0;
                        //box.Focus();
                    }
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void FormInst_Shown(object sender, EventArgs e)
        {
            textBox_info.Text = fmParent.instInfos[instIdx].InstType.ToString() + "  " + fmParent.instInfos[instIdx].instrumentId;
            this.Text = this.Name;
        }
    }
}
