using GeneralInformation.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using WikiExtractor.Maui.App.Repository.UserStore;

namespace WikiExtractor.Maui.App.Repository
{
    public static class DatabaseService
    {
        private static readonly Lazy<AppDatabase> _appDatabase = new(() => new AppDatabase());
        public static AppDatabase AppDatabase => _appDatabase.Value;

        private static readonly Lazy<UserStoreDatabase> _userStoreDatabase = new(() => new UserStoreDatabase());
        public static UserStoreDatabase UserStoreDatabase => _userStoreDatabase.Value;
    }
}
