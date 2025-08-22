using Cognex.VisionPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using MVision.Common.Shared;
using DOTNET.VISION.Model;
using System.Windows.Forms;
using MVision.VisionLib.Cognex;

namespace MVision.UI.CogDisplayViews.View
{
    /// <summary>
    /// CogDisplayView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CogDisplayView : System.Windows.Controls.UserControl
    {
        #region Properties

        private WindowsFormsHost camViewHost = new WindowsFormsHost();
        public WindowsFormsHost CamViewHost
        {
            get { return camViewHost; }
            set { this.camViewHost = value; }
        }

        CognexVisionPro visionPro = null;

        CogRecordDisplay cogRecord = null;

        public bool FixAirspace
        {
            get { return this.asfCogView.FixAirspace; }
            set { this.asfCogView.FixAirspace = value; }
        }

        private bool isCenterGrid = false;
        public bool IsCenterGrid
        {
            get { return isCenterGrid; }
            set { isCenterGrid = value; }
        }

        private bool isCustomGrid = false;
        public bool IsCustomGrid
        {
            get { return isCustomGrid; }
            set { isCustomGrid = value; }
        }

        public event Action CogDisplayLoaded;
        public event MouseEventHandler CogDisplayMouseMove;
        public event MouseEventHandler CogDisplayMouseUp;
        public event MouseEventHandler CogDisplayMouseDown;
        public event EventHandler CogDisplayClick;
        public event EventHandler CogDisplayDoubleClick;

        string displayName = String.Empty;

        #endregion

        #region Construct

        public CogDisplayView()
        {
            InitializeComponent();

            this.DataContext = this;

            visionPro = new CognexVisionPro();

            this.Loaded += CogDisplayView_Loaded;

            Initialize();            //220702 Kkm 기존= Loaded에서 실행 -> 변경= 생성자 단에서 실행
        }

        bool _isInted = false;
        private void Initialize()
        {
            if (!_isInted)
            {
                _isInted = true;

                CamViewHost.Child = this.cogRecord = new CogRecordDisplay();
                this.cogRecord.CreateControl();

                this.cogRecord.HorizontalScrollBar = false;
                this.cogRecord.VerticalScrollBar = false;
                this.cogRecord.AutoFit = true;
                this.cogRecord.BackColor = Color.DarkGray;
                this.cogRecord.MouseDown += CogRecord_MouseDown;
                this.cogRecord.MouseUp += CogRecord_MouseUp;
                this.cogRecord.MouseMove += CogRecord_MouseMove;
                this.cogRecord.DoubleClick += CogRecord_DoubleClick;
                this.cogRecord.Click += CogRecord_Click;

                CogDisplayLoaded?.Invoke();
            }
        }
        private void CogDisplayView_Loaded(object sender, RoutedEventArgs e)
        {
        }
        public void DisposeDisplay()
        {
            if (this.cogRecord != null) this.cogRecord.Dispose();
        }

        #endregion

        #region Public Method

        public void SetFixAirspace(bool value)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                if (this.FixAirspace != value)
                {

                    FixAirspace = value;
                }
            }));
        }

        public void SetDisplayName(string DisplayName)
        {
            displayName = DisplayName;
        }

        public string GetDisplayName()
        {
            return displayName;
        }

        bool firstSet = false;

        public bool SetImage(ICogImage image)
        {
            if (this.cogRecord == null) return false;

            if (image == null) return false;

            this.cogRecord.Image = image;

            if (!firstSet)
            {
                firstSet = true;
                FitImage();
            }

            return true;
        }

        public CogTransform2DLinear GetDisplayTransform()
        {
            if (cogRecord.Image == null) return null;

            return cogRecord.GetTransform("#", "*") as CogTransform2DLinear;
        }

        public ICogImage GetCogImage()
        {
            if (this.cogRecord == null) return null;

            if (cogRecord.Image == null) return null;

            var image = this.cogRecord.Image;

            return image;
        }

        public void SetBorderRed(bool red)
        {
            if (red)
                this.bodMain.BorderBrush = System.Windows.Media.Brushes.Red;
            else
                this.bodMain.BorderBrush = null;
        }

        public void SetGraphic(CogGraphicInteractiveCollection cogGraphicInteractiveCollection, string groupName, bool checkForDuplicates)
        {
            try
            {
                this.cogRecord.InteractiveGraphics.AddList(cogGraphicInteractiveCollection, groupName, checkForDuplicates);
            }
            catch (Exception ex)
            {
            }
        }

        public void SetMouseMode(eDisplayMouseMode mouseMode)
        {
            this.cogRecord.MouseMode = (Cognex.VisionPro.Display.CogDisplayMouseModeConstants)mouseMode;
        }

        public void SetCenterGrid()
        {
            if (cogRecord.Image == null) return;

            CogLine lineH = new CogLine();
            lineH.Color = CogColorConstants.Orange;
            CogLine lineV = new CogLine();
            lineV.Color = CogColorConstants.Orange;

            int width = cogRecord.Image.Width / 2;
            int height = cogRecord.Image.Height / 2;

            lineH.SetFromStartXYEndXY(width, 0, width, height);
            lineV.SetFromStartXYEndXY(0, height, width, height);

            this.cogRecord.InteractiveGraphics.Add(lineV, "CenterGrid", false);
            this.cogRecord.InteractiveGraphics.Add(lineH, "CenterGrid", false);
        }
        public void SetCustomGrid(double x, double y, int num, bool Reverse)
        {
            if (cogRecord.Image == null) return;

            CogLine lineH = new CogLine();
            lineH.Color = CogColorConstants.Magenta;
            CogLine lineV = new CogLine();
            lineV.Color = CogColorConstants.Magenta;

            var gridXY = this.GetDisplayPointToImagePoint(new XY(x, y));

            if (Reverse)
            {
                lineH.SetFromStartXYEndXY(gridXY.X, 0, gridXY.X, this.cogRecord.Image.Height);
                lineV.SetFromStartXYEndXY(0, gridXY.Y, this.cogRecord.Image.Width, gridXY.Y);
            }
            else
            {
                lineH.SetFromStartXYEndXY(this.cogRecord.Image.Width - gridXY.X, 0, this.cogRecord.Image.Width - gridXY.X, this.cogRecord.Image.Height);
                lineV.SetFromStartXYEndXY(0, gridXY.Y, this.cogRecord.Image.Width, gridXY.Y);
            }

            this.cogRecord.InteractiveGraphics.Add(lineV, $"CustomGrid{num}", false);
            this.cogRecord.InteractiveGraphics.Add(lineH, $"CustomGrid{num}", false);
            this.cogRecord.InteractiveGraphics.Add(visionPro.CreateLabelTopLeft($"X : {Math.Round(gridXY.X, 2)},  Y : {Math.Round(gridXY.Y, 2)}"), $"CustomLabel{num}", false);
        }
        public void SetCenterRectangleGrid()
        {
            if (cogRecord.Image == null) return;

            CogLine lineH = new CogLine();
            lineH.Color = CogColorConstants.Orange;
            CogLine lineV = new CogLine();
            lineV.Color = CogColorConstants.Orange;
            CogRectangle rectangle = new CogRectangle();
            rectangle.Color = CogColorConstants.Orange;

            int width = cogRecord.Image.Width / 2;
            int height = cogRecord.Image.Height / 2;

            lineH.SetFromStartXYEndXY(width, 0, width, height);
            lineV.SetFromStartXYEndXY(0, height, width, height);
            rectangle.SetCenterWidthHeight(width, height, 300, 300);

            this.cogRecord.InteractiveGraphics.Add(lineV, "CenterGrid", false);
            this.cogRecord.InteractiveGraphics.Add(lineH, "CenterGrid", false);
            this.cogRecord.InteractiveGraphics.Add(rectangle, "CenterGrid", false);
        }
        public bool IsLiveDisplay()
        {
            if (cogRecord == null) return false;

            return cogRecord.LiveDisplayRunning;
        }

        public void StartLiveDisplay(ICogAcqFifo fifo)
        {
            if (cogRecord == null || fifo == null) return;

            cogRecord.StartLiveDisplay(fifo, false);
        }

        public bool StopLiveDisplay()
        {
            if (cogRecord == null) return true;

            if (cogRecord.LiveDisplayRunning)
                cogRecord.StopLiveDisplay();

            return true;
        }

        public void FitImage()
        {
            if (cogRecord == null) return;

            cogRecord.Fit();
        }

        public void ClearImage()
        {
            if (cogRecord == null) return;

            cogRecord.Image = null;
        }

        public void ClearGraphics()
        {
            if (cogRecord == null || cogRecord.InteractiveGraphics == null) return;

            cogRecord.InteractiveGraphics.Clear();
        }
        public void RemoveGraphic(string gropName)
        {
            if (cogRecord == null || cogRecord.InteractiveGraphics == null) return;

            if (cogRecord.InteractiveGraphics.FindItem(gropName, Cognex.VisionPro.Display.CogDisplayZOrderConstants.Front) != -1)
                cogRecord.InteractiveGraphics.Remove(gropName);
        }

        #endregion

        #region CogRecord Event

        private void CogRecord_Click(object sender, EventArgs e)
        {
            var display = sender as CogRecordDisplay;

            display.Name = displayName.ToString();

            CogDisplayClick?.Invoke(display, e);
        }

        private void CogRecord_DoubleClick(object sender, EventArgs e)
        {
            var display = sender as CogRecordDisplay;

            display.Name = displayName.ToString();

            CogDisplayDoubleClick?.Invoke(display, e);
        }

        private void CogRecord_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var display = sender as CogRecordDisplay;

            display.Name = displayName.ToString();

            CogDisplayMouseMove?.Invoke(display, e);
        }

        private void CogRecord_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var display = sender as CogRecordDisplay;

            display.Name = displayName.ToString();

            CogDisplayMouseUp?.Invoke(display, e);
        }

        private void CogRecord_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //var display = sender as CogRecordDisplay;

            //display.Name = displayName.ToString();

            //var pointX = e.X;
            //var pointY = e.Y;

            //if (IsCustomGrid)
            //    SetCustomGrid(pointX, pointY, display.Width, display.Height);

            //CogDisplayMouseDown?.Invoke(display, e);
        }
        private XY GetDisplayPointToImagePoint(XY inputXY)
        {
            CogTransform2DLinear xform;

            double _outputX, _outputY;

            xform = this.cogRecord.GetTransform("#", "*") as CogTransform2DLinear;
            xform.MapPoint(inputXY.X, inputXY.Y, out _outputX, out _outputY);

            return new XY(_outputX, _outputY);
        }
        #endregion
    }
}
