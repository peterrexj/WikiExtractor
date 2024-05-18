using Pj.Library.Helpers.Database.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiExtractor.DbModels
{
    public class WikiPicture : ModelBase
    {
        public int MasterId { get; set; }
        public int Sequence { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string? Path { get; set; }
        public string? Caption { get; set; }
        public int IsPrimary { get; set; }
        public bool IsPrimaryBool => IsPrimary == 1;
    }
}
