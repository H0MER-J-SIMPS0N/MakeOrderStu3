using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MakeOrderStu3.Views
{
    public class PatientView : UserControl
    {
        public PatientView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
