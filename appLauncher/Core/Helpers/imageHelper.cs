using appLauncher.Core.Brushes;
using appLauncher.Core.Model;



using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.Threading;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;

namespace appLauncher.Core.Helpers
{
    public static class ImageHelper
    {

        public static ObservableCollection<PageBackgrounds> backgroundImage { get; set; } = new ObservableCollection<PageBackgrounds>();
        private static int _imageCurrentSelection = 0;
        private static int _imageLastSelection = 0;
        private static PageImageBrush _images;
        private static SolidColorBrush _backColor = new SolidColorBrush(Colors.Transparent);
        public static Brush GetBackbrush
        {
            get
            {
                if (backgroundImage.Count > 0)
                {
                    return _images;
                }
                return _backColor;
            }
        }
        public static async Task SetBackImage()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                 {
                     if (backgroundImage.Count > 0)
                     {
                         if (_imageCurrentSelection >= ImageHelper.backgroundImage.Count - 1)
                         {
                             _imageCurrentSelection = 0;
                         }
                         else
                         {
                             _imageCurrentSelection += 1;
                         }
                         _images = new PageImageBrush(backgroundImage[_imageCurrentSelection].BackgroundImageBytes.AsBuffer().AsStream().AsRandomAccessStream());
                     }
                     else
                     {
                         SolidColorBrush brushes = new SolidColorBrush(SettingsHelper.totalAppSettings.AppBackgroundColor);
                         _backColor = brushes;
                         brushes = null;
                     }
                 });
            GC.Collect();
        }
        public static void AddPageBackround(PageBackgrounds pageBackgrounds)
        {
            if (backgroundImage.Any(x => x.BackgroundImageDisplayName == pageBackgrounds.BackgroundImageDisplayName))
            {
                return;
            }
            else
            {
                backgroundImage.Add(pageBackgrounds);
            }
        }
        public static void RemovePageBackground(string pageBackgrounds)
        {
            backgroundImage.Remove(x => x.BackgroundImageDisplayName == pageBackgrounds);
        }
        public static async Task LoadBackgroundImages()
        {
            if (!SettingsHelper.totalAppSettings.Images)
            {
                return;
            }
            List<PageBackgrounds> imagesList = new List<PageBackgrounds>();
            if (await IsFilePresent("images.json"))
            {
                try
                {
                    StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("images.json");
                    string apps = await Windows.Storage.FileIO.ReadTextAsync(item);
                    List<PageBackgrounds> images = JsonConvert.DeserializeObject<List<PageBackgrounds>>(apps);
                    backgroundImage = new ObservableCollection<PageBackgrounds>(images);
                }
                catch (Exception e)
                {
                }
            }
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ThreadPoolTimer threadpoolTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
                {
                    await SetBackImage();
                }, SettingsHelper.totalAppSettings.ImageRotationTime);
            });
        }
        public static async Task SaveImageOrder()
        {
            try
            {
                if (backgroundImage.Count() > 0)
                {
                    string imageOrder = JsonConvert.SerializeObject(backgroundImage.ToList(), Formatting.Indented);
                    StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("images.json", CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(item, imageOrder);
                }
            }
            catch (Exception es)
            {
            }
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
        public static async Task<byte[]> ConvertImageFiletoByteArrayAsync(StorageFile fileName)
        {
            using (var inputStream = await fileName.OpenSequentialReadAsync())
            {
                var readStream = inputStream.AsStreamForRead();
                byte[] buffer = new byte[readStream.Length];

                await readStream.ReadAsync(buffer, 0, buffer.Length);
                return buffer;
            }
        }
        public static async Task<IRandomAccessStream> ConvertfromByteArraytoRandomAccessStream(byte[] imageByte)
        {
            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                await stream.WriteAsync(imageByte.AsBuffer());
                stream.Seek(0);
                return stream;
            }
        }
    }
}
