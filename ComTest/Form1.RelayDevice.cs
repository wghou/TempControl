using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO.Ports;
using System.Diagnostics;
using Modbus;
using Modbus.Data;
using Modbus.Device;
using Modbus.Utility;
using System.Drawing;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ComTest
{
    public partial class Form1
    {
        /// <summary>
        /// 继电器板卡 M - 串口
        /// </summary>
        SerialPort ryPortM = new SerialPort();
        /// <summary>
        /// 继电器板卡 M - NModbus Slave
        /// </summary>
        Modbus.Device.ModbusSlave ryDeviceM;
        /// <summary>
        /// 继电器板卡 M - 设备号
        /// </summary>
        byte unitIdM = 1;
        bool SportRyM_enable = false;
        Thread slaveThreadM;

        /// <summary>
        /// 继电器板卡 S - 串口
        /// </summary>
        SerialPort ryPortS = new SerialPort();
        /// <summary>
        /// 继电器板卡 S - NModbus Slave
        /// </summary>
        Modbus.Device.ModbusSlave ryDeviceS;
        /// <summary>
        /// 继电器板卡 S - 设备号
        /// </summary>
        byte unitIdS = 1;
        bool SportRyS_enable = false;
        Thread slaveThreadS;


        ////////////////////////////
        // 继电器状态
        ///
        /// <summary>
        /// 继电器设备错误状态
        /// </summary>
        enum RyStatus : int
        {
            /// <summary>
            /// 处于正常工作状态
            /// </summary>
            OK = 0,
            /// <summary>
            /// 连接断开 - 即不返回任何数据
            /// </summary>
            DisConnected = 1,
            /// <summary>
            /// 数据错误
            /// </summary>
            DataErr = 2,
        }
        /// <summary>
        /// 锁 - 用于锁定继电器状态参数
        /// </summary>
        object ryLocker = new object();
        /// <summary>
        /// 继电器设备 - 当前工作状态 - 默认正常工作
        /// </summary>
        RyStatus ryErrStatus = RyStatus.OK;
        /// <summary>
        /// 继电器设备 - 在产生一次错误状态后，是否保持错误状态 - 默认不保持
        /// </summary>
        bool ryErrLast = false;
        /// <summary>
        /// 继电器设备代码错误
        /// </summary>
        bool ryCodeErr = false;


        bool initNModbus(JObject obj)
        {

            if (obj.ContainsKey("RelayM"))
            {
                JObject child = (JObject)obj["RelayM"];

                SportRyM_enable = child.ContainsKey("Enable") ? (bool)child["Enable"] : true;
                if (SportRyM_enable)
                {
                    // configure serial port
                    ryPortM.PortName = child.ContainsKey("PortName") ? child["PortName"].ToString() : "COM2";
                    ryPortM.BaudRate = child.ContainsKey("BaudRate") ? (int)child["BaudRate"] : 9600;
                    ryPortM.DataBits = 8;
                    ryPortM.Parity = Parity.None;
                    ryPortM.StopBits = StopBits.One;
                    ryPortM.Open();

                    // create modbus slave
                    ryDeviceM = ModbusSerialSlave.CreateRtu(unitIdM, ryPortM);
                    ryDeviceM.DataStore = DataStoreFactory.CreateDefaultDataStore();

                    //ryDeviceM.Listen();
                    slaveThreadM = new Thread(new ThreadStart(ryDeviceM.Listen));
                    slaveThreadM?.Start();

                    // 上位机写入完成
                    ryDeviceM.WriteComplete += RyDeviceM_WriteComplete;
                }
            }

            // 继电器 2 - 通信端口
            if (obj.ContainsKey("RelayS"))
            {
                JObject child = (JObject)obj["RelayS"];

                SportRyS_enable = child.ContainsKey("Enable") ? (bool)child["Enable"] : true;
                if (SportRyS_enable)
                {
                    // configure serial port
                    ryPortS.PortName = child.ContainsKey("PortName") ? child["PortName"].ToString() : "COM2";
                    ryPortS.BaudRate = child.ContainsKey("BaudRate") ? (int)child["BaudRate"] : 9600;
                    ryPortS.DataBits = 8;
                    ryPortS.Parity = Parity.None;
                    ryPortS.StopBits = StopBits.One;
                    ryPortS.Open();

                    // create modbus slave
                    ryDeviceS = ModbusSerialSlave.CreateRtu(unitIdS, ryPortS);
                    ryDeviceS.DataStore = DataStoreFactory.CreateDefaultDataStore();

                    //ryDeviceS.Listen();
                    slaveThreadS = new Thread(new ThreadStart(ryDeviceS.Listen));
                    slaveThreadS?.Start();

                    // 上位机写入完成
                    ryDeviceS.WriteComplete += RyDeviceM_WriteComplete;
                }
            }

            return true;
        }

        private void RyDeviceM_WriteComplete(object sender, ModbusSlaveRequestEventArgs e)
        {
            Color[] ryM_clr = new Color[16];
            Color[] ryS_clr = new Color[16];
            for(int i = 0; i < 16; i++)
            {
                ryM_clr[i] = ryDeviceM.DataStore.CoilDiscretes[i + 1] ? Color.FromArgb(1, 111, 1) : Color.FromArgb(111, 1, 1);
                ryS_clr[i] = ryDeviceS.DataStore.CoilDiscretes[i + 1] ? Color.FromArgb(1, 111, 1) : Color.FromArgb(111, 1, 1);
            }

            hslStatusManagement_ryM.SetColorAll(ryM_clr);
            hslStatusManagement_ryS.SetColorAll(ryS_clr);
        }
    }
}
