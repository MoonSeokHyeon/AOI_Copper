using Cognex.VisionPro;
using Cognex.VisionPro.Display;
using DOTNET.Logging;
using DOTNET.Utils;
using MVision.Common.Event;
using MVision.Common.Event.EventArg;
using MVision.Common.Interface;
using MVision.Common.Model;
using MVision.Common.Shared;
using MVision.Device.Camera;
using MVision.Formulas.Common;
using MVision.MairaDB;
using MVision.Manager.Vision;
using MVision.UI.CogDisplayViews.View;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace MVision.UI.CogDisplayViews.ViewModel
{
    public class SingleCamViewModel : BindableBase, IDisposable
    {
        #region Properties

        CogDisplayView cogDisplay = null;
        public CogDisplayView CogDisplay { get => this.cogDisplay; set => SetProperty(ref this.cogDisplay, value); }

        private ProcessPosition processPos = new ProcessPosition();
        public ProcessPosition ProcessPos
        {
            get { return processPos; }
            set { SetProperty(ref this.processPos, value); }
        }

        private bool _isLIve;

        public bool IsLive
        {
            get { return _isLIve; }
            set { SetProperty(ref this._isLIve, value); }
        }

        public event System.Windows.Forms.MouseEventHandler CogDisplayMouseMove;
        public event System.Windows.Forms.MouseEventHandler CogDisplayMouseUp;
        public event System.Windows.Forms.MouseEventHandler CogDisplayMouseDown;

        public event EventHandler CogDisplayClick;
        public event EventHandler CogDisplayDoubleClick;

        ICamera camera = null;

        eSystemState systemState = eSystemState.Manual;
        IContainerProvider provider = null;
        CameraManager cameraManager = null;
        //CognexVisionPro visionPro = null;

        IEventAggregator _eventAggregator = null;
        GUIMessageEvent _gUIMessageEvent = null;
        MariaManager mariaManager = null;

        #endregion


        #region ICommand

        public ICommand GrabCommand { get; set; }
        public ICommand LiveCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand FindCommand { get; set; }
        public ICommand CusorCommand { get; set; }
        public ICommand PanCommand { get; set; }
        public ICommand ZoomInCommand { get; set; }
        public ICommand ZoomOutCommand { get; set; }
        public ICommand FitCommand { get; set; }
        public ICommand RulerSquareCommand { get; set; }

        #endregion

        #region Construct

        public SingleCamViewModel(IEventAggregator eventAggregator, IContainerProvider prov, CameraManager cameraManager, MariaManager mariaManager)
        {
            this.provider = prov;
            this.cameraManager = cameraManager;
            this.mariaManager = mariaManager;

            this._eventAggregator = eventAggregator;
            this._eventAggregator.GetEvent<GUIMessageEvent>().Unsubscribe(OnReceivedMessage);
            this._eventAggregator.GetEvent<GUIMessageEvent>().Subscribe(OnReceivedMessage, ThreadOption.UIThread);

            this._eventAggregator.GetEvent<SchedulerMessageEvent>().Unsubscribe(UICallbackCommunication);
            this._eventAggregator.GetEvent<SchedulerMessageEvent>().Subscribe(UICallbackCommunication, ThreadOption.UIThread);

            this._gUIMessageEvent = this._eventAggregator.GetEvent<GUIMessageEvent>();

            CogDisplay = provider.Resolve<CogDisplayView>();

            this.CogDisplay.CogDisplayMouseMove += CogDisplay_CogDisplayMouseMove;
            this.CogDisplay.CogDisplayMouseUp += CogDisplay_CogDisplayMouseUp;
            this.CogDisplay.CogDisplayMouseDown += CogDisplay_CogDisplayMouseDown;
            this.CogDisplay.CogDisplayClick += CogDisplay_CogDisplayClick;
            this.CogDisplay.CogDisplayDoubleClick += CogDisplay_CogDisplayDoubleClick;

            eventAggregator.GetEvent<ApplicationExitEvent>().Subscribe((o) => Dispose(), true);

            InitializeCommand();
        }
        public void Dispose()
        {
            this.CogDisplay.DisposeDisplay();
        }

        public void Init()
        {
            this.IsLive = false;
            this.camera = cameraManager.Cameras.Values.FirstOrDefault(_ => _.CameraID == ProcessPos.CameraID);
        }


        private void InitializeCommand()
        {
            this.GrabCommand = new DelegateCommand(ExecuteGrabCommand);
            this.LiveCommand = new DelegateCommand(ExecuteLiveCommand);
            this.FindCommand = new DelegateCommand(ExecuteFindCommand);
            this.CusorCommand = new DelegateCommand(ExecuteCursorCommand);
            this.PanCommand = new DelegateCommand(ExecutePanCommand);
            this.ZoomInCommand = new DelegateCommand(ExecuteZoomInCommand);
            this.ZoomOutCommand = new DelegateCommand(ExecuteZoomOutCommand);
            this.FitCommand = new DelegateCommand(ExecuteFitCommand);
            this.RulerSquareCommand = new DelegateCommand(ExecuteRulerSquareCommand);
        }

        private void OnReceivedMessage(GUIEventArgs obj)
        {
            switch (obj.MessageKind)
            {
                case Common.Shared.eGUIMessageKind.SystemStateChange:
                    this.ChangeSystemState(obj);
                    break;
                case Common.Shared.eGUIMessageKind.FixAirspaceChanged:
                    this.CogDisplay.SetFixAirspace((bool)obj.Arg);
                    break;
            }
        }

        private void ChangeSystemState(GUIEventArgs obj)
        {
            var reqState = CastTo<eSystemState>.From<object>(obj.Arg);
            this.systemState = reqState;
        }

        #endregion

        #region UI Call Back Communication

        private void UICallbackCommunication(SchedulerEventArgs obj)
        {
            switch (obj.MessageKind)
            {
                case eSchedulerMessageKind.FindRunProcesser:
                    switch (obj.SubKind)
                    {
                        case eFindPos.Display:
                            receiveFindRunResult(obj);
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        private void receiveFindRunResult(SchedulerEventArgs obj)
        {
            var arg = CastTo<FindResult>.From<object>(obj.Arg);
            var graphics = arg.ResultGraphic as CogGraphicInteractiveCollection;
            var image = arg.InputImage as CogImage8Grey;
            var pos = obj.processPosition;

            if (image == null) return;

            if (pos.ExecuteZoneID == ProcessPos.ExecuteZoneID && pos.Quadrant == ProcessPos.Quadrant)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        // 라이브 모드 중이면 중단 (중복 접근 방지)
                        if (CogDisplay.IsLiveDisplay())
                        {
                            CogDisplay.StopLiveDisplay();
                            IsLive = false;
                        }

                        CogDisplay.ClearImage();
                        CogDisplay.ClearGraphics();

                        CogDisplay.SetImage(image);
                        CogDisplay.SetGraphic(graphics, "FindGraphics", true);
                    }
                    catch (Exception ex)
                    {
                        // 예외 로깅
                    }
                });
            }
        }

        #endregion

        #region ExcuteCommand

        private void ExecuteGrabCommand()
        {
            if (this.systemState == eSystemState.Auto) return;

            if (camera == null) return;

            if (this.cogDisplay.IsLiveDisplay())
            {
                this.cogDisplay.StopLiveDisplay();
                this.IsLive = false;
            }

            this.CogDisplay.ClearImage();
            this.CogDisplay.ClearGraphics();

            var image = (ICogImage)this.camera.GrabOneShot();

            this.CogDisplay.SetImage(image);
            this.CogDisplay.SetCenterRectangleGrid();
        }

        private void ExecuteLiveCommand()
        {
            if (this.systemState == eSystemState.Auto) return;

            if (this.cogDisplay.IsLiveDisplay())
            {
                this.cogDisplay.StopLiveDisplay();
                this.IsLive = false;
                return;
            }
            this.IsLive = true;

            var cogCamera = this.camera as CognexCamera;

            this.CogDisplay.StartLiveDisplay(cogCamera.Fifo);
            this.CogDisplay.SetCenterRectangleGrid();
        }

        private void ExecuteFindCommand()
        {
            if (this.cogDisplay.IsLiveDisplay())
            {
                this.cogDisplay.StopLiveDisplay();
                this.IsLive = false;
            }

            var message = new GUIEventArgs();
            message.MessageKind = eGUIMessageKind.RunProcesser;
            message.ProcessPosition = ProcessPos;
            message.SubKind = eFindPos.Display;

            this._gUIMessageEvent.Publish(message);
        }

        private void ExecuteCursorCommand()
        {
            this.CogDisplay.SetMouseMode(eDisplayMouseMode.Pointer);
        }

        private void ExecutePanCommand()
        {
            this.CogDisplay.SetMouseMode(eDisplayMouseMode.Pan);
        }

        private void ExecuteZoomInCommand()
        {
            this.CogDisplay.SetMouseMode(eDisplayMouseMode.ZoomIn);
        }

        private void ExecuteZoomOutCommand()
        {
            this.CogDisplay.SetMouseMode(eDisplayMouseMode.ZoomOut);
        }

        private void ExecuteFitCommand()
        {
            this.CogDisplay.FitImage();
        }

        private void ExecuteRulerSquareCommand()
        {
        }

        #endregion

        #region Events

        private void CogDisplay_CogDisplayMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            CogDisplayMouseMove?.Invoke(sender, e);
        }

        private void CogDisplay_CogDisplayMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            CogDisplayMouseUp?.Invoke(sender, e);
        }

        private void CogDisplay_CogDisplayMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            CogDisplayMouseDown?.Invoke(sender, e);
        }

        private void CogDisplay_CogDisplayClick(object sender, EventArgs e)
        {
            CogDisplayClick?.Invoke(sender, e);
        }

        private void CogDisplay_CogDisplayDoubleClick(object sender, EventArgs e)
        {
            CogDisplayDoubleClick?.Invoke(sender, e);
        }

        #endregion
    }
}
