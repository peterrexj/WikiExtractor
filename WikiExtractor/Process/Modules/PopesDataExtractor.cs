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
            //var centuryPopes01 = toStore!.ExtractListTabularByCentury("/wiki/List_of_popes", "Popes of the 1st century", new List<string> { "All", "1st century" });
            //var centuryPopes02 = toStore!.ExtractListTabularByCentury("/wiki/List_of_popes", "Popes of the 2nd century", new List<string> { "All", "2nd century" });
            //var centuryPopes03 = toStore!.ExtractListTabularByCentury("/wiki/List_of_popes", "Popes of the 3rd century", new List<string> { "All", "3rd century" });
            //var centuryPopes04 = toStore!.ExtractListTabularByCentury("/wiki/List_of_popes", "Popes of the 4th century", new List<string> { "All", "4th century" });
            //var centuryPopes05 = toStore!.ExtractListTabularByCentury("/wiki/List_of_popes", "Popes of the 5th century", new List<string> { "All", "5th century" });
            //var centuryPopes06 = toStore!.ExtractListTabularByCentury("/wiki/List_of_popes", "Popes of the 6th century", new List<string> { "All", "6th century" });
            var centuryPopes07 = toStore!.ExtractListTabularByCentury("/wiki/List_of_popes", "Popes of the 7th century", new List<string> { "All", "7th century" }, hasPortrait: false, hasPersonalName: false);

            var centuryPopes08 = toStore!.ExtractListTabularByCentury("/wiki/List_of_popes", "Popes of the 8th century", new List<string> { "All", "8th century" }, hasPortrait: true, hasPersonalName: true);


        }
    }
}
