using MVision.Common.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.Common.Model
{
    public class ProcessPosition
    {
        public eCameraID CameraID { get; set; }
        public eExecuteZoneID ExecuteZoneID { get; set; }
        public eQuadrant Quadrant { get; set; }

        public ProcessPosition()
        {
            this.CameraID = eCameraID.Camera1;
            this.ExecuteZoneID = eExecuteZoneID.NONE;
            this.Quadrant = eQuadrant.FirstQuadrant;
        }

        public ProcessPosition(eCameraID camid, eExecuteZoneID zondid, eQuadrant quadid)
        {
            this.CameraID = camid;
            this.ExecuteZoneID = zondid;
            this.Quadrant = quadid;
        }
    }
}
