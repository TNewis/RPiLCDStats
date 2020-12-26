using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RPiLCDPiServer.Views
{
    public class AudioTabView : UserControl
    {
        public AudioTabView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
