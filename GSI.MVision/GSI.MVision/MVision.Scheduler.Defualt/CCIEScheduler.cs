using DOTNET.Concurrent;
using DOTNET.Logging;
using DOTNET.PLC.Model;
using DOTNET.PLC.SLMP;
using MVision.Common.Event;
using MVision.Common.Event.EventArg;
using MVision.Common.Shared;
using MVision.Formulas.Common;
using MVision.MairaDB;
using MVision.Manager.Robot;
using MVision.Manager.Vision;
using MVision.SchedulerBase;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.Scheduler.Defualt
{
    public class CCIEScheduler : ScheduleBase
    {
        #region Properties

        Logger logger = Logger.GetLogger();
        Logger Unit1Logger = Logger.GetLogger("U1");
        Logger Unit2Logger = Logger.GetLogger("U2");
        Logger Unit3Logger = Logger.GetLogger("U3");
        Logger TactLogger = Logger.GetLogger("Tact");
        Logger dbLogger = Logger.GetLogger("DB");

        AlignFormulas algorithmProcesser = null;

        TsQueue<eSchedulerCommandKind> U1 = new TsQueue<eSchedulerCommandKind>();
        TsQueue<eSchedulerCommandKind> U2 = new TsQueue<eSchedulerCommandKind>();
        TsQueue<eSchedulerCommandKind> U3 = new TsQueue<eSchedulerCommandKind>();
        TsQueue<eSchedulerCommandKind> U4 = new TsQueue<eSchedulerCommandKind>();

        public Dictionary<string, int> previousBitStates { get; private set; } = new Dictionary<string, int>();

        IEventAggregator _eventAggregator = null;
        IContainerProvider provider = null;

        #endregion

        #region Struct

        public CCIEScheduler(IEventAggregator eventAggregator, IContainerProvider provider, AlignFormulas algorithmProcesser, SlmpManager plc, MariaManager sql, LibraryManager libraryManager, LightControlManager lightControlManager)
            : base(eventAggregator, provider, plc, sql, libraryManager, lightControlManager)
        {
            this.algorithmProcesser = algorithmProcesser;
            eventAggregator.GetEvent<ApplicationExitEvent>().Subscribe((o) => Dispose(), true);

            this._eventAggregator = eventAggregator;
            this.provider = provider;
        }

        public override void Init()
        {
            var u1 = new SchedulerWorker("U1");
            u1.WorkQueue = this.U1;
            //u1.Align1 = U1_AlignAction;
            //u1.Calibration1 = U1_Calibration1Action;
            base.Addworker(u1);

            var u2 = new SchedulerWorker("U2");
            u2.WorkQueue = this.U2;
            //u1.Align1 = U1_AlignAction;
            //u1.Calibration1 = U1_Calibration1Action;
            base.Addworker(u2);

            var u3 = new SchedulerWorker("U3");
            u3.WorkQueue = this.U3;
            //u1.Align1 = U1_AlignAction;
            //u1.Calibration1 = U1_Calibration1Action;
            base.Addworker(u3);

            base.Init();

            // word block data init
            var u1InitData = plc.ReadWord("FR_WORD_U1").IntValue;
            var u2InitData = plc.ReadWord("FR_WORD_U2").IntValue;
            var u3InitData = plc.ReadWord("FR_WORD_U3").IntValue;

            previousBitStates.Add("FR_WORD_U1", u1InitData);
            previousBitStates.Add("FR_WORD_U2", u2InitData);
            previousBitStates.Add("FR_WORD_U3", u3InitData);

        }
        #endregion

        protected override void Plc_OnBitChanged(BitBlock block)
        {
        }

        protected override void Plc_OnWordChanged(WordBlock block)
        {
            // 관심 없는 태그는 무시
            if (!previousBitStates.ContainsKey(block.Name))
                return;

            // 현재 값과 이전 값
            int previous = previousBitStates[block.Name];
            int current = block.IntValue;

            // 변화 없음
            if (current == previous)
                return;

            // XOR 연산으로 변경된 비트 추출
            ushort changedBits = (ushort)(current ^ previous);

            // 이전 값 갱신
            previousBitStates[block.Name] = current;

            logger.I($"[{block.Name}] 워드 값 변경: {previous} → {current}");

            // 변경된 비트들만 순회
            for (int i = 0; i < 16; i++)
            {
                if (((changedBits >> i) & 1) == 1)
                {
                    bool isOn = ((current >> i) & 1) == 1;

                    if (!isOn) continue;

                    // 워드별 처리 분기
                    switch (block.Name)
                    {
                        case "FR_WORD_U1":
                            Changed_U1(i, isOn);
                            break;

                        case "FR_WORD_U2":
                            break;

                        case "FR_WORD_U3":
                            break;
                    }
                }
            }
        }
        private void Changed_U1(int bitIndex, bool isOn)
        {
            switch (bitIndex)
            {
                case 0:
                    if (isOn)
                    {
                    }
                    break;
                case 1:
                    if (isOn)
                    {
                    }
                    break;
                case 2:
                    if (isOn)
                    {
                    }
                    break;
                case 3:
                    if (isOn)
                    {
                    }
                    break;
                case 4:
                    if (isOn)
                    {
                    }
                    break;
                case 5:
                    if (isOn)
                    {
                    }
                    break;
                case 6:
                    if (isOn)
                    {
                    }
                    break;
                case 8:
                    if (isOn)
                    {
                    }
                    break;
                case 9:
                    if (isOn)
                    {
                    }
                    break;
                case 10:
                    if (isOn)
                    {
                    }
                    break;
                case 11:
                    if (isOn)
                    {
                    }
                    break;
                case 12:
                    if (isOn)
                    {
                    }
                    break;
                case 13:
                    if (isOn)
                    {
                    }
                    break;
                case 14:
                    if (isOn)
                    {
                    }
                    break;
                case 15:
                    if (isOn)
                    {
                    }
                    break;
            }
        }

        protected override void RunProcesser(GUIEventArgs obj)
        {
            throw new NotImplementedException();
        }

        #region Private Method
        private bool ReadBitInWord(string tagName, int bitIndex)
        {
            try
            {
                // 1. 현재 워드 값 읽기 (2바이트)
                ushort currentValue = (ushort)plc.ReadWord(tagName).IntValue;

                // 2. 비트 마스크 계산
                int mask = 1 << bitIndex;

                // 3. 마스킹 후 ON/OFF 판별
                bool isOn = (currentValue & mask) != 0;

                logger.I($"[PLC READ] {tagName} Bit {bitIndex} → {(isOn ? "ON" : "OFF")} (Value: {currentValue})");

                return isOn;
            }
            catch (Exception ex)
            {
                logger.E($"[PLC READ ERROR] {tagName}: {ex.Message}");
                return false;
            }
        }

        private void WriteBitInWord(string tagName, int bitIndex, bool value)
        {
            try
            {
                // 1. 현재 워드 값 읽기 (2바이트)
                ushort currentValue = (ushort)plc.ReadWord(tagName).IntValue;

                // 2. 비트 마스크 계산
                int mask = (ushort)(1 << bitIndex);

                // 3. 비트 조작 (ON/OFF)
                int newValue = value
                    ? (ushort)(currentValue | mask)
                    : (ushort)(currentValue & ~mask);

                if (newValue == currentValue) return;

                    plc.WriteWord(tagName, newValue);

                logger.I($"[PLC WRITE] {tagName} Bit {bitIndex} → {(value ? "ON" : "OFF")} (Value: {currentValue} → {newValue})");
            }
            catch (Exception ex)
            {
                logger.E($"[PLC WRITE ERROR] {tagName}: {ex.Message}");
            }
        }

        #endregion
    }
}
