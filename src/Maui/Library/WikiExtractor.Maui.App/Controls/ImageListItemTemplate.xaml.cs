using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Pj.Library;
using System;
using System.Linq;
using Microsoft.Maui.Controls.Shapes;

namespace WikiExtractor.Maui.App.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImageListItemTemplate : DataTemplate
    {
        public ImageListItemTemplate()
        {
            InitializeComponent();
        }

        private void Border_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                if (sender != null)
                {
                    var width = ((Border)sender).Bounds.Width;
                    if (width > 600) width = 600; //For Tablet with higher width, the width is set back to 600
                    var automationId = ((Border)sender).AutomationId?.SplitAndTrim(",")?.ToList();
                    if (width > 0 && automationId?.Count() == 2)
                    {
                        //item 0 - height
                        //item 1 - width

                        var actualHeight = (automationId[0].ToDouble() / automationId[1].ToDouble()) * width;
                        ((Border)sender).HeightRequest = actualHeight;
                        //For tablets, since the width is shortened, the picture will sit in the centre with gaps around the border.
                        //hence removing the border and radius
                        if (width >= 600)
                        {
                            ((Border)sender).WidthRequest = width;
                            ((Border)sender).Stroke = Colors.Transparent;
                            ((Border)sender).StrokeThickness = 1;
                            ((Border)sender).StrokeShape = new RoundRectangle { CornerRadius = 5 };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //CaptureErrorOnPage(ex);
            }
        }
    }
}