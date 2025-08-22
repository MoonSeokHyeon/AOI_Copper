using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.Common.DBModel
{
    public class ModelData : BindableBase
    {
        public int modelID;
        public int ModelID
        {
            get { return modelID; }
            set { SetProperty(ref modelID, value); }
        }

        public string modelName;
        public string ModelName
        {
            get { return modelName; }
            set { SetProperty(ref modelName, value); }
        }

        public string description;
        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value); }
        }

        public DateTime CreateTime { get; set; }


        public ModelData()
        {
            CreateTime = DateTime.Now;
        }
    }
}
