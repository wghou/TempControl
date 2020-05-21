using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Ports;
using NLog;
using static Others.MySqlWriter;

namespace SensorDevice
{
    public partial class SensorSBE37
    {
        /// <summary>
        /// 传感器命令
        /// </summary>
        private class CmdChain
        {
            private int currentCmdIdx = -1;
            private string[] cmdString = new string[] { "DS", "TS", "QS", "QS" };

            public string FetchNextCmd()
            {
                if (++currentCmdIdx > 3) currentCmdIdx = 0;

                return cmdString[currentCmdIdx];
            }

            public void ResetCmd() { currentCmdIdx = -1; }
        }

        /// <summary>
        /// 所有指令的合集
        /// </summary>
        private CmdChain cmds = new CmdChain();

        /// <summary>
        /// 执行 Idle 步骤
        /// </summary>
        protected override void internalIdleStep() { }
        /// <summary>
        /// 进入 Measure 步骤
        /// </summary>
        protected override void internalEnterMeasureStep() {
            // 清空数据缓存
            sensorData.Clear();
            cmds.ResetCmd();
        }
        /// <summary>
        /// 执行 Measure 步骤
        /// </summary>
        protected override void internalMeasureStep() {
            // 设备未启用
            if (Enable == false) return;

            // todo: 发送指令等
            sendCMD(cmds.FetchNextCmd());
        }
        /// <summary>
        /// 进入 Store 步骤
        /// </summary>
        protected override void internalEnterStoreStep() { }
        /// <summary>
        /// 执行 Store 步骤
        /// </summary>
        protected override void internalStoreStep() {
            // 设备未启用
            if (Enable == false) return;

            // 将数据写入数据库
            sqlWriter.InsertValue(sensorData);
            // 进入空闲状态
            _sensorMachine.Fire(TriggerSensor.Stop);
        }

        /// <summary>
        /// 接收到数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // 设备未启用
            if (Enable == false) return;

            

            // todo: 解析数据
            try
            {
                string data = sPort.ReadLine();
                sPort.DiscardInBuffer();

                SensorSBE37Data dt = new SensorSBE37Data();

                // 只有在 Measure 状态，才会存储数据
                if (_sensorState == StateSensor.Measure)
                {
                    appendStoreCache(dt);
                }
                // 记录当前数据
                appendSensorData(dt);

                // 触发数据接收事件
                base.OnDataReceived(dt);
            }
            catch (Exception ex)
            {
                nlogger.Error("标准数据采集器设备接受数据发生异常：" + ex.Message);
                // 触发错误产生事件
                base.OnErrorOccur(Err_sr.Error);
            }
        }



        /// <summary>
        /// 向设备发送指令
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private string sendCMD(string cmd)
        {
            string dt = string.Empty;
            try
            {
                // 打开串口
                if (!sPort.IsOpen) this.sPort.Open();
                // 写入指令
                this.sPort.WriteLine(cmd);
            }
            catch(Exception ex)
            {
                nlogger.Error("传感器设备读取参数失败！");
                // 关闭串口
                try { sPort.Open(); } catch{ }

                return string.Empty;
            }
            return dt;
        }
    }
}
