using System.Collections.Concurrent;
using WikiExtractor.Exts;
using WikiExtractor.Models;
using WikiExtractor.Process.Extractor;

namespace WikiExtractor.Process.Modules
{
    public class WorldLeadersExtractor : DataExtractorBase
    {
        protected WorldLeadersWikiExtractionToStore? toStore = null;

        public WorldLeadersExtractor() : base("World Leaders", "WikiStoreWorldLeaders.db") { }

        protected override void Initialize(bool doClean)
        {
            base.Initialize(doClean);
            toStore = new WorldLeadersWikiExtractionToStore();
        }

        public void ExtractData()
        {
            Initialize(true);
            int menuItemCounter = 0;

            if (wikiAppController == null) return;
            if (toStore == null) return;

            wikiAppController.AddMenuItem("World Leaders", "All", "World Leaders", menuItemCounter++);
            wikiAppController.AddMenuItem("Australia", "AUS PM", "Prime ministers of Australia", menuItemCounter++);
            wikiAppController.AddMenuItem("New Zealand", "NewZealand PM", "Prime ministers of New Zealand", menuItemCounter++);
            wikiAppController.AddMenuItem("Japan", "JPN PM", "Prime ministers of Japan", menuItemCounter++);
            wikiAppController.AddMenuItem("United States", "US Pre", "Presidents of United States", menuItemCounter++);
            wikiAppController.AddMenuItem("United Kingdom", "UK PM", "Prime ministers of United Kingdom", menuItemCounter++);
            wikiAppController.AddMenuItem("India", "IN PM", "Prime ministers of India", menuItemCounter++);
            wikiAppController.AddMenuItem("Canada", "CN PM", "Prime ministers of Canada", menuItemCounter++);
            wikiAppController.AddMenuItem("Germany", "GER PM", "Presidents of Germany", menuItemCounter++);
            wikiAppController.AddMenuItem("France", "FR PM", "Presidents of France", menuItemCounter++);

            //EnablePrimaryMetadataContent();

            var stkAustralia = toStore.ExtractListTabularData("Australia", "/wiki/List_of_prime_ministers_of_Australia", new List<string> { "All", "AUS PM" }).ToStack();
            var stkNewZealand = toStore.ExtractListTabularData("NewZealand", "/wiki/List_of_prime_ministers_of_New_Zealand", new List<string> { "All", "NewZealand PM" }).ToStack();
            var stkJapan = toStore.ExtractListTabularData("Japan", "/wiki/List_of_prime_ministers_of_Japan", new List<string> { "All", "JPN PM" }).ToStack();
            var stkUnitedStates = toStore.ExtractListTabularData("UnitedStates", "/wiki/List_of_presidents_of_the_United_States", new List<string> { "All", "US Pre" }).ToStack();
            var stkUnitedKingdom = toStore.ExtractListTabularData("UnitedKingdom", "/wiki/List_of_prime_ministers_of_the_United_Kingdom", new List<string> { "All", "UK PM" }).ToStack();
            var stkIndia = toStore.ExtractListTabularData("India", "/wiki/List_of_prime_ministers_of_India", new List<string> { "All", "IN PM" }).ToStack();
            var stkCanada = toStore.ExtractListTabularData("Canada", "/wiki/List_of_prime_ministers_of_Canada", new List<string> { "All", "CN PM" }).ToStack();
            var stkFrance = toStore.ExtractListTabularData("France", "/wiki/List_of_presidents_of_France", new List<string> { "All", "FR PM" }).ToStack();
            var stkGermany = toStore.ExtractListTabularData("Germany", "/wiki/List_of_presidents_of_Germany", new List<string> { "All", "GER PM" }).ToStack();

            //var stacks = new List<Stack<WikiWhatToExtractModel>> { stkGermany };

            var stacks = new List<Stack<WikiWhatToExtractModel>> { stkAustralia, stkNewZealand, stkJapan, stkUnitedStates, stkUnitedKingdom, stkIndia, stkCanada, stkFrance, stkGermany };
            List<WikiWhatToExtractModel> worldLeadersCollection = new();

            bool hasElements;
            do
            {
                hasElements = false;
                foreach (var stack in stacks)
                {
                    if (stack.Count > 0)
                    {
                        worldLeadersCollection.Add(stack.Pop());
                        Console.WriteLine(worldLeadersCollection.Last().Title);
                        hasElements = true;
                    }
                }
            } while (hasElements);

            int totalCount = worldLeadersCollection.Count;
            int currentIndex = 1;

            ConcurrentBag<Tuple<WikiPageModel, List<MetaDataModel>, WikiWhatToExtractModel>> bag = new();

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = ProcessConstants.UseCache ? 5 : 1
            };

            Parallel.ForEach(worldLeadersCollection, parallelOptions, leader =>
            {
                try
                {
                    //Thread.Sleep(1000);
                    lock (_lock)
                    {
                        Console.WriteLine($"[{currentIndex}/{totalCount}] [{(int)(((decimal)currentIndex / (decimal)totalCount) * 100)}%] World Leaders [{leader.Title}]: {leader.Route}");
                        currentIndex = currentIndex + 1;
                    }
                    var rawData = toStore.SinglePageContentExtract(leader);
                    bag.Add(new Tuple<WikiPageModel, List<MetaDataModel>, WikiWhatToExtractModel>(rawData.Item1, rawData.Item2, leader));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });


            ////foreach (var saints in saintsCollection)
            Parallel.ForEach(worldLeadersCollection, new ParallelOptions { MaxDegreeOfParallelism = 5 }, leader =>
            {
                var bagItem = bag.FirstOrDefault(f => f.Item3.Id == leader.Id);
                if (bagItem == null || bagItem.Item1 == null || bagItem.Item2 == null || bagItem.Item3 == null)
                {
                    throw new Exception("Bag item cannot be mapped, this could be due to the extraction failure");
                }
            });

            currentIndex = 1;
            //foreach (var saints in saintsCollection)
            Parallel.ForEach(worldLeadersCollection, new ParallelOptions { MaxDegreeOfParallelism = 1 }, leader =>
            {
                try
                {
                    var bagItem = bag.FirstOrDefault(f => f.Item3.Id == leader.Id);
                    toStore.SinglePageContentStore(bagItem.Item1, bagItem.Item2, bagItem.Item3);
                    Console.WriteLine($"[{currentIndex}/{totalCount}] [{(int)(((decimal)currentIndex / (decimal)totalCount) * 100)}%] Leader [{leader.Title}]: {leader.Route}");
                    //Thread.Sleep(1000);
                    currentIndex = currentIndex + 1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });

            ////Clean the data
            //CleanDataWithDump();
        }

        public void EnablePrimaryMetadataContent()
        {
            if (wikiAppController == null)
            {
                Initialize(false);
            }
            wikiAppController!.EnableWithPrimaryMetadataContent(new List<string>
            {
                "Country",
                "Preceded by",
                "Succeeded by",
                "Political party",
                "Birth-Death",
                "Riding",
                "Cabinet",
                "Children",
                "Spouse",
                "Resting place",
                "Constituency",
                "Monarch",
                "Education",
                "Occupation",
                "Days in office",
                "Term",
            }, 5);

        }

        public void CleanDataWithDump()
        {
            Initialize(false);

            var data = wikiAppController.GetListOfWikiItems(new List<string> { "All" }).ToList();

            foreach (var item in data)
            {
                Console.WriteLine($"Primary image fix for: {item.Name}");
                var personaData = wikiAppController?.GetViewModelById(item.Id);
                if (personaData != null && personaData.Metadatas?.Any(f => f.Key == "Portrait") == true)
                {
                    wikiAppController.UpdatePrimaryImage(item.Id, personaData.Metadatas?.First(f => f.Key == "Portrait").Description);
                }
            }

            foreach (var item in data)
            {
                Console.WriteLine($"Removing metadata [not required]: {item.Name}");
                wikiAppController.RemoveMetadataInfo(item.Id, "Portrait", "No", "Website");
            }
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
        }
    }
}
