using Pj.Library.Helpers.Database.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiExtractor.DbModels
{
    public class Tag : ModelBase
    {
        public string Name { get; set; }
    }
}
