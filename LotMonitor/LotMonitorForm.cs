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
    public partial class IotMonitorForm : Form
    {
        HslControls.HslSwitch[] switchRyM = new HslControls.HslSwitch[8];
        HslControls.HslSwitch[] switchRyS = new HslControls.HslSwitch[8];

        TextBox[] textBoxes_srType = new TextBox[6];
        TextBox[] textBoxes_srVersion = new TextBox[6];
        TextBox[] textBoxes_srSerial = new TextBox[6];

        // 状态灯闪烁
        Bitmap mBmpIot;
        private bool flp = false;
        private Timer timPic = new Timer();

        public IotMonitorForm()
        {
            InitializeComponent();

            switchRyM[0] = this.hslSwitch_RelayM0;
            switchRyM[1] = this.hslSwitch_RelayM1;
            switchRyM[2] = this.hslSwitch_RelayM2;
            switchRyM[3] = this.hslSwitch_RelayM3;
            switchRyM[4] = this.hslSwitch_RelayM4;
            switchRyM[5] = this.hslSwitch_RelayM5;
            switchRyM[6] = this.hslSwitch_RelayM6;
            switchRyM[7] = this.hslSwitch_RelayM7;

            switchRyS[0] = this.hslSwitch_RelayS0;
            switchRyS[1] = this.hslSwitch_RelayS1;
            switchRyS[2] = this.hslSwitch_RelayS2;
            switchRyS[3] = this.hslSwitch_RelayS3;
            switchRyS[4] = this.hslSwitch_RelayS4;
            switchRyS[5] = this.hslSwitch_RelayS5;
            switchRyS[6] = this.hslSwitch_RelayS6;
            switchRyS[7] = this.hslSwitch_RelayS7;

            textBoxes_srType[0] = this.textBox_type0;
            textBoxes_srVersion[0] = this.textBox_version0;
            textBoxes_srSerial[0] = this.textBox_serial0;
            textBoxes_srType[1] = this.textBox_type1;
            textBoxes_srVersion[1] = this.textBox_version1;
            textBoxes_srSerial[1] = this.textBox_serial1;
            textBoxes_srType[2] = this.textBox_type2;
            textBoxes_srVersion[2] = this.textBox_version2;
            textBoxes_srSerial[2] = this.textBox_serial2;
            textBoxes_srType[3] = this.textBox_type3;
            textBoxes_srVersion[3] = this.textBox_version3;
            textBoxes_srSerial[3] = this.textBox_serial3;
            textBoxes_srType[4] = this.textBox_type4;
            textBoxes_srVersion[4] = this.textBox_version4;
            textBoxes_srSerial[4] = this.textBox_serial4;
            textBoxes_srType[5] = this.textBox_type5;
            textBoxes_srVersion[5] = this.textBox_version5;
            textBoxes_srSerial[5] = this.textBox_serial5;

            mBmpIot = new Bitmap(pictureBox_iot.Width, pictureBox_iot.Height);
            timPic.Interval = 500;
            timPic.Tick += TimPic_Tick;
            timPic.Start();
        }

        private void IotMonitorForm_Shown(object sender, EventArgs e)
        {
            setupIotClient();
        }

        ///////////////////////////////////////////////////
        // 状态指示灯闪烁
        private void TimPic_Tick(object sender, EventArgs e)
        {
            Graphics mGhpIot = Graphics.FromImage(mBmpIot);
            if (flp)
            {
                mGhpIot.Clear(SystemColors.Control);
                flp = false;
            }
            else
            {
                mGhpIot.Clear(isMqttConnected ? Color.Green : Color.Red);
                flp = true;
            }
            pictureBox_iot.Image = mBmpIot;
        }

        // 点击继电器开关 - 任意一个
        private void hslSwitch_RelayM0_Click_1(object sender, EventArgs e)
        {

        }
    }
}
