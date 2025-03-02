using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;
using appLauncher.Core.Pages;

using Windows.Storage;
using Windows.System.Threading;
using Windows.UI;

namespace appLauncher.Core.Services
{
    public class ImageService : IServices
    {
        private bool loadFile;
        private ThreadPoolTimer _updateBackBrush;
        private int selectedIndex;
        private ObservableCollection<PageBackgrounds> _backgrounds;
        public ObservableCollection<PageBackgrounds> BackGrounds
        {
            get
            {
                if (_backgrounds==null|| _backgrounds.Count()==0)
                {
                   
                    return null;
                }
                return _backgrounds;
            }
            set
            {
                _backgrounds = value;
            }
        }
        public Task Load()
        {
            throw new NotImplementedException();
        }
        public Task Save()
        {
            throw new NotImplementedException();
        }
        public Task Reload()
        {
            throw new NotImplementedException();
        }
        public static async Task<bool> IsFilePresent(string fileName, string folderPath = "")
        {
            IStorageItem item;
            if (folderPath == "")
            {
                item = await ApplicationData.Current.LocalFolder.TryGetItemAsync(fileName);
            }
            else
            {
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
                item = await folder.TryGetItemAsync(fileName);
            }

            return item != null;

        }
    }
}
