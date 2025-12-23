using Pj.Library;
using Pj.Library.Mobile.Model;
using Pj.Library.Mobile.Sqlite;
using WikiExtractor.Maui.App.Exts;
using WikiExtractor.Maui.App.Services;
using WikiExtractor.Process.Repository;
using WikiExtractor.Repository;
using WikiExtractor.Repository.UserStore;

namespace WikiExtractor.Maui.App.Repository.UserStore
{
    public class UserStoreDatabase : AppSqliteRepository, IUserStoreDatabase
    {
        public ItemReadTrackerRepository ItemReadTrackerRepository { get; set; }
        public new SettingsSqliteRepository SettingsRepository => base.SettingsRepository;
        public RequestRecordRepository RequestRecordRepository { get; set; }
        public AppSettingsRepository AppSettingsRepository { get; set; }
        public QuizResponseRepository QuizResponseRepository { get; set; }

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
                var localStorage = ServiceLocator.GetService<ILocalStorage>();
                if (localStorage?.SqlLiteHelper != null)
                {
                    return localStorage.SqlLiteHelper;
                }
                
                // Fallback to CustomServices
                var customStorage = CustomServices.LocalStorage;
                if (customStorage?.SqlLiteHelper != null)
                {
                    return customStorage.SqlLiteHelper;
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
            RequestRecordRepository = new RequestRecordRepository(_dbHelper);
            AppSettingsRepository = new AppSettingsRepository(_dbHelper);
            QuizResponseRepository = new QuizResponseRepository(_dbHelper);

            CollectRepository(ItemReadTrackerRepository, 
                RequestRecordRepository,
                AppSettingsRepository,
                QuizResponseRepository);
            
            base.InitializeDatabase();

            //The current version of DB is driven from the interface implementation
            //Based on the current version, the schema will be generated and used to execute from create/modify table
            //On any requirement to update or create table, in the repository add the script according to the version of database to get the right script

            var currentDbVersion = SettingsRepository.GetValue("DatabaseVersion").ToInteger();
            if (sqliteFileHelper.CurrentVersion != currentDbVersion)
            {
                SettingsRepository.Update("DatabaseVersion", sqliteFileHelper.CurrentVersion.ToString());
            }
        }
    }
}
