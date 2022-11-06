using Pj.Library.Helpers.Database.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.Models;

namespace WikiExtractor.DbModels
{
    public class Metadata : ModelBase
    {
		public int MasterId { get; set; }
		public string Key { get; set; }
		public string Value { get; set; }
		public string Type { get; set; }
		public int Sequence { get; set; }
		public MetadataType TypeByEnum => (MetadataType)Enum.Parse(typeof(MetadataType), Type);
    }
}
