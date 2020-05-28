using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// Error Monitor 的逻辑
// 对硬件执行读写操作时，操作完成后，生成结果、错误信息
// 1 - 触发 TempDevice / RelayDevice UPdatedEvent，向主界面传递 结果/ 错误信息，用于更新界面信息
// 2 - 判断标志位，可选择在 ErrorStatusMonitor 中记录错误信息，并触发 ErrorStatusChangedEvent
//
namespace Device
{
    public enum ErrorCode : int
    {
        /// <summary>
        /// 温度不降
        /// </summary>
        TempNotDown = 0,
        /// <summary>
        /// 温度持续下降
        /// </summary>
        TempNotUp,
        /// <summary>
        /// 温度波动过大
        /// </summary>
        TempFlucLarge,
        /// <summary>
        /// 温度持续上升
        /// </summary>
        TempBasis,
        /// <summary>
        /// 控温槽中的温度超出界限
        /// </summary>
        TempOutRange,
        /// <summary>
        /// 传感仪器错误
        /// </summary>
        InstDevError,
        /// <summary>
        /// 继电器设备错误
        /// </summary>
        RelayError,
        /// <summary>
        /// 温控设备错误
        /// </summary>
        TemptError,
        /// <summary>
        /// 温控设备参数写入错误
        /// </summary>
        TempParamSetError,
        /// <summary>
        /// 测温电桥错误
        /// </summary>
        BridgeError,
        /// <summary>
        /// 其他错误
        /// </summary>
        CodeError,
        /// <summary>
        /// 清除所有错误状态
        /// </summary>
        ClearErrorStatus
    }

    public partial class DeviceStateM
    {
        /// <summary>
        /// 设备错误状态
        /// </summary>
        private Dictionary<ErrorCode, uint> _deviceErrorMonitor = new Dictionary<ErrorCode, uint>();
        /// <summary>
        /// 当前时间步骤内错误状态
        /// </summary>
        private Dictionary<ErrorCode, uint> _deviceCurrentError = new Dictionary<ErrorCode, uint>();
        private object _errLocker = new object();
        private uint lastErrCnt = 0;

        /// <summary>
        /// 清空错误状态
        /// </summary>
        public void ResetErrorStatus()
        {
            lock (_errLocker)
            {
                foreach (ErrorCode item in Enum.GetValues(typeof(ErrorCode)))
                {
                    _deviceErrorMonitor[item] = 0;
                    _deviceCurrentError[item] = 0;
                }
            }
        }

        /// <summary>
        /// 检查设备是否错误
        /// </summary>
        /// <returns></returns>
        public uint CheckErrorStatus()
        {
            uint err = 0;
            lock (_errLocker)
            {
                foreach (ErrorCode item in Enum.GetValues(typeof(ErrorCode)))
                {
                    err += _deviceErrorMonitor[item];
                }
            }
            return err;
        }


        /// <summary>
        /// 读取设备错误状态
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Dictionary<ErrorCode, uint> GetErrorStatus()
        {
            lock (_errLocker)
            {
                return _deviceErrorMonitor;
            }
        }



        /// <summary>
        /// 清空当前时间步骤内错误状态
        /// </summary>
        public void ResetCurrentErrorStatus()
        {
            lock (_errLocker)
            {
                foreach (ErrorCode item in Enum.GetValues(typeof(ErrorCode)))
                {
                    _deviceCurrentError[item] = 0;
                }
            }
        }

        /// <summary>
        /// 查看当前时间步骤内错误状态
        /// </summary>
        /// <returns></returns>
        public uint CheckCurrentErrorStatus()
        {
            uint err = 0;
            lock (_errLocker)
            {
                foreach (ErrorCode item in Enum.GetValues(typeof(ErrorCode)))
                {
                    err += _deviceCurrentError[item];
                }
            }
            return err;
        }


        /// <summary>
        /// 读取当前时间步骤内设备错误状态
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Dictionary<ErrorCode, uint> GetCurrentErrorStatus()
        {
            lock (_errLocker)
            {
                return _deviceCurrentError;
            }
        }


        /// <summary>
        /// 向 Monitor 写入错误状态
        /// </summary>
        /// <param name="err"></param>
        private void SetErrorStatus(ErrorCode err)
        {
            lock (_errLocker)
            {
                _deviceErrorMonitor[err]++;
                _deviceCurrentError[err]++;
            }
        }


        ////////////////////////////////////////

        /// <summary>
        /// 故障判断 - 温度超出界限
        /// </summary>
        private void ErrorCheckOutRange()
        {
            if (tpDeviceM.temperatures.Count == 0) return;

            // 判断控温槽温度是否越过了界限
            if (tpDeviceM.temperatures.Last() > _runningParameters.tempMaxValue || tpDeviceM.temperatures.Last() < _runningParameters.tempMinValue)
            {
                SetErrorStatus(ErrorCode.TempOutRange);
            }
        }

        /// <summary>
        /// 故障判断 - 温度偏离 - 检测到故障则返回 true
        /// </summary>
        /// <returns></returns>
        private void ErrorCheckBasis()
        {
            if (tpDeviceM.temperatures.Count == 0) return;

            // 判断温度偏离设定点
            if (Math.Abs(tpDeviceM.temperatures.Last() - currentTemptPointState.stateTemp) > _runningParameters.tempBiasFaultThr)
            {
                SetErrorStatus(ErrorCode.TempBasis);
            }

            return;
        }

        /// <summary>
        /// 故障判断 - 温度不下降 - 检测到故障则返回 true
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private void ErrorCheckTempNotDown()
        {
            if (tpDeviceM.temperatures.Count == 0) return;

            // 故障判断 - 温度不下降 / 温度持续上升
            // 进入某一状态后，等待 tempNotUpOrDownFaultTimeSec 再判断温度是否上升 tempNotUpOrDwonFaultThr
            int count = _runningParameters.tempNotUpOrDownFaultTimeSec / _runningParameters.readTempIntervalSec;
            if (currentTemptPointState.stateCounts < count)
                return;

            // 判断温度不下降
            // 如果 tpDeviceM.temperatures 中存储的温度值过少，即系统运行时间太短，则不检测 
            //int count = 1 * 10 * 1000 / tpDeviceM.readTempInterval;
            if (tpDeviceM.temperatures.Count > count)
            {
                // 如果count 个之前的温度值减去当前温度 小于 0.4 ，说明温度没有下降
                if (tpDeviceM.temperatures[tpDeviceM.temperatures.Count - count] - tpDeviceM.temperatures.Last() < _runningParameters.tempNotUpOrDwonFaultThr)
                    SetErrorStatus(ErrorCode.TempNotDown);
            }

            return;
        }

        /// <summary>
        /// 故障判断 - 温度波动度过大 - 检测到故障则返回 true
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private void ErrorCheckTempFlucLarge()
        {
            if (tpDeviceM.temperatures.Count == 0) return;

            // 进入某一状态后，等待 flucFaultTimeSec 再判断波动度 flucFaultThr
            int count = _runningParameters.flucFaultTimeSec / _runningParameters.readTempIntervalSec;
            if (currentTemptPointState.stateCounts < count)
                return;

            // 判断温度波动大
            // 如果 tpDeviceM.temperatures 中存储的温度值过少，即系统运行时间太短，则不检测 
            //int count = 1 * 10 * 1000 / tpDeviceM.readTempInterval;
            float fluc = 0.0f;
            // 如果获取波动度大于 0.4，说明波动度过大
            if (tpDeviceM.GetFluc(count, out fluc) && fluc > _runningParameters.flucFaultThr)
            {
                SetErrorStatus(ErrorCode.TempFlucLarge);
            }

            return;
        }

        /// <summary>
        /// 故障判断 - 温度没有上升 - 检测到故障则返回 true
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private void ErrorCheckTempNotUp()
        {
            if (tpDeviceM.temperatures.Count == 0) return;

            // 故障判断 - 温度不升高 / 温度持续下降
            // 进入某一状态后，等待 tempNotUpOrDownFaultTimeSec 再判断温度是否上升 tempNotUpOrDwonFaultThr
            int count = _runningParameters.tempNotUpOrDownFaultTimeSec / _runningParameters.readTempIntervalSec;
            if (currentTemptPointState.stateCounts < count)
                return;

            // 温度没有上升
            // 如果 tpDeviceM.temperatures 中存储的温度值过少，即系统运行时间太短，则不检测 
            //int count = 1 * 10 * 1000 / tpDeviceM.readTempInterval;
            if (tpDeviceM.temperatures.Count > count)
            {
                // 如果count 个之前的温度值 减去 当前温度，小于 0.4 ，说明温度没有上升
                if (tpDeviceM.temperatures.Last() - tpDeviceM.temperatures[tpDeviceM.temperatures.Count - count] < _runningParameters.tempNotUpOrDwonFaultThr)
                    SetErrorStatus(ErrorCode.TempNotUp);
            }

            return;
        }
    }
}
