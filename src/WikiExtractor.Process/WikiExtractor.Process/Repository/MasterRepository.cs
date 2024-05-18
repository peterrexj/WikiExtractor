using Pj.Library;
using Pj.Library.Datastore.Repository;
using System.Text;
using WikiExtractor.DbModels;

namespace WikiExtractor.Repository
{
    public class MasterRepository : RepositorySqliteNetBase<Master>, IRepositoryBase<Master>, IRepositoryBaseAppExtension
    {
        public MasterRepository(DatabaseHelper databaseHelper) : base(databaseHelper, "tblMaster",
           "Name, Route",
           "Name, Route")
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
