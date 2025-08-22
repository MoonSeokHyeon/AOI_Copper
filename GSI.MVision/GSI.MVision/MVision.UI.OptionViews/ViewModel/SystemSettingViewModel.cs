using MVision.Common.DBModel;
using MVision.MairaDB;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.UI.OptionViews.ViewModel
{
    public class SystemSettingViewModel : BindableBase
    {
        #region Properties
        private string groupBoxHeader;
        public string GroupBoxHeader
        {
            get { return groupBoxHeader; }
            set { SetProperty(ref this.groupBoxHeader, value); }
        }

        private ObservableCollection<SystemSetting> settingList = new ObservableCollection<SystemSetting>();
        public ObservableCollection<SystemSetting> SettingList
        {
            get { return settingList; }
            set { SetProperty(ref this.settingList, value); }
        }

        MariaManager sql = null;
        IContainerProvider provider = null;

        #endregion

        #region Struct
        public SystemSettingViewModel(IContainerProvider provider, MariaManager sql)
        {
            this.sql = sql;
            this.provider = provider;
        }

        public void Init()
        {
        }

        #endregion

        #region Public Method
        public void LoadSystemSetting()
        {
            var rr = this.sql.SystemSettingData.All().ToList();
            rr.ForEach(x =>
            {
                //if (!x.Name.Equals(ConstETCString.UseTimeSync) || !x.Name.Equals(ConstETCString.UseAlwaysLightOn) || !x.Name.Equals(ConstGraphicString.ShowOnlyOKCaliper))
                this.SettingList.Add(new SystemSetting { Name = x.Name, Desc = x.Desc, Value = x.Value });
            });
        }

        public void SaveSystemSetting()
        {
            var settingll = this.SettingList.ToList();
            settingll.ForEach(x =>
            {
                var item = sql.SystemSettingData.All().ToList().FirstOrDefault(i => i.Name.Equals(x.Name));

                item.Value = x.Value;

                //if (!x.Name.Equals(ConstETCString.UseTimeSync) || !x.Name.Equals(ConstETCString.UseAlwaysLightOn) || !x.Name.Equals(ConstGraphicString.ShowOnlyOKCaliper))
                sql.SystemSettingData.Update(item);
            });
        }

        #endregion
    }
}
