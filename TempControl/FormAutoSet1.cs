using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace TempControl
{
    public partial class FormAutoSet1 : Form
    {
        // 设备
        Device.DeviceStateM devicesAll;
        /// <summary>
        /// 自动控制流程的温度点 - 仅用于显示
        /// </summary>
        BindingList<ParamShow> BList = new BindingList<ParamShow>();
        /// <summary>
        /// 温度点参数列表
        /// </summary>
        List<TempParam> paramList = new List<TempParam>();

        /// <summary>
        /// 是否已开始自动控温流程
        /// </summary>
        bool flowStart = false;

        public delegate void SetAutoButtonEventHandler(bool st);
        public event SetAutoButtonEventHandler SetAutoButtonEvent;


        private TextBox tx = null;
        private TextBox[] paramMTextBox = new TextBox[3];
        private float[] paramMCache = new float[3];

        public FormAutoSet1(Device.DeviceStateM dev)
        {
            InitializeComponent();
            devicesAll = dev;
            paramMTextBox[0] = textBox_tpSetM;
            paramMTextBox[1] = textBox_advanceM;
            paramMTextBox[2] = textBox_fuzzyM;

            this.timer1.Interval = 10000;
            this.timer1.Tick += Timer1_Tick;
            this.backgroundWorker1.DoWork += BackgroundWorker1_DoWork;
        }

        // 间隔一段时间，查询
        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            List<bool> finishedStatus = new List<bool>();
            lock (devicesAll.stepLocker)
            {
                // 获取当前温度点列表中的完成状态
                for (int i = 0; i < devicesAll.temperaturePointList.Count; i++)
                {
                    finishedStatus.Add(devicesAll.temperaturePointList[i].finished);
                }
            }
            if (finishedStatus.Count != paramList.Count)
                return;

            // 将温度点状态更新到显示列表中
            bool changed = false;
            for (int i = 0; i < paramList.Count; i++)
            {
                if (paramList[i].finished != finishedStatus[i])
                {
                    // 更新状态
                    changed = true;
                    paramList[i].finished = finishedStatus[i];
                }
            }
            // 如果有改变，则更新显示
            if (changed) this.BeginInvoke(new EventHandler(delegate { updateDataGridView(); }));

            if (changed) Debug.WriteLine("FormAuto 状态更新定时器，状态已更新.");
            else Debug.WriteLine("FormAuto 状态更新定时器，状态未更新.");
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            this.backgroundWorker1.RunWorkerAsync();
        }

        private void updateDataGridView()
        {
            // 排序
            if (devicesAll._runningParameters.sort == "ascend")
            {
                // 升序
                paramList.Sort();
            }
            else
            {
                // 降序
                paramList.Sort();
                paramList.Reverse();
            }

            // 将温度点数值显示在 DataGridView 中
            BList.Clear();
            for (int i = 0; i < paramList.Count; i++)
            {
                ParamShow ps1 = new ParamShow();
                ps1._index = i + 1;
                if (paramList[i].finished == true)
                    ps1._finish = "已测量";
                else
                    ps1._finish = "未测量";
                paramList[i].paramM.CopyTo(ps1.param, 0);
                BList.Add(ps1);
            }
            dataGridView1.ClearSelection();
        }

        // 窗体载入函数
        private void FormAutoSet1_Load(object sender, EventArgs e)
        {
            index.DataPropertyName = "Index";

            tpSet.DataPropertyName = "TemptSet";
            advance.DataPropertyName = "TempAdjust";
            fuzzy.DataPropertyName = "Advance";
            edit.DataPropertyName = "Edit";
            finish.DataPropertyName = "Finish";


            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = BList;


            lock (this.devicesAll.stepLocker)
            {
                // 判断温控列表中是否有温度点
                if (devicesAll.temperaturePointList.Count != 0)
                {
                    // 将 devicesAll.temperaturePointList 中未测量完成的温度点
                    foreach (var st in devicesAll.temperaturePointList)
                    {
                        TempParam ts = new TempParam();
                        st.paramM.CopyTo(ts.paramM, 0);
                        ts.finished = st.finished;
                        paramList.Add(ts);
                    }

                    // 输入窗口的默认数值
                    //devicesAll.tpDeviceM.tpParam.CopyTo(paramMCache, 0);
                    Array.Copy(devicesAll.tpDeviceM.tpParam, paramMCache, 3);

                    paramMTextBox[1].Text = paramMCache[1].ToString("0.000");
                    paramMTextBox[2].Text = paramMCache[2].ToString("0.000");
                }
                else
                {
                    // 从缓存文本中读取温度点
                    try
                    {
                        string[] lines = File.ReadAllLines(@"./params.cache", Encoding.UTF8);
                        for (int i = 0; i < lines.Length; i++)
                        {
                            // 主槽参数
                            string line1 = lines[i];
                            TempParam ts = new TempParam();
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
                            }
                            else
                            {
                                break;
                            }
                            // 装入列表中
                            paramList.Add(ts);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }

                // 判断是否已开始自动控温流程
                if (this.devicesAll._state == Device.State.Start ||
                        devicesAll._state == Device.State.AddNitrogen ||
                        devicesAll._state == Device.State.AddOxygen ||
                        devicesAll._state == Device.State.Stand ||
                        devicesAll._state == Device.State.Measure)
                {
                    // 正在自动控温
                    this.checkBox_start.Checked = true;
                    this.checkBox_start.Text = "停止";
                    flowStart = true;
                    SetAutoButtonEvent?.Invoke(true);

                    // 判断是否已设定完成实验测量后关机
                    this.checkBox_shutDown.Checked = devicesAll._runningParameters.shutDownComputer;
                    this.checkBox_shutDown.Enabled = false;
                }
                else if (devicesAll._state == Device.State.Idle ||
                        devicesAll._state == Device.State.ShutdownPC)
                {
                    // 未开始自动控温
                    this.checkBox_start.Checked = false;
                    this.checkBox_start.Text = "开始";
                    flowStart = false;
                    SetAutoButtonEvent?.Invoke(false);

                    // 判断是否已设定完成实验测量后关机
                    this.checkBox_shutDown.Checked = false;
                    this.checkBox_shutDown.Enabled = true;
                }
            }

            // 更新表格显示
            updateDataGridView();
        }


        /// <summary>
        /// 开始 / 暂停自动控温流程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox_start_Click(object sender, EventArgs e)
        {
            if (this.checkBox_start.Checked == true)
            {
                // 如果列表中没有温度点，则不开始控温流程
                if (paramList.Count == 0)
                {
                    flowStart = false;
                    SetAutoButtonEvent?.Invoke(false);
                    this.checkBox_start.Checked = false;
                    this.checkBox_start.Text = "开始";
                    return;
                }


                // 将温度点数据保存到缓存中
                try
                {
                    // 清空原有文件
                    FileStream fs = File.Open(@"./params.cache", FileMode.OpenOrCreate, FileAccess.Write);
                    if (fs != null)
                    {
                        fs.Close();
                    }

                    StreamWriter sw = new StreamWriter(@"./params.cache", false, Encoding.UTF8);
                    for (int i = 0; i < paramList.Count; i++)
                    {
                        for (int j = 0; j < paramList[i].paramM.Length - 1; j++)
                        {
                            sw.Write(paramList[i].paramM[j].ToString() + " ");
                        }
                        sw.Write(paramList[i].paramM[paramList[i].paramM.Length - 1].ToString());
                        sw.WriteLine();
                    }
                    sw.Flush();
                    sw.Close();
                }
                catch (Exception ex)
                {
                    Utils.Logger.Sys("存储温度点缓存时，发生异常！");
                    MessageBox.Show("存储温度点缓存时，发生异常！");
                    this.checkBox_start.Checked = false;
                    return;
                }

                // 开始自动控温流程
                lock (devicesAll.stepLocker)
                {
                    // 清空原有的温度点数据
                    devicesAll.temperaturePointList.Clear();
                    // 将实验流程数据写入 Devices 类中
                    for (int i = 0; i < paramList.Count; i++)
                    {
                        // deviceAll.controlFlowList 中的 StateFlow.flowState 必须设置为 Undefine
                        // 只保存温度点，不再保存
                        Device.TemptPointStruct tp = new Device.TemptPointStruct();
                        paramList[i].paramM.CopyTo(tp.paramM, 0);
                        tp.finished = paramList[i].finished;
                        devicesAll.temperaturePointList.Add(tp);
                    }

                    if (devicesAll._state == Device.State.Idle)
                    {
                        devicesAll.StartAutoControl();
                    }
                    else
                    {
                        MessageBox.Show("Error");
                    }

                    // 实验完成后是否关闭计算机
                    devicesAll._runningParameters.shutDownComputer = this.checkBox_shutDown.Checked;
                    this.checkBox_shutDown.Enabled = false;
                }

                Utils.Logger.Op("点击自动控温设置界面 开始 按键，开始执行自动控温流程...");
                Utils.Logger.Sys("点击自动控温设置界面 开始 按键，开始执行自动控温流程...");

                Utils.Logger.Op("设定的温度点有：");
                Utils.Logger.Sys("设定的温度点有：");

                foreach (var st in paramList)
                {
                    Utils.Logger.Op(st.paramM[0].ToString("0.0000"));
                    Utils.Logger.Sys(st.paramM[0].ToString("0.0000"));
                    Utils.Logger.Op("是否已测量：" + st.finished.ToString());
                    Utils.Logger.Sys("是否已测量：" + st.finished.ToString());
                }

                // 自动控温流程已开始
                flowStart = true;
                SetAutoButtonEvent?.Invoke(true);
                checkBox_start.Text = "停止";

                // 开启定时器 - 状态更新
                this.timer1.Start();
            }
            else
            {
                // 停止定时器 - 状态更新
                this.timer1.Stop();


                // 暂停自动控温流程
                lock (this.devicesAll.stepLocker)
                {
                    // 控温流程当前工作状态 - 停止
                    //this.devicesAll.currentState.flowState = Device.Devices.State.Stop;
                    //this.devicesAll.currentState.stateChanged = true;

                    if (devicesAll.temperaturePointList.Count != 0)
                    {
                        paramList.Clear();
                        // 将 devicesAll.temperaturePointList 中未测量完成的温度点
                        foreach (var st in devicesAll.temperaturePointList)
                        {
                            TempParam ts = new TempParam();
                            st.paramM.CopyTo(ts.paramM, 0);
                            ts.finished = st.finished;
                            paramList.Add(ts);
                        }
                    }

                    devicesAll.SuspendAutoControl();

                    // 取消 - 实验完成后不关闭计算机
                    devicesAll._runningParameters.shutDownComputer = false;
                    this.checkBox_shutDown.Enabled = true;
                }

                // 更新表格显示
                updateDataGridView();

                // 自动控温流程已停止
                flowStart = false;
                SetAutoButtonEvent?.Invoke(false);
                checkBox_start.Text = "开始";
            }

            return;
        }

        // 返回主界面，关闭窗口
        private void button_cancel_Click(object sender, EventArgs e)
        {

            // 取消操作
            this.Dispose();
        }


        // 添加温度点
        private void button_add_Click(object sender, EventArgs e)
        {
            // 如果已经开始自动控温流程，则无法编辑
            if (flowStart)
                return;


            float valuef = 0.0f;
            // 主槽参数
            for (int i = 0; i < 3; i++)
            {
                // 将相应的参数加入到 paramMCache 中
                if (float.TryParse(this.paramMTextBox[i].Text, out valuef))
                {
                    paramMCache[i] = valuef;
                }
                else
                {
                    MessageBox.Show("温度点参数格式不正确，请检查!");
                    return;
                }
            }
            if (paramMCache[0] > devicesAll._runningParameters.tempMaxValue || paramMCache[0] < devicesAll._runningParameters.tempMinValue)
            {
                MessageBox.Show("温度点超出界限 ( " + devicesAll._runningParameters.tempMinValue.ToString("0.0000") + " - " + devicesAll._runningParameters.tempMaxValue.ToString("0.0000") + " )，请检查!");
                return;
            }

            // 判断温度点是否已经存在于 BList 中
            foreach (TempParam st in paramList)
            {
                if (st.paramM[0] == paramMCache[0])
                {
                    MessageBox.Show("温度点已经存在！");
                    textBox_tpSetM.Text = "";
                    return;
                }
            }



            // 添加温度点
            TempParam ts = new TempParam();
            paramMCache.CopyTo(ts.paramM, 0);
            // 添加到 
            paramList.Add(ts);

            // 更新表格显示
            updateDataGridView();

            // 删除列表中的温度设定值
            textBox_tpSetM.Text = "";

            Utils.Logger.Op("添加温度设定点: " + ts.paramM[0].ToString("0.0000"));
            Utils.Logger.Sys("添加温度设定点: " + ts.paramM[0].ToString("0.0000"));

        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            // 删除文本框中的文本
            if (tx != null)
            {
                tx.Text = "";
            }


            // 如果已经开始自动控温流程，则无法编辑
            if (flowStart)
                return;

            // 删除选中的温度点
            for (int i = BList.Count; i > 0; i--)
            {
                if (dataGridView1.Rows[i - 1].Selected == true)
                {
                    Utils.Logger.Op("删除了温度设定点: " + BList[i - 1].TemptSet);
                    Utils.Logger.Sys("删除了温度设定点: " + BList[i - 1].TemptSet);

                    BList.RemoveAt(i - 1);
                    paramList.RemoveAt(i - 1);

                    break;
                }
            }

            // 更新表格显示
            updateDataGridView();

        }


        // 选中表格行时，取消编辑文本
        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (tx != null)
            {
                tx.BackColor = System.Drawing.SystemColors.Control;
                tx = null;
            }
        }

        // 重新编辑按键
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;

            // 如果已经开始自动控温流程，则无法编辑
            if (flowStart)
                return;

            if (dataGridView1.Columns[e.ColumnIndex].Name == "edit")
            {
                // 获取该行的参数
                //paramList[e.RowIndex].paramM.CopyTo(paramMCache, 0);
                Array.Copy(paramList[e.RowIndex].paramM, paramMCache, 3);
                //paramList[e.RowIndex / 2].paramS.CopyTo(paramSCache, 0);
                // 将参数写入到文本框中
                for (int i = 0; i < 3; i++)
                {
                    paramMTextBox[i].Text = paramMCache[i].ToString("0.000");
                }
                // 从 paramList 中删除该项
                paramList.RemoveAt(e.RowIndex);
                // 重新显示
                updateDataGridView();
            }
            else if (dataGridView1.Columns[e.ColumnIndex].Name == "finish")
            {
                if (paramList.Count < e.RowIndex) return;

                paramList[e.RowIndex].finished = !paramList[e.RowIndex].finished;
                updateDataGridView();
            }
        }


        ////////////////////////////////////////////////////////////
        //// 表格单元合并 / 重新绘制
        ////////////////////////////////////////////////////////////
        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            // 合并序号列
            if (dataGridView1.Columns[e.ColumnIndex].Name == "index" && e.RowIndex >= 0)
            {
                Brush gridBrush = new SolidBrush(this.dataGridView1.GridColor);
                Pen gridLinePen = new Pen(gridBrush);
                SolidBrush backBrush = new SolidBrush(e.CellStyle.BackColor);
                SolidBrush fontBrush = new SolidBrush(e.CellStyle.ForeColor);

                e.Graphics.FillRectangle(backBrush, e.CellBounds);

                // 绘制单元格相互间隔的区分线条，dataGridView自己会处理左侧和上册边缘的线条，因此，只需绘制下边框和右边框
                // dataGridView 控件绘制单元格时，不绘制左边和上边框，共用左单元格的右边框，上一单元格的下边框

                // 不是最后一行且单元格的值不为 null
                if (e.RowIndex < this.dataGridView1.RowCount - 1 && this.dataGridView1.Rows[e.RowIndex + 1].Cells[e.ColumnIndex].Value != null)
                {
                    // 若与下一单元格值不同
                    if (e.Value.ToString() != this.dataGridView1.Rows[e.RowIndex + 1].Cells[e.ColumnIndex].Value.ToString())
                    {
                        //下边缘的线
                        e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left, e.CellBounds.Bottom - 1,
                        e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
                        //绘制值
                        if (e.Value != null)
                        {
                            e.Graphics.DrawString(e.Value.ToString(), e.CellStyle.Font,
                                Brushes.Crimson, e.CellBounds.X + 2,
                                e.CellBounds.Y + 2, StringFormat.GenericDefault);
                        }
                    }
                    //若与下一单元格值相同 
                    else
                    {
                        //背景颜色
                        //e.CellStyle.BackColor = Color.LightPink;   //仅在CellFormatting方法中可用
                        this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.LightBlue;
                        this.dataGridView1.Rows[e.RowIndex + 1].Cells[e.ColumnIndex].Style.BackColor = Color.LightBlue;
                        //只读（以免双击单元格时显示值）
                        this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = true;
                        this.dataGridView1.Rows[e.RowIndex + 1].Cells[e.ColumnIndex].ReadOnly = true;
                    }
                }
                //最后一行或单元格的值为null
                else
                {
                    //下边缘的线
                    e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left, e.CellBounds.Bottom - 1,
                        e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);

                    //绘制值
                    if (e.Value != null)
                    {
                        e.Graphics.DrawString(e.Value.ToString(), e.CellStyle.Font,
                            Brushes.Crimson, e.CellBounds.X + 2,
                            e.CellBounds.Y + 2, StringFormat.GenericDefault);
                    }
                }

                //右侧的线
                e.Graphics.DrawLine(gridLinePen, e.CellBounds.Right - 1,
                    e.CellBounds.Top, e.CellBounds.Right - 1,
                    e.CellBounds.Bottom - 1);

                //设置处理事件完成（关键点），只有设置为ture,才能显示出想要的结果。
                e.Handled = true;
            }
        }


        /// <summary>
        /// 从数据库中查询参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_chk_Click(object sender, EventArgs e)
        {
            return;

            float val = 0.0f;
            // 从数据库中查询主槽温度设定参数
            if (float.TryParse(textBox_tpSetM.Text, out val))
            {
                Utils.CTempSets tpSet = null;
                tpSet = Utils.DataBase.checkTempSetM(val, 2.0f);

                if (tpSet != null && tpSet.Count != 0)
                {
                    float[] parm = { 0, 0, 0, 0, 0, 0, 0 };
                    for (int i = 0; i < tpSet.Count; i++)
                    {
                        for (int j = 1; j < 7; j++)
                        {
                            parm[j] += tpSet[i][j];
                        }
                    }

                    for (int i = 2; i < 7; i++)
                    {
                        paramMTextBox[i].Text = (parm[i] / tpSet.Count).ToString();
                    }
                }
            }
        }



        ///////////////////////////////////////////////////////////////
        // 键盘操作

        /// <summary>
        /// 检查是否可以继续向小数点后添加数字 - 
        /// true 表示可以继续添加，false 表示不可以
        /// </summary>
        /// <param name="str">数字字符串</param>
        /// <param name="digit">小数点后的位数</param>
        /// <returns></returns>
        bool checkDotPosition(string str, int digit)
        {
            if (!str.Contains('.')) return true;

            if (str.Length - str.IndexOf('.') < 3) return true;

            return false;
        }


        private void button9_Click(object sender, EventArgs e)
        {
            if (tx != null)
            {
                if (tx.Text.Length == 1 && tx.Text == "0")
                {
                    tx.Text = "9";
                }
                else if (tx.Text.Length == 2 && tx.Text == "-0")
                {
                    tx.Text = "-9";
                }
                else
                {
                    tx.Text += "9";
                }
            }
            else
            {
                MessageBox.Show("请先选定设定项!");
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (tx != null)
            {
                if (tx.Text.Length == 1 && tx.Text == "0")
                {
                    tx.Text = "8";
                }
                else if (tx.Text.Length == 2 && tx.Text == "-0")
                {
                    tx.Text = "-8";
                }
                else
                {
                    tx.Text += "8";
                }
            }
            else
            {
                MessageBox.Show("请先选定设定项!");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (tx != null)
            {
                if (tx.Text.Length == 1 && tx.Text == "0")
                {
                    tx.Text = "7";
                }
                else if (tx.Text.Length == 2 && tx.Text == "-0")
                {
                    tx.Text = "-7";
                }
                else
                {
                    tx.Text += "7";
                }
            }
            else
            {
                MessageBox.Show("请先选定设定项!");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (tx != null)
            {
                if (tx.Text.Length == 1 && tx.Text == "0")
                {
                    tx.Text = "6";
                }
                else if (tx.Text.Length == 2 && tx.Text == "-0")
                {
                    tx.Text = "-6";
                }
                else
                {
                    tx.Text += "6";
                }
            }
            else
            {
                MessageBox.Show("请先选定设定项!");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (tx != null)
            {
                if (tx.Text.Length == 1 && tx.Text == "0")
                {
                    tx.Text = "5";
                }
                else if (tx.Text.Length == 2 && tx.Text == "-0")
                {
                    tx.Text = "-5";
                }
                else
                {
                    tx.Text += "5";
                }
            }
            else
            {
                MessageBox.Show("请先选定设定项!");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (tx != null)
            {
                if (tx.Text.Length == 1 && tx.Text == "0")
                {
                    tx.Text = "4";
                }
                else if (tx.Text.Length == 2 && tx.Text == "-0")
                {
                    tx.Text = "-4";
                }
                else
                {
                    tx.Text += "4";
                }
            }
            else
            {
                MessageBox.Show("请先选定设定项!");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (tx != null)
            {
                if (tx.Text.Length == 1 && tx.Text == "0")
                {
                    tx.Text = "3";
                }
                else if (tx.Text.Length == 2 && tx.Text == "-0")
                {
                    tx.Text = "-3";
                }
                else
                {
                    tx.Text += "3";
                }
            }
            else
            {
                MessageBox.Show("请先选定设定项!");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (tx != null)
            {
                if (tx.Text.Length == 1 && tx.Text == "0")
                {
                    tx.Text = "2";
                }
                else if (tx.Text.Length == 2 && tx.Text == "-0")
                {
                    tx.Text = "-2";
                }
                else
                {
                    tx.Text += "2";
                }
            }
            else
            {
                MessageBox.Show("请先选定设定项!");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (tx != null)
            {
                if (tx.Text.Length == 1 && tx.Text == "0")
                {
                    tx.Text = "1";
                }
                else if (tx.Text.Length == 2 && tx.Text == "-0")
                {
                    tx.Text = "-1";
                }
                else
                {
                    tx.Text += "1";
                }

            }
            else
            {
                MessageBox.Show("请先选定设定项!");
            }
        }

        private void button0_Click(object sender, EventArgs e)
        {
            if (tx != null)
            {
                if (tx.Text.Length == 2 && tx.Text == "-0")
                {

                }
                else if (tx.Text.Length != 1 || tx.Text == "-" || int.Parse(tx.Text) != 0)
                {
                    tx.Text += "0";
                }
            }
            else
            {
                MessageBox.Show("请先选定设定项!");
            }
        }

        private void buttonNegtive_Click(object sender, EventArgs e)
        {
            if (tx != null)
            {
                if (tx.Text == "")
                {
                    tx.Text = "-";
                }
                else if (tx.Text[0] == '-')
                {
                    tx.Text = tx.Text.Remove(0, 1);
                }
                else
                {
                    tx.Text = tx.Text.Insert(0, "-");
                }
            }
            else
            {
                MessageBox.Show("请先选定设定项!");
            }
        }

        private void buttonPoint_Click(object sender, EventArgs e)
        {
            if (tx != null)
            {
                if (!tx.Text.Contains("."))
                {
                    if (tx.Text.Length == 0)
                    {
                        tx.Text = "0.";
                    }
                    else if (tx.Text.Length == 1 && tx.Text == "-")
                    {
                        tx.Text = "-0.";
                    }
                    else
                    {
                        tx.Text += ".";
                    }
                }
            }
            else
            {
                MessageBox.Show("请先选定设定项!");
            }
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            if (tx != null)
            {
                if (tx.Text.Length > 0)
                    tx.Text = tx.Text.Substring(0, tx.Text.Length - 1);
            }
            //else
            //{
            //    MessageBox.Show("请先选定设定项!");
            //}
        }

        // 编辑文本时，取消表格行的选中
        private void textBox_tpSet_Enter(object sender, EventArgs e)
        {
            if (tx != null)
            {
                tx.BackColor = System.Drawing.SystemColors.Control;
            }

            tx = this.textBox_tpSetM;
            tx.BackColor = System.Drawing.SystemColors.Window;
            dataGridView1.ClearSelection();
        }

        private void textBox_advance_Enter(object sender, EventArgs e)
        {
            if (tx != null)
            {
                tx.BackColor = System.Drawing.SystemColors.Control;
            }

            tx = this.textBox_advanceM;
            tx.BackColor = System.Drawing.SystemColors.Window;
            dataGridView1.ClearSelection();
        }

        private void textBox_fuzzy_Enter(object sender, EventArgs e)
        {
            if (tx != null)
            {
                tx.BackColor = System.Drawing.SystemColors.Control;
            }

            tx = this.textBox_fuzzyM;
            tx.BackColor = System.Drawing.SystemColors.Window;
            dataGridView1.ClearSelection();
        }

        internal class ParamShow
        {
            public int _index;
            public string _edit = "编辑";
            public string _finish = "未测量";
            /// <summary>
            /// 温控设备的参数值
            /// </summary>
            public float[] param = new float[7];


            /// <summary>
            /// 编号
            /// </summary>
            public string Index { get { return _index.ToString("0"); } }
            /// <summary>
            /// 温度值
            /// </summary>
            public string TemptSet { get { return param[0].ToString("0.0000"); } }
            /// <summary>
            /// 温度修订值
            /// </summary>
            public string TempAdjust { get { return param[1].ToString("0.0000"); } }
            /// <summary>
            /// 超前调整值
            /// </summary>
            public string Advance { get { return param[2].ToString("0.000"); } }
            /// <summary>
            /// 模糊系数
            /// </summary>
            public string Fuzzy { get { return param[3].ToString("0"); } }
            /// <summary>
            /// 比例系数
            /// </summary>
            public string Ratio { get { return param[4].ToString("0"); } }
            /// <summary>
            /// 积分系数
            /// </summary>
            public string Integration { get { return param[5].ToString("0"); } }
            /// <summary>
            /// 功率系数
            /// </summary>
            public string Power { get { return param[6].ToString("0"); } }
            /// <summary>
            /// 波动度阈值
            /// </summary>
            public string FlucThr { get { return param[7].ToString("0.000"); } }
            /// <summary>
            /// 温度阈值
            /// </summary>
            public string TempThr { get { return param[8].ToString("0.000"); } }

            public string Edit { get { return _edit; } }
            public string Finish { get { return _finish; } }
        }

        /// <summary>
        /// 温度点设定状态
        /// </summary>
        internal class TempParam : IComparable
        {
            public bool finished = false;
            /// <summary>
            /// 主槽温控设备的参数值
            /// </summary>
            public float[] paramM = new float[7];

            /// <summary>
            /// 比较函数
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int CompareTo(Object obj)
            {
                if (obj == null) return 1;
                TempParam otherState = obj as TempParam;
                if (paramM[0] > otherState.paramM[0]) { return 1; }
                else
                {
                    if (paramM[0] == otherState.paramM[0]) { return 0; }
                    else { return -1; }
                }
            }
        }
    }
}