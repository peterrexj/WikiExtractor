using Pj.Library.Mobile.Sqlite;
using WikiExtractor.Maui.App.Services;
using WikiExtractor.Repository;

namespace GeneralInformation.Repository
{
    public class AppDatabase : AppSqliteRepository, IWikiDatabase
    {
        public AppDatabase() : base(Pj.Library.DatabaseHelper.DatabaseType.SqLiteDevice,
             GetLocalStorageHelper())
        {
            InitializeDatabase();
        }

        private static Pj.Library.Mobile.DeviceDependency.ISqlitHelper GetLocalStorageHelper()
        {
            try
            {
                var localStorage = CustomServices.LocalStorage;
                if (localStorage?.SqlLiteHelper != null)
                {
                    return localStorage.SqlLiteHelper;
                }
                
                // Fallback: try to get from ServiceLocator
                var fallbackStorage = ServiceLocator.GetService<ILocalStorage>();
                if (fallbackStorage?.SqlLiteHelper != null)
                {
                    return fallbackStorage.SqlLiteHelper;
                }
                
                throw new InvalidOperationException("LocalStorage service is not available during AppDatabase initialization");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to initialize AppDatabase: {ex.Message}", ex);
            }
        }
        public MasterRepository MasterRepository { get; set; }
        public MetadataRepository MetadataRepository { get; set; }
        public ParagraphPrimaryContentRepository ParagraphPrimaryContentRepository { get; set; }
        public ParagraphHeader2Repository ParagraphHeader2Repository { get; set; }
        public ParagraphHeader3Repository ParagraphHeader3Repository { get; set; }
        public ParagraphContentRepository ParagraphContentRepository { get; set; }
        public ParagraphImageRepository ParagraphImageRepository { get; set; }
        public WikiPictureRepository WikiPictureRepository { get; set; }
        public PhoneSettingsRepository PhoneSettingsRepository { get; set; }
        public TagRepository TagRepository { get; set; }
        public TagItemRepository TagItemRepository { get; set; }
        public AppMenuItemRepository AppMenuItemRepository { get; set; }
        public QuizMasterMetadataRepository QuizMasterMetadataRepository { get; set; }
        public QuizDefinitionRepository QuizDefinitionRepository { get; set; }

        public override void InitializeDatabase()
        {
            MasterRepository = new MasterRepository(_dbHelper);
            MetadataRepository = new MetadataRepository(_dbHelper);
            ParagraphPrimaryContentRepository = new ParagraphPrimaryContentRepository(_dbHelper);
            ParagraphHeader2Repository = new ParagraphHeader2Repository(_dbHelper);
            ParagraphHeader3Repository = new ParagraphHeader3Repository(_dbHelper);
            ParagraphContentRepository = new ParagraphContentRepository(_dbHelper);
            ParagraphImageRepository = new ParagraphImageRepository(_dbHelper);
            WikiPictureRepository = new WikiPictureRepository(_dbHelper);
            PhoneSettingsRepository = new PhoneSettingsRepository(_dbHelper);
            TagItemRepository = new TagItemRepository(_dbHelper);
            TagRepository = new TagRepository(_dbHelper);
            AppMenuItemRepository = new AppMenuItemRepository(_dbHelper);
            QuizMasterMetadataRepository = new QuizMasterMetadataRepository(_dbHelper);
            QuizDefinitionRepository = new QuizDefinitionRepository(_dbHelper);

            base.CollectRepository(MasterRepository, MetadataRepository, ParagraphPrimaryContentRepository,
                ParagraphHeader2Repository, ParagraphHeader3Repository, ParagraphContentRepository,
                ParagraphImageRepository,
                WikiPictureRepository, PhoneSettingsRepository, TagItemRepository, TagRepository,
                AppMenuItemRepository,
                QuizMasterMetadataRepository, QuizDefinitionRepository
                );

            base.InitializeDatabase();
        }
    }
}
