using Pj.Library;
using WikiExtractor.Process.Models;
using WikiExtractor.Process.Process;
using WikiExtractor.Repository;
using WikiExtractor.Repository.UserStore;

namespace WikiExtractor.Process.Modules
{
    public class DataExtractorBase
    {
        protected WikiAppController? wikiAppController = null;
        protected QuizController? QuizController = null;
        protected QuizInsightsController? QuizInsightsController = null;

        protected readonly object _lock = new object();

        public DataExtractorBase(string extractorName, string dbFileName)
        {
            Console.WriteLine($"Hello, {extractorName} Extractor!");
            ProcessConstants.CacheFolder = IoHelper.CombinePath(PjUtility.Runtime.ExecutingRepositoryRootFolder, "Tools", "Cache");
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

            var wikiDb = new WikiDatabase();
            var userDb = new UserStoreDatabase();
            wikiAppController = new WikiAppController(wikiDb, userDb);
            QuizController = new QuizController(wikiDb, userDb);
            QuizInsightsController = new QuizInsightsController(wikiDb, userDb);
        }

        public void CopyDatabaseFileToRootDbFolder()
        {
            IoHelper.CopyFile(ProcessConstants.DatabasePath,
                IoHelper.CombinePath(PjUtility.Runtime.ExecutingRepositoryRootFolder, "Resources", "Databases",  Path.GetFileName(ProcessConstants.DatabasePath)));
        }

        public void EnableQuizData(string fileName)
        {
            Initialize(false);

            var quizDefinitionPath = IoHelper.CombinePath(PjUtility.Runtime.ExecutingFolder,
                "Resources", "Quiz", fileName);

            var quizDefinitionData = SerializationHelper.DeSerializeFromJsonFile<List<QuizDefinitionJsonModel>>(quizDefinitionPath);

            QuizController?.QuizEnableDbWithDetails(quizDefinitionData);
        }

        public void QuizDataInsightsToBuildQuiz(string appName)
        {
            Initialize(false);
            QuizInsightsController.QuizDataInsightsToBuildQuiz(appName);
            QuizInsightsController.ExportQuizDataVisualQuestionsToCsv(appName);
        }
    }
}
