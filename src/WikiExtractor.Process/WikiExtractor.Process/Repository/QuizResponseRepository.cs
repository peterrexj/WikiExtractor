using System;
using Pj.Library;
using Pj.Library.Datastore.Repository;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.Process.DbModels;

namespace WikiExtractor.Process.Repository
{
    public class QuizResponseRepository : RepositorySqliteNetBase<QuizResponse>, IRepositoryBase<QuizResponse>, IRepositoryBaseAppExtension
    {
        public QuizResponseRepository(DatabaseHelper databaseHelper) : base(databaseHelper, "tblQuizResponse",
            "MasterId, MetadataKey, UserResponse, QuestionSetId, CreatedDateTime",
            "MasterId, MetadataKey, UserResponse, QuestionSetId, CreatedDateTime")
        { }

        public string SchemaScript(int databaseVersion)
        {
            var createStr = new StringBuilder();
            if (databaseVersion <= 2 || databaseVersion == 5)
            {
                createStr.Append($@"CREATE TABLE IF NOT EXISTS [{_tableName}] (
	                                [Id]	            INTEGER NOT NULL UNIQUE,
	                                [MasterId]          INTEGER,
                                    [MetadataKey]       TEXT,
                                    [UserResponse]      INTEGER,
                                    [QuestionSetId]     INTEGER,
                                    [CreatedDateTime]   DATE,
	                                PRIMARY KEY([Id] AUTOINCREMENT)
                                    );");
            }

            return createStr.ToString();
        }

        public int GetNewQuestionSetId()
        {
            var newQuestSetQry = $"SELECT IFNULL(MAX(QuestionSetId), 0) + 1 AS NewId FROM {_tableName}";
            var maxId = Task.Run(() => _dbHelper.DbHelper.SqliteConnection.ExecuteScalarAsync<int>(newQuestSetQry)).Result;
            return maxId;
        }
    }
}
