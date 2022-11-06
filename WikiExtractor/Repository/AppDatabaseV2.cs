//using Pj.Library.Mobile.DeviceDependency;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using WikiExtractor.Process;
//using WikiExtractor.Repository;
//using Xamarin.Forms;
//using Xamarin.Forms.PlatformConfiguration;

//[assembly: Dependency(typeof(AppDatabaseV2))]
//namespace WikiExtractor.Repository
//{
//    public class AppDatabaseV2 : ISqlitHelper
//    {
//        public bool CopyDatabase()
//        {
//            return true;
//        }

//        public string PlatformDatabasePath => ProcessConstants.DatabasePath;

//        public string DatabaseFileName => "WikiStore.db";
//        public bool IsDatabaseOnCopyMode => false;
//        public int CurrentVersion => 2;
//        public bool HasSettingsTable => true;

//        private bool _forceCopy;
//        public bool ForceCopy { get { return _forceCopy; } set { _forceCopy = value; } }

//        public long DatabaseFileLength
//        {
//            get
//            {
//                if (File.Exists(PlatformDatabasePath))
//                {
//                    return new FileInfo(PlatformDatabasePath).Length;
//                }
//                else
//                {
//                    return 0;
//                }
//            }
//        }

//        public long DatabaseFileAssetLength
//        {
//            get
//            {
//                return DatabaseFileLength;
//            }
//        }
//    }
//}
