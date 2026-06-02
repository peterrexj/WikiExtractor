using Pj.Library;
using Pj.Library.Datastore.Repository;
using System.Text;
using WikiExtractor.DbModels.UserStore;

namespace WikiExtractor.Repository.UserStore
{
    public class FavouriteTrackerRepository : RepositorySqliteNetBase<FavouriteTrackerModel>, IRepositoryBase<FavouriteTrackerModel>, IRepositoryBaseAppExtension
    {
        public FavouriteTrackerRepository(DatabaseHelper databaseHelper) : base(databaseHelper, "FavouriteTracker",
          "ItemIdentifier, IsFavourite",
          "ItemIdentifier")
        { }

        public string SchemaScript(int databaseVersion)
        {
            var createStr = new StringBuilder();
            if (databaseVersion <= 1)
            {
                createStr.Append($@"CREATE TABLE [{_tableName}] (
	                                [Id]	            INTEGER NOT NULL UNIQUE,
                                    [ItemIdentifier]    TEXT,
	                                [IsFavourite]       INTEGER,
	                                PRIMARY KEY([Id] AUTOINCREMENT)
                                    );");
            }

            return createStr.ToString();
        }
    }
}
