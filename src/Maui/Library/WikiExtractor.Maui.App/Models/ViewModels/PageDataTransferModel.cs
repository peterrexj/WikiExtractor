using System;
using System.Collections.Generic;
using System.Text;

namespace WikiExtractor.Maui.App.Models
{
    public class PageDataTransferModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsMarkedAsViewed { get; set; }

        public void Clear()
        {
            Id = 0;
            Name = string.Empty;
            IsMarkedAsViewed = false;
        }
    }
}
