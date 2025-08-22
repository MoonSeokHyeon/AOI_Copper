using DOTNET.Concurrent;
using MaterialDesignThemes.Wpf;
using MVision.Common.DBModel;
using MVision.Common.Event;
using MVision.Common.Event.EventArg;
using MVision.Common.Shared;
using MVision.MairaDB;
using MVision.UI.InteractivityViews.View;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVision.UI.InteractivityViews.ViewModel
{
    public class ModelManagerViewModel : BindableBase
    {
        #region properties

        private List<ModelData> dataList;
        public List<ModelData> DataList
        {
            get { return dataList; }
            set { SetProperty(ref this.dataList, value); }
        }

        private ModelData selectedModel;
        public ModelData SelectedModel
        {
            get { return selectedModel; }
            set { SetProperty(ref this.selectedModel, value); }
        }

        private bool isSaving;
        public bool IsSaving { get => this.isSaving; set => SetProperty(ref this.isSaving, value); }

        MariaManager sql = null;
        IContainerProvider provider = null;
        GUIMessageEvent guiEventPublisher = null;

        #endregion

        #region ICommand

        public ICommand CloseCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand CreateModelCommand { get; set; }
        public ICommand EditModelCommand { get; set; }
        public ICommand DeleteModelCommand { get; set; }

        #endregion

        #region Construct

        public ModelManagerViewModel(MariaManager sql, IContainerProvider provider, IEventAggregator ea)
        {
            this.sql = sql;
            this.provider = provider;

            this.SaveCommand = new DelegateCommand(ExecuteSaveCommand);
            this.CloseCommand = new DelegateCommand(ExecuteCloseCommand);

            //this.CreateModelCommand = new DelegateCommand(ExecuteCreateCommand);
            this.EditModelCommand = new DelegateCommand(ExecuteEditCommand);
            this.DeleteModelCommand = new DelegateCommand(ExecuteDeleteCommand);

            guiEventPublisher = ea.GetEvent<GUIMessageEvent>();
        }

        public void Init()
        {
            this.DataList = this.sql.ModelData.All().ToList();
            this.SelectedModel = this.DataList.FirstOrDefault();
        }

        #endregion

        #region private Method

        private async void ExecuteDeleteCommand()
        {
            if (selectedModel.ModelID != sql.MachineData.All().FirstOrDefault().CurrentModelId)
            {
                var view = this.provider.Resolve<ComfirmationView>();
                view.ViewModel.Message = $"Do you want to delete Model Name '{SelectedModel.ModelName}' ???";

                if (!DialogHost.IsDialogOpen("ModelManagerDialog"))
                {
                    var result = await DialogHost.Show(view, "ModelManagerDialog") as bool?;

                    if (result == false) return;

                    var deleteModel = this.SelectedModel;
                    sql.ModelData.Delete(this.SelectedModel);

                    var dir = new DirectoryInfo($@"D:\Data\{deleteModel.ModelName}");

                    if (dir != null)
                    {
                        dir.Attributes = FileAttributes.Normal;
                        dir.Delete(true);

                    }

                    this.DataList = this.sql.ModelData.All().ToList();
                    this.SelectedModel = this.DataList.FirstOrDefault();

                    var msg = new GUIEventArgs()
                    {
                        MessageKind = eGUIMessageKind.ChangeModelList,
                    };

                    guiEventPublisher.Publish(msg);
                }
            }
            else
            {
                var view = this.provider.Resolve<NotificationView>();
                view.ViewModel.Message = "Cannot Delete Current Model !!!";

                if (!DialogHost.IsDialogOpen("ModelManagerDialog"))
                    await DialogHost.Show(view, "ModelManagerDialog");

                return;
            }

        }

        private void ExecuteEditCommand()
        {
            var model = this.SelectedModel;
        }

        //private async void ExecuteCreateCommand()
        //{
        //    var view = provider.Resolve<ValueInputView>();
        //    view.ViewModel.InputValueName = "Insert New Model Name";

        //    if (!DialogHost.IsDialogOpen("ModelManagerDialog"))
        //    {
        //        var result = await DialogHost.Show(view, "ModelManagerDialog") as bool?;
        //        if (result == true)
        //        {
        //            var newModel = new ModelData();
        //            newModel.ModelName = view.ViewModel.InputValue.Trim();

        //            newModel.QuadrantDatas = new List<quad>();
        //            this.SelectedModel.QuadrantDatas.ForEach(c =>
        //            {
        //                var newZoneData = new QuadrantData()
        //                {
        //                    processPosition = c.processPosition,
        //                    FindType = c.FindType,
        //                    Score = c.Score,
        //                    AxisCenterXY = c.AxisCenterXY,
        //                    XAxisStartXY = c.XAxisStartXY,
        //                    XAxisEndXY = c.XAxisEndXY,
        //                    YAxisStartXY = c.YAxisStartXY,
        //                    YAxisEndXY = c.YAxisEndXY,
        //                    CamAxisCenterXY = c.CamAxisCenterXY,
        //                    CamXAxisStartXY = c.CamXAxisStartXY,
        //                    CamXAxisEndXY = c.CamXAxisEndXY,
        //                    CamYAxisStartXY = c.CamYAxisStartXY,
        //                    CamYAxisEndXY = c.CamYAxisEndXY,
        //                    FixPosXY = c.FixPosXY,
        //                    ResolutionXY = c.ResolutionXY,
        //                    CamResolutionXY = c.CamResolutionXY,
        //                    CurrentPosCamXY = c.CurrentPosCamXY,
        //                    CurrentRobotPos = c.CurrentRobotPos,
        //                    CurrentUVRWPos = c.CurrentUVRWPos,
        //                    refPosXY = c.refPosXY,
        //                    BasePosXY = c.BasePosXY,
        //                    MovingPitch = c.MovingPitch,
        //                    //CalPointData = c.CalPointData,
        //                    //CalibrationPoints = c.CalibrationPoints,
        //                };

        //                newModel.QuadrantDatas.Add(newZoneData);
        //            });

        //            newModel.VisionOffsets = new List<VisionOffsetData>();
        //            this.SelectedModel.VisionOffsets.ForEach(t =>
        //            {
        //                newModel.VisionOffsets.Add(new VisionOffsetData()
        //                {
        //                    zoneID = t.zoneID,
        //                    plcoffset = t.plcoffset,
        //                });
        //            });

        //            newModel.ProductDatas = new List<ProductData>();
        //            this.SelectedModel.ProductDatas.ForEach(v =>
        //            {
        //                newModel.ProductDatas.Add(new ProductData()
        //                {
        //                    zondID = v.zondID,

        //                    WidthTopSpec = v.WidthTopOffset,
        //                    WidthBottomSpec = v.WidthTopOffset,
        //                    HeightLeftSpec = v.WidthTopOffset,
        //                    HeightRightSpec = v.HeightRightSpec,
        //                    AngleSpec = v.AngleSpec,

        //                    WidthTopOffset = v.WidthTopOffset,
        //                    WidthBottomOffset = v.WidthBottomOffset,
        //                    HeightLeftOffset = v.HeightLeftOffset,
        //                    HeightRightOffset = v.HeightRightOffset,

        //                    LengthCheckWidthTolerance = v.LengthCheckWidthTolerance,
        //                    LengthCheckHeightTolerance = v.LengthCheckHeightTolerance,
        //                    LengthCheckThetaTolerance = v.LengthCheckThetaTolerance,

        //                    VisionOffsetX = v.VisionOffsetX,
        //                    VisionOffsetY = v.VisionOffsetY,
        //                    VisionOffsetT = v.VisionOffsetT,

        //                    uvwData = v.uvwData,
        //                    uvrwData = v.uvrwData,
        //                });
        //            });



        //            sql.ModelData.Add(newModel);
        //            LockUtils.Wait(100);
        //            this.DataList = sql.ModelData.All().ToList();
        //            this.SelectedModel = newModel;

        //            var currentModelId = sql.MachineData.All().FirstOrDefault().CurrentModelId;
        //            var currentModelName = sql.ModelData.All().FirstOrDefault(x => x.ModelID.Equals(currentModelId));
        //            var sourcePath = @"D:\Data\" + currentModelName;
        //            var destPath = @"D:\Data\" + newModel.ModelName;

        //            if (Directory.Exists(sourcePath) && !Directory.Exists(destPath))
        //                DOTNET.FileSystem.FileUtils.CopyDirectory(sourcePath, destPath, true);

        //            var msg = new GUIEventArgs()
        //            {
        //                MessageKind = eGUIMessageKind.ChangeModelList,
        //            };

        //            guiEventPublisher.Publish(msg);

        //        }
        //    }
        //}


        private void ExecuteCloseCommand()
        {
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        private async void ExecuteSaveCommand()
        {
            this.IsSaving = true;

            this.DataList.ForEach(x =>
            {
                sql.ModelData.Edit(x);
            });

            await Task.Delay(TimeSpan.FromSeconds(1));

            this.IsSaving = false;

            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        #endregion
    }
}
