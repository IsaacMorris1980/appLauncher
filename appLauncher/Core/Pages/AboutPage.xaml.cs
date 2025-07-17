using appLauncher.Core.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation; // For OnNavigatedTo

namespace appLauncher.Core.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        public AboutViewModel ViewModel { get; set; }

        public AboutPage()
        {
            // Resolve the ViewModel using the configured Dependency Injection container
            ViewModel = App.ServiceLocator.Resolve<AboutViewModel>();
            this.DataContext = ViewModel; // Set the DataContext for XAML binding

            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            // Any specific logic needed when navigating to this page can go here.
            // For example, if the ViewModel had a LoadDataCommand, you might call it here:
            // ViewModel.LoadDataCommand.Execute(null);
            // However, in this case, the ViewModel's constructor already initializes properties.
        }
    }
}
