using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// 读电桥温度错误
        /// </summary>
        SensorError,
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
        /// 其他错误
        /// </summary>
        CodeError
    }

    public partial class DeviceStateM
    {
        /// <summary>
        /// 设备错误状态
        /// </summary>
        private Dictionary<ErrorCode, uint> _deviceErrorMonitor;


        /// <summary>
        /// 配置设备错误状态
        /// </summary>
        private void ConfigErrorMonitor()
        {
            _deviceErrorMonitor = new Dictionary<ErrorCode, uint>();

            // 清空错误状态
            ResetErrorStatus();
        }


        /// <summary>
        /// 清空错误状态
        /// </summary>
        public void ResetErrorStatus()
        {
            foreach (ErrorCode item in Enum.GetValues(typeof(ErrorCode)))
            {
                _deviceErrorMonitor[item] = 0;
            }
        }

        /// <summary>
        /// 检查设备是否错误
        /// </summary>
        /// <returns></returns>
        public uint CheckErrorStatus()
        {
            // bug
            // 如果 _deviceErrorMonitor 未初始化，读取会出现异常 

            uint err = 0;
            foreach (ErrorCode item in Enum.GetValues(typeof(ErrorCode)))
            {
                err += _deviceErrorMonitor[item];
            }

            return err;
        }

        /// <summary>
        /// 读取设备错误状态
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public uint GetErrorStatus(ErrorCode item)
        {
            // bug
            // 如果 _deviceErrorMonitor 未初始化，读取会出现异常

            return _deviceErrorMonitor[item];
        }


        private void SetErrorStatus(ErrorCode err)
        {
            _deviceErrorMonitor[err]++;
        }



        /// <summary>
        /// 故障判断 - 温度超出界限
        /// </summary>
        private void ErrorCheckOutRange()
        {
            // 判断控温槽温度是否越过了界限
            if (tpDeviceM.temperatures.Last() > _thresholdParameters.tempMaxValue || tpDeviceM.temperatures.Last() < _thresholdParameters.tempMinValue)
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
            // 判断温度偏离设定点
            if (tpDeviceM.temperatures.Count != 0)
            {
                if (Math.Abs(tpDeviceM.temperatures.Last() - currentTemptPointState.stateTemp) > _thresholdParameters.tempBiasFaultThr)
                {
                    SetErrorStatus(ErrorCode.TempBasis);
                }
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
            // 故障判断 - 温度不下降 / 温度持续上升
            // 进入某一状态后，等待 tempNotUpOrDownFaultTimeSec 再判断温度是否上升 tempNotUpOrDwonFaultThr
            int count = _thresholdParameters.tempNotUpOrDownFaultTimeSec / tpDeviceM.readTempIntervalSec;
            if (currentTemptPointState.stateCounts < count)
                return;

            // 判断温度不下降
            // 如果 tpDeviceM.temperatures 中存储的温度值过少，即系统运行时间太短，则不检测 
            //int count = 1 * 10 * 1000 / tpDeviceM.readTempInterval;
            if (tpDeviceM.temperatures.Count > count)
            {
                // 如果count 个之前的温度值减去当前温度 小于 0.4 ，说明温度没有下降
                if (tpDeviceM.temperatures[tpDeviceM.temperatures.Count - count] - tpDeviceM.temperatures.Last() < _thresholdParameters.tempNotUpOrDwonFaultThr)
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
            // 进入某一状态后，等待 flucFaultTimeSec 再判断波动度 flucFaultThr
            int count = _thresholdParameters.flucFaultTimeSec / tpDeviceM.readTempIntervalSec;
            if (currentTemptPointState.stateCounts < count)
                return;

            // 判断温度波动大
            // 如果 tpDeviceM.temperatures 中存储的温度值过少，即系统运行时间太短，则不检测 
            //int count = 1 * 10 * 1000 / tpDeviceM.readTempInterval;
            float fluc = 0.0f;
            // 如果获取波动度大于 0.4，说明波动度过大
            if (tpDeviceM.GetFluc(count, out fluc) && fluc > _thresholdParameters.flucFaultThr)
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
            // 故障判断 - 温度不升高 / 温度持续下降
            // 进入某一状态后，等待 tempNotUpOrDownFaultTimeSec 再判断温度是否上升 tempNotUpOrDwonFaultThr
            int count = _thresholdParameters.tempNotUpOrDownFaultTimeSec / tpDeviceM.readTempIntervalSec;
            if (currentTemptPointState.stateCounts < count)
                return;

            // 温度没有上升
            // 如果 tpDeviceM.temperatures 中存储的温度值过少，即系统运行时间太短，则不检测 
            //int count = 1 * 10 * 1000 / tpDeviceM.readTempInterval;
            if (tpDeviceM.temperatures.Count > count)
            {
                // 如果count 个之前的温度值 减去 当前温度，小于 0.4 ，说明温度没有上升
                if (tpDeviceM.temperatures.Last() - tpDeviceM.temperatures[tpDeviceM.temperatures.Count - count] < _thresholdParameters.tempNotUpOrDwonFaultThr)
                    SetErrorStatus(ErrorCode.TempNotUp);
            }

            return;
        }
    }
}
