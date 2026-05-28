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
        public List<WikiWhatToExtractModel> ExtractListTabularData_Slovenia(HtmlDocument document, List<string>? tags)
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
                    if (cells != null && cells.Length >= 5)
                    {
                        var r = ExtractListTabularData_Slovenia_Rows(cells);
                        if (r != null) { r.Tags = tags.DeepClone(); listOfNames.Add(r); }
                    }
                }
            }
            return listOfNames;
        }
        private WikiWhatToExtractModel? ExtractListTabularData_Slovenia_Rows(HtmlNode[] elements)
        {
            var m = new WikiWhatToExtractModel();
            m.AdditionalMetaData!.Add("Country", "Slovenia");
            int c = 1;
            // Table 1 (SFRY era, 5 tds): ordinal=TH(skipped), td1=name+lifespan, td2=portrait, td3=took, td4=left, td5=party
            // Table 2 (modern 1991+, 8 tds): td1=ordinal, td2=portrait, td3=name+lifespan, td4=took, td5=left, td6=tenure, td7=party, td8=election
            bool modernEra = elements.Length >= 8;
            foreach (var elm in elements)
            {
                if (!modernEra)
                {
                    if (c == 1) Common_PersonDetail_Extract(elm, m, titleRemoveInnerSpan: false, extractBirthDeath: true);
                    if (c == 2) Common_Portrait_Extract(elm, m);
                    if (c == 3) Common_DateType01_Extract(elm, m, "Took office", null, removeSpecialChars: true);
                    if (c == 4) Common_DateType01_Extract(elm, m, "Left office", new[] { "Incumbent" }, removeSpecialChars: true);
                    if (c == 5) Common_SimpleDataType01_Extract(elm, m, "Political Party", removeSpecialChars: false);
                }
                else
                {
                    if (c == 2) Common_Portrait_Extract(elm, m);
                    if (c == 3) Common_PersonDetail_Extract(elm, m, titleRemoveInnerSpan: false, extractBirthDeath: true);
                    if (c == 4) Common_DateType01_Extract(elm, m, "Took office", null, removeSpecialChars: true);
                    if (c == 5) Common_DateType01_Extract(elm, m, "Left office", new[] { "Incumbent" }, removeSpecialChars: true);
                    if (c == 6) Common_SimpleDataType01_Extract(elm, m, "Time in office", removeSpecialChars: false);
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
