using Pj.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.DbModels;

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
        public string Name { get; set; }
        public string NameSubstitue { get; set; }
        public string NameSubstitueFormatted => NameSubstitue.HasValue() ? $"{Environment.NewLine}({NameSubstitue})" : string.Empty;
        public string WikiPath { get; set; }
        public string MainContent { get; set; }
        public string PicturePrimaryPath { get; set; }
        public string PicturePrimaryCaption { get; set; }
        public bool IsPrimaryMetadataContentEnabled { get; set; }
        public bool ShowPrimaryContentMetadata => IsPrimaryMetadataContentEnabled;
        public bool HidePrimaryContentMetadata => !IsPrimaryMetadataContentEnabled;

        public List<MetadataViewModel> PrimaryMetadataContent { get; set; }
        public List<MetadataViewModel> Metadatas { get; set; }
        public List<PictureViewModel> Pictures { get; set; }
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

    public class PictureViewModel
    {
        public long Id { get; set; }
        public string PicturePath { get; set; }
        public string PictureCaption { get; set; }
        public int Sequence { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        
    }

    public class Paragraph2ContentViewModel
    {
        public string Header2 { get; set; }
        public string Content { get; set; }
        public int Sequence { get; set; }
        public List<Paragraph3ContentViewModel>? Para3s { get; set; }
        public bool ContainsHeader3 => Para3s != null && Para3s.Any();
        public bool ContainsHeader2Content => Content.HasValue();
    }

    public class Paragraph3ContentViewModel
    {
        public Paragraph3ContentViewModel()
        {
            PicLinks = new List<PictureViewModel>();
        }
        public string Header3 { get; set; }
        public string Content { get; set; }
        public int Sequence { get; set; }
        public List<PictureViewModel>? PicLinks { get; set; }
    }

    public class PersonaAutoCompleteModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
