using Pj.Library.Mobile.Sqlite;
using Pj.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.DbModels;

namespace WikiExtractor.Repository
{
    public class TagItemRepository : RepositoryBase<TagItem>, IRepositoryBase<TagItem>, IRepositoryBaseAppExtension
    {
        public TagItemRepository(DatabaseHelper databaseHelper) : base(databaseHelper, "tblTagItem",
          "MasterId, TagId",
          "MasterId, TagId")
        { }

        public string SchemaScript(int databaseVersion)
        {
            var createStr = new StringBuilder();
            if (databaseVersion <= 0)
            {
                createStr.Append($@"CREATE TABLE [{_tableName}] (
	                                [Id]	    INTEGER NOT NULL UNIQUE,
	                                [MasterId]	INTEGER,
                                    [TagId]     INTEGER,
	                                PRIMARY KEY([Id] AUTOINCREMENT)
                                    );");
            }

            return createStr.ToString();
        }
    }
}
