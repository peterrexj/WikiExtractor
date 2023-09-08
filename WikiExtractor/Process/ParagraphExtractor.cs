using HtmlAgilityPack;
using Pj.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WikiExtractor.DbModels;
using WikiExtractor.Exts;
using WikiExtractor.Models;

namespace WikiExtractor.Process
{
    public class ParagraphExtractor
    {
        private readonly HelperHtml helperHtml = new();

        public HtmlNodeCollection? _ItemsUnderMainBody(HtmlDocument document)
        {
            return document.DocumentNode.SelectNodes("//div[contains(@class, 'mw-body-content')]//div[contains(@class,'mw-parser-output')]").FirstOrDefault()?.ChildNodes;
        }

        public WikiPageModel? ExtractParaInfo(HtmlDocument document, string route, string name)
        {
            try
            {
                var allItemsUnderMainBody = _ItemsUnderMainBody(document);
                if (allItemsUnderMainBody == null || allItemsUnderMainBody.IsEmpty()) return null;
                Stack<Guid> picIds = new Stack<Guid>();

                var paraDetailsList = new List<WikiParagraphModel>();
                var imageDetailsList = new List<WikiPictureModel>();

                var headerInfo = new WikiParagraphModel
                {
                    Sequence = 1,
                    Header = string.Empty,
                };

                var mainHeader = new List<WikiParagraphDetailModel>();

                paraDetailsList.Add(headerInfo);

                bool foundHeaderParaInfo = false;
                string currentHeaderH2 = string.Empty;
                string currentSubHeaderH3 = string.Empty;

                var currentParaInfoModel = new WikiParagraphModel();

                foreach (var item in allItemsUnderMainBody)
                {
                    if (item.Name == "h2")
                    {
                        if (!foundHeaderParaInfo) foundHeaderParaInfo = true;
                        currentHeaderH2 = item.DecodedInnerText(removeNewLine: true);
                        currentSubHeaderH3 = string.Empty;

                        if (currentHeaderH2.HasValue() && currentParaInfoModel.Header.HasValue() && !currentHeaderH2.EqualsIgnoreCase(currentParaInfoModel.Header))
                        {
                            //New header and new section
                            paraDetailsList.Add(currentParaInfoModel.DeepClone());

                            currentParaInfoModel = new WikiParagraphModel
                            {
                                Sequence = paraDetailsList.Count + 1,
                                Header = currentHeaderH2,
                            };
                        }
                        else
                        {
                            if (currentParaInfoModel.Sequence == 0)
                            {
                                currentParaInfoModel.Sequence = paraDetailsList.Count + 1;
                            }
                        }

                        currentParaInfoModel.Header = currentHeaderH2;
                    }
                    else if (item.Name == "h3")
                    {
                        currentSubHeaderH3 = item.DecodedInnerText(removeNewLine: true);
                    }
                    else if (item.Name == "p" && (foundHeaderParaInfo == false || currentHeaderH2.IsEmpty()))
                    {
                        //Its para and not found the header (which is starting) and currentHeader is empty
                        //which is again not found any headers yet which means still on the first paragraph
                        //foundHeaderParaInfo = true;
                        if (item.DecodedInnerText(removeNewLine: false).HasValue())
                        {
                            //foundHeaderParaInfo = true;
                            var detailItem = new WikiParagraphDetailModel
                            {
                                ContentBuilder = new StringBuilder(item.DecodedInnerText(removeNewLine: true)),
                                Sequence = mainHeader.Count + 1
                            };
                            while (picIds.Count > 0)
                            {
                                detailItem.PictureLinks.Add(picIds.Pop());
                            }
                            mainHeader.Add(detailItem);
                        }
                    }
                    else if (item.Name == "p" && foundHeaderParaInfo)
                    {
                        var detailItem = new WikiParagraphDetailModel
                        {
                            SubHeader = currentSubHeaderH3,
                            ContentBuilder = new StringBuilder(item.DecodedInnerText(removeNewLine: true)),
                            Sequence = currentParaInfoModel.ParagraghInternalModels.Count + 1
                        };
                        while (picIds.Count > 0)
                        {
                            detailItem.PictureLinks.Add(picIds.Pop());
                        }

                        currentParaInfoModel.ParagraghInternalModels.Add(detailItem);
                    }
                    else if (item.Name == "figure" && item.Attributes.Any(f => f.Name.EqualsIgnoreCase("typeof") && f.Value.ContainsIgnoreCase("mw:File/Thumb")))
                    {
                        var imageItem = new WikiPictureModel
                        {
                            Sequence = imageDetailsList.Count + 1,
                            PictureId = Guid.NewGuid(),
                        };
                        var img = helperHtml.LoadHtmlAndSelectNodes(item.InnerHtml, "//img");
                        if (img != null && img.Count == 1)
                        {
                            img.FirstOrDefault()!.Attributes.Where(s => s.Name.HasValue() && s.Value.HasValue())
                                .Iter(s => imageItem.CustomMetadata.AddOrUpdate(s.Name, s.Value));
                        }
                        var imgCaption = helperHtml.LoadHtmlAndSelectNodes(item.InnerHtml, "//figcaption");
                        if (imgCaption != null && imgCaption.Count == 1 && imgCaption.FirstOrDefault()!.InnerText.HasValue())
                        {
                            imageItem.Caption = HtmlAgilityEx.DecodedInnerText(imgCaption.FirstOrDefault()!.InnerText, false);
                        }
                        imageDetailsList.Add(imageItem);

                        picIds.Push(imageItem.PictureId);
                    }
                    else if (item.Name == "div" && item.Attributes.Any(f => f.Name.EqualsIgnoreCase("class") && f.Value.ContainsIgnoreCase("thumb")))
                    {
                        var img = helperHtml.LoadHtmlAndSelectNodes(item.InnerHtml, "//img");
                        var subImageParentElms = helperHtml.LoadHtmlAndSelectNodes(item.InnerHtml, "//div[contains(@class, 'tsingle')]");

                        if (img != null && img.Count > 1 && subImageParentElms != null)
                        {
                            foreach (var subImageParentElm in subImageParentElms)
                            {
                                var subImageElm = helperHtml.LoadHtmlAndSelectNodes(subImageParentElm.InnerHtml, "//img");

                                var imgItem = ExtractImageElmInformation(imageDetailsList.Count, subImageElm, subImageParentElm);
                                imageDetailsList.Add(imgItem);
                                picIds.Push(imgItem.PictureId);
                            }
                        }
                        else
                        {
                            var imgItem = ExtractImageElmInformation(imageDetailsList.Count, img, item);
                            imageDetailsList.Add(imgItem);
                            picIds.Push(imgItem.PictureId);
                        }
                    }
                    else if (item.Name == "ul" && item.Attributes.Any(f => f.Name.EqualsIgnoreCase("class") && f.Value.ContainsIgnoreCase("gallery-packed")))
                    {
                        var subLiElms = helperHtml.LoadHtmlAndSelectNodes(item.InnerHtml, "//li[contains(@class, 'gallerybox')]");
                        foreach (var subLiElm in subLiElms)
                        {
                            var img = helperHtml.LoadHtmlAndSelectNodes(subLiElm.InnerHtml, "//div[contains(@class, 'thumb')]//img");
                            var imgItem = ExtractImageElmInformation(imageDetailsList.Count, img, item);
                            if (imgItem.Caption.IsEmpty())
                            {
                                var imgCaption = helperHtml.LoadHtmlAndSelectNodes(subLiElm.InnerHtml, "//div[contains(@class, 'gallerytext')]");
                                if (imgCaption != null && imgCaption.Count == 1 && imgCaption.FirstOrDefault()!.InnerText.HasValue())
                                {
                                    imgItem.Caption = HtmlAgilityEx.DecodedInnerText(imgCaption.FirstOrDefault()!.InnerText, false);
                                }
                            }
                            imageDetailsList.Add(imgItem);
                            picIds.Push(imgItem.PictureId);
                        }
                    }
                }



                var exclusionList = new[] { "See also", "References", "Further reading", "External links", "Bibliography" };
                var returnList = paraDetailsList.Where(f => !exclusionList.ContainsIgnoreCase(f.Header) && f.ParagraghInternalModels.Count > 0).ToList();


                var headerText = document.DocumentNode.SelectSingleNode("//h1//span").DecodedInnerText(removeNewLine: true);
                var returnValue = new WikiPageModel
                {
                    Header = headerText.HasValue() && headerText.Length > name?.Length ? headerText : name,
                    Route = route,
                    WikiParaCollection = returnList,
                    WikiPictureCollection = imageDetailsList,
                    MainParagraph = mainHeader
                };
                return returnValue;
            }
            catch (Exception)
            {
                return new WikiPageModel();
            }
        }

        //Relatd to the images extraction
        //Link to one of the recent content
        //if (currentParaInfoModel?.ParagraghInternalModels?.Any() == true)
        //{
        //    //currentParaInfoModel.ParagraghInternalModels.Last().PictureLinks.Add(imageItem.PictureId);
        //}
        //else if (currentParaInfoModel?.Header?.HasValue() == true)
        //{
        //    //There are no contents in the para sub details yet but the image is first item, need to create a dummy
        //    currentParaInfoModel.ParagraghInternalModels.Add(new WikiParagraphDetailModel
        //    {
        //        SubHeader = currentSubHeaderH3,
        //        ContentBuilder = new StringBuilder(),
        //        Sequence = currentParaInfoModel.ParagraghInternalModels.Count + 1
        //    });
        //    //currentParaInfoModel.ParagraghInternalModels.Last().PictureLinks.Add(imageItem.PictureId);
        //    currentParaInfoModel.ParagraghInternalModels.Last().PictureLinks.Add(picIds.Pop());
        //}
        //else if (mainHeader?.Any() == true)
        //{
        //    mainHeader.Last().PictureLinks.Add(imageItem.PictureId);
        //}
        //else
        //{
        //    //This is the first image in the page, so will need to create a dummy detail and add the picture
        //    mainHeader?.Add(new WikiParagraphDetailModel
        //    {
        //        ContentBuilder = new StringBuilder(""),
        //        Sequence = mainHeader.Count + 1,
        //    });
        //    //mainHeader?.Last().PictureLinks.Add(imageItem.PictureId);
        //    mainHeader?.Last().PictureLinks.Add(picIds.Pop());
        //}

        private WikiPictureModel ExtractImageElmInformation(int currentSequenceNo, HtmlNodeCollection imgNodes, HtmlNode imgParentContainer)
        {
            var imageItem = new WikiPictureModel
            {
                Sequence = currentSequenceNo + 1,
                PictureId = Guid.NewGuid(),
            };

            if (imgNodes != null && imgNodes.Count == 1)
            {
                imgNodes.FirstOrDefault()!.Attributes.Where(s => s.Name.HasValue() && s.Value.HasValue())
                    .Iter(s => imageItem.CustomMetadata.AddOrUpdate(s.Name, s.Value));
            }
            var imgCaption = helperHtml.LoadHtmlAndSelectNodes(imgParentContainer.InnerHtml, "//div[contains(@class, 'thumbcaption')]");
            if (imgCaption != null && imgCaption.Count == 1 && imgCaption.FirstOrDefault()!.InnerText.HasValue())
            {
                imageItem.Caption = HtmlAgilityEx.DecodedInnerText(imgCaption.FirstOrDefault()!.InnerText, false);
            }
            return imageItem;
        }
    }
}
