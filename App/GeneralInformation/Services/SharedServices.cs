using GeneralInformation.Converters;
using GeneralInformation.Models.ViewModels;
using GeneralInformation.Repository;
using WikiExtractor.Process;
using Xamarin.Forms;

namespace GeneralInformation.Services
{
    public class SharedServices
    {
        private static WikiAppController _wikiAppController;
        public static WikiAppController WikiAppController => _wikiAppController ??= new WikiAppController(DatabaseService.AppDatabase, DatabaseService.UserStoreDatabase);

        private static StringToColorConverter _toColorConverter;
        public static StringToColorConverter ToColorConverter => _toColorConverter ??= new StringToColorConverter();
        public static IValueConverter ToColorConverterAsValueConverter => ToColorConverter;

        public static PageDataTransferModel _pageDataTransferModel;
        public static PageDataTransferModel PageDataTransferModel => _pageDataTransferModel ??= new PageDataTransferModel();
    }
}
