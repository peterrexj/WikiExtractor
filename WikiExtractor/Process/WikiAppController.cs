using Pj.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            return GetViewModel(wikiDatabase.MasterRepository.Get(m => m.Id == id).FirstOrDefault());
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
                persona.MainContent= primaryContent.Content;
            }



            var parah2 = wikiDatabase.ParagraphHeader2Repository.Get(m => m.MasterId == master.Id).ToList();
            var parah3 = wikiDatabase.ParagraphHeader3Repository.Get(m => m.MasterId == master.Id).ToList();
            var parahContents = wikiDatabase.ParagraphContentRepository.Get(m => m.MasterId == master.Id).ToList();
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

        public List<PersonaViewModel> GetListOfWikiItems()
        {
            var masters = wikiDatabase.MasterRepository.GetAll();
            if (masters == null || masters.IsEmpty()) return new List<PersonaViewModel>();

            var pictures = wikiDatabase.WikiPictureRepository.Get(p => p.IsPrimaryBool);
            var primaryContent = wikiDatabase.ParagraphPrimaryContentRepository.GetAll();

            var wikiItems = new List<PersonaViewModel>();
            foreach (var item in masters)
            {
                wikiItems.Add(new PersonaViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    WikiPath = item.Route,
                    MainContent = primaryContent.FirstOrDefault(m => m.MasterId == item.Id)?.Content ?? "",
                    PicturePrimaryPath = pictures.FirstOrDefault(m => m.MasterId == item.Id)?.Path ?? "NoImageAvailable.png",
                    PicturePrimaryCaption = pictures.FirstOrDefault(m => m.MasterId == item.Id)?.Caption ?? ""
                });
            }
            return wikiItems;
        }

        public void MetadataBuild()
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
    }
}
