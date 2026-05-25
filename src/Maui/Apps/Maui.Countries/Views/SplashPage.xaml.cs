using WikiExtractor.Maui.App.Repository;
using WikiExtractor.Maui.App.Services;
using WikiExtractor.Process;

namespace Maui.Countries.Views;

public partial class SplashPage : ContentPage
{
    private readonly IThemeHandler _themeHandler;
    private CancellationTokenSource _factCts = new();

    public SplashPage(IThemeHandler themeHandler)
    {
        _themeHandler = themeHandler;
        InitializeComponent();
        BackgroundColor = Color.FromArgb(AppSettingsService.GetThemeBackgroundColor());
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await LogoImage.FadeTo(1, 600);
        await TitleStack.FadeTo(1, 400);
        await FactStack.FadeTo(1, 300);

        _ = RotateFactsAsync(_factCts.Token);

        await BuildAndTransitionAsync();
    }

    private async Task RotateFactsAsync(CancellationToken ct)
    {
        await FactCacheService.Instance.WaitForInitializationAsync(3000);

        while (!ct.IsCancellationRequested)
        {
            try
            {
                var facts = FactCacheService.Instance.GetFacts(1);
                if (facts.Count > 0)
                {
                    await FactLabel.FadeTo(0, 250);
                    FactLabel.Text = facts[0].FactText;
                    await FactLabel.FadeTo(1, 350);
                }
                await Task.Delay(2800, ct);
            }
            catch (TaskCanceledException) { break; }
            catch { /* swallow — non-critical */ }
        }
    }

    private async Task BuildAndTransitionAsync()
    {
        AppShell? shell = null;

        try
        {
            shell = new AppShell();

            var preloadTask = Task.Run(async () =>
            {
                try
                {
                    await shell.WaitForMenuLoadedAsync();
                    var firstMenuItem = SharedServices.WikiAppController
                        .AppMenuItems().FirstOrDefault();
                    if (firstMenuItem != null)
                    {
                        var tags = firstMenuItem.Tags?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(t => t.Trim()).ToList() ?? new List<string>();
                        var personas = SharedServices.WikiAppController
                            .GetListOfWikiItems(tags.Count > 0 ? tags : null).ToList();
                        var tagKey = string.Join(",", tags);
                        SharedServices.StorePreloadedPersonas(tagKey, personas);
                        if (tagKey.Length > 0)
                            SharedServices.StorePreloadedPersonas("", personas);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[SplashPage] Preload error: {ex.Message}");
                }
            });

            await Task.WhenAll(
                preloadTask,
                Task.Delay(400)
            );
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[SplashPage] Shell build error: {ex.Message}");
        }

        _factCts.Cancel();

        if (shell == null) return;

        shell.Opacity = 0;
        Application.Current!.Windows[0].Page = shell;
        await shell.FadeTo(1, 350);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _factCts.Cancel();
    }
}
