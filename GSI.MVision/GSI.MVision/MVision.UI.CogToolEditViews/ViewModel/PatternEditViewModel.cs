using Cognex.VisionPro.Blob;
using Cognex.VisionPro.PMAlign;
using Cognex.VisionPro;
using MVision.Common.Event;
using MVision.Common.Model;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using Prism.Mvvm;
using MVision.Manager.Vision;
using MVision.VisionLib.Cognex;
using MVision.Formulas.Common;
using DOTNET.Logging;
using DOTNET.WPF.Interfaces;
using MVision.Common.Interface;
using MVision.Common.Shared;
using System.Collections.ObjectModel;
using MVision.UI.CogDisplayViews.View;
using MVision.Common.Template;
using MVision.MairaDB;
using MaterialDesignThemes.Wpf;
using MVision.UI.InteractivityViews.View;
using MVision.Common.Event.EventArg;

namespace MVision.UI.CogToolEditViews.ViewModel
{
    public class PatternEditViewModel : BindableBase
    {
        #region Properties

        CogDisplayView cogDisplay = null;
        public CogDisplayView CogDisplay { get => this.cogDisplay; set => SetProperty(ref this.cogDisplay, value); }

        CogDisplayView cogPatternDisplay = null;
        public CogDisplayView CogPatternDisplay { get => this.cogPatternDisplay; set => SetProperty(ref this.cogPatternDisplay, value); }

        CogDisplayView cogPatternMaskDisplay = null;
        public CogDisplayView CogPatternMaskDisplay { get => this.cogPatternMaskDisplay; set => SetProperty(ref this.cogPatternMaskDisplay, value); }


        private bool isSaving;
        public bool IsSaving { get => this.isSaving; set => SetProperty(ref this.isSaving, value); }

        public ProcessPosition processPos { get; set; }

        public eMultiPattern PatternID { get; set; }
        public eBlobNum BlobID { get; set; }

        public ICogImage inputImage { get; set; }

        IContainerProvider provider = null;
        LibraryManager libraryManager = null;
        MariaManager sql = null;
        CognexVisionPro cognexVisionPro = null;
        IEventAggregator _eventAggregator = null;
        GUIMessageEvent _gUIMessageEvent = null;
        AlignFormulas algorithmProcesser = null;

        CameraManager cameraManager = null;
        ICamera camera = null;

        CogPMAlignTool cogPMAlignTool = null;
        CogBlobTool cogBlobTool = null;

        CogRectangleAffine cogSearchRegion = null;
        CogRectangleAffine cogTrainRegion = null;
        CogCoordinateAxes cogOrigin = null;

        Logger dbLogger = Logger.GetLogger("DB");
        Logger logger = Logger.GetLogger();
        #endregion

        #region Command

        public ICommand SaveDialogCommand { get; set; }

        public ICommand CloseDialogCommand { get; set; }

        public ICommand LoadPatternCommand { get; set; }

        public ICommand SavePatternCommand { get; set; }

        public ICommand PatternTrainCommand { get; set; }

        public ICommand GrabTrainImageCommand { get; set; }

        public ICommand FitInImageCommand { get; set; }

        public ICommand CenterOriginCommand { get; set; }

        public ICommand RunPMAlignToolCommand { get; set; }

        public ICommand GrabImageCommand { get; set; }

        public ICommand LiveImageCommand { get; set; }

        public ICommand SaveImageCommand { get; set; }

        public ICommand LoadImageCommand { get; set; }

        public ICommand RunPatternMaskCommand { get; set; }

        public ICommand SavePatternMaskCommand { get; set; }

        public ICommand DeletePatternMaskCommand { get; set; }

        public ICommand ResfPosSaveCommand { get; set; }


        #endregion

        #region Construct
        public PatternEditViewModel(IContainerProvider prov, LibraryManager libraryManager, MariaManager sql, CameraManager cameraManager, CognexVisionPro cognexVisionPro, IEventAggregator eventAggregator, AlignFormulas algorithmProcesser)
        {
            this.provider = prov;
            this.libraryManager = libraryManager;
            this.sql = sql;
            this.cameraManager = cameraManager;
            this.cognexVisionPro = cognexVisionPro;
            this.algorithmProcesser = algorithmProcesser;

            this.CogDisplay = provider.Resolve<CogDisplayView>();
            this.CogPatternDisplay = provider.Resolve<CogDisplayView>();
            this.CogPatternMaskDisplay = provider.Resolve<CogDisplayView>();

            ////this.SettingLigthView_1 = provider.Resolve<SettingLightView>();
            ////SettingLigthView_1.ViewModel.PortId = 2; ;
            ////SettingLigthView_1.ViewModel.Channel = 1;

            ////this.SettingLigthView_2 = provider.Resolve<SettingLightView>();
            ////SettingLigthView_2.ViewModel.PortId = 2; ;
            ////SettingLigthView_2.ViewModel.Channel = 2;

            ////this.SettingLigthView_3 = provider.Resolve<SettingLightView>();
            ////SettingLigthView_3.ViewModel.PortId = 2; ;
            ////SettingLigthView_3.ViewModel.Channel = 3;

            ////this.SettingLigthView_4 = provider.Resolve<SettingLightView>();
            ////SettingLigthView_4.ViewModel.PortId = 2; ;
            ////SettingLigthView_4.ViewModel.Channel = 4;

            ////this._eventAggregator = eventAggregator;
            ////this._eventAggregator.GetEvent<SchedulerMessageEvent>().Unsubscribe(UICallbackCommunication);
            ////this._eventAggregator.GetEvent<SchedulerMessageEvent>().Subscribe(UICallbackCommunication, ThreadOption.UIThread);
            ////this._gUIMessageEvent = this._eventAggregator.GetEvent<GUIMessageEvent>();

            InitializeCommand();
        }
        public void Init()
        {
            cogPMAlignTool = new CogPMAlignTool(libraryManager.CogJobs[processPos.ExecuteZoneID][processPos.Quadrant].PMAlignDic[PatternID]);
            cogBlobTool = new CogBlobTool(libraryManager.CogJobs[processPos.ExecuteZoneID][processPos.Quadrant].BlobDic[BlobID]);

            camera = cameraManager.Cameras.Values.FirstOrDefault(_ => _.CameraID == processPos.CameraID);

            this.cogSearchRegion = (CogRectangleAffine)cogPMAlignTool.SearchRegion;
            this.cogTrainRegion = (CogRectangleAffine)cogPMAlignTool.Pattern.TrainRegion;

            //ImageSourceList = new List<string>();
            //ImageSourceList.Add("InputImage");
            //ImageSourceList.Add("TrainImage");
            //ImageSourceList.Add("OutputImage");

            //ImageSourceSelectedItem = "InputImage";

            //GetSearchRegionOriginData();
            //GetTrainRegionOriginData();
            //GetTrainParams();
            //GetRunParmas();
            //GetPatternImage();
            //GetMaskParams();


            this.CogDisplay.FixAirspace = false;
            this.CogPatternDisplay.FixAirspace = false;
            this.CogPatternMaskDisplay.FixAirspace = false;
        }

        private void InitializeCommand()
        {
            //this.SaveDialogCommand = new DelegateCommand(ExcuteSaveDialogCommand);
            //this.RunPMAlignToolCommand = new DelegateCommand(ExcuteRunPMAlignToolCommand);

            //GrabImageCommand = new DelegateCommand(ExcuteGrabImageCommand);
            //LiveImageCommand = new DelegateCommand(ExcuteLiveImageCommand);
            //SaveImageCommand = new DelegateCommand(ExecuteImageSaveCommand);
            //LoadImageCommand = new DelegateCommand(ExecuteImageLoadCommand);

            //LoadPatternCommand = new DelegateCommand(ExcuteLoadPatternCommand);
            //SavePatternCommand = new DelegateCommand(ExcuteSavePatternCommand);
            //PatternTrainCommand = new DelegateCommand(ExcutePatternTrainCommand);
            //GrabTrainImageCommand = new DelegateCommand(ExcuteGrabTrainImageCommand);

            //FitInImageCommand = new DelegateCommand(ExcuteFitInImageCommand);
            //CenterOriginCommand = new DelegateCommand(ExcuteCenterOriginCommand);

            //RunPatternMaskCommand = new DelegateCommand(ExcuteRunPatternMaskCommand);
            //SavePatternMaskCommand = new DelegateCommand(ExcuteSaveMaskCommand);
            //DeletePatternMaskCommand = new DelegateCommand(ExcuteDeletePatternMaskCommand);

            //ResfPosSaveCommand = new DelegateCommand(ExcuteResfPosSaveCommandCommand);
            //CreateSegmentToolCommand = new DelegateCommand(ExcuteCreateSegmentToolCommand);
            //DistanceMeasureCommand = new DelegateCommand(ExcuteDistanceMeasureCommand);

        }
        #endregion


        #region Command Method

        #endregion
    }
}
