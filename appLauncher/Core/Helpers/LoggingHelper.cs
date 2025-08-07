using Newtonsoft.Json;

using System;
using System.Threading.Tasks;

using Windows.Storage;

namespace appLauncher.Core.Helpers
{
    public static class LoggingHelper
    {
        public static async Task Log(Exception e)
        {
            string saveappsstring = JsonConvert.SerializeObject(e.ToString(), Formatting.Indented);
            StorageFile appsFile = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("erros.json", CreationCollisionOption.OpenIfExists);
            await FileIO.AppendTextAsync(appsFile, saveappsstring);

        }





    }
}
