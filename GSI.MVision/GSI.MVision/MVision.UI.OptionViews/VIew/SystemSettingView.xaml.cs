using MVision.UI.OptionViews.ViewModel;
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

namespace MVision.UI.OptionViews.VIew
{
    /// <summary>
    /// SystemSettingView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SystemSettingView : UserControl
    {
        public SystemSettingViewModel ViewModel => this.DataContext as SystemSettingViewModel;

        public SystemSettingView()
        {
            InitializeComponent();
            this.Loaded += SystemSettingView_Loaded;
        }

        private void SystemSettingView_Loaded(object sender, RoutedEventArgs e)
        {
            this.ViewModel.Init();
        }
    }
}
