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
        public List<WikiWhatToExtractModel> ExtractListTabularData_Norway(HtmlDocument document, List<string>? tags)
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
                    if (cells != null && cells.Length >= 6)
                    {
                        var r = ExtractListTabularData_Norway_Rows(cells);
                        if (r != null) { r.Tags = tags.DeepClone(); listOfNames.Add(r); }
                    }
                }
            }
            return listOfNames;
        }
        private WikiWhatToExtractModel? ExtractListTabularData_Norway_Rows(HtmlNode[] elements)
        {
            var m = new WikiWhatToExtractModel();
            m.AdditionalMetaData!.Add("Country", "Norway");
            int c = 1;
            // Early-era (tables 1-2, 1814-1905): td1=name, td2=portrait (dual-person rows)
            // Modern-era (tables 3-5, 1905-present): td1=portrait, td2=name (ordinal is TH, skipped)
            bool earlyEra = elements[0].SelectSingleNode(".//a[@href]") != null;
            foreach (var elm in elements)
            {
                if (earlyEra)
                {
                    if (c == 1) Common_PersonDetail_Extract(elm, m, titleRemoveInnerSpan: false, extractBirthDeath: true);
                    if (c == 2) Common_Portrait_Extract(elm, m);
                    if (c == 3) Common_DateType01_Extract(elm, m, "Took office", null, removeSpecialChars: true);
                    if (c == 4) Common_DateType01_Extract(elm, m, "Left office", new[] { "Incumbent" }, removeSpecialChars: true);
                }
                else
                {
                    // td1=portrait, td2=name, td3=took, td4=left, td5=tenure, td6=party swatch(empty), td7=party
                    if (c == 1) Common_Portrait_Extract(elm, m);
                    if (c == 2) Common_PersonDetail_Extract(elm, m, titleRemoveInnerSpan: false, extractBirthDeath: true);
                    if (c == 3) Common_DateType01_Extract(elm, m, "Took office", null, removeSpecialChars: true);
                    if (c == 4) Common_DateType01_Extract(elm, m, "Left office", new[] { "Incumbent" }, removeSpecialChars: true);
                    if (c == 5) Common_SimpleDataType01_Extract(elm, m, "Time in office", removeSpecialChars: false);
                    if (c == 7) Common_SimpleDataType01_Extract(elm, m, "Political Party", removeSpecialChars: false);
                }
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
