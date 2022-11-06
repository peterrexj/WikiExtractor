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
        public AppDatabase(bool createNew)
        {
            CreateNew = createNew;
        }

        public bool CreateNew { get; set; }
        public static bool IsInitialized = false;

        public void InitializeDatabase()
        {
            if (!AppDatabase.IsInitialized && CreateNew)
            {
                IoHelper.DeleteFile(ProcessConstants.DatabasePath);
                _dbHelper = new DatabaseHelper(DatabaseHelper.DatabaseType.SqLite, new[] { ProcessConstants.DatabasePath });
                ApplySchema();
            }
            else
            {
                _dbHelper = new DatabaseHelper(DatabaseHelper.DatabaseType.SqLite, new[] { ProcessConstants.DatabasePath });
            }
            IsInitialized = true;
        }

        void ApplySchema()
        {
            //_dbHelper.DbHelper.ExecuteNonQuery(File.ReadAllText(IoHelper.CombinePath(PjUtility.Runtime.ExecutingFolder, "Repository", "SchemaDb.sql")));
        }

        public System.Data.SQLite.SQLiteConnection? Connection => _dbHelper.DbHelper.Connection as System.Data.SQLite.SQLiteConnection;
    }
}
