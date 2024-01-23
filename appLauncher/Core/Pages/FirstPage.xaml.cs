using appLauncher.Core.Helpers;
using appLauncher.Core.Model;

using Microsoft.Toolkit.Uwp.UI.Controls;

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
        public static InAppNotification showMessage { get; set; }
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
                    if (AnyFavorites())
                    {
                        AppFolder folder = new AppFolder()
                        {
                            Name = "Favorites",
                            Description = "Favorited apps",
                            ListPos = PackageHelper.Apps.GetOriginalCollection().Count - 1,
                            InstalledDate = System.DateTime.Now
                        };

                        await PackageHelper.Apps.AddFolder(folder);
                        await PackageHelper.Apps.RecalculateThePageItems();
                        break;
                    }
                    showMessage.Show("No apps selected as favorite", 2000);
                    break;
                case "used":
                    if (AnyMostUsed())
                    {
                        AppFolder usedfolder = new AppFolder()
                        {
                            Name = "Most Used",
                            Description = "Apps Launched over 5 times using this app",
                            ListPos = PackageHelper.Apps.GetOriginalCollection().Count - 1,
                            InstalledDate = DateTime.Now
                        };
                        await PackageHelper.Apps.AddFolder(usedfolder);
                        await PackageHelper.Apps.RecalculateThePageItems();
                        break;
                    }
                    //  MainPage.Inapp.Show("No apps launched more than 5 times", 500);
                    break;
                default:
                    break;
            }
            await PackageHelper.SaveCollectionAsync();
        }
        private bool AnyFavorites()
        {
            var apps = PackageHelper.Apps.GetOriginalCollection().OfType<FinalTiles>().ToList();
            var folders = PackageHelper.Apps.GetOriginalCollection().OfType<AppFolder>().ToList();
            foreach (var item in folders)
            {
                apps.AddRange(item.FolderApps.ToList());
            }
            bool isfavorite = apps.Any(x => x.Favorite == true);
            return isfavorite;
        }
        private bool AnyMostUsed()
        {
            var apps = PackageHelper.Apps.GetOriginalCollection().OfType<FinalTiles>().ToList();
            var folders = PackageHelper.Apps.GetOriginalCollection().OfType<AppFolder>().ToList();
            foreach (var item in folders)
            {
                apps.AddRange(item.FolderApps.ToList());
            }
            return apps.Any(x => x.LaunchedCount > 5);
        }
        public static void UpdateMessage(string texttodisplay)
        {
            //       Inapp.Show(texttodisplay);
        }

        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            showMessage = Inapp;
        }
    }
}
