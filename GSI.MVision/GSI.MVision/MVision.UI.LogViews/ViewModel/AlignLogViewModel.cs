using MVision.Common.Event;
using MVision.Common.Event.EventArg;
using MVision.Common.Shared;
using MVision.MairaDB;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVision.UI.LogViews.ViewModel
{
    public class AlignLogViewModel : BindableBase
    {
        //private List<AlignHistory> alignLogList;
        //public List<AlignHistory> AlignLogList
        //{
        //    get { return this.alignLogList; }
        //    set { SetProperty(ref this.alignLogList, value); }
        //}

        private DateTime selectedTime;
        public DateTime SelectedTime
        {
            get { return selectedTime; }
            set { SetProperty(ref this.selectedTime, value); }
        }

        private DateTime selectedDate;
        public DateTime SelectedDate
        {
            get { return selectedDate; }
            set { SetProperty(ref this.selectedDate, value); }
        }

        public ICommand SearchLogCommand { get; set; }

        //SqlManager sql = null;
        MariaManager sql = null;

        public AlignLogViewModel(MariaManager sqlManager)
        {
            this.sql = sqlManager;
            this.SearchLogCommand = new DelegateCommand(ExecuteSearchLogCommand);
        }
        public void Init()
        {
            this.SelectedDate = DateTime.Now;
            this.SelectedTime = DateTime.Now;

            //this.AlignLogList = this.sql.AlignHistory.GetAll().OrderByDescending(x => x.CreateDate).Take(200).ToList();
        }

        private void ExecuteSearchLogCommand()
        {
            var targetTime = new DateTime(this.SelectedDate.Year, this.SelectedDate.Month, this.SelectedDate.Day, SelectedTime.Hour, 0, 0);
            //this.AlignLogList = this.sql.AlignHistory.GetAll().Where(d => Math.Abs((d.CreateDate - targetTime).TotalHours) < 1).OrderByDescending(x => x.CreateDate).ToList();
        }
    }
}
