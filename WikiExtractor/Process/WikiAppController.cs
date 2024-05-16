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
            PersonaViewModel wikiItem = null;
            List<ParagraphHeader2> para2Items = null;
            List<ParagraphHeader3> para3Items = null;
            List<ParagraphContent> contents = null;
            List<Tuple<int, PictureViewModel>> pictures = null;
            TaskGroup tgrp = new();

            tgrp.Add(() => wikiItem = GetItemData(masterId));
            tgrp.Add(() => contents = GetItemParagraphContents(masterId));
            tgrp.Add(() => para2Items = GetItemParagraph2s(masterId));
            tgrp.Add(() => para3Items = GetItemParagraph3s(masterId));
            tgrp.WaitAll();
            tgrp.Add(() => pictures = GetItemComputedImages(masterId, wikiItem));
            tgrp.WaitAll();

            BuildParagraph2Paragraph3(wikiItem, contents, para2Items, para3Items, pictures);
            PictureCaptionUpdate(wikiItem);
            return wikiItem;
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
                   from itemReadStatus in itemReadStatusGroup.DefaultIfEmpty(new ItemReadTrackerModel { ItemIdentifier = master.Name, IsRead = 0 })

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
                       ItemReadStatus = masterGroup.FirstOrDefault()!.itemReadStatus.IsReadAsBool,
                   };
        }

        #region Get Data
        public PersonaViewModel GetItemData(int masterId)
        {
            var item = (from master in wikiDatabase.MasterRepository.GetAll().Where(master => master.Id == masterId)

                        join picJoin in wikiDatabase.WikiPictureRepository.GetAll().Where(f => f.MasterId == masterId) on master.Id equals picJoin.MasterId into picGroup
                        from pic in picGroup.DefaultIfEmpty(new WikiPicture { MasterId = master.Id, Path = "NoImageAvailable.png", Caption = string.Empty })

                        join metadataJoin in wikiDatabase.MetadataRepository.GetAll().Where(f => f.MasterId == masterId) on master.Id equals metadataJoin.MasterId into metadataGrp
                        from metadata in metadataGrp.DefaultIfEmpty(new Metadata { Id = 0, MasterId = master.Id })

                        join mainCont in wikiDatabase.ParagraphPrimaryContentRepository.GetAll().Where(f => f.MasterId == masterId) on master.Id equals mainCont.MasterId into mainContGroup
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
                                    Height = f.Height,
                                    ParentName = masterData.master.Name,
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
            return item;
        }
        public List<Tuple<int, PictureViewModel>> GetItemComputedImages(int masterId, PersonaViewModel personaViewModel)
        {
            var pComputedImages = (from p in wikiDatabase.ParagraphImageRepository.Get(m => m.MasterId == masterId)

                               join parahContentsJoin in wikiDatabase.ParagraphContentRepository.Get(m => m.MasterId == masterId) on p.ParagraphId equals parahContentsJoin.Id into parahContentsGrp
                               from paraContents in parahContentsGrp.DefaultIfEmpty(new ParagraphContent { MasterId = masterId, Id = 0 })

                               join picJoin in personaViewModel.Pictures on p.ImageId equals picJoin.Id into picGroup
                               from pic in picGroup.DefaultIfEmpty(new PictureViewModel { Id = 0 })

                               where pic != null && pic.Id != 0 && paraContents != null && paraContents.Id != 0
                               select new Tuple<int, PictureViewModel>(paraContents.Id, pic)).ToList();

            return pComputedImages;
        }
        public List<ParagraphContent> GetItemParagraphContents(int masterId)
        {
            return wikiDatabase.ParagraphContentRepository.Get(m => m.MasterId == masterId).ToList();
        }
        public List<ParagraphHeader2> GetItemParagraph2s(int masterId)
        {
            return wikiDatabase.ParagraphHeader2Repository.Get(m => m.MasterId == masterId).ToList();
        }
        public List<ParagraphHeader3> GetItemParagraph3s(int masterId)
        {
            return wikiDatabase.ParagraphHeader3Repository.Get(m => m.MasterId == masterId).ToList();
        }
        public void BuildParagraph2Paragraph3(PersonaViewModel wikiItem, 
            List<ParagraphContent> contents, 
            List<ParagraphHeader2> paragraph2s, List<ParagraphHeader3> paragraph3s,
            List<Tuple<int, PictureViewModel>> pictures)
        {
            int indexCounter = 0;

            if (contents.Count != 0)
            {
                int sequence = 1;
                //for each para2 item ordered by sequence
                foreach (var para2Item in paragraph2s.OrderBy(f => f.Sequence))
                {
                    var para2Contents = contents.Where(f => f.ParagraphHeader2Id == para2Item.Id && f.ParagraphHeader3Id == 0);
                    if (para2Contents.Any())
                    {
                        ++indexCounter; //there are multiple content and they will fall into the same index
                        foreach (var paraContent in para2Contents)
                        {
                            wikiItem.Paragraphs.Add(new Paragraph2ContentViewModel
                            {
                                Content = paraContent.Content,
                                Header2 = para2Item.Header,
                                Sequence = sequence++,
                                PicLinks = pictures.Where(f => f.Item1 == paraContent.Id).Select(f => f.Item2).ToList(),
                                Id = indexCounter,
                            });
                        }
                    }
                    else
                    {
                        wikiItem.Paragraphs.Add(new Paragraph2ContentViewModel
                        {
                            Content = string.Empty,
                            Header2 = para2Item.Header,
                            Sequence = sequence++
                        });
                    }

                    //Any items matching the Para2 header
                    foreach (var para3Item in paragraph3s.Where(f => f.ParagraphHeader2Id == para2Item.Id).OrderBy(f => f.Sequence).GroupBy(f => f.Header))
                    {
                        ++indexCounter; //there are multiple content and they will fall into the same index
                        foreach (var par3 in para3Item)
                        {
                            var newPara3Container = new Paragraph3ContainerViewModel { Header = par3.Header };

                            foreach (var paraContent in contents.Where(f => f.ParagraphHeader2Id == para2Item.Id && f.ParagraphHeader3Id == par3.Id))
                            {
                                newPara3Container.Para3s.Add(new Paragraph3ContentViewModel
                                {
                                    Content = paraContent.Content,
                                    Sequence = sequence++,
                                    PicLinks = pictures.Where(f => f.Item1 == paraContent.Id).Select(f => f.Item2).ToList(),
                                    Id = indexCounter, //multiple index counter
                                });
                            }
                            wikiItem.Paragraphs.Last().Para3Containers.Add(newPara3Container);
                        }
                    }
                }
            }
        }

        public void PictureCaptionUpdate(PersonaViewModel wikiItem)
        {
            int picCounter = 1;
            foreach (var pic in wikiItem.Pictures)
            {
                if (picCounter == 1 && pic.PictureCaption.IsEmpty())
                {
                    pic.PictureCaption = pic.ParentName;
                }
                pic.CurrentCounter = picCounter++;
            }
        }

        #endregion


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
                var readStatusInt = readStatus ? 1 : 0;
                var itemReadStatus = userStoreDatabase.ItemReadTrackerRepository.Get(f => f.ItemIdentifier.EqualsIgnoreCase(name)).FirstOrDefault();
                if (itemReadStatus == null)
                {
                    userStoreDatabase.ItemReadTrackerRepository.Add(new ItemReadTrackerModel { ItemIdentifier = name, IsRead = readStatusInt }, checkAlreadyExists: true);
                }
                else
                {
                    itemReadStatus.IsRead = readStatusInt;
                    userStoreDatabase.ItemReadTrackerRepository.Update(itemReadStatus);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
