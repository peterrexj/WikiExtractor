using Pj.Library;
using Pj.Library.Mobile.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.DbModels;
using WikiExtractor.DbModels.UserStore;

namespace WikiExtractor.Repository.UserStore
{
    public class ItemReadTrackerRepository : RepositoryBase<ItemReadTrackerModel>, IRepositoryBase<ItemReadTrackerModel>, IRepositoryBaseAppExtension
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
                createStr.Append($@"CREATE TABLE [{_tableName}] (
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
