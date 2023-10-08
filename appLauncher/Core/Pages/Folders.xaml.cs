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
                if (FolderSearch.Text.Length <= 0)
                {
                    this.UnloadObject(AddFolder);
                    this.UnloadObject(RemoveFolder);
                    return;
                }

                IEnumerable<AppFolder> a = PackageHelper.Apps.OfType<AppFolder>().Where(x => x.Name.ToLower() == FolderSearch.Text.ToLower()).ToList();
                if (a.Count() <= 0)
                {
                    this.FindName("AddFolder");
                }
                else
                {
                    this.FindName("RemoveFolder");
                    FolderSearch.ItemsSource = a;
                }
            }
            else
            {
                this.UnloadObject(AddFolder);
                this.UnloadObject(RemoveFolder);
            }
        }

        private void FolderSearch_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {

        }
        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            allFolders.AddRange(PackageHelper.Apps.GetOriginalCollection().OfType<AppFolder>().ToList());
            FolderSearch.ItemsSource = allFolders;
            allTiles.AddRange(PackageHelper.Apps.GetOriginalCollection().OfType<FinalTiles>().ToList());
            AllTiles.ItemsSource = PackageHelper.Apps.OfType<FinalTiles>().ToList();
        }

        private void AddFolder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            string foldername = FolderSearch.Text;
            AppFolder folders = new AppFolder();
            folders.Name = foldername;
            allFolders.Add(folders);
            PackageHelper.Apps.Add(folders);
            Frame.Navigate(typeof(Folders));

        }

        private void RemoveFolder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            AppFolder folder = allFolders.FirstOrDefault(x => x.Name.ToLower() == FolderSearch.Text.ToLower());
            allFolders.Remove(folder);
            PackageHelper.Apps.Removefolder(folder);
        }

        private void addApp_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (all)
            {

            }
        }

        private void removeApp_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
    }
}
