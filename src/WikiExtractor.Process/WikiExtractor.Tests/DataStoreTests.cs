using Pj.Library;
using System.Data;
using WikiExtractor.DbModels;
using WikiExtractor.Exts;
using WikiExtractor.Models;
using WikiExtractor.Process;
using WikiExtractor.Repository;
using WikiExtractor.Repository.UserStore;
using WikiExtractor.ViewModels;

namespace WikiExtractor.Tests
{
    internal class DataStoreTests
    {
        [SetUp]
        public void TestSetup()
        {
            ProcessConstants.UserStoreDatabasePath = IoHelper.CombinePath(PjUtility.Runtime.ExecutingRepositoryRootFolder, "App", "Databases", "UserStore.db");
        }

        [TestCaseSource(nameof(DatabaseFiles))]
        public void Should_Have_Menus(string dbFilePath)
        {
            ProcessConstants.DatabasePath = dbFilePath;
            WikiAppController wikiAppController = new WikiAppController(new WikiDatabase(), new UserStoreDatabase());
            var menuItems = wikiAppController.AppMenuItems();
            Assert.IsTrue(menuItems.Count() > 0);
        }

        [TestCaseSource(nameof(DatabaseFiles))]
        public void Should_Have_Tag_OnMenu(string dbFilePath)
        {
            ProcessConstants.DatabasePath = dbFilePath;
            WikiAppController wikiAppController = new WikiAppController(new WikiDatabase(), new UserStoreDatabase());
            var menuItems = wikiAppController.AppMenuItems();
            var grp = menuItems.Where(f => f.Tags.IsEmpty());
            Assert.IsTrue(grp.Count() == 0);
        }

        [TestCaseSource(nameof(DatabaseFiles))]
        public void Should_Have_Tag_Data_EachMenu(string dbFilePath)
        {
            ProcessConstants.DatabasePath = dbFilePath;
            WikiAppController wikiAppController = new WikiAppController(new WikiDatabase(), new UserStoreDatabase());
            var menuItems = wikiAppController.AppMenuItems();
            foreach (var menuItem in menuItems)
            {
                var data = wikiAppController.GetListOfWikiItems(new List<string> { menuItem.Tags });
                Assert.IsTrue(data.Any());
            }
            //Assert.IsTrue(grp.Count() == 0);
        }

        [TestCaseSource(nameof(DatabaseFiles))]
        public void Should_Not_Have_Duplicate_MenuItemName(string dbFilePath)
        {
            ProcessConstants.DatabasePath = dbFilePath;
            WikiAppController wikiAppController = new WikiAppController(new WikiDatabase(), new UserStoreDatabase());
            var menuItems = wikiAppController.AppMenuItems();
            var grp = menuItems.GroupBy(f => f.MenuItemName).Select(f => new { f.Key, Childs = f.ToList() })
                .Where(f => f.Childs.Count > 1)
                .ToList();
            Assert.IsTrue(grp.Count() == 0);
        }

        [TestCaseSource(nameof(DatabaseFiles))]
        public void Should_Not_Have_Duplicate_Menu_TitleOnThePage(string dbFilePath)
        {
            ProcessConstants.DatabasePath = dbFilePath;
            WikiAppController wikiAppController = new WikiAppController(new WikiDatabase(), new UserStoreDatabase());
            var menuItems = wikiAppController.AppMenuItems();
            var grp = menuItems.GroupBy(f => f.TitleOnThePage).Select(f => new { f.Key, Childs = f.ToList() })
                .Where(f => f.Childs.Count > 1)
                .ToList();
            Assert.IsTrue(grp.Count() == 0);
        }

        [TestCaseSource(nameof(DatabaseFiles))]
        public void Should_Not_Have_Duplicate_Menu_Tag(string dbFilePath)
        {
            ProcessConstants.DatabasePath = dbFilePath;
            WikiAppController wikiAppController = new WikiAppController(new WikiDatabase(), new UserStoreDatabase());
            var menuItems = wikiAppController.AppMenuItems();
            var grp = menuItems.GroupBy(f => f.Tags).Select(f => new { f.Key, Childs = f.ToList() })
                .Where(f => f.Childs.Count > 1)
                .ToList();
            Assert.IsTrue(grp.Count() == 0);
        }

        [TestCaseSource(nameof(DatabaseFiles))]
        public void Should_EachItem_Have_Data(string dbFilePath)
        {
            ProcessConstants.DatabasePath = dbFilePath;
            WikiDatabase wikiDatabase = new WikiDatabase();
            UserStoreDatabase userStoreDatabase = new UserStoreDatabase();
            WikiAppController wikiAppController = new WikiAppController(wikiDatabase, userStoreDatabase);
            var allMasterData = wikiDatabase.MasterRepository.GetAll();
            var allPicsData = wikiDatabase.WikiPictureRepository.GetAll();
            var metadataData = wikiDatabase.MetadataRepository.GetAll();
            var paraPrimaryData = wikiDatabase.ParagraphPrimaryContentRepository.GetAll();
            var para2Data = wikiDatabase.ParagraphHeader2Repository.GetAll();
            var para3Data = wikiDatabase.ParagraphHeader3Repository.GetAll();
            var paraData = wikiDatabase.ParagraphContentRepository.GetAll();

            Parallel.ForEach(allMasterData, new ParallelOptions { MaxDegreeOfParallelism = 10 }, item =>
            {
                var data = wikiAppController.GetViewModelByIdAsync(item.Id).GetAwaiter().GetResult();
                //var data = GetViewModelv2Test(item.Id,
                //    allMasterData, allPicsData, metadataData, paraPrimaryData, para2Data, para3Data, paraData);
                Assert.IsNotNull(data);
                Assert.IsTrue(data.Name.HasValue());
                Assert.IsTrue(data.WikiPath.HasValue());
                Assert.IsFalse(data.Pictures.IsEmpty() &&
                    data.Metadatas.IsEmpty() &&
                    data.Pictures.IsEmpty());
                Assert.IsTrue(data.Paragraphs.Any());
            });
        }

        //private PersonaViewModel GetViewModelv2Test(int masterId,
        //    IEnumerable<Master> maData,
        //    IEnumerable<WikiPicture> allPicsData,
        //    IEnumerable<Metadata> metadataData,
        //    IEnumerable<ParagraphPrimaryContent> paraPrimaryData,
        //    IEnumerable<ParagraphHeader2> para2Data,
        //    IEnumerable<ParagraphHeader3> para3Data,
        //    IEnumerable<ParagraphContent> paraData)
        //{
        //    var persona = (from master in maData

        //                   join picJoin in allPicsData on master.Id equals picJoin.MasterId into picGroup
        //                   from pic in picGroup.DefaultIfEmpty(new WikiPicture { MasterId = master.Id, Path = "NoImageAvailable.png", Caption = string.Empty })

        //                   join metadataJoin in metadataData on master.Id equals metadataJoin.MasterId into metadataGrp
        //                   from metadata in metadataGrp.DefaultIfEmpty(new Metadata { Id = 0, MasterId = master.Id })

        //                   join mainCont in paraPrimaryData on master.Id equals mainCont.MasterId into mainContGroup
        //                   from mainContItem in mainContGroup.DefaultIfEmpty(new ParagraphPrimaryContent { MasterId = master.Id, Content = string.Empty })

        //                   where master.Id == masterId
        //                   group new { master, pic, metadata, mainContItem } by new { master.Id } into masterGroup
        //                   let mainContentData = masterGroup.Select(f => f.mainContItem).Distinct().FirstOrDefault(f => f != null && f.Content.HasValue())
        //                   let masterData = masterGroup.FirstOrDefault()
        //                   let primaryPicData = masterGroup.Select(f => f.pic).Where(f => f.Path.HasValue()).FirstOrDefault(f => f.IsPrimaryBool)
        //                   let picData = masterGroup.Select(f => f.pic).Distinct().Where(f => f.Path.HasValue()).OrderBy(f => f.Sequence)
        //                   let metaData = masterGroup.Select(f => f.metadata).Distinct().OrderBy(f => f.Sequence)
        //                            .Where(item => item.TypeByEnum == MetadataType.Detail && item.Value.HasValue())

        //                   select new PersonaViewModel
        //                   {
        //                       Name = masterData.master.Name,
        //                       WikiPath = masterData.master.Route,
        //                       PicturePrimaryPath = primaryPicData?.Path ?? "",
        //                       PicturePrimaryCaption = primaryPicData?.Caption ?? "",
        //                       Pictures = picData
        //                           .Select(f => new PictureViewModel
        //                           {
        //                               PicturePath = f.Path,
        //                               PictureCaption = f.Caption.HasValue() && f.Caption.Length >= ConfigData.MinLengthOfPictureCaption ? f.Caption : string.Empty,
        //                               Sequence = f.Sequence
        //                           }).ToList(),
        //                       Metadatas = metaData
        //                            .Select(item => new MetadataViewModel
        //                            {
        //                                Key = item.Key,
        //                                Description = item.Value,
        //                                Sequence = item.Sequence,
        //                                GroupHeader = item.Value //Need to get the group header
        //                            }).ToList(),
        //                       MainContent = mainContentData?.Content ?? "",
        //                       Paragraphs = new List<Paragraph2ContentViewModel> { new Paragraph2ContentViewModel
        //                    {
        //                        Content = mainContentData.Content,
        //                        Header2 = masterData.master.Name,
        //                        Sequence = 0
        //                    } }
        //                   }).FirstOrDefault();

        //    var parah2 = para2Data.Where(m => m.MasterId == masterId).ToList();
        //    var parah3 = para3Data.Where(m => m.MasterId == masterId).ToList();
        //    var parahContents = paraData.Where(m => m.MasterId == masterId).ToList();

        //    if (parahContents.Any())
        //    {
        //        int sequence = 1;
        //        foreach (var para2Item in parah2.OrderBy(f => f.Sequence))
        //        {
        //            if (parahContents.Any(f => f.ParagraphHeader2Id != para2Item.Id))
        //            {
        //                persona.Paragraphs.Add(new Paragraph2ContentViewModel
        //                {
        //                    Content = parahContents.FirstOrDefault(f => f.ParagraphHeader2Id == para2Item.Id)!.Content,
        //                    Header2 = para2Item.Header,
        //                    Para3s = new List<Paragraph3ContentViewModel>(),
        //                    Sequence = sequence++
        //                });
        //            }
        //            else
        //            {
        //                persona.Paragraphs.Add(new Paragraph2ContentViewModel
        //                {
        //                    Content = string.Empty,
        //                    Header2 = "Details",
        //                    Para3s = new List<Paragraph3ContentViewModel>(),
        //                    Sequence = sequence++
        //                });
        //            }

        //            if (parah3.Any(f => f.ParagraphHeader2Id == para2Item.Id)) //Any items matching the para2 header
        //            {
        //                foreach (var para3Item in parah3.Where(f => f.ParagraphHeader2Id == para2Item.Id).OrderBy(f => f.Sequence))
        //                {
        //                    if (parahContents.Any(f => f.ParagraphHeader2Id == para2Item.Id && f.ParagraphHeader3Id == para3Item.Id))
        //                    {
        //                        persona.Paragraphs.Last().Para3s!.Add(new Paragraph3ContentViewModel
        //                        {
        //                            Content = parahContents.FirstOrDefault(f => f.ParagraphHeader2Id == para2Item.Id && f.ParagraphHeader3Id == para3Item.Id)!.Content,
        //                            Header3 = para3Item.Header,
        //                            Sequence = sequence++,
        //                        });
        //                    }
        //                }
        //            }

        //        }
        //    }
        //    return persona;
        //}

        [TestCaseSource(nameof(DatabaseFiles))]
        public void Should_EachItem_Have_PrimaryContent(string dbFilePath)
        {
            ProcessConstants.DatabasePath = dbFilePath;
            WikiDatabase wikiDatabase = new WikiDatabase();
            UserStoreDatabase userStoreDatabase = new UserStoreDatabase();
            WikiAppController wikiAppController = new WikiAppController(wikiDatabase, userStoreDatabase);

            var isPrimaryMetadataContentEnabled = wikiDatabase.PhoneSettingsRepository.IsPrimaryMetadatDisplayEnabled;
            var primaryMetadataContentFields = wikiDatabase.PhoneSettingsRepository.PrimaryMetadatDisplayContent;
            var maxMetadataItems = wikiDatabase.PhoneSettingsRepository.MaxMetadataItemToDisplay;

            if (isPrimaryMetadataContentEnabled)
            {
                Assert.NotNull(primaryMetadataContentFields);
                Assert.IsTrue(primaryMetadataContentFields.Any());
                Assert.IsTrue(maxMetadataItems > 0);
            }

            foreach (var menu in wikiAppController.AppMenuItems())
            {
                var items = wikiAppController.GetListOfWikiItems(new List<string> { menu.Tags });
                foreach (var item in items)
                {
                    Assert.NotNull(item);
                    if (item.IsPrimaryMetadataContentEnabled)
                    {
                        Assert.IsTrue(item.PrimaryMetadataContent.Any());
                        Assert.IsTrue(item.ShowPrimaryContentMetadata);
                        foreach (var meta in item.PrimaryMetadataContent)
                        {
                            Assert.NotNull(meta);
                            Assert.IsTrue(meta.Key.HasValue());
                            Assert.IsTrue(meta.Description.HasValue());
                            Assert.IsTrue(primaryMetadataContentFields.Contains(meta.Key));
                        }
                    }
                    else
                    {
                        Assert.IsTrue(item.MainContent.HasValue());
                        Assert.IsTrue(item.ShowPrimaryContentMetadata == false);
                    }
                }
            }
        }

        public static IEnumerable<string> DatabaseFiles
        {
            get
            {
                return Directory.EnumerateFiles(
                    IoHelper.CombinePath(PjUtility.Runtime.ExecutingRepositoryRootFolder,
                    "Resources\\Databases"), "*.db")
                    .Where(f => Path.GetFileNameWithoutExtension(f).EqualsIgnoreCase("UserStore") == false);
            }
        }
    }
}
