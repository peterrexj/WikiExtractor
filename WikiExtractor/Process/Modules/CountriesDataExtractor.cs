using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.Process.Extractor;

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

            var centuryPopes01 = toStore!.ListByDependencyArea("/wiki/List_of_countries_and_dependencies_by_area", new List<string> { "All", "1st century" });

        }
    }
}
