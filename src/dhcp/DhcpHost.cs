using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.ServiceProcess;
using System.Text;
using WinDHCP.Library;

namespace WinDHCP
{
    public partial class DhcpHost : ServiceBase
    {
        DhcpServer m_Server;

        public DhcpHost(DhcpServer server)
        {
            InitializeComponent();

            this.m_Server = server;
        }

        public IPAddress raspberry_address
        {
            get { return this.m_Server.raspberry_address; }
        }

        public string raspberry_mac
        {
            get { return BitConverter.ToString(this.m_Server.raspberry_mac); }
        }


        public void ManualStart(String[] args)
        {
            this.OnStart(args);
        }

        public void ManualStop()
        {
            this.OnStop();
        }

        protected override void OnStart(String[] args)
        {
            this.m_Server.Start();
        }

        protected override void OnStop()
        {
            this.m_Server.Stop();
        }
    }
}
