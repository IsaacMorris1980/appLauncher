// Methods for getting installed apps/games from the device are here. Note: Package = App/Game
using appLauncher.Core.Model;

using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public static class packageHelper
    {

        public static ReadOnlyObservableCollection<Apps> searchApps { get; private set; }
        public static AppPaginationObservableCollection Apps { get; set; }
        public static List<Apps> appsList { get; set; } = new List<Apps>();

        public static event EventHandler AppsRetreived;
        public static PageChangingVariables pageVariables { get; set; } = new PageChangingVariables();

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

        public static async Task LoadCollectionAsync()
        {
            List<Apps> listAppss = new List<Apps>();
            if (await packageHelper.IsFilePresent("collection.json"))
            {
                try
                {

                    StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("collection.json");
                    string apps = await Windows.Storage.FileIO.ReadTextAsync(item);
                    listAppss = JsonConvert.DeserializeObject<List<Apps>>(apps);
                }
                catch (Exception es)
                {
                    await CrashAnalyticsHelper.LoggingCrash(es);
                    await CrashAnalyticsHelper.LoggingAnalytics("Crashed during loading apps list");
                }
            }
            else
            {
                PackageManager packageManager = new PackageManager();
                IEnumerable<Package> appslist = packageManager.FindPackagesForUserWithPackageTypes("", PackageTypes.Main);
                foreach (Package item in appslist)
                {
                    try
                    {
                        Apps Apps = new Apps();
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
                                    await CrashAnalyticsHelper.LoggingCrash(es);
                                    await CrashAnalyticsHelper.LoggingAnalytics("App logo unable to be found");
                                    Apps.Name = item.DisplayName;
                                    Apps.FullName = item.Id.FullName;
                                    Apps.Description = item.Description;
                                    Apps.Developer = item.PublisherDisplayName;
                                    Apps.InstalledDate = item.InstalledDate;
                                    Apps.Tip = $"Name: {item.DisplayName}{Environment.NewLine}Developer: {item.PublisherDisplayName}{Environment.NewLine}Installed: {item.InstalledDate}";
                                    Apps.Logo = new byte[1];
                                    listAppss.Add(Apps);
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
                                listAppss.Add(Apps);
                            }
                            catch (Exception es)
                            {
                                await CrashAnalyticsHelper.LoggingCrash(es);
                                await CrashAnalyticsHelper.LoggingAnalytics("App logo unable to be found");
                                Apps.Name = item.DisplayName;
                                Apps.FullName = item.Id.FullName;
                                Apps.Description = item.Description;
                                Apps.Developer = item.PublisherDisplayName;
                                Apps.InstalledDate = item.InstalledDate;
                                Apps.Tip = $"Name: {item.DisplayName}{Environment.NewLine}Developer: {item.PublisherDisplayName}{Environment.NewLine}Installed: {item.InstalledDate}";
                                Apps.Logo = new byte[1];
                                listAppss.Add(Apps);
                                es = null;
                                continue;
                            }
                        }
                    }
                    catch (Exception es)
                    {

                        await CrashAnalyticsHelper.LoggingCrash(es);
                        await CrashAnalyticsHelper.LoggingAnalytics("Crashed during loading all apps");
                    }
                }
            }


            Apps = new AppPaginationObservableCollection(listAppss);
            searchApps = new ReadOnlyObservableCollection<Apps>(new ObservableCollection<Apps>(listAppss.OrderByDescending(x => x.Name).ToList()));
            AppsRetreived(true, EventArgs.Empty);
        }
        public static async Task SaveCollectionAsync()
        {
            try
            {
                List<Apps> savapps = packageHelper.Apps.GetOriginalCollection().ToList();
                var saveappsstring = JsonConvert.SerializeObject(savapps, Formatting.Indented);
                StorageFile appsfile = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("collection.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(appsfile, saveappsstring);
            }
            catch (Exception es)
            {
                await CrashAnalyticsHelper.LoggingCrash(es);
                await CrashAnalyticsHelper.LoggingAnalytics("Crashed during saving apps list to file");
            }
        }

        public static async Task<bool> LaunchApp(string fullname)
        {

            PackageManager pm = new PackageManager();
            Package pack = pm.FindPackageForUser("", fullname);
            IReadOnlyList<AppListEntry> listentry = await pack.GetAppListEntriesAsync();
            return await listentry[0].LaunchAsync();
        }

        public static async Task RescanForNewApplications()
        {
            List<Apps> listAppss = new List<Apps>();
            PackageManager packageManager = new PackageManager();
            IEnumerable<Package> appslist = packageManager.FindPackagesForUserWithPackageTypes("", PackageTypes.Main);
            foreach (Package item in appslist)
            {
                try
                {
                    Apps Applisted = new Apps();
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
                                Crashes.TrackError(es);
                                Applisted.Name = item.DisplayName;
                                Applisted.FullName = item.Id.FullName;
                                Applisted.Description = item.Description;
                                Applisted.Developer = item.PublisherDisplayName;
                                Applisted.InstalledDate = item.InstalledDate;
                                Applisted.Tip = $"Name: {item.DisplayName}{Environment.NewLine}Developer: {item.PublisherDisplayName}{Environment.NewLine}Installed: {item.InstalledDate}";
                                Applisted.Logo = new byte[1];
                                listAppss.Add(Applisted);
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
                            Applisted.Name = item.DisplayName;
                            Applisted.FullName = item.Id.FullName;
                            Applisted.Description = item.Description;
                            Applisted.Developer = item.PublisherDisplayName;
                            Applisted.InstalledDate = item.InstalledDate;
                            Applisted.Tip = $"Name: {item.DisplayName}{Environment.NewLine}Developer: {item.PublisherDisplayName}{Environment.NewLine}Installed: {item.InstalledDate}";
                            Applisted.Logo = temp;
                            listAppss.Add(Applisted);
                        }
                        catch (Exception es)
                        {
                            Analytics.TrackEvent("App logo unable to be found");
                            Crashes.TrackError(es);
                            Applisted.Name = item.DisplayName;
                            Applisted.FullName = item.Id.FullName;
                            Applisted.Description = item.Description;
                            Applisted.Developer = item.PublisherDisplayName;
                            Applisted.InstalledDate = item.InstalledDate;
                            Applisted.Tip = $"Name: {item.DisplayName}{Environment.NewLine}Developer: {item.PublisherDisplayName}{Environment.NewLine}Installed: {item.InstalledDate}";
                            Applisted.Logo = new byte[1];
                            listAppss.Add(Applisted);
                            es = null;
                            continue;
                        }
                    }

                }
                catch (Exception es)
                {

                    Crashes.TrackError(es);
                }
            }
            List<Apps> listofApps = Apps.GetOriginalCollection().ToList();
            if (listAppss.Count > listofApps.Count)
            {
                IEnumerable<Apps> a = listAppss.Where(x => !listofApps.Any(y => y.Name == x.Name)).ToList();
                foreach (var item in a)
                {
                    listofApps.Add(item);
                }

            }
            else if (listofApps.Count > listAppss.Count)
            {
                IEnumerable<Apps> a = listofApps.Where(x => !listAppss.Any(y => y.Name == x.Name)).ToList();
                foreach (var item in a)
                {
                    listofApps.Remove(item);
                }
            }
            searchApps = new ReadOnlyObservableCollection<Apps>(new ObservableCollection<Apps>(listofApps.OrderBy(x => x.Name)));
            Apps = new AppPaginationObservableCollection(listofApps.OrderBy(x => x.Name));
            return;

        }

    }
}
