using HtmlAgilityPack;
using Pj.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.Models;

namespace WikiExtractor.Process.Extractor
{
    public class SaintsWikiExtractionToStore : WikiExtractionToStoreBase
    {
        private readonly SaintsWikiFinder tabularInformationExtractor = new SaintsWikiFinder();

        public List<WikiWhatToExtractModel> SaintsExtractListTabularData(string route, List<string>? tags)
        {
            return tabularInformationExtractor.SaintsExtractListTabularData(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
        }
        public List<WikiWhatToExtractModel> SaintsExtractPatronSaintsListData(string route, List<string>? tags)
        {
            return tabularInformationExtractor.SaintsExtractPatronSaintsListData(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
        }
        public List<WikiWhatToExtractModel> SaintsExtractByAllPopeListData(string route, List<string>? tags)
        {
            return tabularInformationExtractor.SaintsExtractByAllPopeListData(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
        }
        public List<WikiWhatToExtractModel> SaintsExtractByEachPopeListData(string route, List<string>? tags)
        {
            return tabularInformationExtractor.SaintsExtractByEachPopeListData(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
        }
        public List<WikiWhatToExtractModel> SaintsExtractByCentury(string route, List<string>? tags)
        {
            return tabularInformationExtractor.SaintsExtractByCentury(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
        }
    }
}
