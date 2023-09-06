using GeneralInformation.Services;
using Pj.Library.Mobile.DeviceDependency;
using PopesOfChurch.Droid.DeviceDependencyImpl;
using System;
using System.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(LocalStorage_Android))]
namespace PopesOfChurch.Droid.DeviceDependencyImpl
{
    public class LocalStorage_Android : ILocalStorage
    {
        private ISqlitHelper _qlitHelper;
        public ISqlitHelper SqlLiteHelper
        {
            get
            {
                if (_qlitHelper == null)
                {
                    _qlitHelper = new LocalStorageFactory();
                }
                return _qlitHelper;
            }
        }
    }

    public class LocalStorageFactory : ISqlitHelper
    {
        public LocalStorageFactory() 
        {
        }

        public bool CopyDatabase()
        {
           return true;
        }

        public string PlatformDatabasePath =>
            Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), DatabaseFileName);
        public string DatabaseFileName => "PopesUserStore.db";
        public bool IsDatabaseOnCopyMode => false;
        public int CurrentVersion => 2;
        public bool HasSettingsTable => true;

        private bool _forceCopy;
        public bool ForceCopy { get { return _forceCopy; } set { _forceCopy = value; } }

        public long DatabaseFileLength
        {
            get
            {
                return 0;
            }
        }

        public long DatabaseFileAssetLength
        {
            get
            {
                return 0;
            }
        }
    }
}