using System.ServiceProcess;
using System;
using System.Diagnostics;
using System.Configuration;
using WinDHCP.Library.Configuration;
using WinDHCP.Library;
using NetworkInterface = System.Net.NetworkInformation.NetworkInterface;
using System.Windows.Forms;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace WinDHCP
{
    public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        static void Main(String[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            BasicInterface bi = new BasicInterface();
            Application.Run(bi);
            if(args.Length == 2 && args[1] == "--autostart")
            {
                bi.WindowState = FormWindowState.Minimized;
            }
        }
    }
}