using appLauncher.Core.Helpers;
using appLauncher.Core.Model;

using System;
using System.Collections.Generic;
using System.Linq;

using Windows.System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace appLauncher.Core.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FolderView : Page
    {
        private AppFolder displayfolder;
        private ThreadPoolTimer threadPoolTimer;
        private bool isFirstRun = true;

        public FolderView()
        {
            try
            {

                this.InitializeComponent();

            }
            catch (Exception es)
            {

            }
        }

        private async void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FinalTiles selecteditem = (FinalTiles)AppsinFolders.SelectedItem;
            await PackageHelper.LaunchApp(selecteditem.FullName);
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            displayfolder = (AppFolder)e.Parameter;
        }

        private void MainPage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            threadPoolTimer.Cancel();
            Frame.Navigate(typeof(MainPage));
        }
        private void AppBarButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            threadPoolTimer.Cancel();
            Frame.Navigate(typeof(AboutPage));

        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            if (displayfolder.Name == "Favorites")
            {
                displayfolder.FolderApps.Clear();
                List<AppFolder> folders = PackageHelper.Apps.GetOriginalCollection().OfType<AppFolder>().ToList();
                List<FinalTiles> tiles = PackageHelper.Apps.GetOriginalCollection().OfType<FinalTiles>().ToList();
                foreach (var item in folders)
                {
                    tiles.AddRange(item.FolderApps);
                }
                var nonorderedtiles = tiles.Where(x => x.Favorite == true).ToList();
                for (int i = 0; i < nonorderedtiles.Count; i++)
                {
                    nonorderedtiles[i].FolderListPos = i;
                }
                displayfolder.FolderApps = new System.Collections.ObjectModel.ObservableCollection<FinalTiles>(nonorderedtiles.OrderBy(x => x.FolderListPos).ToList());
            }
            if (displayfolder.Name == "Most Used")
            {
                //  displayfolder.FolderApps.Clear();
                List<FinalTiles> tiles = PackageHelper.Apps.GetOriginalCollection().OfType<FinalTiles>().ToList();
                List<AppFolder> folders = PackageHelper.Apps.GetOriginalCollection().OfType<AppFolder>().ToList();
                foreach (var item in folders)
                {
                    tiles.AddRange(item.FolderApps);
                }
                var nonorderedlist = tiles.Where(x => x.LaunchedCount > 5).ToList();
                for (int i = 0; i < nonorderedlist.Count; i++)
                {
                    nonorderedlist[i].FolderListPos = i;
                }
                displayfolder.FolderApps = new System.Collections.ObjectModel.ObservableCollection<FinalTiles>(nonorderedlist.OrderBy(x => x.FolderListPos).ToList());

            }
            if (displayfolder.FolderApps.Count == 0)
            {
                PackageHelper.Apps.Removefolder(displayfolder);
                FirstPage.navFrame.Navigate(typeof(MainPage));
            }
            Bindings.Update();

        }



        private void SearchField_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var auto = sender;
                if (PackageHelper.SearchApps.Count > 0)
                {
                    sender.ItemsSource = displayfolder.FolderApps.Where(p => p.Name.ToLower().Contains(((AutoSuggestBox)sender).Text.ToLower())).ToList();

                }
            }
        }

        private void SearchField_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            FinalTiles ap = (FinalTiles)args.SelectedItem;
            PackageHelper.LaunchApp(ap.FullName).ConfigureAwait(false);

            sender.ItemsSource = displayfolder.FolderApps;
            sender.Text = String.Empty;
        }

        private void Grid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            Grid grids = (Grid)sender;
            Object item = (e.OriginalSource as FrameworkElement)?.DataContext;
            if (item != null & grids != null)
            {
                if (item.GetType() == typeof(FinalTiles))
                {
                    Frame.Navigate(typeof(AppInformation), (FinalTiles)item);
                }
            }
        }
    }
}

