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
using Newtonsoft.Json.Linq;

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

            baudList.Add(new BaudRateItem() { Id = 0, baudRate = 4800 });
            baudList.Add(new BaudRateItem() { Id = 1, baudRate = 9600 });
            comboBox_baudRate.DataSource = baudList;
            comboBox_baudRate.ValueMember = "Id";
            comboBox_baudRate.DisplayMember = "Name";
            comboBox_baudRate.SelectedValue = 1;

            spIntervalList.Add(new SampleIntervalItem() { Id = 0, interval = 10 });
            spIntervalList.Add(new SampleIntervalItem() { Id = 1, interval = 11 });
            spIntervalList.Add(new SampleIntervalItem() { Id = 2, interval = 12 });
            spIntervalList.Add(new SampleIntervalItem() { Id = 3, interval = 13 });
            spIntervalList.Add(new SampleIntervalItem() { Id = 4, interval = 14 });
            comboBox_spInterval.DataSource = spIntervalList;
            comboBox_spInterval.ValueMember = "Id";
            comboBox_spInterval.DisplayMember = "Name";
            comboBox_spInterval.SelectedValue = 2;

            loadInstInfo();

            _device.InstDeviceInitedEvent += _device_InstDeviceInitedEvent;
        }

        /// <summary>
        /// 事件处理函数
        /// </summary>
        /// <param name="confOK"></param>
        private void _device_InstDeviceInitedEvent(bool confOK)
        {
            if (confOK == false)
            {
                MessageBox.Show("配置仪器端口过程中发生错误！");
            }
            else
            {
                MessageBox.Show("配置仪器成功！");
            }
        }

        /// <summary>
        /// 波特率 类
        /// </summary>
        public class BaudRateItem : IComparable
        {
            public int Id { get; internal set; } = -1;
            public string Name { get { return baudRate.ToString(); } }

            public int baudRate = 9600;

            public int CompareTo(object obj)
            {
                BaudRateItem other = obj as BaudRateItem;
                if (other.baudRate == baudRate) { return 0; }
                else if(other.baudRate > baudRate) { return 1; }
                else { return -1; }
            }
        }

        /// <summary>
        /// 采样间隔 - 类
        /// </summary>
        public class SampleIntervalItem : IComparable
        {
            public int Id { get; internal set; } = -1;
            public string Name { get { return interval.ToString(); } }

            public int interval = 10;

            public int CompareTo(object obj)
            {
                SampleIntervalItem other = obj as SampleIntervalItem;
                if (other.interval == interval) { return 0; }
                else if (other.interval > interval) { return 1; }
                else { return -1; }
            }
        }

        /// <summary>
        /// 端口号 类
        /// </summary>
        public class PortInfoItem : IComparable
        {
            public int Id { get; internal set; }
            public string Name {
                get {
                    if(obtained == true)
                    {
                        return portName + ":占用";
                    }
                    else
                    {
                        return portName + ":空闲";
                    }
                }
            }

            public string portName = "COM";
            public bool obtained = false;

            public int CompareTo(object obj)
            {
                PortInfoItem other = obj as PortInfoItem;
                if(other.portName == portName) { return 0; }
                else{ return -1; }
            }
        }

        /// <summary>
        /// 仪器信息 类
        /// </summary>
        public class InstInfoItem
        {
            public int Id { get; internal set; }
            public string Name
            {
                get
                {
                    if (initialized == true) { return instType + ":已配置"; }
                    else { return instType + ":未配置"; }
                }
            }

            public string instType = "Undefined";
            public int portId = -1;
            public int brId = -1;
            public bool initialized = false;
            public string instrumentID = "null";
        }

        List<PortInfoItem> portList = new List<PortInfoItem>();
        List<InstInfoItem> instList = new List<InstInfoItem>();
        List<BaudRateItem> baudList = new List<BaudRateItem>();
        List<SampleIntervalItem> spIntervalList = new List<SampleIntervalItem>();

        /// <summary>
        /// 从设备中读取最新的配置相关信息
        /// </summary>
        private void loadInstInfo()
        {
            portList.Clear();
            instList.Clear();

            for (int idx = 0; idx < _device._instPorts.Count; idx++)
            {
                try
                {
                    JObject chd = (JObject)_device._instPorts[idx];
                    PortInfoItem pt = new PortInfoItem();
                    pt.Id = idx;
                    pt.portName = chd["PortName"].ToString();
                    // pt.defaultBaudRate = chd["baudRate"].ToObject<int>();
                    portList.Add(pt);
                }
                catch
                {
                    continue;
                }
            }

            var rlt3 = spIntervalList.Where(p => p.interval == InstDeviceStateM<InstSTDData, InstInfoBase>.sampleIntervalSec);
            comboBox_spInterval.SelectedValue = (rlt3.Count() == 0) ? 4 : rlt3.First().Id;

            foreach (var itm in _device._instDevices)
            {
                InstInfoItem pt = new InstInfoItem();
                pt.Id = itm.InstIdx;
                pt.instType = itm.GetBasicInfo().InstType.ToString();
                var rlt = portList.Where(p => p.portName == itm.GetBasicInfo().PortName);
                if(rlt.Count() == 0)
                {
                    pt.portId = -1;
                }
                else
                {
                    pt.portId = rlt.First().Id;
                    portList[pt.portId].obtained = itm.Enable;
                }
                
                var rlt2 = baudList.Where(p => p.baudRate == itm.GetBasicInfo().BaudRate);
                pt.brId = (rlt2.Count() == 0) ? -1 : rlt2.First().Id;
                
                pt.initialized = itm.Enable;
                pt.instrumentID = itm.GetBasicInfo().instrumentId;
                instList.Add(pt);
            }

            textBox_testID.Text = _device._instDevices.Count <= 1 ? "null" : _device._instDevices[1].GetBasicInfo().testId;

            updateComboboxShow();
        }

        /// <summary>
        /// 更新 combobox 显示
        /// </summary>
        private void updateComboboxShow()
        {
            comboBox_port.DataSource = null;
            comboBox_port.DataSource = portList;
            comboBox_port.ValueMember = "Id";
            comboBox_port.DisplayMember = "Name";

            comboBox_inst.DataSource = null;
            comboBox_inst.DataSource = instList;
            comboBox_inst.ValueMember = "Id";
            comboBox_inst.DisplayMember = "Name";

            comboBox_spInterval.DataSource = null;
            comboBox_spInterval.DataSource = spIntervalList;
            comboBox_spInterval.ValueMember = "Id";
            comboBox_spInterval.DisplayMember = "Name";

            textBox_instID.Text = "";
            foreach(var itm in instList)
            {
                textBox_instID.Text += itm.instType + " : " + (itm.initialized ? portList[itm.portId].portName : "未配置") + " : " + baudList[itm.brId].baudRate.ToString() + "\r\n";
            }
        }

        /// <summary>
        /// 将 comboBox_inst 与 comboBox_port 进行关联
        /// </summary>
        private void linkedInst2Port()
        {
            if((comboBox_inst.SelectedItem as InstInfoItem).initialized == true)
            {
                comboBox_port.SelectedValue = (comboBox_inst.SelectedItem as InstInfoItem).portId;
                comboBox_baudRate.SelectedValue = (comboBox_inst.SelectedItem as InstInfoItem).brId;
                comboBox_port.Enabled = false;
                comboBox_baudRate.Enabled = false;
            }
            else
            {
                comboBox_port.Enabled = true;
                comboBox_baudRate.Enabled = true;
            }
        }

        /// 暂存仪器端口配置
        private void checkBox_cfg_Click(object sender, EventArgs e)
        {
            if (instList[(int)comboBox_inst.SelectedValue].initialized == true 
                || portList[(int)comboBox_port.SelectedValue].obtained == true)
            {
                MessageBox.Show("仪器已配置/端口被占用！");
            }
            else
            {
                instList[(int)comboBox_inst.SelectedValue].initialized = true;
                instList[(int)comboBox_inst.SelectedValue].portId = (int)comboBox_port.SelectedValue;
                instList[(int)comboBox_inst.SelectedValue].brId = (int)comboBox_baudRate.SelectedValue;

                portList[(int)comboBox_port.SelectedValue].obtained = true;

                linkedInst2Port();
                updateComboboxShow();
            }
        }

        /// 取消仪器端口配置
        private void checkBox_cfgC_Click(object sender, EventArgs e)
        {
            if (instList[(int)comboBox_inst.SelectedValue].initialized == false)
            {
                return;
            }
            else
            {
                instList[(int)comboBox_inst.SelectedValue].initialized = false;
                portList[instList[(int)comboBox_inst.SelectedValue].portId].obtained = false;
                comboBox_port.Enabled = true;
                comboBox_baudRate.Enabled = true;
                updateComboboxShow();
            }
        }

        /// <summary>
        /// 向设备写入最终配置信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox_set2dev_Click(object sender, EventArgs e)
        {
            bool confOK = true;
            foreach (var itm in instList)
            {
                if(itm.initialized == false) {
                    confOK &= _device._instDevices[itm.Id].DisableInstDevice();
                }
                else
                {
                    _device._instDevices[itm.Id].GetBasicInfo().PortName = portList[itm.portId].portName;
                    _device._instDevices[itm.Id].GetBasicInfo().BaudRate = baudList[itm.brId].baudRate;
                    confOK &= _device._instDevices[itm.Id].InitWithInfo();

                    // todo: 初始化并配置仪器
                    //if(_device._instDevices[itm.Id].GetBasicInfo().InstType != TypeInst.Undefined &&
                    //    _device._instDevices[itm.Id].GetBasicInfo().InstType != TypeInst.Standard)
                    //{
                    //    confOK &= (_device._instDevices[itm.Id] as InstSBE).SetupSBE37();
                    //}
                }
            }

            // 写入采样时间间隔
            InstDeviceStateM<InstSTDData, InstInfoBase>.sampleIntervalSec = spIntervalList[(int)comboBox_spInterval.SelectedValue].interval;

            _device.configInstDeviceInternal();
        }

        /// <summary>
        /// 从设备中重新读取相关数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox_rdDev_Click(object sender, EventArgs e)
        {
            loadInstInfo();
        }

        /// <summary>
        /// comboBox_inst 被选项改变 - 事件处理函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_inst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_inst.SelectedItem == null) return;

            linkedInst2Port();

            //try
            //{
            //    textBox_instID.Text = (comboBox_inst.SelectedItem as InstInfoItem).instrumentID;
            //}
            //catch { }
        }

        private void FormInst_FormClosing(object sender, FormClosingEventArgs e)
        {
            _device.InstDeviceInitedEvent -= _device_InstDeviceInitedEvent;
        }
    }
}
