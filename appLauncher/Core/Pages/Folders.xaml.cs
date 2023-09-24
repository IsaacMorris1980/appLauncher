using appLauncher.Core.Helpers;
using appLauncher.Core.Model;

using System.Collections.Generic;
using System.Linq;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace appLauncher.Core.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Folders : Page
    {
        private List<AppFolder> allFolders = new List<AppFolder>();
        private List<FinalTiles> allTiles = new List<FinalTiles>();
        public Folders()
        {
            this.InitializeComponent();
        }

        private void MainPage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void FolderSearch_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                if (allFolders.Count > 0)
                {
                    IEnumerable<AppFolder> a = allFolders.Where(x => x.Name.ToLower() == FolderSearch.Text.ToLower()).ToList();
                    if (a.Count() > 0)
                    {

                    }

                }

            }
        }

        private void FolderSearch_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {

        }
        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            allFolders.AddRange(PackageHelper.Apps.OfType<AppFolder>().ToList());
            allTiles.AddRange(PackageHelper.Apps.OfType<FinalTiles>().ToList());
        }

        private void AddFolder_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void RemoveFolder_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
    }
}
