namespace WikiExtractor.Process.Models
{
    public class QuizDefinitionJsonModel
    {
        public string Metadata { get; set; }
        public string QuestionRephrase { get; set; }
        public string Fact { get; set; }
        public int MaxLengthForAnswer { get; set; }
    }
}
