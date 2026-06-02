using System;
using System.Collections.Generic;

namespace WikiExtractor.Process.DbModels
{
    public class QuizStatsModel
    {
        public int TotalCorrect   { get; set; }
        public int TotalWrong     { get; set; }
        public int TotalSkipped   { get; set; }
        public int TotalSessions  { get; set; }
        public int TotalAnswered  => TotalCorrect + TotalWrong;
        public double AccuracyPercent => TotalAnswered == 0 ? 0 : Math.Round(TotalCorrect / (double)TotalAnswered * 100, 1);

        public List<QuizSessionScore>   SessionScores   { get; set; } = new();
        public List<QuizTopicAccuracy>  TopicAccuracy   { get; set; } = new();
        public List<QuizSubjectAccuracy> SubjectAccuracy { get; set; } = new();
    }

    public class QuizSessionScore
    {
        public int      SessionId { get; set; }
        public int      Correct   { get; set; }
        public int      Total     { get; set; }
        public DateTime PlayedAt  { get; set; }
        public double   ScorePct  => Total == 0 ? 0 : Math.Round(Correct / (double)Total * 100, 1);
    }

    public class QuizTopicAccuracy
    {
        public string Topic    { get; set; }
        public int    Correct  { get; set; }
        public int    Answered { get; set; }
        public double AccuracyPct => Answered == 0 ? 0 : Math.Round(Correct / (double)Answered * 100, 1);
    }

    public class QuizSubjectAccuracy
    {
        public int    MasterId   { get; set; }
        public string MasterName { get; set; }
        public int    Correct    { get; set; }
        public int    Answered   { get; set; }
        public double AccuracyPct => Answered == 0 ? 0 : Math.Round(Correct / (double)Answered * 100, 1);
    }
}
