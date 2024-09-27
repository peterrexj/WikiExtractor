using System.Collections.ObjectModel;

namespace WikiExtractor.ViewModels
{
    public class QuizQuestionViewModel
    {
        public int Index { get; set; }
        public string Question { get; set; }
        public int MasterId { get; set; }
        public string MasterName { get; set; }
        public int MasterPicWidth { get; set; }
        public int MasterPicHeight { get; set; }
        public string MasterPicPath { get; set; }
        public string MetadataKey { get; set; }
        public ObservableCollection<string> AnswerCollection { get; set; }
        public string CorrectAnswer { get; set; }
    }
}
