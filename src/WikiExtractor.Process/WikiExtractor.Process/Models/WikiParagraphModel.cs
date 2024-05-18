using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiExtractor.Models
{
    [Serializable]
    public class WikiParagraphModel
    {
        public WikiParagraphModel()
        {
            ParagraghInternalModels = new List<WikiParagraphDetailModel>();
        }
        public int Header2InternalId { get; set; }
        public int Sequence { get; set; }
        public string? Header { get; set; }
        public List<WikiParagraphDetailModel> ParagraghInternalModels { get; set; }
    }
}
