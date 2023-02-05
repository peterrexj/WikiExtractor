// See https://aka.ms/new-console-template for more information


using WikiExtractor;
using WikiExtractor.Process.Modules;

var serviceProvider = ContainerConfiguration.Configure();


//var saintsExtractor = new SaintsDataExtractor();
//saintsExtractor.Test();
//saintsExtractor.ExtractData();
//saintsExtractor.TestData();
//saintsExtractor.CleanDataWithDump();

//var popesExtractor = new PopesDataExtractor();
//popesExtractor.Test();
//popesExtractor.ExtractData();

var countriesExtractor = new CountriesDataExtractor();
countriesExtractor.ExtractData();
countriesExtractor.Test();






int u = 0;


