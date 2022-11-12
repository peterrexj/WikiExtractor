// See https://aka.ms/new-console-template for more information


using Microsoft.Extensions.DependencyInjection;
using Pj.Library;
using WikiExtractor;
using WikiExtractor.Process;
using WikiExtractor.Repository;

var serviceProvider = ContainerConfiguration.Configure();
//serviceProvider.GetService<ContinuousRunningProcessor>().Process();

Console.WriteLine("Hello, World!");

ProcessConstants.CacheFolder = IoHelper.CombinePath(PjUtility.Runtime.ExecutingFolder, "Cache");
ProcessConstants.DatabasePath = IoHelper.CombinePath(PjUtility.Runtime.ExecutingFolder, "Db", "WikiStore.db");
//IoHelper.DeleteFile(ProcessConstants.DatabasePath);
WikiAppController wikiAppController = new WikiAppController(new WikiDatabase());

var tt = serviceProvider.GetService<WikiAppController>().GetViewModelByRoute("/wiki/Albertus_Magnus");

//Check Metadata
var items = wikiAppController.GetListOfWikiItems().ToList();
wikiAppController.MetadataBuild();


var extractProcess = new WikiPageRawExtraction();
var listOfSaints = extractProcess.TabularPageContentExtractWithSave("/wiki/List_of_saints");
//var listOfBeatified = extractProcess.TabularPageContentExtractWithSave("/wiki/List_of_beatified_people");

foreach (var saints in listOfSaints)
{
    extractProcess.PersonaSinglePageContentExtractWithSaveToStore(saints.Value, saints.Key);
}

var htmlContent = extractProcess.PersonaSinglePageContentExtractWithSaveToStore("/wiki/Pope_John_Paul_II", string.Empty);
var htmlContent1 = extractProcess.PersonaSinglePageContentExtractWithSaveToStore("/wiki/Pope_Benedict_XVI", string.Empty);
var htmlContent2 = extractProcess.PersonaSinglePageContentExtractWithSaveToStore("/wiki/Abāmūn_of_Tarnūt", string.Empty);


//var tt = serviceProvider.GetService<WikiAppController>().GetViewModelByRoute("/wiki/Pope_John_Paul_II");
var tt01 = serviceProvider.GetService<WikiAppController>().GetListOfWikiItems().ToList();

//var appCtrl = new WikiAppController();
//var tt = appCtrl.GetViewModel("/wiki/Pope_John_Paul_II");


int u = 0;


