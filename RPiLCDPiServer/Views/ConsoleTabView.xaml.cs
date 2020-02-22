using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RPiLCDPiServer.Views
{
    public class ConsoleTabView : UserControl
    {
        public ConsoleTabView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
