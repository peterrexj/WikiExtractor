using GeneralInformation.Models.Mix;
using GeneralInformation.Repository;
using Pj.Library;
using Syncfusion.DataSource;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using static GeneralInformation.Models.Mix.MainListSortDescriptorModel;

namespace GeneralInformation.Exts
{
    public class SettingsHelper
    {
        public static SortByAttribute GetSortAttributeBySelected(MainListSortDescriptorModel sortData)
        {
            return GetSortAttributeBySelected(sortData.PropertyName, sortData.Direction);
        }
        public static SortByAttribute GetSortAttributeBySelected(string propertyName, ListSortDirection direction)
        {
            if (propertyName == "Name" && direction == ListSortDirection.Ascending)
            {
                return SortByAttribute.AtoZ;
            }
            else if (propertyName == "Name" && direction == ListSortDirection.Descending)
            {
                return SortByAttribute.ZtoA;
            }
            else if (propertyName == "ItemReadStatus" && direction == ListSortDirection.Descending)
            {
                return SortByAttribute.Read;
            }
            else if (propertyName == "ItemReadStatus" && direction == ListSortDirection.Ascending)
            {
                return SortByAttribute.UnRead;
            }
            else if (propertyName == "RandomId" && direction == ListSortDirection.Ascending)
            {
                return SortByAttribute.Random;
            }
            else
            {
                return SortByAttribute.Default;
            }
        }
        public static MainListSortDescriptorModel GetSortDescriptorBySelectedItem(SortByAttribute sortByAttribute)
        {
            switch (sortByAttribute)
            {
                case SortByAttribute.AtoZ:
                    return new MainListSortDescriptorModel { PropertyName = "Name", Direction = Syncfusion.DataSource.ListSortDirection.Ascending };
                case SortByAttribute.ZtoA:
                    return new MainListSortDescriptorModel { PropertyName = "Name", Direction = Syncfusion.DataSource.ListSortDirection.Descending };
                case SortByAttribute.Read:
                    return new MainListSortDescriptorModel { PropertyName = "ItemReadStatus", Direction = Syncfusion.DataSource.ListSortDirection.Descending };
                case SortByAttribute.UnRead:
                    return new MainListSortDescriptorModel { PropertyName = "ItemReadStatus", Direction = Syncfusion.DataSource.ListSortDirection.Ascending };
                case SortByAttribute.Default:
                    return new MainListSortDescriptorModel { PropertyName = "Id", Direction = Syncfusion.DataSource.ListSortDirection.Ascending };
                case SortByAttribute.Random:
                    return new MainListSortDescriptorModel { PropertyName = "RandomId", Direction = Syncfusion.DataSource.ListSortDirection.Ascending };
                default:
                    return new MainListSortDescriptorModel { PropertyName = "Id", Direction = Syncfusion.DataSource.ListSortDirection.Ascending };

            }
        }
        public static MainListSortDescriptorModel GetCurrentSortDescriptor()
        {
            var propertyName = DatabaseService.UserStoreDatabase.SettingsRepository.Get(f => f.Name == "SortPropertyName").FirstOrDefault();
            if (propertyName == null || propertyName.Value.IsEmpty())
            {
                SaveSortDescriptor("Id", "ascending");
                return new MainListSortDescriptorModel { PropertyName = "Id", Direction = Syncfusion.DataSource.ListSortDirection.Ascending };

            }
            else
            {
                var direction = DatabaseService.UserStoreDatabase.SettingsRepository.Get(f => f.Name == "SortDirection").FirstOrDefault();
                ListSortDirection listSortDirection = ListSortDirection.Ascending;

                if (direction != null || direction.Value.HasValue())
                {
                    if (direction.Value == "Descending")
                    {
                        listSortDirection = ListSortDirection.Descending;
                    }
                }
                return new MainListSortDescriptorModel { PropertyName = propertyName.Value, Direction = listSortDirection };
            }
        }

        public static void SaveSortDescriptor(string propertyName, string direction)
        {
            DatabaseService.UserStoreDatabase.SettingsRepository.Update("SortPropertyName", propertyName);
            DatabaseService.UserStoreDatabase.SettingsRepository.Update("SortDirection", direction);
        }

        public static bool ShouldShowAlreadyReadItem()
        {
            return DatabaseService.UserStoreDatabase.SettingsRepository.Get(f => f.Name == "ShowAlreadyReadItem").FirstOrDefault()?.Value?.ToBool() ?? true;
        }

        public static void SaveShouldShowAlreadyReadItems(bool shouldShowAlreadyReadItems)
        {
            DatabaseService.UserStoreDatabase.SettingsRepository.Update("ShowAlreadyReadItem", shouldShowAlreadyReadItems.ToString());
        }

        public static AppThemes SelectedTheme
        {
            get
            {
                var direction = DatabaseService.UserStoreDatabase.SettingsRepository.Get(f => f.Name == "AppTheme").FirstOrDefault();
                if (direction == null || direction.Value.IsEmpty())
                {
                    return AppThemes.Light;
                }
                else
                {
                    return (AppThemes)Enum.Parse(typeof(AppThemes), direction.Value);
                }
            }
        }
        public static void SaveTheme(AppThemes theme)
        {
            DatabaseService.UserStoreDatabase.SettingsRepository.Update("AppTheme", theme.ToString());
        }

        private static SpeechOptions speechOptions;
        public async static Task<SpeechOptions> SpeechSettings()
        {
            if (speechOptions == null)
            {
                IEnumerable<Locale> locales = await TextToSpeech.GetLocalesAsync(); 

                Locale locale = null;

                if (locales != null)
                {
                    var cultureName = CultureInfo.CurrentCulture.NativeName;
                    locale = locales?.Where(f => f.Name == cultureName).FirstOrDefault();
                    locale ??= locales?.Where(f => f.Name == "English (Australia)").FirstOrDefault();
                    locale ??= locales?.Where(f => f.Name.StartsWith("English")).FirstOrDefault();
                }

                var settings = new SpeechOptions()
                {
                    Volume = 1.0f,
                    Pitch = 1.0f,
                    Locale = locale
                };
                speechOptions = settings;
            }
            return speechOptions;
        }
    }
}
