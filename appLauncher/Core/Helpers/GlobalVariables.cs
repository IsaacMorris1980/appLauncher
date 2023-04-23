using appLauncher.Core.CustomEvent;
using appLauncher.Core.Model;

using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.Storage;

namespace appLauncher.Core.Helpers
{
    public static class GlobalVariables
    {
        public static int appsperscreen { get; private set; }
        public static DraggedItem itemdragged { get; set; } = new DraggedItem();
        public static int columns { get; set; }
        public static int pagenum { get; private set; }
        public static int numOfPages { get; private set; }
        public static bool isdragging { get; set; }

        private static StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        public static Point startingpoint { get; set; }

        public static event PageChangedDelegate PageNumChanged;

        public static event AppPageSizeChangedDelegate NumofApps;
        public static event PageNumChangedDelegate NumofPagesChanged;


        public static void SetNumOfPages(int appnumOfPages)
        {
            numOfPages = appnumOfPages;
            NumofPagesChanged?.Invoke(new PageNumChangedArgs(appnumOfPages));
        }

        public static void SetPageNumber(int apppagenum)
        {
            if (pagenum != apppagenum)
            {
                pagenum = apppagenum;
                PageNumChanged?.Invoke(new PageChangedEventArgs(apppagenum));
            }

        }
        public static void SetPageSize(int appnumperscreen)
        {
            if (appsperscreen != appnumperscreen)
            {
                appsperscreen = appnumperscreen;
                NumofApps?.Invoke(new AppPageSizeChangedEventArgs(appnumperscreen));
            }


        }
        public static async Task LoggingCrashesAsync(Exception crashtostore)
        {
            Debug.WriteLine(crashtostore.ToString());
            StorageFile errorfile = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("errors.json", CreationCollisionOption.OpenIfExists);
            string errorstr = crashtostore.ToString() + Environment.NewLine + Environment.NewLine;
            await FileIO.WriteTextAsync(errorfile, errorstr);
        }
        public static async Task LogginAnalyticsAsync(string analyticstostore)
        {
            Debug.WriteLine(analyticstostore.ToString());
            StorageFile analyticsfile = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("analytics.json", CreationCollisionOption.OpenIfExists);
            string analyticsstr = analyticstostore.ToString() + Environment.NewLine + Environment.NewLine;
            await FileIO.WriteTextAsync(analyticsfile, analyticsstr);
        }
        public static int NumofRoworColumn(int padding, int objectsize, int sizetofit)
        {
            int amount = 0;
            int intsize = objectsize + (padding + padding);
            int size = intsize;
            while (size <= sizetofit)
            {
                amount += 1;
                size += intsize;
            }
            return amount;

        }



    }


}

