using Pj.Library.Mobile.Sqlite;
using Pj.Library;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Linq;
using WikiExtractor.Exts;
using WikiExtractor.DbModels;

namespace WikiExtractor.Repository
{
    public class PhoneSettingsRepository : RepositoryBase<PhoneSettings>, IRepositoryBase<PhoneSettings>, IRepositoryBaseAppExtension
    {
        public PhoneSettingsRepository(DatabaseHelper databaseHelper) : base(databaseHelper, "tblPhoneSettings",
            "Name, Value", "Name")
        {

        }

        public int Add(string name, string value)
        {
            return base.Add(new PhoneSettings { Name = name, Value = value }, checkAlreadyExists: true);
        }

        public int Update(string name, string value)
        {
            return base.Update(new PhoneSettings { Name = name, Value = value });
        }

        public string GetValue(string name)
        {
            return Get(s => s.Name.EqualsIgnoreCase(name)).FirstOrDefault()?.Value ?? "";
        }

        public string SchemaScript(int databaseVersion)
        {
            var createStr = new StringBuilder();
            if (databaseVersion <= 0)
            {
                createStr.Append($@"CREATE TABLE [{_tableName}] (
	                                [Id]	INTEGER NOT NULL UNIQUE,
	                                [Name]  TEXT,
                                    [Value] TEXT,
	                                PRIMARY KEY([Id] AUTOINCREMENT)
                                    );");
            }

            return createStr.ToString();
        }

        #region UserThemes

        public OSAppTheme GetCurrentTheme()
        {
            var userThemeFromDatabase = GetValue("UserTheme");
            OSAppTheme currentTheme = OSAppTheme.Unspecified;
            if (userThemeFromDatabase.HasValue())
            {
                Enum.TryParse(userThemeFromDatabase, true, out currentTheme);
            }
            else
            {
                currentTheme = Application.Current.RequestedTheme;
            }
            return currentTheme;
        }

        public void UpdateTheme(OSAppTheme appTheme)
        {
            Update("UserTheme", appTheme.ToString());
        }

        #endregion

        #region Google Ads Service
        private int _firstLimitOnAds = 3;
        private int _thenOnLimitOnAds = 6;


        public void UpdateLimitsOnInitialize(int firstLimit, int thenLimit)
        {
            _firstLimitOnAds = firstLimit;
            _thenOnLimitOnAds = thenLimit;
        }

        public void InitializeGoogleAds()
        {
            Update("AdsIntersitialLimitOnRecord", _firstLimitOnAds.ToString());
            ConfigData.AdsIntersitialLimitOnRecord = _firstLimitOnAds;
        }

        public void GoogleAdsIntersitialUpdateLimit()
        {
            Update("AdsIntersitialLimitOnRecord", _thenOnLimitOnAds.ToString());
            ConfigData.AdsIntersitialLimitOnRecord = _thenOnLimitOnAds;
        }
        #endregion
    }
}
