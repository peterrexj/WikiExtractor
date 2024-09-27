using WikiExtractor.Process.Repository;

namespace WikiExtractor.Repository
{
    public interface IWikiDatabase
    {
        MasterRepository MasterRepository { get; set; }
        MetadataRepository MetadataRepository { get; set; }
        ParagraphContentRepository ParagraphContentRepository { get; set; }
        ParagraphHeader2Repository ParagraphHeader2Repository { get; set; }
        ParagraphHeader3Repository ParagraphHeader3Repository { get; set; }
        ParagraphPrimaryContentRepository ParagraphPrimaryContentRepository { get; set; }
        ParagraphImageRepository ParagraphImageRepository { get; set; }
        WikiPictureRepository WikiPictureRepository { get; set; }
        PhoneSettingsRepository PhoneSettingsRepository { get; set; }
        TagRepository TagRepository { get; set; }
        TagItemRepository TagItemRepository { get; set; }
        AppMenuItemRepository AppMenuItemRepository { get; set; }
        QuizMasterMetadataRepository QuizMasterMetadataRepository { get; set; }
        QuizDefinitionRepository QuizDefinitionRepository { get; set; }

        void InitializeDatabase();
    }
}