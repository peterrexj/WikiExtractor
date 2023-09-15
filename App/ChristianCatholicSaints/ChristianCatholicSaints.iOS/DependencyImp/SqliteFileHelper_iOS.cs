using ChristianCatholicSaints.iOS.DependencyImp;
using Pj.Library.Mobile.DeviceDependency;
using System.IO;
using System;
using Xamarin.Forms;
using Foundation;

[assembly: Dependency(typeof(SqliteFileHelper_iOS))]
namespace ChristianCatholicSaints.iOS.DependencyImp
{
    public class SqliteFileHelper_iOS : ISqlitHelper
    {
        public bool CopyDatabase()
        {
            try
            {
                if (!File.Exists(PlatformDatabasePath) || ForceCopy)
                {
                    using (FileStream fs = new FileStream(BundledPath, FileMode.Open, FileAccess.Read))
                    using (BinaryReader br = new BinaryReader(fs))
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

        string BundledPath => Path.Combine(NSBundle.MainBundle.BundlePath, DatabaseFileName);
        public string PlatformDatabasePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), DatabaseFileName);
        public string DatabaseFileName => "WikiStoreSaints.db";
        public bool IsDatabaseOnCopyMode => true;
        public int CurrentVersion => 0;
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
                using (FileStream fs = new FileStream(BundledPath, FileMode.Open, FileAccess.Read))
                using (BinaryReader br = new BinaryReader(fs))
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