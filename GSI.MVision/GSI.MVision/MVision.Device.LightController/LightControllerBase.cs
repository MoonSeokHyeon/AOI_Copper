using DOTNET.IO;
using DOTNET.Logging;
using DOTNET.Utils;
using MVision.Common.DBModel;
using MVision.Common.Interface;
using MVision.Common.Shared;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.Device.LightController
{
    public abstract class LightControllerBase : ILightController
    {
        static Logger logger = Logger.GetLogger();

        internal SerialComm h = new SerialComm();
        public LightControllerData Config { get; set; }
        public virtual int[] ChennelValue { get; set; }

        abstract protected void WriteData(int chanel, int val);
        abstract protected int ReadData(int chanel);

        protected virtual void Open()
        {
            Assert.AreNotEqual(0, this.Config.Port, "PortNo Not Set");

            h.PortName = "COM" + this.Config.Port;
            h.BaudRate = this.Config.BaudRate;
            h.DataBits = this.Config.DataBits;
            h.Parity = (Parity)this.Config.Parity;
            h.StopBits = (StopBits)this.Config.StopBits;
            h.ReadTimeout = 5000;

            h.Open();
        }

        protected virtual void Close() => h.Close();

        public LightControllerBase(LightControllerData config)
        {
            this.Config = config;
        }

        public LightControllerBase(int portNo)
        {
            this.Config = new LightControllerData()
            {
                Port = portNo,
                BaudRate = 9600,
                DataBits = 8,
                Parity = (int)System.IO.Ports.Parity.Even,
                StopBits = (int)System.IO.Ports.StopBits.One,
                MaxChannel = 8,
                MaxVolume = 255,
            };
        }

        public bool LightOffAllChanel()
        {
            bool result = true;

            for (int i = 0; i < (int)Config.MaxChannel; i++)
            {
                if (!this.LightOff(i))
                {
                    result = false;
                }
            }

            return result;
        }

        public bool LightOff(int chanel)
        {
            bool result = false;
            try
            {
                this.WriteData(chanel, 1);
                result = true;
            }
            catch (System.Exception ex)
            {
                this.Close();

                logger.E($"Light Off Error - Chanel {chanel}");
                logger.E(ex);
            }

            return result;
        }

        public bool LightOn(int chanel, int val)
        {
            bool result = false;
            try
            {
                this.WriteData(chanel, val);
                result = true;
            }
            catch (System.Exception ex)
            {
                this.Close();

                logger.E($"Light On Error - Chanel {chanel}");
                logger.E(ex);
            }

            return result;
        }

        public int GetChanelValue(int chanel)
        {
            int ret = 0;

            return ret;
        }
    }
}
