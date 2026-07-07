using Pj.Library;
using Pj.Library.Datastore.Repository;
using System.Text;
using WikiExtractor.DbModels.UserStore;

namespace WikiExtractor.Repository.UserStore
{
    public class StreakTrackerRepository : RepositorySqliteNetBase<StreakTrackerModel>, IRepositoryBase<StreakTrackerModel>, IRepositoryBaseAppExtension
    {
        public StreakTrackerRepository(DatabaseHelper databaseHelper) : base(databaseHelper, "StreakTracker",
          "LastOpenDate, CurrentStreak, BestStreak",
          "LastOpenDate")
        { }

        public string SchemaScript(int databaseVersion)
        {
            var createStr = new StringBuilder();
            if (databaseVersion <= 6)
            {
                createStr.Append($@"CREATE TABLE IF NOT EXISTS [{_tableName}] (
	                                [Id]	            INTEGER NOT NULL UNIQUE,
                                    [LastOpenDate]      TEXT,
	                                [CurrentStreak]     INTEGER,
	                                [BestStreak]        INTEGER,
	                                PRIMARY KEY([Id] AUTOINCREMENT)
                                    );");
            }

            return createStr.ToString();
        }
    }
}
