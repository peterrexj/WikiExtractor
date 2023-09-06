using Pj.Library.Helpers.Database.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace WikiExtractor.DbModels.UserStore
{
    public class ItemReadTrackerModel : ModelBase
    {
        public string ItemIdentifier { get; set; }
        public bool IsRead { get; set; }
    }
}
