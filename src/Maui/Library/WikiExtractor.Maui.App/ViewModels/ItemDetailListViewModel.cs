using System.ComponentModel;

namespace WikiExtractor.Maui.App.ViewModels
{
    public class ItemDetailListViewModel : INotifyPropertyChanged
    {
        public string Type { get; set; }
        public string Content { get; set; }
        public int ContentLinkId { get; set; }
        public bool IsPlayButtonRequired { get; set; }

        private bool _isPlaying;
        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                if (_isPlaying != value)
                {
                    _isPlaying = value;
                    OnPropertyChanged(nameof(IsPlaying));
                }
            }
        }

        private string _imageLocalPath;
        public string ImageLocalPath
        {
            get => _imageLocalPath;
            set
            {
                if (_imageLocalPath != value)
                {
                    _imageLocalPath = value;
                    OnPropertyChanged(nameof(ImageLocalPath));
                }
            }
        }
        public string ImageFileName { get; set; }
        public double ImageHeight { get; set; }
        public string ImageDimension { get; set; }
        public string ImageCaption { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}