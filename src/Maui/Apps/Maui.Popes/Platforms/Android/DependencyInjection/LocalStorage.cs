using WikiExtractor.Maui.App.Services;
using Pj.Library.Mobile.DeviceDependency;
using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

namespace Maui.Wiki.Platforms.Android.DependencyInjection
{
    public class LocalStorage : ILocalStorage
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

        private ISqlitHelper _dbSqliteHelper;
        public ISqlitHelper DbStoreHelper
        {
            get
            {
                if ( _dbSqliteHelper == null)
                {
                    _dbSqliteHelper = new DbStorage();
                }
                return _dbSqliteHelper;
            }
        }
    }

    public class LocalStorageFactory : ISqlitHelper
    {
        public bool CopyDatabase()
        {
            try
            {
                if (!File.Exists(PlatformDatabasePath) || ForceCopy)
                {
                    using (var br = new BinaryReader(global::Android.App.Application.Context.Assets.Open(DatabaseFileName)))
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
        
        public string DatabaseFileName => SharedServiceCore.AppInformation != null ?
            SharedServiceCore.AppInformation?.DbUserStore ?? "WikiUserStore.db"
            : "WikiUserStore.db";

        public bool IsDatabaseOnCopyMode => false;
        public int CurrentVersion => 4;
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
                using (var br = new BinaryReader(global::Android.App.Application.Context.Assets.Open(DatabaseFileName)))
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

    //public static class ServiceHelper
    //{
    //    public static IServiceProvider Services { get; set; }
    //}
}
