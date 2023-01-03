using HtmlAgilityPack;
using Pj.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WikiExtractor.Exts;
using WikiExtractor.Models;

namespace WikiExtractor.Process.Extractor
{
    public class PopesWikiFinder
    {
        public List<WikiWhatToExtractModel> ExtractByCenturyFromTable(HtmlDocument document, List<string>? tags)
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
    }
}
