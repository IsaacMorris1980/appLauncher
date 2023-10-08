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
        public static int _pageNum { get; set; }
        public static int _numOfPages { get; private set; }
        public static bool _isDragging { get; set; }
        public static Point _startingPoint { get; set; }


        public static async Task LoggingCrashesAsync(Exception crashToStore)
        {
            Debug.WriteLine(crashToStore.ToString());
            StorageFile errorFile = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("errors.json", CreationCollisionOption.OpenIfExists);
            string errorStr = crashToStore.ToString() + Environment.NewLine + Environment.NewLine;
            await FileIO.WriteTextAsync(errorFile, errorStr);
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

