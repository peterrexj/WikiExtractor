using Aspose.Cells;
using HtmlAgilityPack;
using Pj.Library;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WikiExtractor.Exts;
using WikiExtractor.Models;

namespace WikiExtractor.Process.Extractor
{
    public class PopesWikiFinder
    {
        private const string _metadata_PontiffNumber = "Pontiff number";
        private const string _metadata_Pontificate = "Pontificate";
        private const string _metadata_EnglishName = "English Name";
        private const string _metadata_LatinName = "Latin Name";
        private const string _metadata_DateAndPlaceOfBirth = "Date & Place Of Birth";
        private const string _metadata_AgeAtStartEndOfPapacy = "Age at start/nend of papacy";
        private const string _metadata_Notes = "Notes";
        private const string _metadata_PersonalName = "Personal Name";



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

            var tableData = document.DocumentNode.SelectNodes($"//table/caption[contains(text(), '{tableFinderText}')]//..//tbody/tr").Skip(1);

            int elePos_Poitiff = 0;
            int elePos_Pontificate = 0;
            int elePos_Potrait = 0;
            int elePos_PersonalName = 0;
            int elePos_Names = 0;
            int elePos_DateAndPlaceOfBirth = 0;
            int elePos_AgeAtStartEndOfPapacy = 0;
            int elePos_Notes = 0;

            if (hasPortrait && hasPersonalName)
            {
                elePos_Poitiff = 0;
                elePos_Pontificate = 1;
                elePos_Potrait = 2;
                elePos_Names = 3;
                elePos_PersonalName = 4;
                elePos_DateAndPlaceOfBirth = 5;
                elePos_AgeAtStartEndOfPapacy = 6;
                elePos_Notes = 7;
            }
            else
            {
                elePos_Poitiff = 0;
                elePos_Pontificate = 1;
                elePos_Names = 2;
                elePos_DateAndPlaceOfBirth = 3;
                elePos_AgeAtStartEndOfPapacy = 4;
                elePos_Notes = 5;
            }

            foreach (var item in tableData)
            {

                var listOfName = new WikiWhatToExtractModel();
                int counter = 1;
                var elements = item.ChildNodes.Where(f => f.Name == "td").ToArray();
                listOfName.AdditionalMetaData!.Add(_metadata_PontiffNumber, elements[elePos_Poitiff].DecodedInnerText(removeNewLine: true).Trim());
                listOfName.AdditionalMetaData!.Add(_metadata_Pontificate, elements[elePos_Pontificate].DecodedInnerText(removeNewLine: true).Trim());
                listOfName.AdditionalMetaData!.Add(_metadata_DateAndPlaceOfBirth, elements[elePos_DateAndPlaceOfBirth].DecodedInnerText(removeNewLine: true).Trim());
                listOfName.AdditionalMetaData!.Add(_metadata_AgeAtStartEndOfPapacy, elements[elePos_AgeAtStartEndOfPapacy].DecodedInnerText(removeNewLine: true).Trim());
                listOfName.AdditionalMetaData!.Add(_metadata_Notes, elements[elePos_Notes].DecodedInnerText(removeNewLine: false).Trim());

                if (hasPersonalName)
                {
                    listOfName.AdditionalMetaData!.Add(_metadata_PersonalName, elements[elePos_PersonalName].DecodedInnerText(removeNewLine: true).Trim());
                }
                if (hasPortrait)
                {

                }

                //Names with HyperLinks
                if (elements[elePos_Names].ChildNodes.Count >= 4)
                {
                    var engName = elements[elePos_Names].ChildNodes.FirstOrDefault(f => f.Name == "b" || f.Name == "i" || f.Name == "a")?.DecodedInnerText(removeNewLine: true).Trim();
                    //if (engName.IsEmpty())
                    //{
                    //    engName = elements[elePos_Names].ChildNodes.FirstOrDefault(f => f.Name == "a" || f.Name == "i")?.DecodedInnerText(removeNewLine: true).Trim();
                    //}
                    listOfName.AdditionalMetaData!.AddOrUpdate(_metadata_EnglishName, engName);
                    listOfName.AdditionalMetaData!.AddOrUpdate(_metadata_LatinName, elements[elePos_Names].ChildNodes.FirstOrDefault(f => f.Name == "span")?.DecodedInnerText(removeNewLine: true).Trim());
                    if (elements[elePos_Names].ChildNodes.FirstOrDefault(f => f.Name == "b" || f.Name == "i")?.ChildNodes.Where(f => f.Name == "a").Count() == 1)
                    {
                        var anchor = elements[elePos_Names].ChildNodes.FirstOrDefault(f => f.Name == "b" || f.Name == "i")?.ChildNodes.FirstOrDefault(f => f.Name == "a");
                        if (anchor != null && anchor.Attributes.Count > 0)
                        {
                            if (anchor.Attributes.Any(a => a.Name == "href" && a.Value.HasValue()))
                            {
                                listOfName.Route = HttpUtility.UrlDecode(HtmlAgilityEx.DecodedInnerText(content: anchor.Attributes["href"].Value, removeNewLine: false));
                            }
                        }
                    }
                    else if (elements[elePos_Names].ChildNodes.Where(f => f.Name == "a").Count() == 1)
                    {
                        var anchor = elements[elePos_Names].ChildNodes.FirstOrDefault(f => f.Name == "a");
                        if (anchor != null && anchor.Attributes.Count > 0)
                        {
                            if (anchor.Attributes.Any(a => a.Name == "href" && a.Value.HasValue()))
                            {
                                listOfName.Route = HttpUtility.UrlDecode(HtmlAgilityEx.DecodedInnerText(content: anchor.Attributes["href"].Value, removeNewLine: false));
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("There is more than 1 <a>, take a closer look");
                    }
                }
                else
                {
                    throw new Exception("There is more nodes than expected, take a closer look");
                }
                

                

                //foreach (var column in item.ChildNodes.Where(f => f.Name == "td"))
                //{

                //    if (counter == 1)
                //    {
                //        listOfName.AdditionalMetaData!.Add(_metadata_PontiffNumber, column.DecodedInnerText(removeNewLine: true).Trim());
                //    }
                //    else if (counter == 2)
                //    {
                //        listOfName.AdditionalMetaData!.Add(_metadata_Pontificate, column.DecodedInnerText(removeNewLine: true).Trim());
                //    }
                //    else if (counter == 3)
                //    {
                //        if (column.ChildNodes.Count >= 4)
                //        {
                //            listOfName.AdditionalMetaData!.AddOrUpdate(_metadata_EnglishName, column.ChildNodes.FirstOrDefault(f => f.Name == "b" || f.Name == "i")?.DecodedInnerText(removeNewLine: true).Trim());
                //            listOfName.AdditionalMetaData!.AddOrUpdate(_metadata_LatinName, column.ChildNodes.FirstOrDefault(f => f.Name == "span")?.DecodedInnerText(removeNewLine: true).Trim());
                //            if (column.ChildNodes.FirstOrDefault(f => f.Name == "b" || f.Name == "i")?.ChildNodes.Where(f => f.Name == "a").Count() == 1)
                //            {
                //                var anchor = column.ChildNodes.FirstOrDefault(f => f.Name == "b" || f.Name == "i" )?.ChildNodes.FirstOrDefault(f => f.Name == "a");
                //                if (anchor != null && anchor.Attributes.Count > 0)
                //                {
                //                    if (anchor.Attributes.Any(a => a.Name == "href" && a.Value.HasValue()))
                //                    {
                //                        listOfName.Route = HttpUtility.UrlDecode(HtmlAgilityEx.DecodedInnerText(content: anchor.Attributes["href"].Value, removeNewLine: false));
                //                    }
                //                }
                //            }
                //            else
                //            {
                //                throw new Exception("There is more than 1 <a>, take a closer look");
                //            }
                //        }
                //        else
                //        {
                //            throw new Exception("There is more nodes than expected, take a closer look");
                //        }
                //    }
                //    else if (counter == 4)
                //    {
                //        listOfName.AdditionalMetaData!.Add(_metadata_DateAndPlaceOfBirth, column.DecodedInnerText(removeNewLine: true).Trim());
                //    }
                //    else if (counter == 5)
                //    {
                //        listOfName.AdditionalMetaData!.Add(_metadata_AgeAtStartEndOfPapacy, column.DecodedInnerText(removeNewLine: true).Trim());
                //    }
                //    else if (counter == 6)
                //    {
                //        listOfName.AdditionalMetaData!.Add(_metadata_Notes, column.DecodedInnerText(removeNewLine: false).Trim());
                //    }
                //    counter++;
                //}
                if (listOfName.Route.IsEmpty())
                {
                    throw new Exception("Did not extract the route!");
                }
                ValidateAdditionalMetaData(listOfName.AdditionalMetaData, _metadata_PontiffNumber);
                ValidateAdditionalMetaData(listOfName.AdditionalMetaData, _metadata_Pontificate);
                ValidateAdditionalMetaData(listOfName.AdditionalMetaData, _metadata_EnglishName);
                ValidateAdditionalMetaData(listOfName.AdditionalMetaData, _metadata_LatinName);
                ValidateAdditionalMetaData(listOfName.AdditionalMetaData, _metadata_DateAndPlaceOfBirth);
                ValidateAdditionalMetaData(listOfName.AdditionalMetaData, _metadata_AgeAtStartEndOfPapacy);
                ValidateAdditionalMetaData(listOfName.AdditionalMetaData, _metadata_Notes);
                if (hasPersonalName)
                {
                    ValidateAdditionalMetaData(listOfName.AdditionalMetaData, _metadata_PersonalName);
                }
                listOfName.Title = listOfName.AdditionalMetaData[_metadata_EnglishName];
                listOfName.Tags = tags;
                listOfName.Sequence = sequence++;

                listOfNames.Add(listOfName);
            }
            return listOfNames;
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
