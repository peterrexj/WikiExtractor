using BenchmarkDotNet.Attributes;
using Pj.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.DbModels;
using WikiExtractor.DbModels.UserStore;
using WikiExtractor.Exts;
using WikiExtractor.Models;
using WikiExtractor.Process;
using WikiExtractor.Repository;
using WikiExtractor.Repository.UserStore;
using WikiExtractor.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WikiExtractor.Test.NFT
{
    [MemoryDiagnoser]
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class DatabaseBenchmarks
    {
        private void Initialize()
        {
            ProcessConstants.UserStoreDatabasePath = IoHelper.CombinePath(PjUtility.Runtime.ExecutingRepositoryRootFolder, "App", "Databases", "UserStore.db");
            ProcessConstants.DatabasePath = "C:\\GIT\\Other\\peterrexj\\WikiExtractor\\App\\Databases\\WikiStoreSaints.db";
            ConfigData.LocalStorageCacheFolderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
        }
        private int GetMasterId(WikiDatabase db) => db.MasterRepository.GetAll().Where(f => f.Name.Contains("Pope John Paul II")).FirstOrDefault()?.Id ?? 1;


        [Benchmark]
        public List<PersonaViewModel> LoadDbOption1()
        {
            Initialize();
            WikiDatabase wikiDatabase = new();
            UserStoreDatabase userStoreDatabase = new();
            //var data = GetItemData(GetMasterId(wikiDatabase), wikiDatabase);
            var item = GetItemDataOption1(wikiDatabase, userStoreDatabase, ["All"]).ToList();
            return item;
        }
        [Benchmark]
        public List<PersonaViewModel> LoadDbOption2()
        {
            Initialize();
            WikiDatabase wikiDatabase = new();
            UserStoreDatabase userStoreDatabase = new();
            //var data = GetItemData(GetMasterId(wikiDatabase), wikiDatabase);
            var item = GetItemDataOption2(wikiDatabase, userStoreDatabase, ["All"]).ToList();
            return item;
        }

        private IEnumerable<PersonaViewModel> GetItemDataOption1(WikiDatabase wikiDatabase, UserStoreDatabase userStoreDatabase, List<string> tags = null, int minListHeight = ConfigData.MinHeightOfListItemInListPage)
        {
            var isPrimaryMetadataContentEnabled = wikiDatabase.PhoneSettingsRepository.IsPrimaryMetadatDisplayEnabled;
            var primaryMetadataContentFields = wikiDatabase.PhoneSettingsRepository.PrimaryMetadatDisplayContent;
            var maxMetadataItems = wikiDatabase.PhoneSettingsRepository.MaxMetadataItemToDisplay;
            var totalMasterCount = wikiDatabase.MasterRepository.TotalCount;

            return from master in wikiDatabase.MasterRepository.GetAll()

                   join tagItemJoin in wikiDatabase.TagItemRepository.GetAll() on master.Id equals tagItemJoin.MasterId into tagItemGrp
                   from tagItem in tagItemGrp.DefaultIfEmpty(new TagItem { Id = 0, MasterId = master.Id })

                   join tagJoin in wikiDatabase.TagRepository.GetAll() on tagItem.TagId equals tagJoin.Id into tagGroup
                   from tag in tagGroup.DefaultIfEmpty(new Tag { Id = 0, Name = string.Empty })

                   join primaryPicJoin in wikiDatabase.WikiPictureRepository.GetAllPrimaryPicturesWithFields("MasterId", "Path", "Caption") on master.Id equals primaryPicJoin.MasterId into primaryPicGroup
                   from primaryPic in primaryPicGroup.DefaultIfEmpty(new WikiPicture { MasterId = master.Id, Path = "NoImageAvailable.png", Caption = string.Empty })

                   join mainCont in wikiDatabase.ParagraphPrimaryContentRepository.GetAll() on master.Id equals mainCont.MasterId into mainContGroup
                   from mainContItem in mainContGroup.DefaultIfEmpty(new ParagraphPrimaryContent { MasterId = master.Id, Content = string.Empty })

                   join metadataJoin in wikiDatabase.MetadataRepository.GetAll() on master.Id equals metadataJoin.MasterId into metadataGrp
                   from metadata in metadataGrp.DefaultIfEmpty(new Metadata { Id = 0, MasterId = master.Id })

                   join itemReadStatusJoin in userStoreDatabase.ItemReadTrackerRepository.GetAll() on master.Name equals itemReadStatusJoin.ItemIdentifier into itemReadStatusGroup
                   from itemReadStatus in itemReadStatusGroup.DefaultIfEmpty(new ItemReadTrackerModel { ItemIdentifier = master.Name, IsRead = 0 })

                   where tags?.Contains(tag.Name) == true || tag.Name.IsEmpty()
                   group new { master, mainContItem, primaryPic, metadata, tagItem, tag, itemReadStatus } by new { master.Id } into masterGroup

                   let primaryMetadata = isPrimaryMetadataContentEnabled ? masterGroup.Select(f => f.metadata).Where(f => primaryMetadataContentFields.Contains(f.Key) && f.Value.HasValueOptimized())
                            .Take(maxMetadataItems)
                            .Select(f => new MetadataViewModel
                            {
                                Key = f.Key,
                                Description = f.Value
                            }).ToList() : new List<MetadataViewModel>()
                   let isPrimaryMetadataEnabled = isPrimaryMetadataContentEnabled && primaryMetadata.Any()

                   select new PersonaViewModel
                   {
                       Id = masterGroup.FirstOrDefault()!.master.Id,
                       RandomId = RandomHelper.RandomNumberGeneratorBetweenRange(0, totalMasterCount),
                       Name = masterGroup.FirstOrDefault()!.master.Name,
                       WikiPath = masterGroup.FirstOrDefault()!.master.Route,
                       MainContent = masterGroup.FirstOrDefault()!.mainContItem?.Content ?? "",
                       PicturePrimaryPath = masterGroup.FirstOrDefault()!.primaryPic?.Path ?? "NoImageAvailable.png",
                       PicturePrimaryCaption = masterGroup.FirstOrDefault()!.primaryPic?.Caption ?? "",
                       IsPrimaryMetadataContentEnabled = isPrimaryMetadataEnabled,
                       PrimaryMetadataContent = primaryMetadata,
                       //Tags = masterGroup.Select(f => f.tag).Select(f => f.Name).Distinct().ToList(),
                       IsBusy = false,
                       ListHeight = minListHeight,
                       ItemReadStatus = masterGroup.FirstOrDefault()!.itemReadStatus.IsReadAsBool,
                   };

        }

        private IEnumerable<PersonaViewModel> GetItemDataOption2(WikiDatabase wikiDatabase, UserStoreDatabase userStoreDatabase, List<string> tags = null, int minListHeight = ConfigData.MinHeightOfListItemInListPage)
        {
            //var masters = wikiDatabase.MasterRepository.GetAll();
            //if (masters == null || masters.IsEmpty()) return new List<PersonaViewModel>();

            var isPrimaryMetadataContentEnabled = wikiDatabase.PhoneSettingsRepository.IsPrimaryMetadatDisplayEnabled;
            var primaryMetadataContentFields = wikiDatabase.PhoneSettingsRepository.PrimaryMetadatDisplayContent;
            var maxMetadataItems = wikiDatabase.PhoneSettingsRepository.MaxMetadataItemToDisplay;
            var totalMasterCount = wikiDatabase.MasterRepository.TotalCount;

            return from master in wikiDatabase.MasterRepository.GetAll()

                   join tagItemJoin in wikiDatabase.TagItemRepository.GetAll() on master.Id equals tagItemJoin.MasterId into tagItemGrp
                   from tagItem in tagItemGrp.DefaultIfEmpty(new TagItem { Id = 0, MasterId = master.Id })

                   join tagJoin in wikiDatabase.TagRepository.GetAll() on tagItem.TagId equals tagJoin.Id into tagGroup
                   from tag in tagGroup.DefaultIfEmpty(new Tag { Id = 0, Name = string.Empty })

                   join primaryPicJoin in wikiDatabase.WikiPictureRepository.GetAllPrimaryPictures() on master.Id equals primaryPicJoin.MasterId into primaryPicGroup
                   from primaryPic in primaryPicGroup.DefaultIfEmpty(new WikiPicture { MasterId = master.Id, Path = "NoImageAvailable.png", Caption = string.Empty })

                   join mainCont in wikiDatabase.ParagraphPrimaryContentRepository.GetAll() on master.Id equals mainCont.MasterId into mainContGroup
                   from mainContItem in mainContGroup.DefaultIfEmpty(new ParagraphPrimaryContent { MasterId = master.Id, Content = string.Empty })

                   join metadataJoin in wikiDatabase.MetadataRepository.GetAll() on master.Id equals metadataJoin.MasterId into metadataGrp
                   from metadata in metadataGrp.DefaultIfEmpty(new Metadata { Id = 0, MasterId = master.Id })

                   join itemReadStatusJoin in userStoreDatabase.ItemReadTrackerRepository.GetAll() on master.Name equals itemReadStatusJoin.ItemIdentifier into itemReadStatusGroup
                   from itemReadStatus in itemReadStatusGroup.DefaultIfEmpty(new ItemReadTrackerModel { ItemIdentifier = master.Name, IsRead = 0 })

                   where tags?.Contains(tag.Name) == true || tag.Name.IsEmpty()
                   group new { master, mainContItem, primaryPic, metadata, tagItem, tag, itemReadStatus } by new { master.Id } into masterGroup

                   let primaryMetadata = isPrimaryMetadataContentEnabled ? masterGroup.Select(f => f.metadata).Where(f => primaryMetadataContentFields.Contains(f.Key) && f.Value.HasValueOptimized())
                            .Take(maxMetadataItems)
                            .Select(f => new MetadataViewModel
                            {
                                Key = f.Key,
                                Description = f.Value
                            }).ToList() : new List<MetadataViewModel>()
                   let isPrimaryMetadataEnabled = isPrimaryMetadataContentEnabled && primaryMetadata.Any()

                   select new PersonaViewModel
                   {
                       Id = masterGroup.FirstOrDefault()!.master.Id,
                       RandomId = RandomHelper.RandomNumberGeneratorBetweenRange(0, totalMasterCount),
                       Name = masterGroup.FirstOrDefault()!.master.Name,
                       WikiPath = masterGroup.FirstOrDefault()!.master.Route,
                       MainContent = masterGroup.FirstOrDefault()!.mainContItem?.Content ?? "",
                       PicturePrimaryPath = masterGroup.FirstOrDefault()!.primaryPic?.Path ?? "NoImageAvailable.png",
                       PicturePrimaryCaption = masterGroup.FirstOrDefault()!.primaryPic?.Caption ?? "",
                       IsPrimaryMetadataContentEnabled = isPrimaryMetadataEnabled,
                       PrimaryMetadataContent = primaryMetadata,
                       //Tags = masterGroup.Select(f => f.tag).Select(f => f.Name).Distinct().ToList(),
                       IsBusy = false,
                       ListHeight = minListHeight,
                       ItemReadStatus = masterGroup.FirstOrDefault()!.itemReadStatus.IsReadAsBool,
                   };
        }

        public PersonaViewModel GetItemData(int masterId, WikiDatabase wikiDatabase)
        {
            var master = wikiDatabase.MasterRepository.GetById(masterId);

            var pic = wikiDatabase.WikiPictureRepository.GetByMasterId(masterId)
                .DefaultIfEmpty(new WikiPicture { MasterId = master.Id, Path = "NoImageAvailable.png", Caption = string.Empty });

            var metadata = wikiDatabase.MetadataRepository.GetByMasterId(masterId)
                .DefaultIfEmpty(new Metadata { Id = 0, MasterId = master.Id });

            var mainContItem = wikiDatabase.ParagraphPrimaryContentRepository.GetByMasterId(masterId)
                .DefaultIfEmpty(new ParagraphPrimaryContent { MasterId = master.Id, Content = string.Empty });

            var item = new PersonaViewModel
            {
                Name = master.Name,
                WikiPath = master.Route,
                PicturePrimaryPath = pic.FirstOrDefault(f => f.Path.HasValueOptimized() && f.IsPrimaryBool)?.Path ?? "",
                PicturePrimaryCaption = pic.FirstOrDefault(f => f.Path.HasValueOptimized() && f.IsPrimaryBool)?.Caption ?? "",
                Pictures = [.. pic.Where(f => f.Path.HasValueOptimized()).OrderBy(f => f.Sequence)
                    .Select(f => new PictureViewModel
                    {
                        Id = f.Id,
                        PicturePath = f.Path,
                        PictureCaption = f.Caption.HasValueOptimized() && f.Caption.Length >= ConfigData.MinLengthOfPictureCaption ? f.Caption : string.Empty,
                        Sequence = f.Sequence,
                        Width = f.Width,
                        Height = f.Height,
                        ParentName = master.Name,
                    })],
                Metadatas = [.. metadata.Where(item => item.TypeByEnum == MetadataType.Detail && item.Value.HasValueOptimized()).OrderBy(f => f.Sequence)
                    .Select(item => new MetadataViewModel
                    {
                        Key = item.Key,
                        Description = item.Value,
                        Sequence = item.Sequence,
                        GroupHeader = item.Value //Need to get the group header
                    })],
                MainContent = mainContItem.FirstOrDefault(f => f != null && f.Content.HasValueOptimized())?.Content ?? "",
                Paragraphs =
                [
                    new Paragraph2ContentViewModel
                    {
                        Content = mainContItem.FirstOrDefault(f => f != null && f.Content.HasValueOptimized())?.Content ?? "",
                        Header2 = master.Name,
                        Sequence = 0
                    }
                ]
            };

            return item;
        }
    }
}
