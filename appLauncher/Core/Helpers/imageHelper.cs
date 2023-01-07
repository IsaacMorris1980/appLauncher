using appLauncher.Core.Model;

using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace appLauncher.Core.Helpers
{
    public static class ImageHelper
    {
        public static ObservableCollection<PageBackgrounds> backgroundImage { get; set; } = new ObservableCollection<PageBackgrounds>();

        public static async Task LoadBackgroundImages()
        {
            List<PageBackgrounds> imageslist = new List<PageBackgrounds>();
            if (!SettingsHelper.totalAppSettings.ImagesLoaded)
            {



                if (SettingsHelper.totalAppSettings.BgImagesAvailable)
                {
                    try
                    {
                        string folderpath = ApplicationData.Current.LocalFolder.Path;
                        string appfolderpath = Path.Combine(folderpath, "backgroundImage");
                        if (Directory.Exists(appfolderpath))
                        {

                            StorageFolder storageFolder = await StorageFolder.GetFolderFromPathAsync(appfolderpath);
                            List<StorageFile> filesinfolder = (await storageFolder.GetFilesAsync()).ToList();
                            foreach (StorageFile item in filesinfolder)
                            {
                                PageBackgrounds pb = new PageBackgrounds();
                                pb.ImageFullPath = item.Path;
                                pb.BackgroundImageDisplayName = item.DisplayName;
                                byte[] imagebytes = await ConvertImageFiletoByteArrayAsync(item);
                                pb.BackgroundImageBytes = imagebytes;
                                BitmapImage image = new BitmapImage();
                                using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                                {
                                    using (DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0)))
                                    {
                                        writer.WriteBytes(imagebytes);
                                        await writer.StoreAsync();
                                    }

                                    await image.SetSourceAsync(stream);

                                }
                                pb.pageBackgroundDisplayImage = image;
                                backgroundImage.Add(pb);
                            }

                        }

                        //StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("images.txt");
                        //string imagesstring = await FileIO.ReadTextAsync(item);
                        //ObservableCollection<PageBackgrounds> images = JsonConvert.DeserializeObject<ObservableCollection<PageBackgrounds>>(imagesstring);
                        //foreach (var items in images)
                        //{
                        //    StorageFile sf = await StorageFile.GetFileFromPathAsync(items.ImageFullPath);
                        //    items.BackgroundImageBytes = await ConvertImageFiletoByteArrayAsync(sf);
                        //    items.pageBackgroundDisplayImage = await ConvertfromByteArraytoBitmapImage(items.BackgroundImageBytes);
                        //}

                    }
                    catch (Exception e)
                    {
                        Analytics.TrackEvent("Crashed during loading background images");
                        Crashes.TrackError(e);
                    }
                    SettingsHelper.totalAppSettings.ImagesLoaded = true;
                }
            }
        }

        public static async Task SaveImageOrder()
        {
            try
            {
                if (backgroundImage.Count() > 0)
                {
                    string imageorder = JsonConvert.SerializeObject(backgroundImage, Formatting.Indented);
                    StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("images.txt", CreationCollisionOption.ReplaceExisting);
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
        public static async Task<BitmapImage> ConvertfromByteArraytoBitmapImage(byte[] imagestr)
        {

            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                using (DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0)))
                {
                    writer.WriteBytes(imagestr);
                    await writer.StoreAsync();
                }
                BitmapImage image = new BitmapImage();
                await image.SetSourceAsync(stream);
                return image;
            }
        }
    }
}
