using HtmlAgilityPack;
using Pj.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using WikiExtractor.Exts;
using WikiExtractor.Models;

namespace WikiExtractor.Process
{
    public class MetadataExtractor
    {
        private readonly HelperHtml helperHtml = new();
        private int _counter = 0;

        public List<MetaDataModel> ExtractMetadataInfo(HtmlDocument document, 
            Dictionary<string, string>? additionalDataCaptured,
            List<string> excludedAdditionalMetadata = null)
        {
            _counter = 1;
            string headerOnGroupOfDetail = string.Empty;
            var tableRows = document.DocumentNode.SelectNodes("//table[contains(@class, 'infobox vcard')]/tbody/tr");
            if (tableRows == null || tableRows.Count == 0)
            {
                tableRows = document.DocumentNode.SelectNodes("//table[contains(@class, 'infobox')]/tbody/tr");
            }
            var metaDataDict = new List<MetaDataModel>();

            if (additionalDataCaptured != null)
            {
                foreach (var additionalData in additionalDataCaptured)
                {
                    if (excludedAdditionalMetadata != null && excludedAdditionalMetadata.ContainsIgnoreCase(additionalData.Key))
                    {
                        continue;
                    }
                    if (additionalData.Value.HasValue())
                    {
                        metaDataDict.Add(new MetaDataModel(_counter++, "", MetadataType.Detail)
                        {
                            Name = additionalData.Key,
                            Description = additionalData.Value
                        });
                    }
                }
            }

            if (tableRows?.Count > 0)
            {
                foreach (var tableRow in tableRows)
                {
                    var loadedInnerHtml = helperHtml.LoadHtmlDocument(tableRow.InnerHtml);

                    var infoboxAbove = ExtractInfoboxAbove(loadedInnerHtml);
                    if (infoboxAbove != null) metaDataDict.Add(infoboxAbove);

                    var infoboxImage = ExtractInfoboxImage(loadedInnerHtml);
                    if (infoboxImage != null && infoboxImage.Count > 0) metaDataDict.AddRange(infoboxImage);

                    var infoboxHeader = ExtractInfoboxHeader(loadedInnerHtml);
                    if (infoboxHeader != null)
                    {
                        metaDataDict.Add(infoboxHeader);
                        headerOnGroupOfDetail = infoboxHeader.Description;
                    }

                    var infoboxLabel = ExtractInfoboxLabel(loadedInnerHtml); //, metaDataDict.Count);
                    if (infoboxLabel != null)
                    {
                        if (HtmlAgilityEx.ContainsStartDot(infoboxLabel.Name))
                        {
                            if (headerOnGroupOfDetail.IsEmpty())
                            {
                                headerOnGroupOfDetail = metaDataDict.Where(f => f.Type == MetadataType.Detail).Last().Name ?? string.Empty;
                            }
                            infoboxLabel.Name = $"{HtmlAgilityEx.RemoveStartDot(infoboxLabel.Name)} ({headerOnGroupOfDetail})".Trim();
                        }
                        else
                        {
                            headerOnGroupOfDetail = string.Empty;
                        }
                        metaDataDict.Add(infoboxLabel);
                    }
                }
            }

            return metaDataDict;
        }

        private MetaDataModel? ExtractInfoboxAbove(HtmlDocument htmlDoc)
        {
            try
            {
                var nodes = helperHtml.LoadHtmlAndSelectNodes(htmlDoc, "//th[contains(@class, 'infobox-above')]");
                if (nodes == null || nodes.IsEmpty()) return null;

                var childNodes = nodes.FirstOrDefault()?.ChildNodes;
                if (childNodes?.IsEmpty() == true) return null;

                StringBuilder content = new();

                var metaData = new MetaDataModel(_counter++, "Info", MetadataType.PrimaryHeader);
                foreach (var cNode in childNodes!)
                {
                    if (cNode.Name.EqualsIgnoreCase("br"))
                    {
                        content.Append(" ");
                        //content.AppendLine();
                    }
                    else
                    {
                        content.Append(cNode.DecodedInnerText(removeNewLine: true));
                    }
                }

                metaData.Description = content.ToString();
                return metaData;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private List<MetaDataModel>? ExtractInfoboxImage(HtmlDocument htmlDoc)
        {
            try
            {
                var metaDatas = new List<MetaDataModel>();

                var nodes = helperHtml.LoadHtmlAndSelectNodes(htmlDoc, "//td[contains(@class, 'infobox-image')]");
                if (nodes == null || nodes.IsEmpty())
                {
                    nodes = helperHtml.LoadHtmlAndSelectNodes(htmlDoc, "//td");
                }
                if (nodes == null || nodes.IsEmpty()) return null;

                var childNodes = nodes.FirstOrDefault()?.ChildNodes;
                if (childNodes?.IsEmpty() == true) return null;

                foreach (var image in HtmlAgilityEx.ExtractAllChildNodes(nodes.FirstOrDefault()?.ChildNodes)?
                    .Where(f => f.Name.EqualsIgnoreCase("img")))
                {
                    var metaData = new MetaDataModel(_counter++, "Image", MetadataType.Image);
                    image.Attributes?.Where(s => s.Name.HasValue() && s.Value.HasValue())
                               .Iter(s =>
                               {
                                   var val = s.Value;
                                   if (s.Name.EqualsIgnoreCase("src") && val.StartsWith("//"))
                                   {
                                       val = $"https:{val}";
                                   }
                                   metaData.CustomMetadata.AddOrUpdate(s.Name, val);
                               });

                    if (metaData.CustomMetadata.ContainsKey("height") && metaData.CustomMetadata["height"].HasValue())
                    {
                        if (metaData.CustomMetadata["height"].ToInteger() < 12)
                        {
                            continue;
                        }
                    }

                    metaDatas.Add(metaData);
                }
                return metaDatas;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private MetaDataModel? ExtractInfoboxHeader(HtmlDocument htmlDoc)
        {
            try
            {
                var nodes = helperHtml.LoadHtmlAndSelectNodes(htmlDoc, "//th[contains(@class, 'infobox-header')]");
                if (nodes == null || nodes.IsEmpty()) return null;

                var description = nodes.FirstOrDefault()?.DecodedInnerText(removeNewLine: true) ?? string.Empty;
                if (description.IsEmpty()) return null;

                return new MetaDataModel(_counter++, "Header", MetadataType.GroupHeader)
                {
                    Description = description
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        private MetaDataModel? ExtractInfoboxLabel(HtmlDocument htmlDoc)//, int currentCount)
        {
            try
            {
                var nodes = helperHtml.LoadHtmlAndSelectNodes(htmlDoc, "//th[contains(@class, 'infobox-label')]");
                if (nodes == null || nodes.IsEmpty()) return null;

                var nextSiblingChildNodes = nodes.FirstOrDefault()?.NextSibling?.ChildNodes;
                if (nextSiblingChildNodes == null || nextSiblingChildNodes.IsEmpty()) return null;

                var metaData = new MetaDataModel(_counter++, "", MetadataType.Detail)
                {
                    Name = nodes.FirstOrDefault()?.DecodedInnerText(removeNewLine: true).Trim() ?? ""
                };

                StringBuilder content = new();

                foreach (var cNode in nextSiblingChildNodes)
                {
                    if (cNode.Name.EqualsIgnoreCase("br"))
                    {
                        content.Append(" ");
                        //content.AppendLine();
                    }
                    else if (cNode.Name != "style")
                    {
                        if (cNode.DecodedInnerText(removeNewLine: true).HasValue())
                        {
                            string appendSpace = "";
                            if (content.ToString().HasValue() &&
                                !(content.ToString().EndsWith(" ") ||
                                    cNode.DecodedInnerText(removeNewLine: true).StartsWith(" ") ||
                                    cNode.DecodedInnerText(removeNewLine: true).StartsWith(",") ||
                                    content.ToString().EndsWith(Environment.NewLine)))
                            {
                                appendSpace = " ";
                            }

                            var liItems = helperHtml.LoadHtmlAndSelectNodes(cNode.InnerHtml, "//li");
                            List<string> listOfWords = new List<string>();
                            if (liItems != null)
                            {
                                foreach (var item in liItems)
                                {
                                    var ite = HtmlAgilityEx.ExtractAllChildNodes(item.ChildNodes);
                                    if (ite != null && ite.Any(f => f.Name.EqualsIgnoreCase("ul") || f.Name.EqualsIgnoreCase("li")) == false)
                                    {
                                        listOfWords.Add(HtmlAgilityEx.DecodedInnerText(item.InnerText, false).Trim());
                                    }
                                }
                            }
                            var listItems = helperHtml.LoadHtmlAndSelectNodes(cNode.InnerHtml, "//li")?.Select(f => HtmlAgilityEx.DecodedInnerText(f.InnerText, false).Trim())?.ToList() ?? new List<string>();
                            if (listItems.Count > 0 && listItems.Any(f => f.HasValue()))
                            {
                                var rawContent = cNode.DecodedInnerText(removeNewLine: true);
                                foreach (var listItem in listItems.Where(f => f.HasValue()))
                                {
                                    rawContent = rawContent.Replace(listItem, "").Trim();
                                }
                                if (rawContent.HasValue() && listOfWords.Count > 0)
                                {
                                    rawContent += ", ";
                                }
                                rawContent += string.Join(", ", listOfWords).Trim();
                                content.Append($"{appendSpace}{rawContent}");
                            }
                            else
                            {
                                content.Append($"{appendSpace}{cNode.DecodedInnerText(removeNewLine: true)}");
                            }
                        }
                    }
                }
                metaData.Description = content.ToString().Trim();
                return metaData;
            }
            catch (Exception)
            {
                //throw;
                return null;
            }
        }
    }
}
