using Pj.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.Process.Extractor;

namespace WikiExtractor.Process.Modules
{
    internal class PopesDataExtractor
    {
        private WikiAppController? wikiAppController = null;
        private SaintsWikiExtractionToStore? wikiPageExtractionStore = null;

        public PopesDataExtractor()
        {
            Console.WriteLine("Hello, Popes Extractor!");
            ProcessConstants.CacheFolder = IoHelper.CombinePath(PjUtility.Runtime.ExecutingFolder, "Cache");
            ProcessConstants.DatabasePath = IoHelper.CombinePath(PjUtility.Runtime.ExecutingFolder, "Db", "WikiStoreSaints.db");
        }
    }
}
