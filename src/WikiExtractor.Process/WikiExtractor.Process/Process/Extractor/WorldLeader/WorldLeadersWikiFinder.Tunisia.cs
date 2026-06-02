using HtmlAgilityPack;
using Pj.Library;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using WikiExtractor.Exts;
using WikiExtractor.Models;

namespace WikiExtractor.Process.Extractor
{
    public partial class WorldLeadersWikiFinder
    {
        public List<WikiWhatToExtractModel> ExtractListTabularData_Tunisia(HtmlDocument document, List<string>? tags)
        {
            List<WikiWhatToExtractModel> listOfNames = new List<WikiWhatToExtractModel>();
            sequence = 1;
            var tableData = document.DocumentNode.SelectNodes("//table[contains(@class, 'wikitable')]");
            foreach (var table in tableData)
            {
                var rows = table.SelectNodes(".//tr");
                foreach (var row in rows)
                {
                    var cells = row.SelectNodes(".//td")?.ToArray();
                    if (cells != null && cells.Length >= 3)
                    {
                        var r = ExtractListTabularData_Tunisia_Rows(cells);
                        if (r != null) { r.Tags = tags.DeepClone(); listOfNames.Add(r); }
                    }
                }
            }
            return listOfNames;
        }
        private WikiWhatToExtractModel? ExtractListTabularData_Tunisia_Rows(HtmlNode[] elements)
        {
            var m = new WikiWhatToExtractModel();
            m.AdditionalMetaData!.Add("Country", "Tunisia");
            int c = 1;
            foreach (var elm in elements)
            {
                if (c == 1) Common_Portrait_Extract(elm, m);
                if (c == 2) Common_PersonDetail_Extract(elm, m, titleRemoveInnerSpan: false, extractBirthDeath: true);
                if (c == 3)
                {
                    var term = elm.DecodedInnerText(removeNewLine: true).SplitAndTrim("–");
                    if (term.Count() >= 2)
                    {
                        m.AdditionalMetaData!.AddOrUpdate("Took office", term.First().Trim());
                        m.AdditionalMetaData!.AddOrUpdate("Left office", term.Skip(1).First().Trim());
                    }
                    else
                    {
                        m.AdditionalMetaData!.AddOrUpdate("Took office", elm.DecodedInnerText(removeNewLine: true).Trim());
                        m.AdditionalMetaData!.AddOrUpdate("Left office", "Incumbent");
                    }
                }
                if (c == 5) Common_SimpleDataType01_Extract(elm, m, "Political Party", removeSpecialChars: false);
                c++;
            }
            if (m.Title.IsEmpty()) return null;
            Console.WriteLine($"Extraction: {m.Title} [{m.Route}]");
            if (!ValidateAdditionalMetaData(m.AdditionalMetaData, "Took office")) return null;
            if (!ValidateAdditionalMetaData(m.AdditionalMetaData, "Left office")) return null;
            m.Sequence = sequence++; return m;
        }
    }
}
