using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stateless;
using System.Diagnostics;
using System.Timers;
using Newtonsoft.Json.Linq;

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

            // 检查传感器设备的错误状态
            foreach(var itm in srDevices)
            {
                if(itm.FetchErrorStatus()) SetErrorStatus(ErrorCode.SensorError);
            }

            // 驱动状态机执行流程
            currentTemptPointState.stateCounts++;
            _machine.Fire(_TickTrigger, _runningParameters.readTempIntervalSec * 1000);

            // 传感器步骤
            foreach(var itm in srDevices)
            {
                itm._tickTimerSample_Elapsed(sender, e);
            }
            
            // 全局错误信息 - 事件
            uint errCnt = CheckErrorStatus();
            if (errCnt != 0 && errCnt != lastErrCnt)
            {
                ErrorStatusChangedEvent?.Invoke(_deviceErrorMonitor);
            }
            lastErrCnt = errCnt;

            // 定时器事件
            TimerTickEndEvent?.Invoke();
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

            _ryConnectTimer.Start();

            _tickTimerSample.Start();
        }


        /// <summary>
        /// 状态转变事件函数
        /// </summary>
        /// <param name="act"></param>
        private void OnTransitionedAction(StateMachine<State, Trigger>.Transition act)
        {
            nlogger.Debug("On State Transitioned.");

            Trigger trigger = act.Trigger;
            State source = act.Source;
            State dest = act.Destination;

            // 状态计数器 清零
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

            if (tpDeviceM.temperatures.Count == 0)
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
        private void IdleEntry()
        {
            nlogger.Debug("Idle Entry.");

            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_0] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_1] = false;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_2] = false;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_3] = false;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_4] = false;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_5] = false;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_6] = false;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_7] = false;

            WriteRelayDeviceM(true);
        }

        /// <summary>
        /// Idle Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void IdleTick(int tic)
        {
            nlogger.Debug("IdleTick: " + tic.ToString() + " ms");
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
            nlogger.Debug("StartTick: " + tic.ToString() + " ms");

            // 如果 temperaturePointList 为空
            if (temperaturePointList.Count == 0)
            {
                _machine.Fire(Trigger.FinishedAll);
                return;
            }

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
                    _machine.Fire(Trigger.SuspendAutoControl);
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
            nlogger.Debug("Start Exit.");
        }


        /// <summary>
        /// TempUp Entry
        /// </summary>
        private void TempUpEntry()
        {
            nlogger.Debug("TempUp Entry.");

            // 升温
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_0] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_1] = false;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_2] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_3] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_4] = false;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_5] = false;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_6] = false;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_7] = false;

            WriteRelayDeviceM(true);

            // 设置主槽 / 辅槽控温设备的参数
            //currentTemptPointState.paramM[1] = 0.0f;    // 将修正值清零
            currentTemptPointState.paramM.CopyTo(tpDeviceM.tpParamToSet, 0);
            //currentTemptPointState.paramS[1] = 0.0f;    // 将修正值清零
            currentTemptPointState.paramS.CopyTo(tpDeviceS.tpParamToSet, 0);
            // 将参数更新到下位机
            // 如果出现错误，则由 _deviceErrorMonitor 记录错误状态
            WriteTempDeviceM(true);
            WriteTempDeviceS(true);
        }

        /// <summary>
        /// TempUp Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void TempUpTick(int tic)
        {
            nlogger.Debug("TempUp Tick: " + tic.ToString() + " ms");

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
            nlogger.Debug("TempDown Entry.");

            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_0] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_1] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_2] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_3] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_4] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_5] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_6] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_7] = true;

            WriteRelayDeviceM(true);

            // 向主槽 / 辅槽控温设备写入全部参数
            //currentTemptPointState.paramM[1] = 0.0f;    // 将修正值清零
            currentTemptPointState.paramM.CopyTo(tpDeviceM.tpParamToSet, 0);
            //currentTemptPointState.paramM[1] = 0.0f;    // 将修正值清零
            currentTemptPointState.paramS.CopyTo(tpDeviceS.tpParamToSet, 0);
            // 将参数更新到下位机
            // 如果出现错误，则通过 _deviceErrorMonitor 记录错误状态
            WriteTempDeviceM(true);
            WriteTempDeviceS(true);
        }

        /// <summary>
        /// TempDown Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void TempDownTick(int tic)
        {
            nlogger.Debug("TempDown Tick: " + tic.ToString() + " ms");

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
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_1] = false;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_2] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_3] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_4] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_5] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_6] = false;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_7] = false;

            // 温度点低于 5 度时，保持 辅槽快冷打开
            if (currentTemptPointState.stateTemp <= _runningParameters.subCoolFNotDownThr)
                ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_6] = true;

            WriteRelayDeviceM(true);

            // 设置主槽 / 辅槽控温设备的参数
            //currentTemptPointState.paramM[1] = 0.0f;    // 将修正值清零
            currentTemptPointState.paramM.CopyTo(tpDeviceM.tpParamToSet, 0);
            //currentTemptPointState.paramS[1] = 0.0f;    // 将修正值清零
            currentTemptPointState.paramS.CopyTo(tpDeviceS.tpParamToSet, 0);
            // 将参数更新到下位机
            // 如果出现错误，则由 _deviceErrorMonitor 记录错误状态

            WriteTempDeviceM(true);
            WriteTempDeviceS(true);

            // 自动采样
            if(currentTemptPointState.autoSample == true)
            {
                if (_sampleMachine.IsInState(AutoSample.StateSample.Normal)) _sampleMachine.Fire(AutoSample.TriggerSample.ClickFist);
                else
                {
                    // 自动采样出现问题
                    _sampleMachine.Fire(AutoSample.TriggerSample.ForceStop);
                    _sampleMachine.Fire(AutoSample.TriggerSample.ClickFist);
                    nlogger.Error("在自动控温中，自动采样出现问题：_sampleMachine.IsInState(Normal)");
                    SetErrorStatus(ErrorCode.CodeError);
                }
            }
        }

        /// <summary>
        /// Control Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void ControlTick(int tic)
        {
            nlogger.Debug("Control Tick: " + tic.ToString() + " ms");

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
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_1] = false;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_2] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_3] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_4] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_5] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_6] = false;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_7] = false;

            // 温度点低于 5 度时，保持 辅槽快冷打开
            if (currentTemptPointState.stateTemp <= _runningParameters.subCoolFNotDownThr)
                ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_6] = true;

            WriteRelayDeviceM(true);
        }


        /// <summary>
        /// Stable Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void StableTick(int tic)
        {
            nlogger.Debug("Stableick: " + tic.ToString() + " ms");

            // error check
            ErrorCheckBasis();          // 当前温度与设定温度点偏离过大
            ErrorCheckTempFlucLarge();  // 波动度过大

            // 判断 - 测温电桥温度值的波动度满足条件（2 分钟 0.001），则进入测量状态
            if (currentTemptPointState.stateCounts > _runningParameters.bridgeSteadyTimeSec / _runningParameters.readTempIntervalSec)
            {
                // 电桥自检正常。。。
                bool steady = tpDeviceM.checkFlucCount(_runningParameters.bridgeSteadyTimeSec / _runningParameters.readTempIntervalSec, _runningParameters.flucValue);
                if (steady)
                {
                    // 温度稳定度达到了要求，进入下一个状态 - 测量
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
            nlogger.Debug("Stable Exit.");
        }



        /// <summary>
        /// Measure Entry
        /// </summary>
        private void MeasureEntry()
        {
            nlogger.Debug("Measure Entry.");

            // 如果需要自动采样
            if (temperaturePointList[currentTemptPointState.tempPointIndex].autoSample == true)
            {
                if (_sampleMachine.IsInState(AutoSample.StateSample.Prepare_2)) _sampleMachine.Fire(AutoSample.TriggerSample.ClickSecond);
                else
                {
                    nlogger.Error("自动控温流程中，自动采样失败： _sampleMachine.IsInState(Normal)");
                    _sampleMachine.Fire(AutoSample.TriggerSample.ForceStop);
                    SetErrorStatus(ErrorCode.CodeError);
                }
            }

            // 开始传感器测量温度
            foreach (var itm in srDevices)
            {
                itm.startMeasure(currentTemptPointState.stateTemp);
            }
        }

        /// <summary>
        /// Measure Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void MeasureTick(int tic)
        {
            nlogger.Debug("MeasureTick: " + tic.ToString() + " ms");

            // measure
            // 电导率测量
            // 海水取样

            // 等待 xx 分钟后，第二次点击
            if (temperaturePointList[currentTemptPointState.tempPointIndex].autoSample == true)
            {
                if (_sampleMachine.IsInState(AutoSample.StateSample.OnSample)) return;
                else if (!_sampleMachine.IsInState(AutoSample.StateSample.Normal)){
                    nlogger.Error("自动控温中，自动采样出现错误：_sampleMachine.IsInState(OnSample or Normal)");
                    _sampleMachine.Fire(AutoSample.TriggerSample.ForceStop);
                }
            }

            

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
                    _machine.Fire(Trigger.ForceShutdownPC);
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
            nlogger.Debug("Measure Exit.");
        }



        /// <summary>
        /// ShutdownPC Entry
        /// </summary>
        private void ShutdownPCEntry()
        {
            nlogger.Debug("Stop Entry.");

            // 关闭所有继电器
            ryDeviceM.closeDevice();
            ryDeviceS.closeDevice();
        }

        /// <summary>
        /// ShutdownPC Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void ShutdownPCTick(int tic)
        {
            nlogger.Debug("StopTick: " + tic.ToString() + " ms");

            System.Diagnostics.Process.Start("shutdown.exe", "-s -t 60");

            _machine.Fire(Trigger.FinishedAll);
        }

        /// <summary>
        /// ShutdownPC Exit
        /// </summary>
        private void ShutdownPCExit()
        {
            nlogger.Debug("Stop Exit.");
        }
    }
}