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
    public class ParagraphImageRepository : RepositoryBase<ParagraphImage>, IRepositoryBase<ParagraphImage>, IRepositoryBaseAppExtension
    {
        public ParagraphImageRepository(DatabaseHelper databaseHelper) : base(databaseHelper, "tblParagraphImage",
          "MasterId, ImageId, ParagraphId",
          "MasterId, ImageId, ParagraphId")
        { }

        public string SchemaScript(int databaseVersion)
        {
            var createStr = new StringBuilder();
            if (databaseVersion <= 0)
            {
                createStr.Append($@"CREATE TABLE [{_tableName}] (
	                                [Id]	        INTEGER NOT NULL UNIQUE,
	                                [MasterId]	    INTEGER,
	                                [ImageId]	    INTEGER,
	                                [ParagraphId]	INTEGER,
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
