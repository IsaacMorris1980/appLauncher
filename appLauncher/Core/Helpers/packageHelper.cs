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

        public static ReadOnlyObservableCollection<AppTile> searchApps { get; private set; }
        public static ObservableCollection<AppTile> appTiles { get; set; }
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
                                apps.appTileSetlogo = new byte[1];
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
                            apps.appTileSetlogo = temp;
                            listapptiles.Add(apps);
                        }
                        catch (Exception es)
                        {
                            Analytics.TrackEvent("App logo unable to be found");
                            Crashes.TrackError(es);
                            apps.appTileSetlogo = new byte[1];
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
                        int localtion = listapptiles.IndexOf(listapptiles.Find(x => x.appTileFullName == orderedapplist[i].appTileFullName));
                        if (localtion >= 0)
                        {
                            AppTile ap = listapptiles[localtion];
                            ap.appTileBackgroundColor = orderedapplist[i].appTileBackgroundColor;
                            ap.appTileBackgroundOpacity = orderedapplist[i].appTileBackgroundOpacity;
                            ap.appTileLogoColor = orderedapplist[i].appTileLogoColor;
                            ap.appTileLogoOpacity = orderedapplist[i].appTileLogoOpacity;
                            ap.appTileTextColor = orderedapplist[i].appTileTextColor;
                            ap.appTileTextOpacity = orderedapplist[i].appTileTextOpacity;


                            listapptiles.RemoveAt(localtion);
                            listapptiles.Insert(i, ap);
                        }

                    }
                }
                catch (Exception e)
                {
                    Analytics.TrackEvent("Crashed during reordering app list to last apps position");
                    Crashes.TrackError(e);
                }
            }

            appTiles = new ObservableCollection<AppTile>(listapptiles);
            searchApps = new ReadOnlyObservableCollection<AppTile>(new ObservableCollection<AppTile>(listapptiles.OrderByDescending(x => x.appTileFullName).ToList()));
            AppsRetreived(true, EventArgs.Empty);
        }
        public static async Task SaveCollectionAsync()
        {
            try
            {
                List<AppTile> savapps = packageHelper.appTiles.ToList();
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
