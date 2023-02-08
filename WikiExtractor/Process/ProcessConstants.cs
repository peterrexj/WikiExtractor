using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiExtractor.Process
{
    public static class ProcessConstants
    {
        public static string DatabasePath { get; set; }
        public static string CacheFolder { get; set; }
        public static bool UseCache { get; set; } = true;
    }
}
