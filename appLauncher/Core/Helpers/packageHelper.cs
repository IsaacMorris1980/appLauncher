// Methods for getting installed apps/games from the device are here. Note: Package = App/Game
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Management.Deployment;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;

namespace appLauncher.Core.Helpers
{
    public static class packageHelper
    {



        public static PackageManager pkgManager = new PackageManager();
        /// <summary>
        /// Gets app package and image and returns them as a new "finalAppItem" which will be used for the app control template.
        /// <para>WARNING: Only use this method when it's REQUIRED. Otherwise use the Async version below this one.</para>
        /// </summary>


        public static ObservableCollection<finalAppItem> getAllApps()
        {
            var listOfInstalledPackages = pkgManager.FindPackagesForUserWithPackageTypes("", PackageTypes.Main);
            List<Package> allPackages = new List<Package>();
            List<Package> packages = new List<Package>();
            allPackages = listOfInstalledPackages.ToList();
            int count = allPackages.Count();

            for (int i = 0; i < count; i++)
            {

                packages.Add(allPackages[i]);


            }


            ObservableCollection<finalAppItem> finalAppItems = new ObservableCollection<finalAppItem>();

            count = packages.Count();

            //Loop will get app entry and logo for each app and create finalAppItem objects with that data.
            for (int i = 0; i < count; i++)
            {
                try
                {
                    List<AppListEntry> singleAppListEntries = new List<AppListEntry>();
                    Task<IReadOnlyList<AppListEntry>> getAppEntriesTask = packages[i].GetAppListEntriesAsync().AsTask();
                    getAppEntriesTask.Wait();
                    singleAppListEntries = getAppEntriesTask.Result.ToList();


                    BitmapImage logo = new BitmapImage();
                    var logoStream = singleAppListEntries[0].DisplayInfo.GetLogo(new Size(50, 50));
                    Task<IRandomAccessStreamWithContentType> logoStreamTask = logoStream.OpenReadAsync().AsTask();
                    logoStreamTask.Wait();
                    IRandomAccessStreamWithContentType logoStreamResult = logoStreamTask.Result;
                    logo.SetSource(logoStreamResult);
                    finalAppItems.Add(new finalAppItem
                    {
                        appEntry = singleAppListEntries[0],
                        appLogo = logo
                    });
                }

                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            return finalAppItems;
        }

        public static event EventHandler AppsRetreived;


        /// <summary>
        /// Gets app package and image and returns them as a new "finalAppItem" asynchronously, which will then be used for the app control template.
        /// <para> Of the two getAllApps() methods, this is the preferred version because it doesn't block the stop the rest of the app from running when 
        /// this is being run.</para>
        /// </summary>
        /// <returns></returns>

        public static async Task getAllAppsAsync()
        {
            List<KeyValuePair<AppListEntry, Package>> someapps = new List<KeyValuePair<AppListEntry, Package>>();
            var listOfInstalledPackages = pkgManager.FindPackagesForUserWithPackageTypes("", PackageTypes.Main);
            List<Package> allPackages = new List<Package>();
            List<Package> packages = new List<Package>();
            allPackages = listOfInstalledPackages.ToList();
            int count = allPackages.Count();
            ObservableCollection<finalAppItem> finalAppItems = new ObservableCollection<finalAppItem>();
            //count = packages.Count();
            for (int i = 0; i < count; i++)
            {
                try

                {
                    List<AppListEntry> singleAppListEntries = new List<AppListEntry>();


                    var appListEntries = await allPackages[i].GetAppListEntriesAsync();
                    Package p = allPackages[i];

                    singleAppListEntries = appListEntries.ToList();
                    if (singleAppListEntries.Count > 0)
                    {
                        Debug.WriteLine("YES!");
                    }
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low, async () =>
                    {
                        try
                        {
                            BitmapImage logo = new BitmapImage();

                            RandomAccessStreamReference logoStream = singleAppListEntries[0].DisplayInfo.GetLogo(new Size(50, 50));

                            IRandomAccessStreamWithContentType whatIWant = await logoStream.OpenReadAsync();

                            logo.SetSource(whatIWant);

                            finalAppItem itemToAdd = new finalAppItem();

                            itemToAdd.appEntry = singleAppListEntries[0];
                            itemToAdd.appPackage = p;
                            itemToAdd.appName = singleAppListEntries[0].DisplayInfo.DisplayName;
                            itemToAdd.appLogo = logo;
                            finalAppItems.Add(itemToAdd);
                        }

                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message);
                        }
                    });
                }

                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }

            }
            AllApps.listOfApps = finalAppItems;

        }
    }
}
