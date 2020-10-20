using System;
using System.Threading;
using System.Windows.Forms;

namespace Siskos_LOL_int_list
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            const string appName = "Sisko's LoL int list";

            var mutex = new Mutex(true, appName, out var createdNew);

            if (!createdNew)
                return;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            GC.KeepAlive(mutex);
        }
    }
}
