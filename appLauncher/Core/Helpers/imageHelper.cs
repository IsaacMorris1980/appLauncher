using appLauncher.Core.Helpers;

using appLauncher.Core.Model;

using Newtonsoft.Json;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Analytics;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.Storage.Streams;

namespace appLauncher.Core.Helpers
{
    public static class imageHelper
    {
      
        public static ObservableCollection<BackgroundImages> backgroundImage { get; set; } = new ObservableCollection<BackgroundImages>();
        public static async Task LoadBackgroundImages()
        {

            if (await logHelper.IsFilePresent("images.txt"))
            {
                try
                {
                    StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("images.txt");
                    backgroundImage = JsonConvert.DeserializeObject<ObservableCollection<BackgroundImages>>(await FileIO.ReadTextAsync(item));
                }
                catch (Exception e)
                {
                    Crashes.TrackError(e);
                    await logHelper.Log(e.ToString());
                }
            }

        }



        public static async Task SaveImageOrder()
        {
            if (backgroundImage.Count()>0)
            {
                try
                {
                    var imageorder = JsonConvert.SerializeObject(backgroundImage, Formatting.Indented);
                    StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("images.txt", CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(item, imageorder);
                }
                catch (Exception e)
                {

                    Crashes.TrackError(e);
                }

            }
            



        }
        public static async Task<byte[]> ReturnImage(StorageFile filename)
        {
            var logoStream = RandomAccessStreamReference.CreateFromFile(filename);
            IRandomAccessStreamWithContentType whatIWant = await logoStream.OpenReadAsync();
            byte[] temp = new byte[whatIWant.Size];
            using (DataReader read = new DataReader(whatIWant.GetInputStreamAt(0)))
            {
                await read.LoadAsync((uint)whatIWant.Size);
                read.ReadBytes(temp);
            }
            return temp;
        }
        
    }
}
