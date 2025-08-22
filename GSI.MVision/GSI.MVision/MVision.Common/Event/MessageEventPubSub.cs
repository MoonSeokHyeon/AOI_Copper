using MVision.Common.Event.EventArg;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.Common.Event
{
    public class ApplicationExitEvent : PubSubEvent<string> { }
    public class SchedulerMessageEvent : PubSubEvent<SchedulerEventArgs> { }
    public class GUIMessageEvent : PubSubEvent<GUIEventArgs> { }
    public class ErrorMessageEvent : PubSubEvent<EventArg.ErrorEventArgs> { }        //Kkm 추후 Alarm상태 UI 반영 및 Alarm History 기록 
}
