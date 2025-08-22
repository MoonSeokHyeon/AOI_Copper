using DOTNET.PLC.Model;
using DOTNET.PLC.XGT;
using DOTNET.Utils;
using MaterialDesignThemes.Wpf;
using MVision.Common.Event.EventArg;
using MVision.Common.Event;
using MVision.Common.Model;
using MVision.Common.Shared;
using MVision.UI.InteractivityViews.View;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Mvvm;
using DOTNET.PLC.SLMP;

namespace MVision.UI.InterfaceViews.ViewModel
{
    public class PLCInterfaceVTypeViewModel : BindableBase
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


        public string SubText { get; set; }

        GUIMessageEvent guiEventPublisher = null;
        IContainerProvider provider = null;
        SlmpManager plc = null;

        List<IntefaceBlock> ToList = null;
        List<IntefaceBlock> FromList = null;
        #endregion

        #region ICommand

        public ICommand ChangeBlockStateCommand { get; set; }

        #endregion

        #region Construct

        public PLCInterfaceVTypeViewModel(IEventAggregator aggregator, IContainerProvider provider, SlmpManager plc)
        {
            this.provider = provider;
            this.plc = plc;

            guiEventPublisher = aggregator.GetEvent<GUIMessageEvent>();

            this.SubText = String.Empty;

            this.ChangeBlockStateCommand = new DelegateCommand<object>(ExecuteChageBlockStateCommand);
        }

        bool isInited = false;
        public void Init()
        {
            this.ToList = new List<IntefaceBlock>();
            this.FromList = new List<IntefaceBlock>();
            //PLC 에 아직 Group Add 가 되지 않았으면 Event 로 전환 필요.
            var bitList = plc.GetGroup("M").BlockList;
            bitList.ForEach(bit =>
            {
                if (bit.Name.StartsWith("TO_"))
                    this.ToList.Add(new IntefaceBlock { Addr = bit.DspAddr, Tag = bit.Name, Value = bit.Value, IsBit = true });

                if (bit.Name.StartsWith("FR_"))
                    this.FromList.Add(new IntefaceBlock { Addr = bit.DspAddr, Tag = bit.Name, Value = bit.Value, IsBit = true });
            });

            var wordList = plc.GetGroup("D").BlockList;
            wordList.ForEach(word =>
            {
                if (word.Name.StartsWith("TO_"))
                    this.ToList.Add(new IntefaceBlock { Addr = word.DspAddr, Tag = word.Name, Value = word.Value, IsBit = false });

                if (word.Name.StartsWith("FR_"))
                    this.FromList.Add(new IntefaceBlock { Addr = word.DspAddr, Tag = word.Name, Value = word.Value, IsBit = false });
            });

            plc.OnBitChanged += Plc_OnBitChanged;
            plc.OnWordChanged += Plc_OnWordChanged;

            SetIO();
        }

        #endregion

        #region Private Method

        private void Plc_OnWordChanged(WordBlock block)
        {
            if (!block.SubText.Equals(this.SubText)) return;

            {
                var item = this.ToList.FirstOrDefault(x => x.Tag.Equals(block.Name));
                if (item != null)
                {
                    item.Value = block.Value;
                    return;
                }
            }

            {
                var item = this.FromList.FirstOrDefault(x => x.Tag.Equals(block.Name));
                if (item != null)
                {
                    item.Value = block.Value;
                    return;
                }
            }

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
            if (!block.SubText.Equals(this.SubText) || block.Name.Equals("TO_ALIVE")) return;

            var itemToList = this.ToList.FirstOrDefault(x => x.Tag.Equals(block.Name));
            if (itemToList != null)
            {
                itemToList.Value = block.Value;
                itemToList.IsOn = block.IsBitOn;
            }

            var itemFromList = this.FromList.FirstOrDefault(x => x.Tag.Equals(block.Name));
            if (itemFromList != null)
            {
                itemFromList.Value = block.Value;
                itemFromList.IsOn = block.IsBitOn;
            }

            var itemToDataList = this.ToDataList.FirstOrDefault(x => x.Tag.Equals(block.Name));
            if (itemToDataList != null)
            {
                itemToDataList.Value = block.Value;
                itemToDataList.IsOn = block.IsBitOn;
                return;
            }

            var itemFromDataList = this.FromDataList.FirstOrDefault(x => x.Tag.Equals(block.Name));
            if (itemFromDataList != null)
            {
                itemFromDataList.Value = block.Value;
                itemFromDataList.IsOn = block.IsBitOn;
                return;
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
                        plc.WriteBit(item.Tag, plc.ReadBit(item.Tag) ? false : true);
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

                if (!DialogHost.IsDialogOpen("RootDialog"))
                {
                    var result = await DialogHost.Show(view, "RootDialog") as bool?;

                    msg.Arg = false;
                    this.guiEventPublisher.Publish(msg);

                    if (result == true)
                    {
                        double ret = 0d;
                        if (double.TryParse(view.ViewModel.InputValue.Trim(), out ret))
                        {
                            var a = ret.ToString();
                            plc.WriteWord(item.Tag, a);
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

            SetIO();
        }

        #endregion

        #region Public Method

        public void SetIO()
        {
            //Todo : Set IO kind of Zone 

            this.ToDataList.Clear();
            this.fromDataList.Clear();

            var bitList = plc.GetGroup("M").BlockList;
            bitList.ForEach(bit =>
            {
                if (bit.SubText.Equals(this.SubText) && !bit.Name.Equals("TO_ALIVE"))
                {
                    var ToBit = this.ToList.FirstOrDefault(x => x.Tag.Equals(bit.Name));
                    var FromBit = this.FromList.FirstOrDefault(x => x.Tag.Equals(bit.Name));

                    if (ToBit != null)
                    {
                        if (ToBit.Tag.StartsWith("TO_"))
                        {
                            if (bit.Value == "0") ToBit.IsOn = false;
                            else ToBit.IsOn = true;

                            this.ToDataList.Add(ToBit);
                        }
                    }

                    if (FromBit != null)
                    {
                        if (FromBit.Tag.StartsWith("FR_"))
                        {
                            if (bit.Value == "0") FromBit.IsOn = false;
                            else FromBit.IsOn = true;

                            this.FromDataList.Add(FromBit);
                        }
                    }
                }
            });

            var wordList = plc.GetGroup("D").BlockList;
            wordList.ForEach(word =>
            {
                if (word.SubText.Equals(this.SubText))
                {
                    var ToWord = this.ToList.FirstOrDefault(y => y.Tag.Equals(word.Name));
                    var FromWord = this.FromList.FirstOrDefault(y => y.Tag.Equals(word.Name));

                    if (ToWord != null)
                    {
                        if (ToWord.Tag.StartsWith("TO_"))
                            this.ToDataList.Add(ToWord);
                    }

                    if (FromWord != null)
                    {
                        if (FromWord.Tag.StartsWith("FR_"))
                            this.FromDataList.Add(FromWord);
                    }

                }
            });
        }

        #endregion
    }
}
