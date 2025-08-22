using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.Common.Shared
{
    public class ConstAlignString
    {
        //Align - Bool
        public const string UseLengthCheck = "UseLengthCheck";
        public const string Target3PAlign = "Target3PAlign";
        public const string Object3PAlign = "Object3PAlign";
        public const string Target2PAlign = "Target2PAlign";
        public const string Object2PAlign = "Object2PAlign";
        public const string UseCheckProjectionDegree = "UseCheckProjectionDegree";
    }

    public class ConstAutoString
    {
        //Auto - String
        public const string GrabDelayTime = "GrabDelayTime";
        public const string MaxReAlignCount = "MaxReAlignCount";
        public const string AutoTimeOutLimit = "AutoTimeOutLimit";
        public const string AutoGrabRetryLimit = "AutoGrabRetryLimit";
        public const string ManualMarkRetryLimit = "ManualMarkRetryLimit";
        public const string InspectionPatternLimit = "InspectionPatternLimit";
        public const string InspectionPatternDistance = "InspectionPatternDistance";
        public const string InspectionLineLimit = "InspectionLineLimit";
        public const string InspectionLineDistance = "InspectionLineDistance";
        public const string ScoreLimit = "ScoreLimit";
    }

    public class ConstETCString
    {
        //Etcetera - Bool
        public const string UseAlwaysLightOn = "UseAlwaysLightOn";
        public const string UseAutoModelChange = "UseAutoModelChange";
        public const string UseTimeSync = "UseTimeSync";
        public const string SaveOKImage = "SaveOKImage";
        public const string SaveNGImage = "SaveNGImage";
        public const string SaveGraphics = "SaveGraphics";
    }

    public class ConstRunModeString
    {
        //Run-Mode - Bool
        public const string UseDryRunMode = "UseDryRunMode";
    }

    public class ConstGraphicString
    {
        //Graphic - Bool
        public const string ShowAutoGrabImage = "ShowAutoGrabImage";
        public const string ShowOnlyOKCaliper = "ShowOnlyOKCaliper";
        public const string ShowRegionOfInterest = "ShowRegionOfInterest";
        public const string ShowIntersectionPoint = "ShowIntersectionPoint";
        public const string ShowXYTScore = "ShowXYTScore";
    }

    public class ConstLogString
    {
        //Log - String
        public const string LogSaveTime = "LogSaveTime";
        public const string LogBackUpDays = "LogBackUpDays";
        public const string ImageBackUpDays = "ImageBackUpDays";
    }

    public class SharedRegion
    {
        public const string MainView = "MainView";
    }
}
