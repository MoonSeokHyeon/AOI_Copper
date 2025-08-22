using DOTNET.Concurrent;
using DOTNET.Logging;
using MVision.Common.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MVision.SchedulerBase
{
    public class SchedulerWorker : IDisposable
    {
        #region Properties

        static Logger logger = Logger.GetLogger(typeof(SchedulerWorker));
        public bool IsProcessing { get; set; }
        public TsQueue<eSchedulerCommandKind> WorkQueue { get; set; } = new TsQueue<eSchedulerCommandKind>();

        public Action Align1 { get; set; }
        public Action Align2 { get; set; }
        public Action Calibration1 { get; set; }
        public Action Calibration2 { get; set; }
        public Action Inspection { get; set; }

        string _name = string.Empty;
        public string Name { get => this._name; }

        #endregion

        #region Construct

        public SchedulerWorker(string name)
        {
            this._name = name;
            Thread4.Go(Run);
        }

        public void Dispose()
        {
            this.WorkQueue.Enqueue(eSchedulerCommandKind.Dispose);
        }

        #endregion

        #region Thread

        void Run()
        {
            Thread.CurrentThread.Name = this._name;
            logger.D($"Scheduler Worker {this._name} - Start Thread");

            while (true)
            {
                try
                {
                    IsProcessing = false;
                    var q = WorkQueue.Dequeue();
                    IsProcessing = true;

                    switch (q)
                    {
                        case eSchedulerCommandKind.Align1:
                            Align1();
                            break;
                        case eSchedulerCommandKind.Align2:
                            Align2();
                            break;
                        case eSchedulerCommandKind.Calibration1:
                            Calibration1();
                            break;
                        case eSchedulerCommandKind.Calibration2:
                            Calibration2();
                            break;
                        case eSchedulerCommandKind.Inspection:
                            Inspection();
                            break;
                        case eSchedulerCommandKind.Dispose:
                            logger.I($"Schecduler Worker Thread Disposed - {this._name}");
                            return;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    logger.E($"Scheduler Worker Expection - {ex}");
                }
            }
        }

        #endregion
    }
}
