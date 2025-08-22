using MVision.Common.Model;
using MVision.Common.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.Common.Event.EventArg
{
    public class SchedulerEventArgs : EventArgs
    {
        public DateTime CreateTime { get; set; }
        public eSchedulerMessageKind MessageKind { get; set; }
        public ProcessPosition processPosition { get; set; } = new ProcessPosition();
        public Enum SubKind { get; set; }
        public Enum LevelKind { get; set; }
        public string MessageKey { get; set; }
        public string MessageText { get; set; }
        public object Arg { get; set; }

        public SchedulerEventArgs()
        {
            this.processPosition = new ProcessPosition();
            this.CreateTime = DateTime.Now;
        }
    }
}
