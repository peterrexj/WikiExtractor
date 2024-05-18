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
        private readonly SaintsWikiFinder wikiFinder = new SaintsWikiFinder();

        public List<WikiWhatToExtractModel> ExtractListTabularData(string route, List<string>? tags)
        {
            return wikiFinder.ExtractListTabularData(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
        }
        public List<WikiWhatToExtractModel> ExtractPatronSaintsListData(string route, List<string>? tags)
        {
            return wikiFinder.ExtractPatronSaintsListData(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
        }
        public List<WikiWhatToExtractModel> ExtractByAllPopeListData(string route, List<string>? tags)
        {
            return wikiFinder.ExtractByAllPopeListData(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
        }
        public List<WikiWhatToExtractModel> ExtractByEachPopeListData(string route, List<string>? tags)
        {
            return wikiFinder.ExtractByEachPopeListData(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
        }
        public List<WikiWhatToExtractModel> ExtractByCentury(string route, List<string>? tags)
        {
            return wikiFinder.ExtractByCentury(WikiPageRouteResponseAsHtmlDocument(route, null), tags);
        }
    }
}
