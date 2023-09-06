using Newtonsoft.Json.Linq;
using Pj.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.DbModels;
using WikiExtractor.Models;
using WikiExtractor.Process;
using WikiExtractor.Process.Extractor;
using WikiExtractor.Repository;

namespace WikiExtractor.Tests
{
    public class StoreProcessTests
    {
        SaintsWikiExtractionToStore wikiPageExtraction;
        ParagraphExtractor paragraphExtractor;
        MetadataExtractor metadataExtractor;
        StoreProcess storeProcess;
        WikiDatabase wikiDatabase;

        private const string Route = "/wiki/Pope_John_Paul_II";

        [SetUp]
        public void Setup()
        {
            ProcessConstants.DatabasePath = IoHelper.CombinePath(PjUtility.Runtime.ExecutingFolder, "Tests", "Db", "WikiStore.db");
            ProcessConstants.UserStoreDatabasePath = IoHelper.CombinePath(PjUtility.Runtime.ExecutingFolder, "Tests", "Db", "UserStore.db");
            ProcessConstants.CacheFolder = IoHelper.CombinePath(PjUtility.Runtime.ExecutingRepositoryRootFolder, "Tests", "Cache");
            wikiPageExtraction = new SaintsWikiExtractionToStore();
            paragraphExtractor = new ParagraphExtractor();
            metadataExtractor = new MetadataExtractor();
            //IoHelper.DeleteFile(ProcessConstants.DatabasePath);
            storeProcess = new StoreProcess();
            wikiDatabase = new WikiDatabase();
        }


        [Order(1)]
        [TestCase(Route)]
        public void Shoud_Save_Data_Master(string route)
        {
            var response = wikiPageExtraction.WikiPageRouteResponseAsHtmlDocument(route, null);
            Assert.IsNotNull(response);

            var paraInfo = paragraphExtractor.ExtractParaInfo(response, route, string.Empty);
            var metadata = metadataExtractor.ExtractMetadataInfo(response, null, null);

            var masterid = storeProcess.StoreInformation(paraInfo, metadata, new Models.WikiWhatToExtractModel { Route = route });
            Assert.IsNotNull(masterid);
            Assert.IsTrue(masterid > 0);
        }

        [Order(2)]
        [TestCase(Route)]
        public void Shoud_Save_Data_Metadata(string route)
        {
            var response = wikiPageExtraction.WikiPageRouteResponseAsHtmlDocument(route, null);
            Assert.IsNotNull(response);

            var paraInfo = paragraphExtractor.ExtractParaInfo(response, route, string.Empty);
            var metadata = metadataExtractor.ExtractMetadataInfo(response, null, null);

            var masterid = storeProcess.StoreInformation(paraInfo, metadata, new Models.WikiWhatToExtractModel { Route = route });
            var actualData = wikiDatabase.MetadataRepository.Get(f => f.MasterId == masterid).ToList();

            //Un comment if there is change in the data, this line will update the source of truth
            SerializationHelper.SerializeToJson(metadata.Where(f => f.Type != MetadataType.Image),
                IoHelper.CombinePath(PjUtility.Runtime.ExecutingRepositoryRootFolder, "WikiExtractor.Tests", "Templates", "Store", "MetaData.json"));

            var expectedData = SerializationHelper.DeSerializeFromJsonFile<List<Metadata>>(IoHelper.CombinePath(PjUtility.Runtime.ExecutingFolder, "Templates", "Store", "MetaData.json"));

            Assert.That(actualData.Count, Is.EqualTo(expectedData.Count));
            for (int i = 0; i < expectedData.Count; i++)
            {
                Assert.IsTrue(expectedData[i].MasterId == actualData[i].MasterId);
                Assert.IsTrue(expectedData[i].Key == actualData[i].Key);
                Assert.IsTrue(expectedData[i].Value == actualData[i].Value);
                Assert.IsTrue(expectedData[i].Type == actualData[i].Type);
                Assert.IsTrue(expectedData[i].Sequence == actualData[i].Sequence);
            }
        }

        [Order(2)]
        [TestCase(Route)]
        public void Shoud_Save_Data_PrimaryContent(string route)
        {
            var response = wikiPageExtraction.WikiPageRouteResponseAsHtmlDocument(route, null);
            Assert.IsNotNull(response);

            var paraInfo = paragraphExtractor.ExtractParaInfo(response, route, string.Empty);
            var metadata = metadataExtractor.ExtractMetadataInfo(response, null, null);

            var masterid = storeProcess.StoreInformation(paraInfo, metadata, new Models.WikiWhatToExtractModel { Route = route });
            var actualData = wikiDatabase.ParagraphPrimaryContentRepository.Get(f => f.MasterId == masterid).ToList();

            //Un comment if there is change in the data, this line will update the source of truth
            //SerializationHelper.SerializeToJson(actualData,
            //    IoHelper.CombinePath(PjUtility.Runtime.ExecutingRepositoryRootFolder, "WikiExtractor.Tests", "Templates", "Store", "PrimaryContent.json"));

            var expectedData = SerializationHelper.DeSerializeFromJsonFile<List<ParagraphPrimaryContent>>(IoHelper.CombinePath(PjUtility.Runtime.ExecutingFolder, "Templates", "Store", "PrimaryContent.json"));

            Assert.That(actualData.Count, Is.EqualTo(expectedData.Count));
            for (int i = 0; i < expectedData.Count; i++)
            {
                Assert.IsTrue(expectedData[i].MasterId == actualData[i].MasterId);
                Assert.IsTrue(expectedData[i].Content == actualData[i].Content);
            }
        }

        [Order(2)]
        [TestCase(Route)]
        public void Shoud_Save_Data_WikiPicture(string route)
        {
            var response = wikiPageExtraction.WikiPageRouteResponseAsHtmlDocument(route, null);
            Assert.IsNotNull(response);

            var paraInfo = paragraphExtractor.ExtractParaInfo(response, route, string.Empty);
            var metadata = metadataExtractor.ExtractMetadataInfo(response, null, null);

            var masterid = storeProcess.StoreInformation(paraInfo, metadata, new Models.WikiWhatToExtractModel { Route = route });
            var actualData = wikiDatabase.WikiPictureRepository.Get(f => f.MasterId == masterid).ToList();

            //Un comment if there is change in the data, this line will update the source of truth
            //SerializationHelper.SerializeToJson(actualData,
            //    IoHelper.CombinePath(PjUtility.Runtime.ExecutingRepositoryRootFolder, "WikiExtractor.Tests", "Templates", "Store", "WikiPicture.json"));

            var expectedData = SerializationHelper.DeSerializeFromJsonFile<List<WikiPicture>>(IoHelper.CombinePath(PjUtility.Runtime.ExecutingFolder, "Templates", "Store", "WikiPicture.json"));

            Assert.That(actualData.Count, Is.EqualTo(expectedData.Count));
            for (int i = 0; i < expectedData.Count; i++)
            {
                Assert.IsTrue(expectedData[i].MasterId == actualData[i].MasterId);
                Assert.IsTrue(expectedData[i].Sequence == actualData[i].Sequence);
                Assert.IsTrue(expectedData[i].Width == actualData[i].Width);
                Assert.IsTrue(expectedData[i].Height == actualData[i].Height);
                Assert.IsTrue(expectedData[i].Path == actualData[i].Path);
                Assert.IsTrue(expectedData[i].Caption == actualData[i].Caption);
                Assert.IsTrue(expectedData[i].IsPrimary == actualData[i].IsPrimary);
                Assert.IsTrue(expectedData[i].IsPrimaryBool == actualData[i].IsPrimaryBool);
            }
        }

        [Order(2)]
        [TestCase(Route)]
        public void Shoud_Save_Data_ParagraphHeader2(string route)
        {
            var response = wikiPageExtraction.WikiPageRouteResponseAsHtmlDocument(route, null);
            Assert.IsNotNull(response);

            var paraInfo = paragraphExtractor.ExtractParaInfo(response, route, string.Empty);
            var metadata = metadataExtractor.ExtractMetadataInfo(response, null, null);

            var masterid = storeProcess.StoreInformation(paraInfo, metadata, new Models.WikiWhatToExtractModel { Route = route });
            var actualData = wikiDatabase.ParagraphHeader2Repository.Get(f => f.MasterId == masterid).ToList();

            //Un comment if there is change in the data, this line will update the source of truth
            //SerializationHelper.SerializeToJson(actualData,
            //    IoHelper.CombinePath(PjUtility.Runtime.ExecutingRepositoryRootFolder, "WikiExtractor.Tests", "Templates", "Store", "ParaHeader2.json"));

            var expectedData = SerializationHelper.DeSerializeFromJsonFile<List<ParagraphHeader2>>(IoHelper.CombinePath(PjUtility.Runtime.ExecutingFolder, "Templates", "Store", "ParaHeader2.json"));

            Assert.That(actualData.Count, Is.EqualTo(expectedData.Count));
            for (int i = 0; i < expectedData.Count; i++)
            {
                Assert.IsTrue(expectedData[i].MasterId == actualData[i].MasterId);
                Assert.IsTrue(expectedData[i].Sequence == actualData[i].Sequence);
                Assert.IsTrue(expectedData[i].Header == actualData[i].Header);
            }
        }

        [Order(2)]
        [TestCase(Route)]
        public void Shoud_Save_Data_ParagraphHeader3(string route)
        {
            var response = wikiPageExtraction.WikiPageRouteResponseAsHtmlDocument(route, null);
            Assert.IsNotNull(response);

            var paraInfo = paragraphExtractor.ExtractParaInfo(response, route, string.Empty);
            var metadata = metadataExtractor.ExtractMetadataInfo(response, null, null);

            var masterid = storeProcess.StoreInformation(paraInfo, metadata, new Models.WikiWhatToExtractModel { Route = route });
            var actualData = wikiDatabase.ParagraphHeader3Repository.Get(f => f.MasterId == masterid).ToList();

            //Un comment if there is change in the data, this line will update the source of truth
            //SerializationHelper.SerializeToJson(actualData,
            //    IoHelper.CombinePath(PjUtility.Runtime.ExecutingRepositoryRootFolder, "WikiExtractor.Tests", "Templates", "Store", "ParaHeader3.json"));

            var expectedData = SerializationHelper.DeSerializeFromJsonFile<List<ParagraphHeader3>>(IoHelper.CombinePath(PjUtility.Runtime.ExecutingFolder, "Templates", "Store", "ParaHeader3.json"));

            Assert.That(actualData.Count, Is.EqualTo(expectedData.Count));
            for (int i = 0; i < expectedData.Count; i++)
            {
                Assert.IsTrue(expectedData[i].MasterId == actualData[i].MasterId);
                Assert.IsTrue(expectedData[i].Sequence == actualData[i].Sequence);
                Assert.IsTrue(expectedData[i].ParagraphHeader2Id == actualData[i].ParagraphHeader2Id);
                Assert.IsTrue(expectedData[i].Sequence == actualData[i].Sequence);
                Assert.IsTrue(expectedData[i].Header == actualData[i].Header);
            }
        }

        [Order(2)]
        [TestCase(Route)]
        public void Shoud_Save_Data_ParagraphContent(string route)
        {
            var response = wikiPageExtraction.WikiPageRouteResponseAsHtmlDocument(route, null);
            Assert.IsNotNull(response);

            var paraInfo = paragraphExtractor.ExtractParaInfo(response, route, string.Empty);
            var metadata = metadataExtractor.ExtractMetadataInfo(response, null, null);

            var masterid = storeProcess.StoreInformation(paraInfo, metadata, new Models.WikiWhatToExtractModel { Route = route });
            var actualData = wikiDatabase.ParagraphContentRepository.Get(f => f.MasterId == masterid).ToList();

            //Un comment if there is change in the data, this line will update the source of truth
            SerializationHelper.SerializeToJson(actualData,
                IoHelper.CombinePath(PjUtility.Runtime.ExecutingRepositoryRootFolder, "WikiExtractor.Tests", "Templates", "Store", "ParaContent.json"));

            var expectedData = SerializationHelper.DeSerializeFromJsonFile<List<ParagraphContent>>(IoHelper.CombinePath(PjUtility.Runtime.ExecutingFolder, "Templates", "Store", "ParaContent.json"));

            Assert.That(actualData.Count, Is.EqualTo(expectedData.Count));
            for (int i = 0; i < expectedData.Count; i++)
            {
                Assert.IsTrue(expectedData[i].MasterId == actualData[i].MasterId);
                Assert.IsTrue(expectedData[i].ParagraphHeader3Id == actualData[i].ParagraphHeader3Id);
                Assert.IsTrue(expectedData[i].ParagraphHeader2Id == actualData[i].ParagraphHeader2Id);
                Assert.IsTrue(expectedData[i].Content == actualData[i].Content);
            }
        }
    }
}
