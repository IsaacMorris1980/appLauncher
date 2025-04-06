using appLauncher.Core.Model;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace appLauncher.Core.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FolderInfo : Page
    {
        public FolderInfo()
        {
            this.InitializeComponent();
        }
        private AppFolder _folder;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _folder = e.Content as AppFolder;
        }

        private void AppBarButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {

        }
    }
}
