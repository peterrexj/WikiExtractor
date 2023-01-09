using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.Process.Extractor;
using WikiExtractor.Exts;

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

            var countries = toStore!.ListByDependencyArea_ForCountries("/wiki/List_of_countries_and_dependencies_by_area", new List<string> { "Countries" }).ToList();
            var noncountries = toStore!.ListByDependencyArea_ForNonCountries("/wiki/List_of_countries_and_dependencies_by_area", new List<string> { "Non Countries" }).ToList();
            
            var groupByCountryStartLetter = countries.Select(f => f.Title).OrderBy(f => f).GroupBy(f => f.Substring(0, 1)).Select(f => new { f.Key, Countries = f.ToList() });

            foreach (var startLetterContent in groupByCountryStartLetter)
            {
                foreach (var country in startLetterContent.Countries)
                {
                    var coun = countries.FirstOrDefault(f => f.Title == country);
                    coun?.Tags?.Add($"Country {startLetterContent.Key}");
                }
            }

            wikiAppController.EnableWithPrimaryMetadataContent(new List<string> { "Total in km2 (mi2)", "Land in km2 (mi2)", "Water in km2 (mi2)" });

            var countriesCollection = countries.Union(noncountries).ToList().WithDefaultFilters();

            int totalCount = countriesCollection.Count;
            int currentIndex = 1;

            //foreach (var saints in saintsCollection)
            Parallel.ForEach(countriesCollection, new ParallelOptions { MaxDegreeOfParallelism = 1 }, saint =>
            {
                try
                {
                    toStore.PersonaSinglePageContentExtractWithSaveToStore(saint);
                    Console.WriteLine($"[{currentIndex}/{totalCount}] [{(int)(((decimal)currentIndex / (decimal)totalCount) * 100)}%] Saints [{saint.Title}]: {saint.Route}");
                    Thread.Sleep(1000);
                    currentIndex = currentIndex + 1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            });
        }
    }
}
