using Pj.Library.Mobile.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;
using WikiExtractor.Repository;

namespace GeneralInformation.Repository
{
    public class AppDatabase : AppSqliteRepository, IWikiDatabase
    {
        public AppDatabase() : base()
        {
            InitializeDatabase();
        }
        public MasterRepository MasterRepository { get; set; }
        public MetadataRepository MetadataRepository { get; set; }
        public ParagraphPrimaryContentRepository ParagraphPrimaryContentRepository { get; set; }
        public ParagraphHeader2Repository ParagraphHeader2Repository { get; set; }
        public ParagraphHeader3Repository ParagraphHeader3Repository { get; set; }
        public ParagraphContentRepository ParagraphContentRepository { get; set; }
        public WikiPictureRepository WikiPictureRepository { get; set; }
        public PhoneSettingsRepository PhoneSettingsRepository { get; set; }
        public TagRepository TagRepository { get; set; }
        public TagItemRepository TagItemRepository { get; set; }
        public AppMenuItemRepository AppMenuItemRepository { get; set; }
        public RequestRecordRepository RequestRecordRepository { get; set; }

        public override void InitializeDatabase()
        {
            MasterRepository = new MasterRepository(_dbHelper);
            MetadataRepository = new MetadataRepository(_dbHelper);
            ParagraphPrimaryContentRepository = new ParagraphPrimaryContentRepository(_dbHelper);
            ParagraphHeader2Repository = new ParagraphHeader2Repository(_dbHelper);
            ParagraphHeader3Repository = new ParagraphHeader3Repository(_dbHelper);
            ParagraphContentRepository = new ParagraphContentRepository(_dbHelper);
            WikiPictureRepository = new WikiPictureRepository(_dbHelper);
            PhoneSettingsRepository = new PhoneSettingsRepository(_dbHelper);
            TagItemRepository = new TagItemRepository(_dbHelper);
            TagRepository = new TagRepository(_dbHelper);
            AppMenuItemRepository= new AppMenuItemRepository(_dbHelper);
            RequestRecordRepository = new RequestRecordRepository(_dbHelper);

            base.CollectRepository(MasterRepository, MetadataRepository, ParagraphPrimaryContentRepository,
                ParagraphHeader2Repository, ParagraphHeader3Repository, ParagraphContentRepository,
                WikiPictureRepository, PhoneSettingsRepository, TagItemRepository, TagRepository,
                AppMenuItemRepository, RequestRecordRepository);

            base.InitializeDatabase();
        }
    }
}
