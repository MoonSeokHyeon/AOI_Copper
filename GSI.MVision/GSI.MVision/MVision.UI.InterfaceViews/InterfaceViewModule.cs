using MVision.UI.InterfaceViews.View;
using MVision.UI.InterfaceViews.ViewModel;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.UI.InterfaceViews
{
    public class InterfaceViewModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            ViewModelLocationProvider.Register<PLCInterfaceHTypeView, PLCInterfaceHTypeViewModel>();
            ViewModelLocationProvider.Register<PLCInterfaceVTypeView, PLCInterfaceVTypeViewModel>();
            //ViewModelLocationProvider.Register<FakePLCInterfaceHTypeView, FakePLCInterfaceHTypeViewModel>();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}
