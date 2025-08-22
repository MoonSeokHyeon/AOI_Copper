using MVision.Common.Shared;
using MVision.UI.CogDisplayViews.View;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.UI.MainViews.ViewModel
{
    public class MainAutoViewModel : BindableBase
    {
        SingleCamView main1SecondDisplay = null;
        public SingleCamView Main1SecondDisplay { get => this.main1SecondDisplay; set => SetProperty(ref this.main1SecondDisplay, value); }

        SingleCamView main1FirstDisplay = null;
        public SingleCamView Main1FirstDisplay { get => this.main1FirstDisplay; set => SetProperty(ref this.main1FirstDisplay, value); }

        SingleCamView main1ThirdDisplay = null;
        public SingleCamView Main1ThirdDisplay { get => this.main1ThirdDisplay; set => SetProperty(ref this.main1ThirdDisplay, value); }

        SingleCamView main1FourthDisplay = null;
        public SingleCamView Main1FourthDisplay { get => this.main1FourthDisplay; set => SetProperty(ref this.main1FourthDisplay, value); }



        SingleCamView main2SecondDisplay = null;
        public SingleCamView Main2SecondDisplay { get => this.main2SecondDisplay; set => SetProperty(ref this.main2SecondDisplay, value); }

        SingleCamView main2FirstDisplay = null;
        public SingleCamView Main2FirstDisplay { get => this.main2FirstDisplay; set => SetProperty(ref this.main2FirstDisplay, value); }

        SingleCamView main2ThirdDisplay = null;
        public SingleCamView Main2ThirdDisplay { get => this.main2ThirdDisplay; set => SetProperty(ref this.main2ThirdDisplay, value); }

        SingleCamView main2FourthDisplay = null;
        public SingleCamView Main2FourthDisplay { get => this.main2FourthDisplay; set => SetProperty(ref this.main2FourthDisplay, value); }


        IContainerProvider provider = null;
        IRegionManager regionManager = null;
        IEventAggregator eventAggregator = null;

        public MainAutoViewModel(IEventAggregator aggregator, IRegionManager region, IContainerProvider provider)
        {
            this.eventAggregator = aggregator;
            this.regionManager = region;
            this.provider = provider;

            this.Main1FirstDisplay = provider.Resolve<SingleCamView>();
            this.Main1FirstDisplay.ViewModel.ProcessPos.CameraID = eCameraID.Camera2;
            this.Main1FirstDisplay.ViewModel.ProcessPos.ExecuteZoneID = eExecuteZoneID.Main1_TARGET;
            this.Main1FirstDisplay.ViewModel.ProcessPos.Quadrant = eQuadrant.FirstQuadrant;

            this.Main1SecondDisplay = provider.Resolve<SingleCamView>();
            this.Main1SecondDisplay.ViewModel.ProcessPos.CameraID = eCameraID.Camera1;
            this.Main1SecondDisplay.ViewModel.ProcessPos.ExecuteZoneID = eExecuteZoneID.Main1_TARGET;
            this.Main1SecondDisplay.ViewModel.ProcessPos.Quadrant = eQuadrant.SecondQuadrant;

            this.Main1ThirdDisplay = provider.Resolve<SingleCamView>();
            this.Main1ThirdDisplay.ViewModel.ProcessPos.CameraID = eCameraID.Camera2;
            this.Main1ThirdDisplay.ViewModel.ProcessPos.ExecuteZoneID = eExecuteZoneID.Main1_TARGET;
            this.Main1ThirdDisplay.ViewModel.ProcessPos.Quadrant = eQuadrant.ThirdQuadrant;

            this.Main1FourthDisplay = provider.Resolve<SingleCamView>();
            this.Main1FourthDisplay.ViewModel.ProcessPos.CameraID = eCameraID.Camera1;
            this.Main1FourthDisplay.ViewModel.ProcessPos.ExecuteZoneID = eExecuteZoneID.Main1_TARGET;
            this.Main1FourthDisplay.ViewModel.ProcessPos.Quadrant = eQuadrant.FourthQuadrant;


            //this.Main2FirstDisplay = provider.Resolve<SingleCamView>();
            //this.Main2FirstDisplay.ViewModel.ProcessPos.CameraID = eCameraID.Camera2;
            //this.Main2FirstDisplay.ViewModel.ProcessPos.ExecuteZoneID = eExecuteZoneID.Main2_TARGET;
            //this.Main2FirstDisplay.ViewModel.ProcessPos.Quadrant = eQuadrant.FirstQuadrant;

            //this.Main2SecondDisplay = provider.Resolve<SingleCamView>();
            //this.Main2SecondDisplay.ViewModel.ProcessPos.CameraID = eCameraID.Camera1;
            //this.Main2SecondDisplay.ViewModel.ProcessPos.ExecuteZoneID = eExecuteZoneID.Main2_TARGET;
            //this.Main2SecondDisplay.ViewModel.ProcessPos.Quadrant = eQuadrant.SecondQuadrant;

            //this.Main2ThirdDisplay = provider.Resolve<SingleCamView>();
            //this.Main2ThirdDisplay.ViewModel.ProcessPos.CameraID = eCameraID.Camera2;
            //this.Main2ThirdDisplay.ViewModel.ProcessPos.ExecuteZoneID = eExecuteZoneID.Main2_TARGET;
            //this.Main2ThirdDisplay.ViewModel.ProcessPos.Quadrant = eQuadrant.ThirdQuadrant;

            //this.Main2FourthDisplay = provider.Resolve<SingleCamView>();
            //this.Main2FourthDisplay.ViewModel.ProcessPos.CameraID = eCameraID.Camera1;
            //this.Main2FourthDisplay.ViewModel.ProcessPos.ExecuteZoneID = eExecuteZoneID.Main2_TARGET;
            //this.Main2FourthDisplay.ViewModel.ProcessPos.Quadrant = eQuadrant.FourthQuadrant;


        }

        public void Init()
        {
        }
    }
}
