using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.UI.InteractivityViews.ViewModel
{
    public class ValueInputViewModel : BindableBase
    {
        #region Properties

        private string inputValue;
        public string InputValue
        {
            get { return inputValue; }
            set { SetProperty(ref this.inputValue, value); }
        }

        private string inputValueName;
        public string InputValueName
        {
            get { return inputValueName; }
            set { SetProperty(ref this.inputValueName, value); }
        }

        private string currentValue;

        public string CurrentValue
        {
            get { return currentValue; }
            set { SetProperty(ref this.currentValue, value); }
        }

        #endregion

        #region Construct

        public ValueInputViewModel()
        {

        }

        public void Init()
        {
        }

        #endregion
    }
}
