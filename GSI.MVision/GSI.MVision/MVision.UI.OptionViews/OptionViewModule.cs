using MVision.UI.OptionViews.VIew;
using MVision.UI.OptionViews.ViewModel;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.UI.OptionViews
{
    public class OptionViewModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            ViewModelLocationProvider.Register<SystemOptionView, SystemOptionViewModel>();
            ViewModelLocationProvider.Register<SystemSettingView, SystemSettingViewModel>();

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
