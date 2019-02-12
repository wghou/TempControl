using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device
{
    public partial class DeviceStateM
    {

        ///////////////////////////////////////////////////
        // public interface

        /// <summary>
        /// 查询控温点列表中的完成状态
        /// </summary>
        /// <param name="st"></param>
        public void CheckTpListFinishedStatus(out List<bool> st)
        {
            st = new List<bool>();
            lock (stepLocker)
            {
                // 获取当前温度点列表中的完成状态
                for (int i = 0; i < temperaturePointList.Count; i++)
                {
                    st.Add(temperaturePointList[i].finished);
                }
            }
        }
    }
}
