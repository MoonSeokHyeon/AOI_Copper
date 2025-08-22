using System;
using DOTNET.Concurrent;
using DOTNET.Logging;
using DOTNET.PLC.Model;
using DOTNET.PLC.SLMP;
using DOTNET.PLC.XGT;
using DOTNET.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prism.Events;
using Prism.Ioc;
using System.Threading.Tasks;
using MVision.SchedulerBase;
using DOTNET.VISION.Model;
using MVision.Formulas.Common;
using MVision.Common.Shared;
using MVision.Common.Event;
using MVision.Manager.Vision;
using MVision.MairaDB;
using MVision.Common.Event.EventArg;
using MVision.Manager.Robot;
using MVision.Common.Model;
using eQuadrant = MVision.Common.Shared.eQuadrant;
using DOTNET.VISION;
using MVision.Common.DBModel;

namespace MVision.Scheduler.Defualt
{
    public class DefualtScheduler : ScheduleBase
    {
        #region Properties

        AlignFormulas algorithmProcesser = null;

        TsQueue<eSchedulerCommandKind> U1 = new TsQueue<eSchedulerCommandKind>();
        TsQueue<eSchedulerCommandKind> U2 = new TsQueue<eSchedulerCommandKind>();
        TsQueue<eSchedulerCommandKind> U3 = new TsQueue<eSchedulerCommandKind>();
        TsQueue<eSchedulerCommandKind> U4 = new TsQueue<eSchedulerCommandKind>();
        TsQueue<eSchedulerCommandKind> U5 = new TsQueue<eSchedulerCommandKind>();
        TsQueue<eSchedulerCommandKind> U6 = new TsQueue<eSchedulerCommandKind>();
        TsQueue<eSchedulerCommandKind> U7 = new TsQueue<eSchedulerCommandKind>();
        TsQueue<eSchedulerCommandKind> U8 = new TsQueue<eSchedulerCommandKind>();

        IEventAggregator _eventAggregator = null;
        IContainerProvider provider = null;
        #endregion

        #region Construct

        public DefualtScheduler(IEventAggregator eventAggregator, IContainerProvider provider, AlignFormulas algorithmProcesser, SlmpManager plc, MariaManager sql, LibraryManager libraryManager, LightControlManager lightControlManager)
            : base(eventAggregator, provider, plc, sql, libraryManager, lightControlManager)
        {
            this.algorithmProcesser = algorithmProcesser;
            eventAggregator.GetEvent<ApplicationExitEvent>().Subscribe((o) => Dispose(), true);

            this._eventAggregator = eventAggregator;
            this.provider = provider;
        }

        public override void Init()
        {
            var u1 = new SchedulerWorker("U1");
            u1.WorkQueue = this.U1;
            u1.Align1 = U1_AlignAction;
            u1.Calibration1 = U1_Calibration1Action;
            base.Addworker(u1);
            base.Init();
        }

        #endregion

        #region PLC

        protected override void Plc_OnBitChanged(BitBlock block)
        {
            if (block.Name.Equals("FR_PLC_ALIVE")) return;

            logger.I(block);

            try
            {
                if (!block.IsBitOn)
                    return;

                SchedulerEventArgs arg = new SchedulerEventArgs();

                switch (block.Name)
                {
                    case "FR_U1_START":
                        this.U1.Enqueue(eSchedulerCommandKind.Align1);

                        arg.MessageKind = eSchedulerMessageKind.ViewLoggerChanged;
                        arg.SubKind = eMessageLevelKind.PLC;
                        arg.LevelKind = eMessageLevelKind.Info;
                        arg.MessageText = "U1 Align Start Command Enqueue";

                        schedulerEventPulisher.Publish(arg);
                        break;

                    case "FR_U1_CAL_START":
                        this.U1.Enqueue(eSchedulerCommandKind.Calibration1);

                        arg.MessageKind = eSchedulerMessageKind.ViewLoggerChanged;
                        arg.LevelKind = eMessageLevelKind.Info;
                        arg.SubKind = eMessageLevelKind.PLC;
                        arg.MessageText = "U1 Calibration Start Command Enqueue";

                        schedulerEventPulisher.Publish(arg);
                        break;

                    case "FR_ERROR_RESET":
                        arg.MessageKind = eSchedulerMessageKind.ViewLoggerChanged;
                        arg.SubKind = eMessageLevelKind.PLC;
                        arg.LevelKind = eMessageLevelKind.Info;
                        arg.MessageText = "PLC Error Reset Bit On";

                        schedulerEventPulisher.Publish(arg);

                        break;

                    case "FR_REQ_MODEL_CHANGE":
                        arg.MessageKind = eSchedulerMessageKind.ViewLoggerChanged;
                        arg.SubKind = eMessageLevelKind.PLC;
                        arg.LevelKind = eMessageLevelKind.Info;
                        arg.MessageText = "Model Change Request";

                        schedulerEventPulisher.Publish(arg);

                        base.ExecuteModelChange();
                        break;
                        
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                ScheduleBase.logger.E(ex.StackTrace);
            }
        }

        protected override void Plc_OnWordChanged(WordBlock block)
        {
        }


        protected override void RunProcesser(GUIEventArgs obj)
        {
           
        }
    
        #endregion

        #region U1 Action

        private void U1_AlignAction()
        {
            if (this._systemState != eSystemState.Auto)
                return;

            var targetResults = new List<FindResult>();

            var targetCurrentPos = new Dictionary<eQuadrant, XYT>();

            var targetPos1 = new ProcessPosition(eCameraID.Camera2, eExecuteZoneID.Main1_TARGET, eQuadrant.FirstQuadrant);
            var targetPos2 = new ProcessPosition(eCameraID.Camera1, eExecuteZoneID.Main1_TARGET, eQuadrant.SecondQuadrant);

            var grabLimit = int.Parse(this.sql.SystemSettingData.All().FirstOrDefault(x => x.Name.Equals("AutoGrabRetryLimit")).Value);

            //light on
            var lightList = sql.LightData.All().ToList().FindAll(x => x.ZoneID.Equals(eExecuteZoneID.Main1_TARGET) && x.ChUse.Equals(true));
            lightList.ForEach(x => lightControlManager.LightControllers[(int)eExecuteZoneID.Main1_TARGET].LightOn((int)x.Channel, x.Value));

            LockUtils.Wait(200);

            SetAlignStartBit("U1");
            SetLogger("U1", "Align Start");

            //var align_Total_sT = SwUtils.CurrentTimeMillis;

            //Run Processor Target first, second
            var targetPos1Result = this.algorithmProcesser.FindRun(targetPos1, eFindPos.Display, null, null);

            if (!targetPos1Result.isSuccess)
            {
                SetLogger("U1", "Pattern Find Fail");
                //SetViewLogger(targetPos1.ExecuteZoneID, "Align1", null, eSchedulerCommandKind.Align, eMessageLevelKind.Info, $"Pattern find fail", false);
                SetAlignEndBit("U1", false);
                return;
            }

            var targetPos2Result = this.algorithmProcesser.FindRun(targetPos2, eFindPos.Display, null, null);

            if (!targetPos2Result.isSuccess)
            {
                SetLogger("U1", "Pattern Find Fail");
                //SetViewLogger(targetPos2.ExecuteZoneID, "Align1", null, eSchedulerCommandKind.Align, eMessageLevelKind.Info, $"Pattern find fail", false);
                SetAlignEndBit("U1", false);
                return;
            }

            targetResults.Add(targetPos1Result);
            targetResults.Add(targetPos2Result);

            //GetOffset
            var offset = this.GetCurrentOffset("U1");
            SetLogger("U1", $"Vision Offset X : {offset.X} Y : {offset.Y} T : {offset.T}");

            //Run Processor Align Run
            var result = algorithmProcesser.AlignRun4P(eExecuteZoneID.Main1_TARGET, targetResults, offset);

            //SetLogger("Tact", $"align_Target_Tact : {align_Total_sT}");

            //if (!result.IsSuccess)
            //{
            //    SetLogger("U1", "Align Fail");
            //    SetEndBit("U1", result.IsSuccess);
            //    return;
            //}

            //SetXYT("U1", result.RevXYT);

            ////SetEndBit("U1", result.IsSuccess);
            //SetEndBit("U1", true);

            ////light off
            ////lightList.ForEach(x => lightControlManager.LightControllers[controllerData.PortNumber].LightOff(x.Channel));

            //SetViewLogger(result.ZoneID, "Align1", result, eSchedulerCommandKind.Align, eMessageLevelKind.Info, $"Align Success", true);

            //SetLogger("U1", $"X : {result.RevXYT.X} Y: {result.RevXYT.Y} T : {result.RevXYT.T}");
            //SetLogger("U1", "1st Align End");
        }

        private void U1_Calibration1Action()
        {
            if (this._systemState != eSystemState.Auto)
                return;


            //Lower Pre Calibration  / eExecuteZoneID.PRE_B
            var firstResult = new FindResult();
            var secondResult = new FindResult();
            var thirdResult = new FindResult();
            var fourthResult = new FindResult();

            var firstPP = new ProcessPosition(eCameraID.Camera5, eExecuteZoneID.Main1_TARGET, eQuadrant.FirstQuadrant);
            var secondPP = new ProcessPosition(eCameraID.Camera6, eExecuteZoneID.Main1_TARGET, eQuadrant.SecondQuadrant);
            var thirdPP = new ProcessPosition(eCameraID.Camera6, eExecuteZoneID.Main1_TARGET, eQuadrant.ThirdQuadrant);
            var fourthPP = new ProcessPosition(eCameraID.Camera5, eExecuteZoneID.Main1_TARGET, eQuadrant.FourthQuadrant);

            var firstCalPoints = new List<CalPixelPoint>();
            var secondCalPoints = new List<CalPixelPoint>();
            var thirdCalPoints = new List<CalPixelPoint>();
            var fourthCalPoints = new List<CalPixelPoint>();

            var movePitch = 0.5;
            var moveTheta = 0.5;

            //Start Bit
            SetCalibrationStartBit("U1");

            SetLogger("U1", "Calibration Target Start");

            SetRevUVW("U1", new XXY());

            //Move 9Point Command XY
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    SetRevUVW("U1", algorithmProcesser.GetXXYYPosData(new XYT((movePitch - (movePitch * i)), -(movePitch - (movePitch * j))), (int)eExecuteZoneID.Main1_TARGET));

                    if (!MoveCommand("U1", false, false)) return;

                    SetLogger("U1", $"Calibration 1st XY Move Done {i * 3 + j + 1}");

                    firstResult = this.algorithmProcesser.FindRun(firstPP, eFindPos.Display, null, null);
                    secondResult = this.algorithmProcesser.FindRun(secondPP, eFindPos.Display, null, null);

                    if (!firstResult.isSuccess || !secondResult.isSuccess)
                    {
                        SetCalibrationEndBit("U1", false);

                        SetLogger("U1", $"1st Find NG {i * 3 + j + 1}");
                        return;
                    }

                    SetLogger("U1", $"1st Find OK {i * 3 + j + 1}");
                    SetLogger("U1", $"First X : {firstResult.ResultXY.X}, Y : {firstResult.ResultXY.Y}");
                    SetLogger("U1", $"Second X : {secondResult.ResultXY.X}, Y : {secondResult.ResultXY.Y}");

                    //Run Processor Lower Object First, Second
                    firstCalPoints.Add(new CalPixelPoint()
                    {
                        Id = i * 3 + j + 1,
                        CalMode = eCalMode.Axis,
                        PixelXY = firstResult.ResultXY,
                    });

                    secondCalPoints.Add(new CalPixelPoint()
                    {
                        Id = i * 3 + j + 1,
                        CalMode = eCalMode.Axis,
                        PixelXY = secondResult.ResultXY,
                    });
                }
            }

            //Move 5Point Command T
            for (int i = 0; i < 5; i++)
            {
                SetRevUVW("U1", algorithmProcesser.GetXXYYPosData(new XYT(0, 0, moveTheta - (moveTheta / 2) * i), (int)eExecuteZoneID.Main1_TARGET));

                if (!MoveCommand("U1", false, false)) return;

                SetLogger("U3", $"Calibration 1st T Move Done {i + 1}");

                firstResult = this.algorithmProcesser.FindRun(firstPP, eFindPos.Display, null, null);
                secondResult = this.algorithmProcesser.FindRun(secondPP, eFindPos.Display, null, null);

                if (!firstResult.isSuccess || !secondResult.isSuccess)
                {
                    SetCalibrationEndBit("U1", false);

                    SetLogger("U3", $"1st Find NG {i + 1}");
                }

                SetLogger("U3", $"1st Find OK {i + 1}");
                SetLogger("U3", $"X : {firstResult.ResultXY.X}, Y : {firstResult.ResultXY.Y}");
                SetLogger("U3", $"X : {secondResult.ResultXY.X}, Y : {secondResult.ResultXY.Y}");

                //Run Processor Lower Object First, Second
                firstCalPoints.Add(new CalPixelPoint()
                {
                    Id = i + 10,
                    CalMode = eCalMode.Rotation,
                    PixelXY = firstResult.ResultXY,
                });

                secondCalPoints.Add(new CalPixelPoint()
                {
                    Id = i + 10,
                    CalMode = eCalMode.Rotation,
                    PixelXY = secondResult.ResultXY,
                });

            }

            //2nd Move Req

            SetLogger("U1", "Calibration 2nd Move Done");

            //Move 9Point Command XY
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    SetRevUVW("U1", algorithmProcesser.GetXXYYPosData(new XYT(movePitch - (movePitch * i), movePitch - (movePitch * j), 0), (int)eExecuteZoneID.Main1_TARGET));

                    if (!MoveCommand("U3", false, true)) return;

                    SetLogger("U3", $"Calibration 2nd XY Move Done {i * 3 + j + 1}");

                    thirdResult = this.algorithmProcesser.FindRun(thirdPP, eFindPos.Display, null, null);
                    fourthResult = this.algorithmProcesser.FindRun(fourthPP, eFindPos.Display, null, null);

                    if (!thirdResult.isSuccess || !fourthResult.isSuccess)
                    {
                        SetCalibrationEndBit("U3", false);

                        SetLogger("U3", $"2nd Find NG {i * 3 + j + 1}");

                        return;
                    }

                    SetLogger("U3", $"2nd Find OK {i * 3 + j + 1}");
                    SetLogger("U3", $"Third X : {thirdResult.ResultXY.X}, Y : {thirdResult.ResultXY.Y}");
                    SetLogger("U3", $"Fourth X : {fourthResult.ResultXY.X}, Y : {fourthResult.ResultXY.Y}");

                    //Run Processor Lower Object Third, Fourth
                    thirdCalPoints.Add(new CalPixelPoint()
                    {
                        Id = i * 3 + j + 1,
                        CalMode = eCalMode.Axis,
                        PixelXY = thirdResult.ResultXY,
                    });

                    fourthCalPoints.Add(new CalPixelPoint()
                    {
                        Id = i * 3 + j + 1,
                        CalMode = eCalMode.Axis,
                        PixelXY = fourthResult.ResultXY,
                    });

                }
            }

            //Move 5Point Command T
            for (int i = 0; i < 5; i++)
            {
                SetRevUVW("U1", algorithmProcesser.GetXXYYPosData(new XYT(0, 0, moveTheta - (moveTheta / 2) * i), (int)eExecuteZoneID.Main1_TARGET));

                if (!MoveCommand("U3", false, true)) return;

                SetLogger("U3", $"Calibration 2nd T Move Done {i + 1}");

                thirdResult = this.algorithmProcesser.FindRun(thirdPP, eFindPos.Display, null, null);
                fourthResult = this.algorithmProcesser.FindRun(fourthPP, eFindPos.Display, null, null);

                if (!thirdResult.isSuccess || !fourthResult.isSuccess)
                {
                    SetCalibrationEndBit("U1", false);

                    SetLogger("U1", $"2nd Find NG {i + 1}");

                    return;
                }

                SetLogger("U1", $"2nd Find OK {i + 1}");
                SetLogger("U1", $"Third X : {thirdResult.ResultXY.X}, Y : {thirdResult.ResultXY.Y}");
                SetLogger("U1", $"Fourth X : {fourthResult.ResultXY.X}, Y : {fourthResult.ResultXY.Y}");

                //Run Processor Object Third, Fourth
                thirdCalPoints.Add(new CalPixelPoint()
                {
                    Id = i + 10,
                    CalMode = eCalMode.Rotation,
                    PixelXY = thirdResult.ResultXY,
                });

                fourthCalPoints.Add(new CalPixelPoint()
                {
                    Id = i + 10,
                    CalMode = eCalMode.Rotation,
                    PixelXY = fourthResult.ResultXY,
                });

            }

            var firstCalData = algorithmProcesser.CalibrationRun(new ProcessPosition(eCameraID.Camera2, eExecuteZoneID.Main1_TARGET, eQuadrant.FirstQuadrant), firstCalPoints, new XYT(movePitch, movePitch, moveTheta));
            var secondCalData = algorithmProcesser.CalibrationRun(new ProcessPosition(eCameraID.Camera1, eExecuteZoneID.Main1_TARGET, eQuadrant.SecondQuadrant), secondCalPoints, new XYT(movePitch, movePitch, moveTheta));
            var thirdCalData = algorithmProcesser.CalibrationRun(new ProcessPosition(eCameraID.Camera1, eExecuteZoneID.Main1_TARGET, eQuadrant.ThirdQuadrant), thirdCalPoints, new XYT(movePitch, movePitch, moveTheta));
            var fourthCalData = algorithmProcesser.CalibrationRun(new ProcessPosition(eCameraID.Camera2, eExecuteZoneID.Main1_TARGET, eQuadrant.FourthQuadrant), fourthCalPoints, new XYT(movePitch, movePitch, moveTheta));

            //End Bit
            if (!SetCalibrationEndBit("U1", true)) return;

            SetLogger("U3", "Calibration END");
        }

        #endregion
    }
}
