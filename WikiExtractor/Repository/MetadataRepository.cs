using Pj.Library;
using Pj.Library.Datastore.Repository;
using System.Linq;
using System.Text;
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
	                                [Id]	INTEGER NOT NULL UNIQUE,
	                                [MasterId]	INTEGER,
	                                [Key]		TEXT,
	                                [Value]		TEXT,
	                                [Sequence]		INTEGER,
	                                [Type]		TEXT,
	                                PRIMARY KEY([Id] AUTOINCREMENT)
                                    );");
            }

            return createStr.ToString();
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
