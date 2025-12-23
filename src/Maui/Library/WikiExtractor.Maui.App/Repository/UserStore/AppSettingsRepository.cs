using Pj.Library;
using Pj.Library.Datastore.Repository;
using System.Linq;
using System.Text;
using WikiExtractor.DbModels;

namespace WikiExtractor.Repository.UserStore
{
    public class AppSettingsRepository : RepositorySqliteNetBase<PhoneSettings>, IRepositoryBase<PhoneSettings>, IRepositoryBaseAppExtension
    {
        public AppSettingsRepository(DatabaseHelper databaseHelper) : base(databaseHelper, "tblPhoneSettings",
            "Name, Value", "Name")
        {

        }

        public int Add(string name, string value)
        {
            return base.Add(new PhoneSettings { Name = name, Value = value }, checkAlreadyExists: true);
        }

        public int Update(string name, string value)
        {
            return base.Update(new PhoneSettings { Name = name, Value = value });
        }

        public string GetValue(string name)
        {
            return Get(s => s.Name.EqualsIgnoreCase(name)).FirstOrDefault()?.Value ?? "";
        }

        public void DeleteByName(string name)
        {
            var id = Get(f => f.Name == name)?.FirstOrDefault()?.Id;
            if (id != null && id != 0)
            {
                Delete(id.Value.ToString());
            }
        }

        public string SchemaScript(int databaseVersion)
        {
            var createStr = new StringBuilder();
            if (databaseVersion <= 0)
            {
                createStr.Append($@"CREATE TABLE [{_tableName}] (
	                                [Id]	INTEGER NOT NULL UNIQUE,
	                                [Name]  TEXT,
                                    [Value] TEXT,
	                                PRIMARY KEY([Id] AUTOINCREMENT)
                                    );");
            }

            return createStr.ToString();
        }

        #region Google Ads Service
        public void UpdateGoogleAdsLimitOnIntersitial(int limitCount)
        {
            Update("AdsIntersitialLimitOnRecord", limitCount.ToString());
        }

        public int GetGoogleAdsIntersitialLimit()
        {
            var value = GetValue("AdsIntersitialLimitOnRecord");
            if (value == null) return 0;
            else return value.ToInteger();
        }
        #endregion
    }
}
