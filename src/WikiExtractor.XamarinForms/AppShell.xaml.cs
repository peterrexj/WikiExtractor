using GeneralInformation.Repository;
using GeneralInformation.Views;
using System;
using WikiExtractor.Process;
using WikiExtractor.XamarinForms.Views;
using Xamarin.Forms;

namespace GeneralInformation
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();

            var wikiAppController = new WikiAppController(DatabaseService.AppDatabase, DatabaseService.UserStoreDatabase);
            var flyoutItems = wikiAppController.AppMenuItems();

            foreach (var flyItem in flyoutItems)
            {
                Items.Add(new FlyoutItem
                {
                    Title = flyItem.MenuItemName,
                    //Route = flyItem.Route,
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
            }

            Routing.RegisterRoute(nameof(WikiListOfItemsPage), typeof(WikiListOfItemsPage));
            Routing.RegisterRoute(nameof(PersonaDetailPage), typeof(PersonaDetailPage));
            Routing.RegisterRoute(nameof(QuizPage), typeof(QuizPage));
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//WikiListOfItemsPage");
        }

        protected override void OnNavigating(ShellNavigatingEventArgs args)
        {
            base.OnNavigating(args);
            WikiListOfItemsPage.Tag = args.Target.Location.OriginalString.Replace("//", "");
        }
    }
}
