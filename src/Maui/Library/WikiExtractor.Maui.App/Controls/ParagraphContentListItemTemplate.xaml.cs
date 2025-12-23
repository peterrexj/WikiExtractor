using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace WikiExtractor.Maui.App.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ParagraphContentListItemTemplate : DataTemplate
    {
        public ParagraphContentListItemTemplate()
        {
            InitializeComponent();
        }
    }
}