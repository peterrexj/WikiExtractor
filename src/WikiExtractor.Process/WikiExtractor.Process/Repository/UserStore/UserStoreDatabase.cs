using Pj.Library;
using Pj.Library.Mobile.Model;
using WikiExtractor.Process.Repository;

namespace WikiExtractor.Repository.UserStore
{
    public class UserStoreDatabase : IUserStoreDatabase
    {
        public ItemReadTrackerRepository ItemReadTrackerRepository { get; set; }
        public FavouriteTrackerRepository FavouriteTrackerRepository { get; set; }
        public SettingsSqliteRepository SettingsRepository { get; set; }
        public RequestRecordRepository RequestRecordRepository { get; set; }
        public AppSettingsRepository AppSettingsRepository { get; set; }
        public QuizResponseRepository QuizResponseRepository { get; set; }
        public QuizFactStatusRepository QuizFactStatusRepository { get; set; }
        public StreakTrackerRepository StreakTrackerRepository { get; set; }

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
            FavouriteTrackerRepository = new FavouriteTrackerRepository(appDatabase._dbHelperUserStore);
            AppSettingsRepository = new AppSettingsRepository(appDatabase._dbHelperUserStore);
            RequestRecordRepository = new RequestRecordRepository(appDatabase._dbHelperUserStore);
            SettingsRepository = new SettingsSqliteRepository(appDatabase._dbHelperUserStore);
            QuizResponseRepository = new QuizResponseRepository(appDatabase._dbHelperUserStore);
            QuizFactStatusRepository = new QuizFactStatusRepository(appDatabase._dbHelperUserStore);
            StreakTrackerRepository = new StreakTrackerRepository(appDatabase._dbHelperUserStore);

            repoExtensions.Add(SettingsRepository);
            repoExtensions.Add(ItemReadTrackerRepository);
            repoExtensions.Add(FavouriteTrackerRepository);
            repoExtensions.Add(RequestRecordRepository);
            repoExtensions.Add(AppSettingsRepository);
            repoExtensions.Add(QuizResponseRepository);
            repoExtensions.Add(QuizFactStatusRepository);
            repoExtensions.Add(StreakTrackerRepository);

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
                SettingsRepository.Add("DatabaseVersion", "1");
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
                                // Use _dbHelperUserStore — not _dbHelper (which is the main wiki DB)
                                appDatabase._dbHelperUserStore.DbHelper.ExecuteNonQuery(qry);
                            }
                        }
                    }
                }
            }
        }
    }
}


