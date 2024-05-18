using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiExtractor.Models
{
    public class MetaDataModel
    {
        public MetaDataModel()
        {
            CustomMetadata = new Dictionary<string, string>();
        }

        public MetaDataModel(int sequence, string name, MetadataType type)
        {
            Sequence = sequence;
            Name = name;
            Type = type;
            CustomMetadata = new Dictionary<string, string>();
        }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public MetadataType Type { get; set; }
        public int Sequence { get;set; }

        public Dictionary<string, string> CustomMetadata { get; set; }
    }
}
