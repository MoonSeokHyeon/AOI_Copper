using MVision.UI.CogDisplayViews.ViewModel;
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

namespace MVision.UI.CogDisplayViews.View
{
    /// <summary>
    /// SingleCamView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SingleCamView : UserControl
    {
        public SingleCamViewModel ViewModel { get => this.DataContext as SingleCamViewModel; }

        public SingleCamView()
        {
            InitializeComponent();

            this.Loaded += SingleCamView_Loaded;
        }

        private void SingleCamView_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Init();
        }

    }
}
