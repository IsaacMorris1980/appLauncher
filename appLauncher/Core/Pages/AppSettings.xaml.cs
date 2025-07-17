using appLauncher.Core.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace appLauncher.Core.Pages
{
    /// <summary>
    /// Code-behind for the AppSettings page.
    /// Delegates logic to the <see cref="AppSettingsViewModel"/>.
    /// </summary>
    public sealed partial class AppSettings : Page
    {
        public AppSettingsViewModel ViewModel { get; set; }

        public AppSettings()
        {
            // Resolve the ViewModel using Dependency Injection
            // This assumes App.ServiceProvider is configured in App.xaml.cs
            ViewModel = App.ServiceLocator.Resolve<AppSettingsViewModel>();
            this.DataContext = ViewModel;

            this.InitializeComponent();

            // Converters (like BooleanToVisibilityConverter) should ideally be defined globally in App.xaml
            // or in a shared resource dictionary and referenced via xmlns.
            // If you still need to add them programmatically for this specific page, ensure their namespace
            // is correctly imported (e.g., using appLauncher.Core.Converters;).
        }

        // All original event handlers (ApplicationTextColor_SelectionChanged, ApplicationBackColor_SelectionChanged,
        // ApplicationTextOpacity_ValueChanged, ApplicationBackOpacity_ValueChanged, SaveSettings_Tapped) are removed.
        // Their logic is now handled by commands and property setters in the ViewModel.
    }
}
