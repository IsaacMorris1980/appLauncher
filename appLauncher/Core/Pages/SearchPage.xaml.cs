using appLauncher.Core.Model;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using appLauncher.Core;
using appLauncher.Core.Helpers;

using System;
using System.Linq;

using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using appLauncher.Core.Control;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace appLauncher.Core.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchPage : Page
    {
        //  public ReadOnlyObservableCollection<finalAppItem> queriedApps = new ReadOnlyObservableCollection<finalAppItem>(AllApps.listOfApps);
        public SearchPage()
        {
            
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += SearchPage_BackRequested;
            DesktopBackButton.ShowBackButton();
            QueriedAppsListView.ItemsSource = packageHelper.Bags.OrderBy(x => x.AppName);
            Analytics.TrackEvent("Search Page Loaded");
            
        }

        private void SearchPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            DesktopBackButton.HideBackButton();
            e.Handled = true;
            Frame.Navigate(typeof(MainPage));
        }

        private void useMeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = useMeTextBox.Text.ToLower();
            if (!String.IsNullOrEmpty(query))
            {
                QueriedAppsListView.ItemsSource = packageHelper.Bags.Where(p => p.AppName.ToLower().Contains(query));

            }
            else
            {
                QueriedAppsListView.ItemsSource = packageHelper.Bags.OrderBy(x => x.AppName);
            }

        }
        private async void QueriedAppsListView_ItemClick(object sender, ItemClickEventArgs e)
        {

            var a = ((AppTile)e.ClickedItem);
            try
            {
                await a.AppListentry.LaunchAsync();
                Analytics.TrackEvent("App Launched From Search Page");
            }
            catch (Exception f)
            {
                Crashes.TrackError(f);
                Analytics.TrackEvent("App Failed to Launch from search page");
            }

        }


    }
}
