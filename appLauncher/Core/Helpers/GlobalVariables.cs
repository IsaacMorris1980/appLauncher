using appLauncher.Core.CustomEvent;
using appLauncher.Core.Model;

using Windows.Foundation;
using Windows.Storage;

namespace appLauncher.Core.Helpers
{
    public static class GlobalVariables
    {
        public static int appsperscreen { get; private set; }
        public static DraggedItem itemdragged { get; set; } = new DraggedItem();
        public static int columns { get; set; }
        public static int oldindex { get; set; }
        public static int newindex { get; set; }
        public static int pagenum { get; private set; }
        public static int NumofPages { get; private set; }
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
                PageNumChanged?.Invoke(new PageChangedEventArgs(apppagenum));
            }

        }
        public static void SetPageSize(int appnumperscreen)
        {
            appsperscreen = appnumperscreen;
            NumofApps?.Invoke(new AppPageSizeChangedEventArgs(appnumperscreen));
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

