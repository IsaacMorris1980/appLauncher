using appLauncher.Core.Model;

using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.Threading;
using Windows.UI.Xaml.Media.Imaging;

namespace appLauncher.Core.Helpers
{
    public static class ImageHelper
    {
        public static BitmapImage _bitmapImage;
        private static int _selectedindex;
        public static ObservableCollection<PageBackgrounds> backgroundImage { get; set; } = new ObservableCollection<PageBackgrounds>();


        public static async Task<List<DisplayImages>> GetDisplayImageAsync()
        {

            List<DisplayImages> allimages = new List<DisplayImages>();
            foreach (PageBackgrounds item in backgroundImage)
            {
                DisplayImages bit = new DisplayImages();
                await bit.displayImage.SetSourceAsync(await ConvertfromByteArraytoRandomAccessStream(item.BackgroundImageBytes));
                bit.displayName = item.BackgroundImageDisplayName;
            }
            return allimages;
        }
        private static async Task RecalculateTheBackgroundItems()
        {

            int currentindex = _selectedindex;
            if (currentindex + 1 > backgroundImage.Count - 1)
            {
                currentindex = 0;
            }
            else
            {
                currentindex += 1;
            }
            BitmapImage bitmap = new BitmapImage();
            await bitmap.SetSourceAsync(await ConvertfromByteArraytoRandomAccessStream(backgroundImage[currentindex].BackgroundImageBytes));
            _bitmapImage = bitmap;
            _selectedindex = currentindex;
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
            List<PageBackgrounds> imageslist = new List<PageBackgrounds>();
            if (await IsFilePresent("images.json"))
            {
                try
                {

                    StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("images.json");
                    string apps = await Windows.Storage.FileIO.ReadTextAsync(item);
                    List<PageBackgrounds> images = JsonConvert.DeserializeObject<List<PageBackgrounds>>(apps);
                    backgroundImage = new ObservableCollection<PageBackgrounds>(images);
                    ThreadPoolTimer imageTimer = ThreadPoolTimer.CreatePeriodicTimer(RecalculateThePageItems, SettingsHelper.totalAppSettings.ImageRotationTime);
                }
                catch (Exception e)
                {
                    Analytics.TrackEvent("Crashed during loading background images");
                    Crashes.TrackError(e);
                }

            }
        }
        private static async void RecalculateThePageItems(ThreadPoolTimer timer)
        {
            await RecalculateTheBackgroundItems();
        }
        public static async Task SaveImageOrder()
        {
            try
            {
                if (backgroundImage.Count() > 0)
                {
                    string imageorder = JsonConvert.SerializeObject(backgroundImage.ToList(), Formatting.Indented);
                    StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("images.json", CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(item, imageorder);
                }
            }
            catch (Exception es)
            {
                Analytics.TrackEvent("Crashed while saving background images");
                Crashes.TrackError(es);
            }




        }


        public static async Task<bool> IsFilePresent(string fileName, string folderpath = "")

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
        public static async Task<byte[]> ConvertImageFiletoByteArrayAsync(StorageFile filename)
        {
            using (var inputStream = await filename.OpenSequentialReadAsync())
            {
                var readStream = inputStream.AsStreamForRead();
                byte[] buffer = new byte[readStream.Length];
                await readStream.ReadAsync(buffer, 0, buffer.Length);
                return buffer;
            }
        }
        public static async Task<IRandomAccessStream> ConvertfromByteArraytoRandomAccessStream(byte[] imagestr)
        {

            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                await stream.WriteAsync(imagestr.AsBuffer());
                stream.Seek(0);
                return stream;
            }
        }

    }
}
