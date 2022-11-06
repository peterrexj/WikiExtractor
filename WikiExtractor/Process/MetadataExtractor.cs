using HtmlAgilityPack;
using Pj.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WikiExtractor.Exts;
using WikiExtractor.Models;

namespace WikiExtractor.Process
{
    public class MetadataExtractor
    {
        private readonly HelperHtml helperHtml = new();
        private int _counter = 0;

        public List<MetaDataModel> ExtractMetadataInfo(HtmlDocument document)
        {
            _counter = 1;
            var tableRows = document.DocumentNode.SelectNodes("//table[contains(@class, 'infobox vcard')]/tbody/tr");
            var metaDataDict = new List<MetaDataModel>();

            foreach (var tableRow in tableRows)
            {
                var loadedInnerHtml = helperHtml.LoadHtmlDocument(tableRow.InnerHtml);

                var infoboxAbove = ExtractInfoboxAbove(loadedInnerHtml);
                if (infoboxAbove != null) metaDataDict.Add(infoboxAbove);

                var infoboxImage = ExtractInfoboxImage(loadedInnerHtml);
                if (infoboxImage != null) metaDataDict.Add(infoboxImage);

                var infoboxHeader = ExtractInfoboxHeader(loadedInnerHtml);
                if (infoboxHeader != null) metaDataDict.Add(infoboxHeader);

                var infoboxLabel = ExtractInfoboxLabel(loadedInnerHtml); //, metaDataDict.Count);
                if (infoboxLabel != null) metaDataDict.Add(infoboxLabel);
            }

            return metaDataDict;
        }

        private MetaDataModel? ExtractInfoboxAbove(HtmlDocument htmlDoc)
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

        private MetaDataModel? ExtractInfoboxImage(HtmlDocument htmlDoc)
        {
            var nodes = helperHtml.LoadHtmlAndSelectNodes(htmlDoc, "//td[contains(@class, 'infobox-image')]");
            if (nodes == null || nodes.IsEmpty()) return null;

            var childNodes = nodes.FirstOrDefault()?.ChildNodes;
            if (childNodes?.IsEmpty() == true) return null;

            var metaData = new MetaDataModel(_counter++, "Image", MetadataType.Image);
            foreach (var cNode in childNodes!)
            {
                if (cNode.Name.EqualsIgnoreCase("a"))
                {
                    var img = cNode.ChildNodes?.Where(c => c.Name.EqualsIgnoreCase("img"))?.FirstOrDefault() ?? null;
                    if (img != null)
                    {
                        img.Attributes?.Where(s => s.Name.HasValue() && s.Value.HasValue())
                            .Iter(s => metaData.CustomMetadata.AddOrUpdate(s.Name, s.Value));
                    }
                }
            }
            return metaData;
        }

        private MetaDataModel? ExtractInfoboxHeader(HtmlDocument htmlDoc)
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

        private MetaDataModel? ExtractInfoboxLabel(HtmlDocument htmlDoc)//, int currentCount)
        {
            var nodes = helperHtml.LoadHtmlAndSelectNodes(htmlDoc, "//th[contains(@class, 'infobox-label')]");
            if (nodes == null || nodes.IsEmpty()) return null;

            var nextSiblingChildNodes = nodes.FirstOrDefault()?.NextSibling?.ChildNodes;
            if (nextSiblingChildNodes == null || nextSiblingChildNodes.IsEmpty()) return null;

            var metaData = new MetaDataModel(_counter++, "", MetadataType.Detail)
            {
                Name = nodes.FirstOrDefault()?.DecodedInnerText(removeNewLine: true) ?? ""
            };

            StringBuilder content = new();

            foreach (var cNode in nextSiblingChildNodes)
            {
                if (cNode.Name.EqualsIgnoreCase("br"))
                {
                    content.Append(" ");
                    //content.AppendLine();
                }
                else
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
                        content.Append($"{appendSpace}{cNode.DecodedInnerText(removeNewLine: true)}");
                    }
                }
            }
            metaData.Description = content.ToString();
            return metaData;
        }
    }
}
