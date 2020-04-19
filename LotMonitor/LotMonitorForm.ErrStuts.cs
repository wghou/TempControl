using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using LotPort;

namespace LotMonitor
{
    public partial class LotMonitorForm
    {
        /// <summary>
        /// 为每种错误定义一种颜色 - 只读
        /// </summary>
        readonly Dictionary<ErrorCode, Color> errorColorMap = new Dictionary<ErrorCode, Color>() {
            { ErrorCode.TempNotDown, Color.FromArgb(123, 201, 111) },
            { ErrorCode.TempNotUp, Color.FromArgb(123, 201, 111) },
            { ErrorCode.TempFlucLarge, Color.FromArgb(123, 201, 111) },
            { ErrorCode.TempBasis, Color.FromArgb(123, 201, 111) },
            { ErrorCode.TempOutRange, Color.FromArgb(123, 201, 111) },
            { ErrorCode.SensorError, Color.FromArgb(123, 201, 111) },
            { ErrorCode.RelayError, Color.FromArgb(123, 201, 111) },
            { ErrorCode.TemptError, Color.FromArgb(123, 201, 111) },
            { ErrorCode.TempParamSetError, Color.FromArgb(123, 201, 111) },
            { ErrorCode.BridgeError, Color.FromArgb(123, 201, 111) },
            { ErrorCode.CodeError, Color.FromArgb(123, 201, 111) }
        };

        // 错误状态显示符的 列数 / 行数
        const int errStatusCol = 50;
        const int errStatusRow = 11;

        Dictionary<ErrorCode, List<int>> errorDict = new Dictionary<ErrorCode, List<int>>();
        Dictionary<ErrorCode, uint> currentErrCnt = new Dictionary<ErrorCode, uint>();
        object errLocker = new object();

        Timer timer_errSt = new Timer();
        // 程序起始时间
        DateTime lastUpdateTime = DateTime.Now;

        private void init_err_st()
        {
            int[] err = new int[50];

            foreach (ErrorCode itm in Enum.GetValues(typeof(ErrorCode)))
            {
                errorDict[itm] = new List<int>(err);
                currentErrCnt[itm] = 0;
            }

            timer_errSt.Interval = 2000;
            timer_errSt.Tick += Timer_errSt_Tick;
            timer_errSt.Start();
        }


        // 定时更新错误显示状态
        private void Timer_errSt_Tick(object sender, EventArgs e)
        {
            // 设每列错误状态为 2 分钟
            label_tm1.Text = DateTime.Now.ToString("hh:mm");
            label_tm2.Text = (DateTime.Now + TimeSpan.FromMinutes(25)).ToString("hh:mm");
            label_tm3.Text = (DateTime.Now + TimeSpan.FromMinutes(50)).ToString("hh:mm");
            label_tm4.Text = (DateTime.Now + TimeSpan.FromMinutes(75)).ToString("hh:mm");
            label_tm5.Text = (DateTime.Now + TimeSpan.FromMinutes(100)).ToString("hh:mm");

            double ticks = (DateTime.Now - lastUpdateTime).TotalMinutes;
            if (ticks < 2.0d) return;

            lastUpdateTime = DateTime.Now;
            lock (errLocker)
            {
                foreach (ErrorCode itm in Enum.GetValues(typeof(ErrorCode)))
                {
                    errorDict[itm].RemoveAt(0);

                    uint errCnt = currentErrCnt[itm];
                    if (errCnt != 0)
                    {
                        errorDict[itm].Add(0);
                    }
                    else {
                        errorDict[itm].Add(155);
                    }

                    // 清空错误计数
                    currentErrCnt[itm] = 0;
                }
            }
            
            hslStatusManagement_err.SetColorAll(PackColorFromValue());

            Console.WriteLine("update the error status.");
        }

        private Color[] PackColorFromValue()
        {
            // 如果 value.Item2 为0，即错误次数为 0，则将不透明度设为 0，那么就没有颜色了
            Color[] colors = new Color[errStatusCol*errStatusRow];

            for(int i = 0; i < errStatusRow; i++)
            {
                for(int j = 0;j < errStatusCol; j++)
                {
                    if (errorDict[(ErrorCode)i][j] == 0)
                    {
                        colors[i * errStatusCol + (errStatusCol - j - 1)] = Color.FromArgb(224, 224, 224);
                    }
                    else {
                        colors[i * errStatusCol + (errStatusCol - j - 1)] = errorColorMap[(ErrorCode)i];
                    }
                }
            }

            return colors;
        }
    }
}
