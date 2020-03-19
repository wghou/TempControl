using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UserPort;

namespace Device
{
    public partial class DeviceStateM
    {
        /// <summary>
        /// 用户接口
        /// </summary>
        private UserPorts _userPorts = new UserPorts();

        public bool isUserPortConnected
        {
            get { return true; }
        }

        /// <summary>
        /// 从用户接口收到数据
        /// </summary>
        /// <param name="Ptype"></param>
        /// <param name="topic"></param>
        /// <param name="message"></param>
        private void _userPorts_UserPortMsgRvEvent(UserPortType Ptype, SubTopic topic, string message)
        {
            nlogger.Info("receive user port message from UserPortType: " + Ptype.ToString());
            nlogger.Info("the message topic is " + topic.ToString() + ", and the message is: " + message);

            switch (topic)
            {
                case SubTopic.Control:
                    if (message.Contains("start"))
                    {
                        // wghou
                        // 添加控温点
                        StartAutoControl();
                    }
                    else if (message.Contains("stop"))
                    {
                        // 保存控温点
                        SuspendAutoControl();
                    }
                    else
                    {
                        nlogger.Warn("Unknow message from control: " + message);
                    }
                    break;

                case SubTopic.Data:

                    break;

                default:
                    Console.WriteLine("Unknown message receieved.");
                    break;
            }
        }

        private string packageDataJson()
        {
            // 所有数据
            JObject allData = new JObject();

            // 主槽温度
            if(tpDeviceM.temperatures.Count != 0)
            {
                JProperty mTp = new JProperty("mTp", tpDeviceM.temperatures.Last()); allData.Add(mTp);
            }
            // 主槽功率
            JProperty mPw = new JProperty("mPw", tpDeviceM.tpPowerShow);
            // 主槽设定值
            JProperty mSt = new JProperty("mSt", tpDeviceM.tpParam[0]);
            // 添加
            allData.Add(mPw); allData.Add(mSt);

            // 辅槽温度
            if (tpDeviceS.temperatures.Count != 0)
            {
                JProperty sTp = new JProperty("sTp", tpDeviceS.temperatures.Last()); allData.Add(sTp);
            }
            // 辅槽功率
            JProperty sPw = new JProperty("sPw", tpDeviceS.tpPowerShow);
            // 辅槽设定值
            JProperty sSt = new JProperty("sSt", tpDeviceS.tpParam[0]);
            // 添加
            allData.Add(sPw); allData.Add(sSt);

            // 继电器 M
            JProperty mRy = new JProperty("mRy", ryDeviceM.ryStatus);
            // 继电器 S
            JProperty sRy = new JProperty("sRy", ryDeviceS.ryStatus);
            // 添加
            allData.Add(mRy); allData.Add(sRy);

            // 错误状态
            string errStr = string.Join(";", GetCurrentErrorStatus().Select(x => Enum.GetName(typeof(ErrorCode), x.Key) + "=" + x.Value.ToString()).ToArray());
            ResetCurrentErrorStatus();
            JProperty err = new JProperty("err", errStr);
            // 添加
            allData.Add(err);

            return allData.ToString();
        }
    }
}
