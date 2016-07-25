
using System;
using WebSocketSharp;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Net;

namespace FluxDiscoverDiagnosis
{
    public class UsbConfig
    {
        
        private WebSocket _ws;
        int _connectingMachine = 0;
        string _deltaName = "";
        string _deltaIP = "";
        public Status status;
        public Form1 parent;

        public enum Status
        {
            ConnectingAPI,
            ListingMachine,
            ConnectingMachine,
            WaitingMachine,
            GettingIP,
            TaskFinished,
            Error
        }


        public UsbConfig(string uri)
        {
            _ws = new WebSocket(uri);
        }

        private void onTick(object sender, ElapsedEventArgs e)
        {
            if (_ws.ReadyState != WebSocketState.Open) return;
            if(this.status == Status.GettingIP)
            {
                this.SendMessage("get network");
            }
        }

        private void SendMessage(string message)
        {
            _ws.Send(message);
        }

        public void Connect()
        {
            StartListen();
            _ws.Connect();
            if(_ws.ReadyState == WebSocketState.Open){
                System.Timers.Timer timer = new System.Timers.Timer();
                timer.Enabled = true;
                timer.Interval = 1000; //执行间隔时间,单位为毫秒; 这里实际间隔为10分钟  
                timer.Start();
                timer.Elapsed += new System.Timers.ElapsedEventHandler(onTick);
            }
        }
        private void StartListen()
        {
            _ws.OnError += (sender, e) =>
            {
                MessageBox.Show("Unable to connet FLUX API");
            };
            _ws.OnOpen += (sender, e) =>
            {
                this.status = Status.ListingMachine;
                this.SendMessage("list\r\n");
            };
            _ws.OnMessage += (sender,  e) => {
                MessageEventArgs erg = e;
                //MessageBox.Show(e.Data);
                dynamic resp = JObject.Parse(erg.Data);
                switch (this.status)
                {
                    case Status.ListingMachine:
                        if (resp.ports != null && resp.ports.Count > 0)
                        {
                            this.status = Status.ConnectingMachine;
                            this._connectingMachine = resp.ports.Count;
                            foreach (string port in resp.ports)
                            {
                                this.SendMessage("connect " + port);
                            }
                        }else if (resp.error != null)
                        {
                            this.SendMessage("list");
                        }
                        break;
                    case Status.ConnectingMachine:
                        if (resp.serial != null)
                        {
                            this._deltaName = resp.name;
                            this.status = Status.GettingIP;
                            MessageBox.Show("You're now connected to " + this._deltaName);
                        }else if(resp.status == "error" && resp.error == "DEVICE_ERROR")
                        {
                            this._connectingMachine--;
                            if (resp.info.Value.Contains("Permission"))
                            {
                                MessageBox.Show("Please detach the USB from your computer side for 10 secs, and attach it again.");
                            }
                            if(this._connectingMachine == 0)
                            {
                                MessageBox.Show("Unable to connect machine with USB");
                                this.status = Status.Error;
                            }
                        }
                        break;
                    case Status.GettingIP:
                        if (resp.ipaddr != null && resp.ipaddr.Count > 0)
                        {
                            bool legitIP = false;
                            foreach (string ip in resp.ipaddr)
                            {
                                IPAddress ipaddr;
                                if (IPAddress.TryParse(ip, out ipaddr))
                                {
                                    if (ipaddr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                                    {
                                        legitIP = true;
                                        this._deltaIP = ip;
                                        this.status = Status.TaskFinished;
                                        this.parent.HandleDeviceIP(this._deltaIP);
                                        MessageBox.Show("Got the IP Address " + _deltaIP);
                                    }
                                }
                            }
                            if (!legitIP)
                            {
                                MessageBox.Show(this._deltaName + " is not properly connected to the WiFi");
                            }
                        }
                        break;
                    default:
                        break;
                }
                   


                
            };
        }
        
    }
}
