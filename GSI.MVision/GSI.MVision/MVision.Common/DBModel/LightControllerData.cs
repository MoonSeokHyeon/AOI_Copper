using MVision.Common.Shared;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.Common.DBModel
{
    public class LightControllerData : BindableBase
    {
        public int port;
        public int Port
        {
            get { return port; }
            set { SetProperty(ref port, value); }
        }

        public int baudRate;
        public int BaudRate
        {
            get { return baudRate; }
            set { SetProperty(ref baudRate, value); }
        }

        public int parity;
        public int Parity
        {
            get { return parity; }
            set { SetProperty(ref parity, value); }
        }

        public int dataBits;
        public int DataBits
        {
            get { return dataBits; }
            set { SetProperty(ref dataBits, value); }
        }

        public int stopBits;
        public int StopBits
        {
            get { return stopBits; }
            set { SetProperty(ref stopBits, value); }
        }

        public int maxChannel;
        public int MaxChannel
        {
            get { return maxChannel; }
            set { SetProperty(ref maxChannel, value); }
        }

        public int maxVolume;
        public int MaxVolume
        {
            get { return maxVolume; }
            set { SetProperty(ref maxVolume, value); }
        }
        public LightControllerData()
        {

        }
    }
}
