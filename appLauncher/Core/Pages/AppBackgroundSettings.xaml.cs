using appLauncher.Core.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using appLauncher.Core.Services; // For ServiceLocator
using converters = appLauncher.Core.Converters; // Ensure this is imported if BooleanToVisibilityConverter is here

namespace appLauncher.Core.Pages
{
    /// <summary>
    /// Code-behind for the AppBackgroundSettings page.
    /// Delegates logic to the <see cref="AppBackgroundSettingsViewModel"/>.
    /// </summary>
    public sealed partial class AppBackgroundSettings : Page
    {
        public AppBackgroundSettingsViewModel ViewModel { get; set; }

        public AppBackgroundSettings()
        {
            // Resolve the ViewModel using your manual ServiceLocator
            ViewModel = App.ServiceLocator.Resolve<AppBackgroundSettingsViewModel>();
            this.DataContext = ViewModel;

            this.InitializeComponent();

            // Converters are typically defined in App.xaml or a shared resource dictionary.
            // If you still need them here for some reason, ensure they are in a separate file
            // and referenced with xmlns:converters="using:appLauncher.Core.Converters".
            // Example:
            if (!this.Resources.ContainsKey("BooleanToVisibilityConverter"))
            {
                this.Resources.Add("BooleanToVisibilityConverter", new converters.BooleanToVisibilityConverter());
            }
           if (!this.Resources.ContainsKey("InverseBooleanToVisibilityConverter"))
           {
                    this.Resources.Add("InverseBooleanToVisibilityConverter", new BooleanToVisibilityConverter { IsInverse = true });
           }
         

        }

        // All original Tapped event handlers (AddButton_TappedAsync, RemoveButton_Tapped) are removed.
        // Their logic is now handled by commands in the ViewModel.
    }
}
