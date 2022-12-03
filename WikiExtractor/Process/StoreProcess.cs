using Pj.Library;
using WikiExtractor.DbModels;
using WikiExtractor.Exts;
using WikiExtractor.Models;
using WikiExtractor.Repository;

namespace WikiExtractor.Process
{
    public class StoreProcess
    {
        WikiDatabase wikiDatabase;

        public StoreProcess()
        {
            wikiDatabase = new WikiDatabase();
        }

        private int StoreMaster(WikiPageModel wikiPageModel)
        {
            return wikiDatabase.MasterRepository.Add(new Master
            {
                Name = wikiPageModel.Header,
                Route = wikiPageModel.Route,
            }, checkAlreadyExists: true);
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

                wikiDatabase.WikiPictureRepository.Add(imageType, checkAlreadyExists: true);
            }

            foreach (var imageType in wikiPageModel.WikiPictureCollection.Select(s => s.ToImageDbModel()))
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

                wikiDatabase.WikiPictureRepository.Add(imageType, checkAlreadyExists: true);
            }

            
        }

        private void StoreParagraph(WikiPageModel wikiPageModel, int masterId)
        {
            foreach (var paraheader in wikiPageModel.WikiParaCollection.OrderBy(f => f.Sequence).ToList())
            {
                paraheader.Header2InternalId = wikiDatabase.ParagraphHeader2Repository.Add(new ParagraphHeader2
                {
                    MasterId = masterId,
                    Header = paraheader.Header,
                    Sequence = paraheader.Sequence

                }, checkAlreadyExists: true);

                Dictionary<string, int> subHeaderDbMapping = new Dictionary<string, int>();

                foreach (var paraContentWithSubHeader in paraheader.ParagraghInternalModels.ToList())
                {
                    paraContentWithSubHeader.Header2InternalId = paraheader.Header2InternalId;
                    if (paraContentWithSubHeader.SubHeader.HasValue())
                    {
                        if (!subHeaderDbMapping.ContainsKey(paraContentWithSubHeader.SubHeader!))
                        {
                            paraContentWithSubHeader.Header3InternalId = wikiDatabase.ParagraphHeader3Repository.Add(new ParagraphHeader3
                            {
                                MasterId = masterId,
                                ParagraphHeader2Id = paraheader.Header2InternalId,
                                Header = paraContentWithSubHeader.SubHeader!,
                                Sequence = paraContentWithSubHeader.Sequence

                            }, checkAlreadyExists: true);
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
                    wikiDatabase.ParagraphContentRepository.Add(new ParagraphContent
                    {
                        MasterId = masterId,
                        ParagraphHeader2Id = paraheader.Header2InternalId,
                        ParagraphHeader3Id = header3Id,
                        Content = p01
                    }, checkAlreadyExists: true);
                }
                //Pass the internal model and get the data
                //Every para should have the collection of internal model which has the contents
                //Group by inside to get the data
            }
        }

        public int StoreInformation(WikiPageModel wikiPageModel, List<MetaDataModel> metadatas)
        {
            if (wikiPageModel == null)
            {
                return 0;
            }

            var masterId = StoreMaster(wikiPageModel);
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
    }
}
