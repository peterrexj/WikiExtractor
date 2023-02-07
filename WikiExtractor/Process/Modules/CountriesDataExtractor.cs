using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.Process.Extractor;
using WikiExtractor.Exts;
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

        public void ExtractData()
        {
            Initialize(true);
            var excludedMetadata = new List<string> { "Rank", "FlagImage" };
            var countries = toStore!.ListByDependencyArea_ForCountries("/wiki/List_of_countries_and_dependencies_by_area", new List<string> { "Countries", "All" }).ToList();
            var noncountries = toStore!.ListByDependencyArea_ForNonCountries("/wiki/List_of_countries_and_dependencies_by_area", new List<string> { "Non Countries", "All" }).ToList();

            var groupByCountryStartLetter = countries.Select(f => f.Title).OrderBy(f => f).GroupBy(f => f.Substring(0, 1)).Select(f => new { f.Key, Countries = f.ToList() });

            int menuItemCounter = 0;
            //wikiAppController!.AddMenuItem("All", "All", "Countries & Non Countries", menuItemCounter++);
            wikiAppController!.AddMenuItem("Countries", "Countries", "Countries", menuItemCounter++);
            wikiAppController!.AddMenuItem("Non Countries", "Non Countries", "Non Countries", menuItemCounter++);

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

            int totalCount = countriesCollection.Count;
            int currentIndex = 1;

            //foreach (var saints in saintsCollection)
            Parallel.ForEach(countriesCollection, new ParallelOptions { MaxDegreeOfParallelism = 1 }, saint =>
            {
                try
                {
                    toStore.PersonaSinglePageContentExtractWithSaveToStore(saint, excludedMetadata);
                    Console.WriteLine($"[{currentIndex}/{totalCount}] [{(int)(((decimal)currentIndex / (decimal)totalCount) * 100)}%] Saints [{saint.Title}]: {saint.Route}");
                    //Thread.Sleep(1000);
                    currentIndex = currentIndex + 1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            });


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
                "Total in km2 (mi2)",
                "Time zone",
                "Currency",
                "Government",
                "Density (Population)",
                "Per capita (GDP (PPP))",
                "Calling code",
                "Land in km2 (mi2)",
                "Water in km2 (mi2)"
            }, 9);
        }

        public void Test()
        {
            Initialize(false);
            wikiAppController.CommonMetadata();
            var testTagFilterData = wikiAppController.GetListOfWikiItems(new List<string> { "Country start with [R]" });


        }
    }
}
