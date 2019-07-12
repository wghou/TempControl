using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stateless;
using System.Diagnostics;
using System.Timers;

namespace Device
{
    /// <summary>
    /// 附带状态机的设备
    /// </summary>
    public partial class DeviceStateM
    {
        /// <summary>
        /// Device State Mechine 自带时钟事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _stepTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // 参数可能会更改，确保每次定时器事件都更新一下
            _tickTimer.Interval = _runningParameters.readTempIntervalSec * 1000;

            // 更新水槽温度值
            UpdateTemptValue();
            // error check
            ErrorCheckOutRange();   // 温度超出界限


            // 驱动状态机执行流程
            _machine.Fire(_TickTrigger, _runningParameters.readTempIntervalSec * 1000);

   
            // 定时器事件
            TimerTickEvent?.Invoke();


            // 全局错误信息 - 事件
            uint errCnt = CheckErrorStatus();
            if(errCnt !=0 && errCnt != lastErrCnt)
            {
                ErrorStatusChangedEvent?.Invoke(_deviceErrorMonitor);
            }
            lastErrCnt = errCnt;
        }

        private void _ryConnectTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (ryDeviceM.DisconnectProtect == true)
            {
                RelayDevice.Err_r err = ryDeviceM.UpdateStatusToDevice();
                if (err != RelayDevice.Err_r.NoError) SetErrorStatus(ErrorCode.RelayError);
            }

            if (ryDeviceS.DisconnectProtect == true)
            {
                RelayDevice.Err_r err = ryDeviceS.UpdateStatusToDevice();
                if (err != RelayDevice.Err_r.NoError) SetErrorStatus(ErrorCode.RelayError);
            }
        }

        /// <summary>
        /// 启动定时器
        /// </summary>
        public void startTimeStep()
        {
            _tickTimer.Start();

           // _ryConnectTimer.Start();

            _machine.Fire(Trigger.ElectOn);
        }


        /// <summary>
        /// 状态转变事件函数
        /// </summary>
        /// <param name="act"></param>
        private void OnTransitionedAction(StateMachine<State, Trigger>.Transition act)
        {
            Trigger trigger = act.Trigger;
            State source = act.Source;
            State dest = act.Destination;

            StateChangedEvent?.Invoke(dest);
        }

        /// <summary>
        /// 状态转变事件函数
        /// </summary>
        /// <param name="act"></param>
        private void OnUnhandledTrigger(State st, Trigger tg)
        {
            nlogger.Trace("Unhandled trigger: state.");

            SetErrorStatus(ErrorCode.CodeError);
        }

        /// <summary>
        /// 下一个温度设定点，是否执行降温状态
        /// </summary>
        /// <param name="tpPoint">下一个温度点设定值</param>
        /// <returns></returns>
        private bool nextPointDown(float tpPoint)
        {
            nlogger.Trace("Next point: " + tpPoint.ToString());

            if(tpDeviceM.temperatures.Count == 0)
            {
                nlogger.Trace("tpDeviceM.temperatures.Count == 0  in nextPointDown.");
                SetErrorStatus(ErrorCode.CodeError);
                return true;
            }

            if (tpPoint < tpDeviceM.temperatures.Last())
                return true;
            else
                return false;
        }

        /// <summary>
        /// Idle Entry
        /// </summary>
        private void UndefineEntry()
        {
            nlogger.Trace("Undefine Entry.");
        }

        /// <summary>
        /// Idle Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void UndefineTick(int tic)
        {
            nlogger.Trace("UndefineTick: " + tic.ToString() + " ms");
            // do nothing
        }

        /// <summary>
        /// Idle Exit
        /// </summary>
        private void UndefineExit()
        {
            nlogger.Trace("Undefine Exit.");
        }


        /// <summary>
        /// Idle Entry
        /// </summary>
        private void IdleEntry()
        {
            nlogger.Trace("Idle Entry.");
        }

        /// <summary>
        /// Idle Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void IdleTick(int tic)
        {
            nlogger.Trace("IdleTick: " + tic.ToString() + " ms");
        }

        /// <summary>
        /// Idle Exit
        /// </summary>
        private void IdleExit()
        {
            nlogger.Trace("Idle Exit.");
        }


        /// <summary>
        /// Start Entry
        /// </summary>
        private void StartEntry()
        {
            nlogger.Trace("Start Entry.");
        }

        /// <summary>
        /// Start Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void StartTick(int tic)
        {
            nlogger.Trace("StartTick: " + tic.ToString() + " ms");

            // 如果 temperaturePointList 为空
            if (temperaturePointList.Count == 0)
            {
                _machine.Fire(Trigger.FinishedAll);
                return;
            }

            // 定义当前状态
            currentTemptPointState.stateCounts = 0;

            // 查找未测量的温度点
            int i = 0;
            for (; i < temperaturePointList.Count; i++)
            {
                if (temperaturePointList[i].finished == false)
                {
                    currentTemptPointState.tempPointIndex = i;
                    currentTemptPointState = temperaturePointList[i];
                    break;
                }
            }

            // 如果全部温度点都已经标记为 已测量，则转入空闲状态
            if (i == temperaturePointList.Count)
            {
                _machine.Fire(Trigger.FinishedAll);
            }
            else
            {
                if (tpDeviceM.temperatures.Count == 0)
                {
                    _machine.Fire(Trigger.ForceStop);
                    return;
                }

                // 如果当前主槽温度刚好处于温度点附近，且满足阈值条件，则直接进入控温状态
                if (Math.Abs(tpDeviceM.temperatures.Last() - currentTemptPointState.stateTemp) < _runningParameters.controlTempThr)
                {
                    // 状态 - 控温
                    _machine.Fire(Trigger.StartControl);
                }
                // 当前温度点小于温度设定点，则升温
                else
                {
                    _machine.Fire(_nextPointTrigger, currentTemptPointState.stateTemp);
                }
            }
        }

        /// <summary>
        /// Start Exit
        /// </summary>
        private void StartExit()
        {
            nlogger.Trace("Start Exit.");
        }


 
        /// <summary>
        /// TempUp Entry
        /// </summary>
        private void TempUpEntry()
        {
            // 升温
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_0] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_4] = false;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_5] = true;
            WriteRelayDeviceM(true);

            // 设置主槽 / 辅槽控温设备的参数
            currentTemptPointState.paramM.CopyTo(tpDeviceM.tpParamToSet, 0);
            // 将参数更新到下位机
            // 如果出现错误，则由 _deviceErrorMonitor 记录错误状态
            WriteTempDeviceM(true);

            if (tpDeviceS.Enable == true)
            {
                currentTemptPointState.paramS.CopyTo(tpDeviceM.tpParamToSet, 0);
                // 将参数更新到下位机
                // 如果出现错误，则由 _deviceErrorMonitor 记录错误状态
                WriteTempDeviceS(true);
            }

            nlogger.Trace("TempUp Entry.");
        }

        /// <summary>
        /// TempUp Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void TempUpTick(int tic)
        {
            nlogger.Trace("TempUp Tick: " + tic.ToString() + " ms");

            // 状态时间计数器
            currentTemptPointState.stateCounts++;

            ErrorCheckTempNotUp();  // 温度不升高


            // 判断 - 温度上升到设定值以上（0.1度），则进入控温状态
            if (tpDeviceM.temperatures.Last() > currentTemptPointState.stateTemp - 0.1)
            {
                // 如果主槽中温度高于设定值，则进入下一个状态 - 控温
                currentTemptPointState.stateCounts = 0;
                _machine.Fire(Trigger.StartControl);
                return;
            }
        }

        /// <summary>
        /// TempUp Exit
        /// </summary>
        private void TempUpExit()
        {
            nlogger.Trace("TempUp Exit.");
        }



        /// <summary>
        /// TempDown Entry
        /// </summary>
        private void TempDownEntry()
        {
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_0] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_4] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_5] = true;
            WriteRelayDeviceM(true);

            // 设置主槽 / 辅槽控温设备的参数
            // 向主槽 / 辅槽控温设备写入全部参数
            currentTemptPointState.paramM.CopyTo(tpDeviceM.tpParamToSet, 0);
            // 将参数更新到下位机
            // 如果出现错误，则通过 _deviceErrorMonitor 记录错误状态
            WriteTempDeviceM(true);

            if (tpDeviceS.Enable == true)
            {
                currentTemptPointState.paramS.CopyTo(tpDeviceM.tpParamToSet, 0);
                // 将参数更新到下位机
                // 如果出现错误，则由 _deviceErrorMonitor 记录错误状态
                WriteTempDeviceS(true);
            }

            nlogger.Trace("TempDown Entry.");
        }

        /// <summary>
        /// TempDown Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void TempDownTick(int tic)
        {
            nlogger.Trace("TempDown Tick: " + tic.ToString() + " ms");

            // 状态时间计数器
            currentTemptPointState.stateCounts++;

            // error check
            ErrorCheckTempNotDown();    // 温度不降低


            // 判断 - 温度上升到设定值以上（0.1度），则进入控温状态
            if (tpDeviceM.temperatures.Last() < currentTemptPointState.stateTemp + 0.1)
            {
                // 如果主槽中温度高于设定值，则进入下一个状态 - 控温
                currentTemptPointState.stateCounts = 0;
                _machine.Fire(Trigger.StartControl);
                return;
            }
        }

        /// <summary>
        /// TempDown Exit
        /// </summary>
        private void TempDownExit()
        {
            nlogger.Trace("TempDown Exit.");
        }

        

        /// <summary>
        /// Control Entry
        /// </summary>
        private void ControlEntry()
        {
            nlogger.Trace("Control Entry.");

            // 首次进入该状态，应改变相应的继电器状态
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_0] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_4] = false;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_5] = false;
            WriteRelayDeviceM(true);
        }

        /// <summary>
        /// Control Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void ControlTick(int tic)
        {
            nlogger.Trace("Control Tick: " + tic.ToString() + " ms");

            // 状态时间计数器
            currentTemptPointState.stateCounts++;

            // error check
            ErrorCheckBasis();          // 当前温度偏离温度设定点过大
            ErrorCheckTempFlucLarge();  // 波动度过大


            // 判断 - 控温状态下，温度波动度满足判断条件（5 分钟 0.001），则立即进入稳定状态
            bool steady = tpDeviceM.checkFlucCount(_runningParameters.steadyTimeSec / _runningParameters.readTempIntervalSec, _runningParameters.flucValue);
            if (steady)
            {
                // 进入下一个状态，下一个状态应该是 稳定
                currentTemptPointState.stateCounts = 0;
                _machine.Fire(Trigger.AchieveSteady);
                nlogger.Info((_runningParameters.steadyTimeSec / 60).ToString("0") + " 分钟温度波动度满足波动度小于 " + _runningParameters.flucValue.ToString("0.0000") + "℃");
            }
        }

        /// <summary>
        /// TempDown Exit
        /// </summary>
        private void ControlExit()
        {
            nlogger.Trace("Control Exit.");
        }



        /// <summary>
        /// Stable Entry
        /// </summary>
        private void StableEntry()
        {
            nlogger.Trace("Stable Entry.");

            // 首次进入该状态，应改变相应的继电器状态
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_0] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_4] = false;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_5] = false;
            WriteRelayDeviceM(true);
        }


        /// <summary>
        /// Stable Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void StableTick(int tic)
        {
            nlogger.Trace("Stableick: " + tic.ToString() + " ms");

            // 状态时间计数器
            currentTemptPointState.stateCounts++;

            // error check
            ErrorCheckBasis();          // 当前温度与设定温度点偏离过大
            ErrorCheckTempFlucLarge();  // 波动度过大


            // 判断 - 测温电桥温度值的波动度满足条件（2 分钟 0.001），则进入测量状态
            if (currentTemptPointState.stateCounts > _runningParameters.bridgeSteadyTimeSec / _runningParameters.readTempIntervalSec)
            {
                // 电桥自检正常。。。
                //if (tpBridge.tpBridgeReadInterval < 1) tpBridge.tpBridgeReadInterval = 1;
                //bool steady = tpBridge.chekFluc(currentState.stateCounts / tpBridge.tpBridgeReadInterval, flucValue);
                if (true)
                {
                    // 温度稳定度达到了要求，进入下一个状态 - 测量
                    currentTemptPointState.stateCounts = 0;
                    _machine.Fire(Trigger.StartMeasure);

                    nlogger.Info((_runningParameters.bridgeSteadyTimeSec / 60).ToString("0") + " 分钟电桥温度波动度小于 " + _runningParameters.flucValue.ToString("0.0000") + "℃，可以测量电导率等数据");
                }
            }
        }

        /// <summary>
        /// Stable Exit
        /// </summary>
        private void StableExit()
        {
            nlogger.Trace("Stable Exit.");
        }

        

        /// <summary>
        /// Measure Entry
        /// </summary>
        private void MeasureEntry()
        {
            nlogger.Trace("Measure Entry.");
        }

        /// <summary>
        /// Measure Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void MeasureTick(int tic)
        {
            nlogger.Trace("MeasureTick: " + tic.ToString() + " ms");

            // 状态时间计数器
            currentTemptPointState.stateCounts++;

            // measure


            // 测量完成，标记
            temperaturePointList[currentTemptPointState.tempPointIndex].finished = true;


            // 查找下一个未测量的温度点
            int i = currentTemptPointState.tempPointIndex + 1;
            for (; i < temperaturePointList.Count; i++)
            {
                if (temperaturePointList[i].finished == false)
                {
                    currentTemptPointState = temperaturePointList[i];
                    currentTemptPointState.tempPointIndex = i;
                    currentTemptPointState.stateCounts = 0;
                    _machine.Fire(_nextPointTrigger, currentTemptPointState.stateTemp);

                    nlogger.Info("开始下一个温度点的控温 - 稳定 - 测量流程...");
                    nlogger.Info("查找到了下一个未测量的温度点 " + currentTemptPointState.stateTemp.ToString());

                    break;
                }
            }

            // 未查找到，则表示已经测量完成了
            if (i == temperaturePointList.Count)
            {
                // 控制状态序列为空，说明实验已经结束了
                if (_runningParameters.shutDownComputer == true)
                {
                    _machine.Fire(Trigger.ForceStop);
                }
                else
                {
                    _machine.Fire(Trigger.FinishedAll);
                }
                nlogger.Info("所有温度点均已测量完成...");
            }
        }

        /// <summary>
        /// Measure Exit
        /// </summary>
        private void MeasureExit()
        {
            nlogger.Trace("Measure Exit.");
        }



        /// <summary>
        /// Stop Entry
        /// </summary>
        private void StopEntry()
        {
            nlogger.Trace("Stop Entry.");

            // 关闭除总电源外的所有继电器
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_0] = false;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_4] = false;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_5] = false;

            WriteRelayDeviceM(true);

        }

        /// <summary>
        /// Stop Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void StopTick(int tic)
        {
            nlogger.Trace("StopTick: " + tic.ToString() + " ms");
        }

        /// <summary>
        /// Stop Exit
        /// </summary>
        private void StopExit()
        {
            nlogger.Trace("Stop Exit.");
        }
    }
}