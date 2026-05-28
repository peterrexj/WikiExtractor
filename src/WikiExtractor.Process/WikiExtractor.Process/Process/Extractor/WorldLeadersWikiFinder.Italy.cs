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
        public List<WikiWhatToExtractModel> ExtractListTabularData_Italy(HtmlDocument document, List<string>? tags)
        {
            List<WikiWhatToExtractModel> listOfNames = new List<WikiWhatToExtractModel>();
            sequence = 1;
            var tableData = document.DocumentNode.SelectNodes("//table[contains(@class, 'wikitable')]");
            foreach (var table in tableData)
            {
                var rows = table.SelectNodes(".//tr");
                foreach (var row in rows)
                {
                    // portrait is <th>, name is <th scope=row> — include all th and td, skip all-th rows
                    var cells = row.SelectNodes(".//td|.//th")?.ToArray();
                    if (cells == null) continue;
                    if (cells.All(f => f.Name == "th")) continue;
                    if (cells.Length >= 6)
                    {
                        var r = ExtractListTabularData_Italy_Rows(cells);
                        if (r != null) { r.Tags = tags.DeepClone(); listOfNames.Add(r); }
                    }
                }
            }
            return listOfNames;
        }
        private WikiWhatToExtractModel? ExtractListTabularData_Italy_Rows(HtmlNode[] elements)
        {
            var m = new WikiWhatToExtractModel();
            m.AdditionalMetaData!.Add("Country", "Italy");
            int c = 1;
            foreach (var elm in elements)
            {
                // th1=portrait, th2(scope=row)=name+lifespan, td1=took, td2=left, td3=tenure, ..., td5=party
                if (c == 1) Common_Portrait_Extract(elm, m);
                if (c == 2) Common_PersonDetail_Extract(elm, m, titleRemoveInnerSpan: false, extractBirthDeath: true);
                if (c == 3) Common_DateType01_Extract(elm, m, "Took office", null, removeSpecialChars: true);
                if (c == 4) Common_DateType01_Extract(elm, m, "Left office", new[] { "Incumbent" }, removeSpecialChars: true);
                if (c == 5) Common_SimpleDataType01_Extract(elm, m, "Time in office", removeSpecialChars: false);
                if (c == 7) Common_SimpleDataType01_Extract(elm, m, "Political Party", removeSpecialChars: false);
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
