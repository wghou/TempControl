using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using static Others.MySqlWriter;

namespace SensorDevice
{
    public partial class SensorSTD
    {
        /// <summary>
        /// 数据连续传输计数器
        /// </summary>
        private int continuityCnt = 0;

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
        }
        /// <summary>
        /// 执行 Measure 步骤
        /// </summary>
        protected override void internalMeasureStep() { }
        /// <summary>
        /// 进入 Store 步骤
        /// </summary>
        protected override void internalEnterStoreStep() { }
        /// <summary>
        /// 执行 Store 步骤
        /// </summary>
        protected override void internalStoreStep() {
            // todo: 存储数据
            foreach(var itm in sensorData)
            {
                StandardDataSqlrd dt = new StandardDataSqlrd();

                sqlWriter.InsertValue(dt);
            }

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
            // 只有在 Measure 状态，才会存储数据
            if (_sensorState != StateSensor.Measure) return;

            // todo: 解析数据
            try
            {
                string data = sPort.ReadLine();

                StandardDeviceData dt = new StandardDeviceData();

                string[] valStrs = data.Split('-');

                dt.dateTime = DateTime.ParseExact(valStrs[0],"yyyy_MM_dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                dt.freqC = float.Parse(valStrs[1]);
                dt.conduct = float.Parse(valStrs[2]);
                dt.freqT = float.Parse(valStrs[3]);
                dt.tempt = float.Parse(valStrs[4]);
                dt.salt = float.Parse(valStrs[5]);

                // 记录当前数据
                appendData(dt);
            }
            catch (Exception ex)
            {
                nlogger.Error("标准数据采集器设备接受数据发生异常：" + ex.Message);
            }
        }
    }
}
