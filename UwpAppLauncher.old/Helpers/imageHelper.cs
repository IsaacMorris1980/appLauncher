using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using UwpAppLauncher.Model;

using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;

namespace UwpAppLauncher.Helpers
{
    public sealed class imageHelper
    {
        public ObservableCollection<PageBackground> backgroundImage { get; set; } = new ObservableCollection<PageBackground>();
        public async Task LoadBackgroundImages()
        {
            List<PageBackground> imageslist = new List<PageBackground>();

            if (await ((App)Application.Current).globalVariables.IsFilePresent("images.json"))
            {
                try
                {
                    string folderpath = ApplicationData.Current.LocalFolder.Path;

                    StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("images.json");
                    string imagesstring = await FileIO.ReadTextAsync(item);
                    ObservableCollection<PageBackground> images = JsonConvert.DeserializeObject<ObservableCollection<PageBackground>>(imagesstring);
                    foreach (var items in images)
                    {
                        StorageFile sf = await StorageFile.GetFileFromPathAsync(items.PageBackgroundPath);
                        items.PageBackgroundImage = await ConvertImageFiletoByteArrayAsync(sf);
                        items.PageBackgroundName = sf.DisplayName;
                        items.PageBackgroundPath = sf.Path;
                        backgroundImage.Add(items);
                    }

                }
                catch (Exception e)
                {
                    Analytics.TrackEvent("Crashed during loading background images");
                    Crashes.TrackError(e);
                }
            }
        }

        public async Task SaveImageOrder()
        {

            try
            {
                if (backgroundImage.Count > 0)
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
        public async Task<byte[]> ConvertImageFiletoByteArrayAsync(StorageFile filename)
        {

            using (IInputStream inputStream = await filename.OpenSequentialReadAsync())
            {
                Stream readStream = inputStream.AsStreamForRead();
                byte[] buffer = new byte[readStream.Length];
                await readStream.ReadAsync(buffer, 0, buffer.Length);
                return buffer;
            }
        }
        public async Task<byte[]> ExtractLogo(AppListEntry entry)
        {
            RandomAccessStreamReference logoStream;
            byte[] temp = new byte[1];
            try
            {
                logoStream = entry.DisplayInfo.GetLogo(new Size(50, 50));
                IRandomAccessStreamWithContentType whatIWant = await logoStream.OpenReadAsync();
                temp = new byte[whatIWant.Size];
                using (DataReader read = new DataReader(whatIWant.GetInputStreamAt(0)))
                {
                    await read.LoadAsync((uint)whatIWant.Size);
                    read.ReadBytes(temp);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return new byte[1];
            }
            return temp;
        }
        public async Task<byte[]> ResizeImage(byte[] imagefile, int reqWidth, int reqHeight)
        {
            //open file as stream
            using (IRandomAccessStream fileStream = imagefile.AsBuffer().AsStream().AsRandomAccessStream())
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fileStream);

                InMemoryRandomAccessStream resizedStream = new InMemoryRandomAccessStream();

                BitmapEncoder encoder = await BitmapEncoder.CreateForTranscodingAsync(resizedStream, decoder);
                double widthRatio = (double)reqWidth / decoder.PixelWidth;
                double heightRatio = (double)reqHeight / decoder.PixelHeight;

                double scaleRatio = Math.Min(widthRatio, heightRatio);

                if (reqWidth == 0)
                {
                    scaleRatio = heightRatio;
                }

                if (reqHeight == 0)
                {
                    scaleRatio = widthRatio;
                }

                uint aspectHeight = (uint)Math.Floor(decoder.PixelHeight * scaleRatio);
                uint aspectWidth = (uint)Math.Floor(decoder.PixelWidth * scaleRatio);

                encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Linear;

                encoder.BitmapTransform.ScaledHeight = aspectHeight;
                encoder.BitmapTransform.ScaledWidth = aspectWidth;

                await encoder.FlushAsync();
                resizedStream.Seek(0);
                byte[] outBuffer = new byte[resizedStream.Size];
                await resizedStream.ReadAsync(outBuffer.AsBuffer(), (uint)resizedStream.Size, InputStreamOptions.None);
                return outBuffer;
            }
        }
    }
}
