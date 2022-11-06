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
    public class ParagraphPrimaryContentRepository : RepositoryBase<ParagraphPrimaryContent>, IRepositoryBase<ParagraphPrimaryContent>, IRepositoryBaseAppExtension
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
    }
}
