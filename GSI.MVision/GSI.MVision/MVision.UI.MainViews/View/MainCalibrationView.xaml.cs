using MVision.UI.MainViews.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MVision.UI.MainViews.View
{
    /// <summary>
    /// MainCalibrationView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainCalibrationView : UserControl
    {
        public MainCalibrationViewModel ViewModel => this.DataContext as MainCalibrationViewModel;

        public MainCalibrationView()
        {
            InitializeComponent();
            this.Loaded += MainCalibrationView_Loaded;

        }
        private void MainCalibrationView_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Init();
        }
        private void PositionSelectedChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue == null) return;

            var header = e.NewValue.ToString();

            this.ViewModel.PositionChanged(header);
        }
    }
}
