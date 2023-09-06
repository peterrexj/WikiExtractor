using GeneralInformation.Converters;
using GeneralInformation.Repository;
using WikiExtractor.Process;
using Xamarin.Forms;

namespace GeneralInformation.Services
{
    public class SharedServices
    {
        private static WikiAppController _wikiAppController;
        public static WikiAppController WikiAppController
        {
            get
            {
                _wikiAppController ??= new WikiAppController(DatabaseService.AppDatabase, DatabaseService.UserStoreDatabase);
                return _wikiAppController;
            }
        }

        private static StringToColorConverter _toColorConverter;
        public static StringToColorConverter ToColorConverter
        {
            get
            {
                _toColorConverter ??= new StringToColorConverter();
                return _toColorConverter;
            }
        }
        public static IValueConverter ToColorConverterAsValueConverter => ToColorConverter;
    }
}
