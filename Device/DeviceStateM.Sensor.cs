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
            if (child.ContainsKey("SensorSBE37"))
            {
                JArray child2 = (JArray)child["SensorSBE37"];

                int numSensor = Math.Min(child2.Count, SensorDeviceBase.MaxSensorNum);

                for (int i = 0; i < numSensor; i++)
                {
                    JObject ob = (JObject)child2[i];
                    SensorSBE37 sr = new SensorSBE37();
                    confOK &= sr.Init(ob);
                    sr.ErrorOccurEvent += SensorDevice_ErrorOccurEvent;

                    srDevices.Add(sr);
                }
            }


            // 配置标准数据采集器
            if (child.ContainsKey("SensorSTD"))
            {
                JObject child2 = (JObject)child["SensorSTD"];

                if (child2.ContainsKey("PortName"))
                {
                    confOK &= sdDevice.Init(child2);
                    sdDevice.ErrorOccurEvent += SensorDevice_ErrorOccurEvent;
                    sdDevice.DataReceivedEvent += SdDevice_DataReceivedEvent;
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
        /// 标准传感器设备接收到数据 - 事件处理函数
        /// </summary>
        /// <param name="data"></param>
        private void SdDevice_DataReceivedEvent(SensorSTDData data)
        {
            SensorSDReceiveDataEvent?.Invoke(data);

            IotSensorValueMessage srVal = new IotSensorValueMessage();
            srVal.SensorData = new List<SensorDataBase>();
            srVal.Topic = IotCS.Client.IotTopic.SensorValue;
            srVal.DorS = IotDorS.Display;
            srVal.SensorData.Add(data);
            _userPorts.PublishMessage(IotCS.Client.IotTopic.SensorValue, JObject.FromObject(srVal));
        }

        /// <summary>
        /// 传感器发生错误事件处理函数
        /// </summary>
        /// <param name="err"></param>
        private void SensorDevice_ErrorOccurEvent(Err_sr err)
        {
            // todo: 设置错误标识码
            SetErrorStatus(ErrorCode.BridgeError);
        }
    }
}
