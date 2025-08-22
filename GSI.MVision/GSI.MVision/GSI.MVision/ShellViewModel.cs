using DOTNET.Excel;
using DOTNET.Logging;
using DOTNET.OSView;
using DOTNET.PLC.SLMP;
using DOTNET.Quartz;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using MVision.Common.DBModel;
using MVision.Common.Event;
using MVision.Common.Event.EventArg;
using MVision.Common.Shared;
using MVision.Common.Template;
using MVision.MairaDB;
using MVision.Manager.Vision;
using MVision.UI.InteractivityViews.View;
using MVision.UI.MainViews.View;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;


namespace GSI.MVision
{
    public class ShellViewModel : BindableBase, IDisposable
    {

        #region Properties

        Logger logger = Logger.GetLogger();

        #region Status

        private double _cpu;
        public double CPU
        {
            get { return this._cpu; }
            set { this.SetProperty(ref this._cpu, value); }
        }

        private double _totalCPU;
        public double TotalCPU
        {
            get { return this._totalCPU; }
            set { this.SetProperty(ref this._totalCPU, value); }
        }

        private double _ram;
        public double RAM
        {
            get { return this._ram; }
            set { this.SetProperty(ref this._ram, value); }
        }

        ListBoxTemplate cpuUsage;
        public ListBoxTemplate CPUUsage
        {
            get { return this.cpuUsage; }
            set { SetProperty(ref this.cpuUsage, value); }
        }

        ListBoxTemplate ramUsage;
        public ListBoxTemplate RAMUsage
        {
            get { return this.ramUsage; }
            set { SetProperty(ref this.ramUsage, value); }
        }

        private List<ListBoxTemplate> hddDriveList;

        public List<ListBoxTemplate> HDDDriveList
        {
            get { return this.hddDriveList; }
            set { SetProperty(ref this.hddDriveList, value); }
        }

        private bool isPLCConnected;
        public bool IsPLCConnected
        {
            get { return isPLCConnected; }
            set { this.SetProperty(ref this.isPLCConnected, value); }
        }

        private string plcConnect;
        public string PLCConnect
        {
            get { return plcConnect; }
            set { this.SetProperty(ref this.plcConnect, value); }
        }

        private long plcScanTime;
        public long PLCScanTime
        {
            get { return plcScanTime; }
            set { SetProperty(ref plcScanTime, value); }
        }

        DateTime _dateTime;
        public DateTime DateTime
        {
            get { return _dateTime; }
            set { this.SetProperty(ref _dateTime, value); }
        }

        private string systemID;
        public string SystemID
        {
            get { return systemID; }
            set { SetProperty(ref this.systemID, value); }
        }


        private bool isCanOperation;
        public bool IsCanOperation
        {
            get { return isCanOperation; }
            set { SetProperty(ref this.isCanOperation, value); }
        }

        #endregion

        #region Model

        private string currentModelName;

        public string CurrentModelName
        {
            get { return currentModelName; }
            set { SetProperty(ref this.currentModelName, value); }
        }

        private List<string> modelList = new List<string>();

        public List<string> ModelList
        {
            get { return this.modelList; }
            set { SetProperty(ref this.modelList, value); }
        }

        private string targetModel;
        public string TargetModel { get => this.targetModel; set => SetProperty(ref this.targetModel, value); }


        #endregion

        eSystemState IsSystemAuto = eSystemState.Manual;
        public eSystemState SystemState { get => this.IsSystemAuto; set => SetProperty(ref this.IsSystemAuto, value); }

        public Shell View { get; set; }

        IContainerProvider provider = null;
        IRegionManager regionManager = null;
        IEventAggregator eventAggregator = null;
        MariaManager sql = null;
        LibraryManager libraryManager = null;
        SlmpManager plc = null;
        GUIMessageEvent guiEventPublisher = null;

        #endregion

        #region ICommand

        public ICommand SelectedMainManuCommand { get; set; }
        public ICommand OptionViewCommand { get; set; }
        public ICommand ModelEditorCommand { get; set; }
        public ICommand ChangeModelCommand { get; set; }
        public ICommand WindowMinCommand { get; set; }

        public ICommand SystemOffCommand { get; set; }

        #endregion

        public ShellViewModel(IEventAggregator aggregator, IRegionManager region, IContainerProvider provider, SlmpManager plc, MariaManager mairaManager, LibraryManager libraryManager)
        {
            this.eventAggregator = aggregator;
            this.regionManager = region;
            this.sql = mairaManager;
            this.libraryManager = libraryManager;
            this.plc = plc;
            this.provider = provider;

            guiEventPublisher = this.eventAggregator.GetEvent<GUIMessageEvent>();

            InitOSView();
            InitDataTime();
            InitDelegeteCommand();

        }
        public void Init()
        {
            this.regionManager.RequestNavigate(SharedRegion.MainView, nameof(MainAutoView));

            SystemState = eSystemState.Manual;
            this.PLCConnect = "PLC Disconnected";

            this.IsCanOperation = true;

            var models = sql.ModelData.All().ToList();
            this.CurrentModelName = sql.MachineData.All().FirstOrDefault().MachineName;
            this.TargetModel = CurrentModelName;

            foreach (var item in models)
            {
                if (this.ModelList.Count > 0) ModelList.Clear();

                this.ModelList.Add(item.ModelName);
            }

        }

        private void InitOSView()
        {
            QuartzUtils.Invoke("RESOURCE_CHECK", QuartzUtils.GetExpnSecond(1), QuzOnResourceUsage);

            this.CPUUsage = new ListBoxTemplate() { Tag = "CPU" };
            this.RAMUsage = new ListBoxTemplate() { Tag = "RAM" };
            
            this.HDDDriveList = new List<ListBoxTemplate>();
            var ll = Mgnt.HddList();
            ll.ForEach(x =>
            {
                var hdd = new ListBoxTemplate();
                hdd.Tag = x.Name;
                var free = x.AvailableFreeSpace;
                var total = x.TotalSize;
                var usage = x.TotalSize - x.AvailableFreeSpace;
                hdd.Value1 = (usage / total) * 100;

                this.HDDDriveList.Add(hdd);
            });
        }
        void InitDelegeteCommand()
        {
            this.SelectedMainManuCommand = new DelegateCommand<string>(ExecuteSelectedMainManuCommand);

            this.ModelEditorCommand = new DelegateCommand(ExecuteModelEditorCommand);
            this.ChangeModelCommand = new DelegateCommand(ExecuteChangeModelCommand);

            this.WindowMinCommand = new DelegateCommand(ExecuteWindowMinCommand);
            this.SystemOffCommand = new DelegateCommand(ExecuteSystemOff);
        }
        void ExecuteSelectedMainManuCommand(string obj)
        {
            switch (obj)
            {
                case "Auto":
                    this.regionManager.RequestNavigate(SharedRegion.MainView, nameof(MainAutoView));
                    break;

                case "Calibration":
                    this.regionManager.RequestNavigate(SharedRegion.MainView, nameof(MainCalibrationView));
                    break;

                case "Edit":
                    this.regionManager.RequestNavigate(SharedRegion.MainView, nameof(MainEditView));
                    break;

                case "Interface":
                    this.regionManager.RequestNavigate(SharedRegion.MainView, nameof(MainInterfaceView));
                    break;

                case "Log":
                    this.regionManager.RequestNavigate(SharedRegion.MainView, nameof(MainVisionLogView));
                    break;

                case "ImageLog":
                    //this.regionManager.RequestNavigate(SharedRegion.MainView, nameof(MainImageLogView));
                    break;

                case "Option":
                    this.regionManager.RequestNavigate(SharedRegion.MainView, nameof(MainOptionView));
                    break;
                default:
                    break;
            }
        }
        private async void ExecuteModelEditorCommand()
        {
            var view = this.provider.Resolve<ModelManagerView>();

            if (!DialogHost.IsDialogOpen("RootDialog"))
                await DialogHost.Show(view, "RootDialog");
        }

        private async void ExecuteChangeModelCommand()
        {
            if (this.SystemState == eSystemState.Auto) return;

            var view = this.provider.Resolve<ComfirmationView>();
            view.ViewModel.Message = $"Model Change to {this.TargetModel} ?";

            var result = await DialogHost.Show(view, "RootDialog") as bool?;
            if (result == true)
            {
                var machine = sql.MachineData.All().FirstOrDefault();
                var targetMode = sql.ModelData.All().FirstOrDefault(x => x.ModelName.Equals(TargetModel));
                machine.CurrentModelId = targetMode.ModelID;
                sql.MachineData.Update(machine);
                CurrentModelName = targetMode.ModelName;

                plc.WriteWord("TO_VISION_MODEL_NUMBER", machine.CurrentModelId.ToString());

                this.libraryManager.CreateJobs();

                var msg = new GUIEventArgs()
                {
                    MessageKind = eGUIMessageKind.ChangeModel,
                };

                this.guiEventPublisher.Publish(msg);
            }
        }

        private async void ExecuteSystemOff()
        {
            var msg = new GUIEventArgs()
            {
                MessageKind = eGUIMessageKind.FixAirspaceChanged,
                Arg = true,
            };

            this.guiEventPublisher.Publish(msg);

            var view = this.provider.Resolve<ComfirmationView>();
            view.ViewModel.Message = "System Off ?";

            var result = await DialogHost.Show(view, "RootDialog") as bool?;

            msg.Arg = false;
            this.guiEventPublisher.Publish(msg);

            if (result == true)
                App.Current.Shutdown();
        }

        private void ExecuteWindowMinCommand()
        {
            View.WindowState = System.Windows.WindowState.Minimized;
        }

        private void QuzOnResourceUsage()
        {
            try
            {
                this.CPU = Mgnt.CpuUseRate();
                this.CPUUsage.Value1 = this.CPU;

                this.RAM = Math.Abs(Mgnt.MemPhysicalUseRate());
                this.RAMUsage.Value1 = this.RAM;

                var ll = Mgnt.HddList();
                ll.ForEach(hdd =>
                {
                    var hddUsage = this.HDDDriveList.FirstOrDefault(x => x.Tag.Equals(hdd.Name));
                    var free = hdd.AvailableFreeSpace;
                    double total = hdd.TotalSize;
                    double usage = hdd.TotalSize - hdd.AvailableFreeSpace;
                    hddUsage.Value1 = (usage / total) * 100;
                });

            }
            catch (Exception ex)
            {
                logger.E(ex);
            }
        }
        private void InitDataTime()
        {
            QuartzUtils.Invoke("TIME_CHECK", QuartzUtils.GetExpnSecond(1), QuzOnDateTime);
        }
        private void QuzOnDateTime()
        {
            this.DateTime = DateTime.Now;
        }

        public void Dispose()
        {
            QuartzUtils.StopSchedule("RESOURCE_CHECK");
            QuartzUtils.StopSchedule("TIME_CHECK");

        }
    }
}
