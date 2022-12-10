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

        public const string AdsIntersitialUnitId = "ca-app-pub-4219645367584712/3235891676";
        //public const string AdsIntersitialUnitId = "ca-app-pub-3940256099942544/1033173712"; //Test
        public static int AdsIntersitialLimitOnRecord { get; set; } = 1;

    }
}
