using Pj.Library;
using Pj.Library.Mobile.Model;

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
        public ParagraphImageRepository ParagraphImageRepository { get; set; }
        public WikiPictureRepository WikiPictureRepository { get; set; }
        public SettingsSqliteRepository SettingsRepository { get; set; }
        public PhoneSettingsRepository PhoneSettingsRepository { get; set; }
        public TagRepository TagRepository { get; set; }
        public TagItemRepository TagItemRepository { get; set; }
        public AppMenuItemRepository AppMenuItemRepository { get; set; }
        public RequestRecordRepository RequestRecordRepository { get; set; }

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

            SettingsRepository = new SettingsSqliteRepository(appDatabase._dbHelper);
            MasterRepository = new MasterRepository(appDatabase._dbHelper);
            MetadataRepository = new MetadataRepository(appDatabase._dbHelper);
            ParagraphPrimaryContentRepository = new ParagraphPrimaryContentRepository(appDatabase._dbHelper);
            ParagraphHeader2Repository = new ParagraphHeader2Repository(appDatabase._dbHelper);
            ParagraphHeader3Repository = new ParagraphHeader3Repository(appDatabase._dbHelper);
            ParagraphContentRepository = new ParagraphContentRepository(appDatabase._dbHelper);
            ParagraphImageRepository = new ParagraphImageRepository(appDatabase._dbHelper);
            WikiPictureRepository = new WikiPictureRepository(appDatabase._dbHelper);
            PhoneSettingsRepository = new PhoneSettingsRepository(appDatabase._dbHelper);
            TagRepository = new TagRepository(appDatabase._dbHelper);
            TagItemRepository = new TagItemRepository(appDatabase._dbHelper);
            AppMenuItemRepository = new AppMenuItemRepository(appDatabase._dbHelper);
            RequestRecordRepository = new RequestRecordRepository(appDatabase._dbHelper);

            repoExtensions.Add(MasterRepository);
            repoExtensions.Add(MetadataRepository);
            repoExtensions.Add(ParagraphPrimaryContentRepository);
            repoExtensions.Add(ParagraphHeader2Repository);
            repoExtensions.Add(ParagraphHeader3Repository);
            repoExtensions.Add(ParagraphContentRepository);
            repoExtensions.Add(ParagraphImageRepository);
            repoExtensions.Add(WikiPictureRepository);
            repoExtensions.Add(PhoneSettingsRepository);
            repoExtensions.Add(TagRepository);
            repoExtensions.Add(TagItemRepository);
            repoExtensions.Add(AppMenuItemRepository);
            repoExtensions.Add(RequestRecordRepository);
            repoExtensions.Add(SettingsRepository);

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
