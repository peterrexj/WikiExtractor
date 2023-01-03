// See https://aka.ms/new-console-template for more information


using WikiExtractor;
using WikiExtractor.Process.Modules;

var serviceProvider = ContainerConfiguration.Configure();
var saintsExtractor = new SaintsDataExtractor();
var popesExtractor = new PopesDataExtractor();

popesExtractor.ExtractData();

//saintsExtractor.ExtractData();
////saintsExtractor.TestData();
//saintsExtractor.DumpData();


int u = 0;


