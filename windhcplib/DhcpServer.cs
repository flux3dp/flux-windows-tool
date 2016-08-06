using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;
using System.Diagnostics;

namespace WinDHCP.Library
{
    public class DhcpServer
    {
        private const Int32 DhcpPort = 67;
        private const Int32 DhcpClientPort = 68;
        private const Int32 DhcpMessageMaxSize = 1024;

        private TimeSpan m_OfferTimeout = TimeSpan.FromSeconds(30);
        private TimeSpan m_LeaseDuration = TimeSpan.FromDays(1);

        private NetworkInterface m_DhcpInterface;
        private IPAddress m_DhcpInterfaceAddress;

        private InternetAddress m_StartAddress = new InternetAddress(192, 168, 1, 100);
        private InternetAddress m_EndAddress = new InternetAddress(192, 168, 1, 150);
        private InternetAddress m_Subnet = new InternetAddress(255, 255, 255, 255);
        private InternetAddress m_Gateway = new InternetAddress(192, 168, 1, 1);
        private String m_DnsSuffix;
        private List<InternetAddress> m_DnsServers = new List<InternetAddress>();
        private SortedList<PhysicalAddress, Boolean> m_Acl = new SortedList<PhysicalAddress, Boolean>();
        private Boolean m_AllowAny = true;
        private Dictionary<PhysicalAddress, InternetAddress> m_Reservations = new Dictionary<PhysicalAddress, InternetAddress>();

        private Object m_LeaseSync = new Object();
        private ReaderWriterLock m_AclLock = new ReaderWriterLock();
        private ReaderWriterLock m_AbortLock = new ReaderWriterLock();
        private Socket m_DhcpSocket;
        private Boolean m_Abort = false;

        private Timer m_CleanupTimer;

        private Dictionary<InternetAddress, AddressLease> m_ActiveLeases = new Dictionary<InternetAddress,AddressLease>();
        private SortedList<InternetAddress, AddressLease> m_InactiveLeases = new SortedList<InternetAddress, AddressLease>();
        public IPAddress raspberry_address;
        public byte[] raspberry_mac;

        public TimeSpan OfferTimeout
        {
            get { return this.m_OfferTimeout; }
            set { this.m_OfferTimeout = value; }
        }

        public TimeSpan LeaseDuration
        {
            get { return this.m_LeaseDuration; }
            set { this.m_LeaseDuration = value; }
        }

        public NetworkInterface DhcpInterface
        {
            get { return this.m_DhcpInterface; }
            set { this.m_DhcpInterface = value; }
        }

        public InternetAddress StartAddress
        {
            get { return this.m_StartAddress; }
            set { this.m_StartAddress = value; }
        }

        public InternetAddress EndAddress
        {
            get { return this.m_EndAddress; }
            set { this.m_EndAddress = value; }
        }

        public InternetAddress Subnet
        {
            get { return this.m_Subnet; }
            set { this.m_Subnet = value; }
        }

        public InternetAddress Gateway
        {
            get { return this.m_Gateway; }
            set { this.m_Gateway = value; }
        }

        public String DnsSuffix
        {
            get { return this.m_DnsSuffix; }
            set { this.m_DnsSuffix = value; }
        }

        public List<InternetAddress> DnsServers
        {
            get { return this.m_DnsServers; }
        }

        public Boolean AllowAny
        {
            get { return this.m_AllowAny; }
            set { this.m_AllowAny = value; }
        }

        public Dictionary<PhysicalAddress, InternetAddress> Reservations
        {
            get { return this.m_Reservations; }
        }

        public DhcpServer()
        {
        }

        public void AddAcl(PhysicalAddress address, Boolean deny)
        {
            this.m_AclLock.AcquireWriterLock(-1);

            try
            {
                if (this.m_Acl.ContainsKey(address))
                {
                    this.m_Acl[address] = !deny;
                }
                else
                {
                    this.m_Acl.Add(address, !deny);
                }
            }
            finally
            {
                this.m_AclLock.ReleaseLock();
            }
        }

        public void RemoveAcl(PhysicalAddress address)
        {
            this.m_AclLock.AcquireWriterLock(-1);

            try
            {
                if (this.m_Acl.ContainsKey(address))
                {
                    this.m_Acl.Remove(address);
                }
            }
            finally
            {
                this.m_AclLock.ReleaseLock();
            }
        }

        public void ClearAcls()
        {
            this.m_AclLock.AcquireWriterLock(-1);

            try
            {
                this.m_Acl.Clear();
            }
            finally
            {
                this.m_AclLock.ReleaseLock();
            }
        }

        public void Start()
        {
            Trace.WriteLine("[Info] Starting DHCP server...");

            this.m_ActiveLeases.Clear();
            this.m_InactiveLeases.Clear();

            for (InternetAddress address = this.m_StartAddress.Copy(); address.CompareTo(this.m_EndAddress) <= 0; address = address.NextAddress())
            {
                this.m_InactiveLeases.Add(address, new AddressLease(null, address, DateTime.MinValue));
            }

            if (this.m_DhcpInterface == null)
            {
                Trace.TraceInformation("[Info] Enumerating network interfaces.");
                foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (nic.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                    {
                        Trace.TraceInformation("Loopback");
                       this.m_DhcpInterface = nic;
                    }
                    else if ((nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet || nic.NetworkInterfaceType == NetworkInterfaceType.GigabitEthernet || nic.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) && nic.OperationalStatus == OperationalStatus.Up)
                    {
                        Trace.WriteLine("[Info] Using network interface \"{0}\".", nic.Name);
                        this.m_DhcpInterface = nic;
                        break;
                    }
                }

#if TRACE
                if (this.m_DhcpInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                {
                    Trace.TraceInformation("[Info] Active Ethernet Network Interface Not Found. Using Loopback.");
                }
#endif
            }
            
            foreach (UnicastIPAddressInformation interfaceAddress in this.m_DhcpInterface.GetIPProperties().UnicastAddresses)
            {
                if (interfaceAddress.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    this.m_DhcpInterfaceAddress = interfaceAddress.Address;
                    Trace.WriteLine("[Info] Interface address " + this.m_DhcpInterfaceAddress.ToString());

                }
            }

            if (this.m_DhcpInterfaceAddress == null)
            {
                Trace.WriteLine("[Warning] DHCP interface address not found.");
                //this.m_DhcpInterfaceAddress = IPAddress.Broadcast;
                //throw new InvalidOperationException("Unabled to Set Dhcp Interface Address.");
            }

            this.m_Abort = false;

            this.m_CleanupTimer = new Timer(new TimerCallback(this.CleanUp), null, 60000, 30000);

            this.m_DhcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.m_DhcpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            this.m_DhcpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            this.m_DhcpSocket.Bind(new IPEndPoint(IPAddress.Any, DhcpPort));
            
            this.Listen();

            Trace.WriteLine("[Info] DHCP server started.");
        }

        private IPAddress interface_address
        {
            get
            {
                if (this.m_DhcpInterfaceAddress == null)
                {
                    foreach (UnicastIPAddressInformation interfaceAddress in this.m_DhcpInterface.GetIPProperties().UnicastAddresses)
                    {
                        if (interfaceAddress.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            this.m_DhcpInterfaceAddress = interfaceAddress.Address;
                            Trace.WriteLine("[Info] Set Interface address as " + this.m_DhcpInterfaceAddress.ToString());

                        }
                    }
                }


                if (this.m_DhcpInterfaceAddress == null)
                {
                    Trace.WriteLine("[Warning] DHCP interface address not found.");
                    return IPAddress.Broadcast;
                }

                return this.m_DhcpInterfaceAddress;
            }
        }

        public void Stop()
        {
            this.m_AbortLock.AcquireWriterLock(-1);

            try
            {
                this.m_Abort = true;

                this.m_CleanupTimer.Dispose();

                this.m_DhcpSocket.Close();
                this.m_DhcpSocket = null;
            }
            catch
            {

            }
            finally
            {
                this.m_AbortLock.ReleaseLock();
            }
        }

        private void Listen()
        {
            Byte[] messageBufer = new Byte[DhcpMessageMaxSize];
            EndPoint source = new IPEndPoint(0, 0);

            this.m_AbortLock.AcquireReaderLock(-1);

            try
            {
                if (this.m_Abort)
                {
                    return;
                }
                this.m_DhcpSocket.BeginReceiveFrom(messageBufer, 0, DhcpMessageMaxSize, SocketFlags.None, ref source, new AsyncCallback(this.OnReceive), messageBufer);
            }
            finally
            {
                this.m_AbortLock.ReleaseLock();
            }
        }

        private void CleanUp(Object state)
        {
            lock (this.m_LeaseSync)
            {
                List<AddressLease> toRemove = new List<AddressLease>();

                foreach (AddressLease lease in this.m_ActiveLeases.Values)
                {
                    if (!lease.Acknowledged && lease.Expiration > DateTime.Now.Add(this.m_OfferTimeout) ||
                        lease.Acknowledged && lease.Expiration < DateTime.Now)
                    {
                        toRemove.Add(lease);
                    }
                }

                foreach (AddressLease lease in toRemove)
                {
                    this.m_ActiveLeases.Remove(lease.Address);
                    lease.Acknowledged = false;
                    this.m_InactiveLeases.Add(lease.Address, lease);
                }
            }
        }

        private void OnReceive(IAsyncResult result)
        {
            DhcpData data = new DhcpData((Byte[])result.AsyncState);
            data.Result = result;

            if (!this.m_Abort)
            {
                DhcpMessage message = new DhcpMessage(data);

                // Queue this request for processing
                ThreadPool.QueueUserWorkItem(new WaitCallback(this.CompleteRequest), data);

                this.Listen();
            }
        }

        private void CompleteRequest(Object state)
        {
            DhcpData messageData = (DhcpData)state;
            EndPoint source = new IPEndPoint(0, 0);

            this.m_AbortLock.AcquireReaderLock(-1);

            try
            {
                if (this.m_Abort)
                {
                    return;
                }

                messageData.BufferSize = this.m_DhcpSocket.EndReceiveFrom(messageData.Result, ref source);
                messageData.Source = (IPEndPoint)source;
            }
            finally
            {
                this.m_AbortLock.ReleaseLock();
            }

            DhcpMessage message;

            try
            {
                message = new DhcpMessage(messageData);
            }
            catch (ArgumentException ex)
            {
                TraceException("Error Parsing Dhcp Message", ex);
                return;
            }
            catch (InvalidCastException ex)
            {
                TraceException("Error Parsing Dhcp Message", ex);
                return;
            }
            catch (IndexOutOfRangeException ex)
            {
                TraceException("Error Parsing Dhcp Message", ex);
                return;
            }
            catch (Exception ex)
            {
                TraceException("Error Parsing Dhcp Message", ex);
                throw;
            }

            if (message.Operation == DhcpOperation.BootRequest)
            {
                Byte[] messageTypeData = message.GetOptionData(DhcpOption.DhcpMessageType);

                if (messageTypeData != null && messageTypeData.Length == 1)
                {
                    DhcpMessageType messageType = (DhcpMessageType)messageTypeData[0];
                    switch (messageType)
                    {
                        case DhcpMessageType.Discover:
                            Trace.WriteLine("[Info] Dhcp DISCOVER Message Received.");
                            this.DhcpDiscover(message);
                            //Trace.WriteLine("[Info] Dhcp DISCOVER Message Processed.");
                            break;
                        case DhcpMessageType.Request:
                            Trace.WriteLine("[Info] Dhcp REQUEST Message Received.");
                            this.DhcpRequest(message);
                            //Trace.WriteLine("[Info] Dhcp REQUEST Message Processed.");
                            break;
                        default:
                            Trace.TraceWarning("[Error] Unknown Dhcp Message ({0}) Received, Ignoring.", messageType.ToString());
                            break;
                    }
                }
                else
                {
                    Trace.TraceWarning("[Error] Unknown Dhcp Data Received, Ignoring.");
                }
            }
        }

        private static void TraceException(String prefix, Exception ex)
        {
            Trace.TraceError("{0}: ({1}) - {2}\r\n{3}", prefix, ex.GetType().Name, ex.Message, ex.StackTrace);

            if (ex.InnerException != null)
            {
                TraceException("    Inner Exception", ex.InnerException);
            }
        }

        private void DhcpDiscover(DhcpMessage message)
        {
            Byte[] addressRequestData = message.GetOptionData(DhcpOption.AddressRequest);
            if (addressRequestData == null)
            {
                addressRequestData = message.ClientAddress;
            }
            
            InternetAddress addressRequest = new InternetAddress(addressRequestData);

            // Assume we're on an ethernet network
            Byte[] hardwareAddressData = new Byte[6];
            Array.Copy(message.ClientHardwareAddress, hardwareAddressData, 6);
            PhysicalAddress clientHardwareAddress = new PhysicalAddress(hardwareAddressData);

            AddressLease offer = null;

            Trace.WriteLine("[Info] DHCP DISCOVER from " + message.Hostname);
            // If this client is explicitly allowed, or they are not denied and the allow any flag is set
            if (this.m_Acl.ContainsKey(clientHardwareAddress) && this.m_Acl[clientHardwareAddress] ||
                !this.m_Acl.ContainsKey(clientHardwareAddress) && this.m_AllowAny)
            {
                if (this.m_Reservations.ContainsKey(clientHardwareAddress))
                {
                    offer = new AddressLease(clientHardwareAddress, this.m_Reservations[clientHardwareAddress], DateTime.Now.Add(this.m_LeaseDuration));
                }
                else
                {
                    lock (this.m_LeaseSync)
                    {
                        if (!addressRequest.Equals(InternetAddress.Empty))
                        {
                            Trace.WriteLine("[Info] Addr requested " + (new IPAddress(addressRequest.ToArray())).ToString());
                            if (this.m_InactiveLeases.ContainsKey(addressRequest))
                            {
                                offer = this.m_InactiveLeases[addressRequest];
                                this.m_InactiveLeases.Remove(addressRequest);
                                this.m_ActiveLeases.Add(addressRequest, offer);
                            }
                            else if(this.m_ActiveLeases.ContainsKey(addressRequest) /*&& this.m_ActiveLeases[addressRequest].Owner.Equals(clientHardwareAddress)*/)
                            {
                                offer = this.m_ActiveLeases[addressRequest];
                            }else
                            {
                                Trace.WriteLine("[Info] Random leased");
                                offer = this.m_InactiveLeases.Values[0];
                                this.m_InactiveLeases.Remove(offer.Address);
                                this.m_ActiveLeases.Add(offer.Address, offer);
                            }
                        }
                        else if (this.m_InactiveLeases.Count > 0)
                        {
                            Trace.WriteLine("[Info] Random leased");
                            offer = this.m_InactiveLeases.Values[0];
                            this.m_InactiveLeases.Remove(offer.Address);
                            this.m_ActiveLeases.Add(offer.Address, offer);
                        }
                    }
                }
            }
            else
            {
                Trace.WriteLine("[Info] DHCP MAC Addr Denied");
            }

            if (offer == null)
            {
                this.SendNak(message);
            }
            else
            {

                Trace.WriteLine("[Info] DISCOVER offering " + (new IPAddress(offer.Address.ToArray()).ToString()) + " to " + message.Hostname);
                offer.Acknowledged = false;
                offer.Expiration = DateTime.Now.Add(this.m_OfferTimeout);
                offer.SessionId = message.SessionId;
                offer.Owner = clientHardwareAddress;
                if (message.Hostname == "raspberrypi") {
                    this.raspberry_address = new IPAddress(offer.Address.ToArray());
                    this.raspberry_mac = message.ClientHardwareAddress;
                };
                this.SendOffer(message, offer);
            }
        }

        private void DhcpRequest(DhcpMessage message)
        {
            Byte[] addressRequestData = message.GetOptionData(DhcpOption.AddressRequest);
            if (addressRequestData == null)
            {
                addressRequestData = message.ClientAddress;
            }

            InternetAddress addressRequest = new InternetAddress(addressRequestData);

            if (addressRequest.IsEmpty)
            {
                Trace.WriteLine("[Error] Address request is empty");
                this.SendNak(message);
                return;
            }

            // Assume we're on an ethernet network
            Byte[] hardwareAddressData = new Byte[6];
            Array.Copy(message.ClientHardwareAddress, hardwareAddressData, 6);
            PhysicalAddress clientHardwareAddress = new PhysicalAddress(hardwareAddressData);

            AddressLease assignment = null;
            Boolean ack = false;
            Trace.WriteLine("[Info] DHCP REQUEST from " + message.Hostname + " addr " + (new IPAddress(addressRequest.ToArray())).ToString());
            // If this client is explicitly allowed, or they are not denied and the allow any flag is set
            if (this.m_Acl.ContainsKey(clientHardwareAddress) && this.m_Acl[clientHardwareAddress] ||
                !this.m_Acl.ContainsKey(clientHardwareAddress) && this.m_AllowAny)
            {
                if (this.m_Reservations.ContainsKey(clientHardwareAddress))
                {
                    assignment = new AddressLease(clientHardwareAddress, this.m_Reservations[clientHardwareAddress], DateTime.Now.Add(this.m_LeaseDuration));
                    if (addressRequest.Equals(assignment.Address))
                    {
                        ack = true;
                    }
                }
                else
                {
                    lock (this.m_LeaseSync)
                    {
                        if (this.m_ActiveLeases.ContainsKey(addressRequest) /* &&
                            (this.m_ActiveLeases[addressRequest].Owner.Equals(clientHardwareAddress) || this.m_ActiveLeases[addressRequest].SessionId == message.SessionId) */)
                        {
                            Trace.WriteLine("Active lease exists, reset lease.");
                            assignment = this.m_ActiveLeases[addressRequest];
                            assignment.Acknowledged = true;
                            assignment.Owner = clientHardwareAddress;
                            assignment.Expiration = DateTime.Now.Add(this.m_LeaseDuration);
                            ack = true;
                        }else if(this.m_InactiveLeases.ContainsKey(addressRequest))
                        {
                            Trace.WriteLine("Active lease not exists, moving from non active lease.");
                            assignment = this.m_InactiveLeases[addressRequest];
                            this.m_InactiveLeases.Remove(addressRequest);
                            this.m_ActiveLeases.Add(addressRequest, assignment);
                            assignment.Acknowledged = true;
                            assignment.Owner = clientHardwareAddress;
                            assignment.Expiration = DateTime.Now.Add(this.m_LeaseDuration);
                            ack = true;
                        }
                    }
                }
            }else
            {
                Trace.WriteLine("[Info] DHCP MAC Addr Denied");
            }

            if (ack)
            {
                if (message.Hostname == "raspberrypi")
                {
                    this.raspberry_address = new IPAddress(assignment.Address.ToArray());
                    this.raspberry_mac = message.ClientHardwareAddress;
                };
                this.SendAck(message, assignment);
            }
            else
            {
                Trace.WriteLine("[Error] DHCP ack not true");
                this.SendNak(message);
            }
        }

        private void SendOffer(DhcpMessage message, AddressLease offer)
        {
            //Trace.TraceInformation("Sending Dhcp Offer.");

            DhcpMessage response = new DhcpMessage();
            response.Operation = DhcpOperation.BootReply;
            response.Hardware = HardwareType.Ethernet;
            response.HardwareAddressLength = 6;
            response.SecondsElapsed = message.SecondsElapsed;
            response.SessionId = message.SessionId;
            response.Flags = message.Flags;

            response.AssignedAddress = offer.Address.ToArray();
            response.ClientHardwareAddress = message.ClientHardwareAddress;

            response.AddOption(DhcpOption.DhcpMessageType, (Byte)DhcpMessageType.Offer);
            response.AddOption(DhcpOption.AddressRequest, offer.Address.ToArray());
            AddDhcpOptions(response);

            Byte[] paramList = message.GetOptionData(DhcpOption.ParameterList);
            if (paramList != null)
            {
                response.OptionOrdering = paramList;
            }

            this.SendReply(response);
            Trace.WriteLine("[Info] DHCP offered " + offer.Address.ToString() + " to " + BitConverter.ToString(message.ClientHardwareAddress));
        }

        private void SendAck(DhcpMessage message, AddressLease lease)
        {

            DhcpMessage response = new DhcpMessage();
            response.Operation = DhcpOperation.BootReply;
            response.Hardware = HardwareType.Ethernet;
            response.HardwareAddressLength = 6;
            response.SecondsElapsed = message.SecondsElapsed;
            response.SessionId = message.SessionId;

            response.AssignedAddress = lease.Address.ToArray();
            response.ClientHardwareAddress = message.ClientHardwareAddress;

            response.AddOption(DhcpOption.DhcpMessageType, (Byte)DhcpMessageType.Ack);
            response.AddOption(DhcpOption.AddressRequest, lease.Address.ToArray());
            AddDhcpOptions(response);

            this.SendReply(response);
            Trace.WriteLine("[Info] DHCP ACK sent.");
        }

        private void AddDhcpOptions(DhcpMessage response)
        {
            response.AddOption(DhcpOption.AddressTime, DhcpMessage.ReverseByteOrder(BitConverter.GetBytes((Int32)this.m_LeaseDuration.TotalSeconds)));
            response.AddOption(DhcpOption.Router, this.m_Gateway.ToArray());
            response.AddOption(DhcpOption.SubnetMask, this.m_Subnet.ToArray());

            if (!String.IsNullOrEmpty(this.m_DnsSuffix))
            {
                response.AddOption(DhcpOption.DomainNameSuffix, Encoding.ASCII.GetBytes(this.m_DnsSuffix));
            }

            if (this.m_DnsServers.Count > 0)
            {
                Byte[] dnsServerAddresses = new Byte[this.m_DnsServers.Count * 4];
                for (Int32 i = 0; i < this.m_DnsServers.Count; i++)
                {
                    this.m_DnsServers[i].ToArray().CopyTo(dnsServerAddresses, i * 4);
                }

                response.AddOption(DhcpOption.DomainNameServer, dnsServerAddresses);
            }
        }

        private void SendNak(DhcpMessage message)
        {
            //Trace.TraceInformation("[Info] Sending DHCP Negative Acknowledge.", Thread.CurrentThread.ManagedThreadId);

            DhcpMessage response = new DhcpMessage();
            response.Operation = DhcpOperation.BootReply;
            response.Hardware = HardwareType.Ethernet;
            response.HardwareAddressLength = 6;
            response.SecondsElapsed = message.SecondsElapsed;
            response.SessionId = message.SessionId;

            response.ClientHardwareAddress = message.ClientHardwareAddress;

            response.AddOption(DhcpOption.DhcpMessageType, (Byte)DhcpMessageType.Nak);

            this.SendReply(response);
            Trace.TraceInformation("[Info] DHCP NACK Sent.");
        }

        private void SendReply(DhcpMessage response)
        {
            IPAddress dhcp_response_addr = this.m_DhcpInterfaceAddress;
            if (BitConverter.ToString(response.ClientHardwareAddress) == BitConverter.ToString(this.m_DhcpInterface.GetPhysicalAddress().GetAddressBytes()))
            {
                Trace.TraceInformation("[Info] DHCP Localcast.");
                response.AddOption(DhcpOption.DhcpAddress, this.m_DhcpInterfaceAddress.GetAddressBytes());
            }else
            {
                Trace.TraceInformation("[Info] DHCP Broadcast.");
                response.AddOption(DhcpOption.DhcpAddress, IPAddress.Parse("192.168.15.255").GetAddressBytes());
                dhcp_response_addr = IPAddress.Parse("192.168.15.255");
            }

            Byte[] sessionId = BitConverter.GetBytes(response.SessionId);


            try
            {
                this.m_DhcpSocket.SendTo(response.ToArray(), new IPEndPoint(dhcp_response_addr, DhcpClientPort));
            }
            catch (Exception ex)
            {
                Trace.WriteLine("[Error] Network is not ready " + ex.Message);
                //TraceException("Error Sending Dhcp Reply", ex);
                //throw;
            }
        }
    }
}
