using GeneralInformation.Services;
using Pj.Library.Mobile.DeviceDependency;
using System;
using System.IO;
using Wiki.iOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(LocalStorage))]
namespace Wiki.iOS
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
    }

    public class LocalStorageFactory : ISqlitHelper
    {
        public LocalStorageFactory() { }

        public bool CopyDatabase() => true;

        public string PlatformDatabasePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), DatabaseFileName);
        public string DatabaseFileName => DependencyService.Get<IAppInformation>().DbUserStore;
        public bool IsDatabaseOnCopyMode => false;
        public int CurrentVersion => 2;
        public bool HasSettingsTable => true;

        private bool _forceCopy;
        public bool ForceCopy { get { return _forceCopy; } set { _forceCopy = value; } }

        public long DatabaseFileLength => 0;
        public long DatabaseFileAssetLength => 0;
    }
}