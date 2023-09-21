using Pj.Library;
using Pj.Library.Datastore.Repository;
using System.Linq;
using System.Text;
using WikiExtractor.DbModels;

namespace WikiExtractor.Repository
{
    public class ParagraphPrimaryContentRepository : RepositorySqliteNetBase<ParagraphPrimaryContent>, IRepositoryBase<ParagraphPrimaryContent>, IRepositoryBaseAppExtension
    {
        public ParagraphPrimaryContentRepository(DatabaseHelper databaseHelper) : base(databaseHelper, "tblParagraphPrimaryContent",
           "MasterId, Content",
           "MasterId, Content")
        { }

        public string SchemaScript(int databaseVersion)
        {
            var createStr = new StringBuilder();
            if (databaseVersion <= 0)
            {
                createStr.Append($@"CREATE TABLE [{_tableName}] (
	                                [Id]	INTEGER NOT NULL UNIQUE,
	                                [MasterId]	INTEGER,
	                                [Content]	TEXT,
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
