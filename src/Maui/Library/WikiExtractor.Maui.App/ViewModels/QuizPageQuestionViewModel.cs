using Pj.Library;
// using Syncfusion.Maui.ProgressBar; // Temporarily disabled
using System.Collections.ObjectModel;
using System.IO;
using WikiExtractor.Maui.App.Exts;
using WikiExtractor.ViewModels;
using WikiExtractor.Maui.App.Models.Mix;

namespace WikiExtractor.Maui.App.ViewModels
{
    public class QuizPageQuestionViewModel : MauiBaseViewModel
    {
        public QuizPageQuestionViewModel(QuizQuestionViewModel quizQuestionViewModel)
        {
            FromQuizQuestionViewModel(quizQuestionViewModel);
        }

        private int _id;
        public int Id
        {
            get => _id; set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        private string _question;
        public string Question
        {
            get => _question;
            set
            {
                _question = value;
                OnPropertyChanged(nameof(Question));
            }
        }

        private int _masterId;
        public int MasterId
        {
            get => _masterId;
            set
            {
                _masterId = value;
                OnPropertyChanged(nameof(MasterId));
            }
        }

        private string _masterName;
        public string MasterName
        {
            get => _masterName;
            set
            {
                _masterName = value;
                OnPropertyChanged(nameof(MasterName));
                OnPropertyChanged(nameof(MasterPicLocalFileName));
            }
        }

        private string _masterPicPath;
        public string MasterPicPath
        {
            get => _masterPicPath;
            set
            {
                _masterPicPath = value;
                OnPropertyChanged(nameof(MasterPicPath));
            }
        }
        public string MasterPicLocalFileName => $"{MasterName?.Replace(" ", "_")?.Replace("-", "_")?.Replace(".", "_")}{Path.GetExtension(MasterPicPath ?? "")}";

        private int _masterPicWidth;
        public int MasterPicWidth
        {
            get => _masterPicWidth;
            set
            {
                _masterPicWidth = value;
                OnPropertyChanged(nameof(MasterPicWidth));
            }
        }

        private int _masterPicHeight;
        public int MasterPicHeight
        {
            get => _masterPicHeight;
            set
            {
                _masterPicHeight = value;
                OnPropertyChanged(nameof(MasterPicHeight));
            }
        }

        private string _metadataKey;
        public string MetadataKey
        {
            get => _metadataKey;
            set
            {
                _metadataKey = value;
                OnPropertyChanged(nameof(MetadataKey));
            }
        }

        private StepStatus _stepStatus;
        public StepStatus StepStatus
        {
            get => _stepStatus;
            set
            {
                _stepStatus = value;
                OnPropertyChanged(nameof(StepStatus));
            }
        }

        private ObservableCollection<string> _answerCollection;
        public ObservableCollection<string> AnswerCollection
        {
            get => _answerCollection;
            set
            {
                _answerCollection = value;
                OnPropertyChanged(nameof(AnswerCollection));
            }
        }

        private string _correctAnswer;
        public string CorrectAnswer
        {
            get => _correctAnswer;
            set
            {
                _correctAnswer = value;
                OnPropertyChanged(nameof(CorrectAnswer));
            }
        }

        private string _userSelection;
        public string UserSelection
        {
            get => _userSelection;
            set
            {
                _userSelection = value;
                OnPropertyChanged(nameof(UserSelection));
                OnPropertyChanged(nameof(IsCorrect));
                OnPropertyChanged(nameof(HasUserAnswered));
            }
        }

        public bool IsCorrect => CorrectAnswer == UserSelection;
        public bool HasUserAnswered => !string.IsNullOrEmpty(UserSelection);

        public Color ResultAccentColor
        {
            get
            {
                if (!HasUserAnswered)
                    return GetAppColor("WikiAppQuizProgressSkipColor", "#C8A06E");
                return IsCorrect
                    ? GetAppColor("WikiAppQuizCorrectAnswerColor", "#7EC8A0")
                    : GetAppColor("WikiAppQuizWrongAnswerColor", "#E88080");
            }
        }

        private static Color GetAppColor(string key, string fallback)
        {
            if (Application.Current?.Resources.TryGetValue(key, out var val) == true && val is Color c)
                return c;
            return Color.FromArgb(fallback);
        }

        private Color _segmentColor;
        public Color SegmentColor
        {
            get => _segmentColor;
            set
            {
                _segmentColor = value;
                OnPropertyChanged(nameof(SegmentColor));
            }
        }

        public string QuizUserAnswerBasedBackgroundColor =>
            HasUserAnswered
                ? IsCorrect ? "#4CAF50" : "#F44336"  // Green for correct, Red for wrong
                : "#9E9E9E";  // Gray for not answered

        public void FromQuizQuestionViewModel(QuizQuestionViewModel model)
        {
            if (model == null) { return; }

            Id = model.Index;
            Question = model.Question;
            MasterId = model.MasterId;
            MasterName = model.MasterName;
            MasterPicPath = model.MasterPicPath;
            MasterPicHeight = model.MasterPicHeight;
            MasterPicWidth = model.MasterPicWidth;
            MetadataKey = model.MetadataKey;
            AnswerCollection = model.AnswerCollection;
            CorrectAnswer = model.CorrectAnswer;
        }
    }

    // Temporary enum to replace Syncfusion StepStatus
    public enum StepStatus
    {
        NotStarted,
        InProgress,
        Completed
    }
}