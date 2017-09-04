using appLauncher.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Management.Deployment;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace appLauncher.Helpers
{
   public class packageHelper
   {
       public static PackageManager pkgManager = new PackageManager();
        public  static ObservableCollection<finalAppItem> getAllApps()
        {

            var listOfInstalledPackages = pkgManager.FindPackagesForUserWithPackageTypes("", PackageTypes.Main);
            List<Package> allPackages = new List<Package>();
            List<Package> packages = new List<Package>();
            allPackages = listOfInstalledPackages.ToList();
            int count = allPackages.Count();

            for (int i = 0; i < count; i++)
            {
                if (allPackages[i].SignatureKind == PackageSignatureKind.Store)
                {
                    packages.Add(allPackages[i]);
                }

            }


            ObservableCollection<finalAppItem> finalAppItems = new ObservableCollection<finalAppItem>();
             count = packages.Count();
            for (int i = 0; i < count; i++)
            {
                List<AppListEntry> singleAppListEntries = new List<AppListEntry>();
                Task<IReadOnlyList<AppListEntry>> getAppEntriesTask = packages[i].GetAppListEntriesAsync().AsTask();
                getAppEntriesTask.Wait();
                singleAppListEntries = getAppEntriesTask.Result.ToList();
                var logoStream =  singleAppListEntries[0].DisplayInfo.GetLogo(new Size(50, 50));
                BitmapImage logo = new BitmapImage();
                Task<IRandomAccessStreamWithContentType> stream = logoStream.OpenReadAsync().AsTask();
                stream.Wait();
                IRandomAccessStreamWithContentType whatIWant = stream.Result;
                logo.SetSource(whatIWant);
                finalAppItems.Add(new finalAppItem
                {
                    appEntry = singleAppListEntries[0],
                    appLogo = logo
                });
            }
            return finalAppItems;
        }
   }
}
