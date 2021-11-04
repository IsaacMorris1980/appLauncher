// Methods for getting installed apps/games from the device are here. Note: Package = App/Game

using Newtonsoft.Json;
using Microsoft.AppCenter.Crashes;
using Swordfish.NET.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Management.Deployment;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using appLauncher.Core.Model;


namespace appLauncher.Core.Helpers
{
    public static class packageHelper

    {
        public static PackageManager pkgManager = new PackageManager();
        public static event EventHandler AppsRetreived;
        public static ConcurrentObservableCollection<AppTile> Bags { set; get; } = new ConcurrentObservableCollection<AppTile>();
        public static int appsperscreen { get; set; }
        public static AppTile itemdragged { get; set; }
        public static int columns { get; set; }
        public static int oldindex { get; set; }
        public static int newindex { get; set; }
        public static int pagenum { get; set; }
        public static bool isdragging { get; set; }

        private static StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        public static Windows.Foundation.Point startingpoint { get; set; }
        
        public static int NumofRoworColumn(int padding, int objectsize, int sizetofit)
        {
            int amount = 0;
            int intsize = objectsize + (padding + padding);
            int size = intsize;
            while (size + intsize < sizetofit)
            {
                amount += 1;
                size += intsize;
            }
            return amount;
        }

       public static async Task SaveCollectionAsync()
        {
            Dictionary<string, List<string>> info = new Dictionary<string, List<string>>();
            if (Bags.Count() > 0)
            {
                try
                {
                    foreach (AppTile items in Bags)
                    {
                        List<string> appitems = new List<string>();
                        appitems.Add(ColorHelper.ToDisplayName(items.AppTileBackgroundcolor));
                        appitems.Add(ColorHelper.ToDisplayName(items.AppTileForegroundcolor));
                        appitems.Add(Convert.ToString(items.AppTileBackgroundOpacity));
                        appitems.Add(Convert.ToString(items.AppTileForegroundOpacity));
                        info.Add(items.AppName, appitems);
                        await logHelper.Log(items.ToString());
                    }
                    var te = JsonConvert.SerializeObject(info, Formatting.Indented);
                    StorageFile item = await ApplicationData.Current.LocalFolder.CreateFileAsync("collection.txt", CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(item, te);
                }
                catch (Exception e)
                {

                    Crashes.TrackError(e);
                }
            }
        }
      
      
      
        /// <summary>
        /// Gets app package and image and returns them as a new "finalAppItem" asynchronously, which will then be used for the app control template.
        /// <para> Of the two getAllApps() methods, this is the preferred version because it doesn't block the stop the rest of the app from running when 
        /// this is being run.</para>
        /// </summary>
        /// <returns></returns>

        public static async Task getAllAppsAsync()
        {
            await getAppsFromSystem();
            if (await logHelper.IsFilePresent("collection.txt"))
            {
                StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("collection.txt");
                var apps = await FileIO.ReadLinesAsync(item);
                var ab = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(await FileIO.ReadTextAsync(item));
                int val1 = 0;
                try
                {
                    foreach (KeyValuePair<string, List<string>> items in ab)
                    {
                        AppTile ap = Bags.FirstOrDefault(x => x.AppName == items.Key);
                        int val2 = Bags.IndexOf(ap);
                        Bags.RemoveAt(val2);
                        ap.AppTileBackgroundcolor = logHelper.FromName(items.Value[0]);
                        ap.AppTileForegroundcolor = logHelper.FromName(items.Value[1]);
                        ap.AppTileBackgroundOpacity = Convert.ToDouble(items.Value[2]);
                        ap.AppTileForegroundOpacity = Convert.ToDouble(items.Value[3]);
                        Bags.Insert(val1, ap);
                        val1 += 1;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    Crashes.TrackError(e);
                }
            }
            bool yes = true;
            AppsRetreived?.Invoke(yes, EventArgs.Empty);
        }

        public static async Task getAppsFromSystem()
        {
            var listOfInstalledPackages = pkgManager.FindPackagesForUserWithPackageTypes(string.Empty,PackageTypes.Main);
            List<Package> allPackages = listOfInstalledPackages.ToList();
            RandomAccessStreamReference logoStream = null;
            byte[] logos = null;
          for (int x=0;x<allPackages.Count();x++)
            {
                Package it = allPackages[x];
                var applistentries = await it.GetAppListEntriesAsync();
                   if (applistentries.Count > 0)
                    {
                       
                    try
                    {
                       var te = applistentries.ToList();
                        logoStream = te[0].DisplayInfo.GetLogo(new Size(50, 50));
                        IRandomAccessStreamWithContentType whatIWant = await logoStream.OpenReadAsync();
                        byte[] temp = new byte[whatIWant.Size];
                        using (DataReader read = new DataReader(whatIWant.GetInputStreamAt(0)))
                        {
                            await read.LoadAsync((uint)whatIWant.Size);
                            read.ReadBytes(temp);
                        }
                        logos = temp;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString());
                     //   await Logging.Log(e.ToString());
                        Crashes.TrackError(e);

                    }
                    Bags.Add(new AppTile
                    {
                        AppListentry = applistentries.ToList()[0],
                        Pack = it,
                        AppTileBackgroundOpacity = 1,
                        AppTileForegroundOpacity = .5,
                        AppTileBackgroundcolor = Colors.Black,
                        AppTileForegroundcolor = Colors.Red,
                        appLogo = logos.Length >= 0 ? logos : new byte[0]
                    });
                   await logHelper.Log(it.DisplayName);
            }
                            
            }
        }

        
    }
}