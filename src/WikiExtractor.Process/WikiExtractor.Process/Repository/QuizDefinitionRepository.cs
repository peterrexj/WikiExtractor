using Pj.Library;
using Pj.Library.Datastore.Repository;
using System.Text;
using WikiExtractor.Process.DbModels;

namespace WikiExtractor.Repository
{
    public class QuizDefinitionRepository : RepositorySqliteNetBase<QuizDefinition>, IRepositoryBase<QuizDefinition>, IRepositoryBaseAppExtension
    {
        public QuizDefinitionRepository(DatabaseHelper databaseHelper) : base(databaseHelper, "tblQuizDefinition",
            "MetadataKey, QuestionPhrase",
            "MetadataKey, QuestionPhrase")
        {
        }

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
