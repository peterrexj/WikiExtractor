using System;
using System.Collections.Generic;
using System.Text;

namespace GeneralInformation.Repository
{
    public static class DatabaseService
    {
        private static AppDatabase appDatabase;
        public static AppDatabase AppDatabase => appDatabase ?? (appDatabase = new AppDatabase());
    }
}
