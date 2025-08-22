using Cognex.VisionPro;
using Cognex.VisionPro.ImageProcessing;
using DOTNET.Concurrent;
using DOTNET.Logging;
using DOTNET.Utils;
using MVision.Common.DBModel;
using MVision.Common.Interface;
using MVision.Common.Model;
using MVision.Common.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.Device.Camera
{
    public class CognexCamera : ICamera
    {
        #region Properties

        Logger logger = Logger.GetLogger();

        private ICogAcqFifo fifo = null;
        public ICogAcqFifo Fifo
        {
            get { return fifo; }
        }

        public bool IsConnected { get; set; }
        public int Width { get => this.cameraInfo.Width; }
        public int Height { get => this.cameraInfo.Height; }
        public byte[] GrabBuffer => throw new System.NotImplementedException();

        public CameraData CameraInfo => this.cameraInfo;

        public eCameraID CameraID => this.cameraInfo.CamID;

        public bool IsGrabbing { get; private set; } = false;

        CameraData cameraInfo = null;

        public event Action<eCameraID> ConnectionLost;
        public event Action<eCameraID> Connected;
        public event Action<eCameraID> CameraClosed;
        public event Action<byte[], eCameraID> ImageGrabbed;
        public event Action<eCameraID> GrabStarted;
        public event Action<eCameraID> GrabStopped;
        public event Action<ICogImage> CogImageGrabbed;

        TaskCancel _taskCancel = new TaskCancel();
        #endregion

        #region Construct

        public CognexCamera(CameraData info)
        {
            cameraInfo = info;
        }

        #endregion

        #region Private Method

        bool ConnectCamera(string SerialNo)
        {
            var frameGrabbers = new CogFrameGrabbers();

            try
            {
                var grabber = frameGrabbers.Cast<ICogFrameGrabber>().FirstOrDefault(x => x.SerialNumber.Equals(SerialNo));
                if (grabber == null) return false;

                this.fifo = grabber.CreateAcqFifo("Generic GigEVision (Mono)", CogAcqFifoPixelFormatConstants.Format8Grey, 0, true);
                if (this.fifo == null)
                    this.IsConnected = false;
                else
                {
                    ChangeCameraParam(cameraInfo);
                }

                logger.I($"Camera {this.cameraInfo.CamID} - {this.cameraInfo.SerialNumber} Conected !!!");
            }
            catch (System.Exception ex)
            {
                this.fifo = null;
                return false;
            }
            this.IsConnected = true;

            return true;
        }

        void DisconnectCamera()
        {
            if (this.fifo != null) this.Fifo.FrameGrabber.Disconnect(false);
        }

        ICogImage GrabImageCamera()
        {
            int trigger = 0;
            ICogImage grabImage;

            if (fifo != null)
            {
                try
                {
                    grabImage = fifo.Acquire(out trigger);
                }
                catch (Exception e)
                {
                    logger.E($"Camera Grab Err - {e}");
                    return null;
                }
            }
            else
            {
                return null;
            }

            var rotate = (CogIPOneImageFlipRotateOperationConstants)this.cameraInfo.ImageRotateType;

            var IpOneImageTool = new CogIPOneImageTool();
            IpOneImageTool.Operators.Add(new CogIPOneImageFlipRotate() { OperationInPixelSpace = rotate });
            IpOneImageTool.InputImage = grabImage;
            IpOneImageTool.Run();

            IpOneImageTool.OutputImage.PixelFromRootTransform = new CogTransform2DLinear(); //! 회전 후 이미지 좌상단을 0, 0으로 다시 만든다.
            ICogImage ret = IpOneImageTool.OutputImage;
            IpOneImageTool.Dispose();

            return ret;
        }

        #endregion

        #region Public Method

        public bool Connect()
        {
            return this.ConnectCamera(this.cameraInfo.SerialNumber);
        }

        public bool DestoryCamera()
        {
            this.DisconnectCamera();
            return true;
        }

        public object GrabOneShot()
        {
            return this.GrabImageCamera();
        }
        public bool GrabContinuous()
        {
            var task = Task4.Go(() =>
            {
                this.IsGrabbing = true;
                this.GrabStarted?.Invoke(this.CameraID);
                while (!_taskCancel.Canceled)
                {
                    LockUtils.Wait(5);
                    try
                    {
                        var image = this.GrabImageCamera();
                        DlgUtils.Invoke(CogImageGrabbed, image);
                    }
                    catch (Exception ex)
                    {
                        logger.E($"Camera {this.CameraID} - Continuous Grab Exception [{ex}]");
                    }
                }
                this.IsGrabbing = false;
                this.GrabStopped?.Invoke(this.CameraID);
            });
            _taskCancel.Add(task);

            return true;
        }

        public bool StopGrabContinuous()
        {
            _taskCancel.Cancel();
            _taskCancel.WaitAll();

            return true;
        }

        public bool ChangeCameraParam(CameraData cameraData)
        {
            if (this.Fifo == null) return false;

            try
            {
                //this.Fifo.OwnedBrightnessParams.Brightness = cameraData.Brightness;
                this.Fifo.OwnedExposureParams.Exposure = cameraData.ExposureTimeAbs / 1000;
                //this.Fifo.OwnedContrastParams.Contrast = cameraData.Contrast;
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
