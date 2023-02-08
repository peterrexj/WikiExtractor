using Pj.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.Exts;
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
            var centuryPopes01 = toStore!.ExtractListTabularByCentury("/wiki/List_of_popes", "Popes of the 1st century", new List<string> { "All", "1st century" }, hasPortrait: false, hasPersonalName: false);
            var centuryPopes02 = toStore!.ExtractListTabularByCentury("/wiki/List_of_popes", "Popes of the 2nd century", new List<string> { "All", "2nd century" }, hasPortrait: false, hasPersonalName: false);
            var centuryPopes03 = toStore!.ExtractListTabularByCentury("/wiki/List_of_popes", "Popes of the 3rd century", new List<string> { "All", "3rd century" }, hasPortrait: false, hasPersonalName: false);
            var centuryPopes04 = toStore!.ExtractListTabularByCentury("/wiki/List_of_popes", "Popes of the 4th century", new List<string> { "All", "4th century" }, hasPortrait: false, hasPersonalName: false);
            var centuryPopes05 = toStore!.ExtractListTabularByCentury("/wiki/List_of_popes", "Popes of the 5th century", new List<string> { "All", "5th century" }, hasPortrait: false, hasPersonalName: false);
            var centuryPopes06 = toStore!.ExtractListTabularByCentury("/wiki/List_of_popes", "Popes of the 6th century", new List<string> { "All", "6th century" }, hasPortrait: false, hasPersonalName: false);
            var centuryPopes07 = toStore!.ExtractListTabularByCentury("/wiki/List_of_popes", "Popes of the 7th century", new List<string> { "All", "7th century" }, hasPortrait: false, hasPersonalName: false);
            var centuryPopes08 = toStore!.ExtractListTabularByCentury("/wiki/List_of_popes", "Popes of the 8th century", new List<string> { "All", "8th century" }, hasPortrait: true, hasPersonalName: true);
            var centuryPopes09 = toStore!.ExtractListTabularByCentury("/wiki/List_of_popes", "Popes of the 9th century", new List<string> { "All", "9th century" }, hasPortrait: true, hasPersonalName: true);
            var centuryPopes10 = toStore!.ExtractListTabularByCentury("/wiki/List_of_popes", "Popes of the 10th century", new List<string> { "All", "10th century" }, hasPortrait: false, hasPersonalName: false);
            var centuryPopes11 = toStore!.ExtractListTabularByCentury("/wiki/List_of_popes", "Popes of the 11th century", new List<string> { "All", "11th century" }, hasPortrait: true, hasPersonalName: true);
            var centuryPopes12 = toStore!.ExtractListTabularByCentury("/wiki/List_of_popes", "Popes of the 12th century", new List<string> { "All", "12th century" }, hasPortrait: true, hasPersonalName: true);
            var centuryPopes13 = toStore!.ExtractListTabularByCentury("/wiki/List_of_popes", "Popes of the 13th century", new List<string> { "All", "13th century" }, hasPortrait: true, hasPersonalName: true);
            var centuryPopes14 = toStore!.ExtractListTabularByCentury("/wiki/List_of_popes", "Popes of the 14th century", new List<string> { "All", "14th century" }, hasPortrait: true, hasPersonalName: true);
            var centuryPopes15 = toStore!.ExtractListTabularByCentury("/wiki/List_of_popes", "Popes of the 15th century", new List<string> { "All", "15th century" }, hasPortrait: true, hasPersonalName: true);
            var centuryPopes16 = toStore!.ExtractListTabularByCentury("/wiki/List_of_popes", "Popes of the 16th century", new List<string> { "All", "16th century" }, hasPortrait: true, hasPersonalName: true);
            var centuryPopes17 = toStore!.ExtractListTabularByCentury("/wiki/List_of_popes", "Popes of the 17th century", new List<string> { "All", "17th century" }, hasPortrait: true, hasPersonalName: true);
            var centuryPopes18 = toStore!.ExtractListTabularByCentury("/wiki/List_of_popes", "Popes of the 18th century", new List<string> { "All", "18th century" }, hasPortrait: true, hasPersonalName: true);
            var centuryPopes19 = toStore!.ExtractListTabularByCentury("/wiki/List_of_popes", "Popes of the 19th century", new List<string> { "All", "19th century" }, hasPortrait: true, hasPersonalName: true);
            var centuryPopes20 = toStore!.ExtractListTabularByCentury("/wiki/List_of_popes", "Popes of the 20th century", new List<string> { "All", "20th century" }, hasPortrait: true, hasPersonalName: true);
            var centuryPopes21 = toStore!.ExtractListTabularByCentury("/wiki/List_of_popes", "Popes of the 21st century", new List<string> { "All", "21st century" }, hasPortrait: true, hasPersonalName: true);

            wikiAppController!.AddMenuItem("All Popes", "All", "Popes Of Church", 1);
            wikiAppController.AddMenuItem("21st century", "21st century", "21st Century", 2);
            wikiAppController.AddMenuItem("20th century", "20th century", "20th Century", 3);
            wikiAppController.AddMenuItem("19th century", "19th century", "19th Century", 4);
            wikiAppController.AddMenuItem("18th century", "18th century", "18th Century", 5);
            wikiAppController.AddMenuItem("17th century", "17th century", "17th Century", 6);
            wikiAppController.AddMenuItem("16th century", "16th century", "16th Century", 7);
            wikiAppController.AddMenuItem("15th century", "15th century", "15th Century", 8);
            wikiAppController.AddMenuItem("14th century", "14th century", "14th Century", 9);
            wikiAppController.AddMenuItem("13th century", "13th century", "13th Century", 10);
            wikiAppController.AddMenuItem("12th century", "12th century", "12th Century", 11);
            wikiAppController.AddMenuItem("11th century", "11th century", "11th Century", 12);
            wikiAppController.AddMenuItem("10th century", "10th century", "10th Century", 13);
            wikiAppController.AddMenuItem("9th century", "9th century", "9th Century", 14);
            wikiAppController.AddMenuItem("8th century", "8th century", "8th Century", 15);
            wikiAppController.AddMenuItem("7th century", "7th century", "7th Century", 16);
            wikiAppController.AddMenuItem("6th century", "6th century", "6th Century", 17);
            wikiAppController.AddMenuItem("5th century", "5th century", "5th Century", 18);
            wikiAppController.AddMenuItem("4th century", "4th century", "4th Century", 19);
            wikiAppController.AddMenuItem("3rd century", "3rd century", "3rd Century", 20);
            wikiAppController.AddMenuItem("2nd century", "2nd century", "2nd Century", 21);
            wikiAppController.AddMenuItem("1st century", "1st century", "1st Century", 22);

            EnablePrimaryMetadataContent();

            var popesCollection = centuryPopes21
                 .Union(centuryPopes20).Union(centuryPopes19).Union(centuryPopes18).Union(centuryPopes17).Union(centuryPopes16)
                 .Union(centuryPopes15).Union(centuryPopes14).Union(centuryPopes13).Union(centuryPopes12).Union(centuryPopes11)
                 .Union(centuryPopes10).Union(centuryPopes09).Union(centuryPopes08).Union(centuryPopes07).Union(centuryPopes06)
                 .Union(centuryPopes05).Union(centuryPopes04).Union(centuryPopes03).Union(centuryPopes02).Union(centuryPopes01)
                 .ToList()
             .WithDefaultFilters();


            int totalCount = popesCollection.Count;
            int currentIndex = 1;

            //foreach (var saints in saintsCollection)
            Parallel.ForEach(popesCollection, new ParallelOptions { MaxDegreeOfParallelism = 1 }, saint =>
            {
                try
                {
                    toStore.PersonaSinglePageContentExtractWithSaveToStore(saint);
                    Console.WriteLine($"[{currentIndex}/{totalCount}] [{(int)(((decimal)currentIndex / (decimal)totalCount) * 100)}%] Saints [{saint.Title}]: {saint.Route}");
                    //Thread.Sleep(1000);
                    currentIndex = currentIndex + 1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            });
        }

        public void EnablePrimaryMetadataContent()
        {
            if (wikiAppController == null)
            {
                Initialize(false);
            }
            wikiAppController!.EnableWithPrimaryMetadataContent(new List<string> 
            { 
                "Pontiff number", 
                "English Name", 
                "Personal Name", 
                "Date & Place Of Birth" 
            }, 4);
        }

        public void Test()
        {
            Initialize(false);
            //wikiAppController.DisablePrimaryMetadataContent();
            //wikiAppController.EnableWithPrimaryMetadataContent(new List<string> { "Pontiff number", "English Name", "Personal Name", "Latin Name", "Date & Place Of Birth", "Died", "Church" });

            var images = wikiAppController.GetPrimaryImages();

            foreach (var image in images)
            {

                var resp = new TestApiHttp()
                       .OpenFullUrl(image)
                       .Download("");
            }

        }
    }
}
