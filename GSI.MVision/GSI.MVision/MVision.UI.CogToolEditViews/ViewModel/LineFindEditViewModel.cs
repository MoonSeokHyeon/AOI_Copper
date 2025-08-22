using Cognex.VisionPro;
using Cognex.VisionPro.Caliper;
using DOTNET.Logging;
using DOTNET.Utils;
using DOTNET.WPF.Interfaces;
using MaterialDesignThemes.Wpf;
using MVision.Common.Event;
using MVision.Common.Event.EventArg;
using MVision.Common.Interface;
using MVision.Common.Model;
using MVision.Common.Shared;
using MVision.Common.Template;
using MVision.Device.Camera;
using MVision.Formulas.Common;
using MVision.MairaDB;
using MVision.Manager.Vision;
using MVision.UI.CogDisplayViews.View;
using MVision.UI.InteractivityViews.View;
using MVision.VisionLib.Cognex;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace MVision.UI.CogToolEditViews.ViewModel
{
    public class LineFindEditViewModel : BindableBase
    {
        #region Properties

        CogDisplayView cogDisplay = null;
        public CogDisplayView CogDisplay { get => this.cogDisplay; set => SetProperty(ref this.cogDisplay, value); }

        private List<string> imageSourceList;
        public List<string> ImageSourceList
        {
            get { return imageSourceList; }
            set { SetProperty(ref this.imageSourceList, value); }
        }

        private string imageSourceSelectedItem;
        public string ImageSourceSelectedItem
        {
            get { return imageSourceSelectedItem; }
            set
            {
                if (SetProperty(ref this.imageSourceSelectedItem, value))
                {
                    ChangeImageSource(imageSourceSelectedItem);
                }
            }
        }

        private ObservableCollection<ListBoxItem> caliperParams;
        public ObservableCollection<ListBoxItem> CaliperParams
        {
            get { return caliperParams; }
            set { SetProperty(ref this.caliperParams, value); }
        }

        private ObservableCollection<ListBoxItem> lineSegmentParams;
        public ObservableCollection<ListBoxItem> LineSegmentParams
        {
            get { return lineSegmentParams; }
            set { SetProperty(ref this.lineSegmentParams, value); }
        }

        private ObservableCollection<ListBoxItem> fitterParams;
        public ObservableCollection<ListBoxItem> FitterParams
        {
            get { return fitterParams; }
            set { SetProperty(ref this.fitterParams, value); }
        }

        private List<CogCaliperPolarityConstants> edgePolarityParams;
        public List<CogCaliperPolarityConstants> EdgePolarityParams
        {
            get { return edgePolarityParams; }
            set { SetProperty(ref this.edgePolarityParams, value); }
        }

        private CogCaliperPolarityConstants edgePolaritySelectedItem;
        public CogCaliperPolarityConstants EdgePolaritySelectedItem
        {
            get { return edgePolaritySelectedItem; }
            set
            {
                if (SetProperty(ref this.edgePolaritySelectedItem, value))
                {
                    SetEdgePolarity(edgePolaritySelectedItem);
                }
            }
        }

        private ObservableCollection<ListBoxItem> edgeParams;
        public ObservableCollection<ListBoxItem> EdgeParams
        {
            get { return edgeParams; }
            set { SetProperty(ref this.edgeParams, value); }
        }

        private List<string> findLineToolList;
        public List<string> FindLineToolList
        {
            get { return findLineToolList; }
            set { SetProperty(ref this.findLineToolList, value); }
        }

        private string selectedTool;
        public string SelectedTool
        {
            get { return this.selectedTool; }
            set
            {
                if (SetProperty(ref this.selectedTool, value))
                {
                    if (selectedTool == null) return;

                    SelectedToolChange(selectedTool);
                }
            }
        }

        bool isInit = false;

        public IWindowView View { get; set; }
        public ProcessPosition processPos { get; set; }
        public eKindFindLine kindFindLine { get; set; }
        public ICogImage inputImage { get; set; }

        public bool isUseFixture { get; set; }

        IContainerProvider provider = null;
        LibraryManager libraryManager = null;
        AlignFormulas alignFormulas = null;
        MariaManager sql = null;
        CameraManager cameraManager = null;

        CognexVisionPro cognexVisionPro = null;
        CogFindLineTool cogFindLineTool = null;
        IEventAggregator _eventAggregator = null;
        GUIMessageEvent _gUIMessageEvent = null;

        ICamera camera = null;

        Logger dbLogger = Logger.GetLogger("DB");
        Logger logger = Logger.GetLogger();
        #endregion

        #region Command
        public ICommand GrabImageCommand { get; set; }
        public ICommand LiveImageCommand { get; set; }
        public ICommand SaveImageCommand { get; set; }
        public ICommand LoadImageCommand { get; set; }
        public ICommand SwapSearchDirectionCommand { get; set; }
        public ICommand RunLineFindToolCommand { get; set; }
        public ICommand ElectricModeCommand { get; set; }
        public ICommand GraphicsSetCommand { get; set; }
        public ICommand ResfPosSaveCommand { get; set; }
        public ICommand SaveDialogCommand { get; set; }
        public ICommand CloseDialogCommand { get; set; }

        #endregion

        #region Construct

        public LineFindEditViewModel(IContainerProvider prov, LibraryManager libraryManager, MariaManager sql, CameraManager cameraManager, CognexVisionPro cognexVisionPro, IEventAggregator eventAggregator, AlignFormulas algorithmProcesser)
        {
            this.provider = prov;
            this.libraryManager = libraryManager;
            this.sql = sql;
            this.cameraManager = cameraManager;
            this.cognexVisionPro = cognexVisionPro;
            this.alignFormulas = algorithmProcesser;

            this.CogDisplay = provider.Resolve<CogDisplayView>();

            this._eventAggregator = eventAggregator;
            this._eventAggregator.GetEvent<SchedulerMessageEvent>().Unsubscribe(UICallbackCommunication);
            this._eventAggregator.GetEvent<SchedulerMessageEvent>().Subscribe(UICallbackCommunication, ThreadOption.UIThread);

            this.cognexVisionPro = new CognexVisionPro();

            this._gUIMessageEvent = this._eventAggregator.GetEvent<GUIMessageEvent>();

            InitializeCommand();
        }

        public void Init()
        {
            if (!isInit) isInit = true;
            else return;

            this.cogFindLineTool = new CogFindLineTool(libraryManager.CogJobs[processPos.ExecuteZoneID][processPos.Quadrant].FindLineDic[kindFindLine]);
            camera = cameraManager.Cameras.Values.FirstOrDefault(_ => _.CameraID == processPos.CameraID);

            if(ImageSourceList == null)
            {
                ImageSourceList = new List<string>();
                ImageSourceList.Add("InputImage");
                ImageSourceList.Add("OutputImage");

                ImageSourceSelectedItem = "InputImage";
            }

            if(FindLineToolList == null)
            {
                FindLineToolList = new List<string>();
                FindLineToolList.Add("Vertical");
                FindLineToolList.Add("Horizon");

                SelectedTool = "Vertical";
            }    

            GetCaliperSetting();
            GetEdgeModeSetting();
            GetFitterSetting();

            //if (inputImage != null) SetLineGraphic();

            this.CogDisplay.FixAirspace = false;
        }

        private void InitializeCommand()
        {
            GrabImageCommand = new DelegateCommand(ExcuteGrabImageCommand);
            LiveImageCommand = new DelegateCommand(ExcuteLiveImageCommand);
            SaveImageCommand = new DelegateCommand(ExecuteImageSaveCommand);
            LoadImageCommand = new DelegateCommand(ExecuteImageLoadCommand);

            this.SwapSearchDirectionCommand = new DelegateCommand(ExcuteSwapSearchDirectionCommand);
            this.RunLineFindToolCommand = new DelegateCommand(ExcuteRunLineFindToolCommand);
            this.ElectricModeCommand = new DelegateCommand(ExcuteElectricModeCommand);
            this.GraphicsSetCommand = new DelegateCommand(ExcuteGraphicsSetCommand);
            this.ResfPosSaveCommand = new DelegateCommand(ExcuteResfPosSaveCommand);

            this.SaveDialogCommand = new DelegateCommand(ExcuteSaveDialogCommand);
        }

        private void UICallbackCommunication(SchedulerEventArgs obj)
        {
            if (this.processPos != obj.processPosition) return;

            switch (obj.MessageKind)
            {
                case eSchedulerMessageKind.FindRunProcesser:
                    switch (obj.SubKind)
                    {
                        case eFindPos.CaliperEdit:
                            CallBackFindLine(obj);
                            break;

                        default:
                            break;
                    }
                    break;

                default:
                    break;
            }
        }
        private void CallBackFindLine(SchedulerEventArgs obj)
        {
            var result = CastTo<FindResult>.From(obj.Arg);
            var graphic = (CogGraphicInteractiveCollection)result.ResultGraphic;

            GetFindPatternGraphic(graphic);
        }

        private void GetFindPatternGraphic(CogGraphicInteractiveCollection graphic)
        {
            if (graphic == null) return;

            if (ImageSourceSelectedItem == "OutputImage")
            {
                CogDisplay.ClearGraphics();
                CogDisplay.SetGraphic(graphic, "LineResult", true);
            }
        }

        private void ExcuteRunLineFindToolCommand()
        {
            eKindFindLine kindLine = eKindFindLine.None;

            if (SelectedTool == "Vertical")
                kindLine = eKindFindLine.VerLine;

            if (SelectedTool == "Horizon")
                kindLine = eKindFindLine.HorLine;

            var findResult = alignFormulas.FindRun(this.processPos, eFindPos.CaliperEdit, kindLine, this.inputImage);

            if (this.ImageSourceSelectedItem == "OutputImage") SetResult(findResult);
        }

        private async void ExcuteSaveDialogCommand()
        {
            this.CogDisplay.SetFixAirspace(true);

            var view = this.provider.Resolve<ComfirmationView>();
            view.ViewModel.Message = "Do you want to save ?";

            //if (!DialogHost.IsDialogOpen("CaliperDialog"))
            //{
            //    var result = await DialogHost.Show(view, "CaliperDialog") as bool?;
            //    if (result == true)
            //    {
            //        if (cogFindLineTool == null) return;

            //        libraryManager.CogJobs[processPos.ExecuteZoneID][processPos.Quadrant].FindLineDic[kindFindLine] = cogFindLineTool;
            //        libraryManager.CogJobs[processPos.ExecuteZoneID][processPos.Quadrant].SaveTools();
            //    }
            //}

            libraryManager.CogJobs[processPos.ExecuteZoneID][processPos.Quadrant].FindLineDic[kindFindLine] = cogFindLineTool;
            libraryManager.CogJobs[processPos.ExecuteZoneID][processPos.Quadrant].SaveTools();

            this.CogDisplay.SetFixAirspace(false);
        }

        #endregion

        #region Command Method
        private void ExcuteGrabImageCommand()
        {
            if (this.CogDisplay.IsLiveDisplay())
            {
                this.CogDisplay.StopLiveDisplay();
            }

            this.CogDisplay.ClearImage();
            this.CogDisplay.ClearGraphics();

            if (this.camera.IsGrabbing)
            {
                this.camera.StopGrabContinuous();
                DOTNET.Concurrent.LockUtils.Wait(500);
            }
            var image = (ICogImage)this.camera.GrabOneShot();
            this.inputImage = image;

            this.cogDisplay.SetImage(image);
            //this.CogDisplay.SetCenterRectangleGrid();
        }
        private void ExcuteLiveImageCommand()
        {
            if (this.CogDisplay.IsLiveDisplay())
            {
                this.CogDisplay.StopLiveDisplay();
                return;
            }

            var cogCamera = this.camera as CognexCamera;

            this.CogDisplay.StartLiveDisplay(cogCamera.Fifo);
            this.CogDisplay.SetCenterRectangleGrid();
        }
        private void ExecuteImageLoadCommand()
        {
            var filePath = string.Empty;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = @"D:\LOG\VAS\FPCB\ImageLog";
            openFileDialog.Filter = "bmp files (*.bmp)|*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //Get the path of specified file
                filePath = openFileDialog.FileName;
            }
            var loadImage = cognexVisionPro.LoadImage(filePath);
            inputImage = loadImage;

            if (inputImage != null)
                this.CogDisplay.SetImage(loadImage);
        }
        private void ExecuteImageSaveCommand()
        {
            if (inputImage == null) return;

            var path = ConfigurationManager.AppSettings["SAVE_PATH"];

            libraryManager.CogJobs[processPos.ExecuteZoneID][processPos.Quadrant].SaveImage(inputImage, path);
        }
        private async void ExcuteResfPosSaveCommand()
        {
            //var modelData = this.sql.SystemInfo.GetAll().FirstOrDefault().CurrentModel;
            //var qaudData = modelData.QuadrantDatas.FirstOrDefault(x => x.processPosition.ExecuteZoneID.Equals(this.processPos.ExecuteZoneID) && x.processPosition.Quadrant.Equals(this.processPos.Quadrant));

            //var posResult = alignFormulas.FindRun(this.processPos, eFindPos.CornerEdit, null, this.inputImage);

            //if (!posResult.isSuccess) return;

            //var view = this.provider.Resolve<ComfirmationView>();
            //view.ViewModel.Message = "Do you want to Ref Pos save ?";

            //var result = await DialogHost.Show(view, "RootDialog") as bool?;
            //if (result == true)
            //{
            //    qaudData.refPosXY = posResult.ResultXY;

            //    sql.ModelData.Edit(modelData);
            //    dbLogger.I($"{processPos.ExecuteZoneID} - {processPos.Quadrant} Ref Pos Change");
            //}

        }
        #endregion

        #region CogDisplay

        private void ChangeImageSource(string imageSourceSelectedItem)
        {
            if (inputImage == null) return;

            if (imageSourceSelectedItem == "InputImage")
            {
                CogDisplay.SetImage(inputImage);
                CogDisplay.ClearGraphics();
            }
            else if (imageSourceSelectedItem == "OutputImage")
            {
                ExcuteRunLineFindToolCommand();
            }
            else
            {

            }
        }

        private void SetResult(FindResult result)
        {
            this.CogDisplay.ClearGraphics();

            if (result == null) return;

            if (result.InputImage == null) return;

            this.CogDisplay.ClearImage();

            this.CogDisplay.SetImage((ICogImage)result.InputImage);
            this.cogDisplay.SetGraphic((CogGraphicInteractiveCollection)result.ResultGraphic, "ManualResult", true);
        }

        private void SetLineGraphic()
        {
            var lineGraphics = cognexVisionPro.CreateLineGraphic(inputImage, cogFindLineTool);

            CogDisplay.SetGraphic(lineGraphics, "FindLine", true);
        }

        private void ClearGraphic()
        {
            CogDisplay.ClearGraphics();
        }

        #endregion

        #region Setting

        private void GetCaliperSetting()
        {
            var caliper = cogFindLineTool.RunParams;

            CaliperParams = new System.Collections.ObjectModel.ObservableCollection<ListBoxItem>();

            CaliperParams.Add(new ListBoxItem() { Name = "CaliperNumber", Value = caliper.NumCalipers.ToString() });
            CaliperParams.Add(new ListBoxItem() { Name = "SearchLength", Value = caliper.CaliperSearchLength.ToString() });
            CaliperParams.Add(new ListBoxItem() { Name = "ProjectionLength", Value = caliper.CaliperProjectionLength.ToString() });
            CaliperParams.Add(new ListBoxItem() { Name = "SearchDirection", Value = (caliper.CaliperSearchDirection * 180 / Math.PI).ToString() });

            LineSegmentParams = new System.Collections.ObjectModel.ObservableCollection<ListBoxItem>();

            LineSegmentParams.Add(new ListBoxItem() { Name = "StartX", Value = caliper.ExpectedLineSegment.StartX.ToString() });
            LineSegmentParams.Add(new ListBoxItem() { Name = "StartY", Value = caliper.ExpectedLineSegment.StartY.ToString() });
            LineSegmentParams.Add(new ListBoxItem() { Name = "EndX", Value = caliper.ExpectedLineSegment.EndX.ToString() });
            LineSegmentParams.Add(new ListBoxItem() { Name = "EndY", Value = caliper.ExpectedLineSegment.EndY.ToString() });

            CaliperParams.ToList().ForEach(item => { item.OnValueChanged += SetCaliperDataChanged; });
            LineSegmentParams.ToList().ForEach(item => { item.OnValueChanged += SetSegmentationDataChanged; });
        }

        private void GetFitterSetting()
        {
            var fitter = cogFindLineTool.RunParams;

            FitterParams = new System.Collections.ObjectModel.ObservableCollection<ListBoxItem>();
            FitterParams.Add(new ListBoxItem() { Name = "NumToIgonore", Value = fitter.NumToIgnore.ToString() });

            FitterParams.ToList().ForEach(item => { item.OnValueChanged += SetFitterDataChanged; });
        }

        private void GetEdgeModeSetting()
        {
            var edge = cogFindLineTool.RunParams;

            if (EdgePolarityParams == null)
            {
                EdgePolarityParams = new List<CogCaliperPolarityConstants>();
                EdgePolarityParams.Add(CogCaliperPolarityConstants.LightToDark);
                EdgePolarityParams.Add(CogCaliperPolarityConstants.DarkToLight);
                EdgePolarityParams.Add(CogCaliperPolarityConstants.DontCare);
            }

            EdgePolaritySelectedItem = EdgePolarityParams.FirstOrDefault(x => x.Equals(edge.CaliperRunParams.Edge0Polarity));

            EdgeParams = new System.Collections.ObjectModel.ObservableCollection<ListBoxItem>();

            EdgeParams.Add(new ListBoxItem() { Name = "ContrastThreshold", Value = edge.CaliperRunParams.ContrastThreshold.ToString() });
            EdgeParams.Add(new ListBoxItem() { Name = "FilterHalfSizeInPixels", Value = edge.CaliperRunParams.FilterHalfSizeInPixels.ToString() });
            //TODO : ADD Anothers ex) Caliper Scoreing -> Contrast, Position, PositionNeg, Calipers Graphs

            EdgeParams.ToList().ForEach(item => { item.OnValueChanged += SetEdgeDataChanged; });
        }

        private void ExcuteSwapSearchDirectionCommand()
        {
            var startX = this.cogFindLineTool.RunParams.ExpectedLineSegment.StartX;
            var startY = this.cogFindLineTool.RunParams.ExpectedLineSegment.StartY;
            var endX = this.cogFindLineTool.RunParams.ExpectedLineSegment.EndX;
            var endY = this.cogFindLineTool.RunParams.ExpectedLineSegment.EndY;

            this.cogFindLineTool.RunParams.ExpectedLineSegment.StartX = endX;
            this.cogFindLineTool.RunParams.ExpectedLineSegment.StartY = endY;
            this.cogFindLineTool.RunParams.ExpectedLineSegment.EndX = startX;
            this.cogFindLineTool.RunParams.ExpectedLineSegment.EndY = startY;

            //TODO : Image에서 벗어나있는 경우 Exception

            GetCaliperSetting();
            ClearGraphic();
            SetLineGraphic();
        }

        #endregion

        #region Caliper Settings

        private void SetEdgePolarity(CogCaliperPolarityConstants mode)
        {
            var edge0Polarity = cogFindLineTool.RunParams;
            edge0Polarity.CaliperRunParams.Edge0Polarity = mode;

            cogFindLineTool.RunParams = edge0Polarity;
        }

        private void SetCaliperDataChanged(object sender, EventArgs e)
        {
            var caliper = cogFindLineTool.RunParams;

            //TODO : Double 검사 필요

            caliper.NumCalipers = CaliperParams.FirstOrDefault(t => t.Name.Equals("CaliperNumber")).ToInt();
            caliper.CaliperSearchLength = CaliperParams.FirstOrDefault(t => t.Name.Equals("SearchLength")).ToDouble();
            caliper.CaliperProjectionLength = CaliperParams.FirstOrDefault(t => t.Name.Equals("ProjectionLength")).ToDouble();
            caliper.CaliperSearchDirection = CaliperParams.FirstOrDefault(t => t.Name.Equals("SearchDirection")).ToDouble() * Math.PI / 180;

            cogFindLineTool.RunParams = caliper;
            ClearGraphic();
            SetLineGraphic();
        }

        private void SetSegmentationDataChanged(object sender, EventArgs e)
        {
            var segmentData = cogFindLineTool.RunParams;

            //TODO : Double 검사 필요

            segmentData.ExpectedLineSegment.StartX = LineSegmentParams.FirstOrDefault(t => t.Name.Equals("StartX")).ToDouble();
            segmentData.ExpectedLineSegment.StartY = LineSegmentParams.FirstOrDefault(t => t.Name.Equals("StartY")).ToDouble();
            segmentData.ExpectedLineSegment.EndX = LineSegmentParams.FirstOrDefault(t => t.Name.Equals("EndX")).ToDouble();
            segmentData.ExpectedLineSegment.EndY = LineSegmentParams.FirstOrDefault(t => t.Name.Equals("EndY")).ToDouble();

            cogFindLineTool.RunParams = segmentData;
            ClearGraphic();
            SetLineGraphic();
        }

        private void SetFitterDataChanged(object sender, EventArgs e)
        {
            var fitterData = cogFindLineTool.RunParams;

            fitterData.NumToIgnore = int.Parse(FitterParams.FirstOrDefault(t => t.Name.Equals("NumToIgonore")).Value);

            cogFindLineTool.RunParams = fitterData;
        }

        private void SetEdgeDataChanged(object sender, EventArgs e)
        {
            var edgeData = cogFindLineTool.RunParams;

            edgeData.CaliperRunParams.ContrastThreshold = EdgeParams.FirstOrDefault(t => t.Name.Equals("ContrastThreshold")).ToDouble();
            edgeData.CaliperRunParams.FilterHalfSizeInPixels = EdgeParams.FirstOrDefault(t => t.Name.Equals("FilterHalfSizeInPixels")).ToInt();

            cogFindLineTool.RunParams = edgeData;
        }

        #endregion

        #region Selected Change
        private void SelectedToolChange(string toolName)
        {
            switch (toolName)
            {
                case "Vertical":
                    processPos = processPos;
                    kindFindLine = eKindFindLine.VerLine;
                    break;
                case "Horizon":
                    processPos = processPos;
                    kindFindLine = eKindFindLine.HorLine;
                    break;
                default:
                    break;
            }

            cogFindLineTool = new CogFindLineTool(libraryManager.CogJobs[processPos.ExecuteZoneID][processPos.Quadrant].FindLineDic[kindFindLine]);

            GetCaliperSetting();
            GetEdgeModeSetting();
            GetFitterSetting();
            ClearGraphic();

            if (inputImage != null) SetLineGraphic();

            this.CogDisplay.FixAirspace = false;
        }
        #endregion

        #region Results

        private void ExcuteElectricModeCommand()
        {
        }
        private void ExcuteGraphicsSetCommand()
        {
            CogDisplay.ClearGraphics();

            if (inputImage != null) SetLineGraphic();
        }
        #endregion

        #region Public Method

        public void Query()
        {
        }
        public void Clear()
        {
        }
        public void Subscribe()
        {
        }

        public void Unsubscribe()
        {
        }
        #endregion
    }
}
