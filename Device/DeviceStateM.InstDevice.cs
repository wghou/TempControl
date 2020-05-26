﻿using System;
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
        public List<InstDeviceBase> _instDevices = new List<InstDeviceBase>();
        /// <summary>
        /// 标准数据采集器
        /// </summary>
        public InstSTD sdDeviceRef;
        /// <summary>
        /// 仪器设备的端口
        /// </summary>
        public JArray _instPorts = new JArray();

        /// <summary>
        /// 初始化仪器设备
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        private bool initInstDevices(JArray child)
        {
            try
            {
                // 设置仪器总数量
                int numInst = Math.Min(child.Count, InstDeviceBase.MaxInstNum);
                _instDevices.Clear();

                if (numInst < 1)
                {
                    nlogger.Warn("There is not enough number of ports for Instrument: " + numInst.ToString());
                    return false;
                }

                // 添加标准仪器接口
                InstInfoBase info = new InstInfoBase() {
                                        InstType = TypeInst.Standard,
                                        SensorFlag = TypeSensor.Conduct | TypeSensor.Tempt };
                sdDeviceRef = new InstSTD(info);
                
                sdDeviceRef.ErrorOccurEvent += InstDevice_ErrorOccurEvent;
                sdDeviceRef.DataReceivedEvent += SdDevice_DataReceivedEvent;
                _instDevices.Add(sdDeviceRef);


                // todo: 记录端口号、波特率等
                _instPorts = child;
            }
            catch(Exception ex)
            {
                nlogger.Warn("Expection in : DeviceStateM.initInstDevices(): " + ex.Message);
                return false;
            }

            return true;
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

            try
            {
                // 根据 TestID，从远程数据库查找标准器信息 InstSqlrd，配置 SensorSD
                List<InstSqlrd> instSql = sqlWriter.QueryValue<InstSqlrd>()
                                            .Where(q => q.vTestID.Equals(testId))
                                            .ToList();

                if (instSql.Count == 0) { return false; }

                _instDevices.RemoveRange(1, _instDevices.Count - 1);

                foreach (var itm in instSql)
                {
                    // 根据 TestID 以及 InstrumentID，从远程数据库查找仪器信息 SensorSqlrd，配置 SensorSBE37
                    List<SensorSqlrd> srSql = sqlWriter.QueryValue<SensorSqlrd>()
                                                        .Where(q => q.vInstrumentID.Equals(itm.vInstrumentID))
                                                        .ToList();
                    foreach (var sr in srSql)
                    {
                        itm.sensors.Add(sr);
                    }

                    // 添加各类不同型号仪器对应的接口
                    // todo: 根据 sql 反回的 InstrumentType，新建不同的仪器类型
                    InstSBE ist = new InstSBE(itm);
                    ist.ErrorOccurEvent += InstDevice_ErrorOccurEvent;
                    _instDevices.Add(ist);
                }
            }
            catch(Exception ex)
            {
                nlogger.Error("Exception: " + ex.Message);
                return false;
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
