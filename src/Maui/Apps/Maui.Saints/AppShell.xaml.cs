using System.Diagnostics;
using WikiExtractor.Maui.App.Views;

namespace Maui.Saints
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
