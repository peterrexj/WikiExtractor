using Pj.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WikiExtractor.Exts;

namespace WikiExtractor.ViewModels
{
    public class PersonaViewModel : BaseViewModel, IListDynamicHeight
    {
        public PersonaViewModel()
        {
            Metadatas = new List<MetadataViewModel>();
            Pictures = new List<PictureViewModel>();
            Paragraphs = new List<Paragraph2ContentViewModel>();
            PrimaryMetadataContent = new List<MetadataViewModel>();
        }
        public int Id { get; set; }

        private int randomId;
        public int RandomId
        {
            get => randomId; set
            {
                randomId = value;
                OnPropertyChanged("RandomId");
            }
        }

        public string Name { get; set; }
        public string NameSubstitue { get; set; }
        public string NameSubstitueFormatted => NameSubstitue.HasValue() ? $"{Environment.NewLine}({NameSubstitue})" : string.Empty;
        public string WikiPath { get; set; }
        public string MainContent { get; set; }
        public string PicturePrimaryPath { get; set; }
        public string PicturePrimaryLocalFileName => $"{Name.RemoveSpecialChars(excludeUnderscore: false)}{Path.GetExtension(PicturePrimaryPath)}";
        public string PicturePrimaryCaption { get; set; }
        public int PicturePrimaryWidth { get; set; }
        public int PicturePrimaryHeight { get; set; }
        public bool IsPrimaryMetadataContentEnabled { get; set; }
        public bool ShowPrimaryContentMetadata => IsPrimaryMetadataContentEnabled;
        public bool HidePrimaryContentMetadata => !IsPrimaryMetadataContentEnabled;

        public bool _itemReadStatus;
        public bool ItemReadStatus
        {
            get => _itemReadStatus;
            set
            {
                _itemReadStatus = value;
                OnPropertyChanged("ItemReadStatus");
            }
        }

        public List<MetadataViewModel> PrimaryMetadataContent { get; set; }
        public List<MetadataViewModel> Metadatas { get; set; }

        private List<PictureViewModel> _pictures;
        public List<PictureViewModel> Pictures { get => _pictures; set => SetProperty(ref _pictures, value); }

        public List<Paragraph2ContentViewModel> Paragraphs { get; set; }
        public List<string> Tags { get; set; }

        private double _listHeight;
        public double ListHeight
        {
            get
            {
                return _listHeight;
            }
            set
            {
                _listHeight = value;
                OnPropertyChanged("ListHeight");
            }
        }
        //public List<(string PicturePrimaryPath, string PicturePrimaryCaption)> PictureImages { get; }
    }

    public class MetadataViewModel
    {
        public string Key { get; set; }
        public string Description { get; set; }
        public int Sequence { get; set; }
        public string GroupHeader { get; set; }
    }

    public class PictureViewModel : BaseViewModel
    {
        public long Id { get; set; }

        private string _picturePath;
        public string PicturePath
        {
            get => _picturePath;
            set
            {
                _picturePath = value;
                OnPropertyChanged("PicturePath");
                OnPropertyChanged("PictureLocalFileName");
                OnPropertyChanged("PictureLocalPath");
            }
        }

        private string _pictureCaption;
        public string PictureCaption { get => _pictureCaption; set => SetProperty(ref _pictureCaption, value); }

        public int Sequence { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public string ParentName { get; set; }

        private int _currentCounter;
        public int CurrentCounter
        {
            get => _currentCounter; set
            {
                _currentCounter = value;
                OnPropertyChanged("CurrentCounter");
                OnPropertyChanged("PictureLocalFileName");
                OnPropertyChanged("PictureLocalPath");
            }
        }

        public string PictureLocalFileName => $"{ParentName.RemoveSpecialChars(excludeUnderscore: false)}{CurrentCounter}{Path.GetExtension(PicturePath)}";
        public string PictureLocalPath => Path.Combine(ConfigData.LocalStorageCacheFolderPath, PictureLocalFileName);
    }

    public class Paragraph2ContentViewModel
    {
        public Paragraph2ContentViewModel()
        {
            PicLinks = new List<PictureViewModel>();
            Para3Containers = new List<Paragraph3ContainerViewModel>();
        }
        public string Header2 { get; set; }
        public string Content { get; set; }
        public int Sequence { get; set; }
        public List<Paragraph3ContainerViewModel> Para3Containers { get; set; }
        public bool ContainsHeader3 => Para3Containers != null && Para3Containers?.SelectMany(f => f.Para3s).Any() == true;
        public bool ContainsHeader3Content => ContainsHeader3 && Para3Containers?.SelectMany(f => f.Para3s).Any(f => f.Content.HasValue()) == true;

        public bool ContainsHeader2Content => Content.HasValue();
        public List<PictureViewModel>? PicLinks { get; set; }
        public int Id { get; set; }
    }

    public class Paragraph3ContainerViewModel
    {
        public Paragraph3ContainerViewModel()
        {
            Para3s = new List<Paragraph3ContentViewModel>();
        }
        public string Header { get; set; }
        public List<Paragraph3ContentViewModel> Para3s { get; set; }
    }

    public class Paragraph3ContentViewModel
    {
        public Paragraph3ContentViewModel()
        {
            PicLinks = new List<PictureViewModel>();
        }
        public int Id { get; set; }
        public int Sequence { get; set; }
        public string Content { get; set; }
        public List<PictureViewModel> PicLinks { get; set; }
    }

    public class PersonaAutoCompleteModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
