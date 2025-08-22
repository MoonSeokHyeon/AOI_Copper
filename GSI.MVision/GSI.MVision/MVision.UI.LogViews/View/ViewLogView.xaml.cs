using MVision.Common.Event.EventArg;
using MVision.Common.Shared;
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
    /// ViewLogView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ViewLogView : UserControl
    {
        public ViewLogViewModel ViewModel => this.DataContext as ViewLogViewModel;

        public ViewLogView()
        {
            InitializeComponent();
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            var data = e.Row.DataContext as SchedulerEventArgs;
            switch (data.SubKind)
            {
                case eMessageLevelKind.Warn:
                    e.Row.Foreground = Brushes.Brown;
                    break;
                case eMessageLevelKind.Fail:
                    e.Row.Foreground = Brushes.Red;
                    break;
                case eMessageLevelKind.Info:
                    e.Row.Foreground = Brushes.Black;
                    break;
                default:
                    break;
            }
        }
    }
}
