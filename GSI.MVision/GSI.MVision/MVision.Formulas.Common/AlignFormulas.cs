using Cognex.VisionPro;
using Cognex.VisionPro.Dimensioning;
using Cognex.VisionPro.PMAlign;
using Cognex.VisionPro.Caliper;
using DOTNET.Logging;
using DOTNET.Utils;
using MVision.Common.DBModel;
using MVision.Common.Event;
using MVision.Common.Event.EventArg;
using MVision.Common.Model;
using MVision.Common.Shared;
using MVision.MairaDB;
using MVision.Manager.Vision;
using MVision.VisionLib.Cognex;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DOTNET.VISION.Model;
using DOTNET.VISION;
using eQuadrant = MVision.Common.Shared.eQuadrant;

namespace MVision.Formulas.Common
{
    public class AlignFormulas
    {
        #region Properties

        Logger logger = Logger.GetLogger();

        IEventAggregator eventAggregator = null;
        MariaManager sql = null;
        CameraManager cameraManager = null;
        LibraryManager libraryManager = null;
        LightControlManager lightControlManager = null;
        CognexVisionPro visionProTools = null;
        CognexJob cognexJob = null;

        protected SchedulerMessageEvent _SchedulerEventPulisher = null;

        #endregion

        #region Construct

        public AlignFormulas(IEventAggregator eventAggregator, MariaManager sql, CameraManager cameraManager, LibraryManager libraryManager, LightControlManager lightControlManager)
        {
            this.eventAggregator = eventAggregator;
            this.sql = sql;
            this.cameraManager = cameraManager;
            this.libraryManager = libraryManager;
            this.lightControlManager = lightControlManager;

            visionProTools = new CognexVisionPro();
            cognexJob = new CognexJob();

            this._SchedulerEventPulisher = this.eventAggregator.GetEvent<SchedulerMessageEvent>();
        }

        #endregion

        #region Publich Method

        public FindResult FindRun(ProcessPosition position, object findPos, object kind, object inpuImage)
        {
            var toolResult = new FindResult();

            var modelData = this.sql.ModelData.All().FirstOrDefault();
            var zoneData = this.sql.CalibrationData.All().FirstOrDefault(x => x.ModelID == modelData.ModelID);
            var pos = CastTo<eFindPos>.From<object>(findPos);

            switch (pos)
            {
                case eFindPos.PatternEdit:
                    var pmNum = CastTo<eMultiPattern>.From<object>(kind);
                    toolResult = FindPattern(position, pmNum, inpuImage);
                    break;
                case eFindPos.CaliperEdit:
                    var lineKind = CastTo<eKindFindLine>.From<object>(kind);
                    toolResult = FindLine(position, lineKind, inpuImage);
                    break;
                case eFindPos.CornerEdit:
                    toolResult = FindCorner(position, inpuImage);
                    break;
                default:
                    break;
            }

            if (!toolResult.isSuccess)
            {
                return null;
            }

            return toolResult;
        }

        public AlignLogData AlignRun4P(eExecuteZoneID objectPos, List<FindResult> objectResults, XYT offset)
        {
            //Algorithm 구현

            var objectCalDatas = sql.CalibrationData.All().ToList().FindAll(x => x.ExecuteZoneId == objectPos);

            var dbObjectXY = new Dictionary<eQuadrant, XY>();

            var currentObjectXY = new Dictionary<eQuadrant, XY>();

            var currentObjectPosXY = new Dictionary<eQuadrant, XY>();

            var currentObjectAver = new Dictionary<eQuadrant, XY>();

            objectResults.ForEach(x =>
            {
                var dbCalData = objectCalDatas.FirstOrDefault(t => t.Quadrant.Equals(x.ProcessPosition.Quadrant));
                var calData = dbCalData.ConvertToCalibrationData();

                var realXY = calData.TransPixelToRealPos(x.ResultXY);
                var refPosXY = calData.TransPixelToRealPos(dbCalData.RefPosXY1);

                var current = dbCalData.FixPosXY;
                var currentPos = new XY(current.X, current.Y);

                dbObjectXY.Add(x.ProcessPosition.Quadrant, refPosXY + currentPos);

                currentObjectXY.Add(x.ProcessPosition.Quadrant, realXY + currentPos);
                currentObjectPosXY.Add(x.ProcessPosition.Quadrant, currentPos);
            });

            //width, Height 계산
            var ObjectTop = VisionMathUtils.GetLength(currentObjectXY[eQuadrant.FirstQuadrant], currentObjectXY[eQuadrant.SecondQuadrant]);
            var ObjectBottom = VisionMathUtils.GetLength(currentObjectXY[eQuadrant.ThirdQuadrant], currentObjectXY[eQuadrant.FourthQuadrant]);
            var ObjectLeft = VisionMathUtils.GetLength(currentObjectXY[eQuadrant.SecondQuadrant], currentObjectXY[eQuadrant.ThirdQuadrant]);
            var ObjectRight = VisionMathUtils.GetLength(currentObjectXY[eQuadrant.FirstQuadrant], currentObjectXY[eQuadrant.FourthQuadrant]);

            var currentObjectUpperTheta = VisionMathUtils.RadianToDegree(VisionMathUtils.GetNomalizeTheta(currentObjectXY[eQuadrant.FirstQuadrant], currentObjectXY[eQuadrant.SecondQuadrant]));
            var currentObjectLowerTheta = VisionMathUtils.RadianToDegree(VisionMathUtils.GetNomalizeTheta(currentObjectXY[eQuadrant.ThirdQuadrant], currentObjectXY[eQuadrant.FourthQuadrant]));
            var currentObjectLeftTheta = VisionMathUtils.RadianToDegree(VisionMathUtils.GetNomalizeTheta(currentObjectXY[eQuadrant.SecondQuadrant], currentObjectXY[eQuadrant.ThirdQuadrant]));
            var currentObjectRightTheta = VisionMathUtils.RadianToDegree(VisionMathUtils.GetNomalizeTheta(currentObjectXY[eQuadrant.FirstQuadrant], currentObjectXY[eQuadrant.FourthQuadrant]));

            var dbObjectUpperTheta = VisionMathUtils.RadianToDegree(VisionMathUtils.GetNomalizeTheta(dbObjectXY[eQuadrant.FirstQuadrant], dbObjectXY[eQuadrant.SecondQuadrant]));
            var dbObjectLowerTheta = VisionMathUtils.RadianToDegree(VisionMathUtils.GetNomalizeTheta(dbObjectXY[eQuadrant.ThirdQuadrant], dbObjectXY[eQuadrant.FourthQuadrant]));
            var dbObjectLeftTheta = VisionMathUtils.RadianToDegree(VisionMathUtils.GetNomalizeTheta(dbObjectXY[eQuadrant.SecondQuadrant], dbObjectXY[eQuadrant.ThirdQuadrant]));
            var dbObjectRightTheta = VisionMathUtils.RadianToDegree(VisionMathUtils.GetNomalizeTheta(dbObjectXY[eQuadrant.FirstQuadrant], dbObjectXY[eQuadrant.FourthQuadrant]));

            var currentobjectTheta = (currentObjectUpperTheta + currentObjectLowerTheta + currentObjectLeftTheta + currentObjectRightTheta) / 4;
            var dbobjectTheta = (dbObjectUpperTheta + dbObjectLowerTheta + dbObjectLeftTheta + dbObjectRightTheta) / 4;

            var objectTheta = (currentobjectTheta) - (dbobjectTheta);

            for (eQuadrant i = eQuadrant.FirstQuadrant; i < eQuadrant.FourthQuadrant + 1; i++)
            {
                currentObjectAver.Add(i, (VisionMathUtils.GetPointMovedTheta(currentObjectXY[i], objectTheta - offset.T) - currentObjectPosXY[i]) - (dbObjectXY[i] - currentObjectPosXY[i]));
            }

            var objectAverXY = (currentObjectAver[eQuadrant.FirstQuadrant] + currentObjectAver[eQuadrant.SecondQuadrant] + currentObjectAver[eQuadrant.ThirdQuadrant] + currentObjectAver[eQuadrant.FourthQuadrant]) / 4;

            //Target - Object
            var retXYT = new XYT(-objectAverXY.X + offset.X, -objectAverXY.Y + offset.Y, -objectTheta + offset.T);

            var alignHistory = new AlignLogData()
            {
                ZoneID = objectPos,
                RevXYT = retXYT,
                ObjectXYT = new XYT(objectAverXY.X, objectAverXY.Y, objectTheta),
                OffsetXYT = offset,
                ObjWidthTop = ObjectTop,
                ObjWidthBottom = ObjectBottom,
                ObjHeightLeft = ObjectLeft,
                ObjHeightRight = ObjectRight,
                IsSuccess = true
            };

            //DB Save
            this.sql.AlignLogData.Add(alignHistory);

            var processPos = new ProcessPosition()
            {
                ExecuteZoneID = eExecuteZoneID.Main1_OBJECT
            };

            //GUI Event Publish
            var meg = new SchedulerEventArgs();
            meg.MessageKind = eSchedulerMessageKind.AddAlignHistory;
            meg.processPosition = processPos;
            meg.Arg = alignHistory;

            _SchedulerEventPulisher.Publish(meg);

            return alignHistory;
        }

        public DOTNET.VISION.Model.CalibrationData CalibrationRun(ProcessPosition processPosition, List<CalPixelPoint> points, XYT MovingValue)
        {
            var calData = sql.CalibrationData.All().FirstOrDefault(x => x.ModelID.Equals(1) && 
                                                                        x.CamId.Equals(processPosition.CameraID) && 
                                                                        x.ExecuteZoneId.Equals(processPosition.ExecuteZoneID) && 
                                                                        x.Quadrant.Equals(processPosition.Quadrant));

            var list = new List<CalPixelPoint>();

            points.ForEach(x =>
            {
                list.Add(new CalPixelPoint()
                {
                    Id = x.Id,
                    CalMode = x.CalMode,
                    PixelXY = x.PixelXY,
                });
            });

            var CenterPoint = list.FirstOrDefault(x => x.Id.Equals(5));
            var calResult = CalibrationAlgorithm.RunCalibration(list, new XYT(), new XY(3840, 2748), MovingValue);
            var baseXY = calResult.TransPixelToRealPos(CenterPoint.PixelXY);

            calData.ConvertToQuadrantData(calResult);
            //calData.CurrentUVRWPos = CurrentUVRW;
            //calData.CurrentPosCamXY = CurrentCamPos;
            //calData.CurrentRobotPos = CurrentRobotPos;
            calData.MovingPitch = MovingValue;
            calData.BasePosXY = baseXY;

            sql.CalibrationData.Update(calData);

            //GUI Event Publish
            var meg = new SchedulerEventArgs();
            meg.MessageKind = eSchedulerMessageKind.CalibrationComplete;
            meg.processPosition = processPosition;
            meg.Arg = calData;

            _SchedulerEventPulisher.Publish(meg);

            return calResult;
        }

        public XXY GetXXYYPosData(XYT xyt, int uvwPos)
        {
            var uvwDB = sql.UVWData.All().FirstOrDefault(x => x.UVWPos.Equals(uvwPos));

            var info = new XXYInfo();
            info.Radius = uvwDB.Radius;
            info.AngleX1 = uvwDB.AngleX1;
            info.AngleX2 = uvwDB.AngleX2;
            info.AngleY1 = uvwDB.AngleY1;
            info.DirR0 = uvwDB.DirR0;
            info.DirX1 = uvwDB.DirX1;
            info.DirX2 = uvwDB.DirX2;
            info.DirY1 = uvwDB.DirY1;

            var XXYUtils = new XXYUtils(info);

            return XXYUtils.GetPos(xyt);
        }


        #endregion

        #region Private Method

        private FindResult FindPattern(ProcessPosition position, eMultiPattern multiPattern, object image)
        {
            var tool = new CogPMAlignTool();

            CogPMAlignResult pmResult = null;
            CogImage8Grey inputImage = null;

            var FindPattern = new FindResult();
            var graphics = new CogGraphicInteractiveCollection();

            //image check
            if (image == null)
                inputImage = (CogImage8Grey)cameraManager.GrabOneShot(position.CameraID);
            else
                inputImage = (CogImage8Grey)image;

            //Image save
            //libraryManager.CogJobs[position.ExecuteZoneID][position.Quadrant].SaveImage((ICogImage)inputImage, $"D:\\LOG\\VAS\\MVision\\ImageLog");  // 경로 db로 변경 예정

            var dbScore = 85;   //db로 만들예정

            var pmAlignDic = libraryManager.CogJobs[position.ExecuteZoneID][position.Quadrant].PMAlignDic;

            foreach (var item in pmAlignDic)
            {
                if (item.Value == null) continue;

                tool = item.Value;
                var result = visionProTools.RunPMAlignTool(tool, inputImage);

                if (result == null) continue;

                if (pmResult == null || result.Score > pmResult.Score)
                    pmResult = result;
            }

            if (pmResult != null && pmResult.Score * 100 >= dbScore)
            {
                FindPattern.isSuccess = true;
                FindPattern.InputImage = inputImage;
                FindPattern.ResultXY = new XY(pmResult.GetPose().TranslationX, pmResult.GetPose().TranslationY);
                FindPattern.Score = pmResult.Score * 100;

                graphics.Add(pmResult.CreateResultGraphics(CogPMAlignResultGraphicConstants.Origin));
                graphics.Add(pmResult.CreateResultGraphics(CogPMAlignResultGraphicConstants.CoordinateAxes));
                graphics.Add(pmResult.CreateResultGraphics(CogPMAlignResultGraphicConstants.MatchFeatures));
                graphics.Add(pmResult.CreateResultGraphics(CogPMAlignResultGraphicConstants.BoundingBox));
                graphics.Add(pmResult.CreateResultGraphics(CogPMAlignResultGraphicConstants.MatchShapeModels));

                var sb = new StringBuilder();

                sb.Append($" X [{Math.Round(pmResult.GetPose().TranslationX, 2)}]   /   Y [{Math.Round(pmResult.GetPose().TranslationY, 2)}]   /   S [{Math.Round(pmResult.Score, 4) * 100}]");
                graphics.Add(visionProTools.CreateLabel(sb.ToString()));


            }//T [{Math.Round(pmResult.GetPose().Rotation, 2)}] 임시제거
            else
            {
                FindPattern.InputImage = inputImage;

                var sb = new StringBuilder();

                sb.Append($"Cannot Find Pattern");
                graphics.Add(visionProTools.CreateLabel(sb.ToString()));

                FindPattern.isSuccess = false;
                FindPattern.errorCode = eErrorCode.MarkNG;
            }

            //final graphics
            FindPattern.ResultGraphic = graphics;

            var meg = new SchedulerEventArgs();
            meg.MessageKind = eSchedulerMessageKind.FindRunProcesser;
            meg.SubKind = eFindPos.Display;
            meg.processPosition = position;
            meg.Arg = FindPattern;

            _SchedulerEventPulisher.Publish(meg);

            return FindPattern;
        }

        private FindResult FindLine(ProcessPosition position, eKindFindLine lineKind, object image)
        {
            var findLine = new FindResult();

            CogImage8Grey inputImage = null;
            var graphics = new CogGraphicInteractiveCollection();

            if (image == null)
                inputImage = (CogImage8Grey)cameraManager.GrabOneShot(position.CameraID);
            else
                inputImage = (CogImage8Grey)image;

            //Image save
            //libraryManager.CogJobs[position.ExecuteZoneID][position.Quadrant].SaveImage((ICogImage)inputImage, ConfigurationManager.AppSettings["SAVE_PATH"]);

            var tool = libraryManager.CogJobs[position.ExecuteZoneID][position.Quadrant].FindLineDic[lineKind];

            var toolResult = visionProTools.RunFineLineTool(tool, inputImage);

            XY resultXY = new XY();

            if (toolResult == null || toolResult.NumPointsFound == 0)
            {
                findLine.isSuccess = false;

                var sb = new StringBuilder();
                sb.Append($"{lineKind.ToString()} - Cannot Find Line");
                graphics.Add(visionProTools.CreateLabelTopLeft(sb.ToString()));

                findLine.ResultGraphic = graphics;

                return findLine;
            }

            for (int i = 0; i < toolResult.Count; i++)
            {
                if (toolResult[i] == null || !toolResult[i].Found) continue;

                findLine.isSuccess = true;
                findLine.InputImage = (CogImage8Grey)inputImage;
                resultXY.X = toolResult[i].X;
                resultXY.Y = toolResult[i].Y;
                findLine.ResultXYs.Add(resultXY);

                graphics.Add(toolResult[i].CreateResultGraphics(CogFindLineResultGraphicConstants.DataPoint));
            }

            var Line = toolResult.GetLine();
            graphics.Add(Line);

            //final graphics
            findLine.ResultGraphic = graphics;

            var meg1 = new SchedulerEventArgs();
            meg1.MessageKind = eSchedulerMessageKind.FindRunProcesser;
            meg1.SubKind = eFindPos.CaliperEdit;
            meg1.processPosition = position;
            meg1.Arg = findLine;

            _SchedulerEventPulisher.Publish(meg1);

            return findLine;
        }

        private FindResult FindCorner(ProcessPosition position, object image)
        {
            var findCorner = new FindResult();
            var graphics = new CogGraphicInteractiveCollection();
            CogImage8Grey inputImage;

            //line tool setting
            var verLinetool = libraryManager.CogJobs[position.ExecuteZoneID][position.Quadrant].FindLineDic[eKindFindLine.VerLine];
            var horLinetool = libraryManager.CogJobs[position.ExecuteZoneID][position.Quadrant].FindLineDic[eKindFindLine.HorLine];

            if (image == null)
                inputImage = (CogImage8Grey)cameraManager.GrabOneShot(position.CameraID);
            else
                inputImage = (CogImage8Grey)image;

            //Image save
            //libraryManager.CogJobs[position.ExecuteZoneID][position.Quadrant].SaveImage((ICogImage)inputImage, ConfigurationManager.AppSettings["SAVE_PATH"]);

            verLinetool.InputImage = inputImage;
            horLinetool.InputImage = inputImage;
            findCorner.InputImage = inputImage;

            var verLineResult = visionProTools.RunFineLineTool(verLinetool, verLinetool.InputImage);
            var horLineResult = visionProTools.RunFineLineTool(horLinetool, horLinetool.InputImage);

            if (verLineResult == null || verLineResult.NumPointsFound == 0)
            {
                //error
                var errorVer = new StringBuilder();

                findCorner.InputImage = inputImage;
                findCorner.isSuccess = false;
                findCorner.errorCode = eErrorCode.LineNG;

                errorVer.Append($" Cannot Find Vertical Line ");
                graphics.Add(visionProTools.CreateLabel(errorVer.ToString()));
                findCorner.ResultGraphic = graphics;

                return findCorner;
            }

            for (int i = 0; i < verLineResult.Count; i++)
            {
                if (verLineResult[i] == null) continue;
                graphics.Add(verLineResult[i].CreateResultGraphics(CogFindLineResultGraphicConstants.DataPoint));
            }

            var verLine = verLineResult.GetLine();
            graphics.Add(verLine);

            if (horLineResult == null || horLineResult.NumPointsFound == 0)
            {
                //error
                var errorHor = new StringBuilder();

                findCorner.InputImage = inputImage;
                findCorner.isSuccess = false;
                findCorner.errorCode = eErrorCode.LineNG;

                errorHor.Append($" Cannot Find Horizontal Line ");
                graphics.Add(visionProTools.CreateLabel(errorHor.ToString()));

                return findCorner;
            }

            for (int i = 0; i < horLineResult.Count; i++)
            {
                if (horLineResult[i] == null) continue;
                graphics.Add(horLineResult[i].CreateResultGraphics(CogFindLineResultGraphicConstants.DataPoint));
            }

            var horLine = horLineResult.GetLine();
            graphics.Add(horLine);

            var crossPoint = visionProTools.GetCrossPoint(verLine, horLine, (CogImage8Grey)inputImage);

            if (crossPoint == null)
            {
                //error
                var errorCross = new StringBuilder();

                findCorner.InputImage = inputImage;
                findCorner.isSuccess = false;
                findCorner.errorCode = eErrorCode.LineNG;

                errorCross.Append($" Cannot Find Cross Point ");
                graphics.Add(visionProTools.CreateLabel(errorCross.ToString()));

                return findCorner;
            }

            findCorner.InputImage = inputImage;
            findCorner.isSuccess = true;
            findCorner.ResultXYT = crossPoint;
            findCorner.ResultXY.X = crossPoint.X;
            findCorner.ResultXY.Y = crossPoint.Y;

            var sb = new StringBuilder();

            sb.Append($" X [{Math.Round(findCorner.ResultXYT.X, 2)}]   /   Y [{Math.Round(findCorner.ResultXYT.Y, 2)}]   /   T [{Math.Round(findCorner.ResultXYT.T, 2)}]");
            graphics.Add(visionProTools.CreateLabel(sb.ToString()));

            //final graphics
            findCorner.ResultGraphic = graphics;

            var meg = new SchedulerEventArgs();
            meg.MessageKind = eSchedulerMessageKind.FindRunProcesser;
            meg.SubKind = eFindPos.CornerEdit;
            meg.processPosition = position;
            meg.Arg = findCorner;

            _SchedulerEventPulisher.Publish(meg);

            return findCorner;
        }

        private FindResult FindFixtureCorner(ProcessPosition position, object inImage)
        {
            var findFixtureCorner = new FindResult();
            CogImage8Grey inputImage;
            CogPMAlignResult pmResult = null;

            var graphics = new CogGraphicInteractiveCollection();

            //line tool setting
            var verLinetool = libraryManager.CogJobs[position.ExecuteZoneID][position.Quadrant].FindLineDic[eKindFindLine.VerLine];
            var horLinetool = libraryManager.CogJobs[position.ExecuteZoneID][position.Quadrant].FindLineDic[eKindFindLine.HorLine];

            if (inImage == null)
                inputImage = (CogImage8Grey)cameraManager.GrabOneShot(position.CameraID);
            else
                inputImage = (CogImage8Grey)inImage;

            //Image save
            //libraryManager.CogJobs[position.ExecuteZoneID][position.Quadrant].SaveImage((ICogImage)inputImage, ConfigurationManager.AppSettings["SAVE_PATH"]);

            //pm tool setting
            findFixtureCorner.InputImage = inputImage;

            var pmAlignDic = libraryManager.CogJobs[position.ExecuteZoneID][position.Quadrant].PMAlignDic;

            foreach (var item in pmAlignDic)
            {
                if (item.Value == null) continue;

                var tool = item.Value;

                var result = visionProTools.RunPMAlignTool(tool, inputImage);

                if (result == null) continue;

                if (pmResult == null || result.Score > pmResult.Score)
                    pmResult = result;
            }

            if (pmResult != null)
            {
                graphics.Add(pmResult.CreateResultGraphics(CogPMAlignResultGraphicConstants.Origin));
                graphics.Add(pmResult.CreateResultGraphics(CogPMAlignResultGraphicConstants.CoordinateAxes));
                graphics.Add(pmResult.CreateResultGraphics(CogPMAlignResultGraphicConstants.MatchFeatures));
                graphics.Add(pmResult.CreateResultGraphics(CogPMAlignResultGraphicConstants.BoundingBox));
                graphics.Add(pmResult.CreateResultGraphics(CogPMAlignResultGraphicConstants.MatchShapeModels));
            }
            else
            {
                //error
                var errorPattern = new StringBuilder();

                findFixtureCorner.InputImage = inputImage;
                findFixtureCorner.isSuccess = false;
                findFixtureCorner.errorCode = eErrorCode.MarkNG;

                errorPattern.Append($" Cannot Find Pattern");
                graphics.Add(visionProTools.CreateLabel(errorPattern.ToString()));

                var megErrorPattern = new SchedulerEventArgs();
                megErrorPattern.MessageKind = eSchedulerMessageKind.FindRunProcesser;
                if (inImage == null) megErrorPattern.SubKind = eFindPos.Display;
                else megErrorPattern.SubKind = eFindPos.ModelEdit;
                megErrorPattern.processPosition = position;
                megErrorPattern.Arg = findFixtureCorner;

                _SchedulerEventPulisher.Publish(megErrorPattern);

                return findFixtureCorner;
            }

            //fixture tool setting
            var fixtureTool = libraryManager.CogJobs[position.ExecuteZoneID][position.Quadrant].fixtureTool;

            var fixtureResult = visionProTools.RunFixtureTool(fixtureTool, inputImage, pmResult);

            if (fixtureResult == null)
            {
                Assert.NotNull(fixtureResult, "Fixture is Not Found");
            }

            var verLineResult = visionProTools.RunFineLineTool(verLinetool, fixtureResult);
            var horLineResult = visionProTools.RunFineLineTool(horLinetool, fixtureResult);

            if (verLineResult == null || verLineResult.NumPointsFound == 0)
            {
                //error
                var errorVer = new StringBuilder();

                findFixtureCorner.InputImage = inputImage;
                findFixtureCorner.isSuccess = false;
                findFixtureCorner.errorCode = eErrorCode.LineNG;

                errorVer.Append($" Cannot Find Vertical Line ");
                graphics.Add(visionProTools.CreateLabel(errorVer.ToString()));

                var megErrorVer = new SchedulerEventArgs();
                megErrorVer.MessageKind = eSchedulerMessageKind.FindRunProcesser;
                if (inImage == null) megErrorVer.SubKind = eFindPos.Display;
                else megErrorVer.SubKind = eFindPos.ModelEdit;
                megErrorVer.processPosition = position;
                megErrorVer.Arg = findFixtureCorner;

                _SchedulerEventPulisher.Publish(megErrorVer);

                return findFixtureCorner;
            }

            for (int i = 0; i < verLineResult.Count; i++)
            {
                if (verLineResult[i] == null) continue;
                graphics.Add(verLineResult[i].CreateResultGraphics(CogFindLineResultGraphicConstants.DataPoint));
            }

            var verLine = verLineResult.GetLine();
            graphics.Add(verLine);

            if (horLineResult == null || horLineResult.NumPointsFound == 0)
            {
                //error
                var errorHor = new StringBuilder();

                findFixtureCorner.InputImage = inputImage;
                findFixtureCorner.isSuccess = false;
                findFixtureCorner.errorCode = eErrorCode.LineNG;

                errorHor.Append($" Cannot Find Horizontal Line ");
                graphics.Add(visionProTools.CreateLabel(errorHor.ToString()));

                var megErrorHor = new SchedulerEventArgs();
                megErrorHor.MessageKind = eSchedulerMessageKind.FindRunProcesser;
                if (inImage == null) megErrorHor.SubKind = eFindPos.Display;
                else megErrorHor.SubKind = eFindPos.ModelEdit;
                megErrorHor.processPosition = position;
                megErrorHor.Arg = findFixtureCorner;

                _SchedulerEventPulisher.Publish(megErrorHor);

                return findFixtureCorner;
            }

            for (int i = 0; i < horLineResult.Count; i++)
            {
                if (horLineResult[i] == null) continue;
                graphics.Add(horLineResult[i].CreateResultGraphics(CogFindLineResultGraphicConstants.DataPoint));
            }

            var horLine = horLineResult.GetLine();
            graphics.Add(horLine);

            var crossPoint = visionProTools.GetCrossPoint(verLine, horLine, fixtureTool, true);

            if (crossPoint == null)
            {
                //error
                var errorCross = new StringBuilder();

                findFixtureCorner.InputImage = inputImage;
                findFixtureCorner.isSuccess = false;
                findFixtureCorner.errorCode = eErrorCode.LineNG;

                errorCross.Append($" Cannot Find Cross Point ");
                graphics.Add(visionProTools.CreateLabel(errorCross.ToString()));

                var megErrorCross = new SchedulerEventArgs();
                megErrorCross.MessageKind = eSchedulerMessageKind.FindRunProcesser;
                if (inImage == null) megErrorCross.SubKind = eFindPos.Display;
                else megErrorCross.SubKind = eFindPos.ModelEdit;
                megErrorCross.processPosition = position;
                megErrorCross.Arg = findFixtureCorner;

                _SchedulerEventPulisher.Publish(megErrorCross);

                return findFixtureCorner;
            }

            var sb = new StringBuilder();

            findFixtureCorner.InputImage = inputImage;
            findFixtureCorner.isSuccess = true;
            findFixtureCorner.ResultXYT = crossPoint;

            sb.Append($" X [{Math.Round(findFixtureCorner.ResultXYT.X, 2)}]   /   Y [{Math.Round(findFixtureCorner.ResultXYT.Y, 2)}]   /   T [{Math.Round(findFixtureCorner.ResultXYT.T, 2)}]");
            graphics.Add(visionProTools.CreateLabel(sb.ToString()));

            findFixtureCorner.ResultGraphic = graphics;

            var meg = new SchedulerEventArgs();
            meg.MessageKind = eSchedulerMessageKind.FindRunProcesser;
            if (inImage == null) meg.SubKind = eFindPos.Display;
            else meg.SubKind = eFindPos.ModelEdit;
            meg.processPosition = position;
            meg.Arg = findFixtureCorner;

            _SchedulerEventPulisher.Publish(meg);

            return findFixtureCorner;
        }

        //private XY TransCoordinatesPixelToCal(XY pixel, DOTNET.VISION.Model.CalibrationData calData)
        //{
        //    XY retXY = new XY();

        //    if (pixel == null || calData == null) return retXY;

        //    retXY.X = VisionMathUtils.GetCrossDistance(pixel, calData.YAxisStartXY, calData.YAxisEndXY);
        //    retXY.Y = VisionMathUtils.GetCrossDistance(pixel, calData.XAxisStartXY, calData.XAxisEndXY);

        //    retXY = retXY * calData.ResolutionXY;

        //    return retXY;
        //}


        #endregion

    }

}