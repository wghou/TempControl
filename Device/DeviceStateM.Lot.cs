using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using LotClient;

namespace Device
{
    public partial class DeviceStateM
    {
        private MyMqttClient _lotClient = new MyMqttClient();

        /// <summary>
        /// 是否与 mqtt server 连接成功
        /// </summary>
        public bool isMqttConnected { get { return _lotClient.isConnected; } }

        public void setMqttEnable(bool st)
        {
            _lotClient.Enabled = st;
        }

        private bool setupLotClient()
        {
            _lotClient.Initialize(@"./lotConfig.json", MyMqttClient.SubTopic.Control);

            _lotClient.MessageReceievedEvent += LotClient_MessageReceievedEvent;

            return true;
        }

        /// <summary>
        /// 接收到 Server 消息
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="message"></param>
        private void LotClient_MessageReceievedEvent(MyMqttClient.SubTopic topic, string message)
        {
            switch (topic)
            {
                case MyMqttClient.SubTopic.Control:

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
            JProperty mTp = new JProperty("mTp",tpDeviceM.temperatures.Last());
            // 主槽功率
            JProperty mPw = new JProperty("mPw", tpDeviceM.tpPowerShow);
            // 主槽设定值
            JProperty mSt = new JProperty("mSt", tpDeviceM.tpParam[0]);
            // 添加
            allData.Add(mTp); allData.Add(mPw); allData.Add(mSt);

            // 辅槽温度
            JProperty sTp = new JProperty("sTp", tpDeviceS.temperatures.Last());
            // 辅槽功率
            JProperty sPw = new JProperty("sPw", tpDeviceS.tpPowerShow);
            // 辅槽设定值
            JProperty sSt = new JProperty("sSt", tpDeviceS.tpParam[0]);
            // 添加
            allData.Add(sTp); allData.Add(sPw); allData.Add(sSt);

            // 继电器 M
            JProperty mRy = new JProperty("mRy", ryDeviceM.ryStatus);
            // 继电器 S
            JProperty sRy = new JProperty("sRy", ryDeviceS.ryStatus);
            // 添加
            allData.Add(mRy); allData.Add(sRy);

            // 错误状态
            string errStr = string.Join(";", _deviceErrorMonitor.Select(x => Enum.GetName(typeof(ErrorCode), x.Key) + "=" + x.Value.ToString()).ToArray());
            JProperty err = new JProperty("err", errStr);
            // 添加
            allData.Add(err);

            return allData.ToString();
        }
    }
}
