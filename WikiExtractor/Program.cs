// See https://aka.ms/new-console-template for more information


using WikiExtractor;
using WikiExtractor.Process.Modules;

var serviceProvider = ContainerConfiguration.Configure();
var saintsExtractor = new SaintsDataExtractor();
//var popesExtractor = new PopesDataExtractor();
//var countriesExtractor = new CountriesDataExtractor();

//popesExtractor.Test();
//popesExtractor.ExtractData();

saintsExtractor.Test();
//saintsExtractor.ExtractData();
//saintsExtractor.TestData();
//saintsExtractor.DumpData();

//countriesExtractor.ExtractData();

int u = 0;


