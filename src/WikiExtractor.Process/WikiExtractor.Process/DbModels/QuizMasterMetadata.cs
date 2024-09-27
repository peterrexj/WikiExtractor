using Pj.Library.Helpers.Database.Repository;

namespace WikiExtractor.Process.DbModels
{
    public class QuizMasterMetadata : ModelBase
    {
        public int MasterId { get; set; }
        public string MetadataKey { get; set; }
    }
}
