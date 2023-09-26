using Pj.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WikiExtractor.DbModels;
using WikiExtractor.DbModels.UserStore;
using WikiExtractor.Exts;
using WikiExtractor.Models;
using WikiExtractor.Repository;
using WikiExtractor.Repository.UserStore;
using WikiExtractor.ViewModels;

namespace WikiExtractor.Process
{
    public class WikiAppController
    {
        readonly IWikiDatabase wikiDatabase;
        readonly IUserStoreDatabase userStoreDatabase;

        public WikiAppController(IWikiDatabase wikiDb, IUserStoreDatabase userStoreDb)
        {
            wikiDatabase = wikiDb;
            userStoreDatabase = userStoreDb;
        }

        public PersonaViewModel GetViewModelById(int id)
        {
            return GetViewModelv2(id);
        }

        private PersonaViewModel GetViewModelv2(int masterId)
        {
            PersonaViewModel persona = null;
            List<ParagraphHeader2> parah2 = null;
            List<ParagraphHeader3> parah3 = null;
            List<ParagraphContent> parahContents = null;
            List<Tuple<int, PictureViewModel>> pComputedImages = null;
            TaskGroup tgrp = new();

            tgrp.Add(() =>
            {
                persona = (from master in wikiDatabase.MasterRepository.GetAll()

                           join picJoin in wikiDatabase.WikiPictureRepository.GetAll() on master.Id equals picJoin.MasterId into picGroup
                           from pic in picGroup.DefaultIfEmpty(new WikiPicture { MasterId = master.Id, Path = "NoImageAvailable.png", Caption = string.Empty })

                           join metadataJoin in wikiDatabase.MetadataRepository.GetAll() on master.Id equals metadataJoin.MasterId into metadataGrp
                           from metadata in metadataGrp.DefaultIfEmpty(new Metadata { Id = 0, MasterId = master.Id })

                           join mainCont in wikiDatabase.ParagraphPrimaryContentRepository.GetAll() on master.Id equals mainCont.MasterId into mainContGroup
                           from mainContItem in mainContGroup.DefaultIfEmpty(new ParagraphPrimaryContent { MasterId = master.Id, Content = string.Empty })

                           where master.Id == masterId
                           group new { master, pic, metadata, mainContItem } by new { master.Id } into masterGroup
                           let mainContentData = masterGroup.Select(f => f.mainContItem).Distinct().FirstOrDefault(f => f != null && f.Content.HasValue())
                           let masterData = masterGroup.FirstOrDefault()
                           let primaryPicData = masterGroup.Select(f => f.pic).Where(f => f.Path.HasValue()).FirstOrDefault(f => f.IsPrimaryBool)
                           let picData = masterGroup.Select(f => f.pic).Distinct().Where(f => f.Path.HasValue()).OrderBy(f => f.Sequence)
                           let metaData = masterGroup.Select(f => f.metadata).Distinct().OrderBy(f => f.Sequence)
                                    .Where(item => item.TypeByEnum == MetadataType.Detail && item.Value.HasValue())

                           select new PersonaViewModel
                           {
                               Name = masterData.master.Name,
                               WikiPath = masterData.master.Route,
                               PicturePrimaryPath = primaryPicData?.Path ?? "",
                               PicturePrimaryCaption = primaryPicData?.Caption ?? "",
                               Pictures = picData
                                   .Select(f => new PictureViewModel
                                   {
                                       Id = f.Id,
                                       PicturePath = f.Path,
                                       PictureCaption = f.Caption.HasValue() && f.Caption.Length >= ConfigData.MinLengthOfPictureCaption ? f.Caption : string.Empty,
                                       Sequence = f.Sequence,
                                       Width = f.Width,
                                       Height = f.Height
                                   }).ToList(),
                               Metadatas = metaData
                                    .Select(item => new MetadataViewModel
                                    {
                                        Key = item.Key,
                                        Description = item.Value,
                                        Sequence = item.Sequence,
                                        GroupHeader = item.Value //Need to get the group header
                                    }).ToList(),
                               MainContent = mainContentData?.Content ?? "",
                               Paragraphs = new List<Paragraph2ContentViewModel> 
                               { 
                                   new Paragraph2ContentViewModel
                                    {
                                        Content = mainContentData.Content,
                                        Header2 = masterData.master.Name,
                                        Sequence = 0
                                    } 
                               }
                           }).FirstOrDefault();
            });

            tgrp.Add(() => parahContents = wikiDatabase.ParagraphContentRepository.Get(m => m.MasterId == masterId).ToList());
            tgrp.WaitAll();
            tgrp.Add(() => parah2 = wikiDatabase.ParagraphHeader2Repository.Get(m => m.MasterId == masterId).ToList());
            tgrp.Add(() => parah3 = wikiDatabase.ParagraphHeader3Repository.Get(m => m.MasterId == masterId).ToList());
            tgrp.Add(() =>
            {
                pComputedImages = (from p in wikiDatabase.ParagraphImageRepository.Get(m => m.MasterId == masterId)

                                   join parahContentsJoin in wikiDatabase.ParagraphContentRepository.Get(m => m.MasterId == masterId) on p.ParagraphId equals parahContentsJoin.Id into parahContentsGrp
                                   from paraContents in parahContentsGrp.DefaultIfEmpty(new ParagraphContent { MasterId = masterId, Id = 0 })

                                   join picJoin in persona.Pictures on p.ImageId equals picJoin.Id into picGroup
                                   from pic in picGroup.DefaultIfEmpty(new PictureViewModel { Id = 0 })

                                   where pic != null && pic.Id != 0 && paraContents != null && paraContents.Id != 0
                                   select new Tuple<int, PictureViewModel>(paraContents.Id, pic)).ToList();
            });
            tgrp.WaitAll();


            if (parahContents.Any())
            {
                int sequence = 1;
                foreach (var para2Item in parah2.OrderBy(f => f.Sequence))
                {
                    var para2Contents = parahContents.Where(f => f.ParagraphHeader2Id == para2Item.Id && f.ParagraphHeader3Id == 0);
                    if (para2Contents.Any())
                    {
                        foreach (var paraContent in para2Contents)
                        {
                            persona.Paragraphs.Add(new Paragraph2ContentViewModel
                            {
                                Content = paraContent.Content,
                                Header2 = para2Item.Header,
                                Para3s = new List<Paragraph3ContentViewModel>(),
                                Sequence = sequence++,
                                PicLinks = pComputedImages.Where(f => f.Item1 == paraContent.Id).Select(f => f.Item2).ToList(),
                            });
                        }
                    }
                    else
                    {
                        persona.Paragraphs.Add(new Paragraph2ContentViewModel
                        {
                            Content = string.Empty,
                            Header2 = para2Item.Header,
                            Para3s = new List<Paragraph3ContentViewModel>(),
                            Sequence = sequence++
                        });
                    }

                    if (parah3.Any(f => f.ParagraphHeader2Id == para2Item.Id)) //Any items matching the para2 header
                    {
                        foreach (var para3Item in parah3.Where(f => f.ParagraphHeader2Id == para2Item.Id).OrderBy(f => f.Sequence))
                        {
                            foreach (var paraContent in parahContents.Where(f => f.ParagraphHeader2Id == para2Item.Id && f.ParagraphHeader3Id == para3Item.Id))
                            {
                                persona.Paragraphs.Last().Para3s!.Add(new Paragraph3ContentViewModel
                                {
                                    Content = paraContent.Content,
                                    Header3 = para3Item.Header,
                                    Sequence = sequence++,
                                    PicLinks = pComputedImages.Where(f => f.Item1 == paraContent.Id).Select(f => f.Item2).ToList(),
                                });
                            }
                        }
                    }

                }
            }

            //Update the picture local file name
            int picCounter = 1;
            foreach (var pic in persona.Pictures)
            {
                pic.ParentName = persona.Name;
                pic.CurrentCounter = picCounter++;
            }
            return persona;
        }

        public IEnumerable<PersonaViewModel> GetListOfWikiItems(List<string> tags = null, int minListHeight = ConfigData.MinHeightOfListItemInListPage)
        {
            //var masters = wikiDatabase.MasterRepository.GetAll();
            //if (masters == null || masters.IsEmpty()) return new List<PersonaViewModel>();

            var isPrimaryMetadataContentEnabled = wikiDatabase.PhoneSettingsRepository.IsPrimaryMetadatDisplayEnabled;
            var primaryMetadataContentFields = wikiDatabase.PhoneSettingsRepository.PrimaryMetadatDisplayContent;
            var maxMetadataItems = wikiDatabase.PhoneSettingsRepository.MaxMetadataItemToDisplay;
            var totalMasterCount = wikiDatabase.MasterRepository.GetAll().Count();

            return from master in wikiDatabase.MasterRepository.GetAll()

                   join tagItemJoin in wikiDatabase.TagItemRepository.GetAll() on master.Id equals tagItemJoin.MasterId into tagItemGrp
                   from tagItem in tagItemGrp.DefaultIfEmpty(new TagItem { Id = 0, MasterId = master.Id })

                   join tagJoin in wikiDatabase.TagRepository.GetAll() on tagItem.TagId equals tagJoin.Id into tagGroup
                   from tag in tagGroup.DefaultIfEmpty(new Tag { Id = 0, Name = string.Empty })

                   join primaryPicJoin in wikiDatabase.WikiPictureRepository.Get(p => p.IsPrimaryBool) on master.Id equals primaryPicJoin.MasterId into primaryPicGroup
                   from primaryPic in primaryPicGroup.DefaultIfEmpty(new WikiPicture { MasterId = master.Id, Path = "NoImageAvailable.png", Caption = string.Empty })

                   join mainCont in wikiDatabase.ParagraphPrimaryContentRepository.GetAll() on master.Id equals mainCont.MasterId into mainContGroup
                   from mainContItem in mainContGroup.DefaultIfEmpty(new ParagraphPrimaryContent { MasterId = master.Id, Content = string.Empty })

                   join metadataJoin in wikiDatabase.MetadataRepository.GetAll() on master.Id equals metadataJoin.MasterId into metadataGrp
                   from metadata in metadataGrp.DefaultIfEmpty(new Metadata { Id = 0, MasterId = master.Id })

                   join itemReadStatusJoin in userStoreDatabase.ItemReadTrackerRepository.GetAll() on master.Name equals itemReadStatusJoin.ItemIdentifier into itemReadStatusGroup
                   from itemReadStatus in itemReadStatusGroup.DefaultIfEmpty(new ItemReadTrackerModel { ItemIdentifier = master.Name, IsRead = false })

                   where tags?.Contains(tag.Name) == true || tag.Name.IsEmpty()
                   group new { master, mainContItem, primaryPic, metadata, tagItem, tag, itemReadStatus } by new { master.Id } into masterGroup

                   let primaryMetadata = isPrimaryMetadataContentEnabled ? masterGroup.Select(f => f.metadata).Where(f => primaryMetadataContentFields.Contains(f.Key) && f.Value.HasValue())
                            .Take(maxMetadataItems)
                            .Select(f => new MetadataViewModel
                            {
                                Key = f.Key,
                                Description = f.Value
                            }).ToList() : new List<MetadataViewModel>()
                   let isPrimaryMetadataEnabled = isPrimaryMetadataContentEnabled ? primaryMetadata.Any() : false

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
                       ItemReadStatus = masterGroup.FirstOrDefault()!.itemReadStatus.IsRead,
                   };
        }

        public List<PersonaViewModel> UpdateTags(List<PersonaViewModel> datas)
        {
            var temp = from master in wikiDatabase.MasterRepository.GetAll()
                       join tagItemJoin in wikiDatabase.TagItemRepository.GetAll() on master.Id equals tagItemJoin.MasterId into tagItemGrp
                       from tagItem in tagItemGrp.DefaultIfEmpty(new TagItem { Id = 0, MasterId = master.Id })

                       join tagJoin in wikiDatabase.TagRepository.GetAll() on tagItem.TagId equals tagJoin.Id into tagGroup
                       from tag in tagGroup.DefaultIfEmpty(new Tag { Id = 0, Name = string.Empty })

                       select new
                       {
                           Masterid = master.Id,
                           Name = master.Name,
                           Tags = tag.Name
                       };

            var grpTags = temp.GroupBy(f => f.Masterid)
                .Select(f => new { MasterId = f.Key, Items = f.ToList() })
                .ToList();

            foreach (var data in datas)
            {
                data.Tags = grpTags.FirstOrDefault(f => f.MasterId == data.Id)?.Items.Select(f => f.Tags).ToList() ?? new List<string>();
            }

            return datas;


            //var temp = from tagItems in wikiDatabase.TagItemRepository.GetAll()
            //           join tagJoin in wikiDatabase.TagRepository.GetAll() on tagItems.TagId equals tagJoin.Id into tagGroup
            //           from tag in tagGroup.DefaultIfEmpty(new Tag { Id = 0, Name = string.Empty })
            //           where 
        }

        public List<string> GetPrimaryImages()
        {
            return wikiDatabase.WikiPictureRepository.Get(f => f.IsPrimaryBool).Select(f => f.Path).Where(f => f.HasValue()).ToList();
        }
        public void UpdatePrimaryImage(int masterId, string picUrl)
        {
            var picModel = wikiDatabase.WikiPictureRepository.Get(f => f.MasterId == masterId && f.IsPrimaryBool).First();
            picModel.Path = picUrl;
            wikiDatabase.WikiPictureRepository.Update(picModel, "Path");
        }

        public IEnumerable<ItemReadTrackerModel> GetItemReadTrackData()
        {
            return userStoreDatabase.ItemReadTrackerRepository.GetAll();
        }

        public void CommonMetadata()
        {

            var primaryContent = wikiDatabase.PhoneSettingsRepository.PrimaryMetadatDisplayContent;

            var t = wikiDatabase.MetadataRepository.Get(f => f.TypeByEnum == MetadataType.Detail).ToList();
            var properties = t.GroupBy(f => f.Key)
                .Select(f => new
                {
                    Filter = f.Key,
                    Values = f.Select(c => c.Value).ToList()
                })
                .OrderByDescending(f => f.Values.Count)
                .ToList();

            var p = wikiDatabase.WikiPictureRepository.GetAll();
            var imagesGroup = p.GroupBy(g => g.MasterId)
                .Select(g => new
                {
                    g.Key,
                    Images = g.Select(f => f.Path).ToList()
                })
                .ToList();
        }

        public void AddMenuItem(string menuItemName, string tags, string titleOnThePage, int sequence)
        {
            wikiDatabase.AppMenuItemRepository.Add(new AppMenuItem { MenuItemName = menuItemName, Tags = tags, TitleOnThePage = titleOnThePage, Sequence = sequence }, checkAlreadyExists: true);
        }
        public IEnumerable<AppMenuItem> AppMenuItems()
        {
            return wikiDatabase.AppMenuItemRepository.GetAll().OrderBy(o => o.Sequence);
        }

        public void EnableWithPrimaryMetadataContent(List<string> primaryMetadataContent, int maxItemToDisplay)
        {
            wikiDatabase.PhoneSettingsRepository.EnablePrimaryMetadatDisplay(maxItemToDisplay);
            wikiDatabase.PhoneSettingsRepository.AddPrimaryMetadatDisplayContent(primaryMetadataContent);
        }
        public void DisablePrimaryMetadataContent()
        {
            wikiDatabase.PhoneSettingsRepository.RemoveAllPrimaryMetadatDisplayContent();
            wikiDatabase.PhoneSettingsRepository.DisablePrimaryMetadatDisplay();
        }
        public void RemoveMetadataInfo(int masterId, params string[] metadataKeys)
        {
            foreach (var key in metadataKeys)
            {
                foreach (var item in wikiDatabase.MetadataRepository.Get(f => f.MasterId == masterId).Where(f => f.Key == key))
                {
                    wikiDatabase.MetadataRepository.Delete(item.Id.ToString());
                }
            }
        }

        public void UpdateItemRead(string name, bool readStatus)
        {
            try
            {
                if (userStoreDatabase.ItemReadTrackerRepository.Get(f => f.ItemIdentifier.EqualsIgnoreCase(name)).Count() == 0)
                {
                    userStoreDatabase.ItemReadTrackerRepository.Add(new ItemReadTrackerModel { ItemIdentifier = name, IsRead = readStatus }, checkAlreadyExists: true);
                }
                else
                {
                    userStoreDatabase.ItemReadTrackerRepository.Update(new ItemReadTrackerModel { ItemIdentifier = name, IsRead = readStatus });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
