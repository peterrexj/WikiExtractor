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

        protected static void LogPhase(string label) =>
            Console.WriteLine($"\n{'='.ToString().PadRight(70, '=')} \n  PHASE: {label}\n{'='.ToString().PadRight(70, '=')}");

        protected static void LogProgress(string phase, int current, int total, long phaseStartMs, string itemName)
        {
            var elapsed = TimeSpan.FromMilliseconds(Environment.TickCount64 - phaseStartMs);
            var pct = total > 0 ? (int)((decimal)current / total * 100) : 0;
            string eta;
            if (current > 1 && total > 0)
            {
                var msPerItem = elapsed.TotalMilliseconds / (current - 1);
                var remaining = TimeSpan.FromMilliseconds(msPerItem * (total - current + 1));
                eta = remaining.TotalHours >= 1
                    ? $"{(int)remaining.TotalHours}h {remaining.Minutes:D2}m"
                    : remaining.TotalMinutes >= 1
                        ? $"{(int)remaining.TotalMinutes}m {remaining.Seconds:D2}s"
                        : $"{remaining.Seconds}s";
            }
            else
            {
                eta = "calculating…";
            }
            Console.WriteLine($"  [{phase}] {current}/{total} ({pct}%)  ETA: {eta}  |  {itemName}");
        }

        protected static void LogPhaseSummary(string phase, int total, long phaseStartMs)
        {
            var elapsed = TimeSpan.FromMilliseconds(Environment.TickCount64 - phaseStartMs);
            var avg = total > 0 ? elapsed.TotalSeconds / total : 0;
            Console.WriteLine($"  [{phase}] Done — {total} items in {elapsed:mm\\:ss}  (avg {avg:F1}s/item)");
        }

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
                // Only delete the main wiki DB — UserStore.db holds user data (favourites,
                // read state, streaks) that must survive extraction re-runs.
                AppDatabase.IsInitialized = false;
                IoHelper.DeleteFile(ProcessConstants.DatabasePath);
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

        public void WritePostProcessReport()
        {
            Initialize(false);
            var reportFolder = Path.Combine(Path.GetDirectoryName(ProcessConstants.DatabasePath)!, "..", "ExtractionReports");
            var reporter = new PostProcessReporter(wikiAppController!, ProcessConstants.DatabasePath, reportFolder);
            reporter.Write();
        }
    }
}
