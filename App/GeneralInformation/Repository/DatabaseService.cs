using GeneralInformation.Repository.UserStore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeneralInformation.Repository
{
    public static class DatabaseService
    {
        private static AppDatabase appDatabase;
        public static AppDatabase AppDatabase => appDatabase ??= new AppDatabase();

        private static UserStoreDatabase userStoreDatabase;
        public static UserStoreDatabase UserStoreDatabase => userStoreDatabase ??= new UserStoreDatabase();
    }
}
