using HtmlAgilityPack;
using Pj.Library;
using System.Globalization;
using System.Text.RegularExpressions;
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

            var tableData = document.DocumentNode.SelectNodes($"//table[contains(@class, 'wikitable')]");

            foreach (var table in tableData)
            {
                var rows = table.SelectNodes(".//tr");

                foreach (var row in rows)
                {
                    var cells = row.SelectNodes(".//td|.//th")?.ToArray();
                    if (cells.All(f => f.Name == "th")) continue;
                    if (cells != null)
                    {
                        if (cells.Count() < 6)
                        {
                            continue;
                        }
                        var extractedData = ExtractListTabularData_Australia_Rows(cells);
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
        private WikiWhatToExtractModel? ExtractListTabularData_Australia_Rows(HtmlNode[] elements)
        {
            var listOfName = new WikiWhatToExtractModel();

            int tColCounter = 1;

            foreach (var elm in elements)
            {
                if (tColCounter == 2)
                {
                    Common_Portrait_Extract(elm, listOfName);
                }
                if (tColCounter == 3)
                {
                    Common_PersonDetail_Extract(elm, listOfName, titleRemoveInnerSpan: false, extractBirthDeath: false);
                    Common_Complex_BirthDeath(elm, listOfName);
                }
                if (tColCounter == 5)
                {
                    Common_DateType01_Extract(elm, listOfName, "Took office", null, removeSpecialChars: true);
                }
                if (tColCounter == 6)
                {
                    Common_DateType01_Extract(elm, listOfName, "Left office", new[] { "Incumbent" }, removeSpecialChars: true);
                }
                if (tColCounter == 7)
                {
                    Common_SimpleDataType01_Extract(elm, listOfName, "Time in office", removeSpecialChars: false);
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

            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Birth-Death");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Took office");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Left office");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Time in office");

            listOfName.Sequence = sequence++;
            return listOfName;
        }

        public List<WikiWhatToExtractModel> ExtractListTabularData_India(HtmlDocument document, List<string>? tags)
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
                        var extractedData = ExtractListTabularData_India_Rows(cells);
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
        private WikiWhatToExtractModel? ExtractListTabularData_India_Rows(HtmlNode[] elements)
        {
            var listOfName = new WikiWhatToExtractModel();

            int tColCounter = 1;

            foreach (var elm in elements)
            {
                if (tColCounter == 2)
                {
                    Common_Portrait_Extract(elm, listOfName);
                }
                if (tColCounter == 3)
                {
                    Common_PersonDetail_Extract(elm, listOfName, titleRemoveInnerSpan: false, extractBirthDeath: false);
                    Common_Complex_BirthDeath(elm, listOfName);
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

            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Birth-Death");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Took office");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Left office");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Time in office");

            listOfName.Sequence = sequence++;
            return listOfName;
        }

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

            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Birth-Death");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Took office");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Left office");

            listOfName.Sequence = sequence++;
            return listOfName;
        }

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

            int tcolCounter = 1;

            foreach (var elm in elements)
            {
                if (tcolCounter == 2)
                {
                    Common_Portrait_Extract(elm, listOfName);
                }
                if (tcolCounter == 3)
                {
                    Common_PersonDetail_Extract(elm, listOfName, titleRemoveInnerSpan: false, extractBirthDeath: true);
                }
                if (tcolCounter == 4)
                {
                    Common_DateType01_Extract(elm, listOfName, "Took office", null, removeSpecialChars: false);
                }
                if (tcolCounter == 5)
                {
                    Common_DateType01_Extract(elm, listOfName, "Left office", new[] { "incumbent" }, removeSpecialChars: false);
                }
                if (tcolCounter == 8)
                {
                    Common_SimpleDataType01_Extract(elm, listOfName, "Political party", removeSpecialChars: false);
                }
                if (tcolCounter == 9)
                {
                    Common_SimpleDataType01_Extract(elm, listOfName, "Riding", removeSpecialChars: false);
                }
                if (tcolCounter == 10)
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

            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Birth-Death");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Took office");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Left office");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Political party");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Riding");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Cabinet");

            listOfName.Sequence = sequence++;
            return listOfName;
        }

        public List<WikiWhatToExtractModel> ExtractListTabularData_Germany(HtmlDocument document, List<string>? tags)
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
                        var extractedData = ExtractListTabularData_Germany_Rows(cells);
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
        private WikiWhatToExtractModel? ExtractListTabularData_Germany_Rows(HtmlNode[] elements)
        {
            var listOfName = new WikiWhatToExtractModel();

            int tColCounter = 1;

            foreach (var elm in elements)
            {
                if (tColCounter == 1)
                {
                    Common_Portrait_Extract(elm, listOfName);
                }
                if (tColCounter == 2)
                {
                    Common_PersonDetail_Extract(elm, listOfName, titleRemoveInnerSpan: false, extractBirthDeath: true);
                }
                if (tColCounter == 3)
                {
                    Common_DateType01_Extract(elm, listOfName, "Took office", null, removeSpecialChars: true);
                }
                if (tColCounter == 4)
                {
                    Common_DateType01_Extract(elm, listOfName, "Left office", new[] { "Incumbent" }, removeSpecialChars: true);
                }
                if (tColCounter == 5)
                {
                    Common_SimpleDataType01_Extract(elm, listOfName, "Time in office", removeSpecialChars: false);
                }
                if (tColCounter == 6 || tColCounter == 7)
                {
                    Common_SimpleDataType01_Extract(elm, listOfName, "Party", removeSpecialChars: false);
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

            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Birth-Death");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Took office");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Left office");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Time in office");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Party");

            listOfName.Sequence = sequence++;
            return listOfName;
        }

        public List<WikiWhatToExtractModel> ExtractListTabularData_France(HtmlDocument document, List<string>? tags)
        {
            List<WikiWhatToExtractModel> listOfNames = new List<WikiWhatToExtractModel>();
            sequence = 1;

            var tableData = document.DocumentNode.SelectNodes($"//table[contains(@class, 'wikitable')]");

            foreach (var table in tableData.Skip(2))
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
                        var extractedData = ExtractListTabularData_France_Rows(cells);
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
        private WikiWhatToExtractModel? ExtractListTabularData_France_Rows(HtmlNode[] elements)
        {
            var listOfName = new WikiWhatToExtractModel();

            int tColCounter = 1;

            foreach (var elm in elements)
            {
                if (tColCounter == 1)
                {
                    Common_Portrait_Extract(elm, listOfName);
                }
                if (tColCounter == 2)
                {
                    Common_PersonDetail_Extract(elm, listOfName, titleRemoveInnerSpan: false, extractBirthDeath: true);
                }
                if (tColCounter == 3)
                {
                    Common_DateType01_Extract(elm, listOfName, "Took office", null, removeSpecialChars: true);
                }
                if (tColCounter == 4)
                {
                    Common_DateType01_Extract(elm, listOfName, "Left office", new[] { "Incumbent" }, removeSpecialChars: true);
                }
                if (tColCounter == 5)
                {
                    Common_SimpleDataType01_Extract(elm, listOfName, "Time in office", removeSpecialChars: false);
                }
                if (tColCounter == 6 || tColCounter == 7)
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

            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Birth-Death");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Took office");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Left office");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Time in office");
            //ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Political Party");

            listOfName.Sequence = sequence++;
            return listOfName;
        }

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

            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Birth-Death");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Took office");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Left office");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Time in office");
            //ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Political Party");

            listOfName.Sequence = sequence++;
            return listOfName;
        }

        public List<WikiWhatToExtractModel> ExtractListTabularData_Japan(HtmlDocument document, List<string>? tags)
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
                        var extractedData = ExtractListTabularData_Japan_Rows(cells);
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
        private WikiWhatToExtractModel? ExtractListTabularData_Japan_Rows(HtmlNode[] elements)
        {
            var listOfName = new WikiWhatToExtractModel();

            int tColCounter = 1;

            foreach (var elm in elements)
            {
                if (tColCounter == 2)
                {
                    Common_Portrait_Extract(elm, listOfName);
                }
                if (tColCounter == 3)
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
                if (tColCounter == 8)
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

            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Birth-Death");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Took office");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Left office");
            ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Time in office");
            //ValidateAdditionalMetaData(listOfName.AdditionalMetaData, "Political Party");

            listOfName.Sequence = sequence++;
            return listOfName;
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
    }
}
