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
        public static int _appsPerScreen { get; private set; }
        public static DraggedItem _Itemdragged { get; set; } = new DraggedItem();
        public static int _columns { get; set; }
        public static int _pageNum { get; private set; }
        public static int _numOfPages { get; private set; }
        public static bool _isDragging { get; set; }
        public static Point _startingPoint { get; set; }

        public static event PageChangedDelegate PageNumChanged;

        public static event AppPageSizeChangedDelegate NumofApps;
        public static event PageNumChangedDelegate NumofPagesChanged;


        public static void SetNumOfPages(int appNumOfPages)
        {
            _numOfPages = appNumOfPages;
            NumofPagesChanged?.Invoke(new PageNumChangedArgs(appNumOfPages));
        }

        public static void SetPageNumber(int appPageNum)
        {
            if (_pageNum != appPageNum)
            {
                _pageNum = appPageNum;
                PageNumChanged?.Invoke(new PageChangedEventArgs(appPageNum));
            }

        }
        public static void SetPageSize(int appNumPerScreen)
        {
            if (_appsPerScreen != appNumPerScreen)
            {
                _appsPerScreen = appNumPerScreen;
                NumofApps?.Invoke(new AppPageSizeChangedEventArgs(appNumPerScreen));
            }


        }
        public static async Task LoggingCrashesAsync(Exception crashToStore)
        {
            Debug.WriteLine(crashToStore.ToString());
            StorageFile errorFile = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("errors.json", CreationCollisionOption.OpenIfExists);
            string errorStr = crashToStore.ToString() + Environment.NewLine + Environment.NewLine;
            await FileIO.WriteTextAsync(errorFile, errorStr);
        }
        public static async Task LogginAnalyticsAsync(string analyticsToStore)
        {
            Debug.WriteLine(analyticsToStore.ToString());
            StorageFile analyticsFile = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("analytics.json", CreationCollisionOption.OpenIfExists);
            string analyticsStr = analyticsToStore.ToString() + Environment.NewLine + Environment.NewLine;
            await FileIO.WriteTextAsync(analyticsFile, analyticsStr);
        }
        public static int NumofRoworColumn(int padding, int objectSize, int sizeToFit)
        {
            int amount = 0;
            int intSize = objectSize + (padding + padding);
            int size = intSize;
            while (size <= sizeToFit)
            {
                amount += 1;
                size += intSize;
            }
            return amount;

        }



    }


}

