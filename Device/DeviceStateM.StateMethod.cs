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
            //ErrorCheckOutRange();   // 温度超出界限

            // 驱动状态机执行流程
            currentTemptPointState.stateCounts++;
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

            // 向 mqtt server 发布主题信息
            _userPorts.PublishMessage(UserPort.SubTopic.Data, packageDataJson(), false, UserPort.UserPortType.All);
        }


        /// <summary>
        /// 继电器模块自动连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            // wghou
            // 如果是 ignore 里面的事件，是否会触发 onTransitionAction，也就是计数器会不会清零
            // 至少这样就能知道，一个状态（供氧 / 供氮）多久了
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
        /// Idle Entry
        /// </summary>
        private void IdleEntry()
        {
            nlogger.Trace("Idle Entry.");

            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_7] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_0] = false;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_1] = false;
            WriteRelayDeviceM(true);
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
                    SetErrorStatus(ErrorCode.TemptError);
                    return;
                }

                // 如果当前主槽温度刚好处于温度点附近，且满足阈值条件，则直接进入控温状态
                if (Math.Abs(tpDeviceM.temperatures.Last() - currentTemptPointState.stateTemp) < _runningParameters.controlTempThr)
                {
                    // 状态 - 稳定
                    _machine.Fire(Trigger.StartStand);
                }
                // 当前氧气含量大于设定值，则加氮气
                else if( tpDeviceM.temperatures.Last() > currentTemptPointState.stateTemp)
                {
                    _machine.Fire(Trigger.NeedNitrogen);
                }
                else
                {
                    _machine.Fire(Trigger.NeedNitrogen);
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
        private void AddOxygenEntry()
        {
            // 加氮 5 分钟
            currentTemptPointState.stateHoldCounts = _runningParameters.addGasHoldCounts;


            // 记录当前管路中的气体类型
            _lastGasState = State.AddOxygen;

            // 加氧
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_7] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_0] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_1] = false;
            WriteRelayDeviceM(true);

            nlogger.Trace("TempUp Entry.");
        }

        /// <summary>
        /// TempUp Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void AddOxygenTick(int tic)
        {
            nlogger.Trace("TempUp Tick: " + tic.ToString() + " ms");

            //ErrorCheckTempNotUp();  // 温度不升高

            // 
            if (currentTemptPointState.stateCounts > currentTemptPointState.stateHoldCounts)
            {

                // 如果当前主槽温度刚好处于温度点附近，且满足阈值条件，则直接进入控温状态
                if (Math.Abs(tpDeviceM.temperatures.Last() - currentTemptPointState.stateTemp) < _runningParameters.controlTempThr)
                {
                    // 状态 - 稳定
                    _machine.Fire(Trigger.StartStand);
                }
                // 当前氧气含量大于设定值，则加氮气
                else if (tpDeviceM.temperatures.Last() > currentTemptPointState.stateTemp)
                {
                    _machine.Fire(Trigger.NeedNitrogen);
                }
                else
                {
                    _machine.Fire(Trigger.NeedOxygen);
                }
            }

            return;
        }

        /// <summary>
        /// TempUp Exit
        /// </summary>
        private void AddOxygenExit()
        {
            nlogger.Trace("TempUp Exit.");
        }



        /// <summary>
        /// TempDown Entry
        /// </summary>
        private void AddNitrogenEntry()
        {
            // 加氮 5 分钟
            currentTemptPointState.stateHoldCounts = _runningParameters.addGasHoldCounts;


            // 记录当前管路中的气体类型
            _lastGasState = State.AddNitrogen;

            // 加氮
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_7] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_0] = false;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_1] = true;
            WriteRelayDeviceM(true);

            nlogger.Trace("TempDown Entry.");
        }

        /// <summary>
        /// TempDown Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void AddNitrogenTick(int tic)
        {
            nlogger.Trace("TempDown Tick: " + tic.ToString() + " ms");

            // error check
            //ErrorCheckTempNotDown();    // 温度不降低

            //
            if (currentTemptPointState.stateCounts > currentTemptPointState.stateHoldCounts)
            {

                // 如果当前主槽温度刚好处于温度点附近，且满足阈值条件，则直接进入控温状态
                if (Math.Abs(tpDeviceM.temperatures.Last() - currentTemptPointState.stateTemp) < _runningParameters.controlTempThr)
                {
                    // 状态 - 稳定
                    _machine.Fire(Trigger.StartStand);
                }
                // 当前氧气含量大于设定值，则加氮气
                else if (tpDeviceM.temperatures.Last() > currentTemptPointState.stateTemp)
                {
                    _machine.Fire(Trigger.NeedNitrogen);
                }
                else
                {
                    _machine.Fire(Trigger.NeedOxygen);
                }
            }

            return;
        }

        /// <summary>
        /// TempDown Exit
        /// </summary>
        private void AddNitrogenExit()
        {
            nlogger.Trace("TempDown Exit.");
        }


        /// <summary>
        /// Stand Entry
        /// </summary>
        private void StandEntry()
        {
            // 加氮 5 分钟
            currentTemptPointState.stateHoldCounts = _runningParameters.standHoldCounts;

            nlogger.Trace("Stand Entry.");

            // 首次进入该状态，应改变相应的继电器状态
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_7] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_0] = false;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_1] = false;
            WriteRelayDeviceM(true);
        }


        /// <summary>
        /// Stand Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void StandTick(int tic)
        {
            nlogger.Trace("Stand tick: " + tic.ToString() + " ms");

            // error check
            //ErrorCheckBasis();          // 当前温度与设定温度点偏离过大
            //ErrorCheckTempFlucLarge();  // 波动度过大


            if (currentTemptPointState.stateCounts > currentTemptPointState.stateHoldCounts)
            {

                // 如果当前主槽温度刚好处于温度点附近，且满足阈值条件，则直接进入控温状态
                if (Math.Abs(tpDeviceM.temperatures.Last() - currentTemptPointState.stateTemp) < _runningParameters.controlTempThr)
                {
                    // 状态 - 进入稳定
                    _machine.Fire(Trigger.WaitSteady);
                }
                // 当前氧气含量大于设定值，则加氮气
                else if (tpDeviceM.temperatures.Last() > currentTemptPointState.stateTemp)
                {
                    _machine.Fire(Trigger.NeedNitrogen);
                }
                else
                {
                    _machine.Fire(Trigger.NeedOxygen);
                }
            }
        }

        /// <summary>
        /// Stand Exit
        /// </summary>
        private void StandExit()
        {
            nlogger.Trace("Stand Exit.");
        }


        /// <summary>
        /// Steady Entry
        /// </summary>
        private void SteadyEntry()
        {
            // 加氮 5 分钟
            currentTemptPointState.stateHoldCounts = _runningParameters.standHoldCounts;

            nlogger.Trace("Steady Entry.");

            // 首次进入该状态，应改变相应的继电器状态
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_7] = true;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_0] = false;
            ryDeviceM.ryStatusToSet[(int)RelayDevice.Cmd_r.OUT_1] = false;
            WriteRelayDeviceM(true);
        }


        /// <summary>
        /// Steady Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void SteadyTick(int tic)
        {
            nlogger.Trace("Steady tick: " + tic.ToString() + " ms");

            // error check
            //ErrorCheckBasis();          // 当前温度与设定温度点偏离过大
            //ErrorCheckTempFlucLarge();  // 波动度过大


            if (currentTemptPointState.stateCounts > currentTemptPointState.stateHoldCounts)
            {

                // 如果当前主槽温度刚好处于温度点附近，且满足阈值条件，则直接进入控温状态
                if (Math.Abs(tpDeviceM.temperatures.Last() - currentTemptPointState.stateTemp) < _runningParameters.controlTempThr)
                {
                    // 状态 - 稳定
                    _machine.Fire(Trigger.StartMeasure);
                }
                // 当前氧气含量大于设定值，则加氮气
                else if (tpDeviceM.temperatures.Last() > currentTemptPointState.stateTemp)
                {
                    _machine.Fire(Trigger.NeedNitrogen);
                }
                else
                {
                    _machine.Fire(Trigger.NeedOxygen);
                }
            }
        }

        /// <summary>
        /// Steady Exit
        /// </summary>
        private void SteadyExit()
        {
            nlogger.Trace("Steady Exit.");
        }



        /// <summary>
        /// Measure Entry
        /// </summary>
        private void MeasureEntry()
        {
            // 测量完成，标记
            temperaturePointList[currentTemptPointState.tempPointIndex].finished = true;

            nlogger.Trace("Measure Entry.");
        }

        /// <summary>
        /// Measure Tick
        /// </summary>
        /// <param name="tic"> 时间步长 </param>
        private void MeasureTick(int tic)
        {
            nlogger.Trace("MeasureTick: " + tic.ToString() + " ms");

            // 等待
        }

        /// <summary>
        /// Measure Exit
        /// </summary>
        private void MeasureExit()
        {
            nlogger.Trace("Measure Exit.");
        }


        /// <summary>
        /// 点击按键，进入下一个氧气点
        /// </summary>
        public void ask4nextPoint()
        {
            if (!_machine.IsInState(State.Measure)) return;

            if (_machine.IsInState(State.Measure)) _machine.Fire(Trigger.StartMeasure);
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