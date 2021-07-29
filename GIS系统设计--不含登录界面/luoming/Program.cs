using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _luoming
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            LoadResoureDll.RegistDLL();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.EngineOrDesktop);
            Application.Run(new Form1());

            //Login login = new Login();

            ////界面转换
            //login.ShowDialog();

            //if (login.DialogResult == DialogResult.OK)
            //{
            //    login.Dispose();
            //    Application.Run(new Form1());
            //}
            //else if (login.DialogResult == DialogResult.Cancel)
            //{
            //    login.Dispose();
            //    return;
            //}
        }
    }
}
