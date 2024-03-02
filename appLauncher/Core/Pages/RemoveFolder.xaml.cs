using appLauncher.Core.Helpers;
using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;

using System.Collections.Generic;
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
    public sealed partial class RemoveFolder : Page
    {
        private List<AppFolder> allFolders;
        private List<FinalTiles> allTiles;
        private AppFolder selectedFolder;
        public RemoveFolder()
        {
            this.InitializeComponent();

            allFolders = PackageHelper.Apps.GetOriginalCollection().OfType<AppFolder>().ToList();
            allTiles = PackageHelper.Apps.GetOriginalCollection().OfType<FinalTiles>().ToList();

        }

        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (allFolders != null)
            {
                Folderslist.ItemsSource = allFolders;
                if (selectedFolder != null)
                {
                    Folderslist.SelectedItem = selectedFolder;
                }
            }

            //AllTiles.ItemsSource = allTiles;
            //Folderslist.ItemsSource = allFolders;
            //restoreFolder = selectedFolder;

        }





        private void AppBarButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            AppFolder removeFolder = (AppFolder)Folderslist.SelectedItem;
            allTiles.AddRange(removeFolder.FolderApps);
            allFolders.Remove(removeFolder);
            List<IApporFolder> recombine = new List<IApporFolder>();
            recombine.AddRange(allFolders);
            recombine.AddRange(allTiles);
            PackageHelper.Apps = new AppPaginationObservableCollection(recombine.OrderBy(x => x.ListPos));
            PackageHelper.Apps.RecalculateThePageItems();
            Bindings.Update();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                selectedFolder = (AppFolder)e.Parameter;
            }
        }






        private void Folderslist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Folderslist.SelectedIndex < 0)
            {
                return;
            }
            this.FindName("AppsinFolder");
            AppFolder selfolder = allFolders.First(x => x.Name == ((AppFolder)Folderslist.SelectedItem).Name);
            selectedFolder = selfolder;

            AppsinFolder.ItemsSource = selfolder.FolderApps;
        }


        ////private async void SaveEdits_Tapped(object sender, TappedRoutedEventArgs e)
        ////{
        ////    if (!IsEditing)
        ////    {
        ////        selectedFolder = restoreFolder;
        ////        allFolders[allFolders.IndexOf(allFolders.First(x => x.Name == restoreFolder.Name))] = restoreFolder;

        ////    }
        ////    else
        ////    {
        ////        var intsel = allFolders.IndexOf(allFolders.First(x => x.Name == restoreFolder.Name));
        ////        selectedFolder.Name = FolderName.Text;
        ////        allFolders[intsel] = selectedFolder;
        ////    }
        ////    List<IApporFolder> recombinedlist = new List<IApporFolder>();
        ////    recombinedlist.AddRange(allFolders);
        ////    recombinedlist.AddRange(allTiles);
        ////    this.UnloadObject(Edits);
        ////    this.UnloadObject(SaveEdits);
        ////    this.UnloadObject(CancelEdits);

        ////    PackageHelper.Apps = new AppPaginationObservableCollection(recombinedlist.OrderBy(x => x.ListPos));
        ////    Bindings.Update();
        ////}






    }
}
