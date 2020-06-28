using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device.IotPort
{
    public partial class IotPortServer
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public bool Init()
        {
            try
            {
                sPort.BaudRate = 9600;

                sPort.DataReceived += SPort_DataReceived;

                sPort.Open();
            }
            catch(Exception ex)
            {

            }

            _timer.AutoReset = false;
            _timer.Interval = 5000 * 2;
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();

            return true;
        }


        /// <summary>
        /// 计算并获取温度波动值
        /// </summary>
        /// <param name="count">温度监测次数</param>
        /// <param name="fluctuation">温度波动值</param>
        /// <returns>返回成功与否</returns>
        public bool GetFluc(int count, out float fluctuation)
        {
            if (temperatures.Count == 0 || temperatures.Count < count)
            {
                // If there is not temperature data in list, output extreme fluctuation
                fluctuation = -1;
                return false;
            }
            else
            {
                fluctuation = temperatures.GetRange(temperatures.Count - count, count).Max() -
                    temperatures.GetRange(temperatures.Count - count, count).Min();
                return true;
            }
        }
    }
}
