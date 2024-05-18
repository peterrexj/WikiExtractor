using BenchmarkDotNet.Attributes;
using Pj.Library;
using WikiExtractor.Exts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiExtractor.Test.NFT
{
    [MemoryDiagnoser]
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class ExtensionBenchmarks
    {
        private readonly string _data = "[Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]using System.Threading.Tasks;using System.Text;Peter";

        [Benchmark]
        public void PjLibrary()
        {
            var result = _data.ContainsIgnoreCase("Peter");
        }

        [Benchmark]
        public void Optimized()
        {
            var result = _data.ContainsOptimized("Peter", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
