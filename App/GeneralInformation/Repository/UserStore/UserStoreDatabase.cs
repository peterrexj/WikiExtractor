using GeneralInformation.Services;
using Pj.Library;
using Pj.Library.Mobile.Model;
using Pj.Library.Mobile.Sqlite;
using WikiExtractor.Repository;
using WikiExtractor.Repository.UserStore;
using Xamarin.Forms;

namespace GeneralInformation.Repository.UserStore
{
    public class UserStoreDatabase : AppSqliteRepository, IUserStoreDatabase
    {
        public ItemReadTrackerRepository ItemReadTrackerRepository { get; set; }
        public new SettingsSqliteRepository SettingsRepository => base.SettingsRepository;
        public RequestRecordRepository RequestRecordRepository { get; set; }
        public AppSettingsRepository AppSettingsRepository { get; set; }

        public UserStoreDatabase() : base(DatabaseHelper.DatabaseType.SqLiteDevice, DependencyService.Get<ILocalStorage>().SqlLiteHelper)
        {
            InitializeDatabase();
        }

        public override void InitializeDatabase()
        {
            ItemReadTrackerRepository = new ItemReadTrackerRepository(_dbHelper);
            RequestRecordRepository = new RequestRecordRepository(_dbHelper);
            AppSettingsRepository = new AppSettingsRepository(_dbHelper);

            base.CollectRepository(ItemReadTrackerRepository, 
                RequestRecordRepository,
                AppSettingsRepository);
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
