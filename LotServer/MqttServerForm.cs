using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace LotServer
{
    public partial class MqttServerForm : Form
    {
        public MqttServerForm()
        {
            InitializeComponent();

            //this.WindowState = FormWindowState.Minimized;
            //this.Hide();
            //this.notifyIcon1.Visible = true;

            setupMqtt();

            ipAddressControl1.IPAddress = mqttServerUrl;
            textBox_port.Text = mqttPort.ToString();
            textBox_username.Text = mqttUsername;
            textBox_password.Text = mqttPassword;
        }

        private void MqttServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.ApplicationExitCall)
            {
                Application.Exit();

                mqttServer.StopAsync();
            }
            else
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
                this.Hide();
                this.notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            notifyIcon1.Visible = true;
            this.Show();
            this.Activate();
            this.WindowState = FormWindowState.Normal;
        }

        private void MqttServerForm_Shown(object sender, EventArgs e)
        {
            mqttServer.StartAsync(options);
        }

        private void checkBox_start_Click(object sender, EventArgs e)
        {
            if(this.checkBox_start.Text == "停止服务")
            {
                mqttServer.StopAsync();
            }
            else
            {
                mqttServer.StartAsync(options);
            }
        }

        private void checkBox_log_Click(object sender, EventArgs e)
        {

        }

        private void checkBox_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
