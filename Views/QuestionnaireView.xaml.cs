using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MakeOrderStu3.Views
{
    public class QuestionnaireView : UserControl
    {
        public QuestionnaireView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
