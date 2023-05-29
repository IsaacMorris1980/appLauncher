// Methods for getting installed apps/games from the device are here. Note: Package = App/Game
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
using Windows.UI;

namespace appLauncher.Core.Helpers
{
    public static class PackageHelper
    {

        public static List<AppTiles> SearchApps { get; set; }
        public static AppPaginationObservableCollection Apps { get; set; }
        public static List<AppTiles> appsList { get; set; } = new List<AppTiles>();

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
            List<AppTiles> listApps = new List<AppTiles>();
            if (await PackageHelper.IsFilePresent("collection.json"))
            {
                try
                {

                    StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("collection.json");
                    string apps = await Windows.Storage.FileIO.ReadTextAsync(item);
                    listApps = JsonConvert.DeserializeObject<List<AppTiles>>(apps);
                }
                catch (Exception es)
                {

                }
            }
            else
            {
                PackageManager packageManager = new PackageManager();
                IEnumerable<Package> appsLists = packageManager.FindPackagesForUserWithPackageTypes("", PackageTypes.Main);
                foreach (Package item in appsLists)
                {
                    try
                    {
                        AppTiles Apps = new AppTiles();
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
                                    Apps.Name = item.DisplayName;
                                    Apps.FullName = item.Id.FullName;
                                    Apps.Description = item.Description;
                                    Apps.Developer = item.PublisherDisplayName;
                                    Apps.InstalledDate = item.InstalledDate;
                                    Apps.Tip = $"Name: {item.DisplayName}{Environment.NewLine}Developer: {item.PublisherDisplayName}{Environment.NewLine}Installed: {item.InstalledDate}";
                                    Apps.Logo = new byte[1];
                                    listApps.Add(Apps);
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
                                Apps.Name = item.DisplayName;
                                Apps.FullName = item.Id.FullName;
                                Apps.Description = item.Description;
                                Apps.Developer = item.PublisherDisplayName;
                                Apps.InstalledDate = item.InstalledDate;
                                Apps.Tip = $"Name: {item.DisplayName}{Environment.NewLine}Developer: {item.PublisherDisplayName}{Environment.NewLine}Installed: {item.InstalledDate}";
                                Apps.Logo = temp;
                                listApps.Add(Apps);
                            }
                            catch (Exception es)
                            {
                                Apps.Name = item.DisplayName;
                                Apps.FullName = item.Id.FullName;
                                Apps.Description = item.Description;
                                Apps.Developer = item.PublisherDisplayName;
                                Apps.InstalledDate = item.InstalledDate;
                                Apps.Tip = $"Name: {item.DisplayName}{Environment.NewLine}Developer: {item.PublisherDisplayName}{Environment.NewLine}Installed: {item.InstalledDate}";
                                Apps.Logo = new byte[1];
                                listApps.Add(Apps);
                                es = null;
                                continue;
                            }
                        }
                    }
                    catch (Exception es)
                    {
                    }
                }
            }
            if (!SettingsHelper.totalAppSettings.Tiles)
            {
                for (int i = 0; i < listApps.Count(); i++)
                {
                    listApps[i].BackColor = Colors.Black;
                    listApps[i].TextColor = Colors.Red;
                    listApps[i].LogoColor = Colors.Blue;
                }
            }
            Apps = new AppPaginationObservableCollection(listApps);
            AppsRetreived(true, EventArgs.Empty);
        }
        public static async Task SaveCollectionAsync()
        {
            try
            {
                List<AppTiles> saveApps = PackageHelper.Apps.GetOriginalCollection().ToList();
                if (!SettingsHelper.totalAppSettings.Tiles)
                {
                    for (int i = 0; i < saveApps.Count(); i++)
                    {
                        saveApps[i].BackColor = Colors.Black;
                        saveApps[i].TextColor = Colors.Red;
                        saveApps[i].LogoColor = Colors.Blue;
                    }
                }
                var saveappsstring = JsonConvert.SerializeObject(saveApps, Formatting.Indented);
                StorageFile appsFile = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("collection.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(appsFile, saveappsstring);
            }
            catch (Exception es)
            {

            }
        }
        public static async Task<bool> LaunchApp(string fullname)
        {
            PackageManager pm = new PackageManager();
            Package pack = pm.FindPackageForUser("", fullname);
            IReadOnlyList<AppListEntry> listEntry = await pack.GetAppListEntriesAsync();
            return await listEntry[0].LaunchAsync();
        }
        public static async Task RescanForNewApplications()
        {
            List<AppTiles> listApps = new List<AppTiles>();
            PackageManager packageManager = new PackageManager();
            IEnumerable<Package> appsList = packageManager.FindPackagesForUserWithPackageTypes("", PackageTypes.Main);
            foreach (Package item in appsList)
            {
                try
                {
                    AppTiles AppListed = new AppTiles();
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
                                AppListed.Name = item.DisplayName;
                                AppListed.FullName = item.Id.FullName;
                                AppListed.Description = item.Description;
                                AppListed.Developer = item.PublisherDisplayName;
                                AppListed.InstalledDate = item.InstalledDate;
                                AppListed.Tip = $"Name: {item.DisplayName}{Environment.NewLine}Developer: {item.PublisherDisplayName}{Environment.NewLine}Installed: {item.InstalledDate}";
                                AppListed.Logo = new byte[1];
                                listApps.Add(AppListed);
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
                            AppListed.Name = item.DisplayName;
                            AppListed.FullName = item.Id.FullName;
                            AppListed.Description = item.Description;
                            AppListed.Developer = item.PublisherDisplayName;
                            AppListed.InstalledDate = item.InstalledDate;
                            AppListed.Tip = $"Name: {item.DisplayName}{Environment.NewLine}Developer: {item.PublisherDisplayName}{Environment.NewLine}Installed: {item.InstalledDate}";
                            AppListed.Logo = temp;
                            listApps.Add(AppListed);
                        }
                        catch (Exception es)
                        {
                            AppListed.Name = item.DisplayName;
                            AppListed.FullName = item.Id.FullName;
                            AppListed.Description = item.Description;
                            AppListed.Developer = item.PublisherDisplayName;
                            AppListed.InstalledDate = item.InstalledDate;
                            AppListed.Tip = $"Name: {item.DisplayName}{Environment.NewLine}Developer: {item.PublisherDisplayName}{Environment.NewLine}Installed: {item.InstalledDate}";
                            AppListed.Logo = new byte[1];
                            listApps.Add(AppListed);
                            es = null;
                            continue;
                        }
                    }
                }
                catch (Exception es)
                {
                }
            }
            List<AppTiles> listOfApps = Apps.GetOriginalCollection().ToList();
            if (listApps.Count > listOfApps.Count)
            {
                IEnumerable<AppTiles> a = listApps.Where(x => !listOfApps.Any(y => y.Name == x.Name)).ToList();
                foreach (var item in a)
                {
                    listOfApps.Add(item);
                }
            }
            else if (listOfApps.Count > listApps.Count)
            {
                IEnumerable<AppTiles> a = listOfApps.Where(x => !listApps.Any(y => y.Name == x.Name)).ToList();
                foreach (var item in a)
                {
                    listOfApps.Remove(item);
                }
            }
            //searchApps = new ReadOnlyObservableCollection<Apps>(new ObservableCollection<Apps>(listofApps.OrderBy(x => x.Name)));
            if (!SettingsHelper.totalAppSettings.Tiles)
            {
                for (int i = 0; i < listOfApps.Count(); i++)
                {
                    listOfApps[i].BackColor = Colors.Black;
                    listOfApps[i].TextColor = Colors.Red;
                    listOfApps[i].LogoColor = Colors.Blue;
                }
            }
            Apps = new AppPaginationObservableCollection(listOfApps.OrderBy(x => x.Name));
            return;
        }
    }
}
