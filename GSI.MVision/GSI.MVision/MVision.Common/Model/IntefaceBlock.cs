using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MVision.Common.Model
{
    public class IntefaceBlock : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected bool SetField<T>(ref T field, T newValue = default(T), bool isComparer = true, [CallerMemberName] string propertyName = null)
        {
            if (isComparer)
                if (EqualityComparer<T>.Default.Equals(field, newValue)) return false;

            field = newValue;

            if (propertyName != null)
            {
                OnPropertyChanged(propertyName);

                return true;
            }

            return false;
        }

        #endregion

        private string addr;

        public string Addr
        {
            get { return addr; }
            set { SetField(ref this.addr, value); }
        }

        private string tag;
        public string Tag
        {
            get { return tag; }
            set { SetField(ref this.tag, value); }
        }

        private string value;
        public string Value
        {
            get { return value; }
            set { SetField(ref this.value, value); }
        }

        private bool isOn;
        public bool IsOn
        {
            get { return isOn; }
            set { SetField(ref this.isOn, value); }
        }

        public bool IsBit { get; set; }
    }
}
