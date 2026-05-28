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
        public List<WikiWhatToExtractModel> ExtractListTabularData_Poland(HtmlDocument document, List<string>? tags)
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
                        var r = ExtractListTabularData_Poland_Rows(cells);
                        if (r != null) { r.Tags = tags.DeepClone(); listOfNames.Add(r); }
                    }
                }
            }
            return listOfNames;
        }
        private WikiWhatToExtractModel? ExtractListTabularData_Poland_Rows(HtmlNode[] elements)
        {
            var m = new WikiWhatToExtractModel();
            m.AdditionalMetaData!.Add("Country", "Poland");
            int c = 1;
            foreach (var elm in elements)
            {
                // Table A (interwar, 7+ TDs): swatch=TH(skip), td1=portrait, td2=name, td3=took, td4=left, td5=tenure, td6=offices, td7=party
                // Table B (post-WWII, 6 TDs): index+swatch=TH(skip), td1=portrait, td2=name, td3=took, td4=left, td5=party, td6=cabinet
                bool tableB = elements.Length <= 6;
                if (c == 1) Common_Portrait_Extract(elm, m);
                if (c == 2) Common_PersonDetail_Extract(elm, m, titleRemoveInnerSpan: false, extractBirthDeath: true);
                if (c == 3) Common_DateType01_Extract(elm, m, "Took office", null, removeSpecialChars: true);
                if (c == 4) Common_DateType01_Extract(elm, m, "Left office", new[] { "Incumbent" }, removeSpecialChars: true);
                if (!tableB && c == 5) Common_SimpleDataType01_Extract(elm, m, "Time in office", removeSpecialChars: false);
                if (tableB && c == 5) Common_SimpleDataType01_Extract(elm, m, "Political Party", removeSpecialChars: false);
                if (!tableB && c == 7) Common_SimpleDataType01_Extract(elm, m, "Political Party", removeSpecialChars: false);
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
