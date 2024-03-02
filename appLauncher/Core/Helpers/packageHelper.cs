// Methods for getting installed apps/games from the device are here. Note: Package = App/Game
using appLauncher.Core.Extensions;
using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;
using appLauncher.Core.Pages;

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
    public static class PackageHelper
    {

        public static List<IApporFolder> SearchApps { get; set; }
        public static AppPaginationObservableCollection Apps { get; set; }
        public static ObservableCollection<IApporFolder> AllApps { get; set; }
        public static IEnumerable<Package> packages { get; set; } = new List<Package>();

        public static event EventHandler AppsRetreived;
        public static FinalTiles CurrentWorkingTile { get; set; }
        public static PageChangingVariables pageVariables { get; set; } = new PageChangingVariables();


        public static async Task<bool> IsFilePresent(string fileName, string folderPath = "")
        {
            IStorageItem item;
            if (folderPath == "")
            {
                item = await ApplicationData.Current.LocalFolder.TryGetItemAsync(fileName);
            }
            else
            {
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
                item = await folder.TryGetItemAsync(fileName);
            }
            return item != null;
        }
        public static async Task LoadCollectionAsync()
        {
            PackageManager pm = new PackageManager();
            List<FinalTiles> allApps = await GetApps();
            List<IApporFolder> listApps = new List<IApporFolder>();
            List<FinalTiles> tiles = new List<FinalTiles>();
            List<AppFolder> folders = new List<AppFolder>();
            bool filesexist = false;


            if (await IsFilePresent("allapps.json"))
            {
                StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("allapps.json");
                string apps = await Windows.Storage.FileIO.ReadTextAsync(item);
                tiles = JsonConvert.DeserializeObject<List<FinalTiles>>(apps);
                filesexist = true;

            }
            if (await IsFilePresent("folders.json"))
            {
                StorageFile items = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("folders.json");
                string apps = await Windows.Storage.FileIO.ReadTextAsync(items);
                folders = JsonConvert.DeserializeObject<List<AppFolder>>(apps);
                filesexist = true;
            }
            if (filesexist)
            {
                if (tiles.Count > 0)
                {
                    foreach (FinalTiles item in tiles)
                    {
                        try
                        {
                            FinalTiles applist = new FinalTiles();
                            applist = allApps.Find(x => x.FullName == item.FullName);
                            if (applist != null)
                            {


                                applist.BackColor = item.BackColor;
                                applist.LogoColor = item.LogoColor;
                                applist.TextColor = item.TextColor;
                                applist.ListPos = item.ListPos;
                                applist.FolderListPos = item.FolderListPos;
                                applist.Favorite = item.Favorite;
                                await applist.SetLogo();
                                listApps.Add(applist);
                            }

                        }
                        catch (Exception ex)
                        {

                            await MainPage.LoggingCrashesAsync(ex);
                        }

                    }

                }
                if (folders.Count > 0)
                {
                    foreach (var item in folders)
                    {
                        foreach (var items in item.FolderApps)
                        {
                            FinalTiles applist = new FinalTiles();
                            applist = allApps.First(x => x.FullName == items.FullName);
                            try
                            {
                                applist.BackColor = items.BackColor;
                                applist.LogoColor = items.LogoColor;
                                applist.TextColor = items.TextColor;
                                applist.ListPos = items.ListPos;
                                applist.FolderListPos = items.FolderListPos;
                                applist.Favorite = items.Favorite;
                                await applist.SetLogo();
                            }
                            catch (Exception ex)
                            {

                                await MainPage.LoggingCrashesAsync(ex);
                            }
                            await items.SetLogo();
                        }
                        listApps.Add(item);
                    }

                }

            }
            else
            {
                foreach (var item in allApps.OfType<FinalTiles>().ToList())
                {
                    await item.SetLogo();
                }
                listApps.AddRange(allApps);
            }

            Apps = new AppPaginationObservableCollection(listApps.OrderBy(x => x.ListPos).ToList());
            SearchApps = listApps.OrderBy(x => x.Name).ToList();
            //      await Apps.RecalculateThePageItems();
            AppsRetreived(true, EventArgs.Empty);
        }

        public static async Task<List<FinalTiles>> GetApps()
        {
            PackageManager pm = new PackageManager();
            List<Package> packages = pm.FindPackagesForUserWithPackageTypes("", PackageTypes.Main).ToList();
            List<FinalTiles> listApps = new List<FinalTiles>();
            int loc = 0;
            foreach (Package item in packages)
            {
                try
                {
                    IReadOnlyList<AppListEntry> appsEntry = await item.GetAppListEntriesAsync();
                    if (appsEntry.Count > 0)
                    {
                        try
                        {
                            FinalTiles finalTile = new FinalTiles()
                            {
                                Pack = item,
                                Entry = appsEntry[0],
                                ListPos = loc,
                            };
                            await finalTile.SetLogo();
                            listApps.Add(finalTile);
                            loc += 1;
                        }
                        catch (Exception es)
                        {
                            FinalTiles finalTile = new FinalTiles()
                            {
                                Pack = item,
                                Entry = appsEntry[0],
                                ListPos = loc,

                            };
                            await finalTile.SetLogo();
                            listApps.Add(finalTile);
                            es = null;
                            loc += 1;
                            continue;
                        }
                    }
                }
                catch (Exception es)
                {

                }
            }
            return listApps;
        }

        public static async Task SaveCollectionAsync()
        {
            try
            {
                List<FinalTiles> saveApps = PackageHelper.Apps.GetOriginalCollection().OfType<FinalTiles>().ToList();
                string saveappsstring = JsonConvert.SerializeObject(saveApps, Formatting.Indented);
                if (saveApps.Count > 0)
                {
                    StorageFile appsFile = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("allapps.json", CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(appsFile, saveappsstring);
                }
                List<AppFolder> saveFolders = PackageHelper.Apps.GetOriginalCollection().OfType<AppFolder>().ToList();
                string savefolderstring = JsonConvert.SerializeObject(saveFolders, Formatting.Indented);
                if (saveFolders.Count > 0)
                {
                    StorageFile folderFile = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("folders.json", CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(folderFile, savefolderstring);
                }
            }
            catch (Exception es)
            {

            }
        }
        public static async Task<bool> LaunchApp(string fullname)
        {
            Package pm = new PackageManager().FindPackageForUser("", fullname);
            IReadOnlyList<AppListEntry> listEntry = await pm.GetAppListEntriesAsync();
            return await listEntry[0].LaunchAsync();
        }
        public static async Task RescanForNewApplications()
        {
            List<FinalTiles> listApps = await GetApps();

            List<FinalTiles> listOfApps = Apps.GetOriginalCollection().OfType<FinalTiles>().ToList();
            if (listApps.Count > listOfApps.Count)
            {
                IEnumerable<FinalTiles> a = listApps.Where(x => !listOfApps.Any(y => y.Name == x.Name)).ToList();
                int loc = Apps.GetOriginalCollection().Count;
                foreach (var item in a)
                {
                    item.ListPos = loc + 1;
                    listOfApps.Add(item);
                    loc += 1;
                }
            }
            else if (listOfApps.Count > listApps.Count)
            {
                IEnumerable<FinalTiles> a = listOfApps.Where(x => !listApps.Any(y => y.Name == x.Name)).ToList();
                foreach (var item in a)
                {
                    if (listOfApps.OfType<FinalTiles>().Any(x => x.FullName == item.FullName))
                    {
                        listOfApps.OfType<FinalTiles>().ToList().Remove(item);
                    }
                    else if (listOfApps.OfType<AppFolder>().Any(x => x.FolderApps.Any(y => y.FullName == item.FullName)))
                    {
                        foreach (AppFolder items in listOfApps.OfType<AppFolder>())
                        {
                            if (items.FolderApps.Any(z => z.FullName == item.FullName))
                            {
                                items.FolderApps.Remove<FinalTiles>(x => x.FullName == item.FullName);
                            }
                        }
                    }
                }
            }
            Apps = new AppPaginationObservableCollection(listOfApps.OrderBy(x => x.Name));
            Apps.RecalculateThePageItems();
            return;
        }
        public static void RemoveFromSearch(string fullNmae)
        {
            var folders = SearchApps.OfType<AppFolder>().ToList();
            var apps = SearchApps.OfType<FinalTiles>().ToList();
            List<IApporFolder> recombine = new List<IApporFolder>();
            foreach (AppFolder folder in folders)
            {
                foreach (FinalTiles item in folder.FolderApps)
                {
                    if (item.FullName == fullNmae)
                    {
                        folder.FolderApps.Remove(item);
                    }
                }
            }
            foreach (var item in apps)
            {
                if (item.FullName == fullNmae)
                {
                    apps.Remove(item);
                }
            }
            recombine.AddRange(folders);
            recombine.AddRange(apps);
            SearchApps = new List<IApporFolder>(recombine.OrderBy(x => x.ListPos));
        }
    }
}
