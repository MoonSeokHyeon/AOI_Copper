using Cognex.VisionPro;
using DOTNET.Concurrent;
using DOTNET.Logging;
using Matrox.MatroxImagingLibrary;
using MVision.Common.DBModel;
using MVision.Common.Interface;
using MVision.Common.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MVision.Device.Camera
{
    public class MatroxCamera : ICamera
    {
        #region Properties

        private MIL_ID milSystem;
        private MIL_ID milDigitizer;
        private MIL_ID milImageBuffer = MIL.M_NULL;
        private MIL_ID milCoefWB_New = MIL.M_NULL;

        private int camNum;
        private string camfilePath;
        private MIL_INT mWidth;
        private MIL_INT mHeight;
        private bool IsGrabCancel;

        public CameraData CameraInfo => this.cameraInfo;

        private int width;
        public int Width { get => this.width; set => this.width = value; }

        private int height;
        public int Height { get => this.height; set => this.height = value; }

        private byte[] grabBuffer { get; set; }
        public byte[] GrabBuffer { get => this.grabBuffer; }

        private bool isConnected { get; set; }
        public bool IsConnected { get => this.isConnected; }

        public bool IsGrabbing { get; private set; } = false;

        public eCameraID CameraID { get; set; }
        CameraData cameraInfo = null;

        public event Action<eCameraID> ConnectionLost;
        public event Action<eCameraID> Connected;
        public event Action<eCameraID> CameraClosed;
        public event Action<byte[], eCameraID> ImageGrabbed;
        public event Action<eCameraID> GrabStarted;
        public event Action<eCameraID> GrabStopped;

        TaskCancel _taskCancel = new TaskCancel();

        #endregion

        public MatroxCamera(CameraData info, MIL_ID systemId, string pixelFormat = "Mono 8")
        {
            this.cameraInfo = info;

            this.milSystem = systemId;
            this.camNum = info.PortId;
            this.CameraID = (eCameraID)info.PortId;

            isConnected = false;
        }

        public bool ChangeCameraParam(CameraData cameraData)
        {
            throw new NotImplementedException();
        }

        public bool Connect()
        {
            try
            {
                MIL.MdigAlloc(milSystem, camNum, camfilePath, MIL.M_DEFAULT, ref milDigitizer);

                MIL.MdigInquire(milDigitizer, MIL.M_SIZE_X, ref mWidth);
                this.Width = (int)mWidth;

                MIL.MdigInquire(milDigitizer, MIL.M_SIZE_Y, ref mHeight);
                this.Height = (int)mHeight;

                MIL_ID sizeBand = MIL.M_NULL;
                MIL.MdigInquire(milDigitizer, MIL.M_SIZE_BAND, ref sizeBand);

                MIL_ID type = MIL.M_NULL;
                MIL.MdigInquire(milDigitizer, MIL.M_TYPE, ref type);

                double ExposureTime = CameraInfo.ExposureTimeAbs;
                MIL.MdigControlFeature(milDigitizer, 16384000, "ExposureTime", MIL.M_TYPE_DOUBLE, ref ExposureTime);

                // Allocate a mono image buffer and display it.
                MIL.MbufAlloc2d(milSystem,
                                MIL.MdigInquire(milDigitizer, MIL.M_SIZE_X),
                                MIL.MdigInquire(milDigitizer, MIL.M_SIZE_Y),
                                8 + MIL.M_UNSIGNED,
                                MIL.M_IMAGE + MIL.M_GRAB,
                                ref milImageBuffer);

                MIL.MbufClear(milImageBuffer, 0x0);

                MIL.MdigControl(milDigitizer, MIL.M_GRAB_SCALE, 1.0);
            }
            catch (MILException e)
            {
                return false;
            }
            catch (Exception e)
            {
                return false;
            }

            isConnected = true;

            return true;
        }

        public bool DestoryCamera()
        {
            throw new NotImplementedException();
        }

        public bool GrabContinuous()
        {
            throw new NotImplementedException();
        }

        public object GrabOneShot()
        {
            try
            {
                // 1. 이미지 획득
                byte[] rawImage = new byte[this.Width * this.Height];
                MIL.MbufGet2d(milImageBuffer, 0, 0, this.Width, this.Height, rawImage);

                // 2. Bitmap 생성 및 그레이 팔레트 설정
                Bitmap bitmap = new Bitmap(this.Width, this.Height, PixelFormat.Format8bppIndexed);

                ColorPalette palette = bitmap.Palette;

                for (int i = 0; i < 256; i++)
                    palette.Entries[i] = Color.FromArgb(i, i, i);
                bitmap.Palette = palette;

                // 3. Bitmap에 rawImage 복사
                BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),ImageLockMode.WriteOnly,PixelFormat.Format8bppIndexed);Marshal.Copy(rawImage, 0, bmpData.Scan0, rawImage.Length);

                bitmap.UnlockBits(bmpData);

                // 4. CogImage8Grey 변환
                var cogImage = new CogImage8Grey(bitmap);

                ICogImage ret = cogImage;
                // 5. 필요 시 콜백 전송
                ImageGrabbed?.Invoke(rawImage, CameraID);

                return ret;
            }
            catch (MILException e)
            {
                return null;
            }
            catch (Exception e)
            {
                return null;
            }

        }

        public bool StopGrabContinuous()
        {
            throw new NotImplementedException();
        }
    }
}
