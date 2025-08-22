using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.Common.Shared
{
    public enum eSystemState
    {
        None = -1,
        Manual,
        Auto,
    }

    public enum ePlcKind
    {
        None,
        COMMON,
        ALIGN,
        CAL,
        FDC,
    }
    public enum eDisplayMouseMode
    {
        Pointer,
        Pan,
        ZoomIn,
        ZoomOut,
        UserDefined,
        Touch
    }
    public enum eSchedulerCommandKind
    {
        None,
        Align1,
        Align2,
        Inspection,
        Calibration1,
        Calibration2,
        Dispose,
    }

    public enum eSchedulerMessageKind
    {
        None,
        WorkEnque,
        CameraPropertyChanged,
        ChangeModel,
        Calibration,
        CalibrationComplete,
        CalDataCalculateComplete,
        AddAlignHistory,
        AddInspectionHistory,
        ViewLoggerChanged,
        FailModelChange,
        CompleteModelChange,
        CompleteProcessor,
        FindRunProcesser,
    }

    public enum eGUIMessageKind
    {
        None,
        SystemStateChange,
        ChangeModel,
        ChangeModelList,
        LightValueChange,
        CameraPropertyChanged,
        FixAirspaceChanged,
        MainViewChanged,
        ViewLoggerChanged,
        RunProcesser,
        FitImage,           //220629 Kkm 마우스모드 추가
        MouseCursor,
        MousePan,
        MouseZoomIn,
        MouseZoomOut,
    }

    public enum eMessageLevelKind
    {
        None,
        Warn,
        Fail,
        PLC,
        Info,
    }
    public enum ePLCInterfaceWord
    {
        None,
        ALIGN_START,
        ALIGN_END,
        CAL_START,
        CAL_END,
        MOVE,
        MOVE_DONE,
        ALGIN_OK,
        ALIGN_NG,
        CAL_OK,
        CAL_NG,
        INSPECTION_START,
        INSPECTION_END,
        INSPECTION_OK,
        INSPECTION_NG,
        SPARE,
    }

    public enum eCameraID
    {
        None,
        Camera1,
        Camera2,
        Camera3,
        Camera4,
        Camera5,
        Camera6,
        Camera7,
        Camera8,
        Camera9,
        Camera10,
        Camera11,
        Camera12,
        Camera13,
        Camera14,
        Camera15,
        Camera16,
    }

    public enum eExecuteZoneID
    {
        NONE, //전체의 영역을 선택 시 사용.
              //PRE_A,
              //PRE_B,
        Main1_TARGET,
        Main1_OBJECT,
        Main2_TARGET,
        Main2_OBJECT,
        Inspection1_TARGET,
        Inspection1_OBJECT,
        TAPE_FEEDER,
        TAPE_FEEDER2,
        //Sub1_TARGET,
        //Sub1_OBJECT,
        //Sub2_TARGET,
        //Sub2_OBJECT,
        //Main2_TARGET,
        //Main2_OBJECT,
        //INSPECTION2_TARGET,
        //INSPECTION2_OBJECT,
        //TAPE_FEEDER,
        //LOADER,
        //UNLOADER,
    }

    public enum eQuadrant
    {
        None,
        FirstQuadrant,
        SecondQuadrant,
        ThirdQuadrant,
        FourthQuadrant,
    }

    public enum eCameraEventKind
    {
        None,
        OneShotGrab,
        ContinuousGrab,
        StopContinususGrab,
        GrabComplated,
        StartGrabbing,
        StopGrabbing,
    }

    public enum eMultiPattern
    {
        None,
        PM1,
        PM2,
        PM3,
        PM4,
        CalPM,
    }

    public enum eKindFindLine
    {
        None,
        VerLine,
        HorLine,
    }

    public enum eBlobNum
    {
        None,
        Blob1,
        Blob2,
        Blob3,
        Blob4,
    }

    public enum eFindType
    {
        None,
        Pattern,
        Line,
        Corner,
        FixtureCorner,
        Inspection,
    }

    public enum eFindPos
    {
        None,
        ModelEdit,
        PatternEdit,
        CaliperEdit,
        CornerEdit,
        BlobEdit,
        InspectionEdit,
        InspectionLineEdit,
        Display,
    }

    public enum eErrorCode
    {
        None = 0,
        CheckROI,
        MarkNG,
        LengthCheckNG,
        NoMark,
        VisionLimit,
        LineNG,
        CameraDisconnected,
        CalibrationDataError,
        NoLine,
        InspectionRangeOver,
        InspectionDataIsNotTrained,
        CrossLineAngleRangeOver,
        BlobNG,
        NoInputImage,
    }

    public enum eRotateFlipType
    {
        None,
        Rotate90Deg,
        Rotate180Deg,
        Rotate270Deg,
        Flip,
        FlipAndRotate90Deg,
        FlipAndRotate180Deg,
        FlipAndRotate270Deg,
    }

    public enum eAlignMethod
    {
        None,
        UVW,
        UVRW,
        XYT,
    }
    public enum ePort
    {
        None = 0,
        COM1,
        COM2,
        COM3,
        COM4,
    }
    public enum eChannel
    {
        None = 0,
        Ch1,
        Ch2,
        Ch3,
        Ch4,
        Ch5,
        Ch6,
        Ch7,
        Ch8,
    }
    public enum eRobot
    {
        None,
        Robot1,
        Robot2,
        Robot3,
        Robot4,
    }
}
