namespace WikiExtractor.XamarinForms.ViewModels
{
    public class ItemDetailListViewModel
    {
        public string Type { get; set; }
        public string Content { get; set; }
        public int ContentLinkId { get; set; }
        public bool IsPlayButtonRequired { get; set; }


        public string ImageLocalPath { get; set; }
        public string ImageFileName { get; set; }
        public double ImageHeight { get; set; }
        public string ImageDimension { get; set; }
        public string ImageCaption { get; set; }

    }
}
