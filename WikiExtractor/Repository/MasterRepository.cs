using Pj.Library;
using Pj.Library.Mobile.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.DbModels;

namespace WikiExtractor.Repository
{
    public class MasterRepository : RepositoryBase<Master>, IRepositoryBase<Master>, IRepositoryBaseAppExtension
    {
        public MasterRepository(DatabaseHelper databaseHelper) : base(databaseHelper, "tblMaster",
           "Name, Route",
           "Route")
            { }

        public string SchemaScript(int databaseVersion)
        {
            var createStr = new StringBuilder();
            if (databaseVersion <= 0)
            {
                createStr.Append($@"CREATE TABLE [{_tableName}] (
	                                [Id]	INTEGER NOT NULL UNIQUE,
	                                [Name]	TEXT,
	                                [Route] TEXT,
	                                PRIMARY KEY([Id] AUTOINCREMENT)
                                    );");
            }

            return createStr.ToString();
        }
    }
}
