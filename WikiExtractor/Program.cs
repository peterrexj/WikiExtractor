// See https://aka.ms/new-console-template for more information


using WikiExtractor;
using WikiExtractor.Process.Modules;

var serviceProvider = ContainerConfiguration.Configure();

//var saintsExtractor = new SaintsDataExtractor();
//saintsExtractor.EnablePrimaryMetadataContent();
//saintsExtractor.CopyDatabaseFileToRootDbFolder();

//saintsExtractor.ExtractData();
//saintsExtractor.TestData();
//saintsExtractor.CleanDataWithDump();

//var popesExtractor = new PopesDataExtractor();
////popesExtractor.Test();
////popesExtractor.ExtractData();
//popesExtractor.EnablePrimaryMetadataContent();
//popesExtractor.CopyDatabaseFileToRootDbFolder();

var countriesExtractor = new CountriesDataExtractor();
//countriesExtractor.ExtractData();
//countriesExtractor.Test();
countriesExtractor.EnablePrimaryMetadataContent();
countriesExtractor.CopyDatabaseFileToRootDbFolder();


int u = 0;


