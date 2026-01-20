using Pj.Library.Mobile.DeviceDependency;
using WikiExtractor.Maui.App.Services;

namespace Maui.Wiki.Platforms.Android.DependencyInjection
{
    public class DbStorage : ISqlitHelper
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
            Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), DatabaseFileName);

        public string DatabaseFileName
        {
            get
            {
                if (SharedServiceCore.AppInformation == null) return "WikiUserStore.db";

                // Explicitly specify the extension method to resolve ambiguity
                //var info = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions
                //    .GetRequiredService<IAppInformation>(ServiceHelper.Services);
                return SharedServiceCore.AppInformation.DbWikiStore ?? "WikiUserStore.db";
            }
        }

        //public string DatabaseFileName => DependencyService.Get<IAppInformation>().DbWikiStore;
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
}
