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
        public List<WikiWhatToExtractModel> ExtractListTabularData_Paraguay(HtmlDocument document, List<string>? tags)
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
                        var r = ExtractListTabularData_Paraguay_Rows(cells);
                        if (r != null) { r.Tags = tags.DeepClone(); listOfNames.Add(r); }
                    }
                }
            }
            return listOfNames;
        }
        private WikiWhatToExtractModel? ExtractListTabularData_Paraguay_Rows(HtmlNode[] elements)
        {
            var m = new WikiWhatToExtractModel();
            m.AdditionalMetaData!.Add("Country", "Paraguay");
            int c = 1;
            // Table 0 (7 TDs — ordinal=TD at c1): c1=ordinal, c2=portrait, c3=name, c4=took, c5=left, c6=tenure, c7=party
            // Table 1 (9 TDs — ordinal=TH, with Elected): c1=portrait, c2=empty, c3=name, c4=elected, c5=took, c6=left, c7=tenure, c8=party
            bool withElection = elements.Length >= 9;
            foreach (var elm in elements)
            {
                if (!withElection && c == 2) Common_Portrait_Extract(elm, m);
                if (!withElection && c == 3) Common_PersonDetail_Extract(elm, m, titleRemoveInnerSpan: false, extractBirthDeath: true);
                if (!withElection && c == 4) Common_DateType01_Extract(elm, m, "Took office", null, removeSpecialChars: true);
                if (!withElection && c == 5) Common_DateType01_Extract(elm, m, "Left office", new[] { "Incumbent" }, removeSpecialChars: true);
                if (!withElection && c == 6) Common_SimpleDataType01_Extract(elm, m, "Time in office", removeSpecialChars: false);
                if (!withElection && c == 7) Common_SimpleDataType01_Extract(elm, m, "Political Party", removeSpecialChars: false);
                if (withElection && c == 1) Common_Portrait_Extract(elm, m);
                if (withElection && c == 3) Common_PersonDetail_Extract(elm, m, titleRemoveInnerSpan: false, extractBirthDeath: true);
                if (withElection && c == 5) Common_DateType01_Extract(elm, m, "Took office", null, removeSpecialChars: true);
                if (withElection && c == 6) Common_DateType01_Extract(elm, m, "Left office", new[] { "Incumbent" }, removeSpecialChars: true);
                if (withElection && c == 7) Common_SimpleDataType01_Extract(elm, m, "Time in office", removeSpecialChars: false);
                if (withElection && c == 8) Common_SimpleDataType01_Extract(elm, m, "Political Party", removeSpecialChars: false);
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
