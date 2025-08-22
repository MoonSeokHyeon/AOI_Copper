using MaterialDesignThemes.Wpf;
using MVision.Common.Model;
using MVision.Common.Shared;
using MVision.UI.CogToolEditViews.View;
using MVision.UI.InterfaceViews.View;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Interop;

namespace MVision.UI.MainViews.ViewModel
{
    public class MainEditViewModel : BindableBase
    {
        #region Properties
        //PatternEditView pnlEditView1 = null;
        //public PatternEditView PNLEditView1 { get => this.pnlEditView1; set => SetProperty(ref this.pnlEditView1, value); }

        //PatternEditView pnlEditView2 = null;
        //public PatternEditView PNLEditView2 { get => this.pnlEditView2; set => SetProperty(ref this.pnlEditView2, value); }

        //PatternEditView pnlEditView3 = null;
        //public PatternEditView PNLEditView3 { get => this.pnlEditView3; set => SetProperty(ref this.pnlEditView3, value); }

        //PatternEditView pnlEditView4 = null;
        //public PatternEditView PNLEditView4 { get => this.pnlEditView4; set => SetProperty(ref this.pnlEditView4, value); }

        LineFindEditView pnlEditView1 = null;
        public LineFindEditView PNLEditView1 { get => this.pnlEditView1; set => SetProperty(ref this.pnlEditView1, value); }

        LineFindEditView pnlEditView2 = null;
        public LineFindEditView PNLEditView2 { get => this.pnlEditView2; set => SetProperty(ref this.pnlEditView2, value); }

        LineFindEditView pnlEditView3 = null;
        public LineFindEditView PNLEditView3 { get => this.pnlEditView3; set => SetProperty(ref this.pnlEditView3, value); }

        LineFindEditView pnlEditView4 = null;
        public LineFindEditView PNLEditView4 { get => this.pnlEditView4; set => SetProperty(ref this.pnlEditView4, value); }


        PatternEditView windowEditView1 = null;
        public PatternEditView WindowEditView1 { get => this.windowEditView1; set => SetProperty(ref this.windowEditView1, value); }

        PatternEditView windowEditView2 = null;
        public PatternEditView WindowEditView2 { get => this.windowEditView2; set => SetProperty(ref this.windowEditView2, value); }

        PatternEditView windowEditView3 = null;
        public PatternEditView WindowEditView3 { get => this.windowEditView3; set => SetProperty(ref this.windowEditView3, value); }

        PatternEditView windowEditView4 = null;
        public PatternEditView WindowEditView4 { get => this.windowEditView4; set => SetProperty(ref this.windowEditView4, value); }

        PatternEditView filmEditView1 = null;
        public PatternEditView FilmEditView1 { get => this.filmEditView1; set => SetProperty(ref this.filmEditView1, value); }

        PatternEditView filmEditView2 = null;
        public PatternEditView FilmEditView2 { get => this.filmEditView2; set => SetProperty(ref this.filmEditView2, value); }

        PatternEditView filmEditView3 = null;
        public PatternEditView FilmEditView3 { get => this.filmEditView3; set => SetProperty(ref this.filmEditView3, value); }

        PatternEditView filmEditView4 = null;
        public PatternEditView FilmEditView4 { get => this.filmEditView4; set => SetProperty(ref this.filmEditView4, value); }


        IContainerProvider provider = null;
        IEventAggregator eventAggregator = null;

        #endregion

        #region Commands

        public ICommand AlignPattern1ClickCommand { get; set; }
        public ICommand AlignPattern2ClickCommand { get; set; }

        #endregion

        #region Construct

        public MainEditViewModel(IEventAggregator eventAggregator, IContainerProvider prov)
        {
            this.eventAggregator = eventAggregator;
            this.provider = prov;

            //pnl
            this.PNLEditView1 = provider.Resolve<LineFindEditView>();
            PNLEditView1.ViewModel.processPos = new ProcessPosition(eCameraID.Camera2, eExecuteZoneID.Main1_OBJECT, eQuadrant.FirstQuadrant);
            PNLEditView1.ViewModel.kindFindLine = eKindFindLine.VerLine;

            this.PNLEditView2 = provider.Resolve<LineFindEditView>();
            PNLEditView2.ViewModel.processPos = new ProcessPosition(eCameraID.Camera1, eExecuteZoneID.Main1_OBJECT, eQuadrant.SecondQuadrant);
            PNLEditView2.ViewModel.kindFindLine = eKindFindLine.VerLine;

            this.PNLEditView3 = provider.Resolve<LineFindEditView>();
            PNLEditView3.ViewModel.processPos = new ProcessPosition(eCameraID.Camera1, eExecuteZoneID.Main1_OBJECT, eQuadrant.ThirdQuadrant);
            PNLEditView3.ViewModel.kindFindLine = eKindFindLine.VerLine;

            this.PNLEditView4 = provider.Resolve<LineFindEditView>();
            PNLEditView4.ViewModel.processPos = new ProcessPosition(eCameraID.Camera2, eExecuteZoneID.Main1_OBJECT, eQuadrant.FourthQuadrant);
            PNLEditView4.ViewModel.kindFindLine = eKindFindLine.VerLine;

            //this.PNLEditView1 = provider.Resolve<PatternEditView>();
            //PNLEditView1.ViewModel.processPos = new ProcessPosition(eCameraID.Camera2, eExecuteZoneID.Main1_TARGET, eQuadrant.FirstQuadrant);
            //PNLEditView1.ViewModel.PatternID = eMultiPattern.PM1;
            //PNLEditView1.ViewModel.BlobID = eBlobNum.Blob1;

            //this.PNLEditView2 = provider.Resolve<PatternEditView>();
            //PNLEditView2.ViewModel.processPos = new ProcessPosition(eCameraID.Camera1, eExecuteZoneID.Main1_TARGET, eQuadrant.FirstQuadrant);
            //PNLEditView2.ViewModel.PatternID = eMultiPattern.PM1;
            //PNLEditView2.ViewModel.BlobID = eBlobNum.Blob1;

            //this.PNLEditView3 = provider.Resolve<PatternEditView>();
            //PNLEditView3.ViewModel.processPos = new ProcessPosition(eCameraID.Camera3, eExecuteZoneID.Main1_TARGET, eQuadrant.ThirdQuadrant);
            //PNLEditView3.ViewModel.PatternID = eMultiPattern.PM1;
            //PNLEditView3.ViewModel.BlobID = eBlobNum.Blob1;

            //this.PNLEditView4 = provider.Resolve<PatternEditView>();
            //PNLEditView4.ViewModel.processPos = new ProcessPosition(eCameraID.Camera4, eExecuteZoneID.Main1_TARGET, eQuadrant.FourthQuadrant);
            //PNLEditView4.ViewModel.PatternID = eMultiPattern.PM1;
            //PNLEditView4.ViewModel.BlobID = eBlobNum.Blob1;

            //window
            //this.WindowEditView1 = provider.Resolve<PatternEditView>();
            //WindowEditView1.ViewModel.processPos = new ProcessPosition(eCameraID.Camera2, eExecuteZoneID.Main2_TARGET, eQuadrant.FirstQuadrant);
            //WindowEditView1.ViewModel.PatternID = eMultiPattern.PM1;
            //WindowEditView1.ViewModel.BlobID = eBlobNum.Blob1;

            //this.WindowEditView2 = provider.Resolve<PatternEditView>();
            //WindowEditView2.ViewModel.processPos = new ProcessPosition(eCameraID.Camera1, eExecuteZoneID.Main2_TARGET, eQuadrant.FirstQuadrant);
            //WindowEditView2.ViewModel.PatternID = eMultiPattern.PM1;
            //WindowEditView2.ViewModel.BlobID = eBlobNum.Blob1;

            //this.WindowEditView3 = provider.Resolve<PatternEditView>();
            //WindowEditView3.ViewModel.processPos = new ProcessPosition(eCameraID.Camera3, eExecuteZoneID.Main2_TARGET, eQuadrant.ThirdQuadrant);
            //WindowEditView3.ViewModel.PatternID = eMultiPattern.PM1;
            //WindowEditView3.ViewModel.BlobID = eBlobNum.Blob1;

            //this.WindowEditView4 = provider.Resolve<PatternEditView>();
            //WindowEditView4.ViewModel.processPos = new ProcessPosition(eCameraID.Camera4, eExecuteZoneID.Main2_TARGET, eQuadrant.FourthQuadrant);
            //WindowEditView4.ViewModel.PatternID = eMultiPattern.PM1;
            //WindowEditView4.ViewModel.BlobID = eBlobNum.Blob1;


            //film
            //this.FilmEditView1 = provider.Resolve<PatternEditView>();
            //FilmEditView1.ViewModel.processPos = new ProcessPosition(eCameraID.Camera2, eExecuteZoneID.Main2_TARGET, eQuadrant.FirstQuadrant);
            //FilmEditView1.ViewModel.PatternID = eMultiPattern.PM1;
            //FilmEditView1.ViewModel.BlobID = eBlobNum.Blob1;

            //this.FilmEditView2 = provider.Resolve<PatternEditView>();
            //FilmEditView1.ViewModel.processPos = new ProcessPosition(eCameraID.Camera1, eExecuteZoneID.Main2_TARGET, eQuadrant.FirstQuadrant);
            //FilmEditView2.ViewModel.PatternID = eMultiPattern.PM1;
            //FilmEditView2.ViewModel.BlobID = eBlobNum.Blob1;

            //this.FilmEditView3 = provider.Resolve<PatternEditView>();
            //FilmEditView1.ViewModel.processPos = new ProcessPosition(eCameraID.Camera3, eExecuteZoneID.Main2_TARGET, eQuadrant.ThirdQuadrant);
            //FilmEditView3.ViewModel.PatternID = eMultiPattern.PM1;
            //FilmEditView3.ViewModel.BlobID = eBlobNum.Blob1;

            //this.FilmEditView4 = provider.Resolve<PatternEditView>();
            //FilmEditView1.ViewModel.processPos = new ProcessPosition(eCameraID.Camera4, eExecuteZoneID.Main2_TARGET, eQuadrant.FourthQuadrant);
            //FilmEditView4.ViewModel.PatternID = eMultiPattern.PM1;
            //FilmEditView4.ViewModel.BlobID = eBlobNum.Blob1;
        }

        public void Init()
        {
            

        }


        #endregion

    
    }
}
