using Pj.Library.Helpers.Database.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace WikiExtractor.DbModels.UserStore
{
    public class ItemReadTrackerModel : ModelBase
    {
        public string ItemIdentifier { get; set; }
        public int IsRead { get; set; }
        public bool IsReadAsBool => IsRead == 0 ? false : true;
    }
}
