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
        int sequence = 1;

        private List<WikiWhatToExtractModel> ExtractListTabularData_Stub(HtmlDocument document, List<string>? tags, string country)
        {
            // TODO: implement column mapping for this country
            Console.WriteLine($"[STUB] {country} — HTML downloaded, extraction not yet implemented.");
            return new List<WikiWhatToExtractModel>();
        }

        private void Common_Portrait_Extract(HtmlNode? elm, WikiWhatToExtractModel listOfName, string fieldName = "Portrait")
        {
            if (elm == null) return;
            if (listOfName.AdditionalMetaData == null) return;
            if (listOfName.AdditionalMetaData.ContainsKey(fieldName)) return;

            var portraitElm = elm.SelectNodes($"{elm.XPath}//img")?.FirstOrDefault();
            if (portraitElm != null && portraitElm.Attributes.Count > 0 && portraitElm.Attributes.Any(f => f.Name == "src") &&
                portraitElm.Attributes.FirstOrDefault(f => f.Name == "src")?.Value.HasValue() == true)
            {
                var portraitUrl = portraitElm.Attributes["src"].Value;
                if (portraitUrl.StartsWith("http") == false)
                {
                    portraitUrl = $"https:{(portraitUrl.StartsWith("//") ? "" : "//")}{portraitUrl}";
                }
                listOfName.AdditionalMetaData!.AddOrUpdate(fieldName, portraitUrl);
            }
        }

        private void Common_PersonDetail_Extract(HtmlNode? elm, WikiWhatToExtractModel listOfName,
            bool titleRemoveInnerSpan,
            bool extractBirthDeath,
            string birthDeathFieldName = "Birth-Death")
        {
            if (elm == null) return;
            if (listOfName.AdditionalMetaData == null) return;
            if (listOfName.AdditionalMetaData.ContainsKey(birthDeathFieldName)) return;
            if (listOfName.Title.HasValue()) return;

            var personElm = elm.SelectNodes($"{elm.XPath}//b/a")?.FirstOrDefault() ??
                        elm.SelectNodes($"{elm.XPath}/a")?.FirstOrDefault() ??
                        elm.SelectNodes($"{elm.XPath}/div/a")?.FirstOrDefault();

            if (personElm != null)
            {
                if (personElm == null) throw new Exception("The name element is missing");
                if (personElm.Attributes.Count > 0 &&
                    personElm.Attributes.Any(a => a.Name == "href" && a.Value.HasValue()))
                {
                    listOfName.Route = HttpUtility.UrlDecode(HtmlAgilityEx.DecodedInnerText(content: personElm.Attributes["href"].Value, removeNewLine: false));
                    if (titleRemoveInnerSpan)
                    {
                        //var childNotRequiredElm = 
                        //    (elm.SelectNodes($"{personElm.XPath}/span") ?? Enumerable.Empty<HtmlNode>())
                        //        .Concat(elm.SelectNodes($"{personElm.XPath}/i") ?? Enumerable.Empty<HtmlNode>())
                        //    .Where(e => e?.InnerText?.HasValue() == true);

                        var childNotRequiredElm = from e in elm.SelectNodes($"{personElm.XPath}//span") ?? Enumerable.Empty<HtmlNode>()
                                                  where e?.InnerText?.HasValue() == true
                                                  select e;
                        if (childNotRequiredElm != null)
                        {
                            var nodesToRemove = childNotRequiredElm.ToList();
                            foreach (var cRemove in nodesToRemove)
                            {
                                try
                                {
                                    personElm.RemoveChild(cRemove);
                                }
                                catch (Exception)
                                {  //suppress the exception as the remove can remove the inner nodes since the selector is to get all the spans 
                                }
                            }
                        }
                    }
                    listOfName.Title = personElm.DecodedInnerText(removeNewLine: true).Trim();
                }
                else throw new Exception("The first element <a> does not have required details");
                if (!extractBirthDeath) return;

                var spanContainerElm = elm.SelectNodes($"{elm.XPath}//small")?.FirstOrDefault();
                if (spanContainerElm == null ||
                    //This is the extraction on the bottom and check of value exists is done here
                    //some scenarios value is not the (-) in this format, it has to go through this path if value not there
                    spanContainerElm != null && spanContainerElm.DecodedInnerText(removeNewLine: true)?.RegexMatchGroupValue("\\(([^)]*)\\)[^(]*$", 0)?.RegexMatchGroupValue("\\((.*?)\\)", 0)?.HasValue() == false)
                {
                    var search01 = elm.SelectNodes($"{elm.XPath}//span") ?? Enumerable.Empty<HtmlNode>();
                    var search02 = elm.SelectNodes($"{elm.XPath}//li") ?? Enumerable.Empty<HtmlNode>();

                    var spanContainerElmNewSearch = from e in search01.Concat(search02)
                                                    let txt = e.DecodedInnerText(removeNewLine: true)
                                                    where txt != null && txt.Contains('(') && txt.Contains(')') && txt.ContainsAnyNumber()
                                                    select e;

                    if (spanContainerElmNewSearch != null)
                    {
                        spanContainerElm = spanContainerElmNewSearch.FirstOrDefault();
                    }
                    else
                    {
                        throw new Exception("The span container element which has details about the person is missing");
                    }
                }

                var textRaw = spanContainerElm.DecodedInnerText(removeNewLine: true);
                var birthDeathExtracted = textRaw.RegexMatchGroupValue("\\(([^)]*)\\)[^(]*$", 0);
                var birthDeathParsed = birthDeathExtracted.RegexMatchGroupValue("\\((.*?)\\)", 0);

                var term = birthDeathParsed.SplitAndTrim("–");
                listOfName.AdditionalMetaData.Add(birthDeathFieldName, string.Join(" - ", term).ReplaceMultiple("", "(", ")"));
            }
        }

        private void Common_Complex_BirthDeath(HtmlNode? elm, WikiWhatToExtractModel listOfName, string birthDeathFieldName = "Birth-Death")
        {
            if (elm == null) return;
            if (listOfName.AdditionalMetaData == null) return;
            if (listOfName.AdditionalMetaData.ContainsKey(birthDeathFieldName)) return;

            var spanContainerElm = elm.SelectNodes($"{elm.XPath}")?.FirstOrDefault();
            if (spanContainerElm == null) throw new Exception("The span container element which has details about the person is missing");

            string pattern = @"\((\d{4})[–-](\d{4})\)|\(born (\d{4})\)|\(b\. (\d{4})\)";

            MatchCollection matches = Regex.Matches(spanContainerElm.DecodedInnerText(removeNewLine: true).Trim(), pattern);

            foreach (Match match in matches)
            {
                if (listOfName.AdditionalMetaData.ContainsKey(birthDeathFieldName)) return;
                if (match.Groups[1].Success && match.Groups[2].Success) // Match for birth and death years
                {
                    string birthYear = match.Groups[1].Value;
                    string deathYear = match.Groups[2].Value;
                    if (birthYear.HasValue() && deathYear.HasValue())
                    {
                        listOfName.AdditionalMetaData.Add(birthDeathFieldName, $"{birthYear} - {deathYear}");
                    }
                }
                else if (match.Groups[3].Success) // Match for "born" birth year
                {
                    string birthYear = match.Groups[3].Value;
                    if (!string.IsNullOrWhiteSpace(birthYear))
                    {
                        listOfName.AdditionalMetaData[birthDeathFieldName] = $"born {birthYear}";
                    }
                }
                else if (match.Groups[4].Success) // Match for "b." birth year
                {
                    string birthYear = match.Groups[4].Value;
                    if (!string.IsNullOrWhiteSpace(birthYear))
                    {
                        listOfName.AdditionalMetaData[birthDeathFieldName] = $"born {birthYear}";
                    }
                }
            }
        }


        private void Common_DateType01_Extract(HtmlNode? elm, WikiWhatToExtractModel listOfName, string fieldName, string[]? additionalContentToCheck, bool removeSpecialChars)
        {
            if (elm == null) return;
            if (listOfName.AdditionalMetaData == null) return;
            if (listOfName.AdditionalMetaData.ContainsKey(fieldName)) return;

            var monthNames = DateTimeFormatInfo.CurrentInfo.MonthNames.Where(f => f.HasValue());
            var dataInRaw = elm.DecodedInnerText(removeNewLine: true);
            if (monthNames.Any(f => dataInRaw.ContainsIgnoreCase(f) ||
                (additionalContentToCheck != null && additionalContentToCheck.Any(g => dataInRaw.EqualsIgnoreCase(g)))))
            {
                if (removeSpecialChars)
                {
                    dataInRaw = dataInRaw.RemoveSpecialChars(excludeWhitespace: true);
                }
                listOfName.AdditionalMetaData.Add(fieldName, dataInRaw.Trim());
            }
        }

        private void Common_SimpleDataType01_Extract(HtmlNode? elm, WikiWhatToExtractModel listOfName, string fieldName, bool removeSpecialChars)
        {
            if (elm == null) return;
            if (listOfName.AdditionalMetaData == null) return;
            if (listOfName.AdditionalMetaData.ContainsKey(fieldName)) return;

            var dataInRaw = elm.DecodedInnerText(removeNewLine: true);
            if (dataInRaw.HasValue())
            {
                if (removeSpecialChars)
                {
                    dataInRaw = dataInRaw.RemoveSpecialChars(excludeWhitespace: true);
                }
                listOfName.AdditionalMetaData.Add(fieldName, dataInRaw.Trim());
            }
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

        private WikiWhatToExtractModel? ExtractListTabularData_Monarch_Rows(HtmlNode[] elements, string country)
        {
            var m = new WikiWhatToExtractModel();
            m.AdditionalMetaData!.Add("Country", country);
            int c = 1;
            foreach (var elm in elements)
            {
                // succession-table-monarch: td1=name+lifespan, td2=lifespan-text, td3=reign-start, td4=reign-end, td5=notes, td6=dynasty, td7=portrait
                if (c == 7) Common_Portrait_Extract(elm, m);
                if (c == 1) Common_PersonDetail_Extract(elm, m, titleRemoveInnerSpan: false, extractBirthDeath: false);
                if (c == 2) Common_Complex_BirthDeath(elm, m);
                if (c == 3) Common_DateType01_Extract(elm, m, "Took office", null, removeSpecialChars: true);
                if (c == 4) Common_DateType01_Extract(elm, m, "Left office", new[] { "Incumbent" }, removeSpecialChars: true);
                if (c == 6) Common_SimpleDataType01_Extract(elm, m, "Dynasty", removeSpecialChars: false);
                c++;
            }
            if (m.Title.IsEmpty()) return null;
            Console.WriteLine($"Extraction: {m.Title} [{m.Route}]");
            ValidateAdditionalMetaData(m.AdditionalMetaData, "Took office");
            ValidateAdditionalMetaData(m.AdditionalMetaData, "Left office");
            m.Sequence = sequence++; return m;
        }
    }
}
