using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluxDiscoverDiagnosis
{
    public class DiscoverResult
    {
        public DiscoverResult()
        {
            RenewS1();
            ts2 = DateTime.MinValue;
        }

        public void RenewS1()
        {
            ts1 = DateTime.Now;
        }

        public void RenewS2(string name, string ver)
        {
            ts2 = DateTime.Now;
            Name = name;
            FirmwareVersion = ver;
        }

        DateTime ts1;
        DateTime ts2;

        public string Name { get; set; }
        public string FirmwareVersion { get; set; }
        public string IPAddr { get { return EndPoint.ToString(); } }

        public Guid UUID { get; set; }
        public string Src { get; set; }
        public int ST1 { get { return (int)((DateTime.Now - ts1).TotalSeconds); } }

        public int? ST2 { get {
                if(ts2 == DateTime.MinValue)
                {
                    return null;
                } else
                {
                    return (int)((DateTime.Now - ts2).TotalSeconds);
                }
        } }

        public System.Net.EndPoint EndPoint;
    }
}
