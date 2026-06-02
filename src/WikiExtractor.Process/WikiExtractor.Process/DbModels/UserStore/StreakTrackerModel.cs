using Pj.Library.Helpers.Database.Repository;

namespace WikiExtractor.DbModels.UserStore
{
    public class StreakTrackerModel : ModelBase
    {
        public string LastOpenDate { get; set; }
        public int CurrentStreak { get; set; }
        public int BestStreak { get; set; }
    }
}
