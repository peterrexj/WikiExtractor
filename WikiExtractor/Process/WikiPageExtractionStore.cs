using HtmlAgilityPack;
using OpenQA.Selenium.Remote;
using Pj.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.Models;

namespace WikiExtractor.Process
{
    public class WikiPageExtractionStore
    {
        private readonly ParagraphExtractor paragraphExtractor = new ParagraphExtractor();
        private readonly WikiInformationExtractor tabularInformationExtractor = new WikiInformationExtractor();
        private readonly MetadataExtractor metadataExtractor = new MetadataExtractor();
        private readonly StoreProcess storeProcess = new StoreProcess();

        private string GetRouteToFileName(string route)
        {
            return IoHelper.CombinePath(ProcessConstants.CacheFolder, $"{route.RemoveSpecialChars()}.txt");
        }
        private string GetFromCache(string route)
        {
            var file = GetRouteToFileName(route);
            if (IoHelper.FileExists(file))
            {
                return File.ReadAllText(file);
            }
            else return string.Empty;
        }
        private void ToCache(string route, string content)
        {
            var file = GetRouteToFileName(route);
            IoHelper.CreateDirectory(file);
            IoHelper.DeleteFile(file);
            File.WriteAllText(file, content);
        }

        public string WikiPageRouteResponse(string route)
        {
            var cache = GetFromCache(route);
            if (cache.IsEmpty())
            {
                var resp = new TestApiHttp()
                   .SetEnvironment("https://en.wikipedia.org")
                   .PrepareRequest(route)
                   .GetWithRetry(
                       assertOk: true,
                       timeToSleepBetweenRetryInMilliseconds: 1000,
                       retryOption: 10,
                       throwExceptionOnAssertFail: true,
                       retryOnRequestTimeout: true,
                       httpStatusCodes: new[] { HttpStatusCode.ProxyAuthenticationRequired }
                   );

                resp.AssertResponseStatusForSuccess();
                ToCache(route, resp.ResponseBody.ContentString);
                cache = resp.ResponseBody.ContentString;
            }
            return cache;
        }

        public HtmlDocument WikiPageRouteResponseAsHtmlDocument(string route, string content)
        {
            var doc = new HtmlDocument();
            if (content.IsEmpty())
            {
                content = WikiPageRouteResponse(route);
            }
            doc.LoadHtml(content);
            return doc;
        }

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

        public int PersonaSinglePageContentExtractWithSaveToStore(WikiWhatToExtractModel wikiData)
        {
            var response = WikiPageRouteResponseAsHtmlDocument(wikiData.Route, null);

            var paraInfo = paragraphExtractor.ExtractParaInfo(response, wikiData.Route, wikiData.Title);
            var metadata = metadataExtractor.ExtractMetadataInfo(response);

            return storeProcess.StoreInformation(paraInfo, metadata, wikiData);
        }
    }
}
