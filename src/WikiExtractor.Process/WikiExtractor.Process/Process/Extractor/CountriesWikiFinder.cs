using HtmlAgilityPack;
using Pj.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WikiExtractor.Exts;
using WikiExtractor.Models;

namespace WikiExtractor.Process.Extractor
{
    public class CountriesWikiFinder
    {
        private const string _metadata_Rank = "Rank";
        private const string _metadata_TotalKm = "Total in km2 (mi2)";
        private const string _metadata_Land = "Land in km2 (mi2)";
        private const string _metadata_Water = "Water in km2 (mi2)";
        private const string _metadata_WaterPercentage = "Water %";
        private const string _metadata_Notes = "Notes";
        private const string _metadata_FlagImage = "FlagImage";
        int sequence = 1;

        public List<WikiWhatToExtractModel> ListByDependencyArea_ForCountries(HtmlDocument document, List<string>? tags)
        {
            return ListByDependencyArea(document, tags, "Countries");
        }

        public List<WikiWhatToExtractModel> ListByDependencyArea_ForNonCountries(HtmlDocument document, List<string>? tags)
        {
            return ListByDependencyArea(document, tags, "NonCountries");
        }


        private List<WikiWhatToExtractModel> ListByDependencyArea(HtmlDocument document, List<string>? tags, string extractionType)
        {
            List<WikiWhatToExtractModel> listOfNames = new List<WikiWhatToExtractModel>();
            sequence = 1;

            var tableData = document.DocumentNode.SelectNodes($"//table[contains(@class, 'wikitable')]//tbody/tr");
            foreach (var tableRow in tableData)
            {
                if (tableRow.ChildNodes.Count(f => f.Name == "td") <= 5)
                {
                    continue;
                }
                var elements = tableRow.ChildNodes.Where(f => f.Name == "td").ToArray();
                var extractedData = ExtractListOfCountryByDependencyArea(elements, extractionType);
                if (extractedData != null)
                {
                    extractedData.Tags = tags.DeepClone();
                    listOfNames.Add(extractedData);
                }
            }

            return listOfNames.OrderByDescending(f => f.Sequence).ToList(); ;
        }
        private WikiWhatToExtractModel? ExtractListOfCountryByDependencyArea(HtmlNode[] elements, string extractionType)
        {
            var listOfName = new WikiWhatToExtractModel();

            int elePos_Rank = 0;
            int elePos_Country = 1;
            int elePos_TotalKm = 2;
            int elePos_Land = 3;
            int elePos_Water = 4;
            int elePos_WaterPercentage = 5;
            int elePos_Notes = 6;

            var num = elements[elePos_Rank].DecodedInnerText(removeNewLine: true).Trim();

            if (elements.Count() == 6 && num.Trim().ContainsAllNumber() == false)
            {
                elePos_Rank = 0;
                elePos_Country = 0;
                elePos_TotalKm = 1;
                elePos_Land = 2;
                elePos_Water = 3;
                elePos_WaterPercentage = 4;
                elePos_Notes = 5;
            }
            else if (elements.Count() == 6)
            {
                elePos_Rank = 0;
                elePos_Country = 1;
                elePos_TotalKm = 2;
                elePos_Land = 3;
                elePos_Water = 4;
                elePos_WaterPercentage = 5;
                elePos_Notes = -1;
            }
            else
            {
                elePos_Rank = 0;
                elePos_Country = 1;
                elePos_TotalKm = 2;
                elePos_Land = 3;
                elePos_Water = 4;
                elePos_WaterPercentage = 5;
                elePos_Notes = 6;
            }

            if (extractionType == "Countries")
            {
                var isExtractionValid = num.HasValue() && !num.Contains("–");
                if (isExtractionValid == false)
                {
                    return null;
                }
            }
            else if (extractionType == "NonCountries")
            {
                var isExtractionValid = num.HasValue() && num.Contains("–");
                if (isExtractionValid == false)
                {
                    return null;
                }

                if (elements[elePos_Country].DecodedInnerText(true).Contains("World")) { return null; }
            }




            listOfName.AdditionalMetaData!.AddOrUpdate(_metadata_Rank, num);

            HtmlNode countryNameDetailsElement;
            if (elements[elePos_Country].ChildNodes.Count == 1 && elements[elePos_Country].ChildNodes.FirstOrDefault().Name == "span" || elements[elePos_Country].ChildNodes.FirstOrDefault().Name == "i")
            {
                countryNameDetailsElement = elements[elePos_Country].ChildNodes.FirstOrDefault();
            }
            else
            {
                countryNameDetailsElement = elements[elePos_Country];
            }

            if (countryNameDetailsElement.ChildNodes.Any(f => f.Name == "a"))
            {
                var anchor = countryNameDetailsElement.ChildNodes.FirstOrDefault(f => f.Name == "a");
                if (anchor != null && anchor.Attributes.Count > 0)
                {
                    if (anchor.Attributes.Any(a => a.Name == "href" && a.Value.HasValue()))
                    {
                        listOfName.Route = HttpUtility.UrlDecode(HtmlAgilityEx.DecodedInnerText(content: anchor.Attributes["href"].Value, removeNewLine: false));
                        listOfName.Title = anchor.DecodedInnerText(removeNewLine: true).Trim();
                        if (extractionType == "NonCountries")
                        {
                            listOfName.Title = elements[elePos_Country].DecodedInnerText(true).Trim();
                        }
                    }
                }
            }
            else
            {
                return null;
            }

            //Flag Extraction
            var flagIcon = countryNameDetailsElement.SelectNodes($"{elements[elePos_Country].XPath}//*[contains(@class, 'flagicon')]//img")?.FirstOrDefault();
            if (flagIcon != null && flagIcon.Attributes.Count > 0 && flagIcon.Attributes.Any(f => f.Name == "src") &&
                flagIcon.Attributes.FirstOrDefault(f => f.Name == "src")?.Value.HasValue() == true)
            {
                var flagUrl = flagIcon.Attributes["src"].Value;
                var n = Path.GetFileName(flagUrl);
                var m = n.ReplaceWithRegex("(\\d+)px", "150px");
                var newFlagUrl = flagUrl.Replace(n, m);
                if (newFlagUrl.StartsWith("http") == false)
                {
                    newFlagUrl = $"https://{newFlagUrl}";
                }
                listOfName.AdditionalMetaData!.Add(_metadata_FlagImage, newFlagUrl);
            }
            else
            {
                Console.WriteLine($"There is no flag for this country {listOfName.Title}");
                //throw new Exception("There is no flag for this country");
            }

            listOfName.AdditionalMetaData!.Add(_metadata_TotalKm, elements[elePos_TotalKm].DecodedInnerText(removeNewLine: true).Trim());
            listOfName.AdditionalMetaData!.Add(_metadata_Land, elements[elePos_Land].DecodedInnerText(removeNewLine: true).Trim());
            listOfName.AdditionalMetaData!.Add(_metadata_Water, elements[elePos_Water].DecodedInnerText(removeNewLine: true).Trim());
            listOfName.AdditionalMetaData!.Add(_metadata_WaterPercentage, elements[elePos_WaterPercentage].DecodedInnerText(removeNewLine: true).Trim());

            if (elePos_Notes > 0)
            {
                listOfName.AdditionalMetaData!.Add(_metadata_Notes, elements[elePos_Notes].DecodedInnerText(removeNewLine: true).Trim());
            }


            Console.WriteLine($"Extraction: {listOfName.Title} [{listOfName.Route}]");
            Console.WriteLine($"Details -> {_metadata_TotalKm}: {listOfName.AdditionalMetaData[_metadata_TotalKm]}");
            Console.WriteLine($"Details -> {_metadata_Land}: {listOfName.AdditionalMetaData[_metadata_Land]}");
            Console.WriteLine($"Details -> {_metadata_Water}: {listOfName.AdditionalMetaData[_metadata_Water]}");
            Console.WriteLine($"Details -> {_metadata_WaterPercentage}: {listOfName.AdditionalMetaData[_metadata_WaterPercentage]}");
            if (elePos_Notes > 0)
            {
                Console.WriteLine($"Details -> {_metadata_Notes}: {listOfName.AdditionalMetaData[_metadata_Notes]}");
            }
            if (listOfName.AdditionalMetaData.ContainsKey(_metadata_FlagImage))
            {
                Console.WriteLine($"Details -> {_metadata_FlagImage}: {listOfName.AdditionalMetaData[_metadata_FlagImage]}");
            }
            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine("");

            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, _metadata_Rank);
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, _metadata_TotalKm);
            if (extractionType == "Countries")
            {
                ValidateAdditionalMetaData(listOfName.AdditionalMetaData, _metadata_Land);
                ValidateAdditionalMetaData(listOfName.AdditionalMetaData, _metadata_Water);
                ValidateAdditionalMetaData(listOfName.AdditionalMetaData, _metadata_WaterPercentage);
                ValidateAdditionalMetaData(listOfName.AdditionalMetaData, _metadata_FlagImage);
            }


            listOfName.Sequence = sequence++;

            return listOfName;
        }


        private void ValidateAdditionalMetaData(Dictionary<string, string> data, string field)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            if (data.ContainsKey(field) == false)
            {
                throw new Exception($"Additional data extraction failed to extract {field}");
            }
            if (data.ContainsKey(field) && data[field].IsEmpty())
            {
                throw new Exception($"Additional data extraction failed to extract any data for the {field}");
            }
        }
    }
}
