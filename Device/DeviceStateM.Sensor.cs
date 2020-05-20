using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Others;
using SensorDevice;

namespace Device
{
    public partial class DeviceStateM
    {
        /// <summary>
        /// 传感器设备 - 多个
        /// </summary>
        private List<SensorDeviceBase> srDevices = new List<SensorDeviceBase>();


        /// <summary>
        /// 初始化传感器设备
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        private bool initSensorDevices(JObject child)
        {
            // todo: unhandled exception

            bool confOK = true;

            srDevices.Clear();

            // 设置传感器
            if (child.ContainsKey("Sensors"))
            {
                JArray child2 = (JArray)child["Sensors"];

                int numSensor = Math.Min(child2.Count, SensorDeviceBase.MaxSensorNum);

                for (int i = 0; i < numSensor; i++)
                {
                    JObject ob = (JObject)child2[i];
                    SensorSBE37 sr = new SensorSBE37();
                    confOK &= sr.Init(ob);

                    srDevices.Add(sr);
                }
            }


            // 配置标准数据采集器
            if (child.ContainsKey("StandardDev"))
            {
                JObject child2 = (JObject)child["StandardDev"];

                if (child2.ContainsKey("PortName"))
                {
                    confOK &= sdDevice.Init(child2);
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
        /// 标准传感器设备发生错误 - 事件
        /// </summary>
        private void SdDevice_StandardDeviceErrorEvent()
        {
            // todo: 设置错误标识码
            SetErrorStatus(ErrorCode.BridgeError);
        }
    }
}
