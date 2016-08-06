using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinDHCP
{
    struct UdpStatus
    {
        public byte[] buffer;
        public EndPoint ipaddr;
    }

    class FluxDiscover
    {

        UdpClient udpc;
        IPAddress localhost = IPAddress.Parse("127.0.0.1");
        Socket s1, s2;
        List<DiscoverResult> dataSet;
        BindingSource dataResults;
        Form form;

        public FluxDiscover(List<DiscoverResult> dataSet, BindingSource dataResults, Form form)
        {
            this.dataSet = dataSet;
            this.dataResults = dataResults;
            this.form = form;
        }

        public void Start()
        {
            udpc = new UdpClient();
            Task.Run(async () =>
            {
                while (true)
                {
                    //IPEndPoint object will allow us to read datagrams sent from any source.
                    UdpReceiveResult receivedResults = await udpc.ReceiveAsync();
                    udpc.Send(receivedResults.Buffer, receivedResults.Buffer.Length, new IPEndPoint(IPAddress.Parse("192.168.1.103"), 1901));
                }
            });
            InitS1Socket();
            InitS2Socket();
        }


        private void LogLV1(Guid gid, System.Net.EndPoint source, string src_type)
        {
            var r = dataSet.Where(rr => rr.UUID == gid).FirstOrDefault();
            if (r != null)
            {
                r.RenewS1();
                r.EndPoint = source;
            }
            else
            {
                r = (DiscoverResult)dataResults.AddNew();
                r.Src = src_type;
                r.UUID = gid;
                r.EndPoint = source;
                dataSet.Add(r);
            }
            if (r.ST2 == null || r.ST2 > 9)
            {
                var header = Encoding.ASCII.GetBytes("FLUX").Concat(new byte[] { 1, 2 }).Concat(gid.ToByteArray()).ToArray();
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

        private void IncomingS1Message(IAsyncResult result)
        {
            if (s1 == null)
            {
                return;
            }
            int l;
            EndPoint ipaddr = new IPEndPoint(IPAddress.Any, 1901);
            try
            {
                l = s1.EndReceiveFrom(result, ref ipaddr);
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            var st = (UdpStatus)result.AsyncState;
            int action_id = st.buffer[5];
            var gid = new Guid(st.buffer.Skip(6).Take(16).ToArray());
            
            //Proxy mode
            if (ipaddr.ToString() == "192.168.1.103:1901")
            {
               // udpc.SendAsync(st.buffer, l, new IPEndPoint(localhost, 1901));
            }

            (form as BasicInterface).HandleDeviceIP((ipaddr as IPEndPoint).Address.ToString(), false);

            if (BitConverter.ToString(st.buffer, 0, 4) == "46-4C-55-58" && action_id == 0)
            {
                try
                {
                    form.Invoke(new Action<Guid, EndPoint, String>(LogLV1), new object[] { gid, ipaddr, "Auto" });
                }
                catch
                {

                }
            }

            s1.BeginReceiveFrom(st.buffer, 0, 4096, 0, ref st.ipaddr, new AsyncCallback(IncomingS1Message), st);
        }

        private void IncomingS2Message(IAsyncResult result)
        {
            EndPoint ipaddr = new IPEndPoint(IPAddress.Any, 0);
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

            (form as BasicInterface).HandleDeviceIP((ipaddr as IPEndPoint).Address.ToString(), false);

            if (BitConverter.ToString(st.buffer, 0, 4) == "46-4C-55-58" && action_id == 3)
            {
                var gid = new Guid(st.buffer.Skip(6).Take(16).ToArray());
                var offset = 30;
                offset += BitConverter.ToUInt16(st.buffer, 26);
                offset += BitConverter.ToUInt16(st.buffer, 28);
                var rmsg = Encoding.UTF8.GetString(st.buffer, offset, l - offset);
                string name = "?"; string ver = "?";

                foreach (string raw in rmsg.Split('\x00'))
                {
                    if (raw.StartsWith("name=")) name = raw.Substring(5);
                    else if (raw.StartsWith("ver=")) ver = raw.Substring(4);
                    try
                    {
                        form.Invoke(new Action<Guid, string, string>(LogLV2), new object[] { gid, name, ver });
                    }
                    catch
                    {

                    }
                }
            }
            else if (BitConverter.ToString(st.buffer, 0, 4) == "46-4C-55-58" && action_id == 0)
            {
                var gid = new Guid(st.buffer.Skip(6).Take(16).ToArray());
                //this._deviceGid = gid;
                form.Invoke(new Action<Guid, System.Net.EndPoint, string>(LogLV1), new object[] { gid, ipaddr, "Manual" });
            }
            else
            {
            }

            try
            {
                s2.BeginReceiveFrom(st.buffer, 0, 4096, 0, ref st.ipaddr, new AsyncCallback(IncomingS2Message), st);
            }
            catch
            {

            }
        }

        private void InitS2Socket()
        {
            (this.form as BasicInterface).AppendLog("[Info] Starting FLUX Discovering service (S2)");
            s2 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            s2.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            UdpStatus st = new UdpStatus();
            st.buffer = new byte[4096];
            st.ipaddr = new System.Net.IPEndPoint(System.Net.IPAddress.Any, 0);

            s2.Bind(st.ipaddr);
            s2.BeginReceiveFrom(st.buffer, 0, 4096, 0, ref st.ipaddr, new AsyncCallback(IncomingS2Message), st);
        }

        private void InitS1Socket()
        {
            (this.form as BasicInterface).AppendLog("[Info] Starting FLUX Discovering service (S1)");
            s1 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            s1.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            var mreq = System.Net.IPAddress.Parse("239.255.255.250").GetAddressBytes().Concat(System.Net.IPAddress.Any.GetAddressBytes()).ToArray();
            s1.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, mreq);
            s1.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, true);
            s1.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.Any, 1901));

            UdpStatus st = new UdpStatus();
            st.buffer = new byte[4096];
            st.ipaddr = new System.Net.IPEndPoint(System.Net.IPAddress.Any, 1901);
            s1.BeginReceiveFrom(st.buffer, 0, 4096, 0, ref st.ipaddr, new AsyncCallback(IncomingS1Message), st);
        }

        public void Ping(IPAddress ipaddr)
        {
            var header = Encoding.ASCII.GetBytes("FLUX").Concat(new byte[] { 1, 0 }).Concat(Guid.Empty.ToByteArray()).ToArray();
            s2.SendTo(header, new IPEndPoint(ipaddr, 1901));
            (this.form as BasicInterface).AppendLog("[DISCOVER] Ping " + ipaddr.ToString());
        }
    }
}
