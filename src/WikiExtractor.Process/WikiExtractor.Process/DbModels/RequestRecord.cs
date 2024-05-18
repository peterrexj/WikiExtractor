using Pj.Library.Helpers.Database.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiExtractor.DbModels
{
    public class RequestRecord : ModelBase
    {
        public DateTime RequestDate { get; set; }
        public int RequestCount { get; set; }
    }
}
