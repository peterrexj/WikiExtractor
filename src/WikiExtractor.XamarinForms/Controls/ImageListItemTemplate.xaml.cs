using Pj.Library;
using Syncfusion.XForms.Border;
using System;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WikiExtractor.XamarinForms.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImageListItemTemplate : DataTemplate
    {
        public ImageListItemTemplate()
        {
            InitializeComponent();
        }

        private void SfBorder_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                if (sender != null)
                {
                    var width = ((SfBorder)sender).Bounds.Width;
                    if (width > 600) width = 600; //For Tablet with higher width, the width is set back to 600
                    var automationId = ((SfBorder)sender).AutomationId?.SplitAndTrim(",")?.ToList();
                    if (width > 0 && automationId?.Count() == 2)
                    {
                        //item 0 - height
                        //item 1 - width

                        var actualHeight = (automationId[0].ToDouble() / automationId[1].ToDouble()) * width;
                        ((SfBorder)sender).HeightRequest = actualHeight;
                        //For tablets, since the width is shortend, the picture will sit in the centre with gaps around the border.
                        //hence removing the border and radius
                        if (width >= 600)
                        {
                            ((SfBorder)sender).WidthRequest = width;
                            ((SfBorder)sender).BorderColor = Color.Transparent;
                            ((SfBorder)sender).BorderWidth = 1;
                            ((SfBorder)sender).CornerRadius = 5;
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