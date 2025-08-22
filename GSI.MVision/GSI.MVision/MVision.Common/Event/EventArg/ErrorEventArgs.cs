using MVision.Common.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.Common.Event.EventArg
{
    public class ErrorEventArgs : EventArgs
    {
        public eExecuteZoneID zone { get; set; }
        public string errorName { get; set; }
        public bool isTrue { get; set; }
        public int Value { get; set; }
    }
}
