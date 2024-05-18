using ChristianCatholicSaints.UWP.DeviceDependencyImpl;
using Pj.Library.Mobile.DeviceDependency;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Windows.Storage;
using Pj.Library;
using GeneralInformation.Services;

[assembly: Dependency(typeof(LocalStorage_Uwp))]
namespace ChristianCatholicSaints.UWP.DeviceDependencyImpl
{
    public class LocalStorage_Uwp : ILocalStorage
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
        private readonly string rootFolder = ApplicationData.Current.LocalFolder.Path;
        public LocalStorageFactory() 
        {
            if (!Directory.Exists(rootFolder))
            {
                Directory.CreateDirectory(rootFolder);
            }
        }

        public string PlatformDatabasePath => Path.Combine(rootFolder, DatabaseFileName);
        public string DatabaseFileName => DependencyService.Get<IAppInformation>().DbUserStore;
        public bool IsDatabaseOnCopyMode => false;
        public int CurrentVersion => 2;
        public bool HasSettingsTable => true;

        private bool _forceCopy;
        public bool ForceCopy { get { return _forceCopy; } set { _forceCopy = value; } }

        public bool CopyDatabase()
        {
            try
            {
                if (!File.Exists(PlatformDatabasePath) || ForceCopy)
                {
                    IoHelper.CreateDirectory(PlatformDatabasePath);
                    using (var br = new BinaryReader(File.OpenRead(Path.Combine(rootFolder, DatabaseFileName))))
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
                using (var br = new BinaryReader(File.OpenRead(Path.Combine(rootFolder, DatabaseFileName))))
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
