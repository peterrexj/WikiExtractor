using Pj.Library;
using Pj.Library.Mobile.Model;
using Pj.Library.Mobile.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace WikiExtractor.Repository
{
    public class WikiDatabase : IWikiDatabase
    {
        AppDatabase appDatabase;
        public MasterRepository MasterRepository { get; set; }
        public MetadataRepository MetadataRepository { get; set; }
        public ParagraphPrimaryContentRepository ParagraphPrimaryContentRepository { get; set; }
        public ParagraphHeader2Repository ParagraphHeader2Repository { get; set; }
        public ParagraphHeader3Repository ParagraphHeader3Repository { get; set; }
        public ParagraphContentRepository ParagraphContentRepository { get; set; }
        public WikiPictureRepository WikiPictureRepository { get; set; }
        public SettingsRepository SettingsRepository { get; set; }

        protected List<IRepositoryBaseAppExtension> repoExtensions;

        public WikiDatabase()
        {
            repoExtensions = new List<IRepositoryBaseAppExtension>();
            appDatabase = new AppDatabase(false);
            InitializeDatabase();
        }

        public void InitializeDatabase()
        {
            appDatabase.InitializeDatabase();

            SettingsRepository = new SettingsRepository(appDatabase._dbHelper);
            MasterRepository = new MasterRepository(appDatabase._dbHelper);
            MetadataRepository = new MetadataRepository(appDatabase._dbHelper);
            ParagraphPrimaryContentRepository = new ParagraphPrimaryContentRepository(appDatabase._dbHelper);
            ParagraphHeader2Repository = new ParagraphHeader2Repository(appDatabase._dbHelper);
            ParagraphHeader3Repository = new ParagraphHeader3Repository(appDatabase._dbHelper);
            ParagraphContentRepository = new ParagraphContentRepository(appDatabase._dbHelper);
            WikiPictureRepository = new WikiPictureRepository(appDatabase._dbHelper);

            repoExtensions.Add(SettingsRepository);
            repoExtensions.Add(MasterRepository);
            repoExtensions.Add(MetadataRepository);
            repoExtensions.Add(ParagraphPrimaryContentRepository);
            repoExtensions.Add(ParagraphHeader2Repository);
            repoExtensions.Add(ParagraphHeader3Repository);
            repoExtensions.Add(ParagraphContentRepository);
            repoExtensions.Add(WikiPictureRepository);

            int currentDbVersion = 0;
            bool requireDbInitialization = false;

            try
            {
                currentDbVersion = SettingsRepository.GetValue("DatabaseVersion").ToInteger();
            }
            catch (Exception)
            {
                requireDbInitialization = true;
            }


            if (requireDbInitialization)
            {
                repoExtensions.Iter(f => appDatabase._dbHelper.DbHelper.ExecuteNonQuery(f.SchemaScript(0)));
                SettingsRepository.Add("DatabaseVersion", "1");
            }
            else
            {
                if (currentDbVersion >= 1)
                {
                    foreach (var repo in repoExtensions)
                    {
                        if (repo != null)
                        {
                            var qry = repo.SchemaScript(currentDbVersion);
                            if (qry.HasValue())
                            {
                                appDatabase._dbHelper.DbHelper.ExecuteNonQuery(qry);
                            }
                        }
                    }
                }
            }

            //ExecuteSchema(MasterRepository.SchemaScript(0));
            //ExecuteSchema(MetadataRepository.SchemaScript(0));
            //ExecuteSchema(ParagraphPrimaryContentRepository.SchemaScript(0));
            //ExecuteSchema(ParagraphHeader2Repository.SchemaScript(0));
            //ExecuteSchema(ParagraphHeader3Repository.SchemaScript(0));
            //ExecuteSchema(ParagraphContentRepository.SchemaScript(0));
            //ExecuteSchema(WikiPictureRepository.SchemaScript(0));
        }

        private void ExecuteSchema(string script)
        {
            if (script.HasValue())
            {
                appDatabase._dbHelper.DbHelper.ExecuteNonQuery(script);
            }
        }
    }
}
