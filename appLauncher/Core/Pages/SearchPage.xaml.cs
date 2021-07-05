using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using appLauncher.Control;
using appLauncher.Model;
using System.Collections.ObjectModel;
using Windows.UI.Core;
using appLauncher.Core;
using appLauncher.mobile.Core;
using appLauncher.mobile.Core.Models;

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
            QueriedAppsListView.ItemsSource = AllApps.queriedApps.OrderBy(x=>x.AppName);
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
                QueriedAppsListView.ItemsSource = AllApps.queriedApps.Where(p => p.AppName.ToLower().Contains(query));

            }
            else
            {
                QueriedAppsListView.ItemsSource = AllApps.queriedApps.OrderBy(x => x.AppName);
            }
           
        }
        private async void QueriedAppsListView_ItemClick(object sender, ItemClickEventArgs e)
        {

            await ((finalAppItem)e.ClickedItem).LaunchAsync() ;


        }


    }
}
