using Pj.Library.Mobile.Model;
using Pj.Library;
using Pj.Library.Mobile.Sqlite;

namespace WikiExtractor.Repository.UserStore
{
    public class UserStoreDatabase : IUserStoreDatabase
    {
        public ItemReadTrackerRepository ItemReadTrackerRepository { get; set; }
        public SettingsRepository SettingsRepository { get; set; }
        public RequestRecordRepository RequestRecordRepository { get; set; }
        public AppSettingsRepository AppSettingsRepository { get; set; }

        protected List<IRepositoryBaseAppExtension> repoExtensions;
        AppDatabase appDatabase;

        public UserStoreDatabase()
        {
            repoExtensions = new List<IRepositoryBaseAppExtension>();
            appDatabase = new AppDatabase(false);
            InitializeDatabase();
        }

        public void InitializeDatabase()
        {
            appDatabase.InitializeDatabaseUserStore();

            ItemReadTrackerRepository = new ItemReadTrackerRepository(appDatabase._dbHelperUserStore);
            AppSettingsRepository = new AppSettingsRepository(appDatabase._dbHelperUserStore);
            RequestRecordRepository = new RequestRecordRepository(appDatabase._dbHelperUserStore);
            SettingsRepository = new SettingsRepository(appDatabase._dbHelperUserStore);
           
            repoExtensions.Add(SettingsRepository);
            repoExtensions.Add(ItemReadTrackerRepository);
            repoExtensions.Add(RequestRecordRepository);
            repoExtensions.Add(AppSettingsRepository);

            int currentDbVersion = 0;
            bool requireDbInitialization = false;

            try
            {
                currentDbVersion = SettingsRepository.GetValue("DatabaseVersion").ToInteger();
            }
            catch (Exception)
            {
                requireDbInitialization = true;
            }


            if (requireDbInitialization)
            {
                repoExtensions.Iter(f => appDatabase._dbHelperUserStore.DbHelper.ExecuteNonQuery(f.SchemaScript(0)));
            }
            else
            {
                if (currentDbVersion >= 1)
                {
                    foreach (var repo in repoExtensions)
                    {
                        if (repo != null)
                        {
                            var qry = repo.SchemaScript(currentDbVersion);
                            if (qry.HasValue())
                            {
                                appDatabase._dbHelper.DbHelper.ExecuteNonQuery(qry);
                            }
                        }
                    }
                }
            }
        }
    }
}


