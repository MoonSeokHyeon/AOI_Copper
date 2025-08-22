using DOTNET.QO;
using DOTNET.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.Device.Robot.Config
{
    class KukaXMLConfig
    {
        string command = string.Empty;
        public string Command
        {
            get => command;
            set => command = value;
        }

        string commandValue = string.Empty;
        public string CommandValue
        {
            get => commandValue;
            set => commandValue = value;
        }

        string targetX1 = string.Empty;
        public string TargetX1
        {
            get => targetX1;
            set => targetX1 = value;
        }

        string targetY1 = string.Empty;
        public string TargetY1
        {
            get => targetY1;
            set => targetY1 = value;
        }

        string targetT1 = string.Empty;
        public string TargetT1
        {
            get => targetT1;
            set => targetT1 = value;

        }

        string targetX2 = string.Empty;
        public string TargetX2
        {
            get => targetX2;
            set => targetX2 = value;
        }

        string targetY2 = string.Empty;
        public string TargetY2
        {
            get => targetY2;
            set => targetY2 = value;
        }

        string targetT2 = string.Empty;
        public string TargetT2
        {
            get => targetT2;
            set => targetT2 = value;

        }

        string teachingX1 = string.Empty;
        public string TeachingX1
        {
            get => teachingX1;
            set => teachingX1 = value;
        }

        string teachingY1 = string.Empty;
        public string TeachingY1
        {
            get => teachingY1;
            set => teachingY1 = value;
        }

        string teachingT1 = string.Empty;
        public string TeachingT1
        {
            get => teachingT1;
            set => teachingT1 = value;

        }
        string teachingX2 = string.Empty;
        public string TeachingX2
        {
            get => teachingX2;
            set => teachingX2 = value;
        }

        string teachingY2 = string.Empty;
        public string TeachingY2
        {
            get => teachingY2;
            set => teachingY2 = value;
        }

        string teachingT2 = string.Empty;
        public string TeachingT2
        {
            get => teachingT2;
            set => teachingT2 = value;

        }

        public class WriteQueueObject : QueueObject
        {
            public MemoryBuffer buffer { get; set; }
            public WriteQueueObject(MemoryBuffer buf)
            {
                buffer = buf;
            }
        }
        public class QoComm : QueueObject
        {
            public QoComm()
            {
            }
        }
        public class QoNotComm : QueueObject
        {
            public QoNotComm()
            {
            }
        }

        public class QoRecd : QueueObject
        {
            public QoRecd()
            {
            }
        }

        public class QoSent : QueueObject
        {
            public QoSent()
            {
            }
        }

        public class QoSleep : QueueObject
        {
            public QoSleep()
            {
            }
        }
    }
}
