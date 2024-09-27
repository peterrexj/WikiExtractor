using Pj.Library;
using Pj.Library.Datastore.Repository;
using System.Text;
using WikiExtractor.Process.DbModels;

namespace WikiExtractor.Repository
{
    public class QuizMasterMetadataRepository(DatabaseHelper databaseHelper)
        : RepositorySqliteNetBase<QuizMasterMetadata>(databaseHelper, "tblQuizMasterMetadata",
            "MasterId, MetadataKey",
            "MasterId, MetadataKey"), IRepositoryBase<QuizMasterMetadata>, IRepositoryBaseAppExtension
    {
        public string SchemaScript(int databaseVersion)
        {
            var createStr = new StringBuilder();
            if (databaseVersion <= 0)
            {
                createStr.Append($@"CREATE TABLE [{_tableName}] (
	                                [Id]	        INTEGER NOT NULL UNIQUE,
	                                [MasterId]	    INTEGER,
	                                [MetadataKey]	TEXT,
	                                PRIMARY KEY([Id] AUTOINCREMENT)
                                    );");
            }

            return createStr.ToString();
        }
    }
}
