using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace TempControl
{
    public partial class FormDebug : Form
    {
        private Device.RunningParamStruct paramAll;

        private TextBox[] devParam = new TextBox[18];
        private TextBox tx = null;

        public FormDebug(Device.RunningParamStruct pms)
        {
            InitializeComponent();
            paramAll = pms;
            devParam[0] = textBox1;
            devParam[1] = textBox2;
            devParam[2] = textBox3;
            devParam[3] = textBox4;
            devParam[4] = textBox5;
            devParam[5] = textBox6;
            devParam[6] = textBox7;
            devParam[7] = textBox8;
            devParam[8] = textBox9;
            devParam[9] = textBox10;
            devParam[10] = textBox11;
            devParam[11] = textBox12;
            devParam[12] = textBox13;
            devParam[13] = textBox14;
            devParam[14] = textBox15;
            devParam[15] = textBox16;
            devParam[16] = textBox17;
            devParam[17] = textBox18;

            //comboBox1.Items
            string[] portNames = SerialPort.GetPortNames();
            comboBox_tp1.Items.AddRange(portNames);
            comboBox_tp2.Items.AddRange(portNames);
            comboBox_ry1.Items.AddRange(portNames);
            comboBox_ry2.Items.AddRange(portNames);
        }
        private void FormDebug_Load(object sender, EventArgs e)
        {
            textBox1.Text = paramAll.steadyTimeSec.ToString("0");
            textBox2.Text = paramAll.bridgeSteadyTimeSec.ToString("0");
            textBox3.Text = paramAll.flucValue.ToString("0.0000");
            textBox4.Text = paramAll.controlTempThr.ToString("0.0000");
            textBox5.Text = paramAll.tempNotUpOrDownFaultTimeSec.ToString("0");
            //textBox6.Text = paramAll.tempNotUpOrDwonFaultThrHigh.ToString("0.0000");
            textBox7.Text = paramAll.flucFaultTimeSec.ToString("0");
            textBox8.Text = paramAll.flucFaultThr.ToString("0.0000");
            textBox9.Text = paramAll.tempBiasFaultThr.ToString("0.0000");
            textBox10.Text = paramAll.tempMaxValue.ToString("0.0000");
            textBox11.Text = paramAll.tempMinValue.ToString("0.0000");
            //textBox12.Text = paramAll.tpBridge.tpBridgeChannel.ToString("0");
            //textBox13.Text = paramAll.tpBridge.tpBridgeReadInterval.ToString("0");
            //textBox14.Text = paramAll.subCoolAndCircleShutdownThr.ToString("0.0000");
            textBox15.Text = paramAll.tempDownCoolFShoutdownDevision.ToString("0.0000");
            textBox16.Text = paramAll.tempDownCoolFShoutdownHot.ToString("0.0000");
            textBox17.Text = paramAll.tempDownCoolFShoutdownCool.ToString("0.0000");
            //textBox18.Text = paramAll.tempNotUpOrDwonFaultThrLow.ToString("0.0000");

            comboBox_sort.SelectedIndex = paramAll.sort == "ascend" ? 0 : 1;
            comboBox_elect.SelectedIndex = paramAll.ryElecEnable == true ? 0 : 1;
            comboBox_tp1.SelectedIndex = comboBox_tp1.Items.IndexOf(paramAll.portTp1);
            comboBox_tp2.SelectedIndex = comboBox_tp2.Items.IndexOf(paramAll.portTp2);
            comboBox_ry1.SelectedIndex = comboBox_ry1.Items.IndexOf(paramAll.portRy1);
            comboBox_ry2.SelectedIndex = comboBox_ry2.Items.IndexOf(paramAll.portRy1);
        }


        private void BntUpdate_Click(object sender, EventArgs e)
        {
            float[] paramCache = new float[18];
            // 设置温控设备参数
            for (int i = 0; i < 18; i++)
            {
                float newVal = 0.0f;

                if (float.TryParse(this.devParam[i].Text, out newVal) != true)
                {
                    // 参数数据格式错误哦
                    MessageBox.Show("参数 " + devParam[i].Text + " 格式错误!");
                    return;
                }

                // 将参数写入参数设置缓存
                paramCache[i] = newVal;
            }

            paramAll.steadyTimeSec = (int)paramCache[0];
            paramAll.bridgeSteadyTimeSec = (int)paramCache[1];
            paramAll.flucValue = paramCache[2];
            paramAll.controlTempThr = paramCache[3];
            paramAll.tempNotUpOrDownFaultTimeSec = (int)paramCache[4];
            //paramAll.tempNotUpOrDwonFaultThrHigh = paramCache[5];
            paramAll.flucFaultTimeSec = (int)paramCache[6];
            paramAll.flucFaultThr = paramCache[7];
            paramAll.tempBiasFaultThr = paramCache[8];
            paramAll.tempMaxValue = paramCache[9];
            paramAll.tempMinValue = paramCache[10];
            //paramAll.tpBridge.tpBridgeChannel = (int)paramCache[11];
            //paramAll.tpBridge.tpBridgeReadInterval = (int)paramCache[12];
            // paramAll.subCoolAndCircleShutdownThr = paramCache[13];
            paramAll.tempDownCoolFShoutdownDevision = paramCache[14];
            paramAll.tempDownCoolFShoutdownHot = paramCache[15];
            paramAll.tempDownCoolFShoutdownCool = paramCache[16];
            //paramAll.tempNotUpOrDwonFaultThrHigh = paramCache[17];

            if (comboBox_sort.SelectedIndex == 0)
            {
                paramAll.sort = "ascend";
            }
            else
            {
                paramAll.sort = "descend";
            }

            if (comboBox_elect.SelectedIndex == 0)
            {
                paramAll.ryElecEnable = true;
            }
            else
            {
                paramAll.ryElecEnable = false;
            }

            // 写入到文本中
            // 相关参数
            string configFilePath = @"./config.ini";
            Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "SteadyTimeSec", paramAll.steadyTimeSec.ToString("0"));
            Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "bridgeSteadyTimeSec", paramAll.bridgeSteadyTimeSec.ToString("0"));
            Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "FlucValue", paramAll.flucValue.ToString("0.0000"));
            Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "controlTempThr", paramAll.controlTempThr.ToString("0.0000"));
            Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "tempNotUpOrDownFaultTimeSec", paramAll.tempNotUpOrDownFaultTimeSec.ToString("0"));
            //Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "tempNotUpOrDwonFaultThrHigh", paramAll.tempNotUpOrDwonFaultThrHigh.ToString("0.0000"));
            //Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "tempNotUpOrDwonFaultThrLow", paramAll.tempNotUpOrDwonFaultThrLow.ToString("0.0000"));
            Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "flucFaultTimeSec", paramAll.flucFaultTimeSec.ToString("0"));
            Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "flucFaultThr", paramAll.flucFaultThr.ToString("0.0000"));
            Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "tempBiasFaultThr", paramAll.tempBiasFaultThr.ToString("0.0000"));
            Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "tempMaxValue", paramAll.tempMaxValue.ToString("0.0000"));
            Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "tempMinValue", paramAll.tempMinValue.ToString("0.0000"));
            //Utils.IniReadWrite.INIWriteValue(configFilePath, "TpBridge", "tpBridgeChannel", paramAll.tpBridge.tpBridgeChannel.ToString("0"));
            //Utils.IniReadWrite.INIWriteValue(configFilePath, "TpBridge", "tpBridgeReadInterval", paramAll.tpBridge.tpBridgeReadInterval.ToString("0"));
            //Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "subCircleShutdownThr", paramAll.subCoolAndCircleShutdownThr.ToString("0.0000"));
            Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "tempDownCoolFShoutdownDevision", paramAll.tempDownCoolFShoutdownDevision.ToString("0.0000"));
            Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "tempDownCoolFShoutdownHot", paramAll.tempDownCoolFShoutdownHot.ToString("0.0000"));
            Utils.IniReadWrite.INIWriteValue(configFilePath, "Paramters", "tempDownCoolFShoutdownCool", paramAll.tempDownCoolFShoutdownCool.ToString("0.0000"));

            Utils.IniReadWrite.INIWriteValue(configFilePath, "Others", "sort", paramAll.sort);
            if (paramAll.ryElecEnable) Utils.IniReadWrite.INIWriteValue(configFilePath, "Others", "ryElecEnable", "Enable");
            else Utils.IniReadWrite.INIWriteValue(configFilePath, "Others", "ryElecEnable", "Disable");
        }

        private void BntRead_Click(object sender, EventArgs e)
        {
            textBox1.Text = paramAll.steadyTimeSec.ToString("0");
            textBox2.Text = paramAll.bridgeSteadyTimeSec.ToString("0");
            textBox3.Text = paramAll.flucValue.ToString("0.0000");
            textBox4.Text = paramAll.controlTempThr.ToString("0.0000");
            textBox5.Text = paramAll.tempNotUpOrDownFaultTimeSec.ToString("0");
            //textBox6.Text = paramAll.tempNotUpOrDwonFaultThrHigh.ToString("0.0000");
            textBox7.Text = paramAll.flucFaultTimeSec.ToString("0");
            textBox8.Text = paramAll.flucFaultThr.ToString("0.0000");
            textBox9.Text = paramAll.tempBiasFaultThr.ToString("0.0000");
            textBox10.Text = paramAll.tempMaxValue.ToString("0.0000");
            textBox11.Text = paramAll.tempMinValue.ToString("0.0000");
            //textBox12.Text = paramAll.tpBridge.tpBridgeChannel.ToString("0");
            //textBox13.Text = paramAll.tpBridge.tpBridgeReadInterval.ToString("0");
            //textBox14.Text = paramAll.subCoolAndCircleShutdownThr.ToString("0.0000");
            textBox15.Text = paramAll.tempDownCoolFShoutdownDevision.ToString("0.0000");
            textBox16.Text = paramAll.tempDownCoolFShoutdownHot.ToString("0.0000");
            textBox17.Text = paramAll.tempDownCoolFShoutdownCool.ToString("0.0000");
            //textBox18.Text = paramAll.tempNotUpOrDwonFaultThrLow.ToString("0.0000");

            if (paramAll.sort == "ascend")
            {
                comboBox_sort.SelectedIndex = 0;
            }
            else
            {
                comboBox_sort.SelectedIndex = 1;
            }

            if (paramAll.ryElecEnable == true)
            {
                comboBox_elect.SelectedIndex = 0;
            }
            else
            {
                comboBox_elect.SelectedIndex = 1;
            }

        }

        private void button10_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void button9_Click(object sender, EventArgs e)
        {
            if (tx != null)
            {
                if (tx.Text.Length == 1 && tx.Text == "0")
                {
                    tx.Text = "9";
                }
                else if (tx.Text.Length == 2 && tx.Text == "-0")
                {
                    tx.Text = "-9";
                }
                else
                {
                    tx.Text += "9";
                }
            }
            else
            {
                MessageBox.Show("请先选定设定项!");
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (tx != null)
            {
                if (tx.Text.Length == 1 && tx.Text == "0")
                {
                    tx.Text = "8";
                }
                else if (tx.Text.Length == 2 && tx.Text == "-0")
                {
                    tx.Text = "-8";
                }
                else
                {
                    tx.Text += "8";
                }
            }
            else
            {
                MessageBox.Show("请先选定设定项!");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (tx != null)
            {
                if (tx.Text.Length == 1 && tx.Text == "0")
                {
                    tx.Text = "7";
                }
                else if (tx.Text.Length == 2 && tx.Text == "-0")
                {
                    tx.Text = "-7";
                }
                else
                {
                    tx.Text += "7";
                }
            }
            else
            {
                MessageBox.Show("请先选定设定项!");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (tx != null)
            {
                if (tx.Text.Length == 1 && tx.Text == "0")
                {
                    tx.Text = "6";
                }
                else if (tx.Text.Length == 2 && tx.Text == "-0")
                {
                    tx.Text = "-6";
                }
                else
                {
                    tx.Text += "6";
                }
            }
            else
            {
                MessageBox.Show("请先选定设定项!");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (tx != null)
            {
                if (tx.Text.Length == 1 && tx.Text == "0")
                {
                    tx.Text = "5";
                }
                else if (tx.Text.Length == 2 && tx.Text == "-0")
                {
                    tx.Text = "-5";
                }
                else
                {
                    tx.Text += "5";
                }
            }
            else
            {
                MessageBox.Show("请先选定设定项!");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (tx != null)
            {
                if (tx.Text.Length == 1 && tx.Text == "0")
                {
                    tx.Text = "4";
                }
                else if (tx.Text.Length == 2 && tx.Text == "-0")
                {
                    tx.Text = "-4";
                }
                else
                {
                    tx.Text += "4";
                }
            }
            else
            {
                MessageBox.Show("请先选定设定项!");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (tx != null)
            {
                if (tx.Text.Length == 1 && tx.Text == "0")
                {
                    tx.Text = "3";
                }
                else if (tx.Text.Length == 2 && tx.Text == "-0")
                {
                    tx.Text = "-3";
                }
                else
                {
                    tx.Text += "3";
                }
            }
            else
            {
                MessageBox.Show("请先选定设定项!");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (tx != null)
            {
                if (tx.Text.Length == 1 && tx.Text == "0")
                {
                    tx.Text = "2";
                }
                else if (tx.Text.Length == 2 && tx.Text == "-0")
                {
                    tx.Text = "-2";
                }
                else
                {
                    tx.Text += "2";
                }
            }
            else
            {
                MessageBox.Show("请先选定设定项!");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (tx != null)
            {
                if (tx.Text.Length == 1 && tx.Text == "0")
                {
                    tx.Text = "1";
                }
                else if (tx.Text.Length == 2 && tx.Text == "-0")
                {
                    tx.Text = "-1";
                }
                else
                {
                    tx.Text += "1";
                }

            }
            else
            {
                MessageBox.Show("请先选定设定项!");
            }
        }

        private void button0_Click(object sender, EventArgs e)
        {
            if (tx != null)
            {
                if (tx.Text.Length == 2 && tx.Text == "-0")
                {

                }
                else if (tx.Text.Length != 1 || tx.Text == "-" || int.Parse(tx.Text) != 0)
                {
                    tx.Text += "0";
                }
            }
            else
            {
                MessageBox.Show("请先选定设定项!");
            }
        }

        private void buttonNegtive_Click(object sender, EventArgs e)
        {
            if (tx != null)
            {
                if (tx.Text == "")
                {
                    tx.Text = "-";
                }
                else if (tx.Text[0] == '-')
                {
                    tx.Text = tx.Text.Remove(0, 1);
                }
                else
                {
                    tx.Text = tx.Text.Insert(0, "-");
                }
            }
            else
            {
                MessageBox.Show("请先选定设定项!");
            }
        }

        private void buttonPoint_Click(object sender, EventArgs e)
        {
            if (tx != null)
            {
                if (!tx.Text.Contains("."))
                {
                    if (tx.Text.Length == 0)
                    {
                        tx.Text = "0.";
                    }
                    else if (tx.Text.Length == 1 && tx.Text == "-")
                    {
                        tx.Text = "-0.";
                    }
                    else
                    {
                        tx.Text += ".";
                    }
                }
            }
            else
            {
                MessageBox.Show("请先选定设定项!");
            }
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            if (tx != null)
            {
                if (tx.Text.Length > 0)
                    tx.Text = tx.Text.Substring(0, tx.Text.Length - 1);
            }
            //else
            //{
            //    MessageBox.Show("请先选定设定项!");
            //}
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            if (tx != null)
            {
                tx.Text = "";
            }
            //else
            //{
            //    MessageBox.Show("请先选定设定项!");
            //}
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (tx != null)
            {
                tx.BackColor = System.Drawing.SystemColors.Control;
            }

            tx = this.textBox1;
            tx.BackColor = System.Drawing.SystemColors.Window;
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (tx != null)
            {
                tx.BackColor = System.Drawing.SystemColors.Control;
            }

            tx = this.textBox2;
            tx.BackColor = System.Drawing.SystemColors.Window;
        }

        private void textBox3_Enter(object sender, EventArgs e)
        {
            if (tx != null)
            {
                tx.BackColor = System.Drawing.SystemColors.Control;
            }

            tx = this.textBox3;
            tx.BackColor = System.Drawing.SystemColors.Window;
        }

        private void textBox4_Enter(object sender, EventArgs e)
        {
            if (tx != null)
            {
                tx.BackColor = System.Drawing.SystemColors.Control;
            }

            tx = this.textBox4;
            tx.BackColor = System.Drawing.SystemColors.Window;
        }

        private void textBox5_Enter(object sender, EventArgs e)
        {
            if (tx != null)
            {
                tx.BackColor = System.Drawing.SystemColors.Control;
            }

            tx = this.textBox5;
            tx.BackColor = System.Drawing.SystemColors.Window;
        }

        private void textBox6_Enter(object sender, EventArgs e)
        {
            if (tx != null)
            {
                tx.BackColor = System.Drawing.SystemColors.Control;
            }

            tx = this.textBox6;
            tx.BackColor = System.Drawing.SystemColors.Window;
        }

        private void textBox7_Enter(object sender, EventArgs e)
        {
            if (tx != null)
            {
                tx.BackColor = System.Drawing.SystemColors.Control;
            }

            tx = this.textBox7;
            tx.BackColor = System.Drawing.SystemColors.Window;
        }

        private void textBox8_Enter(object sender, EventArgs e)
        {
            if (tx != null)
            {
                tx.BackColor = System.Drawing.SystemColors.Control;
            }

            tx = this.textBox8;
            tx.BackColor = System.Drawing.SystemColors.Window;
        }

        private void textBox9_Enter(object sender, EventArgs e)
        {
            if (tx != null)
            {
                tx.BackColor = System.Drawing.SystemColors.Control;
            }

            tx = this.textBox9;
            tx.BackColor = System.Drawing.SystemColors.Window;
        }

        private void textBox10_Enter(object sender, EventArgs e)
        {
            if (tx != null)
            {
                tx.BackColor = System.Drawing.SystemColors.Control;
            }

            tx = this.textBox10;
            tx.BackColor = System.Drawing.SystemColors.Window;
        }

        private void textBox11_Enter(object sender, EventArgs e)
        {
            if (tx != null)
            {
                tx.BackColor = System.Drawing.SystemColors.Control;
            }

            tx = this.textBox11;
            tx.BackColor = System.Drawing.SystemColors.Window;
        }

        private void textBox12_Enter(object sender, EventArgs e)
        {
            if (tx != null)
            {
                tx.BackColor = System.Drawing.SystemColors.Control;
            }

            tx = this.textBox12;
            tx.BackColor = System.Drawing.SystemColors.Window;
        }

        private void textBox13_Enter(object sender, EventArgs e)
        {
            if (tx != null)
            {
                tx.BackColor = System.Drawing.SystemColors.Control;
            }

            tx = this.textBox13;
            tx.BackColor = System.Drawing.SystemColors.Window;
        }

        private void textBox14_Enter(object sender, EventArgs e)
        {
            if (tx != null)
            {
                tx.BackColor = System.Drawing.SystemColors.Control;
            }

            tx = this.textBox14;
            tx.BackColor = System.Drawing.SystemColors.Window;
        }

        private void textBox15_Enter(object sender, EventArgs e)
        {
            if (tx != null)
            {
                tx.BackColor = System.Drawing.SystemColors.Control;
            }

            tx = this.textBox15;
            tx.BackColor = System.Drawing.SystemColors.Window;
        }

        private void textBox16_Enter(object sender, EventArgs e)
        {
            if (tx != null)
            {
                tx.BackColor = System.Drawing.SystemColors.Control;
            }

            tx = this.textBox16;
            tx.BackColor = System.Drawing.SystemColors.Window;
        }

        private void textBox17_Enter(object sender, EventArgs e)
        {
            if (tx != null)
            {
                tx.BackColor = System.Drawing.SystemColors.Control;
            }

            tx = this.textBox17;
            tx.BackColor = System.Drawing.SystemColors.Window;
        }

        private void textBox18_Enter(object sender, EventArgs e)
        {
            if (tx != null)
            {
                tx.BackColor = System.Drawing.SystemColors.Control;
            }

            tx = this.textBox18;
            tx.BackColor = System.Drawing.SystemColors.Window;
        }
    }
}
