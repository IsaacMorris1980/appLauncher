using appLauncher.Core.Helpers;
using appLauncher.Core.Model;

using System;
using System.Collections.Generic;
using System.Linq;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace appLauncher.Core.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateFolders : Page
    {
        private AppFolder _createdFolder = new AppFolder();
        private List<FinalTiles> tiles = new List<FinalTiles>();
        private List<AppFolder> folders = new List<AppFolder>();
        public CreateFolders()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderNamePage();
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                _createdFolder.Name = dialog.FolderName;
            }
            folders = PackageHelper.Apps.GetOriginalCollection().OfType<AppFolder>().ToList();
            tiles = PackageHelper.Apps.GetOriginalCollection().OfType<FinalTiles>().ToList();
        }

        private void AllTiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.FindName("addApp");
            this.UnloadObject(removeApp);
        }

        private void AppsinFolders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.FindName("removeApp");
            this.UnloadObject(addApp);
        }

        private void addApp_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FinalTiles app = (FinalTiles)AllTiles.SelectedItem;
            _createdFolder.FolderApps.Add((FinalTiles)AllTiles.SelectedItem);
            tiles.Remove((FinalTiles)AllTiles.SelectedItem);
            AppsinFolders.ItemsSource = _createdFolder.FolderApps;
            this.UnloadObject(addApp);
            Bindings.Update();

        }

        private void removeApp_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FinalTiles tile = (FinalTiles)AppsinFolders.SelectedItem;
            _createdFolder.FolderApps.Remove(tile);
            tiles.Add(tile);
            this.UnloadObject(removeApp);
            Bindings.Update();

        }
    }
}
