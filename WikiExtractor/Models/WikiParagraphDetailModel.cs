using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiExtractor.Models
{
    [Serializable]
    public class WikiParagraphDetailModel
    {
        public WikiParagraphDetailModel()
        {
            ContentBuilder = new StringBuilder();
        }
        public int Header2InternalId { get; set; }
        public int Header3InternalId { get; set; }
        public int Sequence { get; set; }
        public string? SubHeader { get; set; }
        public string Content => ContentBuilder.ToString();
        public StringBuilder ContentBuilder { get; set; }
    }
}
