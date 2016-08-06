using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinDHCP.Library.Configuration;
using WinDHCP.Library;
using System.Configuration;
using System.Diagnostics;
using Microsoft.Win32;
using FLUX_DHCP;
using System.Net;

namespace WinDHCP
{
    public partial class BasicInterface : Form
    {
        public delegate void AppendLogDelegate(string line);
        public delegate void HandleDeviceDelegate(string ip, bool find_in_local);
        DebugTracer tracer;
        DhcpHost dhcp_host;
        NotifyIcon nf = new NotifyIcon();

        FluxDiscover flux_discover;
        List<DiscoverResult> dataSet;
        BindingSource dataResults;

        private class ListItem
        {
            public NetworkInterface iface;
            public ListItem(NetworkInterface iface)
            {
                this.iface = iface;
            }
            public override string ToString()
            {
                // Generates the text shown in the combo box
                return iface.Name;
            }
        }

        public BasicInterface()
        {
            InitializeComponent();
            //Trace
            tracer = new DebugTracer(this);
            Trace.Listeners.Add(tracer);

            //Notification
            nf.Visible = true;
            nf.Icon = this.Icon;
            nf.Text = "FLUX Delta Connection Tool";
            nf.Click += new System.EventHandler(notify_icon_click);

            //Discovering
            dataSet = new List<DiscoverResult>();

            var results = new System.Collections.Generic.List<DiscoverResult>();
            var b = new System.ComponentModel.BindingList<DiscoverResult>(results);
            dataResults = new System.Windows.Forms.BindingSource(b, null);
            dataGridView1.DataSource = dataResults;

            flux_discover = new FluxDiscover(dataSet, dataResults, this);
            flux_discover.Start();

            //Startup settings
            cb_run_on_startup.Checked = FLUX_DHCP.Properties.Settings.Default.run_on_startup;

            ListNetworkInterfaces();
        }

        public void BindDHCPServer(NetworkInterface iface)
        {
            if (dhcp_host != null)
            {
                tracer.WriteLine("[Info] DHCP Server Stopped.");
                dhcp_host.ManualStop();
            }
            DhcpServerConfigurationSection dhcpConfig = ConfigurationManager.GetSection("dhcpServer") as DhcpServerConfigurationSection;
            DhcpServer server = new DhcpServer();

            server.DhcpInterface = iface;
            server.StartAddress = InternetAddress.Parse(dhcpConfig.StartAddress.Trim());
            server.EndAddress = InternetAddress.Parse(dhcpConfig.EndAddress.Trim());
            server.Subnet = InternetAddress.Parse(dhcpConfig.Subnet.Trim());
            server.Gateway = InternetAddress.Parse(dhcpConfig.Gateway.Trim());
            server.LeaseDuration = dhcpConfig.LeaseDuration;
            server.OfferTimeout = dhcpConfig.OfferTimeout;
            server.DnsSuffix = dhcpConfig.DnsSuffix;

            dhcp_host = new DhcpHost(server);
            try
            {
                dhcp_host.ManualStart(new string[] { });
            }
            catch
            {
                tracer.WriteLine("[Error] Unable to bind dhcp server");
                cb_interface.SelectedIndex = -1;
            }
        }

        public void ListNetworkInterfaces()
        {
            ListItem old_index = (cb_interface.SelectedItem as ListItem);
            cb_interface.Items.Clear();
            foreach (NetworkInterface iface in NetworkInterface.GetAllNetworkInterfaces())
            {
                cb_interface.Items.Add(new ListItem(iface));
                if (iface.Description.StartsWith("Realtek USB FE Family Controller") || iface.NetworkInterfaceType == NetworkInterfaceType.GigabitEthernet || iface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    cb_interface.SelectedIndex = cb_interface.Items.Count - 1;
                }
            }

            if (old_index != null)
            {
                foreach (ListItem iface in cb_interface.Items)
                {
                    if (iface.ToString() == old_index.ToString())
                    {
                        cb_interface.SelectedItem = iface;
                    }
                }
            }

            if (cb_interface.SelectedIndex == -1)
            {
                nf.ShowBalloonTip(5000, "FLUX Delta Connection Tool", "Please attach USB Ethernet interface", ToolTipIcon.Warning);
            }
        }

        public void AppendLog(string line)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new AppendLogDelegate(AppendLog), new object[] { line });
            }
            else
            {
                try
                {
                    this.loggerText.AppendText(line + "\n");
                }catch
                {
                    this.BeginInvoke(new AppendLogDelegate(AppendLog), new object[] { line });
                }
            }
        }

        public void HandleDeviceIP(string ip, bool find_in_local = true)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new HandleDeviceDelegate(HandleDeviceIP), new object[] { ip, find_in_local });
            }
            else
            {
                AppendLog("[Info] FLUX Delta discovered " + ip);
                if (lb_raspberry_address.Text == "-")
                {
                    nf.ShowBalloonTip(10000, "FLUX Delta Discovered", ip, ToolTipIcon.Info);
                    lb_raspberry_address.Text = ip;
                }
                lb_raspberry_address.Text = ip;
            }
        }

        private string binding_interface = "";
        private void cb_interface_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListItem target_interface = cb_interface.SelectedItem as ListItem;
            if(target_interface == null)
            {
                return;
            }
            if (binding_interface == target_interface.ToString())
            {
                return;
            }

            tracer.WriteLine("[Info] Proper network selected " + target_interface.ToString());
            BindDHCPServer(target_interface.iface);
            binding_interface = target_interface.ToString();
        }

        private void BasicInterface_Load(object sender, EventArgs e)
        {
        }

        private void timer_check_ipaddr_Tick(object sender, EventArgs e)
        {
            if (dhcp_host == null) return;

            flux_discover.Ping(IPAddress.Parse(FLUX_DHCP.Properties.Settings.Default.machine_addr));

            if (dhcp_host.raspberry_address != null)
            {
                if (lb_raspberry_address.Text == "-")
                {
                    nf.ShowBalloonTip(10000, "FLUX Delta Discovered", dhcp_host.raspberry_address.ToString(), ToolTipIcon.Info);
                }

                FLUX_DHCP.Properties.Settings.Default.machine_addr = dhcp_host.raspberry_address.ToString();
                FLUX_DHCP.Properties.Settings.Default.Save();
                lb_mac.Text = dhcp_host.raspberry_mac.ToString();
                lb_raspberry_address.Text = dhcp_host.raspberry_address.ToString();
            }
        }

        private void notify_icon_click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Focus();
            this.BringToFront();
        }

        private void timer_check_interface_Tick(object sender, EventArgs e)
        {
            ListNetworkInterfaces();
        }

        private void cb_run_on_startup_CheckedChanged(object sender, EventArgs e)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
               ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (cb_run_on_startup.Checked)
                rk.SetValue("FLUXDeltaConnectionTool", Application.ExecutablePath.ToString() + " --autostart");
            else
                rk.DeleteValue("FLUXDeltaConnectionTool", false);

            FLUX_DHCP.Properties.Settings.Default.run_on_startup = cb_run_on_startup.Checked;
            FLUX_DHCP.Properties.Settings.Default.Save();
        }

        private void BasicInterface_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                nf.ShowBalloonTip(5000, "FLUX Delta Connection Tool is minimized", "You can call the interface in notification bar.", ToolTipIcon.Info);
            }
        }
    }
}
