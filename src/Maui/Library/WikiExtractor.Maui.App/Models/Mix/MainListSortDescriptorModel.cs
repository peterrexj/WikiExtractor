using System;
using System.Collections.Generic;
using System.Text;
using Syncfusion.Maui.DataSource;
// using Syncfusion.Maui.DataSource; // Temporarily disabled
using WikiExtractor.Maui.App.Exts; // For temporary ListSortDirection enum
using WikiExtractor.ViewModels;

namespace WikiExtractor.Maui.App.Models.Mix
{
    public class MainListSortDescriptorModel : BasePropertyChangeModel
    {
        public enum SortByAttribute
        {
            Default,
            AtoZ,
            ZtoA,
            Read,
            UnRead,
            Random
        }

        private string propertyName;
        public string PropertyName
        {
            get => propertyName; set
            {
                propertyName = value;
                OnPropertyChanged("PropertyName");
            }
        }

        private ListSortDirection direction;
        public ListSortDirection Direction
        {
            get => direction; set
            {
                direction = value;
                OnPropertyChanged("Direction");
            }
        }
    }
}
