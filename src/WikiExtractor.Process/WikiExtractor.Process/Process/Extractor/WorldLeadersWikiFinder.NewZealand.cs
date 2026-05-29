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
        public List<WikiWhatToExtractModel> ExtractListTabularData_NewZealand(HtmlDocument document, List<string>? tags)
        {
            List<WikiWhatToExtractModel> listOfNames = new List<WikiWhatToExtractModel>();
            sequence = 1;

            var tableData = document.DocumentNode.SelectNodes($"//table[contains(@class, 'wikitable')]");

            foreach (var table in tableData)
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
                        var extractedData = ExtractListTabularData_NewZealand_Rows(cells);
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
        private WikiWhatToExtractModel? ExtractListTabularData_NewZealand_Rows(HtmlNode[] elements)
        {
            var listOfName = new WikiWhatToExtractModel();
            listOfName.AdditionalMetaData!.Add("Country", "New Zealand");

            int tColCounter = 1;

            foreach (var elm in elements)
            {
                if (tColCounter == 1)
                {
                    Common_Portrait_Extract(elm, listOfName);
                }
                if (tColCounter == 2)
                {
                    Common_PersonDetail_Extract(elm, listOfName, titleRemoveInnerSpan: true, extractBirthDeath: true);
                }
                if (tColCounter == 4)
                {
                    Common_DateType01_Extract(elm, listOfName, "Took office", null, removeSpecialChars: true);
                }
                if (tColCounter == 5)
                {
                    Common_DateType01_Extract(elm, listOfName, "Left office", new[] { "Incumbent" }, removeSpecialChars: true);
                }
                if (tColCounter == 6)
                {
                    Common_SimpleDataType01_Extract(elm, listOfName, "Time in office", removeSpecialChars: false);
                }
                if (tColCounter == 7)
                {
                    Common_SimpleDataType01_Extract(elm, listOfName, "Political Party", removeSpecialChars: false);
                }
                tColCounter++;
            }

            if (listOfName.Title.IsEmpty()) return null;

            Console.WriteLine($"Extraction: {listOfName.Title} [{listOfName.Route}]");
            foreach (var item in listOfName.AdditionalMetaData)
            {
                Console.WriteLine($"Details -> {item.Key}: {item.Value}");
            }

            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine("");

            if (!ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Birth-Death")) return null;
            if (!ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Took office")) return null;
            if (!ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Left office")) return null;
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Time in office");
            //ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Political Party");

            listOfName.Sequence = sequence++;
            return listOfName;
        }
    }
}
