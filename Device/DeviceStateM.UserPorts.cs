using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UserPort;
using System.IO;

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
                    if(message.ToUpper() == "START")
                    {
                        if (_state != State.Idle) return;

                        // 从缓存文本中读取温度点
                        try
                        {
                            string[] lines = File.ReadAllLines(@"./params.cache", Encoding.UTF8);
                            for (int i = 0; i < lines.Length; i++)
                            {
                                // 主槽参数
                                string line1 = lines[i];
                                TemptPointStruct ts = new TemptPointStruct();
                                i++;
                                string[] parmM = line1.Split(' ');
                                if (parmM.Length == 7)
                                {
                                    float vl;
                                    if (float.TryParse(parmM[0], out vl)) ts.paramM[0] = vl;
                                    else break;
                                    if (float.TryParse(parmM[1], out vl)) ts.paramM[1] = vl;
                                    else break;
                                    if (float.TryParse(parmM[2], out vl)) ts.paramM[2] = vl;
                                    else break;
                                    if (float.TryParse(parmM[3], out vl)) ts.paramM[3] = vl;
                                    else break;
                                    if (float.TryParse(parmM[4], out vl)) ts.paramM[4] = vl;
                                    else break;
                                    if (float.TryParse(parmM[5], out vl)) ts.paramM[5] = vl;
                                    else break;
                                    if (float.TryParse(parmM[6], out vl)) ts.paramM[6] = vl;
                                    else break;
                                }
                                else
                                {
                                    break;
                                }

                                // 辅槽参数
                                if (i >= lines.Length) { break; }
                                string line2 = lines[i];
                                string[] parmS = line2.Split(' ');
                                if (parmS.Length == 7)
                                {
                                    float vl;
                                    if (float.TryParse(parmS[0], out vl)) ts.paramS[0] = vl;
                                    else break;
                                    if (float.TryParse(parmS[1], out vl)) ts.paramS[1] = vl;
                                    else break;
                                    if (float.TryParse(parmS[2], out vl)) ts.paramS[2] = vl;
                                    else break;
                                    if (float.TryParse(parmS[3], out vl)) ts.paramS[3] = vl;
                                    else break;
                                    if (float.TryParse(parmS[4], out vl)) ts.paramS[4] = vl;
                                    else break;
                                    if (float.TryParse(parmS[5], out vl)) ts.paramS[5] = vl;
                                    else break;
                                    if (float.TryParse(parmS[6], out vl)) ts.paramS[6] = vl;
                                    else break;
                                }
                                else
                                {
                                    break;
                                }
                                // 装入列表中
                                temperaturePointList.Add(ts);
                            }

                            temperaturePointList.Sort((x, y) => x.stateTemp.CompareTo(y.stateTemp));
                            if (_runningParameters.sort != "ascend")
                            {
                                temperaturePointList.Reverse();
                            }
                        }
                        catch (Exception ex)
                        {

                        }


                        this.StartAutoControl();
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

            // 主题
            JProperty tp = new JProperty("Topic", "Data");
            allData.Add(tp);

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
