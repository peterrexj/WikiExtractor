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
    public class ParagraphContentRepository : RepositoryBase<ParagraphContent>, IRepositoryBase<ParagraphContent>, IRepositoryBaseAppExtension
    {
        public ParagraphContentRepository(DatabaseHelper databaseHelper) : base(databaseHelper, "tblParagraphContent",
           "MasterId, ParagraphHeader2Id, ParagraphHeader3Id, Content",
           "MasterId, ParagraphHeader2Id, ParagraphHeader3Id, Content")
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
	                                [Content]				TEXT,
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
