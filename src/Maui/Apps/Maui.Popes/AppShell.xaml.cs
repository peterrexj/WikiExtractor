using System.Diagnostics;
using WikiExtractor.Maui.App.Repository;
using WikiExtractor.Process;
using WikiExtractor.Maui.App.Views;

namespace Maui.Wiki
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            
            
            // Register the WikiListOfItemsPage for navigation
            Routing.RegisterRoute(nameof(WikiExtractor.Maui.App.Views.WikiListOfItemsPage), typeof(WikiExtractor.Maui.App.Views.WikiListOfItemsPage));
            Routing.RegisterRoute(nameof(QuizPage), typeof(QuizPage));
            Routing.RegisterRoute(nameof(PersonaDetailPage), typeof(PersonaDetailPage));
            Routing.RegisterRoute("QuizResultsPage", typeof(WikiExtractor.Maui.App.Views.QuizResultsPage));
            // Register the SettingsPage for navigation
            Routing.RegisterRoute("settings", typeof(SettingsPage));

            //// Load flyout menu items after initialization
            //Loaded += OnShellLoaded;

            _ = LoadDynamicMenuAsync();

            // Handle navigation to set the Tag property on WikiListOfItemsPage
            Navigating += OnNavigating;
        }
        
        private async void OnShellLoaded(object sender, EventArgs e)
        {
            try
            {
                // Remove the event handler to prevent multiple executions
                Loaded -= OnShellLoaded;
                
                // Ensure database is accessible
                if (DatabaseService.AppDatabase == null)
                {
                    Debug.WriteLine("Warning: AppDatabase is null when loading flyout menu");
                    return;
                }
                
                // Load categories from database
                await LoadFlyoutMenuItems();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading flyout menu: {ex.Message}");
                WikiExtractor.Maui.App.Exts.ExceptionHandler.CaptureException(ex, "AppShell.OnShellLoaded");
            }
        }

        private async Task LoadDynamicMenuAsync()
        {
            try
            {
                // Wait a tiny bit to ensure DB services are initialized if they are lazy
                await Task.Delay(100);

                if (DatabaseService.AppDatabase == null) return;

                var wikiAppController = new WikiAppController(DatabaseService.AppDatabase, DatabaseService.UserStoreDatabase);
                var flyoutItems = wikiAppController.AppMenuItems().ToList();

                if (flyoutItems.Count == 0) return;

                // Optional: Remove the placeholder if you want to start fresh
                // Items.Clear(); 

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
                                ContentTemplate = new DataTemplate(typeof(WikiExtractor.Maui.App.Views.WikiListOfItemsPage))
                            }
                        }
                    });

                    // Add settings after the first item
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
                    }
                }

                // Navigate to the first real item so the user leaves the "Loading" placeholder
                await Shell.Current.GoToAsync($"//{flyoutItems[0].Tags}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Menu Load Error: {ex.Message}");
            }
        }

        private async Task LoadFlyoutMenuItems()
        {
            try
            {
                Debug.WriteLine("Loading flyout menu items from database...");
                
                // Get categories from database
                var wikiAppController = new WikiAppController(DatabaseService.AppDatabase, DatabaseService.UserStoreDatabase);
                var flyoutItems = wikiAppController.AppMenuItems().ToList();

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
                                ContentTemplate = new DataTemplate(typeof(WikiExtractor.Maui.App.Views.WikiListOfItemsPage))
                            }
                        }
                    });

                    // Add settings after the first item
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
                    }
                }
                
                // // Create flyout items for each category
                // foreach (var category in categories)
                // {
                //     var flyoutItem = new FlyoutItem
                //     {
                //         Title = category.Name,
                //         Route = $"category_{category.Id}",
                //         FlyoutIcon = "icon_category.png"
                //     };
                //
                //     // Create ShellContent for this category
                //     var shellContent = new ShellContent
                //     {
                //         Title = category.Name,
                //         Route = $"category_{category.Id}",
                //         ContentTemplate = new DataTemplate(() =>
                //         {
                //             var page = new MainPage();
                //             // We'll load the specific category data when the page appears
                //             return page;
                //         })
                //     };
                //
                //     flyoutItem.Items.Add(shellContent);
                //     Items.Add(flyoutItem);
                // }

                Debug.WriteLine("Flyout menu items loaded successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading flyout menu items: {ex.Message}");
                WikiExtractor.Maui.App.Exts.ExceptionHandler.CaptureException(ex, "AppShell.LoadFlyoutMenuItems");
            }
        }
        
        private void OnNavigating(object sender, ShellNavigatingEventArgs args)
        {
            try
            {
                // Set the Tag property on WikiListOfItemsPage when navigating to it
                if (args.Target.Location.OriginalString.StartsWith("//"))
                {
                    string route = args.Target.Location.OriginalString.Replace("//", "");
                    WikiExtractor.Maui.App.Views.WikiListOfItemsPage.Tag = route;
                    Debug.WriteLine($"Setting WikiListOfItemsPage.Tag to: {route}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in OnNavigating: {ex.Message}");
                WikiExtractor.Maui.App.Exts.ExceptionHandler.CaptureException(ex, "AppShell.OnNavigating");
            }
        }
    }
}
