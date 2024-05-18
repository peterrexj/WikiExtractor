using Pj.Library;
using Pj.Library.Datastore.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.DbModels;

namespace WikiExtractor.Repository
{
    public class ParagraphContentRepository : RepositorySqliteNetBase<ParagraphContent>, IRepositoryBase<ParagraphContent>, IRepositoryBaseAppExtension
    {
        public ParagraphContentRepository(DatabaseHelper databaseHelper) : base(databaseHelper, "tblParagraphContent",
           "MasterId, ParagraphHeader2Id, ParagraphHeader3Id, HashContent, Content",
           "MasterId, ParagraphHeader2Id, ParagraphHeader3Id, HashContent")
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
	                                [ParagraphHeader3Id]	INTEGER,
	                                [HashContent]			INTEGER,
	                                [Content]				TEXT,
	                                PRIMARY KEY([Id] AUTOINCREMENT)
                                    );");
            }

            return createStr.ToString();
        }

        public List<ParagraphContent> GetByMasterId(int masterId)
        {
            return Task.Run(() => _dbHelper.DbHelper.SqliteConnection.QueryAsync<ParagraphContent>($@"Select * from {_tableName} where MasterId = {masterId}")).Result;
        }
        public List<ParagraphContent> GetByMasterIdWithIdOnly(int masterId)
        {
            return Task.Run(() => _dbHelper.DbHelper.SqliteConnection.QueryAsync<ParagraphContent>($@"Select Id from {_tableName} where MasterId = {masterId}")).Result;
        }
        public List<ParagraphContent> GetByMasterIdWithFields(int masterId, params string[] fields)
        {
            return Task.Run(() => _dbHelper.DbHelper.SqliteConnection.QueryAsync<ParagraphContent>($@"Select {string.Join(",", fields)} from {_tableName} where MasterId = {masterId}")).Result;
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
