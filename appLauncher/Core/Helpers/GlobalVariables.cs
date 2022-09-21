using appLauncher.Core.CustomEvent;

using appLauncher.Core.Model;

using System;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.Storage;

namespace appLauncher
{
    public static class GlobalVariables
    {
        public static int appsperscreen { get; private set; }
        public static AppTile itemdragged { get; set; }
        public static int columns { get; set; }
        public static int oldindex { get; set; }
        public static int newindex { get; set; }
        public static int pagenum { get; private set; }
        public static bool isdragging { get; set; }
        public static bool bgimagesavailable { get; set; }

        private static StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        public static Point startingpoint { get; set; }

        public static event PageChangedDelegate PageNumChanged;

        public static event AppPageSizeChangedDelegate NumofApps;

        public static void SetPageNumber(int apppagenum)
        {
            if (pagenum != apppagenum)
            {
                pagenum = apppagenum;
                PageNumChanged.Invoke(new PageChangedEventArgs(apppagenum));
            }

        }
        public static void SetPageSize(int appnumperscreen)
        {
            appsperscreen = appnumperscreen;
            NumofApps.Invoke(new AppPageSizeChangedEventArgs(appnumperscreen));
        }

        public static async Task Logging(string texttolog)
        {
            StorageFolder stors = ApplicationData.Current.LocalFolder;
            await FileIO.AppendTextAsync(await stors.CreateFileAsync("logfile.txt", CreationCollisionOption.OpenIfExists), texttolog);
            await FileIO.AppendTextAsync(await stors.CreateFileAsync("logfile.txt", CreationCollisionOption.OpenIfExists), Environment.NewLine);
        }
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



    }

}

