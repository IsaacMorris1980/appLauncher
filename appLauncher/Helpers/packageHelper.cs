using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Management.Deployment;

namespace appLauncher.Helpers
{
   public class packageHelper
   {
       public static PackageManager pkgManager = new PackageManager();
        public static ObservableCollection<AppListEntry> getAllApps()
        {

            var listOfInstalledPackages = pkgManager.FindPackagesForUserWithPackageTypes("", PackageTypes.Bundle);
            List<Package> packages = new List<Package>();
            packages = listOfInstalledPackages.ToList();
            ObservableCollection<AppListEntry> appListEntries = new ObservableCollection<AppListEntry>();
            int count = packages.Count();
            for (int i = 0; i < count; i++)
            {
                List<AppListEntry> singleAppListEntries = new List<AppListEntry>();
                Task<IReadOnlyList<AppListEntry>> getAppEntriesTask = packages[i].GetAppListEntriesAsync().AsTask();
                getAppEntriesTask.Wait();
                singleAppListEntries = getAppEntriesTask.Result.ToList();
                appListEntries.Add(singleAppListEntries[0]);
                
            }
            return appListEntries;
        }
   }
}
