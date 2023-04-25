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

using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace appLauncher.Core.Helpers
{
    public static class ImageHelper
    {

        public static ObservableCollection<PageBackgrounds> backgroundImage { get; set; } = new ObservableCollection<PageBackgrounds>();
        private static int imagecurrentselection = 0;
        private static int imagelastselection = 0;
        private static ImageBrush images;
        private static SolidColorBrush backcolor;
        public static Brush GetBackbrush
        {
            get
            {
                if (backgroundImage.Count > 0)
                {
                    return images;
                }
                return backcolor;

            }
        }


        public static async Task SetBackImage()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                 {
                     if (backgroundImage.Count > 0)
                     {
                         if (imagecurrentselection >= ImageHelper.backgroundImage.Count - 1)
                         {
                             imagecurrentselection = 0;
                         }
                         else
                         {
                             imagecurrentselection += 1;
                         }
                         BitmapImage image = new BitmapImage();
                         using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                         {
                             await stream.WriteAsync(backgroundImage[imagecurrentselection].BackgroundImageBytes.AsBuffer());
                             stream.Seek(0);
                             await image.SetSourceAsync(stream);
                         }
                         images = new ImageBrush();
                         images.ImageSource = image;
                         image = null;
                     }

                     else
                     {
                         SolidColorBrush brushes = new SolidColorBrush(SettingsHelper.totalAppSettings.appBackgroundColor);
                         backcolor = brushes;
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
            List<PageBackgrounds> imageslist = new List<PageBackgrounds>();
            if (await IsFilePresent("images.json"))
            {
                try
                {
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("images.json");
                        string apps = await Windows.Storage.FileIO.ReadTextAsync(item);
                        List<PageBackgrounds> images = JsonConvert.DeserializeObject<List<PageBackgrounds>>(apps);
                        backgroundImage = new ObservableCollection<PageBackgrounds>(images);
                        ThreadPoolTimer threadpoolTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
                        {
                            //
                            // TODO: Work
                            //

                            //
                            // Update the UI thread by using the UI core dispatcher.
                            //

                            await SetBackImage();
                        }, SettingsHelper.totalAppSettings.ImageRotationTime);
                    });





                }
                catch (Exception e)
                {
                    Analytics.TrackEvent("Crashed during loading background images");
                    Crashes.TrackError(e);
                }

            }
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                ThreadPoolTimer threadpoolTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
                {
                    //
                    // TODO: Work
                    //

                    //
                    // Update the UI thread by using the UI core dispatcher.
                    //

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
