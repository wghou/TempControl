using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Device;
using InstDevice;

namespace TempControl
{
    public partial class FormInst : Form
    {
        // 
        private DeviceStateM _device;

        public FormInst(DeviceStateM dev)
        {
            InitializeComponent();

            _device = dev;

            comboBox_port.DataSource = portList;
            comboBox_port.ValueMember = "Id";
            comboBox_port.DisplayMember = "Name";
            comboBox_inst.DataSource = instList;
            comboBox_inst.ValueMember = "Id";
            comboBox_inst.DisplayMember = "Name";
        }

        public class PortInfoItem
        {
            public int Id { get; internal set; }
            public string Name { get; internal set; } = "";
        }

        public class InstInfoItem
        {
            public int Id { get; internal set; }
            public string Name { get; internal set; } = "";
        }

        List<PortInfoItem> portList = new List<PortInfoItem>();
        List<InstInfoItem> instList = new List<InstInfoItem>();

        private void loadInstInfo()
        {
            foreach(var itm in _device._instDevices)
            {
                PortInfoItem pt = new PortInfoItem();
                pt.Id = _device._instDevices.IndexOf(itm);
                portList.Add(pt);
            }
        }

        /// 配置仪器端口
        private void checkBox_cfg_Click(object sender, EventArgs e)
        {

        }

        /// 取消仪器端口配置
        private void checkBox_cfgC_Click(object sender, EventArgs e)
        {

        }
    }
}
