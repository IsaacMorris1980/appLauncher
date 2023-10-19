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
        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            allFolders = new ObservableCollection<AppFolder>(PackageHelper.Apps.GetOriginalCollection().OfType<AppFolder>().ToList());
            allTiles = new ObservableCollection<FinalTiles>(PackageHelper.Apps.GetOriginalCollection().OfType<FinalTiles>().ToList());
            AllTiles.ItemsSource = allTiles;
            Folderslist.ItemsSource = allFolders;
        }

        private void AddFolder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (Folderslist.SelectedIndex < 0 && FolderName.Text.Length > 0)
            {
                string foldername = FolderName.Text + " (folder)";
                AppFolder folders = new AppFolder();
                selectedFolder = folders;
                folders.Name = foldername;
                allFolders.Add(folders);
                Bindings.Update();
            }
        }

        private void RemoveFolder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (Folderslist.SelectedIndex >= 0)
            {
                AppFolder folder = allFolders.FirstOrDefault(x => x.Name == ((AppFolder)Folderslist.SelectedItem).Name);
                ObservableCollection<FinalTiles> tiles = folder.FolderApps;

                allFolders.Remove(folder);
                Bindings.Update();
            }

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
            Bindings.Update();

        }

        private void Folderslist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Folderslist.SelectedIndex < 0)
            {
                this.UnloadObject(RemoveFolder);
                this.FindName("AddFolder");
                return;
            }
            AppFolder selfolder = allFolders.First(x => x.Name == ((AppFolder)Folderslist.SelectedItem).Name);
            AppsinFolders.ItemsSource = selfolder.FolderApps;
            Bindings.Update();
        }

        private void FolderName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (FolderName.Text.Length > 0)
            //{
            //    var a = this.FindName("AddFolder");
            //}
            //this.UnloadObject(AddFolder);
        }
    }
}
