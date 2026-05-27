using HtmlAgilityPack;
using Pj.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.DbModels;
using WikiExtractor.Models;

namespace WikiExtractor.Process.Extractor
{
    public class WikiExtractionToStoreBase
    {
        protected readonly ParagraphExtractor paragraphExtractor = new ParagraphExtractor();
        protected readonly MetadataExtractor metadataExtractor = new MetadataExtractor();
        protected readonly StoreProcess storeProcess = new StoreProcess();

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
                   .AddDefaultWebHeaders()
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
