using Pj.Library.Helpers.Database.Repository;

namespace WikiExtractor.Process.DbModels
{
    public class QuizDefinition : ModelBase
    {
        public string MetadataKey { get; set; }
        public string QuestionPhrase { get; set; }
        public string Fact { get; set; }
    }
}
