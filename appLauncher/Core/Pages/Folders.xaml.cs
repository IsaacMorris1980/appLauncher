using appLauncher.Core.Helpers;
using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;

using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ObservableCollection<AppFolder> allFolders;
        private ObservableCollection<FinalTiles> allTiles;
        private AppFolder selectedFolder;
        public Folders()
        {
            this.InitializeComponent();
        }

        private async void MainPage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            List<IApporFolder> recombinedlist = new List<IApporFolder>();
            recombinedlist.AddRange(allFolders);
            recombinedlist.AddRange(allTiles);
            PackageHelper.Apps = new AppPaginationObservableCollection(recombinedlist.OrderBy(x => x.ListPos));
            await PackageHelper.SaveCollectionAsync();
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
            selectedFolder = (AppFolder)args.SelectedItem;
            FolderSearch.Text = selectedFolder.Name;
            AppsinFolders.ItemsSource = selectedFolder.FolderApps;
            Bindings.Update();
        }
        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            allFolders = new ObservableCollection<AppFolder>(PackageHelper.Apps.GetOriginalCollection().OfType<AppFolder>().ToList());
            FolderSearch.ItemsSource = allFolders;
            allTiles = new ObservableCollection<FinalTiles>(PackageHelper.Apps.GetOriginalCollection().OfType<FinalTiles>().ToList());
            AllTiles.ItemsSource = allTiles;
        }

        private void AddFolder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            string foldername = FolderSearch.Text + " (folder)";
            AppFolder folders = new AppFolder();
            selectedFolder = folders;
            folders.Name = foldername;
            allFolders.Add(folders);
            PackageHelper.Apps.Add(folders);
            Bindings.Update();

        }

        private void RemoveFolder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            AppFolder folder = allFolders.FirstOrDefault(x => x.Name.ToLower() == FolderSearch.Text.ToLower());
            ObservableCollection<FinalTiles> tiles = folder.FolderApps;

            allFolders.Remove(folder);
            PackageHelper.Apps.Removefolder(folder);
        }

        private void addApp_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FinalTiles app = (FinalTiles)AllTiles.SelectedItem;
            selectedFolder.FolderApps.Add((FinalTiles)AllTiles.SelectedItem);
            allTiles.Remove((FinalTiles)AllTiles.SelectedItem);
            AppsinFolders.ItemsSource = selectedFolder.FolderApps;
            Bindings.Update();
        }

        private void removeApp_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FinalTiles tile = (FinalTiles)AppsinFolders.SelectedItem;
            selectedFolder.FolderApps.Remove(tile);
            allTiles.Add(tile);

        }
    }
}
