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
        public List<WikiWhatToExtractModel> ExtractListTabularData_Canada(HtmlDocument document, List<string>? tags)
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
                        var extractedData = ExtractListTabularData_Canada_Rows(cells);
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
        private WikiWhatToExtractModel? ExtractListTabularData_Canada_Rows(HtmlNode[] elements)
        {
            var listOfName = new WikiWhatToExtractModel();
            listOfName.AdditionalMetaData!.Add("Country", "Canada");

            // No. column is a <th> (not <td>), so .//td skips it — all indices shift by -1
            // td1=portrait, td2=name+birth-death, td3=combined term "start – end",
            // td4=electoral mandates, td5=party colour swatch, td6=party name,
            // td7=parliamentary seat, td8=cabinet, td9=refs
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
                    // Term of office is a single combined cell: "start – end"
                    var term = elm.DecodedInnerText(removeNewLine: true).SplitAndTrim("–");
                    if (term.Count() >= 2)
                    {
                        listOfName.AdditionalMetaData!.AddOrUpdate("Took office", term.First().Trim());
                        listOfName.AdditionalMetaData!.AddOrUpdate("Left office", term.Skip(1).First().Trim());
                    }
                }
                if (tcolCounter == 6)
                {
                    Common_SimpleDataType01_Extract(elm, listOfName, "Political party", removeSpecialChars: false);
                }
                if (tcolCounter == 7)
                {
                    Common_SimpleDataType01_Extract(elm, listOfName, "Riding", removeSpecialChars: false);
                }
                if (tcolCounter == 8)
                {
                    Common_SimpleDataType01_Extract(elm, listOfName, "Cabinet", removeSpecialChars: false);
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
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Political party");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Riding");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Cabinet");

            listOfName.Sequence = sequence++;
            return listOfName;
        }
    }
}
