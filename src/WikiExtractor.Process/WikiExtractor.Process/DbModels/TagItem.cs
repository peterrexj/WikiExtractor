using Pj.Library.Helpers.Database.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiExtractor.DbModels
{
    public class TagItem : ModelBase
    {
        public int MasterId { get; set; }
        public int TagId { get; set; }
    }
}
