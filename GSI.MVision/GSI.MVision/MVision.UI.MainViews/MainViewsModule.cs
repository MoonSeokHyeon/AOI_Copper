using MVision.Common.Shared;
using MVision.UI.MainViews.View;
using MVision.UI.MainViews.ViewModel;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.UI.MainViews
{
    public class MainViewsModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            ViewModelLocationProvider.Register<MainAutoView, MainAutoViewModel>();
            ViewModelLocationProvider.Register<MainInterfaceView, MainInterfaceViewModel>();
            ViewModelLocationProvider.Register<MainVisionLogView, MainVisionLogViewModel>();
            ViewModelLocationProvider.Register<MainEditView, MainEditViewModel>();
            ViewModelLocationProvider.Register<MainCalibrationView, MainCalibrationViewModel>();
            ViewModelLocationProvider.Register<MainOptionView, MainOptionViewModel>();

            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(SharedRegion.MainView, typeof(MainAutoView));
            regionManager.RegisterViewWithRegion(SharedRegion.MainView, typeof(MainInterfaceView));
            regionManager.RegisterViewWithRegion(SharedRegion.MainView, typeof(MainVisionLogView));
            regionManager.RegisterViewWithRegion(SharedRegion.MainView, typeof(MainEditView));
            regionManager.RegisterViewWithRegion(SharedRegion.MainView, typeof(MainCalibrationView));
            regionManager.RegisterViewWithRegion(SharedRegion.MainView, typeof(MainOptionView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
