// Methods for getting installed apps/games from the device are here. Note: Package = App/Game
using appLauncher.Core.Model;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Management.Deployment;
using Windows.Storage;

namespace appLauncher.Core.Helpers
{
    public static class packageHelper
    {
        public static ObservableCollection<AppTile> allAppTiles { get; set; } = new ObservableCollection<AppTile>();
        public static ReadOnlyObservableCollection<AppTile> searchApps { get; private set; }
        public static PaginationObservableCollection appTiles { get; set; }

        public static List<AppTile> appTilesList { get; set; } = new List<AppTile>();
        /// <summary>
        /// Gets app package and image and returns them as a new "finalAppItem" which will be used for the app control template.
        /// <para>WARNING: Only use this method when it's REQUIRED. Otherwise use the Async version below this one.</para>
        /// </summary>


        //     public static ObservableCollection<finalAppItem> getAllApps()
        //     {
        //var listOfInstalledPackages = pkgManager.FindPackagesForUserWithPackageTypes("", PackageTypes.Main);
        //         List<Package> allPackages = new List<Package>();
        //         List<Package> packages = new List<Package>();
        //         allPackages = listOfInstalledPackages.ToList();
        //         int count = allPackages.Count();

        //         for (int i = 0; i < count; i++)
        //         {

        //                 packages.Add(allPackages[i]);


        //         }


        //         ObservableCollection<finalAppItem> finalAppItems = new ObservableCollection<finalAppItem>();

        //         count = packages.Count();

        //         //Loop will get app entry and logo for each app and create finalAppItem objects with that data.
        //         for (int i = 0; i < count; i++)
        //         {
        //             try
        //             {
        //                 List<AppListEntry> singleAppListEntries = new List<AppListEntry>();
        //                 Task<IReadOnlyList<AppListEntry>> getAppEntriesTask = packages[i].GetAppListEntriesAsync().AsTask();
        //                 getAppEntriesTask.Wait();
        //                 singleAppListEntries = getAppEntriesTask.Result.ToList();


        //                 BitmapImage logo = new BitmapImage();
        //                 var logoStream = singleAppListEntries[0].DisplayInfo.GetLogo(new Size(50, 50));
        //                 Task<IRandomAccessStreamWithContentType> logoStreamTask = logoStream.OpenReadAsync().AsTask();
        //                 logoStreamTask.Wait();
        //                 IRandomAccessStreamWithContentType logoStreamResult = logoStreamTask.Result;
        //                 logo.SetSource(logoStreamResult);
        //                 finalAppItems.Add(new finalAppItem
        //                 {
        //                     appEntry = singleAppListEntries[0],
        //                     appLogo = logo
        //                 });
        //             }

        //             catch(Exception e)
        //             {
        //                 Debug.WriteLine(e.Message);
        //             }
        //         }
        //         return finalAppItems;
        //     }

        public static event EventHandler AppsRetreived;

        public static async Task<bool> IsFilePresent(string fileName, string folderpath = "")

        {
            IStorageItem item;
            if (folderpath == "")
            {
                item = await ApplicationData.Current.LocalFolder.TryGetItemAsync(fileName);
            }
            else
            {
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderpath);
                item = await folder.TryGetItemAsync(fileName);
            }

            return item != null;

        }
        public static async Task GetAllAppsAsync()
        {

            PackageManager packageManager = new PackageManager();
            IEnumerable<Package> appslist = packageManager.FindPackagesForUserWithPackageTypes("", PackageTypes.Main);
            foreach (Package item in appslist)
            {
                IReadOnlyList<AppListEntry> t = await item.GetAppListEntriesAsync();
                if (t.Count > 0)
                {
                    appTilesList.Add(new AppTile(item, t[0]));
                }
            }

            appTiles = new PaginationObservableCollection(appTilesList);
            if (AppsRetreived != null)
            {
                AppsRetreived(true, EventArgs.Empty);

            }
        }
        public static Package GetPackage(string appfullename)
        {
            PackageManager pkg = new PackageManager();
            Package p = pkg.FindPackage(appfullename);

            return p;
        }
        public static async Task<AppListEntry> GetAppliestEntry(Package pack)
        {
            IReadOnlyList<AppListEntry> entries = await pack.GetAppListEntriesAsync();
            return entries[0];
        }

        public static async Task LoadCollectionAsync()
        {
            await GetAllAppsAsync();

            if (await packageHelper.IsFilePresent("collection.txt"))
            {
                List<AppTile> orderedapplist = new List<AppTile>();
                StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("collection.txt");
                string apps = await Windows.Storage.FileIO.ReadTextAsync(item);
                orderedapplist = JsonConvert.DeserializeObject<List<AppTile>>(apps);
                IEnumerable<AppTile> a = appTilesList.Where(p => !orderedapplist.Any(p2 => p2.appfullname == p.appfullname));
                if (a.Count() > 0)
                {
                    appTilesList = orderedapplist;
                    appTilesList.AddRange(a);
                }
                else
                {
                    appTilesList = orderedapplist;
                }
                packageHelper.appTiles = new PaginationObservableCollection(appTilesList);
            }


            //List<finalAppItem> oc1 = AllApps.listOfApps.ToList();
            //ObservableCollection<finalAppItem> oc = new ObservableCollection<finalAppItem>();
            //if (await IsFilePresent("collection.txt"))
            //{

            //    StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("collection.txt");
            //    var apps = await FileIO.ReadLinesAsync(item);
            //    if (apps.Count() > 1)
            //    {
            //        foreach (string y in apps)
            //        {
            //            foreach (finalAppItem items in oc1)
            //            {
            //                if (items.appEntry.DisplayInfo.DisplayName == y)
            //                {
            //                    oc.Add(items);
            //                }
            //            }
            //        }
            //    }
            //    AllApps.listOfApps = (oc.Count > 0) ? oc : new ObservableCollection<finalAppItem>(oc1);
            //}





        }
        public static async Task SaveCollectionAsync()
        {
            var te = JsonConvert.SerializeObject(packageHelper.appTiles.GetInternalList(), Formatting.Indented);
            StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("collection.txt", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(item, te);

            //List<finalAppItem> finals = AllApps.listOfApps.ToList();
            //var te = from x in finals select x.appEntry.DisplayInfo.DisplayName;
            //StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("collection.txt", CreationCollisionOption.ReplaceExisting);
            //await FileIO.WriteLinesAsync(item, te);


        }
        /// <summary>
        /// Gets app package and image and returns them as a new "finalAppItem" asynchronously, which will then be used for the app control template.
        /// <para> Of the two getAllApps() methods, this is the preferred version because it doesn't block the stop the rest of the app from running when 
        /// this is being run.</para>
        /// </summary>
        /// <returns></returns>

        //     public static async Task getAllAppsAsync()
        //     {
        //List<KeyValuePair<AppListEntry, Package>> someapps = new List<KeyValuePair<AppListEntry, Package>>();
        //         var listOfInstalledPackages = pkgManager.FindPackagesForUserWithPackageTypes("", PackageTypes.Main);
        //         List<Package> allPackages = new List<Package>();
        //         List<Package> packages = new List<Package>();
        //         allPackages = listOfInstalledPackages.ToList();
        //         int count = allPackages.Count();


        //         //for (int i = 0; i < count; i++)
        //         //{

        //         //    packages.Add(allPackages[i]);


        //         //}


        //         ObservableCollection<finalAppItem> finalAppItems = new ObservableCollection<finalAppItem>();
        //         //count = packages.Count();
        //         for (int i = 0; i < count; i++)
        //         {
        //             try

        //             {
        //	    List<AppListEntry> singleAppListEntries = new List<AppListEntry>();


        //                 var appListEntries = await allPackages[i].GetAppListEntriesAsync();
        //		Package p = allPackages[i];

        //                 singleAppListEntries = appListEntries.ToList();
        //                 if (singleAppListEntries.Count > 0)
        //                 {
        //                     Debug.WriteLine("YES!");
        //                 }
        //                 await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low, async () => {
        //			//UI code here
        //			try
        //			{
        //				BitmapImage logo = new BitmapImage();

        //				var logoStream = singleAppListEntries[0].DisplayInfo.GetLogo(new Size(50, 50));

        //				IRandomAccessStreamWithContentType whatIWant = await logoStream.OpenReadAsync();

        //				logo.SetSource(whatIWant);

        //				finalAppItem itemToAdd = new finalAppItem();

        //				itemToAdd.appEntry = singleAppListEntries[0];

        //				itemToAdd.appLogo = logo;
        //				finalAppItems.Add(itemToAdd);
        //				someapps.Add(new KeyValuePair<AppListEntry, Package>(itemToAdd.appEntry, p));
        //			}

        //			catch (Exception e)
        //                     {
        //                         Debug.WriteLine(e.Message);
        //                     }
        //                 });
        //             }

        //             catch (Exception e)
        //             {
        //                 Debug.WriteLine(e.Message);
        //             }

        //         }
        //         bool yes = true;
        //AllApps.Allpackages = someapps;
        //         AllApps.listOfApps = finalAppItems;
        //         if (AppsRetreived != null)
        //         {
        //         AppsRetreived(yes ,EventArgs.Empty);

        //         }

        //     }
    }
}
