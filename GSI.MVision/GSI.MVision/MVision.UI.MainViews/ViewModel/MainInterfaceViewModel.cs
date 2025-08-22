using MVision.UI.InterfaceViews.View;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.UI.MainViews.ViewModel
{
    public class MainInterfaceViewModel : BindableBase
    {
        PLCInterfaceHTypeView interfacePIOView = null;
        public PLCInterfaceHTypeView InterfacePIOView { get => this.interfacePIOView; set => SetProperty(ref this.interfacePIOView, value); }

        IContainerProvider provider = null;
        public MainInterfaceViewModel(IContainerProvider prov)
        {
            provider = prov;

            this.InterfacePIOView = provider.Resolve<PLCInterfaceHTypeView>();
            this.InterfacePIOView.ViewModel.SubText = "";

        }
    }
}
