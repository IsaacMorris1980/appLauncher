using appLauncher.Core.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace appLauncher.Core.Pages
{
    /// <summary>
    /// Code-behind for the AppLoading page.
    /// Delegates initialization logic to the <see cref="AppLoadingViewModel"/>.
    /// </summary>
    public sealed partial class AppLoading : Page
    {
        public AppLoadingViewModel ViewModel { get; set; }

        public AppLoading()
        {
            // Resolve the ViewModel using Microsoft.Extensions.DependencyInjection
            // This assumes App.ServiceProvider is configured in App.xaml.cs
            ViewModel = App.ServiceLocator.Resolve<AppLoadingViewModel>();
            this.DataContext = ViewModel;

            // This line initializes the UI components defined in AppLoading.xaml.
            // Errors here typically indicate a problem in the XAML markup itself.
            this.InitializeComponent();

            // Trigger the loading process when the page is loaded
            this.Loaded += AppLoading_Loaded;
        }

        /// <summary>
        /// Handles the Loaded event of the page, triggering the ViewModel's loading command.
        /// </summary>
        private void AppLoading_Loaded(object sender, RoutedEventArgs e)
        {
            // Execute the async command without awaiting here.
            // The command itself handles its asynchronous execution and updates the UI via bindings.
            ViewModel.InitializeLoadingCommand.Execute(null);
        }
    }
}
