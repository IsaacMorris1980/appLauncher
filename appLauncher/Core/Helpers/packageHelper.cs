// Methods for getting installed apps/games from the device are here. Note: Package = App/Game
using appLauncher.Core.Interfaces;
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

        public static List<FinalTiles> SearchApps { get; set; }
        public static AppPaginationObservableCollection Apps { get; set; }

        public static List<FinalTiles> AllApps { get; set; } = new List<FinalTiles>();
        public static List<AppFolder> AllFolders { get; set; } = new List<AppFolder>();

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
            bool apped = await IsFilePresent("finalapps.json");
            bool folders = await IsFilePresent("folders.json");
            List<FinalTiles> allapp = new List<FinalTiles>();
            List<AppFolder> allfolder = new List<AppFolder>();

            if (apped)
            {
                allapp = await LoadFinalTiles();
            }
            if (folders)
            {
                allfolder = await LoadAppFolders();
            }
            if (allapp.Count <= 0 && allfolder.Count <= 0)
            {
                allapp = await GetApps();
                listApps.AddRange(allapp);
                Apps = new AppPaginationObservableCollection(listApps);
                // SearchApps = listApps.OrderBy(x => x.Name).ToList();
                AppsRetreived(true, EventArgs.Empty);
            }
            else
            {
                listApps.AddRange(allapp);
                listApps.AddRange(allfolder);
                Apps = new AppPaginationObservableCollection(listApps.OrderBy(x => x.ListPos));
                AppsRetreived(true, EventArgs.Empty);
            }

        }
        public static async Task<List<FinalTiles>> LoadFinalTiles()
        {
            List<FinalTiles> listApps = new List<FinalTiles>();
            try
            {
                StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("collection.json");
                string apps = await Windows.Storage.FileIO.ReadTextAsync(item);
                listApps = JsonConvert.DeserializeObject<List<FinalTiles>>(apps);
                AppsRetreived(true, EventArgs.Empty);
                return listApps;

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
            return null;
        }
        public static async Task<List<AppFolder>> LoadAppFolders()
        {
            List<AppFolder> listApps = new List<AppFolder>();
            try
            {
                StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("folders.json");
                string apps = await Windows.Storage.FileIO.ReadTextAsync(item);
                listApps = JsonConvert.DeserializeObject<List<AppFolder>>(apps);
                AppsRetreived(true, EventArgs.Empty);
                return listApps;

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
            return null;
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
                                if (SettingsHelper.totalAppSettings.Reporting)
                                {
                                    ((App)Application.Current).reportEvents.Add(new Execeptions(es));
                                    ((App)Application.Current).reportCrashandAnalytics.SendEvent(((App)Application.Current).reportEvents, SettingsHelper.totalAppSettings.ClientID, false);
                                    ((App)Application.Current).reportEvents.Clear();
                                }
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
                            if (SettingsHelper.totalAppSettings.Reporting)
                            {
                                ((App)Application.Current).reportEvents.Add(new Execeptions(es));
                                ((App)Application.Current).reportCrashandAnalytics.SendEvent(((App)Application.Current).reportEvents, SettingsHelper.totalAppSettings.ClientID, false);
                                ((App)Application.Current).reportEvents.Clear();
                            }
                            es = null;
                            loc += 1;
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
                    StorageFile appsFile = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("finalapps.json", CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(appsFile, saveappsstring);
                }
                if (saveFolders.Count > 0)
                {
                    StorageFile appsFile = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("folders.json", CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(appsFile, savefolderstring);
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

                            }
                        }
                    }
                }
            }
            //searchApps = new ReadOnlyObservableCollection<Apps>(new ObservableCollection<Apps>(listofApps.OrderBy(x => x.Name)));

            Apps = new AppPaginationObservableCollection(listOfApps.OrderBy(x => x.Name));
            // SearchApps = listOfApps.OrderBy(x => x.Name).ToList();
            return;
        }
    }
}
