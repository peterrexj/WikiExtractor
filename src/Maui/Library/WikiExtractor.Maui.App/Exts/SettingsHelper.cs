using GeneralInformation.Repository;
using Pj.Library;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Syncfusion.Maui.DataSource;
using WikiExtractor.Maui.App.Models.Mix;
using WikiExtractor.Maui.App.Repository;
using WikiExtractor.Maui.App.Services;

namespace WikiExtractor.Maui.App.Exts
{
    public class SettingsHelper
    {
        public static MainListSortDescriptorModel.SortByAttribute GetSortAttributeBySelected(MainListSortDescriptorModel sortData)
        {
            return GetSortAttributeBySelected(sortData.PropertyName, sortData.Direction);
        }
        public static MainListSortDescriptorModel.SortByAttribute GetSortAttributeBySelected(string propertyName, ListSortDirection direction)
        {
            if (propertyName == "Name" && direction == ListSortDirection.Ascending)
            {
                return MainListSortDescriptorModel.SortByAttribute.AtoZ;
            }
            else if (propertyName == "Name" && direction == ListSortDirection.Descending)
            {
                return MainListSortDescriptorModel.SortByAttribute.ZtoA;
            }
            else if (propertyName == "ItemReadStatus" && direction == ListSortDirection.Descending)
            {
                return MainListSortDescriptorModel.SortByAttribute.Read;
            }
            else if (propertyName == "ItemReadStatus" && direction == ListSortDirection.Ascending)
            {
                return MainListSortDescriptorModel.SortByAttribute.UnRead;
            }
            else if (propertyName == "RandomId" && direction == ListSortDirection.Ascending)
            {
                return MainListSortDescriptorModel.SortByAttribute.Random;
            }
            else
            {
                return MainListSortDescriptorModel.SortByAttribute.Default;
            }
        }
        public static MainListSortDescriptorModel GetSortDescriptorBySelectedItem(MainListSortDescriptorModel.SortByAttribute sortByAttribute)
        {
            switch (sortByAttribute)
            {
                case MainListSortDescriptorModel.SortByAttribute.AtoZ:
                    return new MainListSortDescriptorModel { PropertyName = "Name", Direction = ListSortDirection.Ascending };
                case MainListSortDescriptorModel.SortByAttribute.ZtoA:
                    return new MainListSortDescriptorModel { PropertyName = "Name", Direction = ListSortDirection.Descending };
                case MainListSortDescriptorModel.SortByAttribute.Read:
                    return new MainListSortDescriptorModel { PropertyName = "ItemReadStatus", Direction = ListSortDirection.Descending };
                case MainListSortDescriptorModel.SortByAttribute.UnRead:
                    return new MainListSortDescriptorModel { PropertyName = "ItemReadStatus", Direction = ListSortDirection.Ascending };
                case MainListSortDescriptorModel.SortByAttribute.Default:
                    return new MainListSortDescriptorModel { PropertyName = "Id", Direction = ListSortDirection.Ascending };
                case MainListSortDescriptorModel.SortByAttribute.Random:
                    return new MainListSortDescriptorModel { PropertyName = "RandomId", Direction = ListSortDirection.Ascending };
                default:
                    return new MainListSortDescriptorModel { PropertyName = "Id", Direction = ListSortDirection.Ascending };

            }
        }
        public static MainListSortDescriptorModel GetCurrentSortDescriptor()
        {
            var propertyName = DatabaseService.UserStoreDatabase.SettingsRepository.Get(f => f.Name == "SortPropertyName").FirstOrDefault();
            if (propertyName == null || propertyName.Value.IsEmpty())
            {
                SaveSortDescriptor("Id", "ascending");
                return new MainListSortDescriptorModel { PropertyName = "Id", Direction = ListSortDirection.Ascending };

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

        public static bool GetShowFavouritesOnly()
        {
            return DatabaseService.UserStoreDatabase.SettingsRepository.Get(f => f.Name == "ShowFavouritesOnly").FirstOrDefault()?.Value?.ToBool() ?? false;
        }

        public static void SaveShowFavouritesOnly(bool showFavouritesOnly)
        {
            DatabaseService.UserStoreDatabase.SettingsRepository.Update("ShowFavouritesOnly", showFavouritesOnly.ToString());
        }

        public static WikiExtractor.Maui.App.Services.AppThemes SelectedTheme
        {
            get
            {
                var direction = DatabaseService.UserStoreDatabase.SettingsRepository.Get(f => f.Name == "AppTheme").FirstOrDefault();
                if (direction == null || direction.Value.IsEmpty())
                {
                    return WikiExtractor.Maui.App.Services.SharedServiceCore.DefaultAppTheme;
                }
                else
                {
                    return (WikiExtractor.Maui.App.Services.AppThemes)Enum.Parse(typeof(WikiExtractor.Maui.App.Services.AppThemes), direction.Value);
                }
            }
        }
        public static void SaveTheme(WikiExtractor.Maui.App.Services.AppThemes theme)
        {
            DatabaseService.UserStoreDatabase.SettingsRepository.Update("AppTheme", theme.ToString());
        }

        public static float GetSpeechPitch() =>
            float.TryParse(DatabaseService.UserStoreDatabase.SettingsRepository.Get(f => f.Name == "SpeechPitch").FirstOrDefault()?.Value, out var p) ? p : 1.0f;

        public static void SaveSpeechPitch(float pitch)
        {
            DatabaseService.UserStoreDatabase.SettingsRepository.Update("SpeechPitch", pitch.ToString("F2"));
            ResetSpeechSettings();
        }

        public static string GetSpeechVoice() =>
            DatabaseService.UserStoreDatabase.SettingsRepository.Get(f => f.Name == "SpeechVoice").FirstOrDefault()?.Value ?? "System Default";

        public static void SaveSpeechVoice(string voiceName)
        {
            DatabaseService.UserStoreDatabase.SettingsRepository.Update("SpeechVoice", voiceName);
            ResetSpeechSettings();
        }

        private static SpeechOptions speechOptions;

        public static void ResetSpeechSettings() => speechOptions = null;

        public async static Task<SpeechOptions> SpeechSettings()
        {
            if (speechOptions == null)
            {
                IEnumerable<Locale> locales = await TextToSpeech.GetLocalesAsync();

                Locale locale = null;
                var savedVoice = GetSpeechVoice();

                if (locales != null)
                {
                    if (savedVoice != "System Default")
                        locale = locales.FirstOrDefault(f => f.Name == savedVoice);

                    if (locale == null)
                    {
                        var cultureName = CultureInfo.CurrentCulture.NativeName;
                        locale = locales?.Where(f => f.Name == cultureName).FirstOrDefault();
                        locale ??= locales?.Where(f => f.Name == "English (United States)").FirstOrDefault();
                        locale ??= locales?.Where(f => f.Name == "English (Australia)").FirstOrDefault();
                        locale ??= locales?.Where(f => f.Name.StartsWith("English")).FirstOrDefault();
                    }
                }

                speechOptions = new SpeechOptions()
                {
                    Volume = 1.0f,
                    Pitch = GetSpeechPitch(),
                    Locale = locale
                };
            }
            return speechOptions;
        }
    }
}
