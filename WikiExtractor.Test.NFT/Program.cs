using Pj.Library;
using WikiExtractor.Process;
using WikiExtractor.Repository.UserStore;
using WikiExtractor.Repository;
using BenchmarkDotNet.Running;
using WikiExtractor.ViewModels;

namespace WikiExtractor.Test.NFT
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<DatabaseBenchmarks>();


            //BenchmarkRunner.Run<ExtensionBenchmarks>();

            //var dbRunner = new DatabaseBenchmarks();
            //var item1 = dbRunner.LoadDbOption1();
            //var item2 = dbRunner.LoadDbOption2();
            //SerializationHelper.SerializeToJson(item1, "C:\\GIT\\Other\\peterrexj\\WikiExtractor\\App\\Databases\\item1.json");
            //SerializationHelper.SerializeToJson(item2, "C:\\GIT\\Other\\peterrexj\\WikiExtractor\\App\\Databases\\item2.json");
            //var l01 = new FileInfo("C:\\GIT\\Other\\peterrexj\\WikiExtractor\\App\\Databases\\item1.json").Length;
            //var l02 = new FileInfo("C:\\GIT\\Other\\peterrexj\\WikiExtractor\\App\\Databases\\item2.json").Length;
            //var isLengthSame = l01 == l02;

        }
    }
}
