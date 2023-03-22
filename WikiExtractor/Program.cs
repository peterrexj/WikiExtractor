// See https://aka.ms/new-console-template for more information


using WikiExtractor;
using WikiExtractor.Process;
using WikiExtractor.Process.Modules;

var serviceProvider = ContainerConfiguration.Configure();

ProcessConstants.UseCache = true;

var saintsExtractor = new SaintsDataExtractor();
saintsExtractor.ExtractData();
saintsExtractor.EnablePrimaryMetadataContent();
saintsExtractor.CleanDataWithDump();
//saintsExtractor.TestData();
saintsExtractor.CopyDatabaseFileToRootDbFolder();



//var popesExtractor = new PopesDataExtractor();
//popesExtractor.ExtractData();
//popesExtractor.EnablePrimaryMetadataContent();
//popesExtractor.CopyDatabaseFileToRootDbFolder();
//popesExtractor.Test();

//var countriesExtractor = new CountriesDataExtractor();
//countriesExtractor.ExtractData();
////countriesExtractor.Test();
//countriesExtractor.EnablePrimaryMetadataContent();
//countriesExtractor.CopyDatabaseFileToRootDbFolder();

int u = 0;