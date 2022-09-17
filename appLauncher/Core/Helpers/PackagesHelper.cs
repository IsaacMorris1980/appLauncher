using appLauncher.Core.Model;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Management.Deployment;

namespace appLauncher.Core.Helpers
{
    public static class PackagesHelper
    {
        public static async Task<List<AppTile>> GetAllAppsAsync()
        {
            List<AppTile> tiles = new List<AppTile>();
            PackageManager packageManager = new PackageManager();
            IEnumerable<Package> appslist = packageManager.FindPackagesForUserWithPackageTypes("", PackageTypes.Main);
            foreach (Package item in appslist)
            {
                IReadOnlyList<AppListEntry> t = await item.GetAppListEntriesAsync();
                if (t.Count > 0)
                {
                    tiles.Add(new AppTile(item, t[0]));
                }
            }
            return tiles;

        }
        public static Package GetPackage(string appfullename)
        {
            PackageManager pkg = new PackageManager();
            Package p = pkg.FindPackage(appfullename);

            return p;
        }
        public static async Task<AppListEntry> GetAppliestEntry(Package pack)
        {
            IReadOnlyList<AppListEntry> entries = await pack.GetAppListEntriesAsync();
            return entries[0];
        }


    }
}
