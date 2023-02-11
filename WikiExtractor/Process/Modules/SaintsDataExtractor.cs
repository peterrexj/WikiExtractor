using Pj.Library;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WikiExtractor.DbModels;
using WikiExtractor.Exts;
using WikiExtractor.Models;
using WikiExtractor.Process.Extractor;
using WikiExtractor.Repository;

namespace WikiExtractor.Process.Modules
{
    public class SaintsDataExtractor : DataExtractorBase
    {
        protected SaintsWikiExtractionToStore? toStore = null;
        public SaintsDataExtractor() : base("Saints", "WikiStoreSaints.db") { }

        protected override void Initialize(bool doClean)
        {
            base.Initialize(doClean);
            toStore = new SaintsWikiExtractionToStore();
        }
        public void ExtractData()
        {
            Initialize(true);
            
            //Adding Menu Items
            int menuItemCounter = 0;
            wikiAppController!.AddMenuItem("All Saints", "All", "Saints", menuItemCounter++);
            wikiAppController.AddMenuItem("Canonized by Pope Francis", "Canonized by Pope Francis", "Canonized by Pope Francis", menuItemCounter++);
            wikiAppController.AddMenuItem("Canonized by Pope John Paul II", "Canonized by Pope John Paul II", "Canonized by Pope John Paul II", menuItemCounter++);
            wikiAppController.AddMenuItem("Canonized by Pope Leo XIII", "Canonized by Pope Leo XIII", "Canonized by Pope Leo XIII", menuItemCounter++);
            wikiAppController.AddMenuItem("Canonized by Pope Pius XI", "Canonized by Pope Pius XI", "Canonized by Pope Pius XI", menuItemCounter++);
            wikiAppController.AddMenuItem("Canonized by Pope Pius XII", "Canonized by Pope Pius XII", "Canonized by Pope Pius XII", menuItemCounter++);
            wikiAppController.AddMenuItem("Canonized by Pope John XXIII", "Canonized by Pope John XXIII", "Canonized by Pope John XXIII", menuItemCounter++);
            wikiAppController.AddMenuItem("Canonized by Pope Paul VI", "Canonized by Pope Paul VI", "Canonized by Pope Paul VI", menuItemCounter++);
            wikiAppController.AddMenuItem("Patron Saints", "Patron Saints", "Patron Saints", menuItemCounter++);
            wikiAppController.AddMenuItem("Beatified", "Beatified", "Beatified", menuItemCounter++);
            wikiAppController.AddMenuItem("Pope", "By Pope", "Pope", menuItemCounter++);
            wikiAppController.AddMenuItem("21th Century", "21th Century", "21th Century Saints", menuItemCounter++);
            wikiAppController.AddMenuItem("20th Century", "20th Century", "20th Century Saints", menuItemCounter++);
            wikiAppController.AddMenuItem("19th Century", "19th Century", "19th Century Saints", menuItemCounter++);
            wikiAppController.AddMenuItem("18th Century", "18th Century", "18th Century Saints", menuItemCounter++);
            wikiAppController.AddMenuItem("17th Century", "17th Century", "17th Century Saints", menuItemCounter++);
            wikiAppController.AddMenuItem("16th Century", "16th Century", "16th Century Saints", menuItemCounter++);
            wikiAppController.AddMenuItem("15th Century", "15th Century", "15th Century Saints", menuItemCounter++);
            wikiAppController.AddMenuItem("14th Century", "14th Century", "14th Century Saints", menuItemCounter++);
            wikiAppController.AddMenuItem("13th Century", "13th Century", "13th Century Saints", menuItemCounter++);
            wikiAppController.AddMenuItem("12th Century", "12th Century", "12th Century Saints", menuItemCounter++);
            wikiAppController.AddMenuItem("11th Century", "11th Century", "11th Century Saints", menuItemCounter++);
            wikiAppController.AddMenuItem("10th Century", "10th Century", "10th Century Saints", menuItemCounter++);
            wikiAppController.AddMenuItem("9th Century", "9th Century", "9th Century Saints", menuItemCounter++);
            wikiAppController.AddMenuItem("8th Century", "8th Century", "8th Century Saints", menuItemCounter++);
            wikiAppController.AddMenuItem("7th Century", "7th Century", "7th Century Saints", menuItemCounter++);
            wikiAppController.AddMenuItem("6th Century", "6th Century", "6th Century Saints", menuItemCounter++);
            wikiAppController.AddMenuItem("5th Century", "5th Century", "5th Century Saints", menuItemCounter++);
            wikiAppController.AddMenuItem("4th Century", "4th Century", "4th Century Saints", menuItemCounter++);
            wikiAppController.AddMenuItem("3rd Century", "3rd Century", "3rd Century Saints", menuItemCounter++);
            wikiAppController.AddMenuItem("2nd Century", "2nd Century", "2nd Century Saints", menuItemCounter++);
            wikiAppController.AddMenuItem("1st Century", "1st Century", "1st Century Saints", menuItemCounter++);

            EnablePrimaryMetadataContent();

            //Extracting data based on tags
            var listOfSaintsFromLocalUrlFile01 = toStore!.GenericLoadUrlFile(
                IoHelper.CombinePath(PjUtility.Runtime.ExecutingFolder, "Resources", "Saints_AddtionalLinks.txt"), new List<string> { "All" });

            var listOfSaintsByEachPope01 = toStore.ExtractByEachPopeListData("/wiki/List_of_saints_canonized_by_Pope_Benedict_XVI", new List<string> { "All", "Pope Benedict XVI" });
            var listOfSaintsByEachPope02 = toStore.ExtractByEachPopeListData("/wiki/List_of_saints_canonized_by_Pope_John_Paul_II", new List<string> { "All", "Canonized by Pope John Paul II" });
            var listOfSaintsByEachPope03 = toStore.ExtractByEachPopeListData("/wiki/List_of_saints_canonized_by_Pope_Leo_XIII", new List<string> { "All", "Canonized by Pope Leo XIII" });
            var listOfSaintsByEachPope04 = toStore.ExtractByEachPopeListData("/wiki/List_of_saints_canonized_by_Pope_Pius_XI", new List<string> { "All", "Canonized by Pope Pius XI" });
            var listOfSaintsByEachPope05 = toStore.ExtractByEachPopeListData("/wiki/List_of_saints_canonized_by_Pope_Pius_XII", new List<string> { "All", "Canonized by Pope Pius XII" });
            var listOfSaintsByEachPope06 = toStore.ExtractByEachPopeListData("/wiki/List_of_saints_canonized_by_Pope_John_XXIII", new List<string> { "All", "Canonized by Pope John XXIII" });
            var listOfSaintsByEachPope07 = toStore.ExtractByEachPopeListData("/wiki/List_of_saints_canonized_by_Pope_Paul_VI", new List<string> { "All", "Canonized by Pope Paul VI" });
            var listOfSaintsByEachPope08 = toStore.ExtractByEachPopeListData("/wiki/List_of_saints_canonized_by_Pope_Francis", new List<string> { "All", "Canonized by Pope Francis" });

            var listOfSaintsByCentury1 = toStore.ExtractByCentury("/wiki/Chronological_list_of_saints_in_the_1st_century", new List<string> { "All", "1st Century" });
            var listOfSaintsByCentury2 = toStore.ExtractByCentury("/wiki/Chronological_list_of_saints_in_the_2nd_century", new List<string> { "All", "2nd Century" });
            var listOfSaintsByCentury3 = toStore.ExtractByCentury("/wiki/Chronological_list_of_saints_in_the_3rd_century", new List<string> { "All", "3rd Century" });
            var listOfSaintsByCentury4 = toStore.ExtractByCentury("/wiki/Chronological_list_of_saints_in_the_4th_century", new List<string> { "All", "4th Century" });
            var listOfSaintsByCentury5 = toStore.ExtractByCentury("/wiki/Chronological_list_of_saints_in_the_5th_century", new List<string> { "All", "5th Century" });
            var listOfSaintsByCentury6 = toStore.ExtractByCentury("/wiki/Chronological_list_of_saints_in_the_6th_century", new List<string> { "All", "6th Century" });
            var listOfSaintsByCentury7 = toStore.ExtractByCentury("/wiki/Chronological_list_of_saints_in_the_7th_century", new List<string> { "All", "7th Century" });
            var listOfSaintsByCentury8 = toStore.ExtractByCentury("/wiki/Chronological_list_of_saints_in_the_8th_century", new List<string> { "All", "8th Century" });
            var listOfSaintsByCentury9 = toStore.ExtractByCentury("/wiki/Chronological_list_of_saints_in_the_9th_century", new List<string> { "All", "9th Century" });
            var listOfSaintsByCentury10 = toStore.ExtractByCentury("/wiki/Chronological_list_of_saints_in_the_10th_century", new List<string> { "All", "10th Century" });
            var listOfSaintsByCentury11 = toStore.ExtractByCentury("/wiki/Chronological_list_of_saints_and_blesseds_in_the_11th_century", new List<string> { "All", "11th Century" });
            var listOfSaintsByCentury12 = toStore.ExtractByCentury("/wiki/Chronological_list_of_saints_and_blesseds_in_the_12th_century", new List<string> { "All", "12th Century" });
            var listOfSaintsByCentury13 = toStore.ExtractByCentury("/wiki/Chronological_list_of_saints_and_blesseds_in_the_13th_century", new List<string> { "All", "13th Century" });
            var listOfSaintsByCentury14 = toStore.ExtractByCentury("/wiki/Chronological_list_of_saints_and_blesseds_in_the_14th_century", new List<string> { "All", "14th Century" });
            var listOfSaintsByCentury15 = toStore.ExtractByCentury("/wiki/Chronological_list_of_saints_and_blesseds_in_the_15th_century", new List<string> { "All", "15th Century" });
            var listOfSaintsByCentury16 = toStore.ExtractByCentury("/wiki/Chronological_list_of_saints_and_blesseds_in_the_16th_century", new List<string> { "All", "16th Century" });
            var listOfSaintsByCentury17 = toStore.ExtractByCentury("/wiki/Chronological_list_of_saints_and_blesseds_in_the_17th_century", new List<string> { "All", "17th Century" });
            var listOfSaintsByCentury18 = toStore.ExtractByCentury("/wiki/Chronological_list_of_saints_and_blesseds_in_the_18th_century", new List<string> { "All", "18th Century" });
            var listOfSaintsByCentury19 = toStore.ExtractByCentury("/wiki/Chronological_list_of_saints_and_blesseds_in_the_19th_century", new List<string> { "All", "19th Century" });
            var listOfSaintsByCentury20 = toStore.ExtractByCentury("/wiki/Chronological_list_of_saints_and_blesseds_in_the_20th_century", new List<string> { "All", "20th Century" });
            var listOfSaintsByCentury21 = toStore.ExtractByCentury("/wiki/Chronological_list_of_saints_and_blesseds_in_the_21st_century", new List<string> { "All", "21th Century" });

            var listOfSaintsByAllPope = toStore.ExtractByAllPopeListData("/wiki/List_of_saints_by_pope", new List<string> { "All", "By Pope" });
            var listOfPatronSaints = toStore.ExtractPatronSaintsListData("/wiki/List_of_patron_saints_by_occupation_and_activity", new List<string> { "All", "Patron Saints" });
            var listOfSaints = toStore.ExtractListTabularData("/wiki/List_of_saints", new List<string> { "All" });
            var listOfBeatified = toStore.ExtractListTabularData("/wiki/List_of_beatified_people", new List<string> { "All", "Beatified" });


            var saintsCollection =
                listOfSaintsByCentury21
                .Union(listOfSaintsByCentury20)
                .Union(listOfSaintsByCentury19)
                .Union(listOfSaintsByCentury18)
                .Union(listOfSaintsByCentury17)
                .Union(listOfSaintsByCentury16)
                .Union(listOfSaintsByCentury15)
                .Union(listOfSaintsByCentury14)
                .Union(listOfSaintsByCentury13)
                .Union(listOfSaintsByCentury12)
                .Union(listOfSaintsByCentury11)
                .Union(listOfSaintsByCentury10)
                .Union(listOfSaintsByCentury9)
                .Union(listOfSaintsByCentury8)
                .Union(listOfSaintsByCentury7)
                .Union(listOfSaintsByCentury6)
                .Union(listOfSaintsByCentury5)
                .Union(listOfSaintsByCentury4)
                .Union(listOfSaintsByCentury3)
                .Union(listOfSaintsByCentury2)
                .Union(listOfSaintsByCentury1)
                .Union(listOfSaints)
                .Union(listOfSaintsFromLocalUrlFile01)
                .Union(listOfSaintsByAllPope)
                .Union(listOfPatronSaints)
                .Union(listOfBeatified)
                .Union(listOfSaintsByEachPope01).Union(listOfSaintsByEachPope02)
                .Union(listOfSaintsByEachPope03).Union(listOfSaintsByEachPope04)
                .Union(listOfSaintsByEachPope05).Union(listOfSaintsByEachPope06)
                .Union(listOfSaintsByEachPope07).Union(listOfSaintsByEachPope08)
                //.Take(100)
                .ToList()
                .WithDefaultFilters();

            int totalCount = saintsCollection.Count;
            int currentIndex = 1;

            ConcurrentBag<Tuple<WikiPageModel, List<MetaDataModel>, WikiWhatToExtractModel>> bag = new();

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = ProcessConstants.UseCache ? 5 : 1
            };
            
            Parallel.ForEach(saintsCollection, parallelOptions, saint =>
            {
                try
                {
                    //Thread.Sleep(1000);
                    lock (_lock)
                    {
                        Console.WriteLine($"[{currentIndex}/{totalCount}] [{(int)(((decimal)currentIndex / (decimal)totalCount) * 100)}%] Saints [{saint.Title}]: {saint.Route}");
                        currentIndex = currentIndex + 1;
                    }
                    var rawData = toStore.SinglePageContentExtract(saint);
                    bag.Add(new Tuple<WikiPageModel, List<MetaDataModel>, WikiWhatToExtractModel>(rawData.Item1, rawData.Item2, saint));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });


            //foreach (var saints in saintsCollection)
            Parallel.ForEach(saintsCollection, new ParallelOptions { MaxDegreeOfParallelism = 5 }, saint =>
            {
                var bagItem = bag.FirstOrDefault(f => f.Item3.Id == saint.Id);
                if (bagItem == null || bagItem.Item1 == null || bagItem.Item2 == null || bagItem.Item3 == null)
                {
                    throw new Exception("Bag item cannot be mapped, this could be due to the extraction failure");
                }
            });

            currentIndex = 1;
            //foreach (var saints in saintsCollection)
            Parallel.ForEach(saintsCollection, new ParallelOptions { MaxDegreeOfParallelism = 1 }, saint =>
            {
                try
                {
                    var bagItem = bag.FirstOrDefault(f => f.Item3.Id == saint.Id);
                    toStore.SinglePageContentStore(bagItem.Item1, bagItem.Item2, bagItem.Item3);
                    Console.WriteLine($"[{currentIndex}/{totalCount}] [{(int)(((decimal)currentIndex / (decimal)totalCount) * 100)}%] Saints [{saint.Title}]: {saint.Route}");
                    //Thread.Sleep(1000);
                    currentIndex = currentIndex + 1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });

            //Clean the data
            CleanDataWithDump();
        }

        public void EnablePrimaryMetadataContent()
        {
            if (wikiAppController == null)
            {
                Initialize(false);
            }
            wikiAppController!.EnableWithPrimaryMetadataContent(new List<string>
            {
                "Born",
                "Died",
                "Feast",
                "Feast day",
                "Beatified",
                "Major shrine",
                "Church",
                "Buried",
                "Venerated in",
                "Canonized",
                "Predecessor"
            }, 5);
            
        }

        private List<WikiDataCleanerModel> PrepareDumpData()
        {
            List<string> ignoredItems = new();

            var data = wikiAppController?.UpdateTags(wikiAppController?.GetListOfWikiItems(new List<string> { "All" }).ToList());

            var ignoreListFile = IoHelper.CombinePath(PjUtility.Runtime.ExecutingFolder, "Resources", "Saints_IgnoreList.txt");
            if (File.Exists(ignoreListFile))
            {
                ignoredItems.AddRange(File.ReadLines(ignoreListFile).Select(f => f.Trim()));
            }

            var grpNameFlatWrite = data!
                .GroupBy(f => f.Name)
                .Select(f => new { Name = f.Key, Links = f.Where(g => g.Name == f.Key).ToList() })
                .Where(f => f.Links.Count > 1)
                .SelectMany(f => f.Links)
                .ToList();

            var grpPathFlatWrite = data!
                .GroupBy(f => f.WikiPath)
                .Select(f => new { Name = f.Key, Links = f.Where(g => g.Name == f.Key).ToList() })
                .Where(f => f.Links.Count > 1)
                .SelectMany(f => f.Links)
                .ToList();

            var grpMainContentFlatWrite = data!
                .GroupBy(f => f.MainContent)
                .Select(f => new { MainContent = f.Key, Links = f.Where(g => g.MainContent == f.Key).ToList() })
                .Where(f => f.Links.Count > 1)
                .SelectMany(f => f.Links)
                .ToList();

            //Final write and build
            return data!
                .Select(f => new WikiDataCleanerModel
                {
                    Ignored = ignoredItems.ContainsIgnoreCase(f.Name),
                    DuplicateName = grpNameFlatWrite.Any(g => g.Name == f.Name && g.WikiPath == f.WikiPath),
                    DuplicateLink = grpPathFlatWrite.Any(g => g.Name == f.Name && g.WikiPath == f.WikiPath),
                    DuplicateContent = grpMainContentFlatWrite.Any(g => g.Name == f.Name && g.WikiPath == f.WikiPath),
                    Item = f
                }).ToList();
        }
        private void CleanIgnoreListItems(List<WikiDataCleanerModel> data)
        {
            foreach (var item in data.Where(f => f.Ignored))
            {
                Console.WriteLine($"Delete Ignore list item: {item.Item.Name}");
                toStore!.CleanEntry(item.Item.Id);
            }
        }
        private void CleanDuplicateNames(List<WikiDataCleanerModel> data,
            bool optionDelete = true,
            bool optionRenameFromUrl = false)
        {
            if (optionDelete && optionRenameFromUrl)
            {
                optionRenameFromUrl = false;
            }

            //find by duplicate names
            //check content are same
            //make sure the tags are evenly distributed
            var dupNames = data
                .Where(f => f.DuplicateName)
                .GroupBy(f => f.Item.Name)
                .Select(f => new { f.Key, Items = f.ToList() })
                .ToList();

            foreach (var dupItem in dupNames)
            {
                var grpMainContents = dupItem.Items
                    .GroupBy(f => f.Item.MainContent)
                    .Select(f => new { f.Key, Childs = f.ToList() })
                    .ToList();

                if (grpMainContents.Count == 1)
                {
                    var tags = dupItem.Items.SelectMany(f => f.Item.Tags).Distinct().ToList();
                    foreach (var item in dupItem.Items)
                    {
                        toStore!.UpdateTags(tags, item.Item.Id);
                    }
                    if (optionDelete)
                    {
                        if (dupItem.Items.Count > 1)
                        {
                            for (int i = 1; i < dupItem.Items.Count; i++)
                            {
                                Console.WriteLine($"Delete Duplicate list item: {dupItem.Items[i].Item.Name}");
                                toStore!.CleanEntry(dupItem.Items[i].Item.Id);
                            }
                        }
                    }
                    else if (optionRenameFromUrl)
                    {
                        for (int i = 0; i < dupItem.Items.Count; i++)
                        {
                            Console.WriteLine($"Updating Duplicate list item: {dupItem.Items[i].Item.Name}");
                            var url = dupItem.Items[i].Item.WikiPath;
                            url = HttpUtility.UrlDecode(url);
                            url = url.Substring(url.LastIndexOf('/') + 1).ReplaceMultiple(" ", "_");
                            toStore!.UpdateName(url, dupItem.Items[i].Item.Id);
                        }
                    }
                }
                else
                {

                }
            }
        }
        public void CleanDataWithDump()
        {
            Initialize(false);

            var toRaw = PrepareDumpData();
            CleanIgnoreListItems(toRaw);
            CleanDuplicateNames(toRaw, optionDelete: false, optionRenameFromUrl: true);
            toRaw = PrepareDumpData();
            CleanDuplicateNames(toRaw, optionDelete: true);
            toRaw = PrepareDumpData();
            var toWrite = toRaw.Select(f => new WikiDataCleanerWriteModel(f)).OrderBy(f => f.Name);
            CsvHelperEx.WriteToCsv(toWrite,
               IoHelper.CombinePath(Pj.Library.PjUtility.Runtime.ExecutingFolder, "Db", "WikiStoreSaintsDump.csv"),
               hasHeaderRecords: true);

            var dataFullWrite = wikiAppController?.GetListOfWikiItems(new List<string> { "All" }).ToList()
                .Select(f => new { f.Name, f.WikiPath, f.PrimaryMetadataContent }).OrderBy(f => f.Name);

            CsvHelperEx.WriteToCsv(dataFullWrite,
               IoHelper.CombinePath(Pj.Library.PjUtility.Runtime.ExecutingFolder, "Db", "WikiStoreSaintsDumpFullNames.csv"),
               hasHeaderRecords: true);
        }

        public void TestData()
        {
            Initialize(false);
            var data = wikiAppController?.GetListOfWikiItems(new List<string> { "All" }).ToList();
            int counter = 0;

            foreach (var item in data)
            {
                counter++;
                Console.WriteLine($"Testing data for [{counter++}/{data.Count}]: {item.Name}");
                var personaData = wikiAppController?.GetViewModelById(item.Id);
            }
        }

        public void Test()
        {
            Initialize(false);
            wikiAppController.CommonMetadata();

            var pp = wikiAppController.GetListOfWikiItems(new List<string> { "All" }).First();
            var test = wikiAppController.GetViewModelById(pp.Id);
            //var ppA = appCtrl.GetListOfWikiItems(new List<string> { "All" });
            //var tt = appCtrl.GetViewModelByRoute("/wiki/Paul_the_Apostle");


            ////Check Metadata
            //var items = wikiAppController.GetListOfWikiItems().ToList();
            //wikiAppController.MetadataBuild();


            // /wiki/Alberto_Hurtado
            //extractProcess.PersonaSinglePageContentExtractWithSaveToStore(saints.Value, saints.Key);
            //extractProcess.PersonaSinglePageContentExtractWithSaveToStore(new WikiExtractor.Models.WikiWhatToExtractModel { Route = "/wiki/Paul_the_Apostle" });

            //var htmlContent = extractProcess.PersonaSinglePageContentExtractWithSaveToStore("/wiki/Pope_John_Paul_II", string.Empty);
            ////var tt = serviceProvider.GetService<WikiAppController>().GetViewModelByRoute("/wiki/Pope_John_Paul_II");
            //var tt01 = serviceProvider.GetService<WikiAppController>().GetListOfWikiItems().ToList();
        }
    }
}
