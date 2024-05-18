using Pj.Library;
using WikiExtractor.Models;
using WikiExtractor.Process;
using WikiExtractor.Process.Extractor;

namespace WikiExtractor.Tests
{
    internal class ParagraphExtractorTests
    {
        private const string Route = "/wiki/Pope_John_Paul_II";
        private string TemplateRuntimeFile;
        private string TemplateCompileFile;
        private string TemplateCompileItemsFile;
        ParagraphExtractor paragraphExtractor;
        SaintsWikiExtractionToStore wikiPageExtraction;

        [SetUp]
        public void Setup()
        {
            ProcessConstants.DatabasePath = IoHelper.CombinePath(PjUtility.Runtime.ExecutingFolder, "Tests", "Db", "WikiStore.db");
            ProcessConstants.CacheFolder = IoHelper.CombinePath(PjUtility.Runtime.ExecutingRepositoryRootFolder, "Tests", "Cache");
            paragraphExtractor = new ParagraphExtractor();
            wikiPageExtraction = new SaintsWikiExtractionToStore();
            TemplateRuntimeFile = IoHelper.CombinePath(PjUtility.Runtime.ExecutingFolder, "Tests", "TemplateRuntime", "ResponseContent.dat");
            TemplateCompileFile = IoHelper.CombinePath(PjUtility.Runtime.ExecutingFolder, "Templates", "RouteResponse.txt");
            TemplateCompileItemsFile = IoHelper.CombinePath(PjUtility.Runtime.ExecutingFolder, "Templates", "ParagraphItems.json");
        }

        [Order(1)]
        [TestCase(Route)]
        public void Shoud_Return_Reponse_Live(string route)
        {
            IoHelper.DeleteFile(TemplateRuntimeFile);
            IoHelper.CreateDirectory(TemplateRuntimeFile);

            var response = wikiPageExtraction.WikiPageRouteResponse(route);
            Assert.IsNotNull(response);
            Assert.IsNotEmpty(response);

            File.WriteAllText(TemplateRuntimeFile, response);
        }

        [Order(2)]
        [TestCase(Route)]
        public void Should_Return_Items_Paragraph_Live_UnderMainBody(string route)
        {
            Assert.That(TemplateRuntimeFile, Is.Not.Null);
            Assert.That(IoHelper.FileExists(TemplateRuntimeFile), Is.True);
            var htmlDoc = wikiPageExtraction.WikiPageRouteResponseAsHtmlDocument(route, null);
            Assert.That(htmlDoc, Is.Not.Null);

            var item =  paragraphExtractor._ItemsUnderMainBody(htmlDoc);
            Assert.That(item, Is.Not.Null);

            Assert.That(item.Count > 100, Is.True);
        }


        [Order(3)]
        [TestCase(Route)]
        public void Should_Return_Items_Paragraph_Live_Items_NotNull(string route)
        {
            var htmlDoc = wikiPageExtraction.WikiPageRouteResponseAsHtmlDocument(route, null);
            var items = paragraphExtractor.ExtractParaInfo(htmlDoc, route, string.Empty);
            Assert.That(items, Is.Not.Null);
        }

        [Order(3)]
        [TestCase(Route)]
        public void Should_Return_Items_Paragraph_Live_Header(string route)
        {
            var htmlDoc = wikiPageExtraction.WikiPageRouteResponseAsHtmlDocument(route, null);
            var items = paragraphExtractor.ExtractParaInfo(htmlDoc, route, string.Empty);

            Assert.That(items.Header.HasValue(), Is.True);
        }

        [Order(3)]
        [TestCase(Route)]
        public void Should_Return_Items_Paragraph_Live_Route(string route)
        {
            var htmlDoc = wikiPageExtraction.WikiPageRouteResponseAsHtmlDocument(route, null);
            var items = paragraphExtractor.ExtractParaInfo(htmlDoc, route, string.Empty);

            Assert.That(items.Route.HasValue(), Is.True);
        }

        [Order(3)]
        [TestCase(Route)]
        public void Should_Return_Items_Paragraph_Live_MainParagraph(string route)
        {
            var htmlDoc = wikiPageExtraction.WikiPageRouteResponseAsHtmlDocument(route, null);
            var items = paragraphExtractor.ExtractParaInfo(htmlDoc, route, string.Empty);

            Assert.That(items.MainParagraph, Is.Not.Null);
            Assert.That(items.MainParagraph.Count > 1, Is.True);
        }

        [Order(3)]
        [TestCase(Route)]
        public void Should_Return_Items_Paragraph_Live_WikiParaCollection(string route)
        {
            var htmlDoc = wikiPageExtraction.WikiPageRouteResponseAsHtmlDocument(route, null);
            var items = paragraphExtractor.ExtractParaInfo(htmlDoc, route, string.Empty);

            Assert.That(items.WikiParaCollection, Is.Not.Null);
            Assert.That(items.WikiParaCollection.Count > 1, Is.True);
        }

        [Order(3)]
        [TestCase(Route)]
        public void Should_Return_Items_Paragraph_Live_WikiPictureCollection(string route)
        {
            var htmlDoc = wikiPageExtraction.WikiPageRouteResponseAsHtmlDocument(route, null);
            var items = paragraphExtractor.ExtractParaInfo(htmlDoc, route, null);

            Assert.That(items.WikiPictureCollection, Is.Not.Null);
            Assert.That(items.WikiPictureCollection.Count > 1, Is.True);
        }

        [Order(4)]
        [TestCase(Route)]
        public void Should_Return_Items_Paragraph_Mock_WikiPictureCollection(string route)
        {
            //For any changes in the schema of the model
            //Generate a new file and replace the template = TemplateCompileItemsFile content
            //SerializationHelper.SerializeToJson(actualItems, "");

            var htmlDoc = wikiPageExtraction.WikiPageRouteResponseAsHtmlDocument(route, File.ReadAllText(TemplateCompileFile));
            var actualItems = paragraphExtractor.ExtractParaInfo(htmlDoc, route, null);
            var expectedItems = SerializationHelper.DeSerializeFromJsonFile<WikiPageModel>(TemplateCompileItemsFile);

            Assert.That(actualItems, Is.Not.Null);
            Assert.That(expectedItems, Is.Not.Null);

            Assert.IsTrue(actualItems.Header == expectedItems.Header);
            Assert.IsTrue(actualItems.Route == expectedItems.Route);

            Assert.IsTrue(actualItems.MainParagraph.Count == expectedItems.MainParagraph.Count);
            Assert.IsTrue(actualItems.WikiParaCollection.Count == expectedItems.WikiParaCollection.Count);
            Assert.IsTrue(actualItems.WikiPictureCollection.Count == expectedItems.WikiPictureCollection.Count);

            for (int i = 0; i < expectedItems.MainParagraph.Count; i++)
            {
                Assert.IsTrue(actualItems.MainParagraph[i].Content == expectedItems.MainParagraph[i].Content);
                Assert.IsTrue(actualItems.MainParagraph[i].ContentBuilder.ToString() == expectedItems.MainParagraph[i].ContentBuilder.ToString());
                Assert.IsTrue(actualItems.MainParagraph[i].SubHeader == expectedItems.MainParagraph[i].SubHeader);
                Assert.IsTrue(actualItems.MainParagraph[i].Sequence == expectedItems.MainParagraph[i].Sequence);
                Assert.IsTrue(actualItems.MainParagraph[i].Header2InternalId == expectedItems.MainParagraph[i].Header2InternalId);
                Assert.IsTrue(actualItems.MainParagraph[i].Header3InternalId == expectedItems.MainParagraph[i].Header3InternalId);
            }

            for (int i = 0; i < expectedItems.WikiParaCollection.Count; i++)
            {
                Assert.IsTrue(actualItems.WikiParaCollection[i].Header == expectedItems.WikiParaCollection[i].Header);
                Assert.IsTrue(actualItems.WikiParaCollection[i].Header2InternalId == expectedItems.WikiParaCollection[i].Header2InternalId);
                Assert.IsTrue(actualItems.WikiParaCollection[i].Sequence == expectedItems.WikiParaCollection[i].Sequence);
                Assert.IsTrue(actualItems.WikiParaCollection[i].ParagraghInternalModels.Count == expectedItems.WikiParaCollection[i].ParagraghInternalModels.Count);

                for (int j = 0; j < expectedItems.WikiParaCollection[i].ParagraghInternalModels.Count; j++)
                {
                    Assert.IsTrue(actualItems.WikiParaCollection[i].ParagraghInternalModels[j].Content == expectedItems.WikiParaCollection[i].ParagraghInternalModels[j].Content);
                    Assert.IsTrue(actualItems.WikiParaCollection[i].ParagraghInternalModels[j].ContentBuilder.ToString() == expectedItems.WikiParaCollection[i].ParagraghInternalModels[j].ContentBuilder.ToString());
                    Assert.IsTrue(actualItems.WikiParaCollection[i].ParagraghInternalModels[j].SubHeader == expectedItems.WikiParaCollection[i].ParagraghInternalModels[j].SubHeader);
                    Assert.IsTrue(actualItems.WikiParaCollection[i].ParagraghInternalModels[j].Sequence == expectedItems.WikiParaCollection[i].ParagraghInternalModels[j].Sequence);
                    Assert.IsTrue(actualItems.WikiParaCollection[i].ParagraghInternalModels[j].Header2InternalId == expectedItems.WikiParaCollection[i].ParagraghInternalModels[j].Header2InternalId);
                    Assert.IsTrue(actualItems.WikiParaCollection[i].ParagraghInternalModels[j].Header3InternalId == expectedItems.WikiParaCollection[i].ParagraghInternalModels[j].Header3InternalId);

                }
            }

            for (int i = 0; i < expectedItems.WikiPictureCollection.Count; i++)
            {
                Assert.IsTrue(actualItems.WikiPictureCollection[i].Sequence == expectedItems.WikiPictureCollection[i].Sequence);
                Assert.IsTrue(actualItems.WikiPictureCollection[i].Caption == expectedItems.WikiPictureCollection[i].Caption);
                Assert.IsTrue(actualItems.WikiPictureCollection[i].CustomMetadata.Count == expectedItems.WikiPictureCollection[i].CustomMetadata.Count);

                foreach (var key in expectedItems.WikiPictureCollection[i].CustomMetadata.Keys)
                {
                    Assert.IsTrue(actualItems.WikiPictureCollection[i].CustomMetadata[key] == expectedItems.WikiPictureCollection[i].CustomMetadata[key]);
                }
            }
        }
    }
}
