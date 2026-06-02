using Pj.Library;
using Pj.Library.Mobile.Model;
using Pj.Library.Mobile.Sqlite;
using WikiExtractor.Maui.App.Exts;
using WikiExtractor.Maui.App.Services;
using WikiExtractor.Process.Repository;
using WikiExtractor.Repository;
using WikiExtractor.Repository.UserStore;
using WikiExtractor.DbModels.UserStore;

namespace WikiExtractor.Maui.App.Repository.UserStore
{
    public class UserStoreDatabase : AppSqliteRepository, IUserStoreDatabase
    {
        public ItemReadTrackerRepository ItemReadTrackerRepository { get; set; }
        public FavouriteTrackerRepository FavouriteTrackerRepository { get; set; }
        public new SettingsSqliteRepository SettingsRepository => base.SettingsRepository;
        public RequestRecordRepository RequestRecordRepository { get; set; }
        public AppSettingsRepository AppSettingsRepository { get; set; }
        public QuizResponseRepository QuizResponseRepository { get; set; }
        public QuizFactStatusRepository QuizFactStatusRepository { get; set; }
        public StreakTrackerRepository StreakTrackerRepository { get; set; }

        public UserStoreDatabase() : base(DatabaseHelper.DatabaseType.SqLiteDevice, GetSafeLocalStorageHelper())
        {
            try
            {
                if (_dbHelper.DbHelper.CanConnect == false)
                {
                    ExceptionHandler.CaptureException(new Exception("Cannot connect to the Datastore!"));
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
            try
            {
                InitializeDatabase();
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
                //throw;
            }
        }

        private static Pj.Library.Mobile.DeviceDependency.ISqlitHelper GetSafeLocalStorageHelper()
        {
            try
            {
                // Try ServiceLocator first
                if (SharedServiceCore.LocalStorage?.SqlLiteHelper != null)
                {
                    return SharedServiceCore.LocalStorage.SqlLiteHelper;
                }
                
                throw new InvalidOperationException("LocalStorage service is not available during UserStoreDatabase initialization");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to initialize UserStoreDatabase: {ex.Message}", ex);
            }
        }

        public sealed override void InitializeDatabase()
        {
            ItemReadTrackerRepository = new ItemReadTrackerRepository(_dbHelper);
            FavouriteTrackerRepository = new FavouriteTrackerRepository(_dbHelper);
            RequestRecordRepository = new RequestRecordRepository(_dbHelper);
            AppSettingsRepository = new AppSettingsRepository(_dbHelper);
            QuizResponseRepository = new QuizResponseRepository(_dbHelper);
            QuizFactStatusRepository = new QuizFactStatusRepository(_dbHelper);
            StreakTrackerRepository = new StreakTrackerRepository(_dbHelper);

            CollectRepository(ItemReadTrackerRepository,
                FavouriteTrackerRepository,
                RequestRecordRepository,
                AppSettingsRepository,
                QuizResponseRepository,
                QuizFactStatusRepository,
                StreakTrackerRepository);
            
            base.InitializeDatabase();

            var currentDbVersion = SettingsRepository.GetValue("DatabaseVersion").ToInteger();
            if (sqliteFileHelper.CurrentVersion != currentDbVersion)
            {
                // Run migration scripts for the new version before updating the stored version
                var repos = new IRepositoryBaseAppExtension[]
                {
                    ItemReadTrackerRepository,
                    FavouriteTrackerRepository,
                    RequestRecordRepository,
                    AppSettingsRepository,
                    QuizResponseRepository,
                    QuizFactStatusRepository,
                    StreakTrackerRepository
                };
                foreach (var repo in repos)
                {
                    try
                    {
                        var script = repo.SchemaScript(sqliteFileHelper.CurrentVersion);
                        if (!string.IsNullOrWhiteSpace(script))
                            _dbHelper.DbHelper.ExecuteNonQuery(script);
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.CaptureException(ex);
                    }
                }
                SettingsRepository.Update("DatabaseVersion", sqliteFileHelper.CurrentVersion.ToString());
            }
        }
    }
}
