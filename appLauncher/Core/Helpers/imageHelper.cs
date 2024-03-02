using appLauncher.Core.Brushes;
using appLauncher.Core.Extensions;
using appLauncher.Core.Model;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;


namespace appLauncher.Core.Helpers
{
    public static class ImageHelper
    {

        public static ObservableCollection<PageBackgrounds> backgroundImage { get; set; } = new ObservableCollection<PageBackgrounds>();
        private static int _imageCurrentSelection = 0;
        public static ThreadPoolTimer threadpoolTimer;
        private static int _imageLastSelection = 0;
        private static ImageBrush imaged = new ImageBrush();
        private static PageImageBrush _images;
        public static event EventHandler ImagesRetreived;
        private static Brush _imagesBrush;
        private static SolidColorBrush _backColor = new SolidColorBrush();
        private static BitmapImage _bitmap = new BitmapImage();
        public static Brush GetBackbrush
        {
            get
            {
                return _imagesBrush;
            }
        }
        public static async Task SetBackImage()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                 {

                     if (backgroundImage.Count > 0)
                     {
                         Debug.WriteLine(backgroundImage.Count);
                         _images = new PageImageBrush(backgroundImage[_imageCurrentSelection = (_imageCurrentSelection >= backgroundImage.Count - 1) ? 0 : _imageCurrentSelection + 1].BackgroundImageBytes.AsBuffer().AsStream().AsRandomAccessStream());
                         _imagesBrush = _images;

                     }
                     else
                     {
                         _backColor.Color = SettingsHelper.totalAppSettings.AppBackgroundColor;
                         _imagesBrush = _backColor;
                     }
                 });
            GC.WaitForPendingFinalizers();
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
            List<PageBackgrounds> imagesList = new List<PageBackgrounds>();
            if (await IsFilePresent("images.json"))
            {
                try
                {
                    StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("images.json");
                    string apps = await Windows.Storage.FileIO.ReadTextAsync(item);
                    backgroundImage = new ObservableCollection<PageBackgrounds>(JsonConvert.DeserializeObject<List<PageBackgrounds>>(apps));
                }
                catch (Exception es)
                {


                }


            }
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                threadpoolTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
                {
                    await SetBackImage();
                }, SettingsHelper.totalAppSettings.ImageRotationTime.Subtract(TimeSpan.FromSeconds(2)));
            });
            backgroundImage = new ObservableCollection<PageBackgrounds>(imagesList);
            ImagesRetreived?.Invoke(true, EventArgs.Empty);
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
        //public static IRandomAccessStream GetStreamWithStreamWriter(string sampleString, Encoding encoding = null)
        //{
        //    Encoding encoded = encoding == null ? Encoding.UTF8 : encoding;

        //    MemoryStream stream = new MemoryStream(encoded.GetByteCount(sampleString));
        //    using (StreamWriter writer = new StreamWriter(stream, encoded, 1, true))
        //    {
        //        writer.Write(sampleString);
        //        writer.Flush();
        //        stream.Position = 0;
        //    }

        //    return stream.AsRandomAccessStream();
        //}
        //public static async Task<IRandomAccessStream> ConvertfromByteArraytoRandomAccessStream(byte[] imageByte)
        //{
        //    using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
        //    {
        //        await stream.WriteAsync(imageByte.AsBuffer());
        //        stream.Seek(0);
        //        return stream;
        //    }
        //}
    }
}
