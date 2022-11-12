using HtmlAgilityPack;
using Pj.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WikiExtractor.Exts;

namespace WikiExtractor.Process
{
    public class TabularInformationExtractor
    {
        public Dictionary<string, string> ExtractTabularData(HtmlDocument document)
        {
            Dictionary<string, string> listOfNames = new Dictionary<string, string>();
            var temp = document.DocumentNode.SelectNodes("//table[contains(@class, 'wikitable ')]/tbody/tr");
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
                                listOfNames.AddOrUpdate(
                                    anchor.Attributes["title"].Value,
                                    HttpUtility.UrlDecode(
                                        HtmlAgilityEx.DecodedInnerText(content: anchor.Attributes["href"].Value, removeNewLine: false)));
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
