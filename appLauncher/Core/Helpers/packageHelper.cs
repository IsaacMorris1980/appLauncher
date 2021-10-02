// Methods for getting installed apps/games from the device are here. Note: Package = App/Game

using applauncher.mobile.Core.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Management.Deployment;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Concurrent.Collections;

namespace appLauncher.mobile.Core.Helpers
{
    public static class packageHelper
    {
        public static PackageManager pkgManager = new PackageManager();

        public static event EventHandler AppsRetreived;
        public static ConcurrentObservableCollection<AppTile> bags { set; get; } = new ConcurrentObservableCollection<AppTile>();
        private static List<AppTile> savedapps { get; set; }
        public static async Task<bool> LaunchAsync(string AppFullName)
        {
            var packages = await packageHelper.pkgManager.FindPackage(AppFullName).GetAppListEntriesAsync();
            return await packages[0].LaunchAsync();
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
            if (await GlobalVariables.IsFilePresent("collection.txt"))
            {
                StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("collection.txt");
                var apps = await FileIO.ReadLinesAsync(item);
                savedapps = JsonConvert.DeserializeObject<List<AppTile>>(await FileIO.ReadTextAsync(item));

                for (int i = 0; i < savedapps.Count() - 1; i++)
                {
                        var a = bags.IndexOf(savedapps[i]);
                        bags.RemoveAt(bags.IndexOf(savedapps[i]));
                        bags.Insert(i, savedapps[i]);
                    }

                bool yes = true;
                AppsRetreived?.Invoke(yes, EventArgs.Empty);
            }
            else
            {
          await getAppsFromSystem();
               
                //foreach (Package item in allPackages)
                //{
                //    try

                //    {
                //        var appListEntries = await item.GetAppListEntriesAsync();
                //        if (appListEntries.Count > 0)
                //        {
                //            Debug.WriteLine("YES!");
                //        }
                //        await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low, async () =>
                //        {
                //            //UI code here
                //            try
                //            {
                //                var logoStream = appListEntries[0].DisplayInfo.GetLogo(new Size(50, 50));
                //                IRandomAccessStreamWithContentType whatIWant = await logoStream.OpenReadAsync();
                //                byte[] temp = new byte[whatIWant.Size];
                //                using (DataReader read = new DataReader(whatIWant.GetInputStreamAt(0)))
                //                {
                //                    await read.LoadAsync((uint)whatIWant.Size);
                //                    read.ReadBytes(temp);
                //                }

                //                GlobalVariables.Apps.Add(new AppTile
                //                {
                //                    AppDeveloper = item.Id.Publisher,
                //                    AppFullName = item.Id.FullName,
                //                    AppName = appListEntries[0].DisplayInfo.DisplayName,
                //                    AppInstalled = item.InstalledDate,
                //                    AppTileOpacity = 1,
                //                    AppTileBackgroundcolor = Colors.Transparent,
                //                    AppTileForgroundcolor = Colors.Blue,
                //                    appLogo = temp

                //                });

                //            }

                //            catch (Exception e)
                //            {
                //                Debug.WriteLine(e.Message);
                //            }
                //        });


                //    }

                //    catch (Exception e)
                //    {
                //        Debug.WriteLine(e.Message);
                //    }
                //}
                bool yes = true;
                AppsRetreived?.Invoke(yes, EventArgs.Empty);
            }
        }

        public static async Task getAppsFromSystem()
        {
            var listOfInstalledPackages = pkgManager.FindPackagesForUserWithPackageTypes("", PackageTypes.Main);
            List<Package> allPackages = listOfInstalledPackages.ToList();
            Parallel.ForEach(allPackages, async (currentpackage) =>
            {
                var applistentries = await currentpackage.GetAppListEntriesAsync();
                try
                {
                    var logoStream = applistentries[0].DisplayInfo.GetLogo(new Size(50, 50));
                    IRandomAccessStreamWithContentType whatIWant = await logoStream.OpenReadAsync();
                    byte[] temp = new byte[whatIWant.Size];
                    using (DataReader read = new DataReader(whatIWant.GetInputStreamAt(0)))
                    {
                        await read.LoadAsync((uint)whatIWant.Size);
                        read.ReadBytes(temp);
                    }

                    bags.Add(new AppTile
                    {
                        AppDeveloper = currentpackage.Id.Publisher,
                        AppFullName = currentpackage.Id.FullName,
                        AppName = applistentries[0].DisplayInfo.DisplayName,
                        AppInstalled = currentpackage.InstalledDate,
                        AppTileOpacity = 1,
                        AppTileBackgroundcolor = Colors.Transparent,
                        AppTileForgroundcolor = Colors.Blue,
                        appLogo = temp

                    });


                }

                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            });
         
        }
        public static async Task<Byte[]>ReturnImage(StorageFile filename)
        {
            var logoStream = RandomAccessStreamReference.CreateFromFile(filename);
            IRandomAccessStreamWithContentType whatIWant = await logoStream.OpenReadAsync();
            byte[] temp = new byte[whatIWant.Size];
            using (DataReader read = new DataReader(whatIWant.GetInputStreamAt(0)))
            {
                await read.LoadAsync((uint)whatIWant.Size);
                read.ReadBytes(temp);
            }
            return temp;
        }
    }
}
