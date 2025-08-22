using DOTNET.Concurrent;
using DOTNET.Excel;
using DOTNET.FileSystem;
using DOTNET.Logging;
using DOTNET.PLC.Model;
using DOTNET.PLC.XGT;
using DOTNET.PLC;
using DOTNET.Quartz;
using DOTNET.Utils;
using MVision.Common.Event.EventArg;
using MVision.Common.Event;
using MVision.Common.Shared;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVision.MairaDB;
using MVision.Manager.Vision;
using System.Configuration;
using MVision.Common.Model;
using System.Windows.Interop;
using DOTNET.PLC.SLMP;
using MVision.Manager.Robot;
using DOTNET.VISION.Model;

namespace MVision.SchedulerBase
{
    public abstract class ScheduleBase : IDisposable
    {
        #region Properties

        protected static Logger logger = Logger.GetLogger();
        protected Logger Unit1Logger = Logger.GetLogger("U1");
        protected Logger Unit2Logger = Logger.GetLogger("U2");
        protected Logger Unit3Logger = Logger.GetLogger("U3");
        protected Logger Unit4Logger = Logger.GetLogger("U4");
        protected Logger Unit5Logger = Logger.GetLogger("U5");
        protected Logger Unit6Logger = Logger.GetLogger("U6");

        protected IList<SchedulerWorker> workers = new List<SchedulerWorker>();

        protected IEventAggregator eventAggregator = null;
        protected IContainerProvider containerProvider = null;
        protected SlmpManager plc = null;
        //protected SqlManager sql = null;
        protected MariaManager sql = null;
        protected LibraryManager libraryManager = null; 
        protected LightControlManager lightControlManager = null;
        //protected RobotManager robot = null;
        protected eSystemState _systemState = eSystemState.Manual;

        protected SchedulerMessageEvent schedulerEventPulisher = null;

        protected bool IsWokerProcessing => workers.Any(w => w.IsProcessing);

        #endregion

        #region Construct

        public ScheduleBase(IEventAggregator eventAggregator, IContainerProvider containerProvider, SlmpManager plc, MariaManager sql, LibraryManager libraryManager, LightControlManager lightControlManager)
        {
            this.eventAggregator = eventAggregator;
            this.containerProvider = containerProvider;
            this.plc = plc;
            this.sql = sql;
            this.libraryManager = libraryManager;
            this.lightControlManager = lightControlManager;

            this.schedulerEventPulisher = this.eventAggregator.GetEvent<SchedulerMessageEvent>();

            this.eventAggregator.GetEvent<GUIMessageEvent>().Unsubscribe(OnReceivedMessage);
            this.eventAggregator.GetEvent<GUIMessageEvent>().Subscribe(OnReceivedMessage, ThreadOption.BackgroundThread);
          
            DOTNET.Utils.MidnightNotifier.DayChanged += MidnightNotifier_DayChanged;
            DOTNET.Utils.FixTimeNotifier.HourChanged += FixTimeNotifier_HourChanged;
        }

        public void Dispose()
        {
            this.workers.ToList().ForEach(worker => worker.Dispose());
            this.plc.Disconnect();
        }

        public virtual void Init()
        {
            InitPLC();
            //InitRobot();

            LockUtils.Wait(1000);
        }

        #endregion

        #region Abstract Mathod.

        protected abstract void Plc_OnWordChanged(WordBlock block);

        protected abstract void Plc_OnBitChanged(BitBlock block);

        protected abstract void RunProcesser(GUIEventArgs obj);

        //protected abstract void Robot_OnReciveData(object obj);


        #endregion

        #region UI Communication

        private void OnReceivedMessage(GUIEventArgs obj)
        {
            switch (obj.MessageKind)
            {
                case eGUIMessageKind.SystemStateChange:
                    this.ChangeSystemState(obj);
                    break;
                case eGUIMessageKind.ChangeModel:
                    this.ChangeModel(obj);
                    break;
                case eGUIMessageKind.ChangeModelList:
                    break;
                case eGUIMessageKind.LightValueChange:
                    break;
                case eGUIMessageKind.CameraPropertyChanged:
                    break;
                case eGUIMessageKind.MainViewChanged:
                    break;
                case eGUIMessageKind.RunProcesser:
                    this.RunProcesser(obj);
                    break;
                default:
                    break;
            }
        }

        void ChangeSystemState(GUIEventArgs args)
        {
            var reqState = CastTo<eSystemState>.From<object>(args.Arg);
            this._systemState = reqState;

            if (this._systemState != eSystemState.Auto)
                plc.WriteBit("TO_VISION_AUTO_MODE", false);

            else
                plc.WriteBit("TO_VISION_AUTO_MODE", true);

        }

        #endregion

        #region Model Change

        private void ChangeModel(GUIEventArgs obj)
        {
            //var model = sql.SystemInfo.GetAll().FirstOrDefault().CurrentModel;

        }

        protected virtual void ExecuteModelChange()
        {
            //plc.WriteBit("TO_MODEL_CHG_START", true);

            //var targetID = plc.ReadWord("FR_PLC_MODEL_NUMBER").DoubleValue;
            //var log = new SchedulerEventArgs();

            //try
            //{
            //    var systemInfo = sql.SystemInfo.GetAll().FirstOrDefault();

            //    var targetModel = sql.ModelData.FindBy(x => x.Id == targetID).SingleOrDefault();
            //    var modelID = sql.SystemInfo.GetAll().FirstOrDefault().CurrentProductModelId;
            //    var currentModel = sql.ModelData.FindBy(x => x.Id == modelID).SingleOrDefault();

            //    if (targetModel == null)
            //    {
            //        plc.WriteBit("TO_MODEL_CHG_FAIL", true);

            //        log.MessageKind = eSchedulerMessageKind.FailModelChange;
            //        log.MessageText = "Model Change Fail - Request Model ID not Exist";

            //        schedulerEventPulisher.Publish(log);
            //        logger.E($"Model Change Fail - Request Model {targetID} not Exist");

            //        return;
            //    }

            //    systemInfo.CurrentProductModelId = targetModel.Id;

            //    sql.SystemInfo.Edit(systemInfo);

            //    plc.WriteWord("TO_VISION_MODEL_NUMBER", systemInfo.CurrentProductModelId.ToString());

            //    this.libraryManager.CreateJobs();

            //    plc.WriteBit("TO_MODEL_CHG_OK", true);

            //    LockUtils.Wait(1000);

            //    plc.WriteBit("TO_MODEL_CHG_OK", false);
            //    plc.WriteBit("TO_MODEL_CHG_START", false);

            //    log.MessageKind = eSchedulerMessageKind.ChangeModel;
            //    log.MessageKey = targetModel.Name;
            //    log.MessageText = $"Model Changed {currentModel.Name} -> {targetModel.Name}";

            //    schedulerEventPulisher.Publish(log);
            //    logger.I($"Model Changed {currentModel.Name} -> {targetModel.Name}");
            //}
            //catch (Exception e)
            //{
            //    plc.WriteBit("TO_MODEL_CHG_FAIL", true);

            //    log.MessageKind = eSchedulerMessageKind.FailModelChange;
            //    log.MessageText = "Model Change Fail - Request Model ID not Exist";

            //    this.PublishMessageLog(log);

            //    logger.E(e);
            //}

            //var msg = new SchedulerEventArgs() { MessageKind = eSchedulerMessageKind.ChangeModel };
            //PublishMessageLog(msg);
        }

        #endregion

        #region PLC Event & PLC Init

        void InitPLC()
        {
            var fileName = ConfigurationManager.AppSettings["PLCMAP_FILE"];
            var path = Path.Combine(System.Environment.CurrentDirectory) + fileName;
            var plcConfig = new ExcelMapper(path).Fetch<PLCConfig>().ToList().FirstOrDefault();

            var grpB = new SlmpGroup { Name = "M", Device = SlmpDevice.M };
            var grpW = new SlmpGroup { Name = "D", Device = SlmpDevice.D };

            grpB.IsBit = true;

            Assert.IsTrue(MakeBitMap(grpB, fileName), "PLC Bit Map Load Fail");
            Assert.IsTrue(MakeWordMap(grpW, fileName), "PLC Word Map Load Fail");

            this.plc.Config = new SlmpConfig()
            {
                IpAddress = plcConfig.Addr,
                Port = plcConfig.PortNo,
                //RollingCount = 5,
                Id = plcConfig.Name,
                MonitorInterval = 50,
            };
            this.plc.DelayAutoOff = 700;

            this.plc.AddGroup(grpB);
            this.plc.AddGroup(grpW);

            //Event Connection
            this.plc.OnConnect += Plc_OnConnect;
            this.plc.OnCollected += Plc_OnCollected;
            this.plc.OnBitChanged += Plc_OnBitChanged;
            this.plc.OnWordChanged += Plc_OnWordChanged;
            this.plc.OnLog += Plc_OnLog;
            this.plc.HeartBeat("TO_VISION_ALIVE", 3000);

            //Connect UI 표시를 위해 (10sec Delay)
            TimerUtils.Once(3000, () => { this.plc.Connect(); });
        }

        private void Plc_OnConnect(string id)
        {
            logger.I("PLC Connected");

            //var currentModel = sql.SystemInfo.GetAll().FirstOrDefault().CurrentModel;
            //plc.WriteWord("TO_VISION_MODEL_NUMBER", currentModel.Id);

            //ResetPLCInterfaceBit();
        }

        private void Plc_OnDisconnect(string id)
        {
            logger.I("PLC Disconnected");
        }

        protected void Plc_OnFirstColtd(PFConfig cfg)
        {
            logger.I("PLC First Collected");

        }

        private void Plc_OnCollected(MapScan scan)
        {

        }

        private void Plc_OnLog(string log)
        {
            logger.I(log);
        }

        bool MakeWordMap(SlmpGroup grpW, string path)
        {
            var ll = new ExcelMapper(Path.Combine(System.Environment.CurrentDirectory) + path).Fetch<XlsW>("D");
            if (ll == null || !ll.Any())
                return false;

            ll = ll.Where(x => !string.IsNullOrEmpty(x.TagName)).ToList();

            foreach (var item in ll)
            {
                grpW.AddWordBlock(new SlmpWordBlock
                {
                    Name = item.TagName,
                    Address = item.Addr,
                    SubText = item.SubText,
                    SubNo = item.SubNo,
                    Point = item.Point,
                    Format = item.Format,
                    Multiple = item.MultipleV,
                    MultipleFormatter = item.MultipleFormat,
                    IsWatch = item.Watch,
                    //KindE = (ePlcKind)Enum.Parse(typeof(ePlcKind), item.Kind),
                    CallbackOrder = item.CallbackOrder,
                    Comment = item.Comment,
                    BitType = item.BitType,
                });
            }

            return true;
        }

        bool MakeBitMap(SlmpGroup grpB, string path)
        {
            var ll = new ExcelMapper(Path.Combine(System.Environment.CurrentDirectory) + path).Fetch<XlsB>("M");
            if (ll == null || !(ll.Count() > 0))
                return false;

            ll = ll.Where(x => !string.IsNullOrEmpty(x.TagName)).ToList();

            foreach (var item in ll)
            {
                grpB.AddBitBlock(new SlmpBitBlock
                {
                    Name = item.TagName,
                    Address = item.Addr,
                    BitIndex = item.BitIndex,
                    //KindE = (ePlcKind)Enum.Parse(typeof(ePlcKind), item.Kind),
                    SubText = item.SubText,
                    SubNo = item.SubNo,
                    CallbackOrder = item.CallbackOrder,
                    Comment = item.Comment,
                    IsAutoOff = item.AutoOff,
                    //IsBitOn = true,
                });
            }

            return true;
        }

        #endregion

        #region Robot Event & Init
        private void InitRobot()
        {
            //this.robot.OnConnected += Robot_OnConnected;
            //this.robot.OnDisconnected += Robot_OnDisconnected;
            //this.robot.OnReciveData += Robot_OnReciveData;

            //robot.Init();
        }

        private void Robot_OnConnected()
        {
            //var meg = new SchedulerEventArgs();
            //meg.MessageKind = eSchedulerMessageKind.TCPStatusChanged;
            //meg.SubKind = eConnectStasus.Connected;

            //schedulerEventPulisher.Publish(meg);

            logger.I("Robot Server Connected");
        }

        private void Robot_OnDisconnected()
        {
            //var meg = new SchedulerEventArgs();
            //meg.MessageKind = eSchedulerMessageKind.TCPStatusChanged;
            //meg.SubKind = eConnectStasus.Disconnected;

            //schedulerEventPulisher.Publish(meg);

            logger.I("Robot Server Disconnected");
        }

        #endregion

        #region Protected Method

        protected void Addworker(SchedulerWorker worker)
        {
            this.workers.Add(worker);
        }

        protected SchedulerWorker GetCoreWorker(string name)
        {
            return workers.FirstOrDefault(_ => _.Name.Equals(name));
        }

        //protected void PublishAlignLog(AlignHistory log)
        //{
        //    this.sql.AlignHistory.Add(log);

        //    var msg = new SchedulerEventArgs() { MessageKind = eSchedulerMessageKind.AddAlignHistory };
        //    msg.Arg = log;
        //    this.eventAggregator.GetEvent<SchedulerMessageEvent>().Publish(msg);
        //}

        //protected void PublishInspectionLog(InspectionHistory log)
        //{
        //    this.sql.InspectionHistory.Add(log);

        //    var msg = new SchedulerEventArgs() { MessageKind = eSchedulerMessageKind.AddInspectionHistory };
        //    msg.Arg = log;
        //    this.eventAggregator.GetEvent<SchedulerMessageEvent>().Publish(msg);
        //}

        protected void PublishMessageLog(SchedulerEventArgs log)
        {
            var msg = new SchedulerEventArgs() { MessageKind = log.MessageKind };
            msg.Arg = log;
            this.eventAggregator.GetEvent<SchedulerMessageEvent>().Publish(msg);
        }

        #region Set PLC
        protected void SetAlignStartBit(string Unit)
        {
            if (Unit == "U1")
                ResetUVRW(Unit);
            else
                ResetXYT(Unit);

            //ActionResetBit(Unit);

            plc.WriteBit($"TO_{Unit}_READY", false);
            plc.WriteBit($"TO_{Unit}_ALIGN_START", true);

        }

        protected bool SetAlignEndBit(string Unit, bool isOK)
        {
            if (isOK)
                plc.WriteBit($"TO_{Unit}_ALIGN_OK", true);
            else
                plc.WriteBit($"TO_{Unit}_ALIGN_NG", true);

            plc.WriteBit($"TO_{Unit}_ALIGN_END", true);
            plc.WriteBit($"TO_{Unit}_2ND_REQUEST", false);
            plc.WriteBit($"TO_{Unit}_READY", true);

            return true;
        }

        protected void SetCalibrationStartBit(string Unit)
        {
            if (Unit == "U1")
                ResetUVRW(Unit);
            else
                ResetXYT(Unit);

            //ActionResetBit(Unit);

            plc.WriteBit($"TO_{Unit}_READY", false);
            plc.WriteBit($"TO_{Unit}_CALIBRATION_START", true);

        }

        protected bool SetCalibrationEndBit(string Unit, bool judgment)
        {
            if (judgment)
                plc.WriteBit($"TO_{Unit}_CAL_NG", true);

            plc.WriteBit($"TO_{Unit}_CAL_END", true);
            plc.WriteBit($"TO_{Unit}_CAL_START", false);

            if (!plc.WaitChgBit(true, 10000, $"FR_{Unit}_CAL_END"))
            {
                SetResetBit(Unit);
                SetLogger(Unit, "CAL_END is Not true");

                return false;
            }

            plc.WriteBit($"TO_{Unit}_CAL_END", false);

            plc.WriteBit($"TO_{Unit}_CAL_NG", false);

            plc.WriteBit($"TO_{Unit}_READY", true);

            return true;
        }

        protected bool MoveCommand(string Unit, bool isLower, bool is2nd)
        {
            plc.WriteBit($"TO_{Unit}_CAL_MOVE_COMMAND", true);

            LockUtils.Wait(100);

            if (!plc.WaitChgBit(true, 10000, $"FR_{Unit}_CAL_MOVE_ACK"))
            {
                SetLogger("U1", "MOVE_ACK is Not true");

                return false;
            }

            plc.WriteBit($"TO_{Unit}_CAL_MOVE_COMMAND", false);

            LockUtils.Wait(100);

            if (!plc.WaitChgBit(true, 10000, $"FR_{Unit}_CAL_MOVE_DONE"))
            {
                SetLogger("U1", "MOVE_DONE_COMMAND is not true");

                return false;
            }

            plc.WriteBit($"TO_{Unit}_CAL_MOVE_DONE_ACK", true);

            if (!plc.WaitChgBit(false, 10000, $"FR_{Unit}_CAL_MOVE_DONE"))
            {
                SetLogger("U1", "MOVE_DONE_COMMAND is not false");

                return false;
            }

            plc.WriteBit($"TO_{Unit}_CAL_MOVE_DONE_ACK", false);


            return true;
        }
        protected void SetRevUVW(string Unit, XXY xxyy)
        {
            plc.WriteWord($"TO_{Unit}_REV_POSITION_X1", xxyy.X1);
            plc.WriteWord($"TO_{Unit}_REV_POSITION_X2", xxyy.X2);
            plc.WriteWord($"TO_{Unit}_REV_POSITION_Y1", xxyy.Y1);

            SetLogger(Unit, "SET_REV_POSITION_XXY");
        }

        protected void SetResetBit(string Unit)
        {

            plc.WriteBit($"TO_{Unit}_START", false);
            plc.WriteBit($"TO_{Unit}_END", false);
            plc.WriteBit($"TO_{Unit}_NG", false);
            plc.WriteBit($"TO_{Unit}_OK", false);

            plc.WriteBit($"TO_{Unit}_CAL_START", false);
            plc.WriteBit($"TO_{Unit}_CAL_END", false);
            plc.WriteBit($"TO_{Unit}_CAL_MOVE_COMMAND", false);
            plc.WriteBit($"TO_{Unit}_CAL_MOVE_DONE_ACK", false);
            plc.WriteBit($"TO_{Unit}_CAL_NG", false);

            plc.WriteBit($"TO_{Unit}_READY", true);

        }

        protected void ResetXYT(string Unit)
        {
            plc.WriteWord($"TO_{Unit}_TARGET_POSITION_X", "0.0000");
            plc.WriteWord($"TO_{Unit}_TARGET_POSITION_Y", "0.0000");
            plc.WriteWord($"TO_{Unit}_TARGET_POSITION_T", "0.0000");

            SetLogger(Unit, "RESET_TARGET_POSITION_XYT");
        }

        protected void ResetUVRW(string Unit)
        {
            plc.WriteWord($"TO_{Unit}_REV_POSITION_X1", "0.0000");
            plc.WriteWord($"TO_{Unit}_REV_POSITION_X2", "0.0000");
            plc.WriteWord($"TO_{Unit}_REV_POSITION_Y1", "0.0000");
            plc.WriteWord($"TO_{Unit}_REV_POSITION_Y2", "0.0000");

            SetLogger(Unit, "RESET_REV_POSITION_XXYY");

            plc.WriteWord($"TO_{Unit}_OBJECT_POSITION_X1", "0.0000");
            plc.WriteWord($"TO_{Unit}_OBJECT_POSITION_X1", "0.0000");
            plc.WriteWord($"TO_{Unit}_OBJECT_POSITION_X1", "0.0000");
            plc.WriteWord($"TO_{Unit}_OBJECT_POSITION_X1", "0.0000");

            SetLogger(Unit, "RESET_OBJECT_POSITION_XXYY");
        }

        

        protected void SetLogger(string Unit, string str)
        {
            switch (Unit)
            {
                case "U1":
                    Unit1Logger.I(str);
                    break;
                case "U2":
                    Unit2Logger.I(str);
                    break;
                case "U3":
                    Unit3Logger.I(str);
                    break;
                case "U4":
                    Unit4Logger.I(str);
                    break;
                case "U5":
                    Unit5Logger.I(str);
                    break;
                case "U6":
                    Unit6Logger.I(str);
                    break;
            }
        }

        #endregion

        #region Get PLC

        protected (double, double, double) GetCrurrentCamXXY(string Unit)
        {
            var a = plc.ReadWord($"FR_{Unit}_CAM_X1");
            var b = plc.ReadWord($"FR_{Unit}_CAM_X2");
            var c = plc.ReadWord($"FR_{Unit}_CAM_Y1");

            SetLogger(Unit, "GET_CURRENT_CAM_XXY");

            return (a, b, c);
        }

        protected XY GetCrurrentCamXY(string Unit)
        {
            var retValue = new XY();

            retValue.X = plc.ReadWord($"FR_{Unit}_CAM_X");
            retValue.Y = plc.ReadWord($"FR_{Unit}_CAM_Y");

            SetLogger(Unit, "GET_CURRENT_CAM_XY");

            return retValue;
        }

        protected XXYY GetCurrentUVRW(string Unit)
        {
            var retValue = new XXYY();

            retValue.X1 = plc.ReadWord($"FR_{Unit}_CURRENT_X1");
            retValue.X2 = plc.ReadWord($"FR_{Unit}_CURRENT_X2");
            retValue.Y1 = plc.ReadWord($"FR_{Unit}_CURRENT_Y1");
            retValue.Y2 = plc.ReadWord($"FR_{Unit}_CURRENT_Y2");

            SetLogger(Unit, "GET_CURRENT_XXYY");

            return retValue;
        }

        protected (XYT, XYT) GetCurrentXYT(string Unit)
        {
            var retValue1 = new XYT();
            var retValue2 = new XYT();

            retValue1.X = plc.ReadWord($"FR_{Unit}_CURRENT_X1");
            retValue1.Y = plc.ReadWord($"FR_{Unit}_CURRENT_Y1");
            retValue1.T = plc.ReadWord($"FR_{Unit}_CURRENT_T1");

            retValue2.X = plc.ReadWord($"FR_{Unit}_CURRENT_X2");
            retValue2.Y = plc.ReadWord($"FR_{Unit}_CURRENT_Y2");
            retValue2.T = plc.ReadWord($"FR_{Unit}_CURRENT_T2");

            SetLogger(Unit, "GET_CURRENT_XYT");

            return (retValue1, retValue2);
        }

        protected XYT GetCurrentOffset(string Unit)
        {
            var retValue = new XYT();

            retValue.X = plc.ReadWord($"FR_{Unit}_OFFSET_X");
            retValue.Y = plc.ReadWord($"FR_{Unit}_OFFSET_Y");
            retValue.T = plc.ReadWord($"FR_{Unit}_OFFSET_T");

            SetLogger(Unit, "GET_CURRENT_OFFSET_XYT");

            return retValue;
        }

        #endregion

        #endregion

        #region Private Method

        private void FixTimeNotifier_HourChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    var setDay = CastTo<double>.From<string>(sql.SystemSetting.FindBy(x => x.Name.Equals("ImageBackupDay")).Single().Value);
            //    var dt = DateTime.Now.AddDays(-setDay);
            //    var directoryPath = ConfigurationManager.AppSettings["SAVE_PATH"];

            //    int delCount = 0;

            //    foreach (string path in FileUtils.GetFiles(directoryPath, "*", true))
            //    {
            //        var fi = new FileInfo(path);
            //        if (fi.LastWriteTime > dt)
            //            continue;

            //        FileUtils.DeleteFileIfExist(path);
            //        delCount++;
            //        logger.D($"Deleted File - Name : [ {fi.Name} ]");

            //        if (delCount > 200)
            //            break;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    logger.E(ex);
            //}
        }

        private void MidnightNotifier_DayChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    //Image Delete
            //    logger.D($"Image BackUp Delete Start");

            //    var setDay = CastTo<double>.From<string>(sql.SystemSetting.FindBy(x => x.Name.Equals("ImageBackUpDays")).Single().Value);
            //    var dt = DateTime.Now.AddDays(-setDay);
            //    var directoryPath = ConfigurationManager.AppSettings["SAVE_PATH"];

            //    int delCount = 0;


            //    foreach (string path in FileUtils.GetFiles(directoryPath, "*", true))
            //    {
            //        var fi = new FileInfo(path);
            //        if (fi.LastWriteTime > dt)
            //            continue;

            //        FileUtils.DeleteFileIfExist(path);
            //        delCount++;
            //        logger.D($"Deleted File - Name : [ {fi.Name} ]");

            //        if (delCount > 200)
            //            break;
            //    }

            //    //History Clean.
            //    logger.D($"Log BackUp Delete Start");

            //    setDay = CastTo<double>.From<string>(sql.SystemSetting.FindBy(x => x.Name.Equals("LogBackUpDays")).Single().Value);
            //    var backup = DateTime.Now.AddDays(-setDay);
            //    this.sql.AlignHistory.Delete(x => x.CreateDate < backup);
            //    this.sql.InspectionHistory.Delete(x => x.CreateDate < backup);

            //    //DB Backup
            //    logger.D($"DB BackUp Delete Start");

            //    var backupDir = @"D:\DB\FPCB\Backup\";
            //    var backupFileName = $@"Backup_{DateTime.Now.ToString("yy/MM/dd")}.sqlite";
            //    //var sourcePath = @"D:\DB\VASFxDb.sqlite";

            //    var connectionString = ConfigurationManager.ConnectionStrings[sql.ConnentionString].ConnectionString;
            //    var startIndex = connectionString.IndexOf("=");
            //    var endIndex = connectionString.IndexOf(";");
            //    var sourcePath = connectionString.Substring(startIndex + 1, (endIndex - startIndex) - 1);

            //    var targetFullPath = System.IO.Path.Combine(backupDir + backupFileName);

            //    if (!Directory.Exists(backupDir))
            //        Directory.CreateDirectory(backupDir);

            //    System.IO.File.Copy(sourcePath, targetFullPath);

            //    //DB Backup Clean
            //    delCount = 0;

            //    foreach (string path in FileUtils.GetFiles(backupDir, "*", false))
            //    {
            //        var fi = new FileInfo(path);
            //        if (fi.LastWriteTime > backup)
            //            continue;

            //        FileUtils.DeleteFileIfExist(path);
            //        delCount++;
            //        logger.D($"Deleted File - Name : [ {fi.Name} ]");

            //        if (delCount > 100)
            //            break;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    logger.E(ex);
            //}
        }
        #endregion


    }
}
