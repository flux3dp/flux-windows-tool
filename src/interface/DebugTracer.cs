using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinDHCP
{
    class DebugTracer : TraceListener
    {
        public BasicInterface basic_interface = null;
        public DebugTracer(BasicInterface bi)
        {
            this.basic_interface = bi;
        }
        

        public override void Write(string message)
        {
            basic_interface.AppendLog(message);
        }

        public override void WriteLine(string message)
        {
            basic_interface.AppendLog(message);
        }
    }
}
