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
        public List<WikiWhatToExtractModel> ExtractListTabularData_Lithuania(HtmlDocument document, List<string>? tags)
        {
            List<WikiWhatToExtractModel> listOfNames = new List<WikiWhatToExtractModel>();
            sequence = 1;
            var tableData = document.DocumentNode.SelectNodes("//table[contains(@class, 'wikitable')]");
            // Tables 0-14: medieval/occupation-era rulers; table 15: modern presidents (post-1990)
            foreach (var table in tableData.Take(15))
            {
                var rows = table.SelectNodes(".//tr");
                foreach (var row in rows)
                {
                    var cells = row.SelectNodes(".//td")?.ToArray();
                    if (cells != null && cells.Length >= 4)
                    {
                        var r = ExtractListTabularData_Lithuania_Rows(cells, modernEra: false);
                        if (r != null) { r.Tags = tags.DeepClone(); listOfNames.Add(r); }
                    }
                }
            }
            foreach (var table in tableData.Skip(15).Take(1))
            {
                var rows = table.SelectNodes(".//tr");
                foreach (var row in rows)
                {
                    var cells = row.SelectNodes(".//td")?.ToArray();
                    if (cells != null && cells.Length >= 5)
                    {
                        var r = ExtractListTabularData_Lithuania_Rows(cells, modernEra: true);
                        if (r != null) { r.Tags = tags.DeepClone(); listOfNames.Add(r); }
                    }
                }
            }
            return listOfNames;
        }
        private WikiWhatToExtractModel? ExtractListTabularData_Lithuania_Rows(HtmlNode[] elements, bool modernEra)
        {
            var m = new WikiWhatToExtractModel();
            m.AdditionalMetaData!.Add("Country", "Lithuania");
            int c = 1;
            // Medieval tables (4-5 tds): td1=name, td2=portrait, td3=birth/parentage
            // Modern table 16 (7 tds): ordinal=TH(skipped), td1=portrait, td2=name+lifespan, td3=elected, td4=took, td5=left, td6=party, td7=notes
            foreach (var elm in elements)
            {
                if (!modernEra)
                {
                    if (c == 2) Common_Portrait_Extract(elm, m);
                    if (c == 1) Common_PersonDetail_Extract(elm, m, titleRemoveInnerSpan: false, extractBirthDeath: false);
                    if (c == 3) Common_Complex_BirthDeath(elm, m);
                }
                else
                {
                    if (c == 1) Common_Portrait_Extract(elm, m);
                    if (c == 2) Common_PersonDetail_Extract(elm, m, titleRemoveInnerSpan: false, extractBirthDeath: true);
                    if (c == 4) Common_DateType01_Extract(elm, m, "Took office", null, removeSpecialChars: true);
                    if (c == 5) Common_DateType01_Extract(elm, m, "Left office", new[] { "Incumbent" }, removeSpecialChars: true);
                    if (c == 6) Common_SimpleDataType01_Extract(elm, m, "Political Party", removeSpecialChars: false);
                }
                c++;
            }
            if (m.Title.IsEmpty()) return null;
            Console.WriteLine($"Extraction: {m.Title} [{m.Route}]");
            if (modernEra)
            {
                ValidateAdditionalMetaData(m.AdditionalMetaData, "Birth-Death");
                ValidateAdditionalMetaData(m.AdditionalMetaData, "Took office");
                ValidateAdditionalMetaData(m.AdditionalMetaData, "Left office");
            }
            m.Sequence = sequence++; return m;
        }
    }
}
