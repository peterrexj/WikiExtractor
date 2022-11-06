using Pj.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.Models;
using WikiExtractor.Repository;
using WikiExtractor.ViewModels;
using Xamarin.Forms;

namespace WikiExtractor.Process
{
    public class WikiAppController
    {
        readonly WikiDatabase wikiDatabase;
        public WikiAppController()
        {
            wikiDatabase = new WikiDatabase();
        }

        public PersonaViewModel GetViewModel(string route)
        {
            var master = wikiDatabase.MasterRepository.Get(m => m.Route == route).FirstOrDefault();
            if (master == null) return null;

            var persona = new PersonaViewModel
            {
                Name = master.Name,
                WikiPath = master.Route,
                Metadatas = new List<MetadataViewModel>(),
                Pictures = new List<PictureViewModel>(),
                Paragraphs = new List<ParagraphContentViewModel>()
            };

            var pictures = wikiDatabase.WikiPictureRepository.Get(m => m.MasterId == master.Id).ToList();
            if (pictures.Any(f => f.IsPrimaryBool && f.Path.HasValue()))
            {
                persona.PicturePrimaryPath = pictures.FirstOrDefault(f => f.IsPrimaryBool && f.Path.HasValue())?.Path ?? "";
                persona.PicturePrimaryCaption = pictures.FirstOrDefault(f => f.IsPrimaryBool && f.Caption.HasValue())?.Caption ?? "";
            }

            persona.Pictures.AddRange(pictures.Where(f => !f.IsPrimaryBool && f.Path.HasValue())
                .OrderBy(f => f.Sequence)
                .Select(f => new PictureViewModel
                {
                    PicturePath = f.Path,
                    PictureCaption = f.Caption,
                    Sequence = f.Sequence
                }));

            var metadatas = wikiDatabase.MetadataRepository.Get(m => m.MasterId == master.Id).ToList();

            if (metadatas.Any())
            {
                //Take first ONLY header information and rest of the header is left out
                if (metadatas.FirstOrDefault()!.TypeByEnum == MetadataType.PrimaryHeader &&
                    metadatas.FirstOrDefault()!.Value.HasValue())
                {
                    persona.Metadatas.Add(new MetadataViewModel
                    {
                        GroupHeader = "",
                        Key = "",
                        Description = metadatas.FirstOrDefault()!.Value,
                        Sequence = metadatas.FirstOrDefault()!.Sequence
                    });
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


            var parah2 = wikiDatabase.ParagraphHeader2Repository.Get(m => m.MasterId == master.Id).ToList();
            var parah3 = wikiDatabase.ParagraphHeader3Repository.Get(m => m.MasterId == master.Id).ToList();
            var parahContents = wikiDatabase.ParagraphContentRepository.Get(m => m.MasterId == master.Id).ToList();
            if (parahContents.Any())
            {


                int sequence = 1;
                foreach (var para2Item in parah2.OrderBy(f => f.Sequence))
                {
                    if (parah3.Any(f => f.ParagraphHeader2Id == para2Item.Id))
                    {
                        foreach (var para3Item in parah3.Where(f => f.ParagraphHeader2Id == para2Item.Id).OrderBy(f => f.Sequence))
                        {
                            if (parahContents.Any(f => f.ParagraphHeader2Id == para2Item.Id && f.ParagraphHeader3Id == para3Item.Id))
                            {
                                persona.Paragraphs.Add(new ParagraphContentViewModel
                                {
                                    Content = parahContents.FirstOrDefault(f => f.ParagraphHeader2Id == para2Item.Id && f.ParagraphHeader3Id == para3Item.Id)!.Content,
                                    Header2 = para2Item.Header,
                                    Header3 = para3Item.Header,
                                    Sequence = sequence++,
                                });
                            }
                        }
                    }
                    else
                    {
                        if (parahContents.Any(f => f.ParagraphHeader2Id != para2Item.Id))
                        {
                            persona.Paragraphs.Add(new ParagraphContentViewModel
                            {
                                Content = parahContents.FirstOrDefault(f => f.ParagraphHeader2Id == para2Item.Id)!.Content,
                                Header2 = para2Item.Header,
                                Header3 = "",
                                Sequence = sequence++
                            });
                        }
                    }
                }
            }

            return persona;
        }
    }
}
