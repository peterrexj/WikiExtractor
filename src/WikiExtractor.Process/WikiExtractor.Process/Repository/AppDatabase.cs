using Pj.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikiExtractor.Process;

namespace WikiExtractor.Repository
{
    public class AppDatabase
    {
        public DatabaseHelper _dbHelper;
        public DatabaseHelper _dbHelperUserStore;
        public AppDatabase(bool createNew)
        {
            CreateNew = createNew;
        }

        public bool CreateNew { get; set; }
        public static bool IsInitialized = false;
        public static bool IsInitializedUserStore = false;

        public void InitializeDatabase()
        {
            if (!AppDatabase.IsInitialized && CreateNew)
            {
                //IoHelper.DeleteFile(ProcessConstants.DatabasePath);
                _dbHelper = new DatabaseHelper(DatabaseHelper.DatabaseType.SqLite, new[] { ProcessConstants.DatabasePath });
                ApplySchema();
            }
            else
            {
                _dbHelper = new DatabaseHelper(DatabaseHelper.DatabaseType.SqLite, new[] { ProcessConstants.DatabasePath });
            }
            IsInitialized = true;
        }

        public void InitializeDatabaseUserStore()
        {
            if (!AppDatabase.IsInitializedUserStore && CreateNew)
            {
                _dbHelperUserStore = new DatabaseHelper(DatabaseHelper.DatabaseType.SqLite, new[] { ProcessConstants.UserStoreDatabasePath });
            }
            else
            {
                _dbHelperUserStore = new DatabaseHelper(DatabaseHelper.DatabaseType.SqLite, new[] { ProcessConstants.UserStoreDatabasePath });
            }
            IsInitializedUserStore = true;
        }

        void ApplySchema()
        {
            //_dbHelper.DbHelper.ExecuteNonQuery(File.ReadAllText(IoHelper.CombinePath(PjUtility.Runtime.ExecutingFolder, "Repository", "SchemaDb.sql")));
        }

        public Microsoft.Data.Sqlite.SqliteConnection? Connection => _dbHelper.DbHelper.Connection as Microsoft.Data.Sqlite.SqliteConnection;
        public Microsoft.Data.Sqlite.SqliteConnection? ConnectionUserStore => _dbHelperUserStore.DbHelper.Connection as Microsoft.Data.Sqlite.SqliteConnection;
    }
}
