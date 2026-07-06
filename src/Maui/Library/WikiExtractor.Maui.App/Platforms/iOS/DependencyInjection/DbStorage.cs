using Pj.Library.Mobile.DeviceDependency;
using WikiExtractor.Maui.App.Services;
using Foundation;
using System;
using System.IO;
using System.Diagnostics;

namespace WikiExtractor.Maui.App.Platforms.iOS.DependencyInjection
{
    public class DbStorage : ISqlitHelper
    {
        public bool CopyDatabase()
        {
            try
            {
                Debug.WriteLine($"[iOS-DbStorage] CopyDatabase called");
                Debug.WriteLine($"[iOS-DbStorage] DatabaseFileName: {DatabaseFileName}");
                Debug.WriteLine($"[iOS-DbStorage] PlatformDatabasePath: {PlatformDatabasePath}");
                Debug.WriteLine($"[iOS-DbStorage] File exists: {File.Exists(PlatformDatabasePath)}");
                Debug.WriteLine($"[iOS-DbStorage] ForceCopy: {ForceCopy}");

                string sourcePath = NSBundle.MainBundle.PathForResource(Path.GetFileNameWithoutExtension(DatabaseFileName), Path.GetExtension(DatabaseFileName).TrimStart('.'));
                Debug.WriteLine($"[iOS-DbStorage] Source path: {sourcePath ?? "null"}");

                if (sourcePath != null && File.Exists(sourcePath))
                {
                    long assetLen = new FileInfo(sourcePath).Length;
                    long deviceLen = File.Exists(PlatformDatabasePath) ? new FileInfo(PlatformDatabasePath).Length : 0;
                    Debug.WriteLine($"[iOS-DbStorage] Asset size: {assetLen}  Device size: {deviceLen}");

                    if (!File.Exists(PlatformDatabasePath) || ForceCopy || assetLen != deviceLen)
                    {
                        File.Copy(sourcePath, PlatformDatabasePath, true);
                        Debug.WriteLine($"[iOS-DbStorage] Database copied successfully");
                    }
                    else
                    {
                        Debug.WriteLine($"[iOS-DbStorage] Database up to date, skipping copy");
                    }
                }
                else
                {
                    Debug.WriteLine($"[iOS-DbStorage] Source database not found in bundle");
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[iOS-DbStorage] EXCEPTION in CopyDatabase: {ex.GetType().Name}: {ex.Message}");
                return false;
            }
        }

        public string PlatformDatabasePath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), DatabaseFileName);

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

                    var fallbackAppInfo = ServiceLocator.GetService<IAppInformation>();
                    if (fallbackAppInfo?.DbWikiStore != null)
                    {
                        return fallbackAppInfo.DbWikiStore;
                    }

                    return "WikiUserStore.db";
                }
                catch
                {
                    return "WikiUserStore.db";
                }
            }
        }

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
                try
                {
                    string sourcePath = NSBundle.MainBundle.PathForResource(Path.GetFileNameWithoutExtension(DatabaseFileName), Path.GetExtension(DatabaseFileName).TrimStart('.'));

                    if (sourcePath != null && File.Exists(sourcePath))
                    {
                        return new FileInfo(sourcePath).Length;
                    }
                    return 0;
                }
                catch
                {
                    return 0;
                }
            }
        }
    }
}
