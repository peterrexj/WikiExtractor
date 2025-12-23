using Microsoft.Maui.Controls;
using WikiExtractor.Maui.App.ViewModels;

namespace WikiExtractor.Maui.App.Controls
{
    public class ItemDetailListTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Header2Template { get; set; }
        public DataTemplate Header3Template { get; set; }
        public DataTemplate ParagraphContentTemplate { get; set; }
        public DataTemplate ImageTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is not ItemDetailListViewModel currentItem)
                return null;

            switch (currentItem.Type)
            {
                case "Header2":
                    return Header2Template;
                case "Header3":
                    return Header3Template;
                case "Header2Text":
                case "Header3Text":
                case "Text":
                    return ParagraphContentTemplate;
                case "Image":
                    return ImageTemplate;
                default:
                    return ParagraphContentTemplate;
            }
        }
    }
}