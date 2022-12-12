// See https://aka.ms/new-console-template for more information


using Microsoft.Extensions.DependencyInjection;
using Pj.Library;
using WikiExtractor;
using WikiExtractor.DbModels;
using WikiExtractor.Exts;
using WikiExtractor.Process;
using WikiExtractor.Repository;

var serviceProvider = ContainerConfiguration.Configure();
//serviceProvider.GetService<ContinuousRunningProcessor>().Process();

Console.WriteLine("Hello, Saints Extractor!");

ProcessConstants.CacheFolder = IoHelper.CombinePath(PjUtility.Runtime.ExecutingFolder, "Cache");
ProcessConstants.DatabasePath = IoHelper.CombinePath(PjUtility.Runtime.ExecutingFolder, "Db", "WikiStore.db");
IoHelper.DeleteFile(ProcessConstants.DatabasePath);

var appCtrl = new WikiAppController(new WikiDatabase());
var extractProcess = new WikiPageRawExtraction();


appCtrl.AddMenuItem("All Saints", "All", "Saints", 1);
appCtrl.AddMenuItem("Canonized by Pope Francis", "Canonized by Pope Francis", "Canonized by Pope Francis", 2);
appCtrl.AddMenuItem("Canonized by Pope John Paul II", "Canonized by Pope John Paul II", "Canonized by Pope John Paul II", 3);
appCtrl.AddMenuItem("Canonized by Pope John Paul II", "Canonized by Pope John Paul II", "Canonized by Pope John Paul II", 4);
appCtrl.AddMenuItem("Canonized by Pope Leo XIII", "Canonized by Pope Leo XIII", "Canonized by Pope Leo XIII", 5);
appCtrl.AddMenuItem("Canonized by Pope Pius XI", "Canonized by Pope Pius XI", "Canonized by Pope Pius XI", 6);
appCtrl.AddMenuItem("Canonized by Pope Pius XII", "Canonized by Pope Pius XII", "Canonized by Pope Pius XII", 7);
appCtrl.AddMenuItem("Canonized by Pope John XXIII", "Canonized by Pope John XXIII", "Canonized by Pope John XXIII", 8);
appCtrl.AddMenuItem("Canonized by Pope Paul VI", "Canonized by Pope Paul VI", "Canonized by Pope Paul VI", 9);
appCtrl.AddMenuItem("Patron Saints", "Patron Saints", "Patron Saints", 10);
appCtrl.AddMenuItem("Beatified", "Beatified", "Beatified", 11);
appCtrl.AddMenuItem("Pope", "By Pope", "Pope", 12);
appCtrl.AddMenuItem("1st Century", "1st Century", "1st Century Saints", 13);
appCtrl.AddMenuItem("2nd Century", "2nd Century", "2nd Century Saints", 14);
appCtrl.AddMenuItem("3rd Century", "3rd Century", "3rd Century Saints", 15);
appCtrl.AddMenuItem("4th Century", "4th Century", "4th Century Saints", 16);
appCtrl.AddMenuItem("5th Century", "5th Century", "5th Century Saints", 17);
appCtrl.AddMenuItem("6th Century", "6th Century", "6th Century Saints", 18);
appCtrl.AddMenuItem("7th Century", "7th Century", "7th Century Saints", 19);
appCtrl.AddMenuItem("8th Century", "8th Century", "8th Century Saints", 20);
appCtrl.AddMenuItem("9th Century", "9th Century", "9th Century Saints", 21);
appCtrl.AddMenuItem("10th Century", "10th Century", "10th Century Saints", 22);
appCtrl.AddMenuItem("11th Century", "11th Century", "11th Century Saints", 23);
appCtrl.AddMenuItem("12th Century", "12th Century", "12th Century Saints", 24);
appCtrl.AddMenuItem("13th Century", "13th Century", "13th Century Saints", 25);
appCtrl.AddMenuItem("14th Century", "14th Century", "14th Century Saints", 26);
appCtrl.AddMenuItem("15th Century", "15th Century", "15th Century Saints", 27);
appCtrl.AddMenuItem("16th Century", "16th Century", "16th Century Saints", 28);
appCtrl.AddMenuItem("17th Century", "17th Century", "17th Century Saints", 29);
appCtrl.AddMenuItem("18th Century", "18th Century", "18th Century Saints", 30);
appCtrl.AddMenuItem("19th Century", "19th Century", "19th Century Saints", 31);
appCtrl.AddMenuItem("20th Century", "20th Century", "20th Century Saints", 32);
appCtrl.AddMenuItem("21th Century", "21th Century", "21th Century Saints", 33);


////Check Metadata
//var items = wikiAppController.GetListOfWikiItems().ToList();
//wikiAppController.MetadataBuild();


// /wiki/Alberto_Hurtado
//extractProcess.PersonaSinglePageContentExtractWithSaveToStore(saints.Value, saints.Key);
//extractProcess.PersonaSinglePageContentExtractWithSaveToStore(new WikiExtractor.Models.WikiWhatToExtractModel { Route = "/wiki/Paul_the_Apostle" });



var listOfSaintsByEachPope01 = extractProcess.SaintsExtractByEachPopeListData("/wiki/List_of_saints_canonized_by_Pope_Benedict_XVI", new List<string> { "All", "Canonized by Pope John Paul II" });
var listOfSaintsByEachPope02 = extractProcess.SaintsExtractByEachPopeListData("/wiki/List_of_saints_canonized_by_Pope_John_Paul_II", new List<string> { "All", "Canonized by Pope John Paul II" });
var listOfSaintsByEachPope03 = extractProcess.SaintsExtractByEachPopeListData("/wiki/List_of_saints_canonized_by_Pope_Leo_XIII", new List<string> { "All", "Canonized by Pope Leo XIII" });
var listOfSaintsByEachPope04 = extractProcess.SaintsExtractByEachPopeListData("/wiki/List_of_saints_canonized_by_Pope_Pius_XI", new List<string> { "All", "Canonized by Pope Pius XI" });
var listOfSaintsByEachPope05 = extractProcess.SaintsExtractByEachPopeListData("/wiki/List_of_saints_canonized_by_Pope_Pius_XII", new List<string> { "All", "Canonized by Pope Pius XII" });
var listOfSaintsByEachPope06 = extractProcess.SaintsExtractByEachPopeListData("/wiki/List_of_saints_canonized_by_Pope_John_XXIII", new List<string> { "All", "Canonized by Pope John XXIII" });
var listOfSaintsByEachPope07 = extractProcess.SaintsExtractByEachPopeListData("/wiki/List_of_saints_canonized_by_Pope_Paul_VI", new List<string> { "All", "Canonized by Pope Paul VI" });
var listOfSaintsByEachPope08 = extractProcess.SaintsExtractByEachPopeListData("/wiki/List_of_saints_canonized_by_Pope_Francis", new List<string> { "All", "Canonized by Pope Francis" });

var listOfSaintsByCentury1 = extractProcess.SaintsExtractByCentury("/wiki/Chronological_list_of_saints_in_the_1st_century", new List<string> { "All", "1st Century" });
var listOfSaintsByCentury2 = extractProcess.SaintsExtractByCentury("/wiki/Chronological_list_of_saints_in_the_2nd_century", new List<string> { "All", "2nd Century" });
var listOfSaintsByCentury3 = extractProcess.SaintsExtractByCentury("/wiki/Chronological_list_of_saints_in_the_3rd_century", new List<string> { "All", "3rd Century" });
var listOfSaintsByCentury4 = extractProcess.SaintsExtractByCentury("/wiki/Chronological_list_of_saints_in_the_4th_century", new List<string> { "All", "4th Century" });
var listOfSaintsByCentury5 = extractProcess.SaintsExtractByCentury("/wiki/Chronological_list_of_saints_in_the_5th_century", new List<string> { "All", "5th Century" });
var listOfSaintsByCentury6 = extractProcess.SaintsExtractByCentury("/wiki/Chronological_list_of_saints_in_the_6th_century", new List<string> { "All", "6th Century" });
var listOfSaintsByCentury7 = extractProcess.SaintsExtractByCentury("/wiki/Chronological_list_of_saints_in_the_7th_century", new List<string> { "All", "7th Century" });
var listOfSaintsByCentury8 = extractProcess.SaintsExtractByCentury("/wiki/Chronological_list_of_saints_in_the_8th_century", new List<string> { "All", "8th Century" });
var listOfSaintsByCentury9 = extractProcess.SaintsExtractByCentury("/wiki/Chronological_list_of_saints_in_the_9th_century", new List<string> { "All", "9th Century" });
var listOfSaintsByCentury10 = extractProcess.SaintsExtractByCentury("/wiki/Chronological_list_of_saints_in_the_10th_century", new List<string> { "All", "10th Century" });
var listOfSaintsByCentury11 = extractProcess.SaintsExtractByCentury("/wiki/Chronological_list_of_saints_and_blesseds_in_the_11th_century", new List<string> { "All", "11th Century" });
var listOfSaintsByCentury12 = extractProcess.SaintsExtractByCentury("/wiki/Chronological_list_of_saints_and_blesseds_in_the_12th_century", new List<string> { "All", "12th Century" });
var listOfSaintsByCentury13 = extractProcess.SaintsExtractByCentury("/wiki/Chronological_list_of_saints_and_blesseds_in_the_13th_century", new List<string> { "All", "13th Century" });
var listOfSaintsByCentury14 = extractProcess.SaintsExtractByCentury("/wiki/Chronological_list_of_saints_and_blesseds_in_the_14th_century", new List<string> { "All", "14th Century" });
var listOfSaintsByCentury15 = extractProcess.SaintsExtractByCentury("/wiki/Chronological_list_of_saints_and_blesseds_in_the_15th_century", new List<string> { "All", "15th Century" });
var listOfSaintsByCentury16 = extractProcess.SaintsExtractByCentury("/wiki/Chronological_list_of_saints_and_blesseds_in_the_16th_century", new List<string> { "All", "16th Century" });
var listOfSaintsByCentury17 = extractProcess.SaintsExtractByCentury("/wiki/Chronological_list_of_saints_and_blesseds_in_the_17th_century", new List<string> { "All", "17th Century" });
var listOfSaintsByCentury18 = extractProcess.SaintsExtractByCentury("/wiki/Chronological_list_of_saints_and_blesseds_in_the_18th_century", new List<string> { "All", "18th Century" });
var listOfSaintsByCentury19 = extractProcess.SaintsExtractByCentury("/wiki/Chronological_list_of_saints_and_blesseds_in_the_19th_century", new List<string> { "All", "19th Century" });
var listOfSaintsByCentury20 = extractProcess.SaintsExtractByCentury("/wiki/Chronological_list_of_saints_and_blesseds_in_the_20th_century", new List<string> { "All", "20th Century" });
var listOfSaintsByCentury21 = extractProcess.SaintsExtractByCentury("/wiki/Chronological_list_of_saints_and_blesseds_in_the_21st_century", new List<string> { "All", "21th Century" });

var listOfSaintsByAllPope = extractProcess.SaintsExtractByAllPopeListData("/wiki/List_of_saints_by_pope", new List<string> { "All", "By Pope" });
var listOfPatronSaints = extractProcess.SaintsExtractPatronSaintsListData("/wiki/List_of_patron_saints_by_occupation_and_activity", new List<string> { "All", "Patron Saints" });
var listOfSaints = extractProcess.SaintsExtractListTabularData("/wiki/List_of_saints", new List<string> { "All" });
var listOfBeatified = extractProcess.SaintsExtractListTabularData("/wiki/List_of_beatified_people", new List<string> { "All", "Beatified" });

var saintsCollection = listOfPatronSaints
    .Union(listOfSaintsByAllPope)
    .Union(listOfBeatified)
    .Union(listOfSaints)
    .Union(listOfSaintsByEachPope01).Union(listOfSaintsByEachPope02).Union(listOfSaintsByEachPope03).Union(listOfSaintsByEachPope04).Union(listOfSaintsByEachPope05)
    .Union(listOfSaintsByEachPope06).Union(listOfSaintsByEachPope07).Union(listOfSaintsByEachPope08).Union(listOfSaintsByCentury1)
    .Union(listOfSaintsByCentury1).Union(listOfSaintsByCentury2).Union(listOfSaintsByCentury3).Union(listOfSaintsByCentury4).Union(listOfSaintsByCentury5)
    .Union(listOfSaintsByCentury6).Union(listOfSaintsByCentury7).Union(listOfSaintsByCentury8).Union(listOfSaintsByCentury9).Union(listOfSaintsByCentury10)
    .Union(listOfSaintsByCentury11).Union(listOfSaintsByCentury12).Union(listOfSaintsByCentury13).Union(listOfSaintsByCentury14).Union(listOfSaintsByCentury15)
    .Union(listOfSaintsByCentury16).Union(listOfSaintsByCentury17).Union(listOfSaintsByCentury18).Union(listOfSaintsByCentury19).Union(listOfSaintsByCentury20)
    .Union(listOfSaintsByCentury21)
    //.Take(100)
    .ToList()
    .WithDefaultFilters();

int totalCount = saintsCollection.Count;
int currentIndex = 1;

//foreach (var saints in saintsCollection)
Parallel.ForEach(saintsCollection, new ParallelOptions { MaxDegreeOfParallelism = 1 }, saint =>
{
    try
    {
        extractProcess.PersonaSinglePageContentExtractWithSaveToStore(saint);
        Console.WriteLine($"[{currentIndex}/{totalCount}] [{(int)(((decimal)currentIndex / (decimal)totalCount) * 100)}%] Saints [{saint.Title}]: {saint.Route}");
        //Thread.Sleep(1000);
        currentIndex = currentIndex + 1;
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }

});

//var htmlContent = extractProcess.PersonaSinglePageContentExtractWithSaveToStore("/wiki/Pope_John_Paul_II", string.Empty);

////var tt = serviceProvider.GetService<WikiAppController>().GetViewModelByRoute("/wiki/Pope_John_Paul_II");
//var tt01 = serviceProvider.GetService<WikiAppController>().GetListOfWikiItems().ToList();

var pp = appCtrl.GetListOfWikiItems(new List<string> { "Canonized by Pope Leo XIII" });
var ppA = appCtrl.GetListOfWikiItems(new List<string> { "All" });
var tt = appCtrl.GetViewModelByRoute("/wiki/Paul_the_Apostle");


int u = 0;


