using Pj.Library.Helpers.Database.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiExtractor.DbModels
{
    public class ParagraphPrimaryContent : ModelBase
    {
        public int MasterId { get; set; }
        public string Content { get; set; }
    }
}
