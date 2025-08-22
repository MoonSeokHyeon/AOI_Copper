using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.Common.Interface
{
    public interface ILightController
    {
        int[] ChennelValue { get; set; }
        bool LightOn(int chanel, int val);
        bool LightOff(int chanel);
        int GetChanelValue(int chanel);
    }
}
