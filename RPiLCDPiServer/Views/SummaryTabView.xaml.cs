using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RPiLCDPiServer.Views
{
    public class SummaryTabView : UserControl
    {
        public SummaryTabView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
