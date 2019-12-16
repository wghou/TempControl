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

        private TextBox[] devParam = new TextBox[7];
        private TextBox tx = null;

        public FormDebug(Device.RunningParamStruct pms)
        {
            InitializeComponent();
            paramAll = pms;
            devParam[0] = textBox0;
            devParam[1] = textBox1;
            devParam[2] = textBox2;
            devParam[3] = textBox3;
            devParam[4] = textBox4;
            devParam[5] = textBox5;
            devParam[6] = textBox6;
        }

        private void loadFromParam()
        {
            textBox0.Text = paramAll.readTempIntervalSec.ToString("0");
            textBox1.Text = paramAll.flucValue.ToString("0.000");
            textBox1.Text = paramAll.controlTempThr.ToString("0.0000");
            textBox2.Text = paramAll.controlTempThr.ToString("0.0000");
            textBox3.Text = paramAll.tempMaxValue.ToString("0.0000");
            textBox4.Text = paramAll.tempMinValue.ToString("0.0000");
            textBox5.Text = paramAll.standHoldCounts.ToString("0");
            textBox6.Text = paramAll.addGasHoldCounts.ToString("0");
            comboBox_sort.SelectedIndex = paramAll.sort == "ascend" ? 0 : 1;
        }
        private void FormDebug_Load(object sender, EventArgs e)
        {
            loadFromParam();
        }


        private void BntUpdate_Click(object sender, EventArgs e)
        {
            float[] paramCache = new float[7];
            // 设置温控设备参数
            for (int i = 0; i < 7; i++)
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

            if(paramCache[5] <=0 || paramCache[6] <= 0)
            {
                MessageBox.Show("参数格式错误，请检查！");
                return;
            }

            paramAll.readTempIntervalSec = (int)paramCache[0];
            paramAll.flucValue = paramCache[1];
            paramAll.controlTempThr = paramCache[2];
            paramAll.tempMaxValue = paramCache[3];
            paramAll.tempMinValue = paramCache[4];
            paramAll.standHoldCounts = (uint)paramCache[5];
            paramAll.addGasHoldCounts = (uint)paramCache[6];
            
            if (comboBox_sort.SelectedIndex == 0)
            {
                paramAll.sort = "ascend";
            }
            else
            {
                paramAll.sort = "descend";
            }

            // 写入到文本中
            // 相关参数
            if (paramAll.WriteValueConfig(@"./config.ini") == false)
            {
                MessageBox.Show("保存参数失败！");
            }
        }

        private void BntRead_Click(object sender, EventArgs e)
        {
            if(paramAll.ReadValueConfig(@"./config.ini") == false)
            {
                MessageBox.Show("读取参数失败！");
            }

            loadFromParam();
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
                if (tx == textBox0 || tx == textBox5 || tx == textBox6) return;

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

        private void textBox_Enter(object sender, EventArgs e)
        {
            if (tx != null)
            {
                tx.BackColor = System.Drawing.SystemColors.Control;
            }

            tx = (TextBox)sender;
            tx.BackColor = System.Drawing.SystemColors.Window;
        }
    }
}
