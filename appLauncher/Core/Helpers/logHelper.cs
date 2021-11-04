using System;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.UI;

namespace appLauncher.Core.Helpers
{
    public static class logHelper
    {
        public static async Task Log(string texttolog)
        {
            StorageFolder stors = ApplicationData.Current.LocalFolder;
            await FileIO.AppendTextAsync(await stors.CreateFileAsync("logfile.txt", CreationCollisionOption.OpenIfExists), texttolog);
            await FileIO.AppendTextAsync(await stors.CreateFileAsync("logfile.txt", CreationCollisionOption.OpenIfExists), Environment.NewLine);
        }
        public static async Task<bool> IsFilePresent(string fileName)
        {
            var item = await ApplicationData.Current.LocalFolder.TryGetItemAsync(fileName);
            return item != null;
        }
        public static Color FromName(String name)
        {
            var color_props = typeof(Colors).GetProperties();
            foreach (var c in color_props)
            {
                if (name.Equals(c.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return (Color)c.GetValue(new Color(), null);
                }
            }
            return Colors.Transparent;
        }
    }
}
