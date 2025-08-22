using DOTNET.VISION.Model;
using MVision.Common.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.Common.Model
{
    public class FindResult
    {
        public ProcessPosition ProcessPosition { get; set; }
        public XY ResultXY { get; set; }
        public List<XY> ResultXYs { get; set; }
        public XYT ResultXYT { get; set; }
        public bool isSuccess { get; set; }
        public object InputImage { get; set; }
        public object ResultGraphic { get; set; }
        public eErrorCode errorCode { get; set; }
        public double Score { get; set; }
        public double DistanceX { get; set; }
        public double DistanceY { get; set; }

        public FindResult()
        {
            this.ResultXY = new XY();
            this.ResultXYs = new List<XY>();
            this.ResultXYT = new XYT();
        }
    }
}
