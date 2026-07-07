using Pj.Library;
using Pj.Library.Datastore.Repository;
using System;
using System.Linq;
using System.Text;
using WikiExtractor.DbModels;
using WikiExtractor.Exts;

namespace WikiExtractor.Repository
{
    public class RequestRecordRepository : RepositorySqliteNetBase<RequestRecord>, IRepositoryBase<RequestRecord>, IRepositoryBaseAppExtension
    {
        public RequestRecordRepository(DatabaseHelper databaseHelper) : base(databaseHelper, "RequestRecord",
          "RequestDate, RequestCount",
          "RequestDate")
        { }

        public string SchemaScript(int databaseVersion)
        {
            var createStr = new StringBuilder();
            if (databaseVersion <= 0)
            {
                createStr.Append($@"CREATE TABLE IF NOT EXISTS [{_tableName}] (
	                                [Id]	         INTEGER NOT NULL UNIQUE,
	                                [RequestDate]    DATE,
                                    [RequestCount]   INTEGER,
	                                PRIMARY KEY([Id] AUTOINCREMENT)
                                    );");
            }

            return createStr.ToString();
        }

        public void UpdateCount()
        {
            var data = new RequestRecord { RequestDate = DateTime.Today.Date, RequestCount = IncrementCount() };
            var dataAvailable = Find(data);
            if (dataAvailable == null || dataAvailable.Count() == 0)
            {
                DeleteAll();
                Add(data, checkAlreadyExists: false);
            }
            else
            {
                Update(data);
            }
        }

        public int GetCount()
        {
            var dataAvailable = Find(new RequestRecord { RequestDate = DateTime.Today.Date });
            if (dataAvailable != null && dataAvailable.Any())
            {
                return dataAvailable.Max(f => f.RequestCount);
            }
            return 0;
        }
        public int IncrementCount() => GetCount() + 1;
    }
}
