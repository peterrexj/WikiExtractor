using Pj.Library;
using Pj.Library.Datastore.Repository;
using System.Linq;
using System.Text;
using WikiExtractor.DbModels;

namespace WikiExtractor.Repository
{
    public class ParagraphHeader2Repository : RepositorySqliteNetBase<ParagraphHeader2>, IRepositoryBase<ParagraphHeader2>, IRepositoryBaseAppExtension
    {
        public ParagraphHeader2Repository(DatabaseHelper databaseHelper) : base(databaseHelper, "tblParagraphHeader2",
           "MasterId, Header, Sequence",
           "MasterId, Header, Sequence")
        { }

        public string SchemaScript(int databaseVersion)
        {
            var createStr = new StringBuilder();
            if (databaseVersion <= 0)
            {
                createStr.Append($@"CREATE TABLE [{_tableName}] (
	                                [Id]	INTEGER NOT NULL UNIQUE,
	                                [MasterId]	INTEGER,
	                                [Header]	TEXT,
	                                [Sequence]	INTEGER,
	                                PRIMARY KEY([Id] AUTOINCREMENT)
                                    );");
            }

            return createStr.ToString();
        }

        public List<ParagraphHeader2> GetByMasterId(int masterId)
        {
            return Task.Run(() => _dbHelper.DbHelper.SqliteConnection.QueryAsync<ParagraphHeader2>($@"Select * from {_tableName} where MasterId = {masterId}")).Result;
        }
        public List<ParagraphHeader2> GetByMasterIdWithFields(int masterId, params string[] fields)
        {
            return Task.Run(() => _dbHelper.DbHelper.SqliteConnection.QueryAsync<ParagraphHeader2>($@"Select {string.Join(",", fields)} from {_tableName} where MasterId = {masterId}")).Result;
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
