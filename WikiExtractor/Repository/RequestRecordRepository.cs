using Pj.Library.Mobile.Sqlite;
using Pj.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.DbModels;
using WikiExtractor.Exts;

namespace WikiExtractor.Repository
{
    public class RequestRecordRepository : RepositoryBase<RequestRecord>, IRepositoryBase<RequestRecord>, IRepositoryBaseAppExtension
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
                createStr.Append($@"CREATE TABLE [{_tableName}] (
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
                Add(data, checkAlreadyExists: true);
            }
            else
            {
                Update(data);
            }
        }

        public int GetCount()
        {
            var data = Get(d => d.RequestDate == DateTime.Today.Date).FirstOrDefault();
            if (data == null)
                return 0;
            else
                return data.RequestCount;
        }
        public int IncrementCount() => GetCount() + 1;
        public bool RequestOnLimit => GetCount() % ConfigData.AdsIntersitialLimitOnRecord == 0;
    }
}
