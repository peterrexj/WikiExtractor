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
        public List<WikiWhatToExtractModel> ExtractListTabularData_UnitedStates(HtmlDocument document, List<string>? tags)
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
                        var extractedData = ExtractListTabularData_UnitedStates_Rows(cells);
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
        private WikiWhatToExtractModel? ExtractListTabularData_UnitedStates_Rows(HtmlNode[] elements)
        {
            var listOfName = new WikiWhatToExtractModel();
            listOfName.AdditionalMetaData!.Add("Country", "United States");

            int tcolCounter = 1;

            foreach (var elm in elements)
            {
                if (tcolCounter == 1)
                {
                    Common_Portrait_Extract(elm, listOfName);
                }
                if (tcolCounter == 2)
                {
                    Common_PersonDetail_Extract(elm, listOfName, titleRemoveInnerSpan: false, extractBirthDeath: true);
                }
                if (tcolCounter == 3)
                {
                    var term = elm.DecodedInnerText(removeNewLine: true).SplitAndTrim("–");
                    if (term.Count() != 2) throw new Exception("The split on the term did not result with right values");
                    listOfName.AdditionalMetaData!.Add("Took office", term.First());
                    listOfName.AdditionalMetaData!.Add("Left office", term.Skip(1).First());
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

            if (!ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Birth-Death")) return null;
            if (!ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Took office")) return null;
            if (!ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Left office")) return null;

            listOfName.Sequence = sequence++;
            return listOfName;
        }
    }
}
