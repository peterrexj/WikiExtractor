using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiExtractor.Models
{
    public class WikiWhatToExtractModel
    {
        public string Route { get; set; }
        public string Title { get; set; }
        public List<string>? Tags { get; set; }
        public int Sequence { get; set; }
    }
}
