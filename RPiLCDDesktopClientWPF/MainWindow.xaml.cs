using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RPiLCDDesktopClientWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIcon _notifyIcon;
        public MainWindow()
        {
            InitializeComponent();

            _notifyIcon = new NotifyIcon
            {
                Icon = new System.Drawing.Icon("./Assets/avalonia-logo.ico"),
                Visible = true,
                BalloonTipText = "RPiLCD Client is now running in system tray.",
                BalloonTipTitle = "RPiLCD Client Minimised",
                BalloonTipIcon = ToolTipIcon.Info
            };

            _notifyIcon.MouseDoubleClick +=new System.Windows.Forms.MouseEventHandler(NotifyIcon_MouseDoubleClick);
            this.StateChanged += new EventHandler(Window_StateChanged);

            this.ShowInTaskbar = true;
            this.WindowState = WindowState.Minimized;
            this.Visibility = Visibility.Hidden;

            this.ShowInTaskbar = false;
        }

        void NotifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            this.Visibility = Visibility.Visible;
            this.WindowState = WindowState.Normal;
        }

        public void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                _notifyIcon.Visible = true;
                this.ShowInTaskbar = false;
                this.Visibility = Visibility.Hidden;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                _notifyIcon.Visible = false;
                this.ShowInTaskbar = true;
                this.Visibility = Visibility.Visible;
            }
        }
    }
}
