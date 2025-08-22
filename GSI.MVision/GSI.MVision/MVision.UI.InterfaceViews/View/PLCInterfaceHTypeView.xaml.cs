using MVision.UI.InterfaceViews.ViewModel;
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

namespace MVision.UI.InterfaceViews.View
{
    /// <summary>
    /// PLCInterfaceHTypeView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PLCInterfaceHTypeView : UserControl
    {
        public PLCInterfaceHTypeViewModel ViewModel => this.DataContext as PLCInterfaceHTypeViewModel;

        public PLCInterfaceHTypeView()
        {
            InitializeComponent();
            this.Loaded += PLCInterfaceHTypeView_Loaded;

        }
        private void PLCInterfaceHTypeView_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Init();
        }
    }
}
