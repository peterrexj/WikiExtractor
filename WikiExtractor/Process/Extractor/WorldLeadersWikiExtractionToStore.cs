using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.Models;

namespace WikiExtractor.Process.Extractor
{
    public class WorldLeadersWikiExtractionToStore : WikiExtractionToStoreBase
    {
        private readonly WorldLeadersWikiFinder wikiFinder = new();

        public List<WikiWhatToExtractModel> ExtractListTabularData_Australia(string route, List<string>? tags)
        {
            return wikiFinder.ExtractListTabularData_Australia(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
        }

        public List<WikiWhatToExtractModel> ExtractListTabularData_India(string route, List<string>? tags)
        {
            return wikiFinder.ExtractListTabularData_India(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
        }

        public List<WikiWhatToExtractModel> ExtractListTabularData_UnitedStates(string route, List<string>? tags)
        {
            return wikiFinder.ExtractListTabularData_UnitedStates(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
        }

        public List<WikiWhatToExtractModel> ExtractListTabularData_UnitedKingdom(string route, List<string>? tags)
        {
            return wikiFinder.ExtractListTabularData_UnitedKingdom(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
        }

    }
}
