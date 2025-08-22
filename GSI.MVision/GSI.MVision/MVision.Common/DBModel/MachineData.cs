using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.Common.DBModel
{
    public class MachineData : BindableBase
    {
        public int currentModelId;
        public int CurrentModelId
        {
            get { return currentModelId; }
            set { SetProperty(ref currentModelId, value); }
        }

        public string machineName;
        public string MachineName
        {
            get { return machineName; }
            set { SetProperty(ref machineName, value); }
        }

        public MachineData()
        {
        }
    }
}
