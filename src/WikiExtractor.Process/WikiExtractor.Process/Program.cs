using WikiExtractor;
using WikiExtractor.Process;
using WikiExtractor.Process.Modules;

namespace WikiExtractor.Process
{
    public class DataExtractionProgram
    {
        public static void Main(string[] args)
        {
            var serviceProvider = ContainerConfiguration.Configure();

            ProcessConstants.UseCache = false;
            ProcessConstants.RequestDelayInMilliseconds = ProcessConstants.UseCache ? 0 : 2000;

            string mode = "all-extract";
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i].Equals("--mode", StringComparison.OrdinalIgnoreCase))
                {
                    mode = args[i + 1].ToLower();
                    break;
                }
            }

            Console.WriteLine($"[Mode] {mode}");
            Console.WriteLine();

            switch (mode)
            {
                // ── WorldLeaders ──────────────────────────────────────────────────────
                case "worldleaders-extract":
                    RunWorldLeaders();
                    break;

                case "worldleaders-postprocess":
                    RunWorldLeadersPostProcess();
                    break;

                case "worldleaders-full":
                    RunWorldLeaders();
                    RunWorldLeadersPostProcess();
                    break;

                // ── Saints ────────────────────────────────────────────────────────────
                case "saints-extract":
                    RunSaints();
                    break;

                case "saints-postprocess":
                    RunSaintsPostProcess();
                    break;

                case "saints-full":
                    RunSaints();
                    RunSaintsPostProcess();
                    break;

                // ── Popes ─────────────────────────────────────────────────────────────
                case "popes-extract":
                    RunPopes();
                    break;

                case "popes-postprocess":
                    RunPopesPostProcess();
                    break;

                case "popes-full":
                    RunPopes();
                    RunPopesPostProcess();
                    break;

                // ── Countries ─────────────────────────────────────────────────────────
                case "countries-extract":
                    RunCountries();
                    break;

                case "countries-postprocess":
                    RunCountriesPostProcess();
                    break;

                case "countries-full":
                    RunCountries();
                    RunCountriesPostProcess();
                    break;

                // ── All ───────────────────────────────────────────────────────────────
                case "all-extract":
                    RunWorldLeaders();
                    RunSaints();
                    RunPopes();
                    RunCountries();
                    break;

                case "all-full":
                    RunWorldLeaders();
                    RunWorldLeadersPostProcess();
                    RunSaints();
                    RunSaintsPostProcess();
                    RunPopes();
                    RunPopesPostProcess();
                    RunCountries();
                    RunCountriesPostProcess();
                    break;

                default:
                    Console.WriteLine($"Unknown mode '{mode}'. Available modes:");
                    Console.WriteLine("  worldleaders-extract     saints-extract     popes-extract     countries-extract");
                    Console.WriteLine("  worldleaders-postprocess saints-postprocess popes-postprocess countries-postprocess");
                    Console.WriteLine("  worldleaders-full        saints-full        popes-full        countries-full");
                    Console.WriteLine("  all-extract  all-full");
                    break;
            }
        }

        // ── Runners ───────────────────────────────────────────────────────────────────

        private static void RunWorldLeaders()
        {
            var e = new WorldLeadersExtractor();
            e.ExtractData();
        }

        private static void RunWorldLeadersPostProcess()
        {
            var e = new WorldLeadersExtractor();
            e.EnablePrimaryMetadataContent();
            e.CleanDataWithDump();
            e.EnableQuizData("WorldLeadersQuizDefinition.json");
            e.TestData();
            e.CopyDatabaseFileToRootDbFolder();
            e.QuizDataInsightsToBuildQuiz("WorldLeaders");
        }

        private static void RunSaints()
        {
            var e = new SaintsDataExtractor();
            e.ExtractData();
        }

        private static void RunSaintsPostProcess()
        {
            var e = new SaintsDataExtractor();
            e.EnablePrimaryMetadataContent();
            e.CleanDataWithDump();
            e.EnableQuizData("SaintsQuizDefinition.json");
            e.TestData();
            e.CopyDatabaseFileToRootDbFolder();
            e.QuizDataInsightsToBuildQuiz("Saints");
        }

        private static void RunPopes()
        {
            var e = new PopesDataExtractor();
            e.ExtractData();
        }

        private static void RunPopesPostProcess()
        {
            var e = new PopesDataExtractor();
            e.EnablePrimaryMetadataContent();
            e.EnableQuizData("PopesQuizDefinition.json");
            e.CopyDatabaseFileToRootDbFolder();
            e.QuizDataInsightsToBuildQuiz("Popes");
        }

        private static void RunCountries()
        {
            var e = new CountriesDataExtractor();
            e.ExtractData();
        }

        private static void RunCountriesPostProcess()
        {
            var e = new CountriesDataExtractor();
            e.EnablePrimaryMetadataContent();
            e.CopyDatabaseFileToRootDbFolder();
        }
    }
}
