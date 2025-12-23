using GeneralInformation.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using WikiExtractor.Maui.App.Repository.UserStore;

namespace WikiExtractor.Maui.App.Repository
{
    public static class DatabaseService
    {
        private static AppDatabase appDatabase;
        public static AppDatabase AppDatabase => appDatabase ??= new AppDatabase();

        private static UserStoreDatabase userStoreDatabase;
        public static UserStoreDatabase UserStoreDatabase => userStoreDatabase ??= new UserStoreDatabase();
    }
}
