using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Others;
using InstDevice;

namespace Device
{
    public partial class DeviceStateM
    {
        /// <summary>
        /// 仪器设备 - 多个
        /// </summary>
        private List<InstDeviceBase> _instDevices = new List<InstDeviceBase>();


        /// <summary>
        /// 初始化仪器设备
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        private bool initInstDevices(JObject child)
        {
            // todo: unhandled exception

            bool confOK = true;

            _instDevices.Clear();

            // 配置标准数据采集器
            if (child.ContainsKey("InstSTD"))
            {
                JObject child2 = (JObject)child["InstSTD"];

                confOK &= sdDevice.Init(child2);
                sdDevice.ErrorOccurEvent += InstDevice_ErrorOccurEvent;
                sdDevice.DataReceivedEvent += SdDevice_DataReceivedEvent;

                // 添加一个 InstInfo
                InstDeviceBase.InstInfos.Add(new InstInfoBase());
                sdDevice.Link2InstInfo(InstDeviceBase.InstInfos.Count - 1);

                // 使能设备
                if(confOK == true) { sdDevice.Enable = true; }
            }
            else
            {
                confOK = false;
                nlogger.Error("there is no \"InstSTD\" in config.json.");
            }

            // 设置仪器
            if (child.ContainsKey("InstSBE"))
            {
                JArray child2 = (JArray)child["InstSBE"];

                int numInst = Math.Min(child2.Count, InstDeviceBase.MaxInstNum);

                for (int i = 0; i < numInst; i++)
                {
                    JObject ob = (JObject)child2[i];
                    InstSBE ist = new InstSBE();
                    confOK &= ist.Init(ob);
                    ist.ErrorOccurEvent += InstDevice_ErrorOccurEvent;

                    _instDevices.Add(ist);
                }
            }

            return confOK;
        }


        /// <summary>
        /// 从数据库中，读取仪器信息
        /// </summary>
        /// <param name="testId"></param>
        /// <returns></returns>
        private bool getInstInfoFromSql(string testId)
        {
            // 根据 TestID，从远程数据库查找温度点信息 TestOrderSqlrd，配置 SensorDeviceBase.testOrders
            //InstDeviceBase.testOrders = sqlWriter.QueryValue<TestOrderSqlrd>(testIdSql);
            // 温度点列表不能为空
            //if (InstDeviceBase.testOrders.Count == 0)
            //{
            //    msgSend.ExecuteSucceed = false;
            //}

            // 根据 TestID，从远程数据库查找标准器信息 InstSqlrd，配置 SensorSD
            List<InstSqlrd> instSql = sqlWriter.QueryValue<InstSqlrd>(testId);
            if(instSql.Count == 0) { return false; }

            foreach(var itm in instSql)
            {
                // 根据 TestID 以及 InstrumentID，从远程数据库查找仪器信息 SensorSqlrd，配置 SensorSBE37
                List<SensorSqlrd> srSql = sqlWriter.QueryValue<SensorSqlrd>(testId);
                foreach(var sr in srSql)
                {
                    sr.FreshFromSql2Info();
                    itm.sensors.Add(sr);
                }

                itm.FreshFromSql2Info();
                InstDeviceBase.InstInfos.Add(itm);
            }

            return true;
        }


        /// <summary>
        /// 标准仪器设备接收到数据 - 事件处理函数
        /// </summary>
        /// <param name="data"></param>
        private void SdDevice_DataReceivedEvent(InstSTDData data)
        {
            InstSDReceiveDataEvent?.Invoke(data);

            IotInstValueMessage srVal = new IotInstValueMessage();
            srVal.InstData = new List<InstDataBase>();
            srVal.Topic = IotCS.Client.IotTopic.SensorValue;
            srVal.DorS = IotDorS.Display;
            srVal.InstData.Add(data);
            _userPorts.PublishMessage(IotCS.Client.IotTopic.SensorValue, JObject.FromObject(srVal));
        }

        /// <summary>
        /// 仪器发生错误事件处理函数
        /// </summary>
        /// <param name="err"></param>
        private void InstDevice_ErrorOccurEvent(Err_sr err)
        {
            // todo: 设置错误标识码
            SetErrorStatus(ErrorCode.BridgeError);
        }
    }
}
