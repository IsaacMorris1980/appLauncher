﻿using appLauncher.Core.Helpers;
using appLauncher.Core.Model;

using System.Linq;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace appLauncher.Core.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppInformation : Page

    {
        private FinalTiles _tiles;
        public AppInformation()
        {
            this.InitializeComponent();
        }

        private void Home_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var allapps = PackageHelper.Apps.GetOriginalCollection().ToList();
            for (int i = 0; i < allapps.Count(); i++)
            {
                if (allapps[i].Name == _tiles.Name && allapps[i].GetType() == typeof(FinalTiles))
                {
                    ((FinalTiles)allapps[i]).Favorite = _tiles.Favorite;
                }
            }
            PackageHelper.Apps = new AppPaginationObservableCollection(allapps);

            Frame.Navigate(typeof(MainPage));
        }

        private void SettingsPage_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPage));
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _tiles = e.Parameter as FinalTiles;
        }

        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Favorited.IsChecked = _tiles.Favorite;
        }

        private void Favorited_Checked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _tiles.Favorite = (bool)Favorited.IsChecked;
        }
    }
}
