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
        public List<WikiWhatToExtractModel> ExtractListTabularData_UnitedKingdom(HtmlDocument document, List<string>? tags)
        {
            List<WikiWhatToExtractModel> listOfNames = new List<WikiWhatToExtractModel>();
            sequence = 1;

            var tableData = document.DocumentNode.SelectNodes($"//table[contains(@class, 'wikitable')]");

            foreach (var table in tableData.Take(1))
            {
                var rows = table.SelectNodes(".//tr");

                foreach (var row in rows)
                {
                    var cells = row.SelectNodes(".//td")?.ToArray();
                    if (cells != null)
                    {
                        if (cells.Count() < 6)
                        {
                            continue;
                        }
                        var extractedData = ExtractListTabularData_UnitedKingdom_Rows(cells);
                        if (extractedData != null)
                        {
                            extractedData.Tags = tags.DeepClone();
                            listOfNames.Add(extractedData);
                        }
                    }
                }
            }
            return listOfNames;
        }
        private WikiWhatToExtractModel? ExtractListTabularData_UnitedKingdom_Rows(HtmlNode[] elements)
        {
            var listOfName = new WikiWhatToExtractModel();
            listOfName.AdditionalMetaData!.Add("Country", "United Kingdom");

            int tcolCounter = 1;

            foreach (var elm in elements)
            {
                if (tcolCounter == 1 || tcolCounter == 2)
                {
                    Common_Portrait_Extract(elm, listOfName);
                }
                if (tcolCounter == 2 || tcolCounter == 3)
                {
                    Common_PersonDetail_Extract(elm, listOfName, titleRemoveInnerSpan: false, extractBirthDeath: true);
                }
                if (tcolCounter == 3 || tcolCounter == 4)
                {
                    Common_DateType01_Extract(elm, listOfName, "Took office", null, removeSpecialChars: false);
                }
                if (tcolCounter == 4 || tcolCounter == 5)
                {
                    Common_DateType01_Extract(elm, listOfName, "Left office", new[] { "incumbent" }, removeSpecialChars: false);
                }
                if (tcolCounter == 5 || tcolCounter == 6)
                {
                    Common_SimpleDataType01_Extract(elm, listOfName, "Duration", removeSpecialChars: false);
                }
                tcolCounter++;
            }

            if (listOfName.Title.IsEmpty()) return null;
            Console.WriteLine($"Extraction: {listOfName.Title} [{listOfName.Route}]");
            foreach (var item in listOfName.AdditionalMetaData)
            {
                Console.WriteLine($"Details -> {item.Key}: {item.Value}");
            }

            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine("");

            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Birth-Death");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Took office");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Left office");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Duration");

            listOfName.Sequence = sequence++;
            return listOfName;
        }
    }
}
