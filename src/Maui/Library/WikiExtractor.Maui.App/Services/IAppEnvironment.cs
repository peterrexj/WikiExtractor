namespace WikiExtractor.Maui.App.Services
{
    public interface IAppEnvironment
    {
        void SetStatusBarColor(Color color, bool darkStatusBarTint);

        bool DisplayAds { get; }
    }
}