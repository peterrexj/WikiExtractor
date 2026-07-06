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

            ProcessConstants.UseCache = true;
            ProcessConstants.RequestDelayInMilliseconds = ProcessConstants.UseCache ? 0 : 2000;

            string mode = "all-extract";
            string? target = null;
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i].Equals("--mode", StringComparison.OrdinalIgnoreCase))
                    mode = args[i + 1].ToLower();
                else if (args[i].Equals("--target", StringComparison.OrdinalIgnoreCase))
                    target = args[i + 1];
            }

            Console.WriteLine($"[Mode] {mode}");
            if (target != null) Console.WriteLine($"[Target] {target}");
            Console.WriteLine();

            switch (mode)
            {
                // ── WorldLeaders ──────────────────────────────────────────────────────
                case "worldleaders-single":
                    RunWorldLeaders(target);
                    RunWorldLeadersPostProcess();
                    break;

                case "worldleaders-extract":
                    RunWorldLeaders(target);
                    break;

                case "worldleaders-postprocess":
                    RunWorldLeadersPostProcess();
                    break;

                        case "worldleaders-full":
                    RunWorldLeaders(target);
                    RunWorldLeadersPostProcess();
                    break;

                // ── Saints ────────────────────────────────────────────────────────────
                case "saints-single":
                    RunSaints(target);
                    RunSaintsPostProcess();
                    break;

                case "saints-extract":
                    RunSaints(target);
                    break;

                case "saints-postprocess":
                    RunSaintsPostProcess();
                    break;

                case "saints-full":
                    RunSaints(target);
                    RunSaintsPostProcess();
                    break;

                // ── Popes ─────────────────────────────────────────────────────────────
                case "popes-single":
                    RunPopes(target);
                    RunPopesPostProcess();
                    break;

                case "popes-extract":
                    RunPopes(target);
                    break;

                case "popes-postprocess":
                    RunPopesPostProcess();
                    break;

                case "popes-full":
                    RunPopes(target);
                    RunPopesPostProcess();
                    break;

                // ── Countries ─────────────────────────────────────────────────────────
                case "countries-single":
                    RunCountries(target);
                    RunCountriesPostProcess();
                    break;

                case "countries-extract":
                    RunCountries(target);
                    break;

                case "countries-postprocess":
                    RunCountriesPostProcess();
                    break;

                case "countries-full":
                    RunCountries(target);
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

        private static void RunWorldLeaders(string? target = null)
        {
            var e = new WorldLeadersExtractor();
            e.ExtractData(target);
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
            e.WritePostProcessReport();
        }

        private static void RunSaints(string? target = null)
        {
            var e = new SaintsDataExtractor();
            e.ExtractData(target);
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
            e.WritePostProcessReport();
        }

        private static void RunPopes(string? target = null)
        {
            var e = new PopesDataExtractor();
            e.ExtractData(target);
        }

        private static void RunPopesPostProcess()
        {
            var e = new PopesDataExtractor();
            e.EnablePrimaryMetadataContent();
            e.EnableQuizData("PopesQuizDefinition.json");
            e.CopyDatabaseFileToRootDbFolder();
            e.QuizDataInsightsToBuildQuiz("Popes");
            e.WritePostProcessReport();
        }

        private static void RunCountries(string? target = null)
        {
            var e = new CountriesDataExtractor();
            e.ExtractData(target);
        }

        private static void RunCountriesPostProcess()
        {
            var e = new CountriesDataExtractor();
            e.EnablePrimaryMetadataContent();
            e.CopyDatabaseFileToRootDbFolder();
            e.WritePostProcessReport();
        }
    }
}
