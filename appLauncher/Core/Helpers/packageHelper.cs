// Methods for getting installed apps/games from the device are here. Note: Package = App/Game
using appLauncher.Core.Extensions;
using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;
using appLauncher.Core.Pages;

using Newtonsoft.Json;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Devices.Sensors;
using Windows.Management.Deployment;
using Windows.Storage;
using Windows.UI.Xaml.Input;

namespace appLauncher.Core.Helpers
{
    public static class PackageHelper
    {

        public static List<IApporFolder> Search { get; set; }
        public static AppPaginationObservableCollection Apps { get; set; }
        public static event EventHandler AppsRetreived;
        public static PageChangingVariables pageVariables { get; set; }
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
                    var appTasks = tiles.Select(async tile =>
                    {
                        try
                        {
                            FinalTiles applist = new FinalTiles();
                            applist = allApps.Find(x => x.FullName == tile.FullName);
                            if (applist != null)
                            {


                                applist.BackColor = tile.BackColor;
                                applist.LogoColor = tile.LogoColor;
                                applist.TextColor = tile.TextColor;
                                applist.ListPos = tile.ListPos;
                                applist.FolderListPos = tile.FolderListPos;
                                applist.Favorite = tile.Favorite;
                                await applist.SetLogo();
                                return applist;
                            }

                        }
                        catch (Exception ex)
                        {

                            await MainPage.LoggingCrashesAsync(ex);
                        }

                    });
                    listApps = (await Task.WhenAll(appTasks)).Where(x => x != null).Orderby(x => x.ListPos).ToList();
                }
                if (folders.Count > 0)
                {
                    var folderTasks = folders.Select(async folder =>
                    {
                        AppFolder appfolder = new AppFolder();
                        appfolder.Name = folder.Name;
                        appfolder.Description = folder.Description;
                        appfolder.Favorite = folder.Favorite;
                        appfolder.ListPos = folder.ListPos;
                        appfolder.BackColor = folder.BackColor;
                        appfolder.TextColor = folder.TextColor;
                        var folderapps = folder.FolderApps.Select(async apps =>
                        {
                            FinalTiles applist = new FinalTiles();
                            applist = allApps.First(x => x.FullName == apps.FullName);
                            try
                            {
                                applist.BackColor = apps.BackColor;
                                applist.LogoColor = apps.LogoColor;
                                applist.TextColor = apps.TextColor;
                                applist.ListPos = apps.ListPos;
                                applist.FolderListPos = apps.FolderListPos;
                                applist.Favorite = apps.Favorite;
                                await applist.SetLogo();
                                return applist;
                            }
                            catch (Exception ex)
                            {

                                await MainPage.LoggingCrashesAsync(ex);
                            }
                        });
                        appfolder.FolderApps = (await Task.WhenAll(folderapps)).Where(x => x != null).OrderBy(x => x.FolderListPos).ToList();
                        await appfolder.
                        return appfolder;
                    });
                    listApps.AddRange(await Task.WhenAll(folderTasks)).Where(x => x != null).ToList());
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
            Search = listApps.OrderBy(x => x.Name).ToList();
           AppsRetreived(true, EventArgs.Empty);
        }
        public static async Task<List<FinalTiles>> GetApps()
        {
            PackageManager pm = new PackageManager();
            List<Package> packages = pm.FindPackagesForUserWithPackageTypes("", PackageTypes.Main).ToList();
            ConcurrentBag<FinalTiles> allapps = new ConcurrentBag<FinalTiles>();
            List<FinalTiles> listApps = new List<FinalTiles>();
            int loc = 0;

            var tasks = packages.Select(async pack =>
            {
                try
                {
                    IReadOnlyList<AppListEntry> appsEntry = await pack.GetAppListEntriesAsync();
                    if (appsEntry.Count > 0)
                    {
                        try
                        {
                            FinalTiles finalTile = new FinalTiles()
                            {
                                Pack = pack,
                                Entry = appsEntry[0]
                            };
                            await finalTile.SetLogo();
                            return finalTile;
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                    return null;
                }                
                catch (Exception)
                {
                    throw;
                }
            });
           await Task.WhenAll(tasks);
            listApps = (await Task.WhenAll(tasks)).Where(x=>x!=null).OrderBy(x => x.Name).ToList();
            for (int i = 0; i < listApps.Count()-1; i++)
            {
                listApps[i].ListPos = loc;
                loc += 1;
            }
            return listApps;
        }
        public static async Task SaveCollectionAsync()
        {
            try
            {
                List<FinalTiles> saveApps = PackageHelper.Apps.GetOriginalCollection().OfType<FinalTiles>().ToList();
                List<AppFolder> saveFolders = PackageHelper.Apps.GetOriginalCollection().OfType<AppFolder>().ToList();
                string saveappsstring = JsonConvert.SerializeObject(saveApps, Formatting.Indented);
                string savefolderstring = JsonConvert.SerializeObject(saveFolders, Formatting.Indented);
                if (saveApps.Count > 0)
                {
                    StorageFile appsFile = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("allapps.json", CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(appsFile, saveappsstring);
                }
               
               
                if (saveFolders.Count > 0)
                {
                    StorageFile folderFile = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("folders.json", CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(folderFile, savefolderstring);
                }
            }
            catch (Exception es)
            {
                await Logging.Log(es);
            }
        }
        public static async Task<bool> LaunchApp(string fullname)
        {
            try
            {
                Package pm = new PackageManager().FindPackageForUser("", fullname);
                IReadOnlyList<AppListEntry> listEntry = await pm.GetAppListEntriesAsync();
                return await listEntry[0].LaunchAsync();
            }
            catch (Exception es)
            {
                await Logging.Log(es);
            }
            return false;
        }
        public static async Task RescanForNewApplications()
        {
            List<FinalTiles> listApps = await GetApps();          
            // this is incorrect need to fix before release
            List<FinalTiles> listOfApps = Apps.GetOriginalCollection().OfType<FinalTiles>().ToList();
            if (listApps.Count > listOfApps.Count)
            {
                IEnumerable<FinalTiles> a = listApps.Where(x => !listOfApps.Any(y => y.Name == x.Name)).ToList();
                int loc = Apps.GetOriginalCollection().Count; //This is incorrect need fixed before release will create out of range errors
              
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
            var folders = Search.OfType<AppFolder>().ToList();
            var apps = Search.OfType<FinalTiles>().ToList();
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
            Search = new List<IApporFolder>(recombine.OrderBy(x => x.Name));
        }
    }
}
