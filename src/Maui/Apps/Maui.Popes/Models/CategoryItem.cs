using System;

namespace WikiExtractor.Maui.App.Models
{
    public class CategoryItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconName { get; set; }
        public int SortOrder { get; set; }
    }
}