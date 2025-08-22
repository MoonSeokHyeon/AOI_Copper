using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.Common.DBModel
{
    public class UVWData : BindableBase
    {
        public int UVWPos { get; set; }
        public double Radius { get; set; }
        public double AngleX1 { get; set; }
        public double AngleX2 { get; set; }
        public double AngleY1 { get; set; }
        public double AngleY2 { get; set; }
        public bool DirR0 { get; set; }
        public bool DirX1 { get; set; }
        public bool DirX2 { get; set; }
        public bool DirY1 { get; set; }
        public bool DirY2 { get; set; }

        public UVWData()
        {
            this.UVWPos = 0;  

            this.Radius = 180.0;
            this.AngleX1 = 0.0;  
            this.AngleX2 = 180.0; 
            this.AngleY1 = 270.0;  
            this.AngleY2 = 0.0; 
            this.DirR0 = true;    
            this.DirX1 = true;    
            this.DirX2 = true;    
            this.DirY1 = true;    
            this.DirY2 = true;
        }
    }
}
