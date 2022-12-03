using Pj.Library.Helpers.Database.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace WikiExtractor.DbModels
{
    public class PhoneSettings : ModelBase
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
