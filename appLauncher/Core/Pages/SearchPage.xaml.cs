using applauncher.mobile.Core.Model;

using appLauncher.mobile.Core;
using appLauncher.mobile.Core.Helpers;

using System;
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
        //  public ReadOnlyObservableCollection<finalAppItem> queriedApps = new ReadOnlyObservableCollection<finalAppItem>(AllApps.listOfApps);
        public SearchPage()
        {
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += SearchPage_BackRequested;
            DesktopBackButton.ShowBackButton();
            QueriedAppsListView.ItemsSource = GlobalVariables.queriedApps.OrderBy(x => x.AppName);
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
                QueriedAppsListView.ItemsSource = GlobalVariables.queriedApps.Where(p => p.AppName.ToLower().Contains(query));

            }
            else
            {
                QueriedAppsListView.ItemsSource = GlobalVariables.queriedApps.OrderBy(x => x.AppName);
            }

        }
        private async void QueriedAppsListView_ItemClick(object sender, ItemClickEventArgs e)
        {

            await ((AppTile)e.ClickedItem).LaunchAsync();


        }


    }
}
