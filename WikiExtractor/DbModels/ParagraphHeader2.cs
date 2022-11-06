using Pj.Library.Helpers.Database.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiExtractor.DbModels
{
    public class ParagraphHeader2 : ModelBase
    {
        public int MasterId { get; set; }
        public string Header { get; set; }
        public int Sequence { get; set; }   
    }
}
