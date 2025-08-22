using DOTNET.PLC.Model;
using DOTNET.PLC.SLMP;
using DOTNET.Utils;
using MaterialDesignThemes.Wpf;
using MVision.Common.Event;
using MVision.Common.Event.EventArg;
using MVision.Common.Model;
using MVision.Common.Shared;
using MVision.UI.InteractivityViews.View;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVision.UI.InterfaceViews.ViewModel
{
    public class PLCInterfaceHTypeViewModel : BindableBase
    {
        #region Properties

        ObservableCollection<IntefaceBlock> toDataList = new ObservableCollection<IntefaceBlock>();
        public ObservableCollection<IntefaceBlock> ToDataList
        {
            get => this.toDataList;
            set => SetProperty(ref this.toDataList, value);
        }

        ObservableCollection<IntefaceBlock> fromDataList = new ObservableCollection<IntefaceBlock>();
        public ObservableCollection<IntefaceBlock> FromDataList
        {
            get => this.fromDataList;
            set => SetProperty(ref this.fromDataList, value);
        }

        private IntefaceBlock selectedItem;
        public IntefaceBlock SelectedItem
        {
            get { return selectedItem; }
            set { SetProperty(ref this.selectedItem, value); }
        }

        public ICommand ChangeBlockStateCommand { get; set; }

        /// <summary>
        /// 해당 된 영역만 가져 오기 위해.
        /// </summary>
        public string SubText { get; set; }

        GUIMessageEvent guiEventPublisher = null;
        IContainerProvider provider = null;
        SlmpManager plc = null;

        #endregion

        #region Construct

        public PLCInterfaceHTypeViewModel(IEventAggregator aggregator, IContainerProvider provider, SlmpManager plc)
        {
            this.provider = provider;
            this.plc = plc;

            this.ChangeBlockStateCommand = new DelegateCommand<object>(ExecuteChageBlockStateCommand);
        }

        bool isInited = false;
        public void Init()
        {
            if (isInited) return;
            isInited = true;

            //PLC 에 아직 Group Add 가 되지 않았으면 Event 로 전환 필요.
            var bitList = plc.GetGroup("M").BlockList;
            bitList.ForEach(bit =>
            {
                if (bit.SubText.Equals(this.SubText))
                {
                    if (bit.Name.StartsWith("TO_"))
                        this.ToDataList.Add(new IntefaceBlock { Addr = bit.DspAddr/* + "." + bit.BitIndex.ToString("X")*/, Tag = bit.Name, Value = bit.Value, IsBit = true });

                    if (bit.Name.StartsWith("FR_"))
                        this.FromDataList.Add(new IntefaceBlock { Addr = bit.DspAddr/* + "." + bit.BitIndex.ToString("X")*/, Tag = bit.Name, Value = bit.Value, IsBit = true });
                }
            });

            var wordList = plc.GetGroup("D").BlockList;
            wordList.ForEach(word =>
            {
                if (word.SubText.Equals(this.SubText))
                {
                    if (word.Name.StartsWith("TO_"))
                        this.ToDataList.Add(new IntefaceBlock { Addr = word.DspAddr, Tag = word.Name, Value = word.Value, IsBit = false });

                    if (word.Name.StartsWith("FR_"))
                        this.FromDataList.Add(new IntefaceBlock { Addr = word.DspAddr, Tag = word.Name, Value = word.Value, IsBit = false });
                }
            });

            plc.OnBitChanged += Plc_OnBitChanged;
            plc.OnWordChanged += Plc_OnWordChanged;
        }

        #endregion

        #region Private Method

        private void Plc_OnWordChanged(WordBlock block)
        {
            if (!block.SubText.Equals(this.SubText)) return;

            {
                var item = this.ToDataList.FirstOrDefault(x => x.Tag.Equals(block.Name));
                if (item != null)
                {
                    item.Value = block.Value;
                    return;
                }
            }

            {
                var item = this.FromDataList.FirstOrDefault(x => x.Tag.Equals(block.Name));
                if (item != null)
                {
                    item.Value = block.Value;
                    return;
                }
            }
        }

        private void Plc_OnBitChanged(BitBlock block)
        {
            if (!block.SubText.Equals(this.SubText)) return;

            {
                var item = this.ToDataList.FirstOrDefault(x => x.Tag.Equals(block.Name));
                if (item != null)
                {
                    item.Value = block.Value;
                    return;
                }
            }

            {
                var item = this.FromDataList.FirstOrDefault(x => x.Tag.Equals(block.Name));
                if (item != null)
                {
                    item.Value = block.Value;
                    return;
                }
            }

        }

        async void ExecuteChageBlockStateCommand(object obj)
        {
            var item = CastTo<IntefaceBlock>.From(obj);

            if (item.IsBit)
            {
                var view = this.provider.Resolve<ComfirmationView>();
                view.ViewModel.Message = item.IsOn ? $"[{item.Addr}]{item.Tag} Bit Off ?" : $"[{item.Addr}]{item.Tag} Bit On ?";

                var msg = new GUIEventArgs()
                {
                    MessageKind = eGUIMessageKind.FixAirspaceChanged,
                    Arg = true,
                };

                this.guiEventPublisher.Publish(msg);

                if (!DialogHost.IsDialogOpen("RootDialog"))
                {
                    var result = await DialogHost.Show(view, "RootDialog") as bool?;

                    msg.Arg = false;
                    this.guiEventPublisher.Publish(msg);

                    if (result == true)
                        plc.WriteBit(item.Tag, item.IsOn ? false : true);
                }
            }
            else
            {
                var view = this.provider.Resolve<ValueInputView>();
                view.ViewModel.InputValueName = $"[{item.Addr}]" + item.Tag;
                view.ViewModel.CurrentValue = item.Value;

                var msg = new GUIEventArgs()
                {
                    MessageKind = eGUIMessageKind.FixAirspaceChanged,
                    Arg = true,
                };

                this.guiEventPublisher.Publish(msg);

                msg.Arg = false;
                this.guiEventPublisher.Publish(msg);

                if (!DialogHost.IsDialogOpen("RootDialog"))
                {
                    var result = await DialogHost.Show(view, "RootDialog") as bool?;
                    if (result == true)
                    {
                        double ret = 0d;
                        if (double.TryParse(view.ViewModel.InputValue.Trim(), out ret))
                        {
                            plc.WriteWord(item.Tag, ret.ToString());
                        }
                        else
                        {
                            var notificationView = this.provider.Resolve<NotificationView>();
                            notificationView.ViewModel.Message = "Invalid Input Data !";

                            if (!DialogHost.IsDialogOpen("RootDialog"))
                                await DialogHost.Show(notificationView, "RootDialog");
                        }
                    }
                }
            }
        }

        #endregion

        #region Public Method

        public void SetIO(string subText, ePlcKind plcKind)
        {
            //Todo : Set IO kind of Zone 

            plc.OnBitChanged -= Plc_OnBitChanged;
            plc.OnWordChanged -= Plc_OnWordChanged;

            this.ToDataList.Clear();
            this.fromDataList.Clear();

            var bitList = plc.GetGroup("M").BlockList;
            bitList.ForEach(bit =>
            {
                if (bit.SubText.Equals(subText) && bit.KindE.Equals(plcKind))
                {
                    if (bit.Name.StartsWith("TO_"))
                        this.ToDataList.Add(new IntefaceBlock { Addr = bit.DspAddr/* + "." + bit.BitIndex.ToString("X")*/, Tag = bit.Name, Value = bit.Value, IsBit = true });

                    if (bit.Name.StartsWith("FR_"))
                        this.FromDataList.Add(new IntefaceBlock { Addr = bit.DspAddr/* + "." + bit.BitIndex.ToString("X")*/, Tag = bit.Name, Value = bit.Value, IsBit = true });
                }
            });

            var wordList = plc.GetGroup("D").BlockList;
            wordList.ForEach(word =>
            {
                if (word.SubText.Equals(subText) && word.KindE.Equals(plcKind))
                {
                    if (word.Name.StartsWith("TO_"))
                        this.ToDataList.Add(new IntefaceBlock { Addr = word.DspAddr, Tag = word.Name, Value = word.Value, IsBit = false });

                    if (word.Name.StartsWith("FR_"))
                        this.FromDataList.Add(new IntefaceBlock { Addr = word.DspAddr, Tag = word.Name, Value = word.Value, IsBit = false });
                }
            });

            plc.OnBitChanged += Plc_OnBitChanged;
            plc.OnWordChanged += Plc_OnWordChanged;
        }

        #endregion
    }
}
