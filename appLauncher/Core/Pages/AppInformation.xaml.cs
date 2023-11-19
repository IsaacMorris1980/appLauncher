using appLauncher.Core.Model;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace appLauncher.Core.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppInformation : Page

    {
        private FinalTiles _tiles;
        public AppInformation()
        {
            this.InitializeComponent();
        }

        private void Home_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void SettingsPage_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPage));
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _tiles = e.Parameter as FinalTiles;
        }

        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Favorited.IsChecked = _tiles.Favorite;
        }

        private void Favorited_Checked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }
    }
}
