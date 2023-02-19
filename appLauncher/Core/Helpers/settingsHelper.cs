using appLauncher.Core.Model;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

using Newtonsoft.Json;

using System;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.UI.Xaml;

namespace appLauncher.Core.Helpers
{
    public static class SettingsHelper
    {
        public static GlobalAppSettings totalAppSettings { get; set; } = new GlobalAppSettings();
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

        public static async Task LoadAppSettingsAsync()
        {
            if (await SettingsHelper.IsFilePresent("GlobalAppSettings.txt"))
            {
                try
                {
                    StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("GlobalAppSettings.txt");
                    string apps = await Windows.Storage.FileIO.ReadTextAsync(item);
                    GlobalAppSettings appsettings = JsonConvert.DeserializeObject<GlobalAppSettings>(apps);
                    totalAppSettings = appsettings;
                }
                catch (Exception e)
                {
                    Analytics.TrackEvent("Crashed during reordering app list to last apps position");
                    Crashes.TrackError(e);
                }
            }
            else
            {
                GlobalAppSettings appSettings = new GlobalAppSettings();
                totalAppSettings = appSettings;
            }



        }
        public static async Task SaveAppSettingsAsync()
        {
            try
            {
                var te = JsonConvert.SerializeObject(totalAppSettings, Formatting.Indented);
                StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("GlobalAppSettings.txt", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(item, te);
            }
            catch (Exception es)
            {
                Analytics.TrackEvent("Crashed during saving Global App Settings");
                Crashes.TrackError(es);
            }
        }

        public static void ConfigureAppCenter()
        {
            AppCenter.Start("f3879d12-8020-4309-9fbf-71d9d24bcf9b", typeof(Crashes), typeof(Analytics));
            Crashes.SetEnabledAsync(false);
            Analytics.SetEnabledAsync(false);
        }
        public static async Task CheckAppSettings()
        {
            if (!AppCenter.Configured)
            {
                ConfigureAppCenter();

            }
            if (!SettingsHelper.totalAppSettings.disableCrashReporting)
            {
                await Crashes.SetEnabledAsync(true);
            }
            if (!SettingsHelper.totalAppSettings.disableAnalytics)
            {
                await Analytics.SetEnabledAsync(true);
            }
            if (SettingsHelper.totalAppSettings.disableCrashReporting)
            {
                await Crashes.SetEnabledAsync(false);
            }
            if (SettingsHelper.totalAppSettings.disableAnalytics)
            {
                await Analytics.SetEnabledAsync(false);
            }
        }
        public static void SetApplicationResources()
        {
            Application.Current.Resources["AppBarButtonForegroundPointerOver"] = SettingsHelper.totalAppSettings.AppForegroundColorBrush;
            Application.Current.Resources["AppBarButtonBackgroundPointerOver"] = SettingsHelper.totalAppSettings.AppBackgroundColorBrush;
            Application.Current.Resources["ComboBoxBackground"] = SettingsHelper.totalAppSettings.AppBackgroundColorBrush;
            Application.Current.Resources["ComboBoxBackgroundPointerOver"] = SettingsHelper.totalAppSettings.AppBackgroundColorBrush;
            Application.Current.Resources["ComboBoxForeground"] = SettingsHelper.totalAppSettings.AppForegroundColorBrush;
            Application.Current.Resources["ComboBoxDropDownBackground"] = SettingsHelper.totalAppSettings.AppBackgroundColorBrush;
            Application.Current.Resources["ComboBoxPlaceHolderForeground"] = SettingsHelper.totalAppSettings.AppForegroundColorBrush;
        }

    }
}
