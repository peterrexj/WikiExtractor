using Pj.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.Process;
using WikiExtractor.Process.Extractor;

namespace WikiExtractor.Tests
{
    internal class WikiPageExtractionTests
    {
        SaintsWikiExtractionToStore wikiPageExtraction;
        private const string Route = "/wiki/Pope_John_Paul_II";

        [SetUp]
        public void Setup()
        {
            ProcessConstants.DatabasePath = IoHelper.CombinePath(PjUtility.Runtime.ExecutingFolder, "Tests", "Db", "WikiStore.db");
            ProcessConstants.UserStoreDatabasePath = IoHelper.CombinePath(PjUtility.Runtime.ExecutingFolder, "Tests", "Db", "UserStore.db");
            ProcessConstants.CacheFolder = IoHelper.CombinePath(PjUtility.Runtime.ExecutingRepositoryRootFolder, "Tests", "Cache");
            //IoHelper.DeleteFile(ProcessConstants.DatabasePath);
            wikiPageExtraction = new SaintsWikiExtractionToStore();
        }

        [Order(1)]
        [TestCase(Route)]
        public void Shoud_Return_Reponse_Live(string route)
        {
            IoHelper.RecursiveDeleteFolder(ProcessConstants.CacheFolder);
            var response = wikiPageExtraction.WikiPageRouteResponse(route);
            Assert.IsNotNull(response);
            Assert.IsNotEmpty(response);
        }

        [Order(2)]
        [TestCase(Route)]
        public void Should_Return_HtmlDocument_Live(string route)
        {
            var htmlDoc = wikiPageExtraction.WikiPageRouteResponseAsHtmlDocument(route, null);
            Assert.That(htmlDoc, Is.Not.Null);
        }


        [TestCase(Route)]
        public void Should_Return_Response_WhichMatchTemplate(string route)
        {
            var response = wikiPageExtraction.WikiPageRouteResponse(route);

        }
    }
}
