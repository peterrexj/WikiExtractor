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

        public List<WikiWhatToExtractModel> ListByDependencyArea_ForCountries(string route, List<string>? tags)
        {
            return wikiFinder.ListByDependencyArea_ForCountries(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
        }

        public List<WikiWhatToExtractModel> ListByDependencyArea_ForNonCountries(string route, List<string>? tags)
        {
            return wikiFinder.ListByDependencyArea_ForNonCountries(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
        }
    }
}
