using DOTNET.Utils;
using Matrox.MatroxImagingLibrary;
using MVision.Common.Event;
using MVision.Common.Event.EventArg;
using MVision.Common.Interface;
using MVision.Common.Shared;
using MVision.Device.Camera;
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
    public class CameraManager
    {
        #region Properties

        IDictionary<eCameraID, ICamera> cameras = new Dictionary<eCameraID, ICamera>();

        public IDictionary<eCameraID, ICamera> Cameras { get => this.cameras; }

        IContainerProvider provider = null;
        MariaManager sql = null;
        SchedulerMessageEvent schedulerEvent = null;
        GUIMessageEvent guiEvent = null;

        private MIL_ID milApplication = MIL.M_NULL;

        #endregion

        #region Construct

        public CameraManager(IContainerProvider containerProvider, IEventAggregator eventAggregator, MariaManager sql)
        {
            this.provider = containerProvider;
            this.sql = sql;

            eventAggregator.GetEvent<ApplicationExitEvent>().Subscribe((o) => Dispose(), true);

            eventAggregator.GetEvent<GUIMessageEvent>().Unsubscribe(OnReceivedMessage);
            eventAggregator.GetEvent<GUIMessageEvent>().Subscribe(OnReceivedMessage, ThreadOption.BackgroundThread);

            schedulerEvent = eventAggregator.GetEvent<SchedulerMessageEvent>();
            guiEvent = eventAggregator.GetEvent<GUIMessageEvent>();
        }

        private void Dispose()
        {
            foreach (var cam in Cameras.Values)
            {
                cam.DestoryCamera();
            }
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

        #region Camera

        public void CreateCameras()
        {
            var cll = this.sql.CameraData.All();
            Assert.NotNull(cll, "CamaraManager Init - DB Camera Info is Null");

            foreach (var info in cll)
            {
                if(info.deviceType == "MatroxCXP")
                {
                    try
                    {
                        //mil app open
                        MIL.MappAlloc(MIL.M_DEFAULT, ref milApplication);
                        MIL.MappControl(MIL.M_ERROR, MIL.M_THROW_EXCEPTION);

                        // board name 
                        StringBuilder boardName = new StringBuilder();
                        MIL.MappInquire(MIL.M_INSTALLED_SYSTEM_DESCRIPTOR, boardName);

                        // board alloc
                        var boardDic = new Dictionary<int, MIL_ID>();
                        var camsBoardGroup = sql.CameraData.All().GroupBy(_ => _.BoardId);
                        foreach (var item in camsBoardGroup)
                        {
                            MIL_ID board = MIL.M_NULL;
                            MIL.MsysAlloc("M_DEFAULT", item.Key, MIL.M_DEFAULT, ref board);
                            foreach (var c in item.ToList())
                            {
                                var cam = new MatroxCamera(c, board);
                                this.cameras.Add(c.CamID, cam);
                                cam.Connect();
                            }
                        }
                    }

                    catch (Exception ex) { }
                }
                else if (info.deviceType == "GigE")
                {
                    try
                    {
                        var cam = new CognexCamera(info);
                        cam.Connect();

                        if (!this.Cameras.ContainsKey(info.CamID))
                            this.Cameras.Add(info.CamID, cam);
                    }

                    catch (Exception ex) { }
                }
      
            }

            foreach (var cam in this.Cameras)
            {

            }
        }

        public object GrabOneShot(eCameraID camID)
        {
            var grabDelay = int.Parse(this.sql.SystemSettingData.All().FirstOrDefault(x => x.Name.Equals(ConstAutoString.GrabDelayTime)).Value);

            DOTNET.Concurrent.LockUtils.Wait(grabDelay);

            var cam = this.Cameras[camID];

            return cam.GrabOneShot();
        }

        #endregion
    }
}
