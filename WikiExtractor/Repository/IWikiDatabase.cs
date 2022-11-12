using Pj.Library.Mobile.Model;

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
        WikiPictureRepository WikiPictureRepository { get; set; }

        void InitializeDatabase();
    }
}