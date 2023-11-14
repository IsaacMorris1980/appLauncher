using appLauncher.Core.Helpers;
using appLauncher.Core.Model;

using System;
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
    }
}

