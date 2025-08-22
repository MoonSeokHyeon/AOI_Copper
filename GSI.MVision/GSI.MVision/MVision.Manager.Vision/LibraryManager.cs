using MVision.Common.Event.EventArg;
using MVision.Common.Event;
using MVision.Common.Shared;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVision.VisionLib.Cognex;
using MVision.MairaDB;

namespace MVision.Manager.Vision
{
    public class LibraryManager
    {
        #region Properties

        public IDictionary<eExecuteZoneID, Dictionary<eQuadrant, CognexJob>> CogJobs = null;

        IContainerProvider provider = null;
        MariaManager sql = null;
        SchedulerMessageEvent schedulerEvent = null;
        GUIMessageEvent guiEvent = null;

        #endregion

        #region Construct

        public LibraryManager(IContainerProvider containerProvider, IEventAggregator eventAggregator, MariaManager sql)
        {
            this.provider = containerProvider;
            this.sql = sql;

            eventAggregator.GetEvent<ApplicationExitEvent>().Subscribe((o) => Dispose(), true);

            eventAggregator.GetEvent<GUIMessageEvent>().Unsubscribe(OnReceivedMessage);
            eventAggregator.GetEvent<GUIMessageEvent>().Subscribe(OnReceivedMessage, ThreadOption.BackgroundThread);

            schedulerEvent = eventAggregator.GetEvent<SchedulerMessageEvent>();
            guiEvent = eventAggregator.GetEvent<GUIMessageEvent>();
        }

        public void Dispose()
        {

        }

        #endregion

        #region Event Method

        private void OnReceivedMessage(GUIEventArgs obj)
        {
            switch (obj.MessageKind)
            {
                case eGUIMessageKind.CameraPropertyChanged:
                    break;
            }
        }

        #endregion

        #region Job

        public void CreateJobs()
        {
            var currentModel = sql.MachineData.All().FirstOrDefault();
            var model = sql.ModelData.All().FirstOrDefault(x => x.ModelID.Equals(currentModel.CurrentModelId));

            this.CogJobs = new Dictionary<eExecuteZoneID, Dictionary<eQuadrant, CognexJob>>();

            for (eExecuteZoneID i = eExecuteZoneID.Main1_TARGET; i <= eExecuteZoneID.Main2_OBJECT; i++)
            {
                var dic = new Dictionary<eQuadrant, CognexJob>();

                for (eQuadrant j = eQuadrant.FirstQuadrant; j <= eQuadrant.FourthQuadrant; j++)
                {
                    var job = new CognexJob();

                    job.JobInitialize(i, j, model.ModelName);

                    dic.Add(j, job);
                }

                CogJobs.Add(i, dic);
            }

        }

        #endregion
    }
}
