// Methods for getting installed apps/games from the device are here. Note: Package = App/Game
using appLauncher.Core.Model;

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

        public static ReadOnlyObservableCollection<AppTile> searchApps { get; private set; }
        public static PaginationObservableCollection appTiles { get; set; }

        public static List<AppTile> appTilesList { get; set; } = new List<AppTile>();

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
            List<AppTile> listapptiles = new List<AppTile>();
            PackageManager packageManager = new PackageManager();
            IEnumerable<Package> appslist = packageManager.FindPackagesForUserWithPackageTypes("", PackageTypes.Main);

            foreach (Package item in appslist)
            {
                try
                {
                    IReadOnlyList<AppListEntry> t = await item.GetAppListEntriesAsync();
                    if (t.Count > 0)
                    {
                        AppTile apps = new AppTile(item, t[0]);
                        try
                        {
                            RandomAccessStreamReference logoStream;
                            try
                            {
                                logoStream = t[0].DisplayInfo.GetLogo(new Size(50, 50));
                            }
                            catch (Exception es)
                            {
                                Crashes.TrackError(es);
                                apps.Setlogo = new byte[1];
                                listapptiles.Add(apps);
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
                            apps.Setlogo = temp;
                            listapptiles.Add(apps);
                        }
                        catch (Exception es)
                        {
                            Crashes.TrackError(es);
                            apps.Setlogo = new byte[1];
                            listapptiles.Add(apps);
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

            if (await packageHelper.IsFilePresent("collection.txt"))
            {
                try
                {
                    List<AppTile> orderedapplist = new List<AppTile>();
                    StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("collection.txt");
                    string apps = await Windows.Storage.FileIO.ReadTextAsync(item);
                    orderedapplist = JsonConvert.DeserializeObject<List<AppTile>>(apps);
                    for (int i = 0; i < orderedapplist.Count(); i++)
                    {
                        int localtion = listapptiles.IndexOf(listapptiles.Find(x => x.appfullname == orderedapplist[i].appfullname));
                        if (localtion >= 0)
                        {
                            AppTile ap = listapptiles[localtion];
                            listapptiles.RemoveAt(localtion);
                            listapptiles.Insert(i, ap);
                        }

                    }
                }
                catch (Exception e)
                {
                    Crashes.TrackError(e);
                }
            }

            appTiles = new PaginationObservableCollection(listapptiles);
            searchApps = new ReadOnlyObservableCollection<AppTile>(new ObservableCollection<AppTile>(listapptiles.OrderByDescending(x => x.appfullname).ToList()));
            AppsRetreived(true, EventArgs.Empty);
        }
        public static async Task SaveCollectionAsync()
        {
            var te = JsonConvert.SerializeObject(packageHelper.appTiles.GetInternalList(), Formatting.Indented); ;
            StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("collection.txt", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(item, te);
        }

    }
}
