// See https://aka.ms/new-console-template for more information


using WikiExtractor;
using WikiExtractor.Process;
using WikiExtractor.Process.Modules;

var serviceProvider = ContainerConfiguration.Configure();

ProcessConstants.UseCache = true;

var worldLeadersExtractor = new WorldLeadersExtractor();
worldLeadersExtractor.ExtractData();
worldLeadersExtractor.CleanDataWithDump();
worldLeadersExtractor.EnablePrimaryMetadataContent();
worldLeadersExtractor.EnableQuizData("WorldLeadersQuizDefinition.json");
worldLeadersExtractor.CopyDatabaseFileToRootDbFolder();
worldLeadersExtractor.QuizDataInsightsToBuildQuiz("WorldLeaders");


//var saintsExtractor = new SaintsDataExtractor();
//saintsExtractor.ExtractData();
//saintsExtractor.EnablePrimaryMetadataContent();
//saintsExtractor.CleanDataWithDump();
//saintsExtractor.EnableQuizData("SaintsQuizDefinition.json");
////saintsExtractor.TestData();
//saintsExtractor.CopyDatabaseFileToRootDbFolder();
//saintsExtractor.QuizDataInsightsToBuildQuiz("Saints");


//var popesExtractor = new PopesDataExtractor();
//popesExtractor.ExtractData();
//popesExtractor.EnablePrimaryMetadataContent();
//popesExtractor.EnableQuizData("PopesQuizDefinition.json");
//popesExtractor.CopyDatabaseFileToRootDbFolder();
//popesExtractor.Test();
//popesExtractor.QuizDataInsightsToBuildQuiz("Popes");


//var countriesExtractor = new CountriesDataExtractor();
//countriesExtractor.ExtractData();
////countriesExtractor.Test();
//countriesExtractor.EnablePrimaryMetadataContent();
//countriesExtractor.CopyDatabaseFileToRootDbFolder();
int u = 0;