using DOTNET.Logging;
using DOTNET.Utils;
using MVision.Common.DBModel;
using MVision.Common.Event;
using MVision.Common.Interface;
using MVision.Common.Shared;
using MVision.Device.LightController.LightController;
using MVision.MairaDB;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.Manager.Vision
{
    public class LightControlManager : IDisposable
    {
        Logger logger = Logger.GetLogger(typeof(LightControlManager));

        IContainerProvider provider;
        MariaManager sql = null;

        public IDictionary<int, ILightController> LightControllers = new Dictionary<int, ILightController>();
        //public IList<LightValueConfig> Lights = new List<LightValueConfig>();

        public LightControlManager(IContainerProvider containerProvider, IEventAggregator eventAggregator, MariaManager sql)
        {
            this.provider = containerProvider;
            this.sql = sql;

            eventAggregator.GetEvent<ApplicationExitEvent>().Subscribe((o) => Dispose(), true);
        }
        public void CreateLightController()
        {
            var lightList = sql.LightControllerData.All().ToList();

            Assert.NotNull(lightList, "LightController Manager Init - DB LightController Info is Null");

            var ll = lightList.OrderBy(x => x.Port).ToList();

            //TODO : Light Controller DB & Hardware Check

            ll.ForEach(c =>
            {
                var lightConfig = new LightControllerData();

                lightConfig.Port = c.Port;
                lightConfig.BaudRate = c.BaudRate;
                lightConfig.Parity = c.Parity;
                lightConfig.DataBits = c.DataBits;
                lightConfig.StopBits = c.StopBits;
                lightConfig.MaxChannel = c.MaxChannel;
                lightConfig.MaxVolume = c.MaxVolume;

                var lc = new ALT(lightConfig);
                this.LightControllers.Add((int)lightConfig.Port, lc);

                logger.I($"LightController Port {lightConfig.Port} is Conected !!!");
            });
        }

        /// <summary>
        /// Model Change 이 후 한번은 실행해야 함.
        /// Initialization Controller Data
        /// Model Light Value Write To Controller
        /// </summary>
        public void InitControllerValue()
        {

        }

        public bool SetLightValue(int portNumner, int chnnel, int value)
        {
            //var light = this.Lights.FirstOrDefault(l => l.ZoneID == zoneID && l.GrabPos == grabPos);
            var lightData = this.LightControllers[portNumner];
            Assert.NotNull(lightData, "controller is null");

            return lightData.LightOn(chnnel, value);
        }

        public bool SetLightOff(int portNumner, int chnnel)
        {
            //var light = this.Lights.FirstOrDefault(l => l.ZoneID == zoneID && l.GrabPos == grabPos);
            var lightData = this.LightControllers[portNumner];
            Assert.NotNull(lightData, "controller is null");

            return lightData.LightOff(chnnel);
        }

        public void Dispose()
        {

        }
    }
}
