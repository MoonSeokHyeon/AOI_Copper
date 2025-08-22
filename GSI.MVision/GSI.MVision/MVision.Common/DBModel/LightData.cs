using MVision.Common.Shared;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.Common.DBModel
{
    public class LightData : BindableBase
    {
        public int modelID;
        public int ModelID
        {
            get { return modelID; }
            set { SetProperty(ref modelID, value); }
        }

        public eExecuteZoneID zoneID;
        public eExecuteZoneID ZoneID
        {
            get { return zoneID; }
            set { SetProperty(ref zoneID, value); }
        }

        public eQuadrant quadrant;
        public eQuadrant Quadrant
        {
            get { return quadrant; }
            set { SetProperty(ref quadrant, value); }
        }

        public int port;
        public int Port
        {
            get { return port; }
            set { SetProperty(ref port, value); }
        }

        public int channel;
        public int Channel
        {
            get { return channel; }
            set { SetProperty(ref channel, value); }
        }

        public int value;
        public int Value
        {
            get { return value; }
            set { SetProperty(ref this.value, value); }
        }

        public bool chUse;
        public bool ChUse
        {
            get { return chUse; }
            set { SetProperty(ref chUse, value); }
        }

        public LightData()
        {
        }
    }
}
