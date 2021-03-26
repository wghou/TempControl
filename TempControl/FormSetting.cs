using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using NLog;

namespace TempControl
{
    public partial class FormSetting : Form
    {
        private static readonly Logger nlogger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 用于存放所有对象
        /// </summary>
        private Device.TempDevice tpDev;
        private Device.DeviceStateM devicesAll;

        /// <summary>
        /// 存放参数控件数组
        /// </summary>
        private TextBox[] tpParam = new TextBox[7];

        private TextBox tx = null;

        // 窗体构造函数
        public FormSetting(Device.TempDevice dev, Device.DeviceStateM devAll)
        {
            InitializeComponent();

            // 温控设备对象
            tpDev = dev;
            devicesAll = devAll;

            // 温控设备参数控件
            tpParam[0] = TxtTempSet;
            tpParam[1] = TxtTempCorrect;
            tpParam[2] = TxtLeadAdjust;
            tpParam[3] = TxtFuzzy;
            tpParam[4] = TxtRatio;
            tpParam[5] = TxtIntegral;
            tpParam[6] = TxtPower;
        }


        // 窗体加载事件 - 处理函数
        // 从 TempDevice.tpParam 中读取参数值
        // 在 TempDevice 初始化的过程中，已经将硬件中的参数读取到了 tpParam 中，因此，不再直接从硬件中读取参数
        private void FormSetting_Load(object sender, EventArgs e)
        {
            // 注册温控设备参数更新 / 设置事件处理函数
            if (this.Name == "FormSettingM")
            {
                this.devicesAll.TempDeviceMParamUpdatedEvent += DevicesAll_TempDeviceParamUpdatedEvent;
            }
            else
            {
                this.devicesAll.TempDeviceSParamUpdatedEvent += DevicesAll_TempDeviceParamUpdatedEvent;
            }
            
            // 从硬件设备读取参数
            //TempGetSetParamHandler getTempParam = new TempGetSetParamHandler(this.tpDev.UpdateParamFromDevice);
            //getTempParam.BeginInvoke(null, null);
            for (int i = 0; i < tpParam.Length; i++)
            {
                tpParam[i].Text = tpDev.tpParam[i].ToString(tpDev.tpParamFormat[i]);
            }
        }

        private void DevicesAll_TempDeviceParamUpdatedEvent(Device.TempProtocol.Err_t err, float[] param)
        {
            this.BeginInvoke(new EventHandler(delegate
            {
                // 从 TempDevice.tpParam 中读取参数值
                for (int i = 0; i < tpParam.Length; i++)
                {
                    tpParam[i].Text = param[i].ToString(tpDev.tpParamFormat[i]);
                }
            }));

            // 从下位机读取参数失败
            if (err != Device.TempProtocol.Err_t.NoError)
            {
                this.BeginInvoke(new EventHandler(delegate
                {
                    MessageBox.Show("从温控设备更新参数失败! \r错误状态：" + err.ToString());
                }));

                nlogger.Error("从 " + tpDev.tpDeviceName + " 中读取温控设备的参数失败  ErrorCode: " + err.ToString());
            }
            else
            {
                MessageBox.Show("从温控设备更新参数成功!");
            }
        }


        // 窗体关闭事件 - 处理函数
        // 设置窗口关闭时，注销相应的事件
        private void FormSetting_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 注销温控设备参数更新 / 设置事件处理函数
            if (this.Name == "FormSettingM")
            {
                this.devicesAll.TempDeviceMParamUpdatedEvent -= DevicesAll_TempDeviceParamUpdatedEvent;
            }
            else
            {
                this.devicesAll.TempDeviceSParamUpdatedEvent -= DevicesAll_TempDeviceParamUpdatedEvent;
            }
        }


        /// <summary>读取温控设备参数 - 委托 - 用于开辟新的线程读取设备参数 </summary>
        private delegate void TempGetSetParamHandler(bool errCnt);

        // 按键 click 事件 - 处理函数
        // 参数读取按键
        private void BntRead_Click(object sender, EventArgs e)
        {
            // 从硬件设备读取参数
            TempGetSetParamHandler getTempParam;
            if (this.Name == "FormSettingM")
            {
                getTempParam = new TempGetSetParamHandler(this.devicesAll.ReadTempDeviceM);
            }
            else
            {
                getTempParam = new TempGetSetParamHandler(this.devicesAll.ReadTempDeviceS);
            }
            
            getTempParam.BeginInvoke(false, null, null);

            Utils.Logger.Op("点击 查询参数 按键，从 " + tpDev.tpDeviceName + " 中读取温控设备的参数!");
            nlogger.Info("点击 查询参数 按键，从 " + tpDev.tpDeviceName + " 中读取温控设备的参数!");
        }


        // 按键 click 事件 - 处理函数
        // 参数设置按键
        private void BntUpdate_Click(object sender, EventArgs e)
        {
            float[] paramCache = new float[7];
            // 设置温控设备参数
            for (int i = 0; i < 7; i++)
            {
                float newVal = 0.0f;

                if (float.TryParse(this.tpParam[i].Text, out newVal) != true)
                {
                    // 参数数据格式错误哦
                    MessageBox.Show("参数 " + tpDev.tpParamNames[i] + " 格式错误!");
                    return;
                }

                // 将参数写入参数设置缓存
                paramCache[i] = newVal;
            }

            // 限制温度点的设置范围
            if (paramCache[0] > devicesAll._runningParameters.tempMaxValue || paramCache[0] < devicesAll._runningParameters.tempMinValue)
            {
                MessageBox.Show("温度点超出界限 ( " + devicesAll._runningParameters.tempMinValue.ToString("0.0000") + " - " + devicesAll._runningParameters.tempMaxValue.ToString("0.0000") + " )，请检查!");
                return;
            }

            // 将参数写入温控设备缓存
            paramCache.CopyTo(tpDev.tpParamToSet, 0);

            // 向硬件设备更新参数
            TempGetSetParamHandler setTempParam;
            if (this.Name == "FormSettingM")
            {
                setTempParam = new TempGetSetParamHandler(this.devicesAll.WriteTempDeviceM);
            }
            else
            {
                setTempParam = new TempGetSetParamHandler(this.devicesAll.WriteTempDeviceS);
            }
            setTempParam.BeginInvoke(false, null, null);

            Utils.Logger.Op("点击 更新参数 按键，向 " + tpDev.tpDeviceName + " 中写入温控设备的参数!");
            nlogger.Info("点击 更新参数 按键，向 " + tpDev.tpDeviceName + " 中写入温控设备的参数!");
        }


        // 返回主界面
        private void button10_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        // 从数据库读取预留参数
        private void button_checkDataBase_Click(object sender, EventArgs e)
        {
            float val = 0.0f;
            if (!float.TryParse(TxtTempSet.Text, out val))
            {
                MessageBox.Show("温度设定值不正确，请重新输入");
                return;
            }
        }

        // 向数据库写入预留参数
        private void button_writeDataBase_Click(object sender, EventArgs e)
        {
            float val = 0.0f;
            float[] tpSet = new float[7];
            for (int i = 0; i < 7; i++)
            {
                if (!float.TryParse(tpParam[i].Text, out val))
                {
                    MessageBox.Show("参数设定格式错误!");
                    return;
                }
                tpSet[i] = val;
            }
        }



        #region 焦点
        // 焦点
        private void TxtTempSet_Enter(object sender, EventArgs e)
        {
            if (tx != null)
            {
                tx.BackColor = System.Drawing.SystemColors.Control;
            }

            tx = this.TxtTempSet;
            tx.BackColor = System.Drawing.SystemColors.Window;
        }

        private void TxtTempCorrect_Enter(object sender, EventArgs e)
        {
            if (tx != null)
            {
                tx.BackColor = System.Drawing.SystemColors.Control;
            }

            tx = this.TxtTempCorrect;
            tx.BackColor = System.Drawing.SystemColors.Window;
        }

        private void TxtLeadAdjust_Enter(object sender, EventArgs e)
        {
            if (tx != null)
            {
                tx.BackColor = System.Drawing.SystemColors.Control;
            }

            tx = this.TxtLeadAdjust;
            tx.BackColor = System.Drawing.SystemColors.Window;
        }

        private void TxtFuzzy_Enter(object sender, EventArgs e)
        {
            if (tx != null)
            {
                tx.BackColor = System.Drawing.SystemColors.Control;
            }

            tx = this.TxtFuzzy;
            tx.BackColor = System.Drawing.SystemColors.Window;
        }

        private void TxtRatio_Enter(object sender, EventArgs e)
        {
            if (tx != null)
            {
                tx.BackColor = System.Drawing.SystemColors.Control;
            }

            tx = this.TxtRatio;
            tx.BackColor = System.Drawing.SystemColors.Window;
        }

        private void TxtIntegral_Enter(object sender, EventArgs e)
        {
            if (tx != null)
            {
                tx.BackColor = System.Drawing.SystemColors.Control;
            }

            tx = this.TxtIntegral;
            tx.BackColor = System.Drawing.SystemColors.Window;
        }

        private void TxtPower_Enter(object sender, EventArgs e)
        {
            if (tx != null)
            {
                tx.BackColor = System.Drawing.SystemColors.Control;
            }

            tx = this.TxtPower;
            tx.BackColor = System.Drawing.SystemColors.Window;
        }

        #endregion


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
        // end

    }
}
