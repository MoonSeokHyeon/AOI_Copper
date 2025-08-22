using MVision.Common.Event.EventArg;
using MVision.Common.Event;
using MVision.Common.Model;
using MVision.Common.Shared;
using MVision.UI.InterfaceViews.View;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVision.Formulas.Common;
using MVision.UI.CogDisplayViews.View;
using DOTNET.Utils;
using MVision.MairaDB;
using MVision.Common.DBModel;
using Cognex.VisionPro;

namespace MVision.UI.MainViews.ViewModel
{
    public class MainCalibrationViewModel : BindableBase
    {
        #region Properties

        CogDisplayView leftDisplay = null;
        public CogDisplayView LeftDisplay { get => this.leftDisplay; set => SetProperty(ref this.leftDisplay, value); }

        CogDisplayView rightDisplay = null;
        public CogDisplayView RightDisplay { get => this.rightDisplay; set => SetProperty(ref this.rightDisplay, value); }

        string leftDisplayHeader = null;
        public string LeftDisplayHeader { get => this.leftDisplayHeader; set => SetProperty(ref this.leftDisplayHeader, value); }

        string rightDisplayHeader = null;
        public string RightDisplayHeader { get => this.rightDisplayHeader; set => SetProperty(ref this.rightDisplayHeader, value); }

        private string movingXPitch;

        public string MovingXPitch
        {
            get { return movingXPitch; }
            set { SetProperty(ref this.movingXPitch, value); }
        }

        private string movingYPitch;

        public string MovingYPitch
        {
            get { return movingYPitch; }
            set { SetProperty(ref this.movingYPitch, value); }
        }

        private string movingTPitch;

        public string MovingTPitch
        {
            get { return movingTPitch; }
            set { SetProperty(ref this.movingTPitch, value); }
        }

        private ObservableCollection<eExecuteZoneID> calibrationList;
        public ObservableCollection<eExecuteZoneID> CalibrationList { get { return this.calibrationList; } set { SetProperty(ref this.calibrationList, value); } }

        PLCInterfaceVTypeView calibrationBitControl = null;
        public PLCInterfaceVTypeView CalibrationBitControl { get => this.calibrationBitControl; set => SetProperty(ref this.calibrationBitControl, value); }

        private List<CalibrationData> calibrationDataList;
        public List<CalibrationData> CalibrationDataList
        {
            get { return this.calibrationDataList; }
            set { SetProperty(ref this.calibrationDataList, value); }
        }

        IContainerProvider provider = null;
        IEventAggregator eventAggregator = null;
        ProcessPosition processPosition = new ProcessPosition();
        GUIMessageEvent _gUIMessageEvent = null;
        MariaManager sql = null;

        #endregion

        #region Construct

        public MainCalibrationViewModel(IEventAggregator aggregator, IContainerProvider provider, MariaManager sql)
        {
            this.eventAggregator = aggregator;
            this.provider = provider;
            this.sql = sql;

            eventAggregator.GetEvent<ApplicationExitEvent>().Subscribe((o) => Dispose(), true);

            eventAggregator.GetEvent<GUIMessageEvent>().Unsubscribe(OnReceivedMessage);
            eventAggregator.GetEvent<GUIMessageEvent>().Subscribe(OnReceivedMessage, ThreadOption.BackgroundThread);

            eventAggregator.GetEvent<SchedulerMessageEvent>().Unsubscribe(OnSchedulerReceivedMessage);
            eventAggregator.GetEvent<SchedulerMessageEvent>().Subscribe(OnSchedulerReceivedMessage, ThreadOption.BackgroundThread);

            this._gUIMessageEvent = this.eventAggregator.GetEvent<GUIMessageEvent>();

            InitProperties();
        }

        private void Dispose()
        {
            this.leftDisplay.DisposeDisplay();
            this.rightDisplay.DisposeDisplay();
        }

        public void Init()
        {
            CalibrationList = new ObservableCollection<eExecuteZoneID>();

            CalibrationList.Add(eExecuteZoneID.Main1_TARGET);
            CalibrationList.Add(eExecuteZoneID.Main2_TARGET);

            CalibrationBitControl.ViewModel.SubText = "";
        }

        private void InitProperties()
        {
            this.LeftDisplay = new CogDisplayView();
            this.rightDisplay = new CogDisplayView();

            this.CalibrationBitControl = this.provider.Resolve<PLCInterfaceVTypeView>();

        }

        private void OnReceivedMessage(GUIEventArgs obj)
        {
            switch (obj.MessageKind)
            {
                case Common.Shared.eGUIMessageKind.FixAirspaceChanged:
                    this.LeftDisplay.SetFixAirspace((bool)obj.Arg);
                    this.rightDisplay.SetFixAirspace((bool)obj.Arg);
                    break;
            }
        }

        private void OnSchedulerReceivedMessage(SchedulerEventArgs obj)
        {
            switch (obj.MessageKind)
            {
                case eSchedulerMessageKind.FindRunProcesser:
                    switch (obj.SubKind)
                    {
                        case eFindPos.Display:
                            SetGraphics(obj);
                            break;
                    }
                    break;

                case eSchedulerMessageKind.CalDataCalculateComplete:
                    //SetCalibrationResult(obj);
                    break;
                case eSchedulerMessageKind.CalibrationComplete:
                    SetCalibrationResult();
                    break;
            }
        }

        #endregion

        #region ExcuteCommand

        public void PositionChanged(object header)
        {
            var pos = (eExecuteZoneID)Enum.Parse(typeof(eExecuteZoneID), header.ToString());

            this.LeftDisplayHeader = pos.ToString() + "_CAM2";
            this.RightDisplayHeader = pos.ToString() + "_CAM1";

            switch (pos)
            {
                case eExecuteZoneID.Main1_TARGET:
                    CalibrationBitControl.ViewModel.SubText = "";
                    break;
                case eExecuteZoneID.TAPE_FEEDER:
                    CalibrationBitControl.ViewModel.SubText = "";
                    break;
                case eExecuteZoneID.TAPE_FEEDER2:
                    CalibrationBitControl.ViewModel.SubText = "";
                    break;
            }

            CalibrationBitControl.ViewModel.SetIO();

            this.processPosition.ExecuteZoneID = pos;
            this.CalibrationDataList = new List<CalibrationData>();

            //var modelData = this.sql.SystemInfo.GetAll().FirstOrDefault().CurrentModel;
            //var quadData = modelData.QuadrantDatas.FirstOrDefault(x => x.processPosition.ExecuteZoneID.Equals(pos) && x.processPosition.Quadrant.Equals(eQuadrant.FirstQuadrant));

            //MovingXPitch = quadData.MovingPitch.X.ToString();
            //MovingYPitch = quadData.MovingPitch.Y.ToString();
            //MovingTPitch = quadData.MovingPitch.T.ToString();

            SetCalibrationResult();
        }

        #endregion

        #region Private Method

        void SetGraphics(SchedulerEventArgs obj)
        {
            var kind = (eFindPos)obj.SubKind;

            if (kind == eFindPos.ModelEdit) return;

            var result = CastTo<FindResult>.From(obj.Arg);
            var graphics = result.InputImage as CogGraphicInteractiveCollection;
            var image = result.InputImage as CogImage8Grey;

            if (obj.processPosition.ExecuteZoneID != this.processPosition.ExecuteZoneID) return;

            if (obj.processPosition.Quadrant == eQuadrant.FirstQuadrant || obj.processPosition.Quadrant == eQuadrant.FourthQuadrant)
            {
                this.LeftDisplay.ClearGraphics();
                this.LeftDisplay.ClearImage();

                if (image == null) return;
                this.LeftDisplay.SetImage(image);
                this.LeftDisplay.SetGraphic(graphics, "graphics", true);
            }

            if (obj.processPosition.Quadrant == eQuadrant.SecondQuadrant || obj.processPosition.Quadrant == eQuadrant.ThirdQuadrant)
            {
                this.RightDisplay.ClearGraphics();
                this.RightDisplay.ClearImage();

                if (image == null) return;

                this.RightDisplay.SetImage(image);
                this.RightDisplay.SetGraphic(graphics, "graphics", true);
            }
        }

        private void SetCalibrationResult()
        {

            //var modelData = this.sql.SystemInfo.GetAll().FirstOrDefault().CurrentModel;
            //var quadData = modelData.QuadrantDatas.FindAll(x => x.processPosition.ExecuteZoneID.Equals(eExecuteZoneID.Main1_TARGET));
            //var calData = quadData.FindAll(x => x.processPosition.Quadrant.Equals(eQuadrant.FirstQuadrant) || x.processPosition.Quadrant.Equals(eQuadrant.SecondQuadrant));

            //this.CalibrationDataList = calData;
        }
        #endregion
    }
}
