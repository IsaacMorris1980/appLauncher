using appLauncher.Core.Interfaces;
using appLauncher.Core.Model;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Storage;

using Windows.UI.Xaml;

namespace appLauncher.Core.Services
{
    public class SettingsService : ISettingsService
    {
        public GlobalAppSettings TotalAppSettings { get; private set; } = new GlobalAppSettings();

        public async Task LoadAppSettingsAsync()
        {
            var item = await ApplicationData.Current.LocalFolder.TryGetItemAsync("globalappsettings.json");
            if (item != null)
            {
                try
                {
                    string apps = await FileIO.ReadTextAsync((StorageFile)item);
                    TotalAppSettings = JsonConvert.DeserializeObject<GlobalAppSettings>(apps);
                }
                catch (Exception)
                {
                    TotalAppSettings = new GlobalAppSettings();
                }
            }
        }

        public async Task SaveAppSettingsAsync()
        {
            try
            {
                string json = JsonConvert.SerializeObject(TotalAppSettings, Formatting.Indented);
                StorageFile item = await ApplicationData.Current.LocalFolder.CreateFileAsync("globalappsettings.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(item, json);
            }
            catch (Exception)
            {
            }
        }

        public void SetApplicationResources()
        {
            Application.Current.Resources["AppBarButtonForegroundPointerOver"] = TotalAppSettings.AppForegroundColorBrush;
            Application.Current.Resources["AppBarButtonBackgroundPointerOver"] = TotalAppSettings.AppBackgroundColorBrush;
            Application.Current.Resources["ComboBoxBackground"] = TotalAppSettings.AppBackgroundColorBrush;
            Application.Current.Resources["ComboBoxBackgroundPointerOver"] = TotalAppSettings.AppBackgroundColorBrush;
            Application.Current.Resources["ComboBoxForeground"] = TotalAppSettings.AppForegroundColorBrush;
            Application.Current.Resources["ComboBoxDropDownBackground"] = TotalAppSettings.AppBackgroundColorBrush;
            Application.Current.Resources["ComboBoxPlaceHolderForeground"] = TotalAppSettings.AppForegroundColorBrush;
        }
    }
}
