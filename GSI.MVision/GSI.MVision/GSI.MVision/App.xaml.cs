using DOTNET.Logging;
using DOTNET.PLC.SLMP;
using DOTNET.PLC.XGT;
using DOTNET.Quartz;
using DOTNET.Utils;
using DOTNET.WPF;
using MVision.Formulas.Common;
using MVision.MairaDB;
using MVision.Manager.Robot;
using MVision.Manager.Vision;
using MVision.Scheduler.Defualt;
using MVision.UI.CogDisplayViews;
using MVision.UI.CogToolEditViews;
using MVision.UI.InteractivityViews;
using MVision.UI.InterfaceViews;
using MVision.UI.LogViews;
using MVision.UI.MainViews;
using MVision.UI.OptionViews;
using NPOI.SS.Formula.Functions;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GSI.MVision
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Prism.DryIoc.PrismApplication
    {
        Logger logger = Logger.GetLogger();
        //public static ISplashScreen splashScreen;
        private ManualResetEvent resetSplashCreated;
        private Thread splashThread;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (!ProcessUtils.IsOnlyOneInstance)
            {
                MessageBox.Show("Program is already running.");
                this.Shutdown();
                return;
            }

            AppUtils.LogGlobalException();

            AppDomainUtils.SetExceptionHanding();

            try
            {
                LogUtils.Configure("Config/MVision_log4net.xml");
                QuartzUtils.Init(5);


                logger.I(string.Format(string.Empty.PadRight(45, '+') + $"  Ver. {AssemblyUtils.GetVersion()}  " + string.Empty.PadRight(45, '+')));

            }

            catch (Exception ex)
            {
                //logger.E(ex);
                App.Current.Shutdown();
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
           
        }

        protected override Window CreateShell() => Container.Resolve<Shell>();

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance<IContainerProvider>(this.Container);
            containerRegistry.RegisterInstance<IContainerRegistry>(containerRegistry);

            if (!containerRegistry.IsRegistered<SlmpManager>())
                containerRegistry.RegisterSingleton<SlmpManager>();

            if (!containerRegistry.IsRegistered<RobotManager>())
                containerRegistry.RegisterSingleton<RobotManager>();

            containerRegistry.RegisterSingleton<MariaManager>();
            var sql = this.Container.Resolve<MariaManager>();
            sql.Init();

            containerRegistry.RegisterSingleton<CameraManager>();
            var cc = this.Container.Resolve<CameraManager>();
            cc.CreateCameras();

            containerRegistry.RegisterSingleton<LightControlManager>();
            var lc = this.Container.Resolve<LightControlManager>();
            lc.CreateLightController();

            containerRegistry.RegisterSingleton<LibraryManager>();
            var jj = this.Container.Resolve<LibraryManager>();
            jj.CreateJobs();

            containerRegistry.RegisterSingleton<DefualtScheduler>();
            var scheduler = this.Container.Resolve<DefualtScheduler>();
            scheduler.Init();

            //containerRegistry.RegisterSingleton<CCIEScheduler>();
            //var scheduler = this.Container.Resolve<CCIEScheduler>();
            //scheduler.Init();

            containerRegistry.RegisterSingleton<AlignFormulas>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule(typeof(InteractivityViewModule));
            moduleCatalog.AddModule(typeof(LogViewModule));
            moduleCatalog.AddModule(typeof(OptionViewModule));
            moduleCatalog.AddModule(typeof(CogDisplayViewModule));
            moduleCatalog.AddModule(typeof(InterfaceViewModule));
            moduleCatalog.AddModule(typeof(CogToolEditViewModule));
            moduleCatalog.AddModule(typeof(MainViewsModule));
        }
    }
}
