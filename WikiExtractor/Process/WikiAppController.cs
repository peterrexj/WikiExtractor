using Pj.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.DbModels;
using WikiExtractor.Exts;
using WikiExtractor.Models;
using WikiExtractor.Repository;
using WikiExtractor.ViewModels;
using Xamarin.Forms;

namespace WikiExtractor.Process
{
    public class WikiAppController
    {
        readonly IWikiDatabase wikiDatabase;
        public WikiAppController(IWikiDatabase wikiDb)
        {
            wikiDatabase = wikiDb;
        }

        public PersonaViewModel GetViewModelByRoute(string route)
        {
            return GetViewModel(wikiDatabase.MasterRepository.Get(m => m.Route == route).FirstOrDefault());
        }
        public PersonaViewModel GetViewModelById(int id)
        {
            return GetViewModelv2(id);
        }

        private PersonaViewModel GetViewModelv2(int masterId)
        {
            var persona = (from master in wikiDatabase.MasterRepository.GetAll()

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
                                       PicturePath = f.Path,
                                       PictureCaption = f.Caption.HasValue() && f.Caption.Length >= ConfigData.MinLengthOfPictureCaption ? f.Caption : string.Empty,
                                       Sequence = f.Sequence
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
                               Paragraphs = new List<Paragraph2ContentViewModel> { new Paragraph2ContentViewModel
                            {
                                Content = mainContentData.Content,
                                Header2 = masterData.master.Name,
                                Sequence = 0
                            } }
                           }).FirstOrDefault();

            var parah2 = wikiDatabase.ParagraphHeader2Repository.Get(m => m.MasterId == masterId).ToList();
            var parah3 = wikiDatabase.ParagraphHeader3Repository.Get(m => m.MasterId == masterId).ToList();
            var parahContents = wikiDatabase.ParagraphContentRepository.Get(m => m.MasterId == masterId).ToList();

            if (parahContents.Any())
            {
                int sequence = 1;
                foreach (var para2Item in parah2.OrderBy(f => f.Sequence))
                {
                    if (parahContents.Any(f => f.ParagraphHeader2Id != para2Item.Id))
                    {
                        persona.Paragraphs.Add(new Paragraph2ContentViewModel
                        {
                            Content = parahContents.FirstOrDefault(f => f.ParagraphHeader2Id == para2Item.Id)!.Content,
                            Header2 = para2Item.Header,
                            Para3s = new List<Paragraph3ContentViewModel>(),
                            Sequence = sequence++
                        });
                    }
                    else
                    {
                        persona.Paragraphs.Add(new Paragraph2ContentViewModel
                        {
                            Content = string.Empty,
                            Header2 = "Details",
                            Para3s = new List<Paragraph3ContentViewModel>(),
                            Sequence = sequence++
                        });
                    }

                    if (parah3.Any(f => f.ParagraphHeader2Id == para2Item.Id)) //Any items matching the para2 header
                    {
                        foreach (var para3Item in parah3.Where(f => f.ParagraphHeader2Id == para2Item.Id).OrderBy(f => f.Sequence))
                        {
                            if (parahContents.Any(f => f.ParagraphHeader2Id == para2Item.Id && f.ParagraphHeader3Id == para3Item.Id))
                            {
                                persona.Paragraphs.Last().Para3s!.Add(new Paragraph3ContentViewModel
                                {
                                    Content = parahContents.FirstOrDefault(f => f.ParagraphHeader2Id == para2Item.Id && f.ParagraphHeader3Id == para3Item.Id)!.Content,
                                    Header3 = para3Item.Header,
                                    Sequence = sequence++,
                                });
                            }
                        }
                    }

                }
            }
            return persona;
        }
        private PersonaViewModel GetViewModel(DbModels.Master master)
        {
            if (master == null) return null;

            var persona = new PersonaViewModel
            {
                Name = master.Name,
                WikiPath = master.Route,
                Metadatas = new List<MetadataViewModel>(),
                Pictures = new List<PictureViewModel>(),
                Paragraphs = new List<Paragraph2ContentViewModel>()
            };

            var pictures = wikiDatabase.WikiPictureRepository.Get(m => m.MasterId == master.Id).ToList();
            if (pictures.Any(f => f.IsPrimaryBool && f.Path.HasValue()))
            {
                persona.PicturePrimaryPath = pictures.FirstOrDefault(f => f.IsPrimaryBool && f.Path.HasValue())?.Path ?? "";
                persona.PicturePrimaryCaption = pictures.FirstOrDefault(f => f.IsPrimaryBool && f.Caption.HasValue())?.Caption ?? "";
            }

            persona.Pictures.AddRange(pictures.Where(f => /*!f.IsPrimaryBool &&*/ f.Path.HasValue())
                .OrderBy(f => f.Sequence)
                .Select(f => new PictureViewModel
                {
                    PicturePath = f.Path,
                    PictureCaption = f.Caption.HasValue() && f.Caption.Length >= ConfigData.MinLengthOfPictureCaption ? f.Caption : string.Empty,
                    Sequence = f.Sequence
                }));

            var metadatas = wikiDatabase.MetadataRepository.Get(m => m.MasterId == master.Id).ToList();

            if (metadatas.Any())
            {
                //Take first ONLY header information and rest of the header is left out
                if (metadatas.FirstOrDefault()!.TypeByEnum == MetadataType.PrimaryHeader &&
                    metadatas.FirstOrDefault()!.Value.HasValue())
                {
                    persona.NameSubstitue = metadatas.FirstOrDefault()!.Value;
                    //persona.Metadatas.Add(new MetadataViewModel
                    //{
                    //    GroupHeader = "",
                    //    Key = "",
                    //    Description = metadatas.FirstOrDefault()!.Value,
                    //    Sequence = metadatas.FirstOrDefault()!.Sequence
                    //});
                }

                string currentGroup = "";
                foreach (var item in metadatas.OrderBy(f => f.Sequence))
                {
                    if (item.TypeByEnum == MetadataType.GroupHeader)
                    {
                        currentGroup = item.Value;
                        continue;
                    }
                    if (item.TypeByEnum == MetadataType.Detail && item.Value.HasValue())
                    {
                        persona.Metadatas.Add(new MetadataViewModel
                        {
                            Key = item.Key,
                            Description = item.Value,
                            Sequence = item.Sequence,
                            GroupHeader = currentGroup
                        });
                    }
                }
            }

            var primaryContent = wikiDatabase.ParagraphPrimaryContentRepository.Get(m => m.MasterId == master.Id).FirstOrDefault();
            if (primaryContent != null && primaryContent.Content.HasValue())
            {
                persona.Paragraphs.Add(new Paragraph2ContentViewModel
                {
                    Content = primaryContent.Content,
                    Header2 = persona.Name,
                    Sequence = 0
                });
                persona.MainContent = primaryContent.Content;
            }

            var parah2 = wikiDatabase.ParagraphHeader2Repository.Get(m => m.MasterId == master.Id);
            var parah3 = wikiDatabase.ParagraphHeader3Repository.Get(m => m.MasterId == master.Id);
            var parahContents = wikiDatabase.ParagraphContentRepository.Get(m => m.MasterId == master.Id);
            if (parahContents.Any())
            {
                int sequence = 1;
                foreach (var para2Item in parah2.OrderBy(f => f.Sequence))
                {
                    if (parahContents.Any(f => f.ParagraphHeader2Id != para2Item.Id))
                    {
                        persona.Paragraphs.Add(new Paragraph2ContentViewModel
                        {
                            Content = parahContents.FirstOrDefault(f => f.ParagraphHeader2Id == para2Item.Id)!.Content,
                            Header2 = para2Item.Header,
                            Para3s = new List<Paragraph3ContentViewModel>(),
                            Sequence = sequence++
                        });
                    }
                    else
                    {
                        persona.Paragraphs.Add(new Paragraph2ContentViewModel
                        {
                            Content = string.Empty,
                            Header2 = "Details",
                            Para3s = new List<Paragraph3ContentViewModel>(),
                            Sequence = sequence++
                        });
                    }

                    if (parah3.Any(f => f.ParagraphHeader2Id == para2Item.Id)) //Any items matching the para2 header
                    {
                        foreach (var para3Item in parah3.Where(f => f.ParagraphHeader2Id == para2Item.Id).OrderBy(f => f.Sequence))
                        {
                            if (parahContents.Any(f => f.ParagraphHeader2Id == para2Item.Id && f.ParagraphHeader3Id == para3Item.Id))
                            {
                                persona.Paragraphs.Last().Para3s!.Add(new Paragraph3ContentViewModel
                                {
                                    Content = parahContents.FirstOrDefault(f => f.ParagraphHeader2Id == para2Item.Id && f.ParagraphHeader3Id == para3Item.Id)!.Content,
                                    Header3 = para3Item.Header,
                                    Sequence = sequence++,
                                });
                            }
                        }
                    }

                }
            }

            return persona;
        }

        public IEnumerable<PersonaViewModel> GetListOfWikiItems(List<string> tags = null)
        {
            //var masters = wikiDatabase.MasterRepository.GetAll();
            //if (masters == null || masters.IsEmpty()) return new List<PersonaViewModel>();

            var isPrimaryMetadataContentEnabled = wikiDatabase.PhoneSettingsRepository.IsPrimaryMetadatDisplayEnabled;
            var primaryMetadataContentFields = wikiDatabase.PhoneSettingsRepository.PrimaryMetadatDisplayContent;

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

                   where tags?.Contains(tag.Name) == true || tag.Name.IsEmpty()
                   group new { master, mainContItem, primaryPic, metadata, tagItem, tag } by new { master.Id } into masterGroup

                   select new PersonaViewModel
                   {
                       Id = masterGroup.FirstOrDefault()!.master.Id,
                       Name = masterGroup.FirstOrDefault()!.master.Name,
                       WikiPath = masterGroup.FirstOrDefault()!.master.Route,
                       MainContent = masterGroup.FirstOrDefault()!.mainContItem?.Content ?? "",
                       PicturePrimaryPath = masterGroup.FirstOrDefault()!.primaryPic?.Path ?? "NoImageAvailable.png",
                       PicturePrimaryCaption = masterGroup.FirstOrDefault()!.primaryPic?.Caption ?? "",
                       IsPrimaryMetadataContentEnabled = isPrimaryMetadataContentEnabled,
                       PrimaryMetadataContent = isPrimaryMetadataContentEnabled ? masterGroup.Select(f => f.metadata).Where(f => primaryMetadataContentFields.Contains(f.Key) && f.Value.HasValue())
                        .Select(f => new MetadataViewModel
                        {
                            Key = f.Key,
                            Description = f.Value
                        }).ToList() : new List<MetadataViewModel>(),
                       //Tags = masterGroup.Select(f => f.tag).Select(f => f.Name).Distinct().ToList(),
                       IsBusy = false
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

        public void CommonMetadata()
        {
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

        public void EnableWithPrimaryMetadataContent(List<string> primaryMetadataContent)
        {
            wikiDatabase.PhoneSettingsRepository.EnablePrimaryMetadatDisplay();
            wikiDatabase.PhoneSettingsRepository.AddPrimaryMetadatDisplayContent(primaryMetadataContent);
        }
        public void DisablePrimaryMetadataContent()
        {
            wikiDatabase.PhoneSettingsRepository.RemoveAllPrimaryMetadatDisplayContent();
            wikiDatabase.PhoneSettingsRepository.DisablePrimaryMetadatDisplay();
        }
    }
}
