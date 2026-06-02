using WikiExtractor.Process.Repository;

namespace WikiExtractor.Repository.UserStore
{
    public interface IUserStoreDatabase
    {
        ItemReadTrackerRepository ItemReadTrackerRepository { get; set; }
        FavouriteTrackerRepository FavouriteTrackerRepository { get; set; }
        AppSettingsRepository AppSettingsRepository { get; set; }
        RequestRecordRepository RequestRecordRepository { get; set; }
        QuizResponseRepository QuizResponseRepository { get; set; }
        QuizFactStatusRepository QuizFactStatusRepository { get; set; }
        StreakTrackerRepository StreakTrackerRepository { get; set; }

        void InitializeDatabase();
    }
}
