using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using appLauncher.Core.Model;

using Newtonsoft.Json;

using Windows.Storage;

namespace appLauncher.Core.Helpers
{
   public static class settingsHelper
    {
        public static AppSettings appSettings { get; set; } = new AppSettings();
        public static async Task LoadSettings()
        {
            if (await Logging.IsFilePresent("settings.txt"))
            {
                try
                {
                    StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("settings.txt");
                    appSettings = JsonConvert.DeserializeObject<AppSettings>(await FileIO.ReadTextAsync(item));
                }
                catch (Exception e)
                {
                    await Logging.Log(e.ToString());
                }
            }
        }
        public static async Task SaveSettings()
        {
            var a = JsonConvert.SerializeObject(appSettings, Formatting.Indented);
            StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("settings.txt", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(item, a);
        }
    }
}
