using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using static Others.MySqlWriter;

namespace InstDevice
{
    public partial class InstSTD
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
            _instData.Clear();
            _storeCache.Clear();
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
            // 设备未启用
            if (Enable == false) return;

            // 将数据写入数据库
            if (sqlWriter.InsertValue(_instData) == false)
            {
                // 写入数据库失败
                OnErrorOccur(Err_sr.Error);
            }
            // 进入空闲状态
            _instMachine.Fire(TriggerInst.Stop);
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

                InstSTDData dt = new InstSTDData();

                string[] valStrs = data.Split('-');

                dt.vTestID = "wghou";
                dt.vTitularValue = 123123;
                dt.vStandardC = double.Parse(valStrs[4]);
                dt.vStandardT = double.Parse(valStrs[5]);
                dt.measureTime = DateTime.ParseExact(valStrs[0], "yyyy_MM_dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                dt.addTime = dt.measureTime;
                dt.updateTime = dt.measureTime;

                // 只有在 Measure 状态，才会存储数据
                if (_instState == StateInst.Measure) {
                    appendStoreCache(dt);
                }
                // 记录当前数据
                appendInstData(dt);

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
    }
}
