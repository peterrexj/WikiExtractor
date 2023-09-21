using Pj.Library;
using Pj.Library.Datastore.Repository;
using System.Linq;
using System.Text;
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
