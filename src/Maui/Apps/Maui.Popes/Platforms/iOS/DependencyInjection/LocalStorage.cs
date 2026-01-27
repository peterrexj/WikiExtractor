using WikiExtractor.Maui.App.Services;
using Pj.Library.Mobile.DeviceDependency;
using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Foundation;

namespace Maui.Wiki.Platforms.iOS.DependencyInjection
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

        public ISqlitHelper DbStoreHelper
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }

    public class LocalStorageFactory : ISqlitHelper
    {
        public LocalStorageFactory() { }

        public bool CopyDatabase() => true;

        //public string PlatformDatabasePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), DatabaseFileName);
        public string PlatformDatabasePath => NSBundle.MainBundle.PathForResource("WikiStore", "db");
        
        public string DatabaseFileName
        {
            get
            {
                try
                {
                    var appInfo = SharedServiceCore.AppInformation;
                    if (appInfo?.DbWikiStore != null)
                    {
                        return appInfo.DbWikiStore;
                    }
                    
                    // Fallback to ServiceLocator
                    var fallbackAppInfo = ServiceLocator.GetService<IAppInformation>();
                    if (fallbackAppInfo?.DbWikiStore != null)
                    {
                        return fallbackAppInfo.DbWikiStore;
                    }
                    
                    // Default fallback
                    return "WikiStore.db";
                }
                catch
                {
                    return "WikiStore.db"; // Safe fallback
                }
            }
        }
        public bool IsDatabaseOnCopyMode => false;
        public int CurrentVersion => 4;
        public bool HasSettingsTable => true;

        private bool _forceCopy;
        public bool ForceCopy { get { return _forceCopy; } set { _forceCopy = value; } }

        public long DatabaseFileLength => 0;
        public long DatabaseFileAssetLength => 0;
    }
}