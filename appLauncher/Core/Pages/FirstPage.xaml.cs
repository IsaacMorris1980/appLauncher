using appLauncher.Core.Helpers;
using appLauncher.Core.Model;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace appLauncher.Core.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FirstPage : Page
    {
        public static List<FinalTiles> tiles = new List<FinalTiles>();
        public static List<AppFolder> appFolders = new List<AppFolder>();
        public static Frame navFrame { get; set; }
        public FirstPage()
        {
            this.InitializeComponent();
            navFrame = NavFrame;
            NavFrame.Navigate(typeof(AppLoading));
        }

        private void BackButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Debug.WriteLine("In back tapped");
            if (NavFrame.CanGoBack)
            {
                NavFrame.GoBack();
            }
        }

        private void ForwardButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (NavFrame.CanGoForward)
            {
                NavFrame.GoForward();
                NavFrame.BackStack.RemoveAt(NavFrame.BackStackDepth - 1);
                GC.Collect();
            }
        }

        private void AppsButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            NavFrame.Navigate(typeof(MainPage));
        }

        private void SettingsButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            NavFrame.Navigate(typeof(SettingsPage));
        }

        private void AboutButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            NavFrame.Navigate(typeof(AboutPage));
        }

        private void FilterApps_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ((FontIcon)sender).ContextFlyout.ShowAt(((FontIcon)sender));
        }



        private void Search_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ((FontIcon)sender).ContextFlyout.ShowAt((FontIcon)sender);
        }

        private async void Searchbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            MainPage.firstrun = true;
            await PackageHelper.Apps.Search(((TextBox)sender).Text);
        }

        private void CreateSpecialFolder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ((FontIcon)sender).ContextFlyout.ShowAt((FontIcon)sender);
        }

        private void InstallorRemoveApp_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ((FontIcon)sender).ContextFlyout.ShowAt((FontIcon)sender);
        }
        private void InstallorRemvoe(object sender, TappedRoutedEventArgs e)
        {
            switch (((MenuFlyoutItem)sender).Tag)
            {
                case "Install":
                    NavFrame.Navigate(typeof(InstallApps));
                    break;
                case "Remove":
                    navFrame.Navigate(typeof(RemoveApps));
                    break;
                default:
                    break;
            }
        }

        private void Rescan_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
        private void FilterAppsAndFolders(object sender, TappedRoutedEventArgs e)
        {

        }
        private void CreateRemoveFolders(object sender, TappedRoutedEventArgs e)
        {
            switch (((MenuFlyoutItem)sender).Tag)
            {
                case "Create":
                    navFrame.Navigate(typeof(CreateFolders));
                    break;
                case "Remove":
                    break;
                default:
                    break;
            }
        }
        private async void CreateSpecialFolders(object sender, TappedRoutedEventArgs e)
        {
            if (PackageHelper.Apps.GetOriginalCollection().OfType<AppFolder>().Any(x => x.Name == "Favorite") || PackageHelper.Apps.GetOriginalCollection().OfType<AppFolder>().Any(x => x.Name == "Most Used"))
            {
                return;
            }
            switch (((MenuFlyoutItem)sender).Tag)
            {
                case "favorite":
                    AppFolder folder = new AppFolder()
                    {
                        Name = "Favorites",
                        Description = "Favorited apps",
                        ListPos = PackageHelper.Apps.GetOriginalCollection().Count - 1,
                        InstalledDate = System.DateTime.Now
                    };
                    List<FinalTiles> allfavs = new List<FinalTiles>();
                    allfavs.AddRange(tiles.Where(x => x.Favorite == true));
                    foreach (var item in appFolders)
                    {
                        allfavs.AddRange(item.FolderApps.Where(x => x.Favorite == true));
                    }
                    folder.FolderApps = new System.Collections.ObjectModel.ObservableCollection<FinalTiles>(allfavs);
                    PackageHelper.Apps.AddFolder(folder);
                    await PackageHelper.Apps.RecalculateThePageItems();
                    break;
                case "used":
                    AppFolder usedfolder = new AppFolder()
                    {
                        Name = "Most Used",
                        Description = "Apps Launched over 5 times using this app",
                        ListPos = PackageHelper.Apps.GetOriginalCollection().Count - 1,
                        InstalledDate = DateTime.Now
                    };
                    List<FinalTiles> usedtiles = new List<FinalTiles>();
                    usedtiles.AddRange(tiles.Where(x => x.LaunchedCount > 5).ToList());
                    foreach (var item in appFolders)
                    {
                        usedtiles.AddRange(item.FolderApps.Where(x => x.LaunchedCount > 6));
                    }
                    usedfolder.FolderApps = new System.Collections.ObjectModel.ObservableCollection<FinalTiles>(usedtiles);
                    PackageHelper.Apps.AddFolder(usedfolder);
                    await PackageHelper.Apps.RecalculateThePageItems();
                    break;
                default:
                    break;
            }
            await PackageHelper.SaveCollectionAsync();
        }

        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }
    }
}
