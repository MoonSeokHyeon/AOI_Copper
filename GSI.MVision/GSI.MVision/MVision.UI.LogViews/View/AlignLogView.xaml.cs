using MVision.UI.LogViews.ViewModel;
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

namespace MVision.UI.LogViews.View
{
    /// <summary>
    /// AlignLogView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AlignLogView : UserControl
    {
        public AlignLogViewModel ViewModel { get => this.DataContext as AlignLogViewModel; }

        public AlignLogView()
        {
            InitializeComponent();
            this.Loaded += AlignLogView_Loaded;
        }
        private void AlignLogView_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Init();
        }
    }
}
