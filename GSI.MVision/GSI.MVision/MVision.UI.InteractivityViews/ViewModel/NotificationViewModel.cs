using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.UI.InteractivityViews.ViewModel
{
    public class NotificationViewModel : BindableBase
    {
        #region Properties

        private string message;
        public string Message
        {
            get { return message; }
            set { this.SetProperty(ref this.message, value); }
        }

        #endregion

        #region Construct

        public NotificationViewModel()
        {

        }

        #endregion
    }
}
