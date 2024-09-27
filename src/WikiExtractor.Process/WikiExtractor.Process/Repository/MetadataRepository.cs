using Pj.Library;
using Pj.Library.Datastore.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.DbModels;

namespace WikiExtractor.Repository
{
    public class MetadataRepository : RepositorySqliteNetBase<Metadata>, IRepositoryBase<Metadata>, IRepositoryBaseAppExtension
    {
        public MetadataRepository(DatabaseHelper databaseHelper) : base(databaseHelper, "tblMetadata",
           "MasterId, Key, Value, Sequence, Type",
           "MasterId, Key, Value")
        { }

        public string SchemaScript(int databaseVersion)
        {
            var createStr = new StringBuilder();
            if (databaseVersion <= 0)
            {
                createStr.Append($@"CREATE TABLE [{_tableName}] (
	                                [Id]	    INTEGER NOT NULL UNIQUE,
	                                [MasterId]	INTEGER,
	                                [Key]		TEXT,
	                                [Value]		TEXT,
	                                [Sequence]	INTEGER,
	                                [Type]		TEXT,
	                                PRIMARY KEY([Id] AUTOINCREMENT)
                                    );");
            }

            return createStr.ToString();
        }

        public List<Metadata> GetByMasterId(int masterId)
        {
            return Task.Run(() => _dbHelper.DbHelper.SqliteConnection.QueryAsync<Metadata>($@"Select * from {_tableName} where MasterId = {masterId}")).Result;
        }
        public List<Metadata> GetByMasterIdWithFields(int masterId, params string[] fields)
        {
            return Task.Run(() => _dbHelper.DbHelper.SqliteConnection.QueryAsync<Metadata>($@"Select {string.Join(",", fields)} from {_tableName} where MasterId = {masterId}")).Result;
        }

        public void DeleteByMasterId(int masterId)
        {
            var ids = GetAll()
                .Where(f => f.MasterId == masterId)
                .Select(f => f.Id);

            foreach (var id in ids)
            {
                Delete(id.ToString());
            }
        }
    }
}
