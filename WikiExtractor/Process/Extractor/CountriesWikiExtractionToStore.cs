using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.Models;

namespace WikiExtractor.Process.Extractor
{
    public class CountriesWikiExtractionToStore : WikiExtractionToStoreBase
    {
        private readonly CountriesWikiFinder wikiFinder = new CountriesWikiFinder();

        public List<WikiWhatToExtractModel> ListByDependencyArea(string route, List<string>? tags)
        {
            return wikiFinder.ListByDependencyArea(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
        }
    }
}
