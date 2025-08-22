using DOTNET.Concurrent;
using DOTNET.Logging;
using DOTNET.QO;
using DOTNET.TCP;
using DOTNET.Utils;
using DOTNET.VISION.Model;
using MVision.Device.Robot.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using static MVision.Device.Robot.Config.KukaXMLConfig;

namespace MVision.Device.Robot
{
    public class KukaRobot
    {
        Logger logger = Logger.GetLogger("Robot");

        Tcp4 h;
        TsQueue<QueueObject> pullQueue = new TsQueue<QueueObject>();
        TsQueue<QueueObject> writeQueue = new TsQueue<QueueObject>();
        private bool isReadThreadAlive = true;
        public bool IsConnected { get; set; } = false;

        Thread rwThread = null;

        public event Action OnConnected;
        public event Action OnDisconnected;
        public event Action<object> OnReciveData;

        public KukaRobot()
        {

        }

        public void Dispose()
        {
            this.isReadThreadAlive = false;
            LockUtils.Wait(1);
            Thread4.Stop(this.rwThread);
            this.Disconnect();
            logger.I($"{nameof(KukaRobot)} - Dispose");
        }

        public void Init()
        {
            h = new Tcp4();
            h.Comm = new TcpComm()
            {
                Active = true, //true면 클라이언트, false면 서버
                Ip = "172.50.0.147",
                PortNo = 54610,
                T5 = 5,
                T6 = 0,
            };

            this.rwThread = Thread4.Go(ReadTh);
            Thread4.Go(PullTh);
            Thread4.Go(WriteTh);
        }

        void TryToConnectOrListen()
        {
            h.Comm.Ip = "172.50.0.147";
            h.Comm.PortNo = 54610;
            h.Comm.T5 = 5;
            h.Comm.T6 = 0;

            h.Init();

            if (h.Connected)
            {
                pullQueue.Enqueue(new QoComm());
                IsConnected = true;
                logger.I($"Robot {(h.Comm.Active ? "Client" : "Server")} Connected Success");
            }
        }

        public void Disconnect()
        {
            if (h is null) return;
            if (!h.Connected) return;
            h.Close();

            if (!h.Connected)
                pullQueue.Enqueue(new QoNotComm());

            writeQueue.Clear();
        }

        void ReadTh()
        {
            logger.I($"{nameof(ReadTh)} - Start");

            while (isReadThreadAlive)
            {
                try
                {
                    if (h.Connected)
                        ReadTcp();
                    else
                        TryToConnectOrListen();
                }
                catch (ThreadAbortException ex)
                {
                    logger.E(ex);
                    IsConnected = false;
                    break;
                }
                catch (Exception ex)
                {
                    logger.E(ex);
                    IsConnected = false;
                    Disconnect();
                }
            }

            logger.I($"{nameof(ReadTh)} - Stop");
        }

        void ReadTcp()
        {
            var ms = new KukaXMLConfig();

            try
            {
                if (h.Available == 0) return;

                var robotXML = h.Read(h.Available).ToAscii.Trim();
                XDocument doc = XDocument.Parse(robotXML);
                XElement root = doc.Root;

                if (root != null)
                {
                    ms.Command = root.Element("Command")?.Value ?? string.Empty;
                    ms.CommandValue = root.Element("Value")?.Value ?? string.Empty;
                    ms.TeachingX1 = root.Element("TeachingX1")?.Value ?? string.Empty;
                    ms.TeachingY1 = root.Element("TeachingY1")?.Value ?? string.Empty;
                    ms.TeachingT1 = root.Element("TeachingT1")?.Value ?? string.Empty;
                    ms.TeachingX2 = root.Element("TeachingX2")?.Value ?? string.Empty;
                    ms.TeachingY2 = root.Element("TeachingY2")?.Value ?? string.Empty;
                    ms.TeachingT2 = root.Element("TeachingT2")?.Value ?? string.Empty;
                }
            }
            catch (Exception ex)
            {
                logger.I("Error parsing XML: " + ex.Message);
            }

            h.Clean();

            WriteRobotCommand($"TO_Robot_START", true, new XYT(10, 20, 30), new XYT(50, 60, 70));
        }

        void WriteTh()
        {
            logger.I($"{nameof(WriteTh)} - Start");

            while (true)
            {
                try
                {
                    var o = writeQueue.Dequeue();
                    switch (o)
                    {
                        case WriteQueueObject _:
                            var obj = o as WriteQueueObject;
                            WriteTCP(obj);
                            break;

                        default:
                            break;
                    }

                }
                catch (Exception ex)
                {
                    logger.E(ex);
                }
            }
        }

        void WriteTCP(WriteQueueObject wobj)
        {
            if (!h.Connected)
            {
                logger.E("Write Fail - Robot TCP Not Connected");
                return;
            }

            var mb = wobj.buffer;
            h.Send(mb);
        }

        void PullTh()
        {
            logger.I($"{nameof(PullTh)} - Start");
            while (true)
            {
                try
                {
                    var o = pullQueue.Dequeue();
                    switch (o)
                    {
                        case QoComm _:
                            DlgUtils.Invoke(OnConnected);
                            break;
                        case QoNotComm _:
                            DlgUtils.Invoke(OnDisconnected);
                            break;
                        case QoRecd _:
                            DlgUtils.Invoke(OnReciveData, o.Arg0);
                            break;
                        case QoSent _:
                            break;
                        case KukaXMLConfig.QoSleep _:
                            logger.I($"{nameof(PullTh)}: - Sleep");
                            return;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    logger.E(ex);
                }
            }
        }

        public void WriteRobotCommand(string command, bool value, XYT xyt1, XYT xyt2)
        {
            var tag = "Vision";
            var tagCommand = "Command";
            var tagValue = "Value";
            var tagTargerX1 = "TargerX1";
            var tagTargerY1 = "TargerY1";
            var tagTargerT1 = "TargerT1";
            var tagTargerX2 = "TargerX2";
            var tagTargerY2 = "TargerY2";
            var tagTargerT2 = "TargerT2";

            var ms = $"<{tag}>" +
                $"<{tagCommand}>{command}</{tagCommand}>" +
                $"<{tagValue}>{value}</{tagValue}>" +
                $"<{tagTargerX1}>{xyt1.X}</{tagTargerX1}>" +
                $"<{tagTargerY1}>{xyt1.Y}</{tagTargerY1}>" +
                $"<{tagTargerT1}>{xyt1.T}</{tagTargerT1}>" +
                $"<{tagTargerX2}>{xyt2.X}</{tagTargerX2}>" +
                $"<{tagTargerY2}>{xyt2.Y}</{tagTargerY2}>" +
                $"<{tagTargerT2}>{xyt2.T}</{tagTargerT2}>" +
                $"</{tag}>";

            logger.I(ms);

            var mb = new MemoryBuffer();
            mb.AppendAscii(ms);

            writeQueue.Enqueue(new WriteQueueObject(mb));
        }
    }
}
