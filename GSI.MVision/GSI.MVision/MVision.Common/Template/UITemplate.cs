using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MVision.Common.Template
{
    public class ListBoxItem : BindableBase
    {
        public EventHandler OnValueChanged;

        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private dynamic value;
        public dynamic Value
        {
            get { return value; }
            set
            {
                if (SetProperty(ref this.value, value))
                { OnValueChanged?.Invoke(this, null); }
            }
        }

        public double ToDouble()
        {
            var retValue = 0.0;

            try
            {
                retValue = double.Parse(Value);
            }
            catch (Exception ex)
            {
                return double.NaN;
            }

            return retValue;
        }

        public int ToInt()
        {
            var retValue = 0;

            try
            {
                retValue = int.Parse(Value);
            }
            catch (Exception ex)
            {
                return int.MaxValue;
            }

            return retValue;
        }
    }

    public class ItemList : BindableBase
    {
        public EventHandler OnValueChanged;

        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private string data;
        public string Data
        {
            get { return data; }
            set { SetProperty(ref data, value); }
        }
    }

    public class ListBoxItem3Column : BindableBase
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private dynamic value1;
        public dynamic Value1
        {
            get { return value1; } 
            set { SetProperty(ref this.value1, value); }
        }

        private dynamic value2;
        public dynamic Value2
        {
            get { return value2; }
            set { SetProperty(ref this.value2, value); }
        }

        private string unit;
        public string Unit
        {
            get { return unit; }
            set { SetProperty(ref this.unit, value); }
        }
    }

    public class ListBoxTemplate : BindableBase
    {
        private string tag;
        public string Tag
        {
            get { return tag; ; }
            set { SetProperty(ref this.tag, value); }
        }

        private double value1;

        public double Value1
        {
            get { return value1; }
            set { SetProperty(ref this.value1, value); }
        }
    }

    public class SelectableModel : INotifyPropertyChanged
    {
        private bool _isSelected;
        private string _name;
        private string _description;
        private char _code;
        private double _numeric;
        private string _food;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public char Code
        {
            get { return _code; }
            set
            {
                if (_code == value) return;
                _code = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (_description == value) return;
                _description = value;
                OnPropertyChanged();
            }
        }

        public double Numeric
        {
            get { return _numeric; }
            set
            {
                if (_numeric == value) return;
                _numeric = value;
                OnPropertyChanged();
            }
        }

        public string Food
        {
            get { return _food; }
            set
            {
                if (_food == value) return;
                _food = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
