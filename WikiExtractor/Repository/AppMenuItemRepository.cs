using Pj.Library;
using Pj.Library.Datastore.Repository;
using System.Text;
using WikiExtractor.DbModels;

namespace WikiExtractor.Repository
{
    public class AppMenuItemRepository : RepositorySqliteNetBase<AppMenuItem>, IRepositoryBase<AppMenuItem>, IRepositoryBaseAppExtension
    {
        public AppMenuItemRepository(DatabaseHelper databaseHelper) : base(databaseHelper, "tblAppMenuItem",
          "TitleOnThePage, Tags, MenuItemName, Sequence",
          "TitleOnThePage, Tags, MenuItemName, Sequence")
        { }

        public string SchemaScript(int databaseVersion)
        {
            var createStr = new StringBuilder();
            if (databaseVersion <= 0)
            {
                createStr.Append($@"CREATE TABLE [{_tableName}] (
	                                [Id]	            INTEGER NOT NULL UNIQUE,
	                                [TitleOnThePage]	TEXT,	                                
                                    [Tags]	            TEXT,
	                                [MenuItemName]	    TEXT,
                                    [Sequence]          INTEGER,
	                                PRIMARY KEY([Id] AUTOINCREMENT)
                                    );");
            }

            return createStr.ToString();
        }
    }
}
