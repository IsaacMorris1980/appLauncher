using appLauncher.Core;
using appLauncher.Model;

using Microsoft.AppCenter.Analytics;

using System;
using System.Collections.ObjectModel;
using System.Linq;

using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace appLauncher
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchPage : Page
    {
        public ReadOnlyObservableCollection<finalAppItem> queriedApps = new ReadOnlyObservableCollection<finalAppItem>(AllApps.listOfApps);
        public SearchPage()
        {
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += SearchPage_BackRequested;
            DesktopBackButton.ShowBackButton();
            QueriedAppsListView.ItemsSource = queriedApps;
            Analytics.TrackEvent("Search page is loading");
        }

        private void SearchPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            DesktopBackButton.HideBackButton();
            e.Handled = true;
            Analytics.TrackEvent("Navigating back from search page to main page");
            Frame.Navigate(typeof(MainPage));
        }

        private void useMeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = useMeTextBox.Text.ToLower();
            if (!String.IsNullOrEmpty(query))
            {
                QueriedAppsListView.ItemsSource = queriedApps.Where(p => p.appEntry.DisplayInfo.DisplayName.ToLower().Contains(query));

            }
            else
            {
                QueriedAppsListView.ItemsSource = queriedApps;
            }

        }
        private async void QueriedAppsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Analytics.TrackEvent($"App has been found and is attempted to be launched from search page");
            await ((finalAppItem)e.ClickedItem).appEntry.LaunchAsync();


        }


    }
}
