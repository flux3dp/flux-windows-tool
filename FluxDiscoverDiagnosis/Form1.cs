using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;
using System.IO;
using System.Diagnostics;

namespace FluxDiscoverDiagnosis
{
    struct UdpStatus
    {
        public byte[] buffer;
        public System.Net.EndPoint ipaddr;
    }

    public partial class Form1 : Form
    {
        List<DiscoverResult> dataSet;

        System.Windows.Forms.BindingSource dataResults;
        System.Timers.Timer timer;
        Socket s1;
        Socket s2;
        ScanForm sf;
        bool usingWiFi = false;
        string myIP;
        System.Threading.Thread scanning_thread;
        string ipstart;
        string ipend;
        int scan_total = 1;
        int scan_progress = 0;
        private string _deviceIP = "";
        Process flux_api;
        UdpClient udpc;
        
        IPAddress localhost = IPAddress.Parse("127.0.0.1");
        private Guid _deviceGid;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataSet = new List<DiscoverResult>();

            var results = new System. Collections.Generic.List<DiscoverResult>();
            var b = new System.ComponentModel.BindingList<DiscoverResult>(results);
            dataResults = new System.Windows.Forms.BindingSource(b, null);
            dataGridView1.DataSource = dataResults;

            timer = new System.Timers.Timer(500);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;

            InitS2Socket();
            InitS1Socket();
            checkDHCP();
            sf = new ScanForm(this);
            //Use .Net Framwork multicast API
            udpc = new UdpClient(2003);
            Task.Run(async () =>
            {
                while (true)
                {
                    //IPEndPoint object will allow us to read datagrams sent from any source.
                    UdpReceiveResult receivedResults = await udpc.ReceiveAsync();
                    udpc.Send(receivedResults.Buffer, receivedResults.Buffer.Length, new IPEndPoint(IPAddress.Parse("192.168.1.103"), 1901));
                }
            });
            renderProgress();

            if(File.Exists("C:\\Program Files (x86)\\FLUX\\lib\\flux_api\\flux_api.exe"))
            {
                ProcessStartInfo flux_api_info = new ProcessStartInfo("C:\\Program Files (x86)\\FLUX\\lib\\flux_api\\flux_api.exe", "--ip=0.0.0.0 --port=18337");
                flux_api_info.CreateNoWindow = true;
                flux_api_info.ErrorDialog = true;
                flux_api_info.UseShellExecute = false;
                flux_api = Process.Start(flux_api_info);
            }
            else if (File.Exists("C:\\Program Files\\FLUX\\lib\\flux_api\\flux_api.exe"))
            {
                ProcessStartInfo flux_api_info = new ProcessStartInfo("C:\\Program Files\\FLUX\\lib\\flux_api\\flux_api.exe", "--ip=0.0.0.0 --port=18337");
                flux_api_info.CreateNoWindow = true;
                flux_api_info.ErrorDialog = true;
                flux_api_info.UseShellExecute = false;
                flux_api = Process.Start(flux_api_info);
            }
            else
            {
                MessageBox.Show("You will not be able to use USB API", "Unable to detect FLUX Studio Installation");
            }
        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            System.Net.IPAddress ipaddr;
            if (IPAddress.TryParse(txt_ip.Text, out ipaddr))
            {
                fluxPing(ipaddr);
            }
            try
            {
                this.Invoke(new Action(() => { dataGridView1.Refresh(); }));
            }
            catch 
            {

            }
        }
        
        private void LogLV1(Guid gid, System.Net.EndPoint source, string src_type) 
        {
            var r = dataSet.Where(rr => rr.UUID == gid).FirstOrDefault();
            if(r != null)
            {
                r.RenewS1();
                r.EndPoint = source;
            } else
            {
                r = (DiscoverResult)dataResults.AddNew();
                r.Src = src_type;
                r.UUID = gid;
                r.EndPoint = source;
                dataSet.Add(r);
            }
            if (r.ST2 == null || r.ST2 > 9)
            {
                var header = Encoding.ASCII.GetBytes("FLUX").Concat(new byte[] {1, 2}).Concat(gid.ToByteArray()).ToArray();
                s2.SendTo(header, source);
            }
        }

        private void LogLV2(Guid gid, string name, string ver)
        {
            var r = dataSet.Where(rr => rr.UUID == gid).FirstOrDefault();
            if (r != null)
            {
                r.RenewS2(name, ver);
            }
        }

        private void IncommingS1Message(IAsyncResult result)
        {
            if(s1 == null)
            {
                return;
            }
            int l;
            System.Net.EndPoint ipaddr = new System.Net.IPEndPoint(System.Net.IPAddress.Any, 1901);
            try
            {
                l = s1.EndReceiveFrom(result, ref ipaddr);
            }
            catch (System.ObjectDisposedException)
            {
                return;
            }
            var st = (UdpStatus)result.AsyncState;
            int action_id = st.buffer[5];
            var gid = new Guid(st.buffer.Skip(6).Take(16).ToArray());

            //Proxy mode
            if (ipaddr.ToString() == "192.168.1.103:1901")
            {
                udpc.SendAsync(st.buffer, l, new IPEndPoint(localhost, 1901));
            }

            if (BitConverter.ToString(st.buffer, 0, 4) == "46-4C-55-58" && action_id == 0)
            {
                try {
                    this.Invoke(new Action<Guid, System.Net.EndPoint, String>(LogLV1), new object[] { gid, ipaddr, "Auto" });
                }
                catch
                {

                }
            }

            s1.BeginReceiveFrom(st.buffer, 0, 4096, 0, ref st.ipaddr, new AsyncCallback(IncommingS1Message), st);
        }

        private void IncommingS2Message(IAsyncResult result)
        {
            System.Net.EndPoint ipaddr = new System.Net.IPEndPoint(System.Net.IPAddress.Any, 0);
            int l;
            try
            {
                l = s2.EndReceiveFrom(result, ref ipaddr);
            }
            catch
            {
                return;
            }
            UdpStatus st = (UdpStatus)result.AsyncState;
            int action_id = st.buffer[5];
            //Proxy mode
            if (ipaddr.ToString() == "192.168.1.103:1901")
            {
                udpc.SendAsync(st.buffer, l, new IPEndPoint(localhost, 1901));
            }

            if (BitConverter.ToString(st.buffer, 0, 4) == "46-4C-55-58" && action_id == 3)
            {
                var gid = new Guid(st.buffer.Skip(6).Take(16).ToArray());
                var offset = 30;
                offset += BitConverter.ToUInt16(st.buffer, 26);
                offset += BitConverter.ToUInt16(st.buffer, 28);
                var rmsg = Encoding.UTF8.GetString(st.buffer, offset, l - offset);
                string name = "?"; string ver = "?";

                foreach(string raw in rmsg.Split('\x00'))
                {
                    if (raw.StartsWith("name=")) name = raw.Substring(5);
                    else if (raw.StartsWith("ver=")) ver = raw.Substring(4);
                    try {
                        this.Invoke(new Action<Guid, string, string>(LogLV2), new object[] { gid, name, ver });
                    }
                    catch
                    {

                    }
                }
            }
            else if(BitConverter.ToString(st.buffer, 0, 4) == "46-4C-55-58" && action_id == 0)
            {
                var gid = new Guid(st.buffer.Skip(6).Take(16).ToArray());
                this._deviceGid = gid;
                this.Invoke(new Action<Guid, System.Net.EndPoint, string>(LogLV1), new object[] { gid, ipaddr, "Manual" });
            }
            else
            {
            }

            try {
                s2.BeginReceiveFrom(st.buffer, 0, 4096, 0, ref st.ipaddr, new AsyncCallback(IncommingS2Message), st);
            }
            catch
            {

            }
        }

        private void InitS2Socket()
        {
            s2 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            s2.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            UdpStatus st = new UdpStatus();
            st.buffer = new byte[4096];
            st.ipaddr = new System.Net.IPEndPoint(System.Net.IPAddress.Any, 0);

            s2.Bind(st.ipaddr);
            s2.BeginReceiveFrom(st.buffer, 0, 4096, 0, ref st.ipaddr, new AsyncCallback(IncommingS2Message), st);
        }

        private void InitS1Socket()
        {
            s1 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            s1.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            var mreq = System.Net.IPAddress.Parse("239.255.255.250").GetAddressBytes().Concat(System.Net.IPAddress.Any.GetAddressBytes()).ToArray();
            s1.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, mreq);
            s1.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, true);
            s1.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.Any, 1901));

            UdpStatus st = new UdpStatus();
            st.buffer = new byte[4096];
            st.ipaddr = new System.Net.IPEndPoint(System.Net.IPAddress.Any, 1901);
            s1.BeginReceiveFrom(st.buffer, 0, 4096, 0, ref st.ipaddr, new AsyncCallback(IncommingS1Message), st);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(s1 == null)
            {
                InitS1Socket();
                btn_scan.Text = "Stop";
            } else
            {
                s1.Close();
                s1 = null;
                btn_scan.Text = "Start";
            }
        }

        private void btn_ping_click(object sender, EventArgs e)
        {
            System.Net.IPAddress ipaddr;
            if(!IPAddress.TryParse(txt_ip.Text, out ipaddr))
            {
                MessageBox.Show("Bad ipaddr");
                return;
            }
            fluxPing(ipaddr);
        }

        private void refresh_socket()
        {
            if (s1 != null) {
                s1.Close();
                s1 = null;
                InitS1Socket();
            }
        }

        private void fluxPing(System.Net.IPAddress ipaddr)
        {
            var header = Encoding.ASCII.GetBytes("FLUX").Concat(new byte[] { 1, 0 }).Concat(Guid.Empty.ToByteArray()).ToArray();
            s2.SendTo(header, new System.Net.IPEndPoint(ipaddr, 1901));
            if (_deviceGid != null)
            {
                header = Encoding.ASCII.GetBytes("FLUX").Concat(new byte[] { 1, 2 }).Concat(_deviceGid.ToByteArray()).ToArray();
                s2.SendTo(header, new System.Net.IPEndPoint(ipaddr, 1901));
            }
        }
        
        private void btn_scan_click(object sender, EventArgs e)
        {
            if (scanning_thread != null)
            {
                scanning_thread.Abort();
                scanning_thread = null;
                scan_progress = 0;
                pgbar.Value = 0;
                pgbar.Enabled = false;
                txt_status.Text = "Stopped scanning";
            }
            sf.Show(this);
        }

        public void scan(string ipstart, string ipend)
        {
            IPAddress ip_start;
            IPAddress ip_end;
            if (!IPAddress.TryParse(ipstart, out ip_start))
            {
                MessageBox.Show("Bad ipaddr - start");
                return;
            }

            if (!IPAddress.TryParse(ipend, out ip_end))
            {
                MessageBox.Show("Bad ipaddr - end");
                return;
            }

            if (ip2int(ipstart) > ip2int(ipend))
            {
                MessageBox.Show("Starting ip must be less than ending ip");
                return;
            }
            this.ipstart = ipstart;
            this.ipend = ipend;
            this.scan_total = diff(ipstart, ipend);

            if(this.scan_total > 6400)
            {
                MessageBox.Show("Range too wide");
                return;
            }

            pgbar.Enabled = true;
            scanning_thread = new System.Threading.Thread(runScanning);
            scanning_thread.Start();
            
        }

        public delegate void InvokeDelegate();

        public void runScanning()
        {
            while (ip2int(ipstart) < ip2int(ipend))
            {
                fluxPing(IPAddress.Parse(ipstart));
                ipstart = ipStep(ipstart);
                scan_progress++;
                System.Threading.Thread.Sleep(100);
                this.BeginInvoke(new InvokeDelegate(renderProgress));
            }
        }

        public int diff(string ip1, string ip2)
        {
            int count = 0;
            while (ip2int(ip1) < ip2int(ip2) && count < 6400)
            {
                count++;
                ip1 = ipStep(ip1);
            }
            return count;
        }
        

        public UInt64 ip2int(string ip)
        {
            UInt64 ipn = 0;
            foreach (string token in ip.Split('.'))
            {
                ipn = ipn + UInt64.Parse(token);
                ipn = ipn * 255;
            }
            return ipn;
        }

        public string ipStep(string ip)
        {
            int[] iptokens = new int[4];
            int i = 0;
            foreach (string token in ip.Split('.'))
            {
                iptokens[i++] = int.Parse(token);
            }
            iptokens[3]++;
            if (iptokens[3] > 255)
            {
                iptokens[3] = 0;
                iptokens[2]++;
            }
            if (iptokens[2] > 255)
            {
                iptokens[2] = 0;
                iptokens[1]++;
            }
            return String.Join(".", iptokens);
        }

        private void checkDHCP() {
            var card = getDefaultInterface();
            if (card.Supports(NetworkInterfaceComponent.IPv4) == false)
            {
                MessageBox.Show(card.Name + " does not support IPv4");
            }

            if (!card.SupportsMulticast)
            {
                MessageBox.Show(card.Name + " does not support Multicast");
            }

            if(card.OperationalStatus == OperationalStatus.Unknown || card.OperationalStatus == OperationalStatus.Down)
            {
                MessageBox.Show(card.Name + " is not ready");
            }
            IPv4InterfaceProperties p = card.GetIPProperties().GetIPv4Properties();

            if (!p.IsDhcpEnabled)
            {
                MessageBox.Show("Warning:"+ card.Name +" DHCP is not disabled, make sure you're not using PPPoE");
            }

            foreach (UnicastIPAddressInformation ip in card.GetIPProperties().UnicastAddresses)
            {
                if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    myIP = ip.Address.ToString();
                }
            }
            renderNetwork();
        }

        public void renderProgress()
        {
            if (scan_progress > 0)
            {
                txt_status.Text = "Pinging " + ipstart + " / " + ipend;
                pgbar.Value = (scan_progress * 100 / scan_total);
                if (!pgbar.Visible) pgbar.Visible = true;
            }
            else
            {
                pgbar.Visible = false;
            }

        }

        public void renderNetwork()
        {
            string str = "";
            if (usingWiFi)
            {
                str = "WiFi ";
            }else
            {
                str = "Wired Network ";
            }
            str += myIP + "  | ";
            txt_network.Text = str;
            if (_deviceIP != "")
            {
                txt_ip.Text = _deviceIP;
                _deviceIP = "";
            }
        }

        public NetworkInterface getDefaultInterface()
        {
            //Looking for wi-fi
            foreach(NetworkInterface card in NetworkInterface.GetAllNetworkInterfaces()){
                if (card.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    usingWiFi = true;
                    return card;
                }
            }
            //No WiFi? Ethernet?
            var cardDefault = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault();
            if (cardDefault == null) return null;
            return cardDefault;
        }

        private void btn_reconnect_Click(object sender, EventArgs e)
        {
            string strCmdText = "/C ipconfig /release & ipconfig /renew";
            System.Diagnostics.Process.Start("CMD.exe", strCmdText);
        }

        private void unleashIP(string ip)
        {
            if(MessageBox.Show("Add "+ip+" to your firewall whitelist?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }

            string bat = "set IP=" + ip + "\r\n" +
@"set RULE_NAME=""FLUX Delta %IP%""

icacls ""%cd%\lib"" /grant Everyone:(OI)(CI)f

netsh advfirewall firewall show rule name=%RULE_NAME% >nul

if not ERRORLEVEL 1 (
    rem Rule %RULE_NAME% already exists.
    echo Hey, you already got a out rule by that name, you cannot put another one in!
    pause
) else (
    echo Rule %RULE_NAME% does not exist. Creating...
    netsh advfirewall firewall add rule name=%RULE_NAME% dir=in action=allow remoteip=" + ip + @",LocalSubnet enable=yes
    pause
    )";

            string path = Path.GetTempPath() + "firewall.bat";
            File.Delete(path);
            File.WriteAllText(path, bat);
            System.Diagnostics.Process.Start(path);
            //File.Delete(path);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.scanning_thread != null)
            {
                this.scanning_thread.Abort();
            }
            try {
                flux_api.Kill();
            }
            catch
            {

            }
            Application.ExitThread();
            Application.Exit();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string ip = ((dataGridView1.Rows[e.RowIndex].DataBoundItem as DiscoverResult).IPAddr);
            ip = ip.Substring(0, ip.IndexOf(':'));
            unleashIP(ip);
        }

        private void btn_getip_Click(object sender, EventArgs e)
        {
            UsbConfig ws = new UsbConfig("ws://127.0.0.1:18337/ws/usb-config");
            ws.parent = this; 
            ws.Connect();
        }

        public void HandleDeviceIP(string ip)
        {
            this._deviceIP = ip;
            this.BeginInvoke(new InvokeDelegate(renderNetwork));
            fluxPing(IPAddress.Parse(ip));
        }

    }
}
