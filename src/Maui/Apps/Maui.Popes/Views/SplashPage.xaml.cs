using WikiExtractor.Maui.App.Repository;
using WikiExtractor.Maui.App.Services;
using WikiExtractor.Process;

namespace Maui.Wiki.Views;

public partial class SplashPage : ContentPage
{
    private readonly IThemeHandler _themeHandler;
    private CancellationTokenSource _factCts = new();

    public SplashPage(IThemeHandler themeHandler)
    {
        _themeHandler = themeHandler;
        InitializeComponent();
        // Apply the saved theme background synchronously so the page never
        // flashes a wrong color before DynamicResources propagate.
        BackgroundColor = Color.FromArgb(AppSettingsService.GetThemeBackgroundColor());
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Fade-in the logo, then the title, then the fact area
        await LogoImage.FadeTo(1, 600);
        await TitleStack.FadeTo(1, 400);
        await FactStack.FadeTo(1, 300);

        // Rotate facts while we load
        _ = RotateFactsAsync(_factCts.Token);

        // Build the shell (loads DB menu) then swap
        await BuildAndTransitionAsync();
    }

    private async Task RotateFactsAsync(CancellationToken ct)
    {
        // Wait until cache has at least something
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
            // AppShell must be constructed on the main thread (iOS UIKit requirement)
            shell = new AppShell();

            // Pre-load the first menu's list data in the background while the shell loads its menu
            var preloadTask = Task.Run(async () =>
            {
                try
                {
                    await shell.WaitForMenuLoadedAsync();
                    var firstMenuItem = WikiExtractor.Maui.App.Services.SharedServices.WikiAppController
                        .AppMenuItems().FirstOrDefault();
                    if (firstMenuItem != null)
                    {
                        var tags = firstMenuItem.Tags?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(t => t.Trim()).ToList() ?? new List<string>();
                        var personas = WikiExtractor.Maui.App.Services.SharedServices.WikiAppController
                            .GetListOfWikiItems(tags.Count > 0 ? tags : null).ToList();
                        var tagKey = string.Join(",", tags);
                        // Store under the actual tag key and also empty string (first-page default)
                        WikiExtractor.Maui.App.Services.SharedServices.StorePreloadedPersonas(tagKey, personas);
                        if (tagKey.Length > 0)
                            WikiExtractor.Maui.App.Services.SharedServices.StorePreloadedPersonas("", personas);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[SplashPage] Preload error: {ex.Message}");
                }
            });

            // Wait for menu + minimum display time in parallel with preload
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

        // Set shell invisible, swap the page, then fade it in — this way the
        // dark window background stays covered throughout and no white flash appears.
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
