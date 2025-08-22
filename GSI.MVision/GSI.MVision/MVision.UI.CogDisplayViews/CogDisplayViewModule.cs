using MVision.UI.CogDisplayViews.View;
using MVision.UI.CogDisplayViews.ViewModel;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.UI.CogDisplayViews
{
    public class CogDisplayViewModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            ViewModelLocationProvider.Register<SingleCamView, SingleCamViewModel>();
            //ViewModelLocationProvider.Register<SingleDisplayView, DualDisplayViewModel>();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}
