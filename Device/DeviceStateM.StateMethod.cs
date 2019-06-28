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
            StateChangedEvent?.Invoke(_state);

            _tickTimer.Start();

            _ryConnectTimer.Start();
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

            // 状态计数器
            currentTemptPointState.stateCounts = 0;

            StateChangedEvent?.Invoke(dest);
        }

        /// <summary>
        /// 状态转变事件函数
        /// </summary>
        /// <param name="act"></param>
        private void OnUnhandledTrigger(State st, Trigger tg)
        {
            nlogger.Error("Unhandled trigger: state.");

            SetErrorStatus(ErrorCode.CodeError);
        }

        /// <summary>
        /// 下一个温度设定点，是否执行降温状态
        /// </summary>
        /// <param name="tpPoint">下一个温度点设定值</param>
        /// <returns></returns>
        private bool nextPointDown(float tpPoint)
        {
            nlogger.Debug("Next point: " + tpPoint.ToString());

            if(tpDeviceM.temperatures.Count == 0)
            {
                nlogger.Debug("tpDeviceM.temperatures.Count == 0  in nextPointDown.");
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
            nlogger.Debug("Undefine Entry.");
        }

        /// <summary>
        /// Idle Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void UndefineTick(int tic)
        {
            nlogger.Debug("UndefineTick: " + tic.ToString() + " ms");
            // do nothing
        }

        /// <summary>
        /// Idle Exit
        /// </summary>
        private void UndefineExit()
        {
            nlogger.Debug("Undefine Exit.");
        }


        /// <summary>
        /// Idle Entry
        /// </summary>
        private void IdleEntry()
        {
            nlogger.Debug("Idle Entry.");
        }

        /// <summary>
        /// Idle Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void IdleTick(int tic)
        {
            if (srDevice.CheckStopRunRQT() == true)
            {
                _machine.Fire(Trigger.ForceStop);
                nlogger.Info("系统收到结束指令.");
            }
            else if (srDevice.CheckNextPointRQT(out currentTemptPointState.paramM[0]) == true)
            {
                _machine.Fire(Trigger.StartAutoStep);
                nlogger.Info("系统收到下一个温度点的指令: " + currentTemptPointState.stateTemp.ToString());
            }

            nlogger.Info("IdleTick: " + tic.ToString() + " ms");
        }

        /// <summary>
        /// Idle Exit
        /// </summary>
        private void IdleExit()
        {
            nlogger.Debug("Idle Exit.");
        }


        /// <summary>
        /// Start Entry
        /// </summary>
        private void StartEntry()
        {
            nlogger.Debug("Start Entry.");
        }

        /// <summary>
        /// Start Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void StartTick(int tic)
        {
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

            nlogger.Debug("StartTick: " + tic.ToString() + " ms");
        }

        /// <summary>
        /// Start Exit
        /// </summary>
        private void StartExit()
        {
            nlogger.Debug("Start Exit.");
        }


 
        /// <summary>
        /// TempUp Entry
        /// </summary>
        private void TempUpEntry()
        {
            // 升温
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_0] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_1] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_2] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_3] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_4] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_5] = false;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_6] = true;

            WriteRelayDeviceM(true);

            // 设置主槽 / 辅槽控温设备的参数
            currentTemptPointState.paramM[1] = 0.0f;    //  清空修正值
            currentTemptPointState.paramM.CopyTo(tpDeviceM.tpParamToSet, 0);
            // 将参数更新到下位机
            // 如果出现错误，则由 _deviceErrorMonitor 记录错误状态
            WriteTempDeviceM(true);

            nlogger.Debug("TempUp Entry.");
        }

        /// <summary>
        /// TempUp Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void TempUpTick(int tic)
        {
            nlogger.Debug("TempUp Tick: " + tic.ToString() + " ms");

            if (srDevice.CheckStopRunRQT() == true)
            {
                _machine.Fire(Trigger.ForceStop);
                nlogger.Info("系统收到结束指令.");
            }

            // 状态时间计数器
            currentTemptPointState.stateCounts++;

            ErrorCheckTempNotUp();  // 温度不升高


            // 判断 - 温度上升到设定值以上（0.1度），则进入控温状态
            if (tpDeviceM.temperatures.Last() > currentTemptPointState.stateTemp - 0.1)
            {
                // 如果主槽中温度高于设定值，则进入下一个状态 - 控温
                _machine.Fire(Trigger.StartControl);
                return;
            }
        }

        /// <summary>
        /// TempUp Exit
        /// </summary>
        private void TempUpExit()
        {
            nlogger.Debug("TempUp Exit.");
        }



        /// <summary>
        /// TempDown Entry
        /// </summary>
        private void TempDownEntry()
        {
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_0] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_1] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_2] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_3] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_4] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_5] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_6] = true;

            WriteRelayDeviceM(true);

            // 设置主槽 / 辅槽控温设备的参数
            // 向主槽 / 辅槽控温设备写入全部参数
            currentTemptPointState.paramM[1] = 0.0f;    //  清空修正值
            currentTemptPointState.paramM.CopyTo(tpDeviceM.tpParamToSet, 0);
            // 将参数更新到下位机
            // 如果出现错误，则通过 _deviceErrorMonitor 记录错误状态
            WriteTempDeviceM(true);

            nlogger.Debug("TempDown Entry.");
        }

        /// <summary>
        /// TempDown Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void TempDownTick(int tic)
        {
            nlogger.Debug("TempDown Tick: " + tic.ToString() + " ms");

            if (srDevice.CheckStopRunRQT() == true)
            {
                _machine.Fire(Trigger.ForceStop);
                nlogger.Info("系统收到结束指令.");
            }

            // 状态时间计数器
            currentTemptPointState.stateCounts++;

            // error check
            ErrorCheckTempNotDown();    // 温度不降低


            // 判断 - 温度上升到设定值以上（0.1度），则进入控温状态
            if (tpDeviceM.temperatures.Last() < currentTemptPointState.stateTemp + 0.1)
            {
                // 如果主槽中温度高于设定值，则进入下一个状态 - 控温
                _machine.Fire(Trigger.StartControl);
                return;
            }
        }

        /// <summary>
        /// TempDown Exit
        /// </summary>
        private void TempDownExit()
        {
            nlogger.Debug("TempDown Exit.");
        }

        

        /// <summary>
        /// Control Entry
        /// </summary>
        private void ControlEntry()
        {
            nlogger.Debug("Control Entry.");

            // 首次进入该状态，应改变相应的继电器状态
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_0] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_1] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_2] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_3] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_4] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_5] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_6] = true;

            // 如果辅槽制冷本身就是打开的，则不操作
            //if (ryDeviceM.ryStatus[(int)RelayDevice.Cmd_r.OUT_0] == true)
            //{

            //}
            //// 如果辅槽制冷是关闭的，且距离辅槽制冷关闭不足十分钟，则等待
            //else
            //{
            //    if ((DateTime.Now - ryDeviceM.subCoolCloseTime).TotalMinutes < ryDeviceM.waitingTime)
            //    {
            //        // 暂时先保持关闭，等待满 10 分钟后再打开
            //        ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_0] = false;
            //        ryDeviceM.subCoolWaiting = true;
            //    }
            //    else
            //    {
            //        ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_0] = true;
            //    }
            //}


            // 将继电器状态写入下位机
            WriteRelayDeviceM(true);
        }

        /// <summary>
        /// Control Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void ControlTick(int tic)
        {
            nlogger.Debug("Control Tick: " + tic.ToString() + " ms");

            if (srDevice.CheckStopRunRQT() == true)
            {
                _machine.Fire(Trigger.ForceStop);
                nlogger.Info("系统收到结束指令.");
            }

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
                _machine.Fire(Trigger.AchieveSteady);
                nlogger.Info((_runningParameters.steadyTimeSec / 60).ToString("0") + " 分钟温度波动度满足波动度小于 " + _runningParameters.flucValue.ToString("0.0000") + "℃");
            }
        }

        /// <summary>
        /// TempDown Exit
        /// </summary>
        private void ControlExit()
        {
            nlogger.Debug("Control Exit.");
        }



        /// <summary>
        /// Stable Entry
        /// </summary>
        private void StableEntry()
        {
            nlogger.Debug("Stable Entry.");

            // 首次进入该状态，应改变相应的继电器状态
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_0] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_1] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_2] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_3] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_4] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_5] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_6] = true;

            // 将继电器状态写入下位机
            // 如果出现错误，则通过 _deviceErrorMonitor 记录错误状态
            WriteRelayDeviceM(true);
        }


        /// <summary>
        /// Stable Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void StableTick(int tic)
        {
            nlogger.Debug("Stableick: " + tic.ToString() + " ms");

            if (srDevice.CheckStopRunRQT() == true)
            {
                _machine.Fire(Trigger.ForceStop);
                nlogger.Info("系统收到结束指令.");
            }

            // 状态时间计数器
            currentTemptPointState.stateCounts++;

            // error check
            ErrorCheckBasis();          // 当前温度与设定温度点偏离过大
            ErrorCheckTempFlucLarge();  // 波动度过大


            // 判断 - 测温电桥温度值的波动度满足条件（2 分钟 0.001），则进入测量状态
            if (currentTemptPointState.stateCounts > _runningParameters.bridgeSteadyTimeSec / _runningParameters.readTempIntervalSec)
            {
                bool steady = srDevice.chekFluc(_runningParameters.steadyTimeSec / _runningParameters.readTempIntervalSec, _runningParameters.flucValue);
                if (steady == false) return;
                

                float diff = srDevice.temperatures.Last() - tpDeviceM.temperatures.Last();
                if (Math.Abs(diff) < _runningParameters.temperatureDiff)
                {
                    _machine.Fire(Trigger.StartMeasure);

                    nlogger.Info("温度稳定，并且温差小于 0.01度，进行测量");
                }
                else
                {
                    // 设置主槽 / 辅槽控温设备的参数
                    // 向主槽 / 辅槽控温设备写入全部参数
                    tpDeviceM.tpParamToSet[1] = diff;
                    currentTemptPointState.paramM.CopyTo(tpDeviceM.tpParamToSet, 0);
                    
                    // 将参数更新到下位机
                    // 如果出现错误，则通过 _deviceErrorMonitor 记录错误状态
                    WriteTempDeviceM(true);

                    _machine.Fire(Trigger.NeedModify);

                    nlogger.Info("温度稳定，但温差大于0.01度，进行修正");
                }
            }
        }

        /// <summary>
        /// Stable Exit
        /// </summary>
        private void StableExit()
        {
            nlogger.Debug("Stable Exit.");
        }

        

        /// <summary>
        /// Measure Entry
        /// </summary>
        private void MeasureEntry()
        {
            nlogger.Debug("Measure Entry.");
        }

        /// <summary>
        /// Measure Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void MeasureTick(int tic)
        {
            nlogger.Debug("MeasureTick: " + tic.ToString() + " ms");

            // 状态时间计数器
            currentTemptPointState.stateCounts++;

            // measure


            if (srDevice.CheckStopRunRQT() == true)
            {
                _machine.Fire(Trigger.ForceStop);
                nlogger.Info("系统收到结束指令.");
            }
            else if (srDevice.CheckNextPointRQT(out currentTemptPointState.paramM[0]) == true)
            {
                _machine.Fire(_nextPointTrigger, currentTemptPointState.stateTemp);
                nlogger.Info("系统收到下一个温度点的指令: " + currentTemptPointState.stateTemp.ToString());
            }
            else
            {
                _machine.Fire(Trigger.FinishedAll);

                nlogger.Info("未读取到新的温度点，进入空闲");
            }
        }

        /// <summary>
        /// Measure Exit
        /// </summary>
        private void MeasureExit()
        {
            nlogger.Debug("Measure Exit.");
        }



        /// <summary>
        /// Stop Entry
        /// </summary>
        private void StopEntry()
        {
            nlogger.Debug("Stop Entry.");

            // 关闭除总电源外的所有继电器
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_0] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_1] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_2] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_3] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_4] = false;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_5] = false;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_6] = false;

            // 将继电器状态写入下位机
            // 如果出现错误，则通过 FlowControlFaultOccurEvent 事件通知主界面提示错误
            WriteRelayDeviceM(true);

            // 关机指令

        }

        /// <summary>
        /// Stop Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void StopTick(int tic)
        {
            if(_runningParameters.shutDownComputer == true)
            {
                // 关闭计算机
                System.Diagnostics.Process.Start("shutdown.exe", "-s -t 60");
            }

            _machine.Fire(Trigger.FinishedAll);

            nlogger.Debug("StopTick: " + tic.ToString() + " ms");
        }

        /// <summary>
        /// Stop Exit
        /// </summary>
        private void StopExit()
        {
            nlogger.Debug("Stop Exit.");
        }
    }
}