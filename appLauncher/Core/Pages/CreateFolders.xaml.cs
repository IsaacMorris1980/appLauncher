using appLauncher.Core.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using appLauncher.Core.Services; // For ServiceLocator
// Removed: using System; // Tuple is no longer directly used here, only in ViewModel for navigation parameter

namespace appLauncher.Core.Pages
{
    /// <summary>
    /// Code-behind for the CreateFolders page.
    /// Delegates logic to the <see cref="CreateFoldersViewModel"/>.
    /// </summary>
    public sealed partial class CreateFolders : Page
    {
        public CreateFoldersViewModel ViewModel { get; set; }

        public CreateFolders()
        {
            // Resolve the ViewModel using your manual ServiceLocator
            ViewModel = App.ServiceLocator.Resolve<CreateFoldersViewModel>();
            this.DataContext = ViewModel;

            this.InitializeComponent();

            // Removed: Programmatic addition of converters. They are now defined in XAML.
            // if (!this.Resources.ContainsKey("BooleanToVisibilityConverter"))
            // {
            //     this.Resources.Add("BooleanToVisibilityConverter", new BooleanToVisibilityConverter());
            // }
            // if (!this.Resources.ContainsKey("InverseBooleanToVisibilityConverter"))
            // {
            //     this.Resources.Add("InverseBooleanToVisibilityConverter", new BooleanToVisibilityConverter { IsInverse = true });
            // }

            // Trigger ViewModel's initialization logic on page load
            this.Loaded += Page_Loaded;
        }

        /// <summary>
        /// Handles the Loaded event of the page, triggering ViewModel initialization.
        /// </summary>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.InitializeCreationCommand.Execute(null);
        }

        // All other event handlers (AppsinFolders_SelectionChanged, AllTiles_SelectionChanged,
        // addApp_Tapped, removeApp_Tapped, SaveButton_Tapped) are now replaced by XAML
        // Command bindings or TwoWay property bindings to the ViewModel.
        // The x:Load="False" attributes have been replaced with Visibility bindings.

        // Removed: Nested BooleanToVisibilityConverter class. It's now in its own file.
    }
}
