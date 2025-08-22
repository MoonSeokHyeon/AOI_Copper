using MVision.UI.CogToolEditViews.View;
using MVision.UI.CogToolEditViews.ViewModel;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.UI.CogToolEditViews
{
    public class CogToolEditViewModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            ViewModelLocationProvider.Register<PatternEditView, PatternEditViewModel>();
            ViewModelLocationProvider.Register<LineFindEditView, LineFindEditViewModel>();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}
