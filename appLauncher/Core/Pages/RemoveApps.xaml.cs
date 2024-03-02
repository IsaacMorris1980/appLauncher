using appLauncher.Core.Helpers;
using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;

using System;
using System.Collections.Generic;
using System.Linq;

using Windows.Management.Deployment;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace appLauncher.Core.Pages
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RemoveApps : Page
    {
        private PackageManager pm = new PackageManager();
        private List<FinalTiles> finalTiles = new List<FinalTiles>();
        private List<AppFolder> appFolders = new List<AppFolder>();
        private List<FinalTiles> displaytiles = new List<FinalTiles>();
        public RemoveApps()
        {
            this.InitializeComponent();
            finalTiles = PackageHelper.Apps.GetOriginalCollection().OfType<FinalTiles>().ToList();
            appFolders = PackageHelper.Apps.GetOriginalCollection().OfType<AppFolder>().ToList();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            displaytiles.AddRange(finalTiles);
            foreach (var item in appFolders)
            {
                displaytiles.AddRange(item.FolderApps.OfType<FinalTiles>().ToList());
            }
            Apps.ItemsSource = displaytiles;
        }

        private async void AppBarButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            List<FinalTiles> a = Apps.SelectedItems.Cast<FinalTiles>().ToList();
            foreach (FinalTiles tile in a)
            {
                if (finalTiles.Contains(tile))
                {
                    finalTiles.Remove(tile);
                }
                foreach (var item in appFolders)
                {
                    if (item.FolderApps.Contains(tile))
                    {
                        item.FolderApps.Remove(tile);
                    }
                }
                await pm.RemovePackageAsync(tile.FullName);
                List<IApporFolder> recombinedlist = new List<IApporFolder>();
                recombinedlist.AddRange(finalTiles);
                recombinedlist.AddRange(appFolders);
                PackageHelper.Apps = new AppPaginationObservableCollection(recombinedlist);
                PackageHelper.Apps.RecalculateThePageItems();
            }
        }
    }
}
