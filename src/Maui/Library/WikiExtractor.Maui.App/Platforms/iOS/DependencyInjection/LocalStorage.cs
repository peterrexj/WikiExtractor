using WikiExtractor.Maui.App.Services;
using Pj.Library.Mobile.DeviceDependency;
using System;
using System.IO;
using Foundation;

namespace WikiExtractor.Maui.App.Platforms.iOS.DependencyInjection
{
    public class LocalStorage : ILocalStorage
    {
        private ISqlitHelper _sqlitHelper;
        public ISqlitHelper SqlLiteHelper
        {
            get
            {
                if (_sqlitHelper == null)
                {
                    _sqlitHelper = new LocalStorageFactory();
                }
                return _sqlitHelper;
            }
        }

        private ISqlitHelper _dbSqliteHelper;
        public ISqlitHelper DbStoreHelper
        {
            get
            {
                if (_dbSqliteHelper == null)
                {
                    _dbSqliteHelper = new DbStorage();
                }
                return _dbSqliteHelper;
            }
        }
    }

    public class LocalStorageFactory : ISqlitHelper
    {
        public LocalStorageFactory() { }

        public bool CopyDatabase() => true;

        public string PlatformDatabasePath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), DatabaseFileName);

        public string DatabaseFileName
        {
            get
            {
                try
                {
                    var appInfo = SharedServiceCore.AppInformation;
                    if (appInfo?.DbUserStore != null)
                    {
                        return appInfo.DbUserStore;
                    }

                    var fallbackAppInfo = ServiceLocator.GetService<IAppInformation>();
                    if (fallbackAppInfo?.DbUserStore != null)
                    {
                        return fallbackAppInfo.DbUserStore;
                    }

                    return "WikiUserStore.db";
                }
                catch
                {
                    return "WikiUserStore.db";
                }
            }
        }
        public bool IsDatabaseOnCopyMode => false;
        public int CurrentVersion => 5;
        public bool HasSettingsTable => true;

        private bool _forceCopy;
        public bool ForceCopy { get { return _forceCopy; } set { _forceCopy = value; } }

        public long DatabaseFileLength => 0;
        public long DatabaseFileAssetLength => 0;
    }
}
