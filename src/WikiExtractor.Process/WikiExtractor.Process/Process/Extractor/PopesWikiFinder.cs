using HtmlAgilityPack;
using Pj.Library;
using System.Web;
using WikiExtractor.Exts;
using WikiExtractor.Models;

namespace WikiExtractor.Process.Extractor
{
    public class PopesWikiFinder
    {
        private const string MetadataPontiffNumber = "Pontiff number";
        private const string MetadataPontificate = "Pontificate";
        private const string MetadataEnglishName = "English Name";
        private const string MetadataLatinName = "Latin Name";
        private const string MetadataDateAndPlaceOfBirth = "Date & Place Of Birth";
        private const string MetadataAgeAtStartEndOfPapacy = "Age at start/nend of papacy";
        private const string MetadataNotes = "Extras";
        private const string MetadataPersonalName = "Personal Name";
        private const string MetadataPortaritImage = "Portait Image";

        public List<WikiWhatToExtractModel> ExtractByCenturyFromTable(HtmlDocument document, string tableFinderText, List<string>? tags, bool hasPortrait, bool hasPersonalName)
        {
            List<WikiWhatToExtractModel> listOfNames = new List<WikiWhatToExtractModel>();
            int sequence = 1;
            //  Find table: //h4//*[contains(text(), '1st century')]//..//..//table[contains(@class, 'wikitable')]
            //List<List<string>> table = document.DocumentNode.SelectSingleNode("//h4//*[contains(text(), '1st century')]//..//..//table[contains(@class, 'wikitable')]")
            //    .Descendants("tr")
            //    .Skip(1)
            //    .Where(tr => tr.Elements("td").Count() > 1)
            //    .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
            //    .ToList();

            //List<List<string>> headers = document.DocumentNode.SelectSingleNode("//h4//*[contains(text(), '1st century')]//..//..//table[contains(@class, 'wikitable')]")
            //    .Descendants("tr")
            //    .Take(1)
            //    .Select(tr => tr.Elements("th").Select(td => td.InnerText.Trim()).ToList())
            //    .ToList();

            var tableData = document.DocumentNode.SelectNodes($"//table[caption[contains(text(), '{tableFinderText}')]]/tbody/tr")?.Skip(1) ?? Enumerable.Empty<HtmlNode>();

            int elePosPoitiff = 0;
            int elePosPontificate = 0;
            int elePosPersonalName = 0;
            int elePosNames = 0;
            int elePosDateAndPlaceOfBirth = 0;
            int elePosAgeAtStartEndOfPapacy = 0;
            int elePosNotes = 0;

            if (hasPortrait && hasPersonalName)
            {
                elePosPoitiff = 0;
                elePosPontificate = 1;
                elePosNames = 3;
                elePosPersonalName = 4;
                elePosDateAndPlaceOfBirth = 5;
                elePosAgeAtStartEndOfPapacy = 6;
                elePosNotes = 7;
            }
            else
            {
                elePosPoitiff = 0;
                elePosPontificate = 1;
                elePosNames = 2;
                elePosDateAndPlaceOfBirth = 3;
                elePosAgeAtStartEndOfPapacy = 4;
                elePosNotes = 5;
            }

            foreach (var item in tableData)
            {
                if (item.ChildNodes.Count(f => f.Name == "td") <= 5)
                {
                    continue;
                }
                var elements = item.ChildNodes.Where(f => f.Name == "td").ToArray();

                // Wikipedia's name cells are structured as: <b>Name <a href>...</a></b><br/><span>Latin</span>
                // That's 3 direct element children — the old ">= 4" guard was based on a stale page structure.
                var nameElementChildCount = elements[elePosNames].ChildNodes.Count(f => f.Name != "#text");
                if (nameElementChildCount < 2) // need at least <b/i/a> and <span> for Latin name
                {
                    continue;
                }

                var listOfName = new WikiWhatToExtractModel();
                int counter = 1;
                listOfName.AdditionalMetaData!.Add(MetadataPontiffNumber, elements[elePosPoitiff].DecodedInnerText(removeNewLine: true).Trim());
                listOfName.AdditionalMetaData!.Add(MetadataPontificate, elements[elePosPontificate].DecodedInnerText(removeNewLine: true).Trim());

                //Names with HyperLinks
                {
                    var engName = elements[elePosNames].ChildNodes.FirstOrDefault(f => f.Name == "b" || f.Name == "i" || f.Name == "a")?.DecodedInnerText(removeNewLine: true).Trim();
                    listOfName.AdditionalMetaData!.AddOrUpdate(MetadataEnglishName, engName);
                    listOfName.AdditionalMetaData!.AddOrUpdate(MetadataLatinName, elements[elePosNames].ChildNodes.FirstOrDefault(f => f.Name == "span")?.DecodedInnerText(removeNewLine: true).Trim());

                    // Find the <a> anywhere inside the name cell — covers <b><a>, <i><a>, direct <a>
                    var anchor = elements[elePosNames].Descendants("a").FirstOrDefault(a => a.Attributes["href"]?.Value.HasValue() == true);
                    if (anchor != null)
                    {
                        var href = HttpUtility.UrlDecode(HtmlAgilityEx.DecodedInnerText(content: anchor.Attributes["href"].Value, removeNewLine: false));
                        // Strip absolute URL prefix if Wikipedia returns full URL (//en.wikipedia.org/wiki/...)
                        const string wikiPrefix = "//en.wikipedia.org";
                        if (href.StartsWith(wikiPrefix, StringComparison.OrdinalIgnoreCase))
                        {
                            href = href.Substring(wikiPrefix.Length);
                        }
                        listOfName.Route = href;
                    }
                }

                if (hasPersonalName)
                {
                    listOfName.AdditionalMetaData!.Add(MetadataPersonalName, elements[elePosPersonalName].DecodedInnerText(removeNewLine: true).Trim());
                }

                listOfName.AdditionalMetaData!.Add(MetadataDateAndPlaceOfBirth, elements[elePosDateAndPlaceOfBirth].DecodedInnerText(removeNewLine: true).Trim());
                listOfName.AdditionalMetaData!.Add(MetadataAgeAtStartEndOfPapacy, elements[elePosAgeAtStartEndOfPapacy].DecodedInnerText(removeNewLine: true).Trim());
                listOfName.AdditionalMetaData!.Add(MetadataNotes, elements[elePosNotes].DecodedInnerText(removeNewLine: false).Trim());

                if (listOfName.Route.IsEmpty())
                {
                    throw new Exception("Did not extract the route!");
                }
                ValidateAdditionalMetaData(listOfName.AdditionalMetaData, MetadataPontiffNumber);
                ValidateAdditionalMetaData(listOfName.AdditionalMetaData, MetadataPontificate);
                ValidateAdditionalMetaData(listOfName.AdditionalMetaData, MetadataEnglishName);
                ValidateAdditionalMetaData(listOfName.AdditionalMetaData, MetadataLatinName);
                ValidateAdditionalMetaData(listOfName.AdditionalMetaData, MetadataDateAndPlaceOfBirth);
                ValidateAdditionalMetaData(listOfName.AdditionalMetaData, MetadataAgeAtStartEndOfPapacy, checkIsEmpty: false);
                ValidateAdditionalMetaData(listOfName.AdditionalMetaData, MetadataNotes);
                if (hasPersonalName)
                {
                    ValidateAdditionalMetaData(listOfName.AdditionalMetaData, MetadataPersonalName, checkIsEmpty: false);
                }
                listOfName.Title = listOfName.AdditionalMetaData[MetadataEnglishName];
                listOfName.Tags = tags;
                listOfName.Sequence = sequence++;

                listOfNames.Add(listOfName);

                Console.WriteLine($"Extraction -> {listOfName.AdditionalMetaData[MetadataPontiffNumber]} - {listOfName.AdditionalMetaData[MetadataEnglishName]} [{listOfName.AdditionalMetaData[MetadataLatinName]}]");
                Console.WriteLine($"Details -> Pontificate: {listOfName.AdditionalMetaData[MetadataPontificate]}");
                Console.WriteLine($"Details -> Date and Place of birth: {listOfName.AdditionalMetaData[MetadataDateAndPlaceOfBirth]}");
                Console.WriteLine($"Details -> Age at start & end of papacy: {listOfName.AdditionalMetaData[MetadataAgeAtStartEndOfPapacy]}");
                if (listOfName.AdditionalMetaData.ContainsKey(MetadataPersonalName))
                {
                    Console.WriteLine($"Details -> Personal Name: {listOfName.AdditionalMetaData[MetadataPersonalName]}");
                }
                if (listOfName.AdditionalMetaData.ContainsKey(MetadataPortaritImage))
                {
                    Console.WriteLine($"Details -> Portait Link: {listOfName.AdditionalMetaData[MetadataPortaritImage]}");
                }
                Console.WriteLine("-----------------------------------------------------------------------");
                Console.WriteLine("");
            }
            return listOfNames.OrderByDescending(f => f.Sequence).ToList();
        }

        private void ValidateAdditionalMetaData(Dictionary<string, string> data, string field, bool checkIsEmpty = true)
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
                if (checkIsEmpty)
                {
                    throw new Exception($"Additional data extraction failed to extract any data for the {field}");
                }
            }
        }
    }
}
