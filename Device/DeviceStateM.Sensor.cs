using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Others;

namespace Device
{
    public partial class DeviceStateM
    {
        /// <summary>
        /// 传感器设备 - 多个
        /// </summary>
        private List<SensorDevice> srDevices = new List<SensorDevice>();


        /// <summary>
        /// 初始化传感器设备
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        private bool initSensorDevices(JObject child)
        {
            bool confOK = true;

            srDevices.Clear();

            // 设置传感器
            if (child.ContainsKey("Sensor"))
            {
                JObject child2 = (JObject)child["Sensor"];

                int numSensor = Math.Min(child2.Count, SensorDevice.maxSensorNum);

                for (int i = 0; i < numSensor; i++)
                {
                    JObject ob = (JObject)child2[i];
                    SensorDevice sr = new SensorDevice();
                    confOK &= sr.SetPortName(child2.ContainsKey("PortName") ? child2["PortName"].ToString() : "COM0");
                    sr.Enable = child2.ContainsKey("Enable") ? (bool)child2["Enable"] : true;
                    if (!confOK) nlogger.Error("配置辅槽控温设备失败! 端口号: " + sr.srDevicePortName);
                    else nlogger.Debug("配置辅槽控温设备成功! 端口号: " + sr.srDevicePortName);

                    srDevices.Add(sr);
                }
            }

            // 开始识别传感器
            foreach (var itm in srDevices)
            {
                itm.SensorIdentifiedEvent += Itm_SensorIdentifiedEvent;
                itm.startIdentify();
            }

            // 配置标准数据采集器
            if (child.ContainsKey("StandardDev"))
            {
                JObject child2 = (JObject)child["Sensor"];

                if (child2.ContainsKey("PortName"))
                {
                    confOK = sdDevice.SetPortName((string)child2["PortName"]);
                }
                else
                {
                    confOK = false;
                    nlogger.Error("there is no StandardDev.PortName in config.json.");
                }
            }

            return confOK;
        }

        /// <summary>
        /// 传感器设备识别事件
        /// </summary>
        /// <param name="info"></param>
        private void Itm_SensorIdentifiedEvent(SensorDevice.SensorInfo info)
        {
            // todo: 将识别到的传感器信息，发送出去
            //List<SensorDevice.DeviceInfo> states = new List<SensorDevice.DeviceInfo>();
            //SocketSensorMessage srMsg = new SocketSensorMessage(MySocketServer.SocketCmd.SensorInfo);
            //states.Add(info);
            //srMsg.sensorStates = states;
            //_socketServer.pushMessage(srMsg);

            // 同样是触发“识别到传感器事件”
            SensorIdentifiedEvent?.Invoke(info);
        }
    }
}
