using DOTNET.Logging;
using DOTNET.Quartz;
using DOTNET.WPF.Extensions;
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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GSI.MVision
{
    /// <summary>
    /// Shell.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Shell : Window
    {
        Logger logger = Logger.GetLogger();

        public ShellViewModel ViewModel { get => this.DataContext as ShellViewModel; }
        public Shell()
        {
            InitializeComponent();
            this.Loaded += Shell_Loaded;
            this.Closing += Shell_Closing;

            this.windowHead.MouseLeftButtonDown += WindowHead_MouseLeftButtonDown;

        }


        private void Shell_Loaded(object sender, RoutedEventArgs e)
        {
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            WindowExternal.MaximizeToFirstMonitor(this);
            this.iconWindowMaxState.Kind = MaterialDesignThemes.Wpf.PackIconKind.WindowRestore;

            (this.DataContext as ShellViewModel).Init();


        }
        private void Shell_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void Control_ContentRendered(object sender, EventArgs e)
        {
            // Don't forget to unsubscribe from the event
            ((PresentationSource)sender).ContentRendered -= Control_ContentRendered;

            //App.splashScreen.AddMessage("Done !");
            //GSG.NET.Quartz.TimerUtils.Once(500, () => { App.splashScreen.LoadComplete(); });
        }
        private void WindowHead_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (e.ClickCount > 1)
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    this.WindowState = WindowState.Normal;
                    this.iconWindowMaxState.Kind = MaterialDesignThemes.Wpf.PackIconKind.WindowMaximize;
                }
                else
                {
                    WindowExternal.MaximizeToFirstMonitor(this);
                    this.iconWindowMaxState.Kind = MaterialDesignThemes.Wpf.PackIconKind.WindowRestore;
                }
            }
            else
                this.DragMove();
        }

        private void WindowMinimizeClicked(object sender, RoutedEventArgs e)
        {
            if (this.WindowState != WindowState.Minimized)
                this.WindowState = WindowState.Minimized;
        }

        private void WindowMaximizeClicked(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                this.iconWindowMaxState.Kind = MaterialDesignThemes.Wpf.PackIconKind.WindowMaximize;
            }
            else
            {
                WindowExternal.MaximizeToFirstMonitor(this);
                this.iconWindowMaxState.Kind = MaterialDesignThemes.Wpf.PackIconKind.WindowRestore;
            }
        }

    }
}
