using System.Diagnostics;
using WikiExtractor.Maui.App.Views;

namespace Maui.WorldLeaders
{
    public partial class AppShell : AppShellBase
    {
        public AppShell()
        {
            try
            {
                InitializeComponent();
                InitializeShell();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AppShell] EXCEPTION: {ex.Message}");
                throw;
            }
        }
    }
}
