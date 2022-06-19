using appLauncher.Core;
using appLauncher.Model;

using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

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
            try
            {
                this.InitializeComponent();
                SystemNavigationManager.GetForCurrentView().BackRequested += SearchPage_BackRequested;
                DesktopBackButton.ShowBackButton();
                QueriedAppsListView.ItemsSource = queriedApps;
                Analytics.TrackEvent("Search page is loading");
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

        }

        private void SearchPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            try
            {
                DesktopBackButton.HideBackButton();
                e.Handled = true;
                Analytics.TrackEvent("Navigating back from search page to main page");
                Frame.Navigate(typeof(MainPage));
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void useMeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Analytics.TrackEvent("User is starting or continuing to filter apps");
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
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
        private async void QueriedAppsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                Analytics.TrackEvent($"App has been found and is attempted to be launched from search page");
                await ((finalAppItem)e.ClickedItem).appEntry.LaunchAsync();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

        }


    }
}
