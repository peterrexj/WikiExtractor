using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.Models;

namespace WikiExtractor.Process.Extractor
{
    public class PopesWikiExtractionToStore : WikiExtractionToStoreBase
    {
        private readonly PopesWikiFinder wikiFinder = new PopesWikiFinder();

        public List<WikiWhatToExtractModel> ExtractListTabularByCentury(string route, string century, List<string>? tags)
        {
            return wikiFinder.ExtractByCenturyFromTable(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
        }
    }
}
