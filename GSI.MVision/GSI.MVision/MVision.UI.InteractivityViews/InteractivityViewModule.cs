using MVision.UI.InteractivityViews.View;
using MVision.UI.InteractivityViews.ViewModel;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.UI.InteractivityViews
{
    public class InteractivityViewModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            //ViewModelLocationProvider.Register<CalibrationInfoView, CalibrationInfoViewModel>();
            ViewModelLocationProvider.Register<ComfirmationView, ComfirmationViewModel>();
            //ViewModelLocationProvider.Register<ModelManagerView, ModelManagerViewModel>();
            ViewModelLocationProvider.Register<NotificationView, NotificationViewModel>();
            ViewModelLocationProvider.Register<ValueInputView, ValueInputViewModel>();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}
