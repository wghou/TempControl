using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stateless;
using System.Timers;
using NLog;

namespace InstDevice
{
    /// <summary>
    /// 仪器当前的状态
    /// </summary>
    public enum StateInst : int
    {
        /// <summary> 空闲 </summary>
        Idle = 0,
        /// <summary> 测量数据 </summary>
        Measure,
        /// <summary> 存储数据 </summary>
        Store
    }

    public enum TriggerInst : int
    {
        /// <summary> 时刻 </summary>
        TimerTick,
        /// <summary> 开始测量 </summary>
        StartMeasure,
        /// <summary> 开始存储数据 </summary>
        StartStore,
        /// <summary> 停止 </summary>
        Stop
    }

    /// <summary>
    /// 仪器基类-加入了状态机
    /// </summary>
    public abstract partial class InstDeviceStateM<TData, TInfo> : InstDeviceBase
        where TData : InstDataBase, new()
        where TInfo : InstInfoBase
    {
        /// <summary>
        /// 设备线程锁，同一时间只允许单一线程访问设备资源（串口 / 数据）
        /// </summary>
        protected object sdLocker = new object();
        /// <summary>
        /// 仪器数据缓存
        /// </summary>
        protected List<TData> _instData = new List<TData>();
        /// <summary>
        /// 用于存储的缓存数据
        /// </summary>
        protected List<TData> _storeCache = new List<TData>();
        private int dataMaxLen = 1000;
        /// <summary>
        /// 设备线程锁，同一时间只允许单一线程访问设备资源（串口 / 数据）
        /// </summary>
        protected object srLocker = new object();
        /// <summary>
        /// 数据通信指令及解析类
        /// </summary>
        protected ICmdChain<TData> cmdChain;

        /// <summary>
        /// 设备的信息 Info
        /// </summary>
        public TInfo Info
        {
            get
            {
                if (_infoIdx < 0 || _infoIdx >= InstInfos.Count) return null;
                else { return InstInfos[_infoIdx] as TInfo; }
            }
        }


        /// <summary>
        /// 状态机类
        /// </summary>
        protected StateMachine<StateInst, TriggerInst> _instMachine;
        /// <summary>
        /// 当前处于的状态
        /// </summary>
        public StateInst _instState { set; get; } = StateInst.Idle;
        /// <summary>
        /// 时刻Trigger - 带参数 ms
        /// </summary>
        protected StateMachine<StateInst, TriggerInst>.TriggerWithParameters<int> _instTickTrigger;

        /// <summary>
        /// 定时器
        /// </summary>
        protected Timer _tickTimerInst;
        /// <summary>
        /// 自动取样 - 状态计数器
        /// </summary>
        protected uint _instStateCounts = 0;

        /// <summary>
        /// 当前需要测量的温度点
        /// </summary>
        protected float currentTemptPoint = 0.0f;


        public InstDeviceStateM()
        {
            ConfigInstStateless();        }

        /// <summary>
        /// 执行读取信息步骤
        /// </summary>
        /// <returns></returns>
        public override bool StartMeasure()
        {
            if(_instState != StateInst.Idle)
            {
                return false;
            }

            // 开始测量
            _instMachine.Fire(TriggerInst.StartMeasure);
            return true;
        }
        /// <summary>
        /// 开始存储数据
        /// </summary>
        /// <returns></returns>
        public override bool StartStore()
        {
            if (_instState != StateInst.Measure)
            {
                return false;
            }

            // 停止测量，并进入存储数据
            _instMachine.Fire(TriggerInst.StartStore);
            // 执行一次步骤
            _instMachine.Fire(_instTickTrigger, 10);
            return true;
        }
        /// <summary>
        /// 停止测量数据
        /// </summary>
        /// <returns></returns>
        public override bool StopMeasure()
        {
            _instMachine.Fire(TriggerInst.Stop);
            return true;
        }
        /// <summary>
        /// 返回当前仪器的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<TData> GetInstData()
        {
            return _instData;
        }


        /// <summary>
        /// 执行 Idle 步骤
        /// </summary>
        protected virtual void internalIdleStep() { }
        /// <summary>
        /// 进入 Measure 步骤
        /// </summary>
        protected virtual void internalEnterMeasureStep() {
            // 清空数据缓存
            _storeCache.Clear();
            cmdChain.ResetCmd();
        }
        /// <summary>
        /// 执行 Measure 步骤
        /// </summary>
        protected virtual void internalMeasureStep() {
            // 设备未启用
            if (Enable == false) return;

            // todo: 发送指令等
            bool rlt = sendCMD(cmdChain.FetchNextCmd());
            if(rlt == false)
            {
                // 发生了错误
                OnErrorOccur(Err_sr.Error);
            }
        }
        /// <summary>
        /// 进入 Store 步骤
        /// </summary>
        protected virtual void internalEnterStoreStep() {
            
        }
        /// <summary>
        /// 执行 Store 步骤
        /// </summary>
        protected virtual void internalStoreStep() {
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
        /// 处理串口接收到的数据
        /// </summary>
        /// <param name="str"></param>
        public override void internalProceedReceivedData(string str)
        {
            // 设备未启用
            if (Enable == false) return;

            // todo: 解析数据
            try
            {
                TData dt;
                bool rlt = cmdChain.ResolveData(str, out dt);
                if(rlt == false)
                { // 触发错误产生事件
                    base.OnErrorOccur(Err_sr.Error);
                    return;
                }
                if (dt == null) return;
                
                // 只有在 Measure 状态，才会存储数据
                if (_instState == StateInst.Measure)
                {
                    appendStoreCache(dt);
                }
                // 记录当前数据
                appendInstData(dt);

                // 触发数据接收事件
                OnDataReceived(dt);
            }
            catch (Exception ex)
            {
                nlogger.Error("标准数据采集器设备接受数据发生异常：" + ex.Message);
                // 触发错误产生事件
                base.OnErrorOccur(Err_sr.Error);
            }
        }


        /// <summary>
        /// 配置仪器相关的状态机
        /// </summary>
        protected void ConfigInstStateless()
        {
            // new object
            _instMachine = new StateMachine<StateInst, TriggerInst>(() => _instState, s => _instState = s);
            _instTickTrigger = _instMachine.SetTriggerParameters<int>(TriggerInst.TimerTick);

            // on transition action
            _instMachine.OnTransitioned(instOnTransitionedAction);

            // on unhandled trigger
            _instMachine.OnUnhandledTrigger(instOnUnhandledTrigger);

            // StateInst.Idle
            // -> StateInst.Measure
            _instMachine.Configure(StateInst.Idle)
                .OnEntry(t => instIdleEntry())
                .OnExit(t => instIdleExit())
                .InternalTransition(_instTickTrigger, (tic, t) => instIdleTick(tic))
                .Permit(TriggerInst.StartMeasure, StateInst.Measure)
                .Ignore(TriggerInst.StartStore)
                .Ignore(TriggerInst.Stop);


            // StateInst.Measure
            // -> StateInst.StoreData
            // -> StateInst.Idle
            _instMachine.Configure(StateInst.Measure)
                .OnEntry(t => instMeasureEntry())
                .OnExit(t => instMeasureExit())
                .InternalTransition(_instTickTrigger, (tic, t) => instMeasureTick(tic))
                .Permit(TriggerInst.StartStore, StateInst.Store)
                .Permit(TriggerInst.Stop, StateInst.Idle);


            // StateInst.Store
            // -> StateInst.Idle
            _instMachine.Configure(StateInst.Store)
                .OnEntry(t => instStoreEntry())
                .OnExit(t => instStoreExit())
                .InternalTransition(_instTickTrigger, (tic, t) => instStoreTick(tic))
                .Permit(TriggerInst.Stop, StateInst.Idle)
                .Ignore(TriggerInst.StartMeasure);
        }

        /// <summary>
        /// 仪器单步执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _tickTimerSample_Elapsed(object sender, ElapsedEventArgs e)
        {
            // 计数
            _instStateCounts++;

            _instMachine.Fire(_instTickTrigger, 10);
        }


        /// <summary>
        /// 状态转变事件函数
        /// </summary>
        /// <param name="act"></param>
        protected void instOnTransitionedAction(StateMachine<StateInst, TriggerInst>.Transition act)
        {
            nlogger.Debug("On StateInst Transitioned.");

            TriggerInst trigger = act.Trigger;
            StateInst source = act.Source;
            StateInst dest = act.Destination;

            // 状态清零
            _instStateCounts = 0;

            // 触发事件 - 状态转变
            StateChangedEvent?.Invoke(dest);
        }


        /// <summary>
        /// 未定义事件函数
        /// </summary>
        /// <param name="st"></param>
        /// <param name="tg"></param>
        protected void instOnUnhandledTrigger(StateInst st, TriggerInst tg)
        {
            nlogger.Error("Inst Unhandled trigger: state.");
        }


        /// <summary>
        /// Inst Idle Entry
        /// </summary>
        protected void instIdleEntry()
        {
            nlogger.Debug("Inst Idle Entry.");
        }

        /// <summary>
        /// Inst Idle Tick
        /// </summary>
        /// <param name="tic"></param>
        protected void instIdleTick(int tic)
        {
            nlogger.Debug("Inst Idle Tick: " + tic.ToString() + " ms");

            // 执行 idle 步骤
            internalIdleStep();
        }

        /// <summary>
        /// Inst Idle Exit
        /// </summary>
        protected void instIdleExit()
        {
            nlogger.Debug("Inst Idle Exit.");
        }


        /// <summary>
        /// Inst Measure Entry
        /// </summary>
        protected void instMeasureEntry()
        {
            nlogger.Debug("Inst Measure Entry.");

            // 进入 store 步骤
            internalEnterMeasureStep();
        }

        /// <summary>
        /// Inst Measure Tick
        /// </summary>
        /// <param name="tic"></param>
        protected void instMeasureTick(int tic)
        {
            nlogger.Debug("Inst Measure Tick: " + tic.ToString() + " ms");

            // 执行 Measure 步骤
            internalMeasureStep();
        }

        /// <summary>
        /// instrInstument Measure Exit
        /// </summary>
        protected void instMeasureExit()
        {
            nlogger.Debug("Inst Measure Exit.");
        }


        /// <summary>
        /// Inst Store Entry
        /// </summary>
        protected void instStoreEntry()
        {
            nlogger.Debug("Inst Store Entry.");

            // 进入 store 步骤
            internalEnterStoreStep();
        }

        /// <summary>
        /// Inst Store Tick
        /// </summary>
        /// <param name="tic"></param>
        protected void instStoreTick(int tic)
        {
            nlogger.Debug("Inst Store Tick: " + tic.ToString() + " ms");

            // 执行 store 步骤
            internalStoreStep();
        }

        /// <summary>
        /// Inst Store Exit
        /// </summary>
        protected void instStoreExit()
        {
            nlogger.Debug("Inst Store Exit.");
        }

        /// <summary>
        /// 向设备发送指令
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns> 是否发送成功？true: 发送成功；false: 发送失败 </returns>
        private bool sendCMD(string cmd)
        {
            // 空指令，不发送
            if(cmd == string.Empty) { return true; }

            try
            {
                // 打开串口
                if (!sPort.IsOpen) this.sPort.Open();
                // 写入指令
                this.sPort.WriteLine(cmd);
            }
            catch (Exception ex)
            {
                nlogger.Error("仪器设备读取参数失败！");
                // 关闭串口
                try { sPort.Open(); } catch { }

                return false;
            }
            return true;
        }

        /// <summary>
        /// 向 _instData 值列表中添加数据
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="val"></param>
        protected void appendInstData(TData val)
        {
            if (_instData.Count == dataMaxLen)
            {
                _instData.RemoveAt(0);
            }
            _instData.Add(val);
        }

        /// <summary>
        /// 向 _storeCache 值列表中添加数据
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="val"></param>
        protected void appendStoreCache(TData val)
        {
            if (_storeCache.Count == dataMaxLen)
            {
                _storeCache.RemoveAt(0);
            }
            _storeCache.Add(val);
        }
    }
}
