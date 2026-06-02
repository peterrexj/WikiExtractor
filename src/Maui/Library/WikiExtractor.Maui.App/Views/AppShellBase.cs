using System.Diagnostics;
using WikiExtractor.Maui.App.Exts;
using WikiExtractor.Maui.App.Repository;
using WikiExtractor.Process;

namespace WikiExtractor.Maui.App.Views
{
    public abstract class AppShellBase : Shell
    {
        private readonly TaskCompletionSource _menuLoaded = new();

        public Task WaitForMenuLoadedAsync() => Task.WhenAny(_menuLoaded.Task, Task.Delay(10000));

        protected void InitializeShell()
        {
            Routing.RegisterRoute(nameof(WikiListOfItemsPage), typeof(WikiListOfItemsPage));
            Routing.RegisterRoute(nameof(QuizPage), typeof(QuizPage));
            Routing.RegisterRoute(nameof(PersonaDetailPage), typeof(PersonaDetailPage));
            Routing.RegisterRoute("QuizResultsPage", typeof(QuizResultsPage));
            Routing.RegisterRoute("settings", typeof(SettingsPage));
            Routing.RegisterRoute("stats", typeof(StatsPage));

            _ = LoadDynamicMenuAsync();

            Navigating += OnNavigating;
        }

        private async Task LoadDynamicMenuAsync()
        {
            try
            {
                if (DatabaseService.AppDatabase == null) return;

                var wikiAppController = new WikiAppController(DatabaseService.AppDatabase, DatabaseService.UserStoreDatabase);

                var flyoutItems = await Task.Run(() => wikiAppController.AppMenuItems().ToList());

                if (flyoutItems.Count == 0) return;

                for (int i = 0; i < flyoutItems.Count; i++)
                {
                    var flyItem = flyoutItems[i];
                    Items.Add(new FlyoutItem
                    {
                        Title = flyItem.MenuItemName,
                        AutomationId = flyItem.Tags,
                        Items = {
                            new ShellContent
                            {
                                Title = flyItem.MenuItemName,
                                Route = flyItem.Tags,
                                ContentTemplate = new DataTemplate(typeof(WikiListOfItemsPage))
                            }
                        }
                    });

                    if (i == 0)
                    {
                        Items.Add(new FlyoutItem
                        {
                            Title = "Settings",
                            Route = "settings",
                            Items = {
                                new ShellContent
                                {
                                    Title = "Settings",
                                    ContentTemplate = new DataTemplate(typeof(SettingsPage))
                                }
                            }
                        });
                        Items.Add(new FlyoutItem
                        {
                            Title = "My Progress",
                            Route = "stats",
                            Items = {
                                new ShellContent
                                {
                                    Title = "My Progress",
                                    ContentTemplate = new DataTemplate(typeof(StatsPage))
                                }
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AppShell] Menu Load Error: {ex.Message}");
            }
            finally
            {
                _menuLoaded.TrySetResult();
            }
        }

        private void OnNavigating(object sender, ShellNavigatingEventArgs args)
        {
            try
            {
                if (args.Target.Location.OriginalString.StartsWith("//"))
                {
                    string route = args.Target.Location.OriginalString.Replace("//", "");
                    WikiListOfItemsPage.Tag = route;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AppShell] OnNavigating error: {ex.Message}");
                ExceptionHandler.CaptureException(ex, "AppShell.OnNavigating");
            }
        }
    }
}
