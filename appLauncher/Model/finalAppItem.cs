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
    /// <summary>
    /// A class made up of the app list entry and the app logo of each app. This is what each app control displayed represents.
    /// </summary>
   public class finalAppItem
    {
        public AppListEntry appEntry { get; set; }
        public BitmapImage appLogo { get; set; }

        public static ObservableCollection<finalAppItem> listOfApps { get; set; }
        
        
        /// <summary>
        /// Gets installed apps from device and stores them in an ObservableCollection of finalAppItem, which can be accessed from anywhere.
        /// </summary>
        /// <returns></returns>
        public static async Task getApps()
        {
            bool isLoaded = false;
            await packageHelper.getAllAppsAsync();
            isLoaded = true;
            
        }

        //public static async Task getAppsForSplash()
        //{

        //    listOfApps = await packageHelper.getAllAppsAsync();
            
        //}
    }
}
