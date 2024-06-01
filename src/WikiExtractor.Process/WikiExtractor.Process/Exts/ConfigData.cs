namespace WikiExtractor.Exts;

public static class ConfigData
{
    public static int MinLengthOfPictureCaption { get; set; } = 6;
    public const int MinHeightOfListItemInListPage = 120;
    public static string LocalStorageCacheFolderPath { get; set; }
}
