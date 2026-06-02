using HtmlAgilityPack;
using Pj.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TestAny.Essentials.Core.Dtos.Api;
using WikiExtractor.DbModels;
using WikiExtractor.Models;

namespace WikiExtractor.Process.Extractor
{
    public class WikiExtractionToStoreBase
    {
        protected readonly ParagraphExtractor paragraphExtractor = new ParagraphExtractor();
        protected readonly MetadataExtractor metadataExtractor = new MetadataExtractor();
        protected readonly StoreProcess storeProcess = new StoreProcess();

        private static HeaderCollection WebHeaderCollection = new HeaderCollection
                {
                    new TestApiHeader("User-Agent",
                        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                        "AppleWebKit/537.36 (KHTML, like Gecko) " +
                        "Chrome/136.0.0.0 Safari/537.36"),

                    new TestApiHeader("Accept",
                        "text/html,application/xhtml+xml,application/xml;q=0.9," +
                        "image/avif,image/webp,image/apng,*/*;q=0.8"),

                    new TestApiHeader("Accept-Language",
                        "en-AU,en;q=0.9"),

                    new TestApiHeader("Accept-Encoding",
                        "gzip, deflate, br"),

                    new TestApiHeader("Cache-Control", "max-age=0"),

                    new TestApiHeader("Upgrade-Insecure-Requests", "1"),

                    // Modern Chromium client hints
                    new TestApiHeader("sec-ch-ua",
                        "\"Chromium\";v=\"136\", \"Google Chrome\";v=\"136\", \"Not.A/Brand\";v=\"99\""),

                    new TestApiHeader("sec-ch-ua-mobile", "?0"),

                    new TestApiHeader("sec-ch-ua-platform", "\"Windows\""),

                    // Fetch metadata
                    new TestApiHeader("Sec-Fetch-Dest", "document"),
                    new TestApiHeader("Sec-Fetch-Mode", "navigate"),
                    new TestApiHeader("Sec-Fetch-Site", "none"),
                    new TestApiHeader("Sec-Fetch-User", "?1")
                };

        protected string GetRouteToFileName(string route)
        {
            return IoHelper.CombinePath(ProcessConstants.CacheFolder, $"{route.RemoveSpecialChars()}.txt");
        }
        protected string GetFromCache(string route)
        {
            if (ProcessConstants.UseCache == false) return string.Empty;

            var file = GetRouteToFileName(route);
            if (IoHelper.FileExists(file))
            {
                return File.ReadAllText(file);
            }
            else return string.Empty;
        }
        protected void ToCache(string route, string content)
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
                   .AddHeaders(WebHeaderCollection)
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

                if (ProcessConstants.RequestDelayInMilliseconds > 0)
                {
                    Console.WriteLine($"Delaying for {ProcessConstants.RequestDelayInMilliseconds} ms to respect server load...");
                    Thread.Sleep(ProcessConstants.RequestDelayInMilliseconds);
                }
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

        public List<WikiWhatToExtractModel> GenericLoadUrlFile(string filePath, List<string>? tags)
        {
            var whatToExtract = new List<WikiWhatToExtractModel>();
            if (File.Exists(filePath))
            {
                int counter = 1;
                var urls = File.ReadAllLines(filePath).Select(f => f.Trim().Replace(f.GetDomain(), ""));
                foreach (var url in urls)
                {
                    var response = WikiPageRouteResponseAsHtmlDocument(url, null);
                    var paraInfo = paragraphExtractor.ExtractParaInfo(response, url, string.Empty);
                    whatToExtract.Add(new WikiWhatToExtractModel { Route = url, Title = paraInfo.Header, Tags = tags, Sequence = counter++ });
                }
            }
            return whatToExtract;
        }

        public (WikiPageModel, List<MetaDataModel>) SinglePageContentExtract(WikiWhatToExtractModel wikiData,
            List<string> excludedAdditionalMetadata = null)
        {
            var response = WikiPageRouteResponseAsHtmlDocument(wikiData.Route, null);
            var paraInfo = paragraphExtractor.ExtractParaInfo(response, wikiData.Route, wikiData.Title);
            var metadata = metadataExtractor.ExtractMetadataInfo(response, wikiData.AdditionalMetaData, excludedAdditionalMetadata);
            return (paraInfo, metadata);
        }
        public int SinglePageContentStore(WikiPageModel wikiPageModel, List<MetaDataModel> metaDatas,
            WikiWhatToExtractModel wikiData)
        {
            return storeProcess.StoreInformation(wikiPageModel, metaDatas, wikiData);
        }

        public int PersonaSinglePageContentExtractWithSaveToStore(WikiWhatToExtractModel wikiData,
            List<string> excludedAdditionalMetadata = null)
        {
            var response = WikiPageRouteResponseAsHtmlDocument(wikiData.Route, null);

            var paraInfo = paragraphExtractor.ExtractParaInfo(response, wikiData.Route, wikiData.Title);
            var metadata = metadataExtractor.ExtractMetadataInfo(response, wikiData.AdditionalMetaData, excludedAdditionalMetadata);

            return storeProcess.StoreInformation(paraInfo, metadata, wikiData);
        }

        public void UpdateTags(List<string> tags, int masterId)
        {
            storeProcess.StoreTags(tags, masterId);
        }

        public void CleanEntry(int masterId)
        {
            storeProcess.CleanEntry(masterId);
        }

        public void UpdateName(string name, int masterId)
        {
            storeProcess.UpdateName(name, masterId);
        }
    }
}
