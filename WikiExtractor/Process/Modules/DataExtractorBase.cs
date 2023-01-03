using Pj.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.Process.Extractor;
using WikiExtractor.Repository;

namespace WikiExtractor.Process.Modules
{
    public class DataExtractorBase
    {
        protected WikiAppController? wikiAppController = null;
        protected SaintsWikiExtractionToStore? wikiPageExtractionStore = null;

        public DataExtractorBase(string extractorName, string dbFileName)
        {
            Console.WriteLine($"Hello, {extractorName} Extractor!");
            ProcessConstants.CacheFolder = IoHelper.CombinePath(PjUtility.Runtime.ExecutingFolder, "Cache");
            ProcessConstants.DatabasePath = IoHelper.CombinePath(PjUtility.Runtime.ExecutingFolder, "Db", dbFileName);
        }

        protected void Initialize(bool doClean)
        {
            if (doClean)
            {
                IoHelper.DeleteFile(ProcessConstants.DatabasePath);
            }
            wikiAppController = new WikiAppController(new WikiDatabase());
            wikiPageExtractionStore = new SaintsWikiExtractionToStore();
        }
    }
}
