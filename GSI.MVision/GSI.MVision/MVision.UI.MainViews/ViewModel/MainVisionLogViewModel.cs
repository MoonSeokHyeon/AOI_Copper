using MVision.UI.LogViews.View;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.UI.MainViews.ViewModel
{
    public class MainVisionLogViewModel : BindableBase
    {
        ViewLogView historyLogView = null;
        public ViewLogView HistoryLogView { get => this.historyLogView; set => SetProperty(ref this.historyLogView, value); }

        AlignLogView alignLogView = null;
        public AlignLogView AlignLogView { get => this.alignLogView; set => SetProperty(ref this.alignLogView, value); }

        IContainerProvider provider = null;
        public MainVisionLogViewModel(IContainerProvider prov)
        {
            provider = prov;

            this.HistoryLogView = provider.Resolve<ViewLogView>();
            this.AlignLogView = provider.Resolve<AlignLogView>();
        }
        public void Init()
        {
        }
    }
}
