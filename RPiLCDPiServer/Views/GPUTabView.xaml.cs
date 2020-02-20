using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RPiLCDPiServer.Views
{
    public class GPUTabView : UserControl
    {
        public GPUTabView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
