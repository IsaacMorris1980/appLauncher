using appLauncher.Core.Helpers;

using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace appLauncher.Core.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppLoading : Page
    {
        public AppLoading()
        {
            this.InitializeComponent();
            PackageHelper.AppsRetreived += PackageHelper_AppsRetreived;
            ImageHelper.ImagesRetreived += ImageHelper_ImagesRetreived;
        }

        private void ImageHelper_ImagesRetreived(object sender, EventArgs e)
        {

            FirstPage.navFrame.Navigate(typeof(MainPage), PackageHelper.AllApps);
            FirstPage.navFrame.BackStack.RemoveAt(0);
        }

        private async void PackageHelper_AppsRetreived(object sender, EventArgs e)
        {

            await ImageHelper.LoadBackgroundImages();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await PackageHelper.LoadCollectionAsync();
        }
    }
}
