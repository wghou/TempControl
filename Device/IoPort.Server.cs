using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Timers;

namespace Device.IotPort
{
    public partial class IotPortServer
    {
        private SerialPort sPort = new SerialPort();
        private const int baudrate = 9600;
        private const int dataBits = 8;
        private const StopBits stopBits = StopBits.One;
        private const Parity parity = Parity.None;
        private const int readBufferSize = 64;
        private const int writeBufferSize = 64;
        private const int readTimeout = 500;

        private Timer _timer = new Timer();

        /// <summary>
        /// 温度值
        /// </summary>
        public List<float> temperatures = new List<float>();
        private int tempMaxLen = 1000;

        // 用于显示温度曲线的，只保存最新的数据，可以被清空
        public object tpShowLocker = new object();
        public List<float> temperaturesShow = new List<float>();
        /// <summary>
        /// 当前控温温度点
        /// </summary>
        public float currentTemptPoint = 0.0f;
        /// <summary>
        /// 无错误计数
        /// </summary>
        public int noErrCnt = 0;

        /// <summary>
        /// 串口接收到数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            string data = "";
            IotPortEventArgs arg = new IotPortEventArgs();
            try
            {
                data = sPort.ReadTo(CmdEndStr);
                sPort.DiscardInBuffer();

                string[] SplitStr = data.Split(CmdSpChar);

                switch (SplitStr[0].ToUpper())
                {
                    case "NONE":
                        arg.cmd = IotCmd.None;
                        break;

                    case "START":
                        arg.cmd = IotCmd.Start;
                        break;

                    case "TEMPT":
                        arg.cmd = IotCmd.Tempt;
                        float val = float.Parse(SplitStr[1]);
                        arg.value = val;
                        AddTemperature(val);
                        break;

                    case "POINT":
                        arg.cmd = IotCmd.Point;
                        float val2 = float.Parse(SplitStr[1]);
                        arg.value = val2;
                        currentTemptPoint = val2;
                        break;

                    case "STOP":
                        arg.cmd = IotCmd.Stop;
                        break;

                    default:
                        arg.cmd = IotCmd.None;
                        arg.err = IotError.UnknowCmd;
                        break;
                }
            }
            catch(Exception ex)
            {
                arg.err = IotError.Exception;
            }

            try
            {
                sPort.Write(CmdStr[(int)arg.cmd] + "@\r\n");
            }
            catch(Exception ex)
            {

            }
            

            // 触发事件处理函数
            this.MsgReceivedEvent?.Invoke(arg);
        }

        /// <summary>
        /// 向温度值列表中添加温度值
        /// </summary>
        /// <param name="val">温度值</param>
        private void AddTemperature(float val)
        {
            if (temperatures.Count == tempMaxLen)
            {
                temperatures.RemoveAt(0);
            }
            temperatures.Add(val);

            // 添加温度值，用于温度曲线显示
            lock (tpShowLocker)
            {
                if (temperaturesShow.Count == tempMaxLen)
                {
                    temperaturesShow.RemoveAt(0);
                }
                temperaturesShow.Add(val);
            }

            // 无错误计数
            _timer.Stop();
            if(++noErrCnt > tempMaxLen)
            {
                noErrCnt = tempMaxLen;
            }
            _timer.Start();
        }

        /// <summary>
        /// 看门狗定时器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ErrorOccurEvent?.Invoke(IotError.TimeOut);
            noErrCnt = 0;
        }
    }
}
