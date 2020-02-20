using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RPiLCDPiServer.Views
{
    public class CPUTabView : UserControl
    {
        public CPUTabView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
