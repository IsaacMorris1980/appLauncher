using appLauncher.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace appLauncher.Model
{
   public class finalAppItem
    {
        public AppListEntry appEntry { get; set; }
        public BitmapImage appLogo { get; set; }

        public static ObservableCollection<finalAppItem> listOfApps = new ObservableCollection<finalAppItem>();

        public static async Task<bool> getApps()
        {
            bool isLoaded = false;
            listOfApps = await packageHelper.getAllAppsAsync();
            isLoaded = true;
            return isLoaded;
        }
    }
}
