using Pj.Library;
using WikiExtractor.DbModels;
using WikiExtractor.Exts;
using WikiExtractor.Models;
using WikiExtractor.Repository;
using WikiExtractor.Repository.UserStore;

namespace WikiExtractor.Process
{
    public class StoreProcess
    {
        WikiDatabase wikiDatabase;
        UserStoreDatabase userStoreDatabase;
        public StoreProcess()
        {
            wikiDatabase = new WikiDatabase();
            userStoreDatabase = new UserStoreDatabase();
        }

        private int StoreMaster(WikiPageModel wikiPageModel, WikiWhatToExtractModel wikiExtractInfo)
        {
            return wikiDatabase.MasterRepository.Add(new Master
            {
                Name = wikiPageModel.Header,
                Route = wikiPageModel.Route,
                Sequence = wikiExtractInfo.Sequence,
            }, checkAlreadyExists: true);
        }

        public void StoreTags(List<string> tags, int masterId)
        {
            if (tags == null || tags.Count == 0) { return; }
            foreach (var tag in tags)
            {
                var tagId = wikiDatabase.TagRepository.Add(new Tag { Name = tag.Trim() }, true);
                wikiDatabase.TagItemRepository.Add(new TagItem { MasterId = masterId, TagId = tagId }, true);
            }
        }

        private void StoreMetadata(List<MetaDataModel> metadatas, int masterId)
        {
            foreach (var metadata in metadatas.Where(f => f.Type != MetadataType.Image))
            {
                wikiDatabase.MetadataRepository.Add(new Metadata
                {
                    MasterId = masterId,
                    Key = metadata.Name,
                    Value = metadata.Description,
                    Type = metadata.Type.ToString(),
                    Sequence = metadata.Sequence
                }, checkAlreadyExists: true);
            }
        }

        private void StorePrimaryContent(WikiPageModel wikiPageModel, int masterId)
        {
            wikiDatabase.ParagraphPrimaryContentRepository.Add(new ParagraphPrimaryContent
            {
                MasterId = masterId,
                Content = wikiPageModel.MainParagraph.Content()
            }, checkAlreadyExists: true);
        }

        private void StoreWikiPictures(WikiPageModel wikiPageModel, List<MetaDataModel> metadatas, int masterId)
        {
            bool hasPrimaryWikiPictureIdentified = false;
            int counter = 0;
            foreach (var imageType in metadatas.Where(f => f.Type == MetadataType.Image).Select(s => s.ToImageDbModel()).OrderBy(f => f.Sequence))
            {
                imageType.MasterId = masterId;
                if (hasPrimaryWikiPictureIdentified == false)
                {
                    hasPrimaryWikiPictureIdentified = true;
                    imageType.IsPrimary = 1;
                }
                else
                {
                    imageType.IsPrimary = 0;
                }
                imageType.Sequence = counter++;
                wikiDatabase.WikiPictureRepository.Add(imageType, checkAlreadyExists: true);
            }

            foreach (var imageType in wikiPageModel.WikiPictureCollection)
            {
                var imageDbModel = imageType.ToImageDbModel();
                imageDbModel.MasterId = masterId;
                if (hasPrimaryWikiPictureIdentified == false)
                {
                    hasPrimaryWikiPictureIdentified = true;
                    imageDbModel.IsPrimary = 1;
                }
                else
                {
                    imageDbModel.IsPrimary = 0;
                }

                wikiDatabase.WikiPictureRepository.Add(imageDbModel, checkAlreadyExists: true);
                imageType.Id = imageDbModel.Id;
            }
        }

        private void StoreParagraph(WikiPageModel wikiPageModel, int masterId)
        {
            var para2DbModels = new List<ParagraphHeader2>();
            var para3DbModels = new List<ParagraphHeader3>();
            var paraContentDbModels = new List<ParagraphContent>();
            var paraImageDbModels = new List<ParagraphImage>();

            Func<ParagraphHeader2, int> UpdatePara2 = (model) =>
            {
                var data = para2DbModels.FirstOrDefault(f => f.MasterId == model.MasterId && f.Header == model.Header && f.Sequence == model.Sequence);
                if (data != null)
                {
                    return data.Id;
                }
                else
                {
                    model.Id = wikiDatabase.ParagraphHeader2Repository.Add(model, checkAlreadyExists: false);
                    para2DbModels.Add(model);
                    return model.Id;
                }
            };

            Func<ParagraphHeader3, int> UpdatePara3 = (model) =>
            {
                var data = para3DbModels.FirstOrDefault(f => f.MasterId == model.MasterId && f.Header == model.Header && f.Sequence == model.Sequence && f.ParagraphHeader2Id == model.ParagraphHeader2Id);
                if (data != null)
                {
                    return data.Id;
                }
                else
                {
                    model.Id = wikiDatabase.ParagraphHeader3Repository.Add(model, checkAlreadyExists: false);
                    para3DbModels.Add(model);
                    return model.Id;
                }
            };

            Func<ParagraphContent, int> UpdateParaContent = (model) =>
            {
                var data = paraContentDbModels.FirstOrDefault(f => f.MasterId == model.MasterId && f.ParagraphHeader2Id == model.ParagraphHeader2Id && f.ParagraphHeader3Id == model.ParagraphHeader3Id 
                    && f.HashContent == model.HashContent);
                if (data != null)
                {
                    return data.Id;
                }
                else
                {
                    model.Id = wikiDatabase.ParagraphContentRepository.Add(model, checkAlreadyExists: false);
                    paraContentDbModels.Add(model);
                    return model.Id;
                }
            };

            Func<ParagraphImage, int> UpdateParaImage = (model) =>
            {
                var data = paraImageDbModels.FirstOrDefault(f => f.MasterId == model.MasterId && f.ImageId == model.ImageId && f.ParagraphId == model.ParagraphId);
                if (data != null)
                {
                    return data.Id;
                }
                else
                {
                    model.Id = wikiDatabase.ParagraphImageRepository.Add(model, checkAlreadyExists: false);
                    paraImageDbModels.Add(model);
                    return model.Id;
                }
            };

            foreach (var paraheader in wikiPageModel.WikiParaCollection.OrderBy(f => f.Sequence).ToList())
            {
                //Only when there is at least one para with content
                //var hasAnyContent = paraheader.ParagraghInternalModels.Select(f => f.Content).Any(f => f.HasValue());

                //Add the Paragraph header 2
                paraheader.Header2InternalId = UpdatePara2(new ParagraphHeader2 { MasterId = masterId, Header = paraheader.Header, Sequence = paraheader.Sequence });

                Dictionary<string, int> subHeaderDbMapping = new Dictionary<string, int>();

                foreach (var paraContentWithSubHeader in paraheader.ParagraghInternalModels.ToList())
                {
                    paraContentWithSubHeader.Header2InternalId = paraheader.Header2InternalId;
                    if (paraContentWithSubHeader.SubHeader.HasValue())
                    {
                        //For sub header
                        if (!subHeaderDbMapping.ContainsKey(paraContentWithSubHeader.SubHeader!))
                        {
                            paraContentWithSubHeader.Header3InternalId = UpdatePara3(new ParagraphHeader3
                            {
                                MasterId = masterId,
                                ParagraphHeader2Id = paraheader.Header2InternalId,
                                Header = paraContentWithSubHeader.SubHeader!,
                                Sequence = paraContentWithSubHeader.Sequence
                            });
                            subHeaderDbMapping.AddOrUpdate(paraContentWithSubHeader.SubHeader!, paraContentWithSubHeader.Header3InternalId);
                        }
                        else
                        {
                            paraContentWithSubHeader.Header3InternalId = subHeaderDbMapping[paraContentWithSubHeader.SubHeader!]!;
                        }
                    }
                }

                var grps = paraheader.ParagraghInternalModels.GroupBy(f => f.SubHeader).ToList();
                foreach (var grp in grps)
                {
                    var header3Id = 0;

                    var p01 = string.Join(Environment.NewLine, grp.OrderBy(s => s.Sequence).Select(F => F.Content));
                    if (grp.Key.IsEmpty())
                    {

                    }
                    else if (paraheader.ParagraghInternalModels.Any(f => f.SubHeader == grp.Key))
                    {
                        header3Id = paraheader.ParagraghInternalModels.FirstOrDefault(f => f.SubHeader == grp.Key)!.Header3InternalId;
                    }

                    foreach (var grpItem in grp.OrderBy(f => f.Sequence))
                    {
                        var contentId = UpdateParaContent(new ParagraphContent
                        {
                            MasterId = masterId,
                            ParagraphHeader2Id = paraheader.Header2InternalId,
                            ParagraphHeader3Id = header3Id,
                            HashContent = grpItem.Content.GetHashCode(),
                            Content = grpItem.Content
                        });
                        if (grpItem.PictureLinks.Any())
                        {
                            foreach (var pimage in grpItem.PictureLinks)
                            {
                                var pId = wikiPageModel.WikiPictureCollection.FirstOrDefault(f => f.PictureId == pimage)?.Id;
                                if (pId != null && pId.Value > 0)
                                {
                                    UpdateParaImage(new ParagraphImage
                                    {
                                        MasterId = masterId,
                                        ParagraphId = contentId,
                                        ImageId = pId.Value
                                    });
                                }
                            }
                        }
                    }
                }
                //Pass the internal model and get the data
                //Every para should have the collection of internal model which has the contents
                //Group by inside to get the data
            }
        }

        public int StoreInformation(WikiPageModel wikiPageModel, List<MetaDataModel> metadatas, WikiWhatToExtractModel wikiExtractInfo)
        {
            if (wikiPageModel == null)
            {
                return 0;
            }

            if ((metadatas == null || metadatas.IsEmpty()) &&
                (wikiPageModel.WikiParaCollection == null || wikiPageModel.WikiParaCollection.IsEmpty()) &&
                (wikiPageModel.MainParagraph == null || wikiPageModel.MainParagraph.IsEmpty()))
            {
                return 0;
            }

            var masterId = StoreMaster(wikiPageModel, wikiExtractInfo);
            StoreTags(wikiExtractInfo.Tags, masterId);
            StoreMetadata(metadatas, masterId);
            StorePrimaryContent(wikiPageModel, masterId);
            StoreWikiPictures(wikiPageModel, metadatas, masterId);
            StoreParagraph(wikiPageModel, masterId);

            //var all = wikiDatabase.MasterRepository.GetAll().ToList();
            //var a01With = wikiDatabase.ParagraphHeader3Repository.GetAll().ToList();
            //var allMeta = wikiDatabase.MetadataRepository.GetAll();
            //var allMain = wikiDatabase.ParagraphPrimaryContentRepository.GetAll();

            //if (allMeta.Count() != metadatas.Count)
            //{
            //    var except = metadatas.Where(f => !allMeta.Any(a => a.Key == f.Name && a.Value == f.Description && a.Type == f.Type.ToString() && a.Sequence == f.Sequence)).ToList();
            //}

            //var allHeader = wikiDatabase.ParagraphHeader2Repository.GetAll().ToList();
            //var allSubHeader = wikiDatabase.ParagraphHeader3Repository.GetAll().ToList();
            //var allContent = wikiDatabase.ParagraphContentRepository.GetAll().ToList();

            //var allRawList = metadatas.Where(f => f.Type == MetadataType.Image).Select(s => s.ToImageDbModel()).Select(s => s.Path)
            //    .Union(wikiPageModel.WikiPictureCollection.Select(s => s.ToImageDbModel()).Select(s => s.Path))
            //    .ToList();
            //var allImages = wikiDatabase.WikiPictureRepository.GetAll().ToList();

            return masterId;
        }

        public void CleanEntry(int masterId)
        {
            wikiDatabase.ParagraphHeader3Repository.DeleteByMasterId(masterId);
            wikiDatabase.ParagraphHeader2Repository.DeleteByMasterId(masterId);
            wikiDatabase.ParagraphContentRepository.DeleteByMasterId(masterId);
            wikiDatabase.ParagraphPrimaryContentRepository.DeleteByMasterId(masterId);
            wikiDatabase.WikiPictureRepository.DeleteByMasterId(masterId);
            wikiDatabase.MetadataRepository.DeleteByMasterId(masterId);
            wikiDatabase.TagItemRepository.DeleteByMasterId(masterId);
            wikiDatabase.MasterRepository.Delete(masterId.ToString());
        }

        public void UpdateName(string name, int masterId)
        {
            var masterData = wikiDatabase.MasterRepository.Get(f => f.Id == masterId).FirstOrDefault();
            if (masterData != null)
            {
                masterData.Name = name;
                wikiDatabase.MasterRepository.Update(masterData, "Name");
            }
        }
    }
}
