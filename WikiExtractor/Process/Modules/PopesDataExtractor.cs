using Pj.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.Process.Extractor;

namespace WikiExtractor.Process.Modules
{
    internal class PopesDataExtractor : DataExtractorBase
    {
        protected PopesWikiExtractionToStore? toStore = null;

        public PopesDataExtractor() : base("Popes", "WikiStorePopes.db") { }

        protected override void Initialize(bool doClean)
        {
            base.Initialize(doClean);
            toStore = new PopesWikiExtractionToStore();
        }

        public void ExtractData()
        {
            Initialize(true);
            var centuryPopes01 = toStore!.ExtractListTabularByCentury("/wiki/List_of_popes",
                "1st century",
                new List<string> { "All", "1st century" });

        }
    }
}
