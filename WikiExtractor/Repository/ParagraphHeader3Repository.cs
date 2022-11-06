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
    public class ParagraphHeader3Repository : RepositoryBase<ParagraphHeader3>, IRepositoryBase<ParagraphHeader3>, IRepositoryBaseAppExtension
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
    }
}
