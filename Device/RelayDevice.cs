using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using Modbus.Data;
using Modbus.Device;
using Modbus.Utility;
using System.IO.Ports;
using NLog;

namespace Device
{
    /// <summary>
    /// 标控自动化科技
    /// 远程IO模块 8输入/8输出
    /// https://item.taobao.com/item.htm?spm=a1z0k.6846577.0.0.1d078eadcPgGKr&id=553199470529&_u=t2dmg8j26111
    /// </summary>
    public class RelayDevice
    {
        private static readonly Logger nlogger = LogManager.GetCurrentClassLogger();

        /// <summary>串口</summary>
        private SerialPort sPort;
        public string ryDevicePortName;
        private IModbusSerialMaster master;
        private byte slaveId = 1;
        ushort startAddress = 0;
        /// <summary>串口读-写时间间隔</summary>
        private const int intervalOfWR = 20;
        public int numCoils = 8;

        /// <summary>
        /// 继电器断网保护功能，如果勾选，则需要持续与继电器通讯
        /// </summary>
        public bool DisconnectProtect = false;
        public bool Enable = true;

        public enum Cmd_r : int
        {
            OUT_0 = 0,
            OUT_1 = 1,
            OUT_2 = 2,
            OUT_3 = 3,
            OUT_4 = 4,
            OUT_5 = 5,
            OUT_6 = 6,
            OUT_7 = 7,
            OUT_8 = 8,
            OUT_9 = 9,
            OUT_10 = 10,
            OUT_11 = 11,
            OUT_12 = 12,
            OUT_13 = 13,
            OUT_14 = 14,
            OUT_15 = 15
        }

        public enum Err_r : int
        {
            NoError = 0,
            ComError = 1
        }
        /// <summary>
        /// Relay 设备各继电器状态
        /// </summary>
        public bool[] ryStatus = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
        /// <summary>
        /// Relay 设备各继电器将要设置的状态，调用 SetRelayStatusAll 后将更新继电器状态
        /// </summary>
        public bool[] ryStatusToSet = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
        /// <summary>
        /// 辅槽制冷上一次关闭的时间；
        /// 辅槽制冷开关在关闭后，必须要等待至少十分钟，才能再次打开；
        /// 故，在每次关闭时，都记录下关闭时的时间点
        /// </summary>
        public DateTime subCoolCloseTime = DateTime.MinValue;
        public double waitingTime = 5;
        /// <summary>
        /// 辅槽制冷需要延迟打开
        /// 配合 subCoolCLoseTime 使用
        /// </summary>
        public bool subCoolWaiting = false;
        /// <summary>
        /// 设备线程锁，同一时间只允许单一线程访问设备资源（串口 / 数据）
        /// </summary>
        private object ryLocker = new object();


        public RelayDevice()
        {
            sPort = new SerialPort()
            {
                ReadTimeout = 200,
                WriteTimeout = 200,
                BaudRate = 9600,
                NewLine = "\r\n"
            };
            master = ModbusSerialMaster.CreateRtu(sPort);
            master.Transport.Retries = 1;
        }

        public bool SetPortName(string portName)
        {
            // 当 Enable == false 时，返回 true
            ryDevicePortName = portName;

            try
            {
                // 先主动关闭串口
                try { sPort.Close(); } catch { }

                sPort.PortName = portName;

                string[] portNames = SerialPort.GetPortNames();
                if (!portNames.Contains(portName.ToUpper()))
                {
                    nlogger.Error("端口 " + portName + " 不存在");
                    return !Enable;
                }
                // 串口打开 / 关闭测试
                if (!sPort.IsOpen)
                    sPort.Open();
                Thread.Sleep(intervalOfWR);
                if (sPort.IsOpen)
                    sPort.Close();
                return true;

            }
            catch (Exception ex)
            {
                nlogger.Error("继电器设备新建串口时发生异常：" + ex.Message);
                return !Enable;
            }
        }


        /// <summary>
        /// 继电器设备逐个自检
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public Err_r SelfCheck(Tuple<Cmd_r, bool> cmd)
        {
            Err_r err = Err_r.NoError;
            try
            {
                master.WriteSingleCoil(slaveId, (ushort)(startAddress + cmd.Item1), cmd.Item2);
            }
            catch(Exception ex)
            {
                nlogger.Error("继电器设备自检错误 " + ex.Message);
                err = Err_r.ComError;
            }

            return err;
        }


        /// <summary>
        /// 继电器设备自检
        /// </summary>
        /// <returns></returns>
        public Err_r SelfCheck(List<Tuple<Cmd_r, bool>> cmdList)
        {
            Err_r err = Err_r.NoError;
            /////////////////////////////////////////////////////////////////////////
            foreach(var cmd in cmdList)
            {
                try
                {
                    // open the serial port
                    if (!sPort.IsOpen) sPort.Open();

                    master.WriteSingleCoil(slaveId, (ushort)(startAddress + cmd.Item1), cmd.Item2);
                }
                catch(Exception ex)
                {
                    nlogger.Error("继电器设备自检错误 " + ex.Message);
                    err = Err_r.ComError;
                    break;
                }

                Thread.Sleep(1000);
            }

            return err;
        }

        public Err_r UpdateStatusToDevice()
        {
            Err_r err = Err_r.NoError;

            lock (ryLocker)
            {
                try
                {
                    // open the serial port
                    if (!sPort.IsOpen) sPort.Open();

                    bool[] st = new bool[numCoils];
                    Array.Copy(ryStatusToSet, st, numCoils);
                    master.WriteMultipleCoils(slaveId, startAddress, st);

                    Array.Copy(ryStatusToSet, ryStatus, numCoils);

                    sPort.Close();
                }
                catch(Exception ex)
                {
                    Array.Copy(ryStatus, ryStatusToSet,  numCoils);
                    nlogger.Error("继电器设备写入继电器状态失败 " + ex.Message);
                    err = Err_r.ComError;
                }
                
            }
            return err;
        }


        public bool closeDevice()
        {
            for (int i = 0; i < ryStatus.Length; i++) ryStatus[i] = false;

            if (Enable == false) return true;

            bool rlt = true;

            lock (ryLocker)
            {
                // 关闭所有继电器
                for(int i = 0; i < 16; i++)
                {
                    ryStatusToSet[i] = false;
                }

                try
                {
                    // open the serial port
                    if (!sPort.IsOpen) sPort.Open();

                    if (numCoils == 8)
                    {
                        bool[] st = { false, false, false, false, false, false, false, false };
                        master.WriteMultipleCoils(slaveId, startAddress, st);
                    }
                    else
                    {
                        bool[] st = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
                        master.WriteMultipleCoils(slaveId, startAddress, st);
                    }

                    sPort.Close();
                }
                catch (Exception ex)
                {
                    rlt = false;
                    nlogger.Error("关闭继电器设备失败 - 16。");
                }
            }

            return rlt;
        }

    }
}
