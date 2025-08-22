using MVision.UI.LogViews.View;
using MVision.UI.LogViews.ViewModel;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.UI.LogViews
{
    public class LogViewModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            //List
            ViewModelLocationProvider.Register<AlignLogView, AlignLogViewModel>();
            //ViewModelLocationProvider.Register<InspectionLogView, InspectionLogViewModel>();
            ViewModelLocationProvider.Register<ViewLogView, ViewLogViewModel>();

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}
