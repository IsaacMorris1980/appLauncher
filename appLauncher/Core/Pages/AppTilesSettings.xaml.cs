using appLauncher.Core.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using appLauncher.Core.Converters; // Ensure this is imported if BooleanToVisibilityConverter is here

namespace appLauncher.Core.Pages
{
    /// <summary>
    /// Code-behind for the AppTilesSettings page.
    /// Delegates logic to the <see cref="AppTilesSettingsViewModel"/>.
    /// </summary>
    public sealed partial class AppTilesSettings : Page
    {
        public AppTilesSettingsViewModel ViewModel { get; set; }

        public AppTilesSettings()
        {
            // Resolve the ViewModel using Dependency Injection
            // This assumes App.ServiceProvider is configured in App.xaml.cs
            ViewModel = App.ServiceLocator.Resolve<AppTilesSettingsViewModel>();
            this.DataContext = ViewModel;

            this.InitializeComponent();

            // Converters (like BooleanToVisibilityConverter) should ideally be defined globally in App.xaml
            // or in a shared resource dictionary and referenced via xmlns.
            // If you still need to add them programmatically for this specific page, ensure their namespace
            // is correctly imported (e.g., using appLauncher.Core.Converters;).
            if (!this.Resources.ContainsKey("BooleanToVisibilityConverter"))
            {
                this.Resources.Add("BooleanToVisibilityConverter", new BooleanToVisibilityConverter());
            }
            if (!this.Resources.ContainsKey("InverseBooleanToVisibilityConverter"))
            {
                this.Resources.Add("InverseBooleanToVisibilityConverter", new BooleanToVisibilityConverter { IsInverse = true });
            }
        }

        // All original private fields (allapps, selectedapp, sectionofapp, etc.) are removed.
        // All original event handlers (TileLogoColor_SelectionChanged, LogoOpacity_ValueChanged, etc.) are removed.
        // Their logic is now handled by commands and property setters in the ViewModel.
    }
}
