using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.Exts;
using WikiExtractor.Models;
using WikiExtractor.Process.Extractor;
using Pj.Library;

namespace WikiExtractor.Process.Modules
{
    public class CountriesDataExtractor : DataExtractorBase
    {
        protected CountriesWikiExtractionToStore? toStore = null;

        public CountriesDataExtractor() : base("Countries", "WikiStoreCountries.db") { }

        protected override void Initialize(bool doClean)
        {
            base.Initialize(doClean);
            toStore = new CountriesWikiExtractionToStore();
        }

        public void ExtractData(string? targetTitle = null)
        {
            Initialize(true);
            var excludedMetadata = new List<string> { "Rank", "FlagImage" };
            var countries = toStore!.ListByDependencyArea_ForCountries("/wiki/List_of_countries_and_dependencies_by_area", new List<string> { "Countries", "All" }).ToList();
            var noncountries = toStore!.ListByDependencyArea_ForNonCountries("/wiki/List_of_countries_and_dependencies_by_area", new List<string> { "Other known nations", "All" }).ToList();

            var groupByCountryStartLetter = countries.Select(f => f.Title).OrderBy(f => f).GroupBy(f => f.Substring(0, 1)).Select(f => new { f.Key, Countries = f.ToList() });

            int menuItemCounter = 0;
            //wikiAppController!.AddMenuItem("All", "All", "Countries & Non Countries", menuItemCounter++);
            wikiAppController!.AddMenuItem("Countries", "Countries", "Countries", menuItemCounter++);
            wikiAppController!.AddMenuItem("Other known nations", "Other known nations", "Other known nations", menuItemCounter++);

            foreach (var startLetterContent in groupByCountryStartLetter)
            {
                foreach (var country in startLetterContent.Countries)
                {
                    var coun = countries.FirstOrDefault(f => f.Title == country);
                    coun?.Tags?.Add($"Country start with [{startLetterContent.Key}]");
                }
                wikiAppController!.AddMenuItem($"Country start with [{startLetterContent.Key}]", $"Country start with [{startLetterContent.Key}]", $"Country start with [{startLetterContent.Key}]", menuItemCounter++);
            }

            EnablePrimaryMetadataContent();

            var countriesCollection = countries.OrderBy(f => f.Sequence).Union(noncountries.OrderBy(f => f.Sequence)).ToList().WithDefaultFilters();

            if (!string.IsNullOrWhiteSpace(targetTitle))
            {
                countriesCollection = countriesCollection
                    .Where(c => c.Title.Contains(targetTitle, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                Console.WriteLine($"\n[Countries] Target filter '{targetTitle}' → {countriesCollection.Count} match(es)");
            }

            int totalCount = countriesCollection.Count;
            int currentIndex = 1;

            Console.WriteLine($"\n[Countries] Collection assembled: {totalCount} countries");

            ConcurrentBag<Tuple<WikiPageModel, List<MetaDataModel>, WikiWhatToExtractModel>> bag = new();
            ConcurrentBag<Guid> fetchFailedIds = new();

            LogPhase("Fetch pages");
            long fetchStart = Environment.TickCount64;
            Parallel.ForEach(countriesCollection, new ParallelOptions { MaxDegreeOfParallelism = 1 }, country =>
            {
                int idx;
                lock (_lock) { idx = currentIndex++; }
                LogProgress("Fetch", idx, totalCount, fetchStart, $"{country.Title}  ({country.Route})");
                try
                {
                    var rawData = toStore.SinglePageContentExtract(country, excludedMetadata);
                    bag.Add(new Tuple<WikiPageModel, List<MetaDataModel>, WikiWhatToExtractModel>(rawData.Item1, rawData.Item2, country));
                }
                catch (Exception ex)
                {
                    fetchFailedIds.Add(country.Id);
                    Console.WriteLine($"  [FETCH ERROR] {country.Title}: {ex.Message}");
                }
            });
            LogPhaseSummary("Fetch", totalCount, fetchStart);

            foreach (var country in countriesCollection)
            {
                var bagItem = bag.FirstOrDefault(f => f.Item3.Id == country.Id);
                if (bagItem == null || bagItem.Item1 == null || bagItem.Item2 == null)
                    if (!fetchFailedIds.Contains(country.Id))
                        Console.WriteLine($"  [WARN] No page data for [{country.Title}]: {country.Route}");
            }

            currentIndex = 1;
            ConcurrentDictionary<Guid, int> storedMasterIds = new();
            LogPhase("Store to DB");
            long storeStart = Environment.TickCount64;
            Parallel.ForEach(countriesCollection, new ParallelOptions { MaxDegreeOfParallelism = 1 }, country =>
            {
                int idx;
                lock (_lock) { idx = currentIndex++; }
                LogProgress("Store", idx, totalCount, storeStart, country.Title);
                try
                {
                    var bagItem = bag.FirstOrDefault(f => f.Item3.Id == country.Id);
                    if (bagItem == null) return;
                    var masterId = toStore.SinglePageContentStore(bagItem.Item1, bagItem.Item2, bagItem.Item3);
                    storedMasterIds[country.Id] = masterId;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  [STORE ERROR] {country.Title}: {ex.Message}");
                }
            });
            LogPhaseSummary("Store", totalCount, storeStart);

            var extractionRecords = countriesCollection.Select(country =>
            {
                var bagItem = bag.FirstOrDefault(f => f.Item3.Id == country.Id);
                return new ExtractionReporter.ExtractionRecord
                {
                    Item = country,
                    PageModel = bagItem?.Item1,
                    Metadatas = bagItem?.Item2,
                    PageFetchFailed = fetchFailedIds.Contains(country.Id),
                    StoredMasterId = storedMasterIds.TryGetValue(country.Id, out var mid) ? mid : 0,
                };
            }).ToList();

            var reportFolder = Path.Combine(Path.GetDirectoryName(ProcessConstants.DatabasePath)!, "..", "Reports");
            var reporter = new ExtractionReporter(reportFolder, "Countries");
            reporter.WriteReports(extractionRecords, imageValidationDelayMs: ProcessConstants.UseCache ? 0 : 2000, skipImageValidation: true);


            ////Temp code
            //IoHelper.CopyFile(@"C:\GIT\Other\peterrexj\WikiExtractor\WikiExtractor\bin\Debug\net6.0\Db\WikiStoreCountries.db",
            //    @"C:\GIT\Other\peterrexj\WikiExtractor\App\Popes\PopesOfChurch.UWP\Assets\WikiStoreCountries.db");
            //IoHelper.MoveFile(@"C:\GIT\Other\peterrexj\WikiExtractor\WikiExtractor\bin\Debug\net6.0\Db\WikiStoreCountries.db",
            //    @"C:\GIT\Other\peterrexj\WikiExtractor\App\Popes\PopesOfChurch.UWP\Assets", isTargetFolder: true);
        }

        public void EnablePrimaryMetadataContent()
        {
            if (wikiAppController == null)
            {
                Initialize(false);
            }
            wikiAppController!.EnableWithPrimaryMetadataContent(new List<string>
            {
                "Government",
                "Currency",
                "Total in km2 (mi2)",
                "Time zone",
                "Calling code",
                "Official languages",
                "Density (Population)",
                "Per capita (GDP (PPP))",
                "Internet TLD",
            }, 6);
        }

        public void Test()
        {
            Initialize(false);
            wikiAppController.CommonMetadata();
            var testTagFilterData = wikiAppController.GetListOfWikiItems(new List<string> { "Country start with [R]" });
            var contentTest = wikiAppController.GetViewModelByIdAsync(1).GetAwaiter().GetResult();

        }
    }
}
