using Pj.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.Process.Extractor;
using WikiExtractor.Repository;
using WikiExtractor.Repository.UserStore;

namespace WikiExtractor.Process.Modules
{
    public class DataExtractorBase
    {
        protected WikiAppController? wikiAppController = null;
        protected readonly object _lock = new object();

        public DataExtractorBase(string extractorName, string dbFileName)
        {
            Console.WriteLine($"Hello, {extractorName} Extractor!");
            ProcessConstants.CacheFolder = IoHelper.CombinePath(PjUtility.Runtime.ExecutingRepositoryRootFolder, "Cache");
            ProcessConstants.DatabasePath = IoHelper.CombinePath(PjUtility.Runtime.ExecutingFolder, "Db", dbFileName);
            ProcessConstants.UserStoreDatabasePath = IoHelper.CombinePath(PjUtility.Runtime.ExecutingFolder, "Db", "UserStore.db");
        }

        protected virtual void Initialize(bool doClean)
        {
            if (doClean)
            {
                IoHelper.DeleteFile(ProcessConstants.DatabasePath);
                IoHelper.DeleteFile(ProcessConstants.UserStoreDatabasePath);
            }
            wikiAppController = new WikiAppController(new WikiDatabase(), new UserStoreDatabase());
        }

        public void CopyDatabaseFileToRootDbFolder()
        {
            IoHelper.CopyFile(ProcessConstants.DatabasePath,
                IoHelper.CombinePath(PjUtility.Runtime.ExecutingRepositoryRootFolder, $"App\\Databases\\{Path.GetFileName(ProcessConstants.DatabasePath)}"));
        }
    }
}
