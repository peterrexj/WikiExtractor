using System.Collections.ObjectModel;
using Pj.Library;
using WikiExtractor.Maui.App.Exts;
using WikiExtractor.Maui.App.Services;
using WikiExtractor.Maui.App.ViewModels.Charts;
using WikiExtractor.Process.DbModels;
using WikiExtractor.ViewModels;

namespace WikiExtractor.Maui.App.ViewModels
{
    public class StatsPageViewModel : MauiBaseViewModel
    {
        // ── Reading stats ───────────────────────────────────────────────
        private int _totalRead;
        public int TotalRead { get => _totalRead; private set { _totalRead = value; OnPropertyChanged(); } }

        private int _totalItems;
        public int TotalItems { get => _totalItems; private set { _totalItems = value; OnPropertyChanged(); OnPropertyChanged(nameof(ReadProgressText)); } }

        private int _totalFavourites;
        public int TotalFavourites { get => _totalFavourites; private set { _totalFavourites = value; OnPropertyChanged(); } }

        public string ReadProgressText => TotalItems == 0 ? "0 / 0" : $"{TotalRead} / {TotalItems}";

        private ObservableCollection<DataModel> _readChartData = new();
        public ObservableCollection<DataModel> ReadChartData { get => _readChartData; private set { _readChartData = value; OnPropertyChanged(); } }

        // ── Streak stats ────────────────────────────────────────────────
        private int _currentStreak;
        public int CurrentStreak { get => _currentStreak; private set { _currentStreak = value; OnPropertyChanged(); } }

        private int _bestStreak;
        public int BestStreak { get => _bestStreak; private set { _bestStreak = value; OnPropertyChanged(); } }

        private string _lastOpenDate = string.Empty;
        public string LastOpenDate { get => _lastOpenDate; private set { _lastOpenDate = value; OnPropertyChanged(); } }

        // ── Quiz all-time ───────────────────────────────────────────────
        private int _totalSessions;
        public int TotalSessions { get => _totalSessions; private set { _totalSessions = value; OnPropertyChanged(); } }

        private double _accuracyPercent;
        public double AccuracyPercent { get => _accuracyPercent; private set { _accuracyPercent = value; OnPropertyChanged(); OnPropertyChanged(nameof(AccuracyText)); } }
        public string AccuracyText => $"{AccuracyPercent:F0}%";

        private ObservableCollection<DataModel> _quizAllTimeData = new();
        public ObservableCollection<DataModel> QuizAllTimeData { get => _quizAllTimeData; private set { _quizAllTimeData = value; OnPropertyChanged(); } }

        // ── Quiz session trend ──────────────────────────────────────────
        private ObservableCollection<DataModel> _sessionTrendData = new();
        public ObservableCollection<DataModel> SessionTrendData { get => _sessionTrendData; private set { _sessionTrendData = value; OnPropertyChanged(); } }

        private bool _hasSessionTrend;
        public bool HasSessionTrend { get => _hasSessionTrend; private set { _hasSessionTrend = value; OnPropertyChanged(); } }

        // ── Topic accuracy ──────────────────────────────────────────────
        private ObservableCollection<DataModel> _topicData = new();
        public ObservableCollection<DataModel> TopicData { get => _topicData; private set { _topicData = value; OnPropertyChanged(); } }

        private bool _hasTopicData;
        public bool HasTopicData { get => _hasTopicData; private set { _hasTopicData = value; OnPropertyChanged(); } }

        // ── Hardest subjects ────────────────────────────────────────────
        private ObservableCollection<DataModel> _hardestSubjectsData = new();
        public ObservableCollection<DataModel> HardestSubjectsData { get => _hardestSubjectsData; private set { _hardestSubjectsData = value; OnPropertyChanged(); } }

        private bool _hasHardestSubjects;
        public bool HasHardestSubjects { get => _hasHardestSubjects; private set { _hasHardestSubjects = value; OnPropertyChanged(); } }

        // ── Insights ────────────────────────────────────────────────────
        private string _insightText = string.Empty;
        public string InsightText { get => _insightText; private set { _insightText = value; OnPropertyChanged(); } }

        // ── Chart colors ────────────────────────────────────────────────
        private ObservableCollection<Brush> _chartColors = new();
        public ObservableCollection<Brush> ChartColors { get => _chartColors; private set { _chartColors = value; OnPropertyChanged(); } }

        private ObservableCollection<Brush> _sessionChartColors = new();
        public ObservableCollection<Brush> SessionChartColors { get => _sessionChartColors; private set { _sessionChartColors = value; OnPropertyChanged(); } }

        public async Task InitializeAsync()
        {
            try
            {
                IsPageBusy = true;
                BannerAdsUnitId = SharedServiceCore.AdsConfig.BannerAdUnitId;

                var themeTask    = SharedServiceCore.ThemeHandler.GetThemeDataAsync();
                var streakTask   = Task.Run(() => SharedServices.WikiAppController.GetStreak());
                var readTask     = Task.Run(() => SharedServices.WikiAppController.GetItemReadTrackData().ToList());
                var favTask      = Task.Run(() => SharedServices.WikiAppController.GetFavouriteTrackData().ToList());
                var quizTask     = Task.Run(() => SharedServices.QuizController.GetQuizStats());
                var totalTask    = Task.Run(() => SharedServices.WikiAppController.GetListOfWikiItems(new List<string>()).Count());

                await Task.WhenAll(themeTask, streakTask, readTask, favTask, quizTask, totalTask);

                var theme   = await themeTask;
                var streak  = streakTask.Result;
                var reads   = readTask.Result;
                var favs    = favTask.Result;
                var quiz    = quizTask.Result;
                var total   = totalTask.Result;

                // Chart palette
                ChartColors = new ObservableCollection<Brush>
                {
                    new SolidColorBrush(theme.CorrectColor),
                    new SolidColorBrush(Color.FromArgb("#888888"))
                };
                SessionChartColors = new ObservableCollection<Brush>
                {
                    new SolidColorBrush(theme.CorrectColor),
                    new SolidColorBrush(theme.WrongColor),
                    new SolidColorBrush(Color.FromArgb("#C8A06E"))
                };

                // Reading
                var readCount = reads.Count(r => r.IsReadAsBool);
                TotalRead       = readCount;
                TotalItems      = total;
                TotalFavourites = favs.Count(f => f.IsFavouriteAsBool);
                ReadChartData = new ObservableCollection<DataModel>
                {
                    new DataModel { Category = $"Read ({(total == 0 ? 0 : readCount * 100 / total)}%)", Value = readCount },
                    new DataModel { Category = $"Unread", Value = Math.Max(0, total - readCount) }
                };

                // Streak
                CurrentStreak = streak.CurrentStreak;
                BestStreak    = streak.BestStreak;
                LastOpenDate  = streak.LastOpenDate.HasValue() ? streak.LastOpenDate : "—";

                // Quiz all-time
                TotalSessions  = quiz.TotalSessions;
                AccuracyPercent = quiz.AccuracyPercent;
                QuizAllTimeData = new ObservableCollection<DataModel>
                {
                    new DataModel { Category = $"Correct ({quiz.TotalCorrect})",  Value = quiz.TotalCorrect },
                    new DataModel { Category = $"Wrong ({quiz.TotalWrong})",       Value = quiz.TotalWrong },
                    new DataModel { Category = $"Skipped ({quiz.TotalSkipped})",   Value = quiz.TotalSkipped }
                };

                // Session trend (column chart — score % per session)
                if (quiz.SessionScores.Count >= 2)
                {
                    SessionTrendData = new ObservableCollection<DataModel>(
                        quiz.SessionScores.Select((s, i) => new DataModel
                        {
                            Category = $"#{s.SessionId}",
                            Value    = s.ScorePct
                        }));
                    HasSessionTrend = true;
                }

                // Topic accuracy (worst first — most interesting)
                if (quiz.TopicAccuracy.Count > 0)
                {
                    TopicData = new ObservableCollection<DataModel>(
                        quiz.TopicAccuracy.Take(6).Select(t => new DataModel
                        {
                            Category = FormatTopicKey(t.Topic),
                            Value    = t.AccuracyPct
                        }));
                    HasTopicData = true;
                }

                // Hardest subjects
                if (quiz.SubjectAccuracy.Count > 0)
                {
                    HardestSubjectsData = new ObservableCollection<DataModel>(
                        quiz.SubjectAccuracy.Select(s => new DataModel
                        {
                            Category = s.MasterName,
                            Value    = s.AccuracyPct
                        }));
                    HasHardestSubjects = true;
                }

                // Insight
                InsightText = BuildInsight(quiz, readCount, total, streak);
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
            finally
            {
                IsPageBusy = false;
            }
        }

        private static string BuildInsight(QuizStatsModel quiz, int readCount, int total, WikiExtractor.DbModels.UserStore.StreakTrackerModel streak)
        {
            if (quiz.TotalSessions == 0 && readCount == 0)
                return "Start reading and taking quizzes to see your insights here!";

            var parts = new List<string>();

            if (total > 0)
                parts.Add($"You've read {readCount} of {total} articles ({(total == 0 ? 0 : readCount * 100 / total)}% complete).");

            if (streak.CurrentStreak > 1)
                parts.Add($"You're on a {streak.CurrentStreak}-day streak — keep it going!");

            if (quiz.TotalSessions > 0)
            {
                parts.Add($"Across {quiz.TotalSessions} quiz session{(quiz.TotalSessions == 1 ? "" : "s")} your accuracy is {quiz.AccuracyPercent:F0}%.");

                if (quiz.SessionScores.Count >= 3)
                {
                    var first = quiz.SessionScores.First().ScorePct;
                    var last  = quiz.SessionScores.Last().ScorePct;
                    if (last > first + 5)
                        parts.Add("Your quiz scores are improving — great work!");
                    else if (last < first - 5)
                        parts.Add("Your recent scores have dipped — try a quiz to bounce back.");
                }

                if (quiz.TopicAccuracy.Any())
                {
                    var weakest = quiz.TopicAccuracy.First();
                    parts.Add($"Your weakest topic is \"{FormatTopicKey(weakest.Topic)}\" ({weakest.AccuracyPct:F0}% correct).");
                }
            }

            return string.Join(" ", parts);
        }

        private static string FormatTopicKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return key;
            // "BirthYear" → "Birth Year", "NationalTeam" → "National Team"
            var result = System.Text.RegularExpressions.Regex.Replace(key, "([A-Z])", " $1").Trim();
            return result;
        }
    }
}
