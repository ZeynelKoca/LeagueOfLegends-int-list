using System;
using System.Threading;
using System.Windows.Forms;

namespace LoL_int_list
{
    static class Program
    {   
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            const string appName = "Sisko's LoL int list";

            new Mutex(true, appName, out bool createdNew);

            if (!createdNew)
                return;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
