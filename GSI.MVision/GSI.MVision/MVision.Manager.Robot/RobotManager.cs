using DOTNET.Concurrent;
using DOTNET.Logging;
using DOTNET.QO;
using DOTNET.TCP;
using DOTNET.Utils;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static MVision.Manager.Robot.Config.KukaXMLConfig;
using System.Xml.Linq;
using MVision.Manager.Robot.Config;
using MVision.Common.Event;
using MVision.Common.Interface;
using MVision.Common.Shared;
using Prism.Ioc;
using MVision.MairaDB;
using MVision.Device.Robot;

namespace MVision.Manager.Robot
{
    public class RobotManager
    {
        #region Properties

        IDictionary<eRobot, KukaRobot> robots = new Dictionary<eRobot, KukaRobot>();

        public IDictionary<eRobot, KukaRobot> Robots { get => this.robots; }

        IContainerProvider provider = null;
        MariaManager sql = null;
        SchedulerMessageEvent schedulerEvent = null;


        #endregion


        public RobotManager(IContainerProvider containerProvider, IEventAggregator eventAggregator, MariaManager sql)
        {
            this.provider = containerProvider;
            this.sql = sql;

            eventAggregator.GetEvent<ApplicationExitEvent>().Subscribe((o) => Dispose(), true);

            //eventAggregator.GetEvent<GUIMessageEvent>().Unsubscribe(OnReceivedMessage);
            //eventAggregator.GetEvent<GUIMessageEvent>().Subscribe(OnReceivedMessage, ThreadOption.BackgroundThread);
        }

        public void Dispose()
        {
            foreach (var robot in Robots.Values)
            {
                robot.Dispose();
            }
        }

        public void Init()
        {
            var rll = this.sql.CameraData.All();
            Assert.NotNull(rll, "RobotManager Init - DB Robot Info is Null");

            foreach (var info in rll)
            {
                try
                {
                    var robot = new KukaRobot();
                    robot.Init();

                    this.Robots.Add(eRobot.Robot1, robot);
                }
                catch (Exception ex) { }
            }
        }

    }
}
