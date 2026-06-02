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
        public List<WikiWhatToExtractModel> ExtractListTabularData_Uganda(HtmlDocument document, List<string>? tags)
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
                        var r = ExtractListTabularData_Uganda_Rows(cells);
                        if (r != null) { r.Tags = tags.DeepClone(); listOfNames.Add(r); }
                    }
                }
            }
            return listOfNames;
        }
        private WikiWhatToExtractModel? ExtractListTabularData_Uganda_Rows(HtmlNode[] elements)
        {
            var m = new WikiWhatToExtractModel();
            m.AdditionalMetaData!.Add("Country", "Uganda");
            int c = 1;
            // td1=ordinal, td2=portrait, td3=name, td4=took(8TDs) or election(9TDs), td5=left(8TDs) or took(9TDs)
            bool withElection = elements.Length >= 9;
            foreach (var elm in elements)
            {
                if (c == 2) Common_Portrait_Extract(elm, m);
                if (c == 3) Common_PersonDetail_Extract(elm, m, titleRemoveInnerSpan: false, extractBirthDeath: true);
                if (!withElection && c == 4) Common_DateType01_Extract(elm, m, "Took office", null, removeSpecialChars: true);
                if (!withElection && c == 5) Common_DateType01_Extract(elm, m, "Left office", new[] { "Incumbent" }, removeSpecialChars: true);
                if (!withElection && c == 6) Common_SimpleDataType01_Extract(elm, m, "Time in office", removeSpecialChars: false);
                if (!withElection && c == 7) Common_SimpleDataType01_Extract(elm, m, "Political Party", removeSpecialChars: false);
                if (withElection && c == 5) Common_DateType01_Extract(elm, m, "Took office", null, removeSpecialChars: true);
                if (withElection && c == 6) Common_DateType01_Extract(elm, m, "Left office", new[] { "Incumbent" }, removeSpecialChars: true);
                if (withElection && c == 7) Common_SimpleDataType01_Extract(elm, m, "Time in office", removeSpecialChars: false);
                if (withElection && c == 8) Common_SimpleDataType01_Extract(elm, m, "Political Party", removeSpecialChars: false);
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
