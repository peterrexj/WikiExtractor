using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiExtractor.Models
{
    [Serializable]
    public class WikiPictureModel
    {
        public WikiPictureModel()
        {
            CustomMetadata = new Dictionary<string, string>();
        }
        public long Id { get; set; }
        public Guid PictureId { get; set; }
        public int Sequence { get; set; }
        public string? Caption { get; set; }
        public Dictionary<string, string> CustomMetadata { get; set; }
    }
}
