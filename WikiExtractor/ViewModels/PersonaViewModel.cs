using Pj.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiExtractor.ViewModels
{
    public class PersonaViewModel
    {
        public string Name { get; set; }
        public string WikiPath { get; set; }
        public string MainContent { get; set; }
        public string PicturePrimaryPath { get; set; }
        public string PicturePrimaryCaption { get; set; }
        public List<MetadataViewModel> Metadatas { get; set; }
        public List<PictureViewModel> Pictures { get; set; }
        public List<ParagraphContentViewModel> Paragraphs { get; set; }
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
        public string PicturePath { get; set; }
        public string PictureCaption { get; set; }
        public int Sequence { get; set; }
    }

    public class ParagraphContentViewModel
    {
        public string Header2 { get; set; }
        public string Header3 { get; set; }
        public string Content { get; set; }
        public int Sequence { get; set; }
        public bool ContainsSubHeader3 => Header3.HasValue();
    }
}
