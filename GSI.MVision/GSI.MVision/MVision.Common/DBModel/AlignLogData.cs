using MVision.Common.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DOTNET.VISION.Model;

namespace MVision.Common.DBModel
{
    public class AlignLogData
    {
        public eExecuteZoneID ZoneID { get; set; }

        /// <summary>
        /// GrabNo 같은 Camera 에서 1차 , 2차를 구별하기 위해 사용.
        /// </summary>
        public Shared.eQuadrant Quadrant { get; set; }

        public DateTime CreateDate { get; set; }

        public XYT RevXYT { get; set; }
        public XYT TargetXYT { get; set; }
        public XYT ObjectXYT { get; set; }
        public XYT OffsetXYT { get; set; }
        public double ObjWidthTop { get; set; }
        public double ObjWidthBottom { get; set; }
        public double ObjHeightLeft { get; set; }
        public double ObjHeightRight { get; set; }
        public double TarWidthTop { get; set; }
        public double TarWidthBottom { get; set; }
        public double TarHeightLeft { get; set; }
        public double TarHeightRight { get; set; }
        public bool IsSuccess { get; set; }

        public AlignLogData()
        {
            CreateDate = DateTime.Now;
            Quadrant = Shared.eQuadrant.FirstQuadrant;
            RevXYT = new XYT();
            TargetXYT = new XYT();
            ObjectXYT = new XYT();
            OffsetXYT = new XYT();
        }
    }
}
