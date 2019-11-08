using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UserPort
{
    class MySerialClient
    {
        //
        // Topic 的构成形式为： topicDeviceId/SubTopic
        // 也就是说，每制作一台设备时，都会新建一个 DeviceId，该设备下，有两个子主题（Data和Control）
        /// <summary>
        /// 设备序列号 - 在同一类产品中，每个单独的设备都有一个相应的编号
        /// </summary>
        private const string topicDeviceId = "lot_tst";
        /// <summary>
        /// 设备所订阅的主题
        /// </summary>
        private List<string> topicsSubs = new List<string>();


        //
        public delegate void MessageReceievedEventHandler(SubTopic topic, string message);
        /// <summary>
        /// 事件 - 接收到消息
        /// </summary>
        public event MessageReceievedEventHandler MessageReceievedEvent;


        /// <summary>
        /// 是否启用 mqtt
        /// </summary>
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// 查询 mqtt 是否连接成功
        /// </summary>
        public bool isConnected
        {
            get
            {
                return true;
            }
        }



        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public bool Initialize(JObject mqttConfig, params SubTopic[] topics)
        {
            // 清空原有主题
            topicsSubs.Clear();

            // 添加主题
            foreach (var itm in topics)
            {
                topicsSubs.Add(topicDeviceId + "/" + Enum.GetName(typeof(SubTopic), itm));
            }



            return true;

        }


        /// <summary>
        /// 发布主题
        /// </summary>
        /// <param name="Message"></param>
        public void Publish(SubTopic topic, string Message, bool isWait = false)
        {
            if (!Enabled) return;

        }
    }
}
