using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiExtractor.Exts
{
    public static class ConfigData
    {
        public static int MinLengthOfPictureCaption { get; set; } = 6;
        public static int AdsIntersitialLimitOnRecord { get; set; } = 5;
        public const int MinHeightOfListItemInListPage = 120;
        public static string LocalStorageCacheFolderPath { get; set; }
    }
}
