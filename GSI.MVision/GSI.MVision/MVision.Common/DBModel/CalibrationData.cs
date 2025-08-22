using DOTNET.VISION.Model;
using MVision.Common.Model;
using MVision.Common.Shared;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eQuadrant = MVision.Common.Shared.eQuadrant;

namespace MVision.Common.DBModel
{
    public class CalibrationData : BindableBase
    {
        private int modelID;
        public int ModelID
        {
            get { return modelID; }
            set { SetProperty(ref modelID, value); }
        }

        private eCameraID camId;
        public eCameraID CamId
        {
            get { return camId; }
            set { SetProperty(ref camId, value); }
        }

        private eExecuteZoneID executeZoneId;
        public eExecuteZoneID ExecuteZoneId
        {
            get { return executeZoneId; }
            set { SetProperty(ref executeZoneId, value); }
        }

        private Shared.eQuadrant quadrant;
        public Shared.eQuadrant Quadrant
        {
            get { return quadrant; }
            set { SetProperty(ref quadrant, value); }
        }

        private eFindType findType;
        public eFindType FindType
        {
            get { return findType; }
            set { SetProperty(ref findType, value); }
        }

        private double score;
        public double Score
        {
            get { return score; }
            set { SetProperty(ref score, value); }
        }

        private XY axisCenterXY;
        public XY AxisCenterXY
        {
            get { return axisCenterXY; }
            set { SetProperty(ref axisCenterXY, value); }
        }

        private XY xAxisStartXY;
        public XY XAxisStartXY
        {
            get { return xAxisStartXY; }
            set { SetProperty(ref xAxisStartXY, value); }
        }

        private XY xAxisEndXY;
        public XY XAxisEndXY
        {
            get { return xAxisEndXY; }
            set { SetProperty(ref xAxisEndXY, value); }
        }

        private XY yAxisStartXY;
        public XY YAxisStartXY
        {
            get { return yAxisStartXY; }
            set { SetProperty(ref yAxisStartXY, value); }
        }

        private XY yAxisEndXY;
        public XY YAxisEndXY
        {
            get { return yAxisEndXY; }
            set { SetProperty(ref yAxisEndXY, value); }
        }

        private XY fixPosXY;
        public XY FixPosXY
        {
            get { return fixPosXY; }
            set { SetProperty(ref fixPosXY, value); }
        }

        private XY resolutionXY;
        public XY ResolutionXY
        {
            get { return resolutionXY; }
            set { SetProperty(ref resolutionXY, value); }
        }

        private XY currentPosCamXY;
        public XY CurrentPosCamXY
        {
            get { return currentPosCamXY; }
            set { SetProperty(ref currentPosCamXY, value); }
        }

        private XYT currentRobotPos;
        public XYT CurrentRobotPos
        {
            get { return currentRobotPos; }
            set { SetProperty(ref currentRobotPos, value); }
        }

        private XXYY currentUVRWPos;
        public XXYY CurrentUVRWPos
        {
            get { return currentUVRWPos; }
            set { SetProperty(ref currentUVRWPos, value); }
        }

        private XY basePosXY;
        public XY BasePosXY
        {
            get { return basePosXY; }
            set { SetProperty(ref basePosXY, value); }
        }

        private XY refPosXY1;
        public XY RefPosXY1
        {
            get { return refPosXY1; }
            set { SetProperty(ref refPosXY1, value); }
        }

        private XY refPosXY2;
        public XY RefPosXY2
        {
            get { return refPosXY2; }
            set { SetProperty(ref refPosXY2, value); }
        }

        private XYT movingPitch;
        public XYT MovingPitch
        {
            get { return movingPitch; }
            set { SetProperty(ref movingPitch, value); }
        }
      
        public CalibrationData()
        {
            this.CamId =  eCameraID.None;
            this.ExecuteZoneId = eExecuteZoneID.NONE;
            this.Quadrant = eQuadrant.FirstQuadrant;

            this.AxisCenterXY = new XY();
            this.XAxisStartXY = new XY();
            this.XAxisEndXY = new XY();
            this.YAxisStartXY = new XY();
            this.YAxisEndXY = new XY();
            this.FixPosXY = new XY();
            this.ResolutionXY = new XY();
            this.CurrentPosCamXY = new XY();
            this.CurrentRobotPos = new XYT();
            this.CurrentUVRWPos = new XXYY();
            this.RefPosXY1 = new XY();
            this.RefPosXY2 = new XY();
            this.MovingPitch = new XYT();
            this.BasePosXY = new XY();
        }

        public DOTNET.VISION.Model.CalibrationData ConvertToCalibrationData()
        {
            var retValue = new DOTNET.VISION.Model.CalibrationData()
            {
                AxisCenterXY = new XY(this.AxisCenterXY),
                XAxisStartXY = new XY(this.XAxisStartXY),
                XAxisEndXY = new XY(this.XAxisEndXY),
                YAxisStartXY = new XY(this.YAxisStartXY),
                YAxisEndXY = new XY(this.YAxisEndXY),
                FixPosXY = new XY(this.FixPosXY),
                ResolutionXY = new XY(this.ResolutionXY),
            };

            return retValue;
        }

        public bool ConvertToQuadrantData(DOTNET.VISION.Model.CalibrationData calData)
        {
            if (calData == null) return false;

            this.AxisCenterXY = calData.AxisCenterXY;
            this.XAxisStartXY = calData.XAxisStartXY;
            this.XAxisEndXY = calData.XAxisEndXY;
            this.YAxisStartXY = calData.YAxisStartXY;
            this.YAxisEndXY = calData.YAxisEndXY;
            this.FixPosXY = calData.FixPosXY;
            this.ResolutionXY = calData.ResolutionXY;

            return true;
        }
    }
}
