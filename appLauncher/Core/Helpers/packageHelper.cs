// Methods for getting installed apps/games from the device are here. Note: Package = App/Game
using appLauncher.Core.Extensions;
using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Management.Deployment;
using Windows.Storage;
using Windows.Storage.Streams;

namespace appLauncher.Core.Helpers
{
    public static class PackageHelper
    {

        public static List<FinalTiles> SearchApps { get; set; }
        public static AppPaginationObservableCollection Apps { get; set; }

        public static event EventHandler AppsRetreived;
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
                    listApps.AddRange(tiles);
                }
                if (folders.Count > 0)
                {
                    listApps.AddRange(folders);
                }

            }
            else
            {
                List<FinalTiles> applist = await GetApps();
                listApps.AddRange(applist);

            }
            Apps = new AppPaginationObservableCollection(listApps.OrderBy(x => x.ListPos).ToList());
            AppsRetreived(true, EventArgs.Empty);
        }

        public static async Task<List<FinalTiles>> GetApps()
        {
            List<FinalTiles> listApps = new List<FinalTiles>();
            PackageManager packageManager = new PackageManager();
            int loc = 0;
            IEnumerable<Package> appsLists = packageManager.FindPackagesForUserWithPackageTypes("", PackageTypes.Main);
            foreach (Package item in appsLists)
            {
                try
                {

                    IReadOnlyList<AppListEntry> appsEntry = await item.GetAppListEntriesAsync();
                    if (appsEntry.Count > 0)
                    {
                        try
                        {
                            RandomAccessStreamReference logoStream;
                            try
                            {
                                logoStream = appsEntry[0].DisplayInfo.GetLogo(new Size(50, 50));
                            }
                            catch (Exception es)
                            {
                                listApps.Add(new FinalTiles()
                                {
                                    Name = item.DisplayName,
                                    FullName = item.Id.FullName,
                                    ListPos = loc,
                                    Description = item.Description,
                                    Developer = item.PublisherDisplayName,
                                    InstalledDate = item.InstalledDate,
                                    Tip = $"Name: {item.DisplayName}{Environment.NewLine}Developer: {item.PublisherDisplayName}{Environment.NewLine}Installed: {item.InstalledDate}",
                                    Logo = new byte[1]
                                });

                                loc += 1;
                                es = null;
                                continue;
                            }
                            IRandomAccessStreamWithContentType whatIWant = await logoStream.OpenReadAsync();
                            byte[] temp = new byte[whatIWant.Size];
                            using (DataReader read = new DataReader(whatIWant.GetInputStreamAt(0)))
                            {
                                await read.LoadAsync((uint)whatIWant.Size);
                                read.ReadBytes(temp);
                            }
                            listApps.Add(new FinalTiles()
                            {
                                Name = item.DisplayName,
                                FullName = item.Id.FullName,
                                Description = item.Description,
                                ListPos = loc,
                                Developer = item.PublisherDisplayName,
                                InstalledDate = item.InstalledDate,
                                Tip = $"Name: {item.DisplayName}{Environment.NewLine}Developer: {item.PublisherDisplayName}{Environment.NewLine}Installed: {item.InstalledDate}",
                                Logo = temp
                            });
                            loc += 1;
                        }
                        catch (Exception es)
                        {
                            listApps.Add(new FinalTiles()
                            {
                                Name = item.DisplayName,
                                FullName = item.Id.FullName,
                                Description = item.Description,
                                ListPos = loc,
                                Developer = item.PublisherDisplayName,
                                InstalledDate = item.InstalledDate,
                                Tip = $"Name: {item.DisplayName}{Environment.NewLine}Developer: {item.PublisherDisplayName}{Environment.NewLine}Installed: {item.InstalledDate}",
                                Logo = new byte[1]
                            });

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
            return;
        }
    }
}
