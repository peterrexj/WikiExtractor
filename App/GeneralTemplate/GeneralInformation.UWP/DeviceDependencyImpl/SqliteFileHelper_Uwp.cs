using GeneralInformation.UWP.DeviceDependencyImpl;
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

[assembly: Dependency(typeof(SqliteFileHelper_Uwp))]
namespace GeneralInformation.UWP.DeviceDependencyImpl
{
    public class SqliteFileHelper_Uwp : ISqlitHelper
    {
        //public string PlatformDatabasePath =>
        //    Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "Assets", DatabaseFileName);

        public string PlatformDatabasePath => Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Assets", DatabaseFileName);

        public string DatabaseFileName => "WikiStore.db";

        public bool IsDatabaseOnCopyMode => true;

        public int CurrentVersion => 1;

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
                using (var br = new BinaryReader(
                    File.OpenRead(
                        Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Assets", DatabaseFileName))))
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

        public bool CopyDatabase()
        {
            try
            {
                if (!File.Exists(PlatformDatabasePath) || ForceCopy)
                {
                    IoHelper.CreateDirectory(PlatformDatabasePath);
                    using (var br = new BinaryReader(
                        File.OpenRead(
                            Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Assets", DatabaseFileName))))
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
    }
}
