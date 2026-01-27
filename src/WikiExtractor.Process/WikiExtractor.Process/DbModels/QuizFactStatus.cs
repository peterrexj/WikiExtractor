using Pj.Library.Helpers.Database.Repository;
using System;

namespace WikiExtractor.Process.DbModels
{
    public class QuizFactStatus : ModelBase
    {
        public int MasterId { get; set; }
        public string MetadataKey { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
