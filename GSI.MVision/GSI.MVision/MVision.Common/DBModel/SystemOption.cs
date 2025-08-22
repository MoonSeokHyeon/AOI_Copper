using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.Common.DBModel
{
    public class SystemOption : BindableBase
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private bool value;
        public bool Value
        {
            get { return value; }
            set { SetProperty(ref value, value); }
        }

        private string desc;
        public string Desc
        {
            get { return desc; }
            set { SetProperty(ref desc, value); }
        }


        private DateTime editTime;
        public DateTime EditTime
        {
            get { return editTime; }
            set { SetProperty(ref editTime, value); }
        }
    }
}
