using Pj.Library.Helpers.Database.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiExtractor.DbModels
{
    public class ParagraphImage : ModelBase
    {
        public long MasterId { get; set; }
        public long ImageId { get; set; }
        public long ParagraphId { get; set; }
        public int IsSubHeaderContent { get; set; }
        public bool IsSubHeaderContentConvert => IsSubHeaderContent == 0 ? false : true;
    }
}
