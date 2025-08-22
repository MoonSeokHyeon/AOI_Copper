using MVision.Common.DBModel;
using MVision.Common.Model;
using MVision.Common.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.Common.Interface
{
    public interface ICamera
    {
        event Action<eCameraID> ConnectionLost;
        event Action<eCameraID> Connected;
        event Action<eCameraID> CameraClosed;
        event Action<byte[], eCameraID> ImageGrabbed;
        event Action<eCameraID> GrabStarted;
        event Action<eCameraID> GrabStopped;

        int Width { get; }
        int Height { get; }
        byte[] GrabBuffer { get; }

        CameraData CameraInfo { get; }
        eCameraID CameraID { get; }
        bool IsConnected { get; }
        bool IsGrabbing { get; }
        bool DestoryCamera();
        object GrabOneShot();
        bool GrabContinuous();
        bool StopGrabContinuous();
        bool Connect();
        bool ChangeCameraParam(CameraData cameraData);
    }
}
