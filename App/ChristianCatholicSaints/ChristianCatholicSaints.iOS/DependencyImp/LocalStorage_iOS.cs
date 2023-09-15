using ChristianCatholicSaints.iOS.DependencyImp;
using GeneralInformation.Services;
using Pj.Library.Mobile.DeviceDependency;
using System.IO;
using System;
using Xamarin.Forms;

[assembly: Dependency(typeof(LocalStorage_iOS))]
namespace ChristianCatholicSaints.iOS.DependencyImp
{
    public class LocalStorage_iOS : ILocalStorage
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
        public string DatabaseFileName => "SaintsUserStore.db";
        public bool IsDatabaseOnCopyMode => false;
        public int CurrentVersion => 2;
        public bool HasSettingsTable => true;

        private bool _forceCopy;
        public bool ForceCopy { get { return _forceCopy; } set { _forceCopy = value; } }

        public long DatabaseFileLength => 0;
        public long DatabaseFileAssetLength => 0;
    }
}