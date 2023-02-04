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
        public static PaginationObservableCollection appTiles { get; set; }
        public static List<Apps> appTilesList { get; set; } = new List<Apps>();

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

        public static async Task LoadCollectionAsync()
        {
            List<Apps> listapptiles = new List<Apps>();
            if (await packageHelper.IsFilePresent("collection.txt"))
            {
                try
                {

                    StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("collection.txt");
                    string apps = await Windows.Storage.FileIO.ReadTextAsync(item);
                    listapptiles = JsonConvert.DeserializeObject<List<Apps>>(apps);
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
                        Apps apptile = new Apps();
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
                                    apptile.Name = item.DisplayName;
                                    apptile.FullName = item.Id.FullName;
                                    apptile.Description = item.Description;
                                    apptile.Developer = item.Id.Publisher;
                                    apptile.InstalledDate = item.InstalledDate;
                                    apptile.Logo = new byte[1];
                                    listapptiles.Add(apptile);
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
                                apptile.Name = item.DisplayName;
                                apptile.FullName = item.Id.FullName;
                                apptile.Description = item.Description;
                                apptile.Developer = item.Id.Publisher;
                                apptile.InstalledDate = item.InstalledDate;
                                apptile.Logo = temp;
                                listapptiles.Add(apptile);
                            }
                            catch (Exception es)
                            {
                                Analytics.TrackEvent("App logo unable to be found");
                                Crashes.TrackError(es);
                                apptile.Name = item.DisplayName;
                                apptile.FullName = item.Id.FullName;
                                apptile.Description = item.Description;
                                apptile.Developer = item.Id.Publisher;
                                apptile.InstalledDate = item.InstalledDate;
                                apptile.Logo = new byte[1];
                                listapptiles.Add(apptile);
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


            appTiles = new PaginationObservableCollection(listapptiles);
            searchApps = new ReadOnlyObservableCollection<Apps>(new ObservableCollection<Apps>(listapptiles.OrderByDescending(x => x.Name).ToList()));
            AppsRetreived(true, EventArgs.Empty);
        }
        public static async Task SaveCollectionAsync()
        {
            try
            {
                List<Apps> savapps = packageHelper.appTiles.GetOriginalCollection().ToList();
                var te = JsonConvert.SerializeObject(savapps, Formatting.Indented); ;
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
