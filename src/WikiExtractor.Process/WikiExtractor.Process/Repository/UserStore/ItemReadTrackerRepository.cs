using Pj.Library;
using Pj.Library.Datastore.Repository;
using System.Text;
using WikiExtractor.DbModels.UserStore;

namespace WikiExtractor.Repository.UserStore
{
    public class ItemReadTrackerRepository : RepositorySqliteNetBase<ItemReadTrackerModel>, IRepositoryBase<ItemReadTrackerModel>, IRepositoryBaseAppExtension
    {
        public ItemReadTrackerRepository(DatabaseHelper databaseHelper) : base(databaseHelper, "ItemReadTracker",
          "ItemIdentifier, IsRead",
          "ItemIdentifier")
        { }

        public string SchemaScript(int databaseVersion)
        {
            var createStr = new StringBuilder();
            if (databaseVersion <= 1)
            {
                createStr.Append($@"CREATE TABLE IF NOT EXISTS [{_tableName}] (
	                                [Id]	            INTEGER NOT NULL UNIQUE,
                                    [ItemIdentifier]    TEXT,
	                                [IsRead]            INTEGER,
	                                PRIMARY KEY([Id] AUTOINCREMENT)
                                    );");
            }

            return createStr.ToString();
        }
    }
}
