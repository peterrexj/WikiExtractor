using WorldLeaders.Droid.DeviceDependencyImpl;
using GeneralInformation.Services;
using Pj.Library.Mobile.DeviceDependency;
using System;
using System.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(LocalStorage_Android))]
namespace WorldLeaders.Droid.DeviceDependencyImpl
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
            try
            {
                if (!File.Exists(PlatformDatabasePath) || ForceCopy)
                {
                    using (var br = new BinaryReader(Android.App.Application.Context.Assets.Open(DatabaseFileName)))
                    using (var bw = new BinaryWriter(new FileStream(PlatformDatabasePath, FileMode.Create)))
                    {
                        byte[] buffer = new byte[2048];
                        int length = 0;
                        while ((length = br.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            bw.Write(buffer, 0, length);
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string PlatformDatabasePath =>
            Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), DatabaseFileName);
        public string DatabaseFileName => "WorldLeadersUserStore.db";
        public bool IsDatabaseOnCopyMode => false;
        public int CurrentVersion => 2;
        public bool HasSettingsTable => true;

        private bool _forceCopy;
        public bool ForceCopy { get { return _forceCopy; } set { _forceCopy = value; } }

        public long DatabaseFileLength
        {
            get
            {
                if (File.Exists(PlatformDatabasePath))
                {
                    return new FileInfo(PlatformDatabasePath).Length;
                }
                else
                {
                    return 0;
                }
            }
        }

        public long DatabaseFileAssetLength
        {
            get
            {
                int totalLength = 0;
                using (var br = new BinaryReader(Android.App.Application.Context.Assets.Open(DatabaseFileName)))
                {
                    byte[] buffer = new byte[4096];
                    int length = 0;
                    while ((length = br.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        totalLength += length;
                    }
                }

                return totalLength;
            }
        }
    }
}