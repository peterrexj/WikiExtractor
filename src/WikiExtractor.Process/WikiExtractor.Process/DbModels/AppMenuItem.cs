using Pj.Library.Helpers.Database.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiExtractor.DbModels
{
    public class AppMenuItem : ModelBase
    {
        public string TitleOnThePage { get; set; }
        public string Tags { get; set; }
        public string MenuItemName { get; set; }
        public string Route => $"//{Tags}";
        public int Sequence { get; set; }
    }
}
