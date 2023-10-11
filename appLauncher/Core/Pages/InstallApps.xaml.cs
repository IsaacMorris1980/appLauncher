using appLauncher.Core.Helpers;
using appLauncher.Core.Model;

using Windows.Management.Deployment;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace appLauncher.Core.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InstallApps : Page
    {

        private string results;
        public InstallApps()
        {
            this.InitializeComponent();
        }
        private async void Deps_Tapped(object sender, TappedRoutedEventArgs e)
        {

            results = await InstallorRemoveApplication.LoadDependancies();
            if (results != "Success")
            {
                DepsInstalled.IsChecked = false;
            }
            DepsInstalled.IsChecked = true;
            DepsInstalled.Content = "Deps Installed";
        }

        private async void Certs_Tapped(object sender, TappedRoutedEventArgs e)
        {
            results = await InstallorRemoveApplication.InstallCertificate();
            if (results != "Success")
            {
                CertisInstalled.IsChecked = false;
            }
            CertisInstalled.IsChecked = true;
            CertisInstalled.Content = "Cert is installed";
        }
        private async void Install_Tapped(object sender, TappedRoutedEventArgs e)
        {
            results = await InstallorRemoveApplication.InstallApplication();
            ErrororSuccess.Text = results;
        }

        private void ReturnHome_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private async void Remove_Tapped(object sender, TappedRoutedEventArgs e)
        {
            PackageManager pm = new PackageManager();
            FinalTiles tiles = (FinalTiles)listofapps.SelectedItem;

            results = await InstallorRemoveApplication.RemoveApplication(tiles.FullName);
        }

        private void MainPage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
