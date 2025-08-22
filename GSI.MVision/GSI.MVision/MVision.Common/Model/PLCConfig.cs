using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.Common.Model
{
    public class PLCConfig
    {
        public string Name { get; set; }
        public string Addr { get; set; }
        public int PortNo { get; set; }
        public string Dec { get; set; }
        public bool IsAscii { get; set; }
    }
}
