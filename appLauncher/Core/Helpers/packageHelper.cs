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
        public static PaginationObservableCollection AppTiles { get; set; }


        public static event EventHandler AppsRetreived; s

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
            List<Apps> listAppTiles = new List<Apps>();
            if (await packageHelper.IsFilePresent("collection.txt"))
            {
                try
                {

                    StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("collection.txt");
                    string apps = await Windows.Storage.FileIO.ReadTextAsync(item);
                    listAppTiles = JsonConvert.DeserializeObject<List<Apps>>(apps);
                    foreach (Apps oderedapp in listAppTiles)
                    {
                        PackageManager pm = new PackageManager();
                        Package singleapp = pm.FindPackage(oderedapp.FullName);
                        IReadOnlyList<AppListEntry> applistentries = await singleapp.GetAppListEntriesAsync();
                        oderedapp.AppsListEntry = applistentries[0];

                    }
                }
                catch (Exception e)
                {
                    Analytics.TrackEvent("Crashed during loading apps list to last");
                    Crashes.TrackError(e);
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
                            Apps.AppsListEntry = appsEntry[0];
                            Apps.Name = item.DisplayName;
                            Apps.FullName = item.Id.FullName;
                            Apps.Description = item.Description;
                            Apps.Developer = item.Id.Publisher;
                            Apps.InstalledDate = item.InstalledDate;
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
                                    Apps.Logo = new byte[1];
                                    listAppTiles.Add(Apps);
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
                                Apps.Logo = temp;
                                listAppTiles.Add(Apps);
                            }
                            catch (Exception es)
                            {
                                Analytics.TrackEvent("App logo unable to be found");
                                Crashes.TrackError(es);
                                Apps.Logo = new byte[1];
                                listAppTiles.Add(Apps);
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
            }


            AppTiles = new PaginationObservableCollection(listAppTiles);
            searchApps = new ReadOnlyObservableCollection<Apps>(new ObservableCollection<Apps>(listAppTiles.OrderByDescending(x => x.Name).ToList()));
            AppsRetreived(true, EventArgs.Empty);
        }

        public static async Task SaveCollectionAsync()
        {
            try
            {
                List<Apps> savapps = packageHelper.AppTiles.GetOriginalCollection().ToList();
                var te = JsonConvert.SerializeObject(savapps, Formatting.Indented);
                StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("collection.txt", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(item, te);
            }
            catch (Exception es)
            {
                Analytics.TrackEvent("Crashed during saving app list positions");
                Crashes.TrackError(es);
            }
        }



    }
}