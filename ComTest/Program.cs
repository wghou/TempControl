using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComTest
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 注册控件示例，如果注册失败，你的控件仍然只能使用8个小时
            bool isSuccess = HslControls.Authorization.SetAuthorizationCode("e595afbf-474d-43a5-b608-385a5c77680f");
            if (!isSuccess)
            {
                Console.WriteLine("注册失败");
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
