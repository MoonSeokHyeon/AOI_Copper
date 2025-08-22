using MVision.Common.Event.EventArg;
using MVision.Common.Event;
using MVision.Common.Shared;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace MVision.UI.LogViews.ViewModel
{
    public class ViewLogViewModel : BindableBase
    {
        public eExecuteZoneID ZoneID { get; set; } = eExecuteZoneID.NONE;
        public eExecuteZoneID ZoneIDSub { get; set; } = eExecuteZoneID.NONE;

        private ObservableCollection<SchedulerEventArgs> logList = new ObservableCollection<SchedulerEventArgs>();

        public ObservableCollection<SchedulerEventArgs> LogList
        {
            get { return logList; }
            set { SetProperty(ref this.logList, value); ; }
        }

        public ViewLogViewModel(IEventAggregator aggregator)
        {
            aggregator.GetEvent<SchedulerMessageEvent>().Unsubscribe(UICallbackCommunication);
            aggregator.GetEvent<SchedulerMessageEvent>().Subscribe(UICallbackCommunication, ThreadOption.UIThread);
        }
        private void UICallbackCommunication(SchedulerEventArgs obj)
        {
            //if (ZoneID != obj.processPosition.ExecuteZoneID && ZoneIDSub != obj.processPosition.ExecuteZoneID) return;

            switch (obj.MessageKind)
            {
                case eSchedulerMessageKind.ViewLoggerChanged:
                    AddList(obj);
                    break;
                default:
                    break;
            }
        }

        private void AddList(SchedulerEventArgs obj)
        {
            if (obj.MessageText == null) return;

            LogList.Insert(0, obj);

            if (LogList.Count > 200)
                LogList.RemoveAt(LogList.Count - 1);
        }
    }
}
