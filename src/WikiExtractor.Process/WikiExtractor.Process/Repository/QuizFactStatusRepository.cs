using System;
using Pj.Library;
using Pj.Library.Datastore.Repository;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.Process.DbModels;

namespace WikiExtractor.Process.Repository
{
    public class QuizFactStatusRepository : RepositorySqliteNetBase<QuizFactStatus>, IRepositoryBase<QuizFactStatus>, IRepositoryBaseAppExtension
    {
        public QuizFactStatusRepository(DatabaseHelper databaseHelper) : base(databaseHelper, "tblQuizFactStatus",
            "MasterId, MetadataKey, CreatedDateTime",
            "MasterId, MetadataKey")
        { }

        public string SchemaScript(int databaseVersion)
        {
            var createStr = new StringBuilder();
            if (databaseVersion <= 6)
            {
                createStr.Append($@"CREATE TABLE IF NOT EXISTS [{_tableName}] (
	                                [Id]	            INTEGER NOT NULL UNIQUE,
	                                [MasterId]          INTEGER,
                                    [MetadataKey]       TEXT,
                                    [CreatedDateTime]   DATE,
	                                PRIMARY KEY([Id] AUTOINCREMENT)
                                    );");
            }

            return createStr.ToString();
        }
    }
}
