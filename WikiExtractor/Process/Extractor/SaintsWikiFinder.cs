using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WikiExtractor.Models;
using Pj.Library;
using WikiExtractor.Exts;

namespace WikiExtractor.Process.Extractor
{
    public class SaintsWikiFinder
    {
        public List<WikiWhatToExtractModel> SaintsExtractListTabularData(HtmlDocument document, List<string>? tags)
        {
            List<WikiWhatToExtractModel> listOfNames = new List<WikiWhatToExtractModel>();
            var temp = document.DocumentNode.SelectNodes("//table[contains(@class, 'wikitable ')]/tbody/tr");
            int counter = 0;
            bool hasExtracted = false;
            foreach (var item in temp)
            {
                hasExtracted = false;
                if (item.ChildNodes.Any(f => f.Name == "td"))
                {
                    var cell = item.ChildNodes.FirstOrDefault(f => f.Name == "td");
                    if (cell.ChildNodes.Any(f => f.Name == "a"))
                    {
                        var anchor = cell.ChildNodes.FirstOrDefault(f => f.Name == "a");
                        if (anchor != null && anchor.Attributes.Count > 0)
                        {
                            if (anchor.Attributes.Any(a => a.Name == "href" && a.Value.HasValue()) &&
                               anchor.Attributes.Any(a => a.Name == "title" && a.Value.HasValue()))
                            {
                                var route = HttpUtility.UrlDecode(HtmlAgilityEx.DecodedInnerText(content: anchor.Attributes["href"].Value, removeNewLine: false));
                                var title = HtmlAgilityEx.DecodedInnerText(anchor.Attributes["title"].Value, false);
                                if (!listOfNames.Any(f => f.Route == route))
                                {
                                    listOfNames.Add(new WikiWhatToExtractModel { Route = route, Title = title, Tags = tags, Sequence = ++counter });
                                }
                                hasExtracted = true;
                            }
                        }
                    }
                }
            }
            return listOfNames;
        }

        public List<WikiWhatToExtractModel> SaintsExtractPatronSaintsListData(HtmlDocument document, List<string>? tags)
        {
            List<WikiWhatToExtractModel> listOfNames = new List<WikiWhatToExtractModel>();
            var temp = document.DocumentNode.SelectNodes("//li");
            int counter = 0;
            foreach (var item in temp)
            {
                if (item.InnerText.EqualsIgnoreCase("Saints portal"))
                {
                    break;
                }
                else if (item.InnerText.RegexMatching("(\\S+) - (\\S+)"))
                {
                    var splits = string.Join("", item.InnerHtml.SplitAndTrim("-").Skip(1));
                    var tempDoc = new HtmlDocument();
                    tempDoc.LoadHtml(splits);
                    var anchors = tempDoc.DocumentNode.SelectNodes("//a");
                    foreach (var anchor in anchors)
                    {
                        if (anchor != null && anchor.Attributes.Count > 0)
                        {
                            if (anchor.Attributes.Any(a => a.Name == "href" && a.Value.HasValue()) &&
                               anchor.Attributes.Any(a => a.Name == "title" && a.Value.HasValue()))
                            {
                                var route = HttpUtility.UrlDecode(HtmlAgilityEx.DecodedInnerText(content: anchor.Attributes["href"].Value, removeNewLine: false));
                                var title = HtmlAgilityEx.DecodedInnerText(anchor.Attributes["title"].Value, false);
                                if (!listOfNames.Any(f => f.Route == route))
                                {
                                    listOfNames.Add(new WikiWhatToExtractModel { Route = route, Title = title, Tags = tags, Sequence = ++counter });
                                }
                            }
                        }
                    }
                }
            }
            return listOfNames;
        }

        public List<WikiWhatToExtractModel> SaintsExtractByAllPopeListData(HtmlDocument document, List<string>? tags)
        {
            List<WikiWhatToExtractModel> listOfNames = new List<WikiWhatToExtractModel>();
            var temp = document.DocumentNode.SelectNodes("//table[contains(@class, 'wikitable')]//tr//td/a");
            int counter = 0;
            foreach (var item in temp)
            {
                if (item.Name.EqualsIgnoreCase("a"))
                {
                    if (item.Attributes.Count > 0)
                    {
                        if (item.Attributes.Any(a => a.Name == "href" && a.Value.HasValue()) &&
                           item.Attributes.Any(a => a.Name == "title" && a.Value.HasValue()))
                        {
                            var route = HttpUtility.UrlDecode(HtmlAgilityEx.DecodedInnerText(content: item.Attributes["href"].Value, removeNewLine: false));
                            var title = HtmlAgilityEx.DecodedInnerText(item.Attributes["title"].Value, false);
                            if (!listOfNames.Any(f => f.Route == route))
                            {
                                listOfNames.Add(new WikiWhatToExtractModel { Route = route, Title = title, Tags = tags, Sequence = ++counter });
                            }
                        }
                    }
                }
            }
            return listOfNames;
        }

        public List<WikiWhatToExtractModel> SaintsExtractByEachPopeListData(HtmlDocument document, List<string>? tags)
        {
            List<WikiWhatToExtractModel> listOfNames = new List<WikiWhatToExtractModel>();
            var temp = document.DocumentNode.SelectNodes("//table[contains(@class, 'wikitable')]//tr//td[2]/a");
            int counter = 0;
            foreach (var item in temp)
            {
                if (item.Name.EqualsIgnoreCase("a"))
                {
                    if (item.Attributes.Count > 0)
                    {
                        if (item.Attributes.Any(a => a.Name == "href" && a.Value.HasValue()) &&
                           item.Attributes.Any(a => a.Name == "title" && a.Value.HasValue()))
                        {
                            var route = HttpUtility.UrlDecode(HtmlAgilityEx.DecodedInnerText(content: item.Attributes["href"].Value, removeNewLine: false));
                            var title = HtmlAgilityEx.DecodedInnerText(item.Attributes["title"].Value, false);
                            if (!listOfNames.Any(f => f.Route == route))
                            {
                                listOfNames.Add(new WikiWhatToExtractModel { Route = route, Title = title, Tags = tags, Sequence = ++counter });
                            }
                        }
                    }
                }
            }
            return listOfNames;
        }

        public List<WikiWhatToExtractModel> SaintsExtractByCentury(HtmlDocument document, List<string>? tags)
        {
            List<WikiWhatToExtractModel> listOfNames = new List<WikiWhatToExtractModel>();
            var temp = document.DocumentNode.SelectNodes("//table[contains(@class, 'wikitable')]//tr//td[1]/a");
            int counter = 0;
            foreach (var item in temp)
            {
                if (item.Name.EqualsIgnoreCase("a"))
                {
                    if (item.Attributes.Count > 0)
                    {
                        if (item.Attributes.Any(a => a.Name == "href" && a.Value.HasValue()) &&
                           item.Attributes.Any(a => a.Name == "title" && a.Value.HasValue()))
                        {
                            var route = HttpUtility.UrlDecode(HtmlAgilityEx.DecodedInnerText(content: item.Attributes["href"].Value, removeNewLine: false));
                            var title = HtmlAgilityEx.DecodedInnerText(item.Attributes["title"].Value, false);
                            if (!listOfNames.Any(f => f.Route == route))
                            {
                                listOfNames.Add(new WikiWhatToExtractModel { Route = route, Title = title, Tags = tags, Sequence = ++counter });
                            }
                        }
                    }
                }
            }
            return listOfNames;
        }
    }
}
