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
    public class WorldLeadersWikiFinder
    {
        int sequence = 1;

        public List<WikiWhatToExtractModel> ExtractListTabularData_Australia(HtmlDocument document, List<string>? tags)
        {
            List<WikiWhatToExtractModel> listOfNames = new List<WikiWhatToExtractModel>();
            sequence = 1;

            var tableData = document.DocumentNode.SelectNodes($"//table[contains(@class, 'wikitable')]//tbody/tr");
            foreach (var tableRow in tableData.Skip(2))
            {
                if (tableRow.ChildNodes.Count(f => f.Name == "td") + tableRow.ChildNodes.Count(f => f.Name == "th") < 8)
                {
                    continue;
                }
                var elements = tableRow.ChildNodes.Where(f => f.Name == "td" || f.Name == "th").ToArray();
                var extractedData = ExtractListTabularData_Australia_Rows(elements);
                if (extractedData != null)
                {
                    extractedData.Tags = tags.DeepClone();
                    listOfNames.Add(extractedData);
                }
            }

            return listOfNames;
        }
        private WikiWhatToExtractModel? ExtractListTabularData_Australia_Rows(HtmlNode[] elements)
        {
            var listOfName = new WikiWhatToExtractModel();

            int tcolCounter = 1;
            foreach (var elm in elements)
            {
                if (tcolCounter == 1)
                {
                    listOfName.AdditionalMetaData!.Add("No", elm.DecodedInnerText(removeNewLine: true).Trim());
                }
                else if (tcolCounter == 2)
                {
                    var portraitElm = elm.SelectNodes($"{elm.XPath}//img")?.FirstOrDefault();
                    if (portraitElm != null && portraitElm.Attributes.Count > 0 && portraitElm.Attributes.Any(f => f.Name == "src") &&
                        portraitElm.Attributes.FirstOrDefault(f => f.Name == "src")?.Value.HasValue() == true)
                    {
                        var portraitUrl = portraitElm.Attributes["src"].Value;
                        if (portraitUrl.StartsWith("http") == false)
                        {
                            portraitUrl = $"https:{(portraitUrl.StartsWith("//") ? "" : "//")}{portraitUrl}";
                        }
                        listOfName.AdditionalMetaData!.AddOrUpdate("Portrait", portraitUrl);
                    }
                }
                else if (tcolCounter == 3)
                {
                    var subElms = elm.ChildNodes.Where(f => f.Name == "a" || f.Name == "span" || f.Name == "small"
                        || (f.Name == "#text" && f.DecodedInnerText(removeNewLine: true).HasValue())).ToList();
                    if (subElms.Count() < 3)
                    {
                        throw new Exception("There should be only 3 elements in this cell");
                    }
                    { //Name and route
                        if (subElms[0].Name != "a")
                        {
                            throw new Exception("The first elment should be <a> and this holds the Name of the person");
                        }
                        if (subElms[0].Attributes.Count > 0 &&
                            subElms[0].Attributes.Any(a => a.Name == "href" && a.Value.HasValue()))
                        {
                            listOfName.Route = HttpUtility.UrlDecode(HtmlAgilityEx.DecodedInnerText(content: subElms[0].Attributes["href"].Value, removeNewLine: false));
                            listOfName.Title = subElms[0].DecodedInnerText(removeNewLine: true).Trim();
                        }
                        else
                        {
                            throw new Exception("The first elment <a> does not have required details");
                        }
                    }
                    listOfName.AdditionalMetaData.Add("Birth-Death", subElms[1].DecodedInnerText(removeNewLine: true));
                    listOfName.AdditionalMetaData.Add("Constituency", string.Join(", ", subElms.Skip(2).Select(f => f.DecodedInnerText(removeNewLine: true))));
                }
                else if (tcolCounter == 5)
                {
                    listOfName.AdditionalMetaData!.Add("Took office", elm.DecodedInnerText(removeNewLine: true).Trim());
                }
                else if (tcolCounter == 6)
                {
                    listOfName.AdditionalMetaData!.Add("Left office", elm.DecodedInnerText(removeNewLine: true).Trim());
                }
                else if (tcolCounter == 7)
                {
                    listOfName.AdditionalMetaData!.Add("Days in office", elm.DecodedInnerText(removeNewLine: true).Trim());
                }
                else if (tcolCounter == 8)
                {
                    listOfName.AdditionalMetaData!.Add("Political party", elm.DecodedInnerText(removeNewLine: true).Trim());
                }
                //Political party
                tcolCounter++;
            }

            Console.WriteLine($"Extraction: {listOfName.Title} [{listOfName.Route}]");
            Console.WriteLine($"Details -> Birth-Death: {listOfName.AdditionalMetaData["Birth-Death"]}");
            Console.WriteLine($"Details -> Constituency: {listOfName.AdditionalMetaData["Constituency"]}");
            Console.WriteLine($"Details -> Days in office: {listOfName.AdditionalMetaData["Days in office"]}");
            Console.WriteLine($"Details -> Took office: {listOfName.AdditionalMetaData["Took office"]}");
            Console.WriteLine($"Details -> Left office: {listOfName.AdditionalMetaData["Left office"]}");
            Console.WriteLine($"Details -> Political party: {listOfName.AdditionalMetaData["Political party"]}");
            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine("");

            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Birth-Death");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Constituency");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Days in office");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Took office");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Left office");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Political party");

            listOfName.Sequence = sequence++;
            return listOfName;
        }

        public List<WikiWhatToExtractModel> ExtractListTabularData_India(HtmlDocument document, List<string>? tags)
        {
            List<WikiWhatToExtractModel> listOfNames = new List<WikiWhatToExtractModel>();
            sequence = 1;

            var tableData = document.DocumentNode.SelectNodes($"//table[contains(@class, 'wikitable')]//tbody/tr/th[contains(text(), 'Portrait')]//..//..//tr");
            foreach (var tableRow in tableData.Skip(1))
            {
                if (tableRow.ChildNodes.Count(f => f.Name == "td") + tableRow.ChildNodes.Count(f => f.Name == "th") < 8)
                {
                    continue;
                }
                var elements = tableRow.ChildNodes.Where(f => f.Name == "td" || f.Name == "th").ToArray();
                var extractedData = ExtractListTabularData_India_Rows(elements);
                if (extractedData != null)
                {
                    extractedData.Tags = tags.DeepClone();
                    listOfNames.Add(extractedData);
                }
            }

            { //Extract year of service

                List<string> extractedYearOfService = new();

                foreach (var tableRow in tableData.Skip(1))
                {
                    var nameElm = tableRow.SelectNodes($"{tableRow.XPath}//b[contains(text(), 'days')]")?.FirstOrDefault();
                    if (nameElm != null)
                    {
                        extractedYearOfService.Add(nameElm.DecodedInnerText(removeNewLine: true).Trim());
                    }
                }

                if (extractedYearOfService.Count != listOfNames.Count) throw new Exception("The extracted rows for the two logic does not match");
                for (int i = 0; i < extractedYearOfService.Count; i++)
                {
                    listOfNames[i].AdditionalMetaData.Add("Days in office", extractedYearOfService[i]);
                }
            }

            return listOfNames;
        }
        private WikiWhatToExtractModel? ExtractListTabularData_India_Rows(HtmlNode[] elements)
        {
            var listOfName = new WikiWhatToExtractModel();

            int tcolCounter = 1;
            foreach (var elm in elements)
            {
                if (tcolCounter == 1)
                {
                    listOfName.AdditionalMetaData!.Add("No", elm.DecodedInnerText(removeNewLine: true).Trim());
                }
                else if (tcolCounter == 3)
                {
                    var portraitElm = elm.SelectNodes($"{elm.XPath}//img")?.FirstOrDefault();
                    if (portraitElm != null && portraitElm.Attributes.Count > 0 && portraitElm.Attributes.Any(f => f.Name == "src") &&
                        portraitElm.Attributes.FirstOrDefault(f => f.Name == "src")?.Value.HasValue() == true)
                    {
                        var portraitUrl = portraitElm.Attributes["src"].Value;
                        if (portraitUrl.StartsWith("http") == false)
                        {
                            portraitUrl = $"https:{(portraitUrl.StartsWith("//") ? "" : "//")}{portraitUrl}";
                        }
                        listOfName.AdditionalMetaData!.AddOrUpdate("Portrait", portraitUrl);
                    }
                }
                else if (tcolCounter == 4)
                {
                    var nameElm = elm.SelectNodes($"{elm.XPath}//b/a")?.FirstOrDefault();
                    if (nameElm == null) throw new Exception("The name element is missing");
                    if (nameElm.Attributes.Count > 0 &&
                        nameElm.Attributes.Any(a => a.Name == "href" && a.Value.HasValue()))
                    {
                        listOfName.Route = HttpUtility.UrlDecode(HtmlAgilityEx.DecodedInnerText(content: nameElm.Attributes["href"].Value, removeNewLine: false));
                        listOfName.Title = nameElm.DecodedInnerText(removeNewLine: true).Trim();
                    }
                    else throw new Exception("The first elment <a> does not have required details");

                    var spanContainerElm = elm.SelectNodes($"{elm.XPath}//span")?.FirstOrDefault();
                    if (spanContainerElm == null) throw new Exception("The span container element which has details about the person is missing");


                    var subElms = spanContainerElm.ChildNodes.Where(f => f.Name != "br").Skip(1).ToList();
                    if (subElms == null || subElms.Count < 2) throw new Exception("The span container element which has details about the person is missing");

                    listOfName.AdditionalMetaData.Add("Birth-Death", subElms[0].DecodedInnerText(removeNewLine: true).ReplaceMultiple("", "(", ")"));
                    listOfName.AdditionalMetaData.Add("Constituency", string.Join(" ", subElms.Skip(1).Select(f => f.DecodedInnerText(removeNewLine: true))));
                }
                tcolCounter++;
            }

            Console.WriteLine($"Extraction: {listOfName.Title} [{listOfName.Route}]");
            Console.WriteLine($"Details -> Birth-Death: {listOfName.AdditionalMetaData["Birth-Death"]}");
            Console.WriteLine($"Details -> Constituency: {listOfName.AdditionalMetaData["Constituency"]}");
            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine("");

            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Birth-Death");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Constituency");

            listOfName.Sequence = sequence++;
            return listOfName;
        }

        public List<WikiWhatToExtractModel> ExtractListTabularData_UnitedStates(HtmlDocument document, List<string>? tags)
        {
            List<WikiWhatToExtractModel> listOfNames = new List<WikiWhatToExtractModel>();
            sequence = 1;

            var tableData = document.DocumentNode.SelectNodes($"//table[contains(@class, 'wikitable')]//tbody/tr/th[contains(text(), 'Portrait')]//..//..//tr");
            foreach (var tableRow in tableData.Skip(1))
            {
                if (tableRow.ChildNodes.Count(f => f.Name == "td") + tableRow.ChildNodes.Count(f => f.Name == "th") < 7)
                {
                    continue;
                }
                var elements = tableRow.ChildNodes.Where(f => f.Name == "td" || f.Name == "th").ToArray();
                var extractedData = ExtractListTabularData_UnitedStates_Rows(elements);
                if (extractedData != null)
                {
                    extractedData.Tags = tags.DeepClone();
                    listOfNames.Add(extractedData);
                }
            }

            return listOfNames;
        }
        private WikiWhatToExtractModel? ExtractListTabularData_UnitedStates_Rows(HtmlNode[] elements)
        {
            var listOfName = new WikiWhatToExtractModel();

            int tcolCounter = 1;
            foreach (var elm in elements)
            {
                if (tcolCounter == 1)
                {
                    listOfName.AdditionalMetaData!.Add("No", elm.DecodedInnerText(removeNewLine: true).Trim());
                }
                else if (tcolCounter == 2)
                {
                    var portraitElm = elm.SelectNodes($"{elm.XPath}//img")?.FirstOrDefault();
                    if (portraitElm != null && portraitElm.Attributes.Count > 0 && portraitElm.Attributes.Any(f => f.Name == "src") &&
                        portraitElm.Attributes.FirstOrDefault(f => f.Name == "src")?.Value.HasValue() == true)
                    {
                        var portraitUrl = portraitElm.Attributes["src"].Value;
                        if (portraitUrl.StartsWith("http") == false)
                        {
                            portraitUrl = $"https:{(portraitUrl.StartsWith("//") ? "" : "//")}{portraitUrl}";
                        }
                        listOfName.AdditionalMetaData!.AddOrUpdate("Portrait", portraitUrl);
                    }
                }
                else if (tcolCounter == 3)
                {
                    var nameElm = elm.SelectNodes($"{elm.XPath}//b/a")?.FirstOrDefault();
                    if (nameElm == null) throw new Exception("The name element is missing");
                    if (nameElm.Attributes.Count > 0 &&
                        nameElm.Attributes.Any(a => a.Name == "href" && a.Value.HasValue()))
                    {
                        listOfName.Route = HttpUtility.UrlDecode(HtmlAgilityEx.DecodedInnerText(content: nameElm.Attributes["href"].Value, removeNewLine: false));
                        listOfName.Title = nameElm.DecodedInnerText(removeNewLine: true).Trim();
                    }
                    else throw new Exception("The first elment <a> does not have required details");

                    var spanContainerElm = elm.SelectNodes($"{elm.XPath}//span")?.FirstOrDefault();
                    if (spanContainerElm == null) throw new Exception("The span container element which has details about the person is missing");

                    listOfName.AdditionalMetaData.Add("Birth-Death", spanContainerElm.DecodedInnerText(removeNewLine: true).ReplaceMultiple("", "(", ")"));
                }
                else if (tcolCounter == 4)
                {
                    var term = elm.DecodedInnerText(removeNewLine: true).SplitAndTrim("–");
                    if (term.Count() != 2) throw new Exception("The split on the term did not result with right values");
                    listOfName.AdditionalMetaData.Add("Term", string.Join(" - ", term).ReplaceMultiple("", "(", ")"));
                }
                tcolCounter++;
            }

            Console.WriteLine($"Extraction: {listOfName.Title} [{listOfName.Route}]");
            Console.WriteLine($"Details -> Birth-Death: {listOfName.AdditionalMetaData["Birth-Death"]}");
            Console.WriteLine($"Details -> Term: {listOfName.AdditionalMetaData["Term"]}");
            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine("");

            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Birth-Death");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Term");

            listOfName.Sequence = sequence++;
            return listOfName;
        }

        public List<WikiWhatToExtractModel> ExtractListTabularData_UnitedKingdom(HtmlDocument document, List<string>? tags)
        {
            List<WikiWhatToExtractModel> listOfNames = new List<WikiWhatToExtractModel>();
            sequence = 1;

            var tableData = document.DocumentNode.SelectNodes($"//table[contains(@class, 'wikitable')][1]//tbody/tr/th[contains(text(), 'Portrait')]//..//..//tr");
            foreach (var tableRow in tableData.Skip(2))
            {
                if (tableRow.ChildNodes.Count(f => f.Name == "td") + tableRow.ChildNodes.Count(f => f.Name == "th") < 7)
                {
                    continue;
                }
                var elements = tableRow.ChildNodes.Where(f => f.Name == "td" || f.Name == "th").ToArray();
                var extractedData = ExtractListTabularData_UnitedKingdom_Rows(elements);
                if (extractedData != null)
                {
                    extractedData.Tags = tags.DeepClone();
                    listOfNames.Add(extractedData);
                }
            }
            return listOfNames;
        }
        private WikiWhatToExtractModel? ExtractListTabularData_UnitedKingdom_Rows(HtmlNode[] elements)
        {
            var listOfName = new WikiWhatToExtractModel();

            //int tcolCounter = 1;

            bool extractPortrait = false;
            bool extractPerson = false;

            foreach (var elm in elements)
            {
                if (extractPortrait == false)
                {
                    var portraitElm = elm.SelectNodes($"{elm.XPath}//img")?.FirstOrDefault();
                    if (portraitElm != null && portraitElm.Attributes.Count > 0 && portraitElm.Attributes.Any(f => f.Name == "src") &&
                        portraitElm.Attributes.FirstOrDefault(f => f.Name == "src")?.Value.HasValue() == true)
                    {
                        var portraitUrl = portraitElm.Attributes["src"].Value;
                        if (portraitUrl.StartsWith("http") == false)
                        {
                            portraitUrl = $"https:{(portraitUrl.StartsWith("//") ? "" : "//")}{portraitUrl}";
                        }
                        listOfName.AdditionalMetaData!.AddOrUpdate("Portrait", portraitUrl);
                        extractPortrait = true;
                    }
                }
                if (extractPerson == false)
                {
                    var personElm = elm.SelectNodes($"{elm.XPath}//b/a")?.FirstOrDefault() ??
                        elm.SelectNodes($"{elm.XPath}/a")?.FirstOrDefault();

                    if (personElm != null)
                    {
                        if (personElm == null) throw new Exception("The name element is missing");
                        if (personElm.Attributes.Count > 0 &&
                            personElm.Attributes.Any(a => a.Name == "href" && a.Value.HasValue()))
                        {
                            listOfName.Route = HttpUtility.UrlDecode(HtmlAgilityEx.DecodedInnerText(content: personElm.Attributes["href"].Value, removeNewLine: false));
                            listOfName.Title = personElm.DecodedInnerText(removeNewLine: true).Trim();
                        }
                        else throw new Exception("The first elment <a> does not have required details");

                        var spanContainerElm = elm.SelectNodes($"{elm.XPath}//span[string-length(text()) > 0]")?.FirstOrDefault();
                        if (spanContainerElm == null) throw new Exception("The span container element which has details about the person is missing");

                        var textRaw = spanContainerElm.DecodedInnerText(removeNewLine: true);
                        var birthDeathExtracted = textRaw.RegexMatchGroupValue("\\(([^)]*)\\)[^(]*$", 0);
                        var birthDeathParsed = birthDeathExtracted.RegexMatchGroupValue("\\((.*?)\\)", 0);

                        var term = birthDeathParsed.SplitAndTrim("–");
                        listOfName.AdditionalMetaData.Add("Birth-Death", string.Join(" - ", term).ReplaceMultiple("", "(", ")"));

                        textRaw = textRaw.Replace(birthDeathExtracted, "");
                        listOfName.AdditionalMetaData.Add("Office", textRaw);
                        extractPerson = true;
                    }
                }
            }

            Console.WriteLine($"Extraction: {listOfName.Title} [{listOfName.Route}]");
            Console.WriteLine($"Details -> Birth-Death: {listOfName.AdditionalMetaData["Birth-Death"]}");
            Console.WriteLine($"Details -> Term: {listOfName.AdditionalMetaData["Office"]}");
            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine("");

            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Birth-Death");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Office");

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
