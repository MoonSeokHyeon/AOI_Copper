using MaterialDesignThemes.Wpf;
using MVision.MairaDB;
using MVision.UI.OptionViews.VIew;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVision.UI.MainViews.ViewModel
{
    public class MainOptionViewModel : BindableBase
    {
        #region Properties

        SystemOptionView systemOptionsControl = null;
        public SystemOptionView SystemOptionsControl { get => this.systemOptionsControl; set => SetProperty(ref this.systemOptionsControl, value); }

        SystemSettingView systemSettingControl = null;
        public SystemSettingView SystemSettingControl { get => this.systemSettingControl; set => SetProperty(ref this.systemSettingControl, value); }

        private bool isSaving;
        public bool IsSaving { get => this.isSaving; set => SetProperty(ref this.isSaving, value); }

        IContainerProvider provider = null;
        MariaManager sql = null;

        #endregion

        #region ICommand

        public ICommand CloseCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        #endregion     

        public MainOptionViewModel(IContainerProvider prov, MariaManager sql)
        {
            this.provider = prov;
            this.sql = sql;

            this.SystemOptionsControl = provider.Resolve<SystemOptionView>();
            this.SystemSettingControl = provider.Resolve<SystemSettingView>();

            InitDelegeteCommand();
        }

        public void Init()
        {
            this.SystemOptionsControl.ViewModel.LoadSystemOptions();
            this.SystemSettingControl.ViewModel.LoadSystemSetting();
        }

        private void InitDelegeteCommand()
        {
            this.SaveCommand = new DelegateCommand(ExecuteSaveCommand);
        }

        private async void ExecuteSaveCommand()
        {
            this.IsSaving = true;

            this.SystemOptionsControl.ViewModel.SaveSystemOptions();
            this.SystemSettingControl.ViewModel.SaveSystemSetting();

            await Task.Delay(TimeSpan.FromSeconds(1));

            this.IsSaving = false;

            DialogHost.CloseDialogCommand.Execute(null, null);
        }
    }
}
