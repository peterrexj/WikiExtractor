using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.Exts;
using WikiExtractor.Models;

namespace WikiExtractor.Process.Extractor
{
    public class CountriesWikiFinder
    {
        public List<WikiWhatToExtractModel> ListByDependencyArea(HtmlDocument document, List<string>? tags)
        {
            List<WikiWhatToExtractModel> listOfNames = new List<WikiWhatToExtractModel>();
            int sequence = 1;

            var tableData = document.DocumentNode.SelectNodes($"//table[contains(@class, 'wikitable')]//tbody/tr");
            foreach (var tableRow in tableData)
            {
                if (tableRow.ChildNodes.Count(f => f.Name == "td") <= 5)
                {
                    continue;
                }
                var listOfName = new WikiWhatToExtractModel();
                var elements = tableRow.ChildNodes.Where(f => f.Name == "td").ToArray();

                var num = elements[0].DecodedInnerText(removeNewLine: true).Trim();

            }

            return listOfNames;
        }
    }
}
