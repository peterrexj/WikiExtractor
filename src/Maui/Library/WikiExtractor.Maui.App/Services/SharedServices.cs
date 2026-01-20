using WikiExtractor.Maui.App.Converters;
using WikiExtractor.Maui.App.Models;
using WikiExtractor.Maui.App.Repository;
using WikiExtractor.Process;
using WikiExtractor.Process.Process;

namespace WikiExtractor.Maui.App.Services
{
    public class SharedServices
    {
        private static WikiAppController _wikiAppController;
        public static WikiAppController WikiAppController => _wikiAppController ??= new WikiAppController(DatabaseService.AppDatabase, DatabaseService.UserStoreDatabase);

        private static QuizController _quizController;
        public static QuizController QuizController => _quizController ??= new QuizController(DatabaseService.AppDatabase, DatabaseService.UserStoreDatabase);

        public static PageDataTransferModel _pageDataTransferModel;
        public static PageDataTransferModel PageDataTransferModel => _pageDataTransferModel ??= new PageDataTransferModel();
    }
}