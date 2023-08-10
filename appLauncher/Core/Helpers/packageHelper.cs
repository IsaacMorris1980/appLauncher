// Methods for getting installed apps/games from the device are here. Note: Package = App/Game
using appLauncher.Core.Model;

using GoogleAnalyticsv4SDK.Events.Mobile;

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
using Windows.UI.Xaml;

namespace appLauncher.Core.Helpers
{
    public static class PackageHelper
    {

        public static List<AppTiles> SearchApps { get; set; }
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
                    if (SettingsHelper.totalAppSettings.Reporting)
                    {
                        ((App)Application.Current).reportEvents.Add(new Execeptions(es));
                        ((App)Application.Current).reportCrashandAnalytics.SendEvent(((App)Application.Current).reportEvents, SettingsHelper.totalAppSettings.ClientID, false);
                        ((App)Application.Current).reportEvents.Clear();

                    }
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
                                    listApps.Add(new AppTiles()
                                    {
                                        Name = item.DisplayName,
                                        FullName = item.Id.FullName,
                                        Description = item.Description,
                                        Developer = item.PublisherDisplayName,
                                        InstalledDate = item.InstalledDate,
                                        Tip = $"Name: {item.DisplayName}{Environment.NewLine}Developer: {item.PublisherDisplayName}{Environment.NewLine}Installed: {item.InstalledDate}",
                                        Logo = new byte[1]
                                    });
                                    if (SettingsHelper.totalAppSettings.Reporting)
                                    {
                                        ((App)Application.Current).reportEvents.Add(new Execeptions(es));
                                        ((App)Application.Current).reportCrashandAnalytics.SendEvent(((App)Application.Current).reportEvents, SettingsHelper.totalAppSettings.ClientID, false);
                                        ((App)Application.Current).reportEvents.Clear();
                                    }
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
                                listApps.Add(new AppTiles()
                                {
                                    Name = item.DisplayName,
                                    FullName = item.Id.FullName,
                                    Description = item.Description,
                                    Developer = item.PublisherDisplayName,
                                    InstalledDate = item.InstalledDate,
                                    Tip = $"Name: {item.DisplayName}{Environment.NewLine}Developer: {item.PublisherDisplayName}{Environment.NewLine}Installed: {item.InstalledDate}",
                                    Logo = temp
                                });
                            }
                            catch (Exception es)
                            {
                                listApps.Add(new AppTiles()
                                {
                                    Name = item.DisplayName,
                                    FullName = item.Id.FullName,
                                    Description = item.Description,
                                    Developer = item.PublisherDisplayName,
                                    InstalledDate = item.InstalledDate,
                                    Tip = $"Name: {item.DisplayName}{Environment.NewLine}Developer: {item.PublisherDisplayName}{Environment.NewLine}Installed: {item.InstalledDate}",
                                    Logo = new byte[1]
                                });
                                if (SettingsHelper.totalAppSettings.Reporting)
                                {
                                    ((App)Application.Current).reportEvents.Add(new Execeptions(es));
                                    ((App)Application.Current).reportCrashandAnalytics.SendEvent(((App)Application.Current).reportEvents, SettingsHelper.totalAppSettings.ClientID, false);
                                    ((App)Application.Current).reportEvents.Clear();
                                }
                                es = null;
                                continue;
                            }
                        }
                    }
                    catch (Exception es)
                    {
                        if (SettingsHelper.totalAppSettings.Reporting)
                        {
                            ((App)Application.Current).reportEvents.Add(new Execeptions(es));
                            ((App)Application.Current).reportCrashandAnalytics.SendEvent(((App)Application.Current).reportEvents, SettingsHelper.totalAppSettings.ClientID, false);
                            ((App)Application.Current).reportEvents.Clear();
                        }
                    }
                }
            }
            Apps = new AppPaginationObservableCollection(listApps);
            SearchApps = listApps.OrderBy(x => x.Name).ToList();
            AppsRetreived(true, EventArgs.Empty);
        }
        public static async Task SaveCollectionAsync()
        {
            try
            {
                List<AppTiles> saveApps = PackageHelper.Apps.GetOriginalCollection().ToList();
                string saveappsstring = JsonConvert.SerializeObject(saveApps, Formatting.Indented);
                StorageFile appsFile = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("collection.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(appsFile, saveappsstring);
            }
            catch (Exception es)
            {
                if (SettingsHelper.totalAppSettings.Reporting)
                {
                    ((App)Application.Current).reportEvents.Add(new Execeptions(es));
                    ((App)Application.Current).reportCrashandAnalytics.SendEvent(((App)Application.Current).reportEvents, SettingsHelper.totalAppSettings.ClientID, false);
                    ((App)Application.Current).reportEvents.Clear();
                }
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
            List<AppTiles> listApps = new List<AppTiles>();
            IEnumerable<Package> appslist = new PackageManager().FindPackagesForUserWithPackageTypes("", PackageTypes.Main);
            foreach (Package item in appslist)
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
                            catch (Exception)
                            {
                                listApps.Add(new AppTiles()
                                {
                                    Name = item.DisplayName,
                                    FullName = item.Id.FullName,
                                    Description = item.Description,
                                    Developer = item.PublisherDisplayName,
                                    InstalledDate = item.InstalledDate,
                                    Tip = $"Name: {item.DisplayName}{Environment.NewLine}Developer: {item.PublisherDisplayName}{Environment.NewLine}Installed: {item.InstalledDate}",
                                    Logo = new byte[1]
                                });
                                continue;
                            }
                            IRandomAccessStreamWithContentType whatIWant = await logoStream.OpenReadAsync();
                            byte[] temp = new byte[whatIWant.Size];
                            using (DataReader read = new DataReader(whatIWant.GetInputStreamAt(0)))
                            {
                                await read.LoadAsync((uint)whatIWant.Size);
                                read.ReadBytes(temp);
                            }
                            listApps.Add(new AppTiles()
                            {
                                Name = item.DisplayName,
                                FullName = item.Id.FullName,
                                Description = item.Description,
                                Developer = item.PublisherDisplayName,
                                InstalledDate = item.InstalledDate,
                                Tip = $"Name: {item.DisplayName}{Environment.NewLine}Developer: {item.PublisherDisplayName}{Environment.NewLine}Installed: {item.InstalledDate}",
                                Logo = temp
                            });
                        }
                        catch (Exception es)
                        {
                            listApps.Add(new AppTiles()
                            {
                                Name = item.DisplayName,
                                FullName = item.Id.FullName,
                                Description = item.Description,
                                Developer = item.PublisherDisplayName,
                                InstalledDate = item.InstalledDate,
                                Tip = $"Name: {item.DisplayName}{Environment.NewLine}Developer: {item.PublisherDisplayName}{Environment.NewLine}Installed: {item.InstalledDate}",
                                Logo = new byte[1]
                            });
                            if (SettingsHelper.totalAppSettings.Reporting)
                            {
                                ((App)Application.Current).reportEvents.Add(new Execeptions(es));
                                ((App)Application.Current).reportCrashandAnalytics.SendEvent(((App)Application.Current).reportEvents, SettingsHelper.totalAppSettings.ClientID, false);
                                ((App)Application.Current).reportEvents.Clear();
                            }
                            es = null;
                            continue;
                        }
                    }
                }
                catch (Exception es)
                {
                    if (SettingsHelper.totalAppSettings.Reporting)
                    {
                        ((App)Application.Current).reportEvents.Add(new Execeptions(es));
                        ((App)Application.Current).reportCrashandAnalytics.SendEvent(((App)Application.Current).reportEvents, SettingsHelper.totalAppSettings.ClientID, false);
                        ((App)Application.Current).reportEvents.Clear();
                    }

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

            Apps = new AppPaginationObservableCollection(listOfApps.OrderBy(x => x.Name));
            SearchApps = listOfApps.OrderBy(x => x.Name).ToList();
            return;
        }
    }
}
