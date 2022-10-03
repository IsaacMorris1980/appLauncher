using appLauncher.Core.Helpers;
using appLauncher.Core.Model;
using appLauncher.Pages;

using System;
using System.Collections.ObjectModel;
using System.Linq;

using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace appLauncher.Core.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchPage : Page
    {
        public ReadOnlyObservableCollection<AppTile> queriedApps = new ReadOnlyObservableCollection<AppTile>(packageHelper.appTiles);
        public SearchPage()
        {
            this.InitializeComponent();
            //    SystemNavigationManager.GetForCurrentView().BackRequested += SearchPage_BackRequested;
            QueriedAppsListView.ItemsSource = queriedApps;
        }

        //private void SearchPage_BackRequested(object sender, BackRequestedEventArgs e)
        //{

        //    e.Handled = true;
        //    Frame.Navigate(typeof(MainPage));
        //}

        private void useMeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = useMeTextBox.Text.ToLower();
            if (!String.IsNullOrEmpty(query))
            {
                QueriedAppsListView.ItemsSource = queriedApps.Where(p => p._applistentry.DisplayInfo.DisplayName.ToLower().Contains(query));

            }
            else
            {
                QueriedAppsListView.ItemsSource = queriedApps;
            }

        }
        private async void QueriedAppsListView_ItemClick(object sender, ItemClickEventArgs e)
        {

            await ((AppTile)e.ClickedItem).LaunchAsync();


        }



        private void SettingsPage_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(settings));
        }

        private void MainPage_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
