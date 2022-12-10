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
    public class TagRepository : RepositoryBase<Tag>, IRepositoryBase<Tag>, IRepositoryBaseAppExtension
    {
        public TagRepository(DatabaseHelper databaseHelper) : base(databaseHelper, "tblTag",
          "Name",
          "Name")
        { }

        public string SchemaScript(int databaseVersion)
        {
            var createStr = new StringBuilder();
            if (databaseVersion <= 0)
            {
                createStr.Append($@"CREATE TABLE [{_tableName}] (
	                                [Id]	INTEGER NOT NULL UNIQUE,
	                                [Name]	TEXT,
	                                PRIMARY KEY([Id] AUTOINCREMENT)
                                    );");
            }

            return createStr.ToString();
        }
    }
}
