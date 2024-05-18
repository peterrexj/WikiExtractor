using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiExtractor.Models
{
    [Serializable]
    public class WikiPageModel
    {
        public WikiPageModel()
        {
            WikiPictureCollection = new List<WikiPictureModel>();
            WikiParaCollection = new List<WikiParagraphModel>();
            MainParagraph = new List<WikiParagraphDetailModel>();
        }
        public string Header { get; set; }
        public string Route { get; set; }
        public List<WikiParagraphDetailModel> MainParagraph { get; set; }
        public List<WikiPictureModel> WikiPictureCollection { get; set; }
        public List<WikiParagraphModel> WikiParaCollection { get; set; }
    }

}
