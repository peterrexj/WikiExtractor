using Pj.Library;
using Pj.Library.Datastore.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.DbModels;

namespace WikiExtractor.Repository
{
    public class WikiPictureRepository : RepositorySqliteNetBase<WikiPicture>, IRepositoryBase<WikiPicture>, IRepositoryBaseAppExtension
    {
        public WikiPictureRepository(DatabaseHelper databaseHelper) : base(databaseHelper, "tblWikiPicture",
           "MasterId, Sequence, Width, Height, Path, Caption, IsPrimary",
           "MasterId, Path")
        { }

        public string SchemaScript(int databaseVersion)
        {
            var createStr = new StringBuilder();
            if (databaseVersion <= 0)
            {
                createStr.Append($@"CREATE TABLE [{_tableName}] (
	                                [Id]	        INTEGER NOT NULL UNIQUE,
	                                [MasterId]	    INTEGER,
	                                [Sequence]		INTEGER,
	                                [Width]		    INTEGER,
	                                [Height]		INTEGER,
	                                [Path]		    TEXT,
	                                [Caption]	    TEXT,
	                                [IsPrimary]	    INTEGER,
	                                PRIMARY KEY([Id] AUTOINCREMENT)
                                    );");
            }

            return createStr.ToString();
        }

        public List<WikiPicture> GetByMasterId(int masterId)
        {
            return Task.Run(() => _dbHelper.DbHelper.SqliteConnection.QueryAsync<WikiPicture>($@"Select * from {_tableName} where MasterId = {masterId}")).Result;
        }
        public List<WikiPicture> GetAllPrimaryPictures()
        {
            return Task.Run(() => _dbHelper.DbHelper.SqliteConnection.QueryAsync<WikiPicture>($@"Select * from {_tableName} where IsPrimary = 1")).Result;
        }
        public List<WikiPicture> GetAllPrimaryPicturesWithFields(params string[] fields)
        {
            return Task.Run(() => _dbHelper.DbHelper.SqliteConnection.QueryAsync<WikiPicture>($@"Select {string.Join(",", fields)} from {_tableName} where IsPrimary = 1")).Result;
        }

        public void DeleteByMasterId(int masterId)
        {
            var ids = GetAll()
                .Where(f => f.MasterId == masterId)
                .Select(f => f.Id);

            foreach (var id in ids)
            {
                Delete(id.ToString());
            }
        }
    }
}
