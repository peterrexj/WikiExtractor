using Pj.Library;
using Pj.Library.Datastore.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WikiExtractor.DbModels;

namespace WikiExtractor.Repository
{
    public class PhoneSettingsRepository : RepositorySqliteNetBase<PhoneSettings>, IRepositoryBase<PhoneSettings>, IRepositoryBaseAppExtension
    {
        public PhoneSettingsRepository(DatabaseHelper databaseHelper) : base(databaseHelper, "tblPhoneSettings",
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

        #region Primary Metadata Display

        public void EnablePrimaryMetadatDisplay(int maxItemToDisplay)
        {
            if (maxItemToDisplay <= 0)
            {
                throw new Exception("The Max Items to display should be more than 0!");
            }

            Update("PrimaryMetadatDisplay", "true");
            Update("MaxPrimaryMetadatDisplay", maxItemToDisplay.ToString());
        }

        public void DisablePrimaryMetadatDisplay()
        {
            Update("PrimaryMetadatDisplay", "false");
        }

        public bool IsPrimaryMetadatDisplayEnabled => GetValue("PrimaryMetadatDisplay").ToBool();
        public int MaxMetadataItemToDisplay => GetValue("MaxPrimaryMetadatDisplay").ToInteger();

        public void AddPrimaryMetadatDisplayContent(string value)
        {
            if (IsPrimaryMetadatDisplayEnabled)
            {
                var content = PrimaryMetadatDisplayContent;
                content.Add(value);
                var newContent = string.Join(",", content.Distinct());

                RemoveAllPrimaryMetadatDisplayContent();
                Update("PrimaryMetadatDisplayContent", newContent);
            }
            else
            {
                throw new Exception("The [PrimaryMetadatDisplay] is not enabled in the store, you need to enable that first!");
            }
        }
        public void AddPrimaryMetadatDisplayContent(List<string> values)
        {
            if (IsPrimaryMetadatDisplayEnabled)
            {
                var content = PrimaryMetadatDisplayContent;
                content.AddRange(values);
                var newContent = string.Join(",", content.Distinct());

                RemoveAllPrimaryMetadatDisplayContent();
                Update("PrimaryMetadatDisplayContent", newContent);
            }
            else
            {
                throw new Exception("The [PrimaryMetadatDisplay] is not enabled in the store, you need to enable that first!");
            }
        }

        public void RemovePrimaryMetadatDisplayContent(string value)
        {
            if (IsPrimaryMetadatDisplayEnabled)
            {
                var content = PrimaryMetadatDisplayContent;
                var newContent = string.Join(",", content.Where(f => f != value).Distinct());

                RemoveAllPrimaryMetadatDisplayContent();
                Update("PrimaryMetadatDisplayContent", newContent);
            }
            else
            {
                throw new Exception("The [PrimaryMetadatDisplay] is not enabled in the store, you need to enable that first!");
            }
        }

        public void RemoveAllPrimaryMetadatDisplayContent()
        {
            DeleteByName("PrimaryMetadatDisplayContent");
        }

        public List<string> PrimaryMetadatDisplayContent => GetValue("PrimaryMetadatDisplayContent").SplitAndTrim(",").ToList();

        #endregion
    }
}
