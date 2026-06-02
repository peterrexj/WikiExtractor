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
        public List<WikiWhatToExtractModel> ExtractListTabularData_Brazil(HtmlDocument document, List<string>? tags)
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
                        var r = ExtractListTabularData_Brazil_Rows(cells);
                        if (r != null) { r.Tags = tags.DeepClone(); listOfNames.Add(r); }
                    }
                }
            }
            return listOfNames;
        }
        private WikiWhatToExtractModel? ExtractListTabularData_Brazil_Rows(HtmlNode[] elements)
        {
            var m = new WikiWhatToExtractModel();
            m.AdditionalMetaData!.Add("Country", "Brazil");
            int c = 1;
            // 9-TD rows (tables 1,4,5,6 — ordinal=TH, c1=color_swatch, c2=portrait, c3=name, c4=elected, c5=took, c6=left, c7=tenure, c8=party)
            // 8-TD rows (tables 2,3 — ordinal=TH, c1=portrait, c2=name, c3=elected, c4=took, c5=left, c6=tenure, c7=party)
            bool withSwatch = elements.Length >= 9;
            foreach (var elm in elements)
            {
                if (withSwatch && c == 2) Common_Portrait_Extract(elm, m);
                if (withSwatch && c == 3) Common_PersonDetail_Extract(elm, m, titleRemoveInnerSpan: false, extractBirthDeath: true);
                if (withSwatch && c == 5) Common_DateType01_Extract(elm, m, "Took office", null, removeSpecialChars: true);
                if (withSwatch && c == 6) Common_DateType01_Extract(elm, m, "Left office", new[] { "Incumbent" }, removeSpecialChars: true);
                if (withSwatch && c == 7) Common_SimpleDataType01_Extract(elm, m, "Time in office", removeSpecialChars: false);
                if (withSwatch && c == 8) Common_SimpleDataType01_Extract(elm, m, "Political Party", removeSpecialChars: false);
                if (!withSwatch && c == 1) Common_Portrait_Extract(elm, m);
                if (!withSwatch && c == 2) Common_PersonDetail_Extract(elm, m, titleRemoveInnerSpan: false, extractBirthDeath: true);
                if (!withSwatch && c == 4) Common_DateType01_Extract(elm, m, "Took office", null, removeSpecialChars: true);
                if (!withSwatch && c == 5) Common_DateType01_Extract(elm, m, "Left office", new[] { "Incumbent" }, removeSpecialChars: true);
                if (!withSwatch && c == 6) Common_SimpleDataType01_Extract(elm, m, "Time in office", removeSpecialChars: false);
                if (!withSwatch && c == 7) Common_SimpleDataType01_Extract(elm, m, "Political Party", removeSpecialChars: false);
                c++;
            }
            if (m.Title.IsEmpty()) return null;
            Console.WriteLine($"Extraction: {m.Title} [{m.Route}]");
            if (!ValidateAdditionalMetaData(m.AdditionalMetaData, "Birth-Death")) return null;
            if (!ValidateAdditionalMetaData(m.AdditionalMetaData, "Took office")) return null;
            if (!ValidateAdditionalMetaData(m.AdditionalMetaData, "Left office")) return null;
            m.Sequence = sequence++; return m;
        }
    }
}
