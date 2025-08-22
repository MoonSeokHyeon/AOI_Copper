using MVision.Common.Shared;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.Common.DBModel
{
    public class CameraData : BindableBase
    {
        public eCameraID camID;
        public eCameraID CamID
        {
            get { return camID; }
            set { SetProperty(ref camID, value); }
        }

        public int boardId;
        public int BoardId
        {
            get { return boardId; }
            set { SetProperty(ref boardId, value); }
        }

        public int portId;
        public int PortId
        {
            get { return portId; }
            set { SetProperty(ref portId, value); }
        }

        public int width;
        public int Width
        {
            get { return width; }
            set { SetProperty(ref width, value); }
        }

        public int height;
        public int Height
        {
            get { return height; }
            set { SetProperty(ref height, value); }
        }

        public string deviceIpAddress;
        public string DeviceIpAddress
        {
            get { return deviceIpAddress; }
            set { SetProperty(ref deviceIpAddress, value); }
        }

        public string serialNumber;
        public string SerialNumber
        {
            get { return serialNumber; }
            set { SetProperty(ref serialNumber, value); }
        }

        public string deviceType;
        public string DeviceType
        {
            get { return deviceType; }
            set { SetProperty(ref deviceType, value); }
        }

        public int digitalShift;
        public int DigitalShift
        {
            get { return digitalShift; }
            set { SetProperty(ref digitalShift, value); }
        }

        public int gainRaw;
        public int GainRaw
        {
            get { return gainRaw; }
            set { SetProperty(ref gainRaw, value); }
        }

        public double exposureTimeAbs;
        public double ExposureTimeAbs
        {
            get { return exposureTimeAbs; }
            set { SetProperty(ref exposureTimeAbs, value); }
        }

        public double gamma;
        public double Gamma
        {
            get { return gamma; }
            set { SetProperty(ref gamma, value); }
        }

        public RotateFlipType imageRotateType;
        public RotateFlipType ImageRotateType
        {
            get { return imageRotateType; }
            set { SetProperty(ref imageRotateType, value); }
        }

        public CameraData()
        {
            Width = 3840;
            Height = 2748;
            DeviceIpAddress = "192.168.0.0";
            SerialNumber = "99999999";
            DeviceType = "GigE";

            DigitalShift = 0;
            GainRaw = 51;
            ExposureTimeAbs = 35000;
            Gamma = 1.0;
        }
    }
}
