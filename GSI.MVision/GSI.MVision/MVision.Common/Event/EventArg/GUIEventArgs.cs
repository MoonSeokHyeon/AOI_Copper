using MVision.Common.Model;
using MVision.Common.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.Common.Event.EventArg
{
    public class GUIEventArgs : EventArgs
    {
        public DateTime CreateTime { get; set; }

        public eGUIMessageKind MessageKind { get; set; }
        public ProcessPosition ProcessPosition { get; set; } = new ProcessPosition();
        public Enum SubKind { get; set; }
        public Enum SubKind1 { get; set; }
        public string MessageKey { get; set; }
        public string MessageText { get; set; }
        public object Arg { get; set; }

        public GUIEventArgs()
        {
            this.ProcessPosition = new ProcessPosition();
            this.CreateTime = DateTime.Now;
        }
    }
}
