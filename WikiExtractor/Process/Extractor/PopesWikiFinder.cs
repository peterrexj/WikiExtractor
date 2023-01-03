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
    public class PopesWikiFinder
    {
        private const string _metadata_PontiffNumber = "Pontiff number";
        private const string _metadata_Pontificate = "Pontificate";
        private const string _metadata_EnglishName = "English Name";
        private const string _metadata_DateAndPlaceOfBirth = "Date & Place Of Birth";
        private const string _metadata_AgeAtStartEndOfPapacy  = "Age at start/nend of papacy";
        private const string _metadata_Notes = "Notes";

        public List<WikiWhatToExtractModel> ExtractByCenturyFromTable(HtmlDocument document, List<string>? tags)
        {
            List<WikiWhatToExtractModel> listOfNames = new List<WikiWhatToExtractModel>();

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




            var temp = document.DocumentNode.SelectNodes("//table/caption[contains(text(), 'Popes of the 1st century')]//..//tbody/tr").Skip(1);

            int counter = 1;
            bool hasExtracted = false;
            foreach (var item in temp)
            {
                var listOfName = new WikiWhatToExtractModel();

                foreach (var column in item.ChildNodes.Where(f => f.Name == "td"))
                {
                    if (counter == 1)
                    {
                        listOfName.AdditionalMetaData.Add(_metadata_PontiffNumber, "");
                    }
                    else if (counter == 2)
                    {

                    }
                    else if (counter == 3)
                    {

                    }
                    else if (counter == 4 )
                    {

                    }
                    else if (counter == 5)
                    {

                    }
                    else if (counter == 6)
                    {

                    }
                    counter++;
                }
                hasExtracted = false;

                if (item.ChildNodes.Any(f => f.Name == "td"))
                {
                    var cell = item.ChildNodes.FirstOrDefault(f => f.Name == "td");
                    if (counter == 3)
                    {

                    }


                    if (cell.ChildNodes.Any(f => f.Name == "a"))
                    {
                        var anchor = cell.ChildNodes.FirstOrDefault(f => f.Name == "a");
                        if (anchor != null && anchor.Attributes.Count > 0)
                        {
                            if (anchor.Attributes.Any(a => a.Name == "href" && a.Value.HasValue()) &&
                               anchor.Attributes.Any(a => a.Name == "title" && a.Value.HasValue()))
                            {
                                var route = HttpUtility.UrlDecode(HtmlAgilityEx.DecodedInnerText(content: anchor.Attributes["href"].Value, removeNewLine: false));
                                var title = HtmlAgilityEx.DecodedInnerText(anchor.Attributes["title"].Value, false);
                                if (!listOfNames.Any(f => f.Route == route))
                                {
                                    listOfNames.Add(new WikiWhatToExtractModel { Route = route, Title = title, Tags = tags, Sequence = ++counter });
                                }
                                hasExtracted = true;
                            }
                        }
                    }
                }
            }
            return listOfNames;
        }
    }
}
