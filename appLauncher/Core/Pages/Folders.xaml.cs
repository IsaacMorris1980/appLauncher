using appLauncher.Core.Helpers;
using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

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
        private AppFolder restoreFolder;
        private bool IsEditing = false;
        public Folders()
        {
            this.InitializeComponent();
        }

        private void MainPage_Tapped(object sender, TappedRoutedEventArgs e)
        {

            Frame.Navigate(typeof(MainPage));
        }
        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            allFolders = new ObservableCollection<AppFolder>(PackageHelper.Apps.GetOriginalCollection().OfType<AppFolder>().ToList());
            allTiles = new ObservableCollection<FinalTiles>(PackageHelper.Apps.GetOriginalCollection().OfType<FinalTiles>().ToList());
            if (allFolders != null)
            {
                this.FindName("Folderslist");
                Folderslist.ItemsSource = allFolders;
                this.FindName("AddFolder");
                if (selectedFolder != null)
                {
                    Folderslist.SelectedItem = selectedFolder;
                    this.FindName("AllTiles");
                    this.FindName("Edits");
                    this.FindName("removeFolder");
                    this.FindName("InFolder");
                    this.FindName("AllTilesText");
                    this.UnloadObject(AddFolder);
                    this.FindName("addApp");
                }
            }
            //AllTiles.ItemsSource = allTiles;
            //Folderslist.ItemsSource = allFolders;
            //restoreFolder = selectedFolder;

        }




        private async void AddFolder_TappedAsync(object sender, TappedRoutedEventArgs e)
        {
            var dialog = new FolderNamePage();
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                AppFolder folder = new AppFolder();
                folder.Name = dialog.FolderName;
                allFolders.Add(folder);
                Bindings.Update();
                selectedFolder = folder;
                this.FindName("AppsinFolder");
                this.FindName("Folderslist");
                this.FindName("addApp");
                this.FindName("InFolder");
                this.FindName("AllTilesText");
                Folderslist.SelectedItem = folder;
                restoreFolder = selectedFolder;
                IsEditing = true;
                this.UnloadObject(AddFolder);
                this.UnloadObject(Edits);
                this.FindName("FolderName");
                FolderName.Text = folder.Name;

                this.FindName("SaveEdits");
                this.FindName("CancelEdits");
                this.FindName("RemoveFolder");

            }
            else
            {

            }
            ////if (Folderslist.SelectedIndex < 0 && FolderName.Text.Length > 0)
            ////{
            ////    string foldername = FolderName.Text + " (folder)";
            ////    AppFolder folders = new AppFolder();
            ////    selectedFolder = folders;
            ////    folders.Name = foldername;
            ////    allFolders.Add(folders);
            ////    Bindings.Update();
            ////}
        }



        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                selectedFolder = (AppFolder)e.Parameter;
                restoreFolder = selectedFolder;
            }
        }
        private void RemoveFolder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (Folderslist.SelectedIndex >= 0)
            {
                AppFolder folder = allFolders.FirstOrDefault(x => x.Name == ((AppFolder)Folderslist.SelectedItem).Name);
                ObservableCollection<FinalTiles> tiles = folder.FolderApps;
                this.UnloadObject(Edits);
                this.UnloadObject(InFolder);
                this.UnloadObject(AllTilesText);
                allFolders.Remove(folder);
                AppsinFolders.ItemsSource = new List<FinalTiles>();
                AllTiles.ItemsSource = new List<FinalTiles>();
                this.UnloadObject(AppsinFolders);
                this.UnloadObject(AllTiles);
                if (allFolders.Count() <= 0)
                {
                    this.UnloadObject(Folderslist);
                }
                Bindings.Update();
            }

        }

        private void addApp_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FinalTiles app = (FinalTiles)AllTiles.SelectedItem;
            selectedFolder.FolderApps.Add((FinalTiles)AllTiles.SelectedItem);
            allTiles.Remove((FinalTiles)AllTiles.SelectedItem);
            AppsinFolders.ItemsSource = selectedFolder.FolderApps;
            this.UnloadObject(addApp);
            Bindings.Update();

        }

        private void removeApp_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FinalTiles tile = (FinalTiles)AppsinFolders.SelectedItem;
            selectedFolder.FolderApps.Remove(tile);
            allTiles.Add(tile);
            this.UnloadObject(removeApp);
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
            this.FindName("Edits");
            this.FindName("RemoveFolder");

            AppFolder selfolder = allFolders.First(x => x.Name == ((AppFolder)Folderslist.SelectedItem).Name);
            selectedFolder = selfolder;
            restoreFolder = selectedFolder;
            this.FindName("AppsinFolders");
            this.FindName("InFolder");
            this.FindName("FolderName");
            FolderName.Text = selectedFolder.Name;
            AppsinFolders.ItemsSource = selfolder.FolderApps;
            this.FindName("AllTilesText");
            this.FindName("AllTiles");
            AllTiles.ItemsSource = allTiles;
            this.UnloadObject(AddFolder);
            Bindings.Update();
        }

        private void FolderName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private async void SaveEdits_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!IsEditing)
            {
                selectedFolder = restoreFolder;
                allFolders[allFolders.IndexOf(allFolders.First(x => x.Name == restoreFolder.Name))] = restoreFolder;

            }
            else
            {
                var intsel = allFolders.IndexOf(allFolders.First(x => x.Name == restoreFolder.Name));
                selectedFolder.Name = FolderName.Text;
                allFolders[intsel] = selectedFolder;
            }
            List<IApporFolder> recombinedlist = new List<IApporFolder>();
            recombinedlist.AddRange(allFolders);
            recombinedlist.AddRange(allTiles);
            this.UnloadObject(Edits);
            this.UnloadObject(SaveEdits);
            this.UnloadObject(CancelEdits);

            PackageHelper.Apps = new AppPaginationObservableCollection(recombinedlist.OrderBy(x => x.ListPos));
            Bindings.Update();
        }

        private void CancelEdits_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (IsEditing)
            {
                selectedFolder = restoreFolder;
                this.FindName("AddFolder");
                this.UnloadObject(SaveEdits);
                this.UnloadObject(Edits);
                this.UnloadObject(CancelEdits);
                this.UnloadObject(FolderName);
                this.UnloadObject(RemoveFolder);
                Folderslist.SelectedIndex = -1;
                AppsinFolders.ItemsSource = new List<AppFolder>();
                AllTiles.ItemsSource = new List<FinalTiles>();



            }
            else
            {

            }
        }

        private void Edits_Tapped(object sender, TappedRoutedEventArgs e)
        {
            IsEditing = true;
            this.UnloadObject(AddFolder);
            this.UnloadObject(Edits);
            this.UnloadObject(RemoveFolder);
            this.FindName("CancelEdits");
            this.FindName("SaveEdits");
            this.FindName("FolderName");
            FolderName.Text = selectedFolder.Name;
        }

        private void AllTiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.UnloadObject(removeApp);
            this.FindName("addApp");
        }

        private void AppsinFolders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.UnloadObject(addApp);
            this.FindName("removeApp");
        }
    }
}
