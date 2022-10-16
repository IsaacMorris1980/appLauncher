using appLauncher.Core.Brushes;
using appLauncher.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;

namespace appLauncher.Model
{
    /// <summary>
    /// A class made up of the app list entry and the app logo of each app. This is what each app control displayed represents.
    /// </summary>
    public class finalAppItem
    {
        public AppListEntry appEntry { get; set; }
        public Package appPack { get; set; }
        public string appLogo { get; set; }
        public MaskedBrush maskedbrush()
        {
            var a = Convert.FromBase64String(this.appLogo);
            var b = a.AsBuffer().AsStream().AsRandomAccessStream();
            return new MaskedBrush(b, AppTileColor.foregroundColor);
        }
        public SolidColorBrush textcolor()
        {
            return new SolidColorBrush(AppTileColor.foregroundColor);
        }
        public SolidColorBrush backgroundcolor()
        {
            return new SolidColorBrush(AppTileColor.backgroundColor);
        }
        public double foregroundOpacity()
        {
            return AppTileColor.foregroundOpacity;
        }
        public double backgroundOpacity()
        {
            return AppTileColor.backgroundOpacity;
        }
        public async Task SetLogo()
        {
            RandomAccessStreamReference logoStream = this.appEntry.DisplayInfo.GetLogo(new Size(50, 50));
            IRandomAccessStreamWithContentType whatIWant = await logoStream.OpenReadAsync();
            byte[] temp = new byte[whatIWant.Size];
            using (DataReader read = new DataReader(whatIWant.GetInputStreamAt(0)))
            {
                await read.LoadAsync((uint)whatIWant.Size);
                read.ReadBytes(temp);
            }
            this.appLogo = Convert.ToBase64String(temp);
        }

    }


    public static class AllApps
    {
        public static ObservableCollection<finalAppItem> listOfApps { get; set; }
        public static List<KeyValuePair<AppListEntry, Package>> Allpackages { get; set; }


        /// <summary>
        /// Gets installed apps from device and stores them in an ObservableCollection of finalAppItem, which can be accessed from anywhere.
        /// </summary>
        /// <returns></returns>
        public static async Task getApps()
        {
            await packageHelper.getAllAppsAsync();
        }

        //public static async Task getAppsForSplash()
        //{

        //    listOfApps = await packageHelper.getAllAppsAsync();

        //}
    }
}
