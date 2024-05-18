using Pj.Library;
using Pj.Library.Datastore.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.DbModels;

namespace WikiExtractor.Repository
{
    public class ParagraphHeader3Repository : RepositorySqliteNetBase<ParagraphHeader3>, IRepositoryBase<ParagraphHeader3>, IRepositoryBaseAppExtension
    {
        public ParagraphHeader3Repository(DatabaseHelper databaseHelper) : base(databaseHelper, "tblParagraphHeader3",
           "MasterId, ParagraphHeader2Id, Header, Sequence",
           "MasterId, ParagraphHeader2Id, Header, Sequence")
        { }

        public string SchemaScript(int databaseVersion)
        {
            var createStr = new StringBuilder();
            if (databaseVersion <= 0)
            {
                createStr.Append($@"CREATE TABLE [{_tableName}] (
	                                [Id]	                INTEGER NOT NULL UNIQUE,
	                                [MasterId]	            INTEGER,
	                                [ParagraphHeader2Id]	INTEGER,
	                                [Header]	            TEXT,
	                                [Sequence]	            INTEGER,
	                                PRIMARY KEY([Id] AUTOINCREMENT)
                                    );");
            }

            return createStr.ToString();
        }

        public List<ParagraphHeader3> GetByMasterId(int masterId)
        {
            return Task.Run(() => _dbHelper.DbHelper.SqliteConnection.QueryAsync<ParagraphHeader3>($@"Select * from {_tableName} where MasterId = {masterId}")).Result;
        }
        public List<ParagraphHeader3> GetByMasterIdWithFields(int masterId, params string[] fields)
        {
            return Task.Run(() => _dbHelper.DbHelper.SqliteConnection.QueryAsync<ParagraphHeader3>($@"Select {string.Join(",", fields)} from {_tableName} where MasterId = {masterId}")).Result;
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
