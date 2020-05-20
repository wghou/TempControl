using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ComTest
{
    public partial class Form1
    {
        /// <summary>
        /// 用于标准传感器设备定时发送消息的计时器
        /// </summary>
        private Timer sdTimer = new Timer();
        /// <summary>
        /// 数据连续传输计数器
        /// </summary>
        private int continuityCnt = 0;


        /// <summary>
        /// 初始化标准传感器设备
        /// </summary>
        private void InitStandardDevice()
        {
            sdTimer.Interval = 2000;
            sdTimer.AutoReset = true;
            sdTimer.Elapsed += SdTimer_Elapsed;
            sdTimer.Start();
        }


        /// <summary>
        /// 发送标准传感器设备数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SdTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            sPortSTD.WriteLine("12.000000-12.000000-12.000000-12.000000-12.000000-" + continuityCnt.ToString() + "\r\n");
            
            // 连续标志位累进
            continuityCnt += 24;
            if (continuityCnt > 255) continuityCnt = 0;
        }
    }
}
