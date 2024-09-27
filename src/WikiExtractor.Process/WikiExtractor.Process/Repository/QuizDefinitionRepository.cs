using Pj.Library;
using Pj.Library.Datastore.Repository;
using System.Text;
using WikiExtractor.Process.DbModels;

namespace WikiExtractor.Repository
{
    public class QuizDefinitionRepository(DatabaseHelper databaseHelper)
        : RepositorySqliteNetBase<QuizDefinition>(databaseHelper, "tblQuizDefinition",
            "MetadataKey, QuestionPhrase",
            "MetadataKey, QuestionPhrase"), IRepositoryBase<QuizDefinition>, IRepositoryBaseAppExtension
    {
        public string SchemaScript(int databaseVersion)
        {
            var createStr = new StringBuilder();
            if (databaseVersion <= 0)
            {
                createStr.Append($@"CREATE TABLE [{_tableName}] (
	                                [Id]	            INTEGER NOT NULL UNIQUE,
	                                [MetadataKey]	    TEXT,
                                    [QuestionPhrase]	TEXT,
	                                PRIMARY KEY([Id] AUTOINCREMENT)
                                    );");
            }

            return createStr.ToString();
        }
    }
}
