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
    /// MainAutoView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainAutoView : UserControl
    {
        public MainAutoViewModel ViewModel => this.DataContext as MainAutoViewModel;

        public MainAutoView()
        {
            InitializeComponent();
            this.Loaded += MainAutoView_Loaded;

        }

        private void MainAutoView_Loaded(object sender, RoutedEventArgs e)
        {
            this.ViewModel.Init();
        }
    }
}
