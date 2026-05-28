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
        public List<WikiWhatToExtractModel> ExtractListTabularData_CostaRica(HtmlDocument document, List<string>? tags)
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
                        var r = ExtractListTabularData_CostaRica_Rows(cells);
                        if (r != null) { r.Tags = tags.DeepClone(); listOfNames.Add(r); }
                    }
                }
            }
            return listOfNames;
        }
        private WikiWhatToExtractModel? ExtractListTabularData_CostaRica_Rows(HtmlNode[] elements)
        {
            var m = new WikiWhatToExtractModel();
            m.AdditionalMetaData!.Add("Country", "Costa Rica");
            int c = 1;
            // Table 0 (6 TDs): c1=name, c2=portrait, c3=combined "YYYY–YYYY", c4=party
            // Table 1 (8 TDs): c1=name, c2=portrait, c3=took, c4=left, c5=tenure, c6=color, c7=party
            bool separateDates = elements.Length >= 8;
            foreach (var elm in elements)
            {
                if (c == 2) Common_Portrait_Extract(elm, m);
                if (c == 1) Common_PersonDetail_Extract(elm, m, titleRemoveInnerSpan: false, extractBirthDeath: true);
                if (!separateDates && c == 3)
                {
                    var term = elm.DecodedInnerText(removeNewLine: true).SplitAndTrim("–");
                    if (term.Count() >= 2)
                    {
                        m.AdditionalMetaData!.AddOrUpdate("Took office", term.First().Trim());
                        m.AdditionalMetaData!.AddOrUpdate("Left office", term.Skip(1).First().Trim());
                    }
                }
                if (!separateDates && c == 4) Common_SimpleDataType01_Extract(elm, m, "Political Party", removeSpecialChars: false);
                if (separateDates && c == 3) Common_DateType01_Extract(elm, m, "Took office", null, removeSpecialChars: true);
                if (separateDates && c == 4) Common_DateType01_Extract(elm, m, "Left office", new[] { "Incumbent" }, removeSpecialChars: true);
                if (separateDates && c == 5) Common_SimpleDataType01_Extract(elm, m, "Time in office", removeSpecialChars: false);
                if (separateDates && c == 7) Common_SimpleDataType01_Extract(elm, m, "Political Party", removeSpecialChars: false);
                c++;
            }
            if (m.Title.IsEmpty()) return null;
            Console.WriteLine($"Extraction: {m.Title} [{m.Route}]");
            ValidateAdditionalMetaData(m.AdditionalMetaData, "Birth-Death");
            ValidateAdditionalMetaData(m.AdditionalMetaData, "Took office");
            ValidateAdditionalMetaData(m.AdditionalMetaData, "Left office");
            m.Sequence = sequence++; return m;
        }
    }
}
