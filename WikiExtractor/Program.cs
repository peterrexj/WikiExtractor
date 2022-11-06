// See https://aka.ms/new-console-template for more information


using Pj.Library;
using WikiExtractor.Process;

Console.WriteLine("Hello, World!");

ProcessConstants.CacheFolder = IoHelper.CombinePath(PjUtility.Runtime.ExecutingFolder, "Cache");
ProcessConstants.DatabasePath = IoHelper.CombinePath(PjUtility.Runtime.ExecutingFolder, "Db", "WikiStore.db");
IoHelper.DeleteFile(ProcessConstants.DatabasePath);

var extractProcess = new WikiPageRawExtraction();
//var listOfSaints = extractProcess.TabularPageContentExtractWithSave("/wiki/List_of_saints");
//var listOfBeatified = extractProcess.TabularPageContentExtractWithSave("/wiki/List_of_beatified_people");

var htmlContent = extractProcess.PersonaSinglePageContentExtractWithSaveToStore("/wiki/Pope_John_Paul_II");
var htmlContent1 = extractProcess.PersonaSinglePageContentExtractWithSaveToStore("/wiki/Pope_Benedict_XVI");
var htmlContent2 = extractProcess.PersonaSinglePageContentExtractWithSaveToStore("/wiki/Abāmūn_of_Tarnūt");


var appCtrl = new WikiAppController();
var tt = appCtrl.GetViewModel("/wiki/Pope_John_Paul_II");


int u = 0;


