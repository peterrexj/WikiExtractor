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
                createStr.Append($@"CREATE TABLE [{_tableName}] (
	                                [Id]	         INTEGER NOT NULL UNIQUE,
	                                [RequestDate]    DATE,
                                    [RequestCount]   INTEGER,
	                                PRIMARY KEY([Id] AUTOINCREMENT)
                                    );");
            }

            return createStr.ToString();
        }

        public void RefreshCountData()
        {
            DeleteAll();
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
            var data = Get(/*d => d.RequestDate == DateTime.Today.Date*/).FirstOrDefault();
            if (data == null)
                return 0;
            else
                return data.RequestCount;
        }
        public int IncrementCount() => GetCount() + 1;
        public bool RequestOnLimit
        {
            get
            {
                var count = GetCount();
                if (count < ConfigData.AdsIntersitialLimitOnRecord) return false;
                return count % ConfigData.AdsIntersitialLimitOnRecord == 0;
            }
        }
    }
}
