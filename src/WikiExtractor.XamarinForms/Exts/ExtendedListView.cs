using Microsoft.AppCenter.Crashes;
using Pj.Library;
using Syncfusion.ListView.XForms;
using Syncfusion.ListView.XForms.Control.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.Exts;
using WikiExtractor.ViewModels;
using Xamarin.Forms;

namespace GeneralInformation
{
    public class ExtendedListView : SfListView
    {
        VisualContainer container;
        public ExtendedListView()
        {
            container = this.GetVisualContainer();
            container.PropertyChanged += Container_PropertyChanged;
        }

        private void Container_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Run(() =>
                    {
                        if (e.PropertyName.IsEmpty() || e.PropertyName != "Height" || this.BindingContext == null) return;
                        var totalextent = (double)container.GetType().GetRuntimeProperties().FirstOrDefault(container => container.Name == "TotalExtent").GetValue(container);
                        if (totalextent > 0)
                        {
                            if (totalextent < ConfigData.MinLengthOfPictureCaption)
                            {
                                totalextent = ConfigData.MinLengthOfPictureCaption;
                            }
                            (this.BindingContext as IListDynamicHeight).ListHeight = totalextent;
                        }
                    });
                });
            }
            catch (System.Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}
