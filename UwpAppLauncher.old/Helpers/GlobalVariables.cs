using System;
using System.Threading.Tasks;

using Windows.Storage;

namespace UwpAppLauncher.Helpers
{
    public sealed class GlobalVariables
    {
        public int currentpage { get; set; }
        public async Task<bool> IsFilePresent(string fileName, string folderpath = "")
        {
            IStorageItem item;
            if (folderpath == "")
            {
                item = await ApplicationData.Current.LocalFolder.TryGetItemAsync(fileName);
            }
            else
            {
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderpath);
                item = await folder.TryGetItemAsync(fileName);
            }
            return item != null;
        }


    }
}
