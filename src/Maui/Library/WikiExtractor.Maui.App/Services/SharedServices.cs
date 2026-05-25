using WikiExtractor.Maui.App.Converters;
using WikiExtractor.Maui.App.Models;
using WikiExtractor.Maui.App.Repository;
using WikiExtractor.Process;
using WikiExtractor.Process.Process;
using WikiExtractor.ViewModels;

namespace WikiExtractor.Maui.App.Services
{
    public class SharedServices
    {
        private static WikiAppController _wikiAppController;
        public static WikiAppController WikiAppController => _wikiAppController ??= new WikiAppController(DatabaseService.AppDatabase, DatabaseService.UserStoreDatabase);

        public static IAppInformation AppInfo => SharedServiceCore.AppInformation;

        private static QuizController _quizController;
        public static QuizController QuizController => _quizController ??= new QuizController(DatabaseService.AppDatabase, DatabaseService.UserStoreDatabase);

        public static PageDataTransferModel _pageDataTransferModel;
        public static PageDataTransferModel PageDataTransferModel => _pageDataTransferModel ??= new PageDataTransferModel();

        // Pre-loaded list data keyed by tag string (e.g. "all" or comma-joined tags)
        private static readonly Dictionary<string, List<PersonaViewModel>> _preloadedPersonas = new();

        public static void StorePreloadedPersonas(string tagKey, List<PersonaViewModel> personas)
        {
            _preloadedPersonas[tagKey ?? ""] = personas;
        }

        public static List<PersonaViewModel> ConsumePreloadedPersonas(string tagKey)
        {
            var key = tagKey ?? "";
            if (_preloadedPersonas.TryGetValue(key, out var data))
            {
                _preloadedPersonas.Remove(key);
                return data;
            }
            return null;
        }
    }
}